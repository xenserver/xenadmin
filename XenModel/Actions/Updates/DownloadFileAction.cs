/* Copyright (c) Cloud Software Group, Inc. 
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using XenAdmin.Core;
using XenCenterLib;

namespace XenAdmin.Actions.Updates
{
    public class DownloadFileAction : AsyncAction, IByteProgressAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS = 900;
        private const int SLEEP_TIME_BEFORE_RETRY_MS = 5000;

        //If you consider increasing this for any reason (I think 5 is already more than enough), have a look at the usage of SLEEP_TIME_BEFORE_RETRY_MS in DownloadFile() as well.
        private const int MAX_NUMBER_OF_TRIES = 5;

        protected Uri Address => _address;
        protected string OutputPathAndFileName => _outputPathAndFileName;
        protected string FileName => _fileName;
        protected bool CanDownloadFile => _canDownloadFile;
       
        private readonly Uri _address;
        private readonly string _outputPathAndFileName;
        private readonly string _fileName;
        private readonly bool _canDownloadFile;
        private DownloadState _fileState;
        private Exception _downloadError;
        private readonly string _authToken;
        private WebClient _client;


        public string ByteProgressDescription { get; set; }

        

        public DownloadFileAction(string fileName, Uri uri, string outputFileName, string title, bool suppressHistory)
            : base(null, title, fileName, suppressHistory)
        {
            _fileName = fileName;
            _address = uri;
            _canDownloadFile = _address != null;
            _outputPathAndFileName = outputFileName;
            _authToken = XenAdminConfigManager.Provider.GetClientUpdatesQueryParam();
            Title = string.Format(Messages.DOWNLOADING_FILE, fileName);
            Description = Title;
        }

        protected void DownloadFile()
        {
            int errorCount = 0;
            bool needToRetry = false;

            _client = new WebClient();
            _client.DownloadProgressChanged += client_DownloadProgressChanged;
            _client.DownloadFileCompleted += client_DownloadFileCompleted;
            _client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

            // register event handler to detect changes in network connectivity
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;

            try
            {
                do
                {
                    if (Cancelling)
                        throw new CancelledException();

                    if (needToRetry)
                        Thread.Sleep(SLEEP_TIME_BEFORE_RETRY_MS);

                    needToRetry = false;

                    _client.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

                    //start the download
                    _fileState = DownloadState.InProgress;

                    var uriBuilder = new UriBuilder(_address);

                    if (!string.IsNullOrEmpty(_authToken))
                    {
                        var uri = uriBuilder.Uri;
                        if (!uri.IsFile)
                        {
                            uriBuilder.Query = Helpers.AddAuthTokenToQueryString(_authToken, uriBuilder.Query);
                        }
                    }
                    _client.DownloadFileAsync(uriBuilder.Uri, _outputPathAndFileName);

                    bool updateDownloadCancelling = false;

                    //wait for the file to be downloaded
                    while (_fileState == DownloadState.InProgress)
                    {
                        if (!updateDownloadCancelling && (Cancelling || Cancelled))
                        {
                            Description = Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOAD_CANCELLED_DESC;
                            _client.CancelAsync();
                            updateDownloadCancelling = true;
                        }

                        Thread.Sleep(SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS);
                    }

                    if (_fileState == DownloadState.Cancelled)
                        throw new CancelledException();

                    if (_fileState == DownloadState.Error)
                    {
                        needToRetry = true;

                        // this many errors so far - including this one
                        errorCount++;

                        // logging only, it will retry again.
                        log.ErrorFormat(
                            "Error while downloading from '{0}'. Number of errors so far (including this): {1}. Trying maximum {2} times.",
                            _address, errorCount, MAX_NUMBER_OF_TRIES);

                        if (_downloadError == null)
                            log.Error("An unknown error occurred.");
                        else
                            log.Error(_downloadError);
                    }
                } while (errorCount < MAX_NUMBER_OF_TRIES && needToRetry);
            }
            finally
            {
                _client.DownloadProgressChanged -= client_DownloadProgressChanged;
                _client.DownloadFileCompleted -= client_DownloadFileCompleted;

                NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;

                _client.Dispose();
            }

            //if this is still the case after having retried MAX_NUMBER_OF_TRIES number of times.
            if (_fileState == DownloadState.Error)
            {
                log.ErrorFormat("Giving up - Maximum number of retries ({0}) has been reached.", MAX_NUMBER_OF_TRIES);
                throw _downloadError ?? new Exception(Messages.ERROR_UNKNOWN);
            }
        }

        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable && _client != null && _fileState == DownloadState.InProgress)
            {
                _downloadError = new WebException(Messages.NETWORK_CONNECTIVITY_ERROR);
                _fileState = DownloadState.Error;
                _client.CancelAsync();
            }
        }

        protected override void Run()
        {
            if (!_canDownloadFile)
                return;

            log.InfoFormat("Downloading '{0}' (from '{1}') to '{2}'", FileName, _address, _outputPathAndFileName); 
            LogDescriptionChanges = false;
            DownloadFile();
            LogDescriptionChanges = true;

            if (IsCompleted || Cancelled)
                return;

            if (Cancelling)
                throw new CancelledException();

            if (!File.Exists(_outputPathAndFileName))
                throw new Exception(Messages.DOWNLOAD_FILE_NOT_FOUND);

            Description = Messages.COMPLETED;
        }

        protected override void CleanOnError()
        {
            ReleaseDownloadedContent(true);
        }

        public void ReleaseDownloadedContent(bool deleteDownloadedContent = false)
        {
            if (!deleteDownloadedContent)
                return;

            try
            {
                if (File.Exists(_outputPathAndFileName))
                    File.Delete(_outputPathAndFileName);
            }
            catch
            {
                //ignore
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int pc = (int)(95.0 * e.BytesReceived / e.TotalBytesToReceive);
            
            var descr = string.Format(Messages.DOWNLOAD_FILE_ACTION_PROGRESS_DESCRIPTION, FileName, 
                                            Util.DiskSizeString(e.BytesReceived, "F1"),
                                            Util.DiskSizeString(e.TotalBytesToReceive));
            ByteProgressDescription = descr;
            Tick(pc, descr);
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled && _fileState == DownloadState.Error) // cancelled due to network connectivity issue (see NetworkAvailabilityChanged)
                return;

            if (e.Cancelled)
            {
                _fileState = DownloadState.Cancelled;
                log.DebugFormat("'{0}' download cancelled by the user", FileName);
                return;
            }

            if (e.Error != null)
            {
                _downloadError = e.Error;
                log.DebugFormat("'{0}' download failed", FileName);
                _fileState = DownloadState.Error;
                return;
            }

            _fileState = DownloadState.Completed;
            log.DebugFormat("'{0}' download completed successfully", FileName);
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling && !IsCompleted && _fileState == DownloadState.InProgress;
        }
    }

    public class DownloadAndUpdateClientAction : DownloadFileAction, IByteProgressAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _checksum;
        private FileStream _msiStream;

        public DownloadAndUpdateClientAction(string updateName, Uri uri, string outputFileName, string checksum)
            : base(updateName,
                  uri,
                  outputFileName,
                  string.Join("", updateName, ".msi"),
                  true)
        {
            _checksum = checksum;
        }

        protected override void Run()
        {
            if (!CanDownloadFile)
                return;

            log.InfoFormat("Downloading '{0}' installer (from '{1}') to '{2}'", FileName, Address, OutputPathAndFileName);
            Description = string.Format(Messages.DOWNLOADING_FILE, FileName);
            LogDescriptionChanges = false;
            DownloadFile();
            LogDescriptionChanges = true;

            if (IsCompleted || Cancelled)
                return;

            if (Cancelling)
                throw new CancelledException();

            if (!File.Exists(OutputPathAndFileName))
                throw new Exception(Messages.DOWNLOAD_CLIENT_INSTALLER_MSI_NOT_FOUND);

            ValidateMsi();

            Description = Messages.COMPLETED;
        }



        private void ValidateMsi()
        {
            Description = Messages.UPDATE_CLIENT_VALIDATING_INSTALLER;

            _msiStream = new FileStream(OutputPathAndFileName, FileMode.Open, FileAccess.Read);

            var calculatedChecksum = string.Empty;

            var hash = StreamUtilities.ComputeHash(_msiStream, out _);
            if (hash != null)
                calculatedChecksum = string.Join(string.Empty, hash.Select(b => $"{b:x2}"));

            // Check if calculatedChecksum matches what is in xcupdates.xml
            if (!_checksum.Equals(calculatedChecksum, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception(Messages.UPDATE_CLIENT_INVALID_CHECKSUM);

            bool valid;
            try
            {
                // Check digital signature of .msi
                using (var basicSigner = X509Certificate.CreateFromSignedFile(OutputPathAndFileName))
                {
                    using (var cert = new X509Certificate2(basicSigner))
                        valid = cert.Verify();
                }
            }
            catch (Exception e)
            {
                throw new Exception(Messages.UPDATE_CLIENT_FAILED_CERTIFICATE_CHECK, e);
            }

            if (!valid)
                throw new Exception(Messages.UPDATE_CLIENT_INVALID_DIGITAL_CERTIFICATE);
        }

        public new void ReleaseDownloadedContent(bool deleteDownloadedContent = false)
        {
            _msiStream.Dispose();

            if (!deleteDownloadedContent)
                return;

            try
            {
                if (File.Exists(OutputPathAndFileName))
                    File.Delete(OutputPathAndFileName);
            }
            catch
            {
                //ignore
            }
        }
    }

    public class DownloadSourceAction : DownloadFileAction, IByteProgressAction
    {
        public DownloadSourceAction(string sourceName, Version version, Uri uri, string outputFileName)
            : base(String.Format(Messages.SOURCE_FILE_NAME, sourceName, version),
                  uri,
                  outputFileName,
                  string.Format(Messages.DOWNLOADING_FILE, version.ToString() + " " + outputFileName),
                  false)
        {

        }
    }


}

