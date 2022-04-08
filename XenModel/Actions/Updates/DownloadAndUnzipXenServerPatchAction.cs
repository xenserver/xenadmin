/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Net;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using XenAdmin.Core;
using XenCenterLib.Archive;

namespace XenAdmin.Actions.Updates
{
    internal enum DownloadState
    {
        InProgress,
        Cancelled,
        Completed,
        Error
    }


    public abstract class DownloadAndUnzipXenServerPatchActionBase : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly string updateName;
        protected readonly string[] updateFileSuffixes;

        public string PatchPath { get; protected set; }

        public string ByteProgressDescription { get; set; }

        protected DownloadAndUnzipXenServerPatchActionBase(string patchName, params string[] updateFileExtensions)
            : base(null, string.Empty, string.Empty, true) 
        {
            updateName = patchName;
            updateFileSuffixes = (from item in updateFileExtensions select $".{item}").ToArray();
        }

        protected string ExtractFile(string zippedFilePath, bool deleteOriginal)
        {
            log.DebugFormat("Extracting XenServer patch '{0}'", updateName);
            Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_EXTRACTING_DESC, updateName);

            DotNetZipZipIterator iterator = null;

            try
            {
                using (Stream stream = new FileStream(zippedFilePath, FileMode.Open, FileAccess.Read))
                {
                    iterator = ArchiveFactory.Reader(ArchiveFactory.Type.Zip, stream) as DotNetZipZipIterator;
                    if (iterator == null)
                        return null;
                    
                    iterator.CurrentFileExtractProgressChanged += archiveIterator_CurrentFileExtractProgressChanged;

                    log.InfoFormat("Looking in the archive for a file with extension {0}...",
                        string.Join(" or ", updateFileSuffixes));
                    
                    while (iterator.HasNext())
                    {
                        string currentExtension = Path.GetExtension(iterator.CurrentFileName());

                        if (updateFileSuffixes.Any(item => item.ToLowerInvariant() == currentExtension.ToLowerInvariant()))
                        {
                            log.InfoFormat("Found file '{0}'. Extracting...", iterator.CurrentFileName());

                            string path = Path.Combine(Path.GetTempPath(), iterator.CurrentFileName());

                            using (Stream outputStream = new FileStream(path, FileMode.Create))
                            {
                                iterator.ExtractCurrentFile(outputStream, null);
                                log.InfoFormat("Update file extracted to '{0}'", path);
                                return path;
                            }
                        }
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                log.Error("Exception occurred when extracting downloaded archive.", e);
                throw new Exception(Messages.DOWNLOAD_AND_EXTRACT_ACTION_EXTRACTING_ERROR);
            }
            finally
            {
                if (iterator != null)
                {
                    iterator.CurrentFileExtractProgressChanged -= archiveIterator_CurrentFileExtractProgressChanged;
                    iterator.Dispose();
                }

                if (deleteOriginal)
                {
                    try
                    {
                        File.Delete(zippedFilePath);
                    }
                    catch
                    {
                        //ignore
                    }
                }

                log.DebugFormat("Extracting XenServer patch '{0}' and cleaning up completed", updateName);
            }
        }

        private void archiveIterator_CurrentFileExtractProgressChanged(long bytesTransferred, long totalBytesToTransfer)
        {
            UpdateExtractionProgress(bytesTransferred, totalBytesToTransfer);
        }

        protected override void CancelRelatedTask()
        {
        }

        protected abstract void UpdateExtractionProgress(long bytesTransferred, long totalBytesToTransfer);
    }


    public class DownloadAndUnzipXenServerPatchAction : DownloadAndUnzipXenServerPatchActionBase, IByteProgressAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS = 900;
        private const int SLEEP_TIME_BEFORE_RETRY_MS = 5000;

        //If you consider increasing this for any reason (I think 5 is already more than enough),
        //have a look at the usage of SLEEP_TIME_BEFORE_RETRY_MS in DownloadFile() as well.
        private const int MAX_NUMBER_OF_TRIES = 5;

        private readonly bool skipUnzipping;
        private readonly Uri _patchUri;
        private DownloadState patchDownloadState;
        private Exception patchDownloadError;
        private WebClient client;


        public DownloadAndUnzipXenServerPatchAction(string patchName, Uri uri, params string[] updateFileExtensions)
            : base(patchName, updateFileExtensions)
        {
            Title = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_TITLE, patchName);
            _patchUri = uri;
            skipUnzipping = updateFileSuffixes.Any(item => _patchUri.ToString().ToLowerInvariant().EndsWith(item.ToLowerInvariant()));
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling && !IsCompleted && (patchDownloadState == DownloadState.InProgress);
        }

        protected override void Run()
        {
            DownloadFile(out string localPath);

            if (IsCompleted || Cancelled)
                return;

            if (Cancelling)
                throw new CancelledException();

            if (skipUnzipping)
            {
                log.Debug("Moving archive to final destination");
                string newLocalePath = Path.Combine(Path.GetDirectoryName(localPath), updateName);
                if (File.Exists(newLocalePath))
                    File.Delete(newLocalePath);
                File.Move(localPath, newLocalePath);
                PatchPath = newLocalePath;
                log.DebugFormat("XenServer patch '{0}' is ready", updateName);
            }
            else
            {
                PatchPath = ExtractFile(localPath, true);
            }

            if (string.IsNullOrEmpty(PatchPath))
            {
                log.ErrorFormat("The downloaded archive does not contain a file with any of the following extensions: {0}",
                    string.Join(", ", updateFileSuffixes));
                throw new Exception(Messages.DOWNLOAD_AND_EXTRACT_ACTION_FILE_NOT_FOUND);
            }

            Description = Messages.COMPLETED;
        }

        protected override void UpdateExtractionProgress(long bytesTransferred, long totalBytesToTransfer)
        {
            PercentComplete = 95 + (int)(5.0 * bytesTransferred / totalBytesToTransfer);
        }

        private void DownloadFile(out string outputFileName)
        {
            client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;

            //useful when the updates use test locations
            if (DownloadUpdatesXmlAction.IsFileServiceUri(_patchUri))
            {
                log.InfoFormat("Authenticating account...");
                Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_AUTHENTICATING_DESC,
                    BrandManager.COMPANY_NAME_SHORT);
                var credential = TokenManager.GetDownloadCredential(XenAdminConfigManager.Provider);
                client.Headers.Add("Authorization", $"Basic {credential}");
            }

            outputFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            
            log.InfoFormat("Downloading update '{0}' (from '{1}') to '{2}'", updateName, _patchUri, outputFileName);
            Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DESC, updateName);
            LogDescriptionChanges = false;

            int failedTries = 0;
            bool needToRetry = false;

            try
            {
                do
                {
                    if (Cancelling)
                        throw new CancelledException();

                    if (needToRetry)
                        Thread.Sleep(SLEEP_TIME_BEFORE_RETRY_MS);

                    needToRetry = false;

                    client.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

                    patchDownloadState = DownloadState.InProgress;
                    client.DownloadFileAsync(_patchUri, outputFileName);

                    bool patchDownloadCancelling = false;

                    //wait for the file to be downloaded
                    while (patchDownloadState == DownloadState.InProgress)
                    {
                        if (!patchDownloadCancelling && (Cancelling || Cancelled))
                        {
                            Description = Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOAD_CANCELLED_DESC;
                            client.CancelAsync();
                            patchDownloadCancelling = true;
                        }

                        Thread.Sleep(SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS);
                    }

                    if (patchDownloadState == DownloadState.Cancelled)
                        throw new CancelledException();

                    if (patchDownloadState == DownloadState.Error)
                    {
                        needToRetry = true;
                        failedTries++;

                        log.ErrorFormat("Failed to download '{0}' (attempt {1} of maximum {2}).",
                            _patchUri, failedTries, MAX_NUMBER_OF_TRIES);

                        if (failedTries < MAX_NUMBER_OF_TRIES)
                        {
                            if (patchDownloadError == null)
                                log.Error("An unknown error occurred.");
                            else
                                log.Error(patchDownloadError);
                        }
                        else
                        {
                            if (patchDownloadError == null)
                                throw new Exception(Messages.ERROR_UNKNOWN);
                            else
                                throw patchDownloadError;
                        }
                    }
                } while (needToRetry);
            }
            finally
            {
                LogDescriptionChanges = true;

                client.DownloadProgressChanged -= client_DownloadProgressChanged;
                client.DownloadFileCompleted -= client_DownloadFileCompleted;
                NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;

                client.Dispose();
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int pc = (int)(95.0 * e.BytesReceived / e.TotalBytesToReceive);
            var descr = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DETAILS_DESC, updateName,
                                            Util.DiskSizeString(e.BytesReceived, "F1"),
                                            Util.DiskSizeString(e.TotalBytesToReceive));
            ByteProgressDescription = descr;
            Tick(pc, descr);
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled && patchDownloadState == DownloadState.Error) // cancelled due to network connectivity issue (see NetworkAvailabilityChanged)
                return;

            if (e.Cancelled) //user cancelled
            {
                patchDownloadState = DownloadState.Cancelled;
                log.DebugFormat("XenServer patch '{0}' download cancelled by the user", updateName);
                return;
            }

            if (e.Error != null) //failure
            {
                if (e.Error is WebException wex && wex.Response is HttpWebResponse response)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            log.Error($"Could not download {updateName} (401 Unauthorized). Client ID may be invalid or revoked.");
                            patchDownloadError = new Exception(Messages.FILESERVICE_ERROR_401);
                            break;

                        case HttpStatusCode.Forbidden:
                            log.Error($"Could not download {updateName} (403 Forbidden). The account has insufficient permissions.");
                            patchDownloadError = new Exception(Messages.FILESERVICE_ERROR_403);
                            break;

                        case HttpStatusCode.NotFound:
                            log.Error($"Could not download {updateName} (404 File not found).");
                            patchDownloadError = new Exception(Messages.FILESERVICE_ERROR_404);
                            break;

                        default:
                            patchDownloadError = e.Error;
                            break;
                    }
                }
                else
                {
                    patchDownloadError = e.Error;
                }
                
                log.DebugFormat("XenServer patch '{0}' download failed", updateName);
                patchDownloadState = DownloadState.Error;
                return;
            }

            //success
            patchDownloadState = DownloadState.Completed;
            log.DebugFormat("XenServer patch '{0}' download completed successfully", updateName);
        }

        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable && client != null && patchDownloadState == DownloadState.InProgress)
            {
                patchDownloadError = new WebException(Messages.NETWORK_CONNECTIVITY_ERROR);
                patchDownloadState = DownloadState.Error;
                client.CancelAsync();
            }
        }
    }


    public class UnzipXenServerPatchAction : DownloadAndUnzipXenServerPatchActionBase
    {
        private readonly string _zippedUpdatePath;

        public UnzipXenServerPatchAction(string zippedUpdatePath, params string[] updateFileExtensions)
            : base(Path.GetFileNameWithoutExtension(zippedUpdatePath), updateFileExtensions)
        {
            _zippedUpdatePath = zippedUpdatePath;
            Title = string.Format(Messages.UPDATES_WIZARD_EXTRACT_ACTION_TITLE, Path.GetFileNameWithoutExtension(zippedUpdatePath));
        }

        protected override void Run()
        {
            PatchPath = ExtractFile(_zippedUpdatePath, false);
            Description = Messages.COMPLETED;
        }

        protected override void UpdateExtractionProgress(long bytesTransferred, long totalBytesToTransfer)
        {
            PercentComplete = (int)(100.0 * bytesTransferred / totalBytesToTransfer);
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling && !IsCompleted;
        }
    }
}
