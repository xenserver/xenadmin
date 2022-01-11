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
using System.Diagnostics;
using System.Security.Cryptography;
using XenCenterLib;
using System.Text;

namespace XenAdmin.Actions
{
    public class DownloadAndUpdateClientAction : AsyncAction, IByteProgressAction
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS = 900;
        private const int SLEEP_TIME_BEFORE_RETRY_MS = 5000;

        //If you consider increasing this for any reason (I think 5 is already more than enough), have a look at the usage of SLEEP_TIME_BEFORE_RETRY_MS in DownloadFile() as well.
        private const int MAX_NUMBER_OF_TRIES = 5;

        private readonly Uri address;
        private readonly string outputPathAndFileName;
        private readonly string updateName;
        private readonly bool downloadUpdate;
        private DownloadState updateDownloadState;
        private Exception updateDownloadError;
        private string checksum; 

        public string PatchPath { get; private set; }

        public string ByteProgressDescription { get; set; }

        public DownloadAndUpdateClientAction(string updateName, Uri uri, string outputFileName, bool suppressHist, string checksum, params string[] updateFileExtensions)
            : base(null, uri == null
                ? string.Format(Messages.UPDATES_WIZARD_EXTRACT_ACTION_TITLE, updateName)
                : string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_TITLE, updateName), string.Empty, suppressHist)
        {
            this.updateName = updateName;
            address = uri;
            downloadUpdate = address != null;
            this.outputPathAndFileName = outputFileName;
            this.checksum = checksum;
        }

        private WebClient client;

        private void DownloadFile()
        {
            int errorCount = 0;
            bool needToRetry = false;

            client = new WebClient();
            //register download events
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;

            // register event handler to detect changes in network connectivity
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;

            try
            {
                do
                {
                    if (needToRetry)
                        Thread.Sleep(SLEEP_TIME_BEFORE_RETRY_MS);

                    needToRetry = false;

                    client.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

                    //start the download
                    updateDownloadState = DownloadState.InProgress;
                    client.DownloadFileAsync(address, outputPathAndFileName);

                    bool updateDownloadCancelling = false;

                    //wait for the file to be downloaded
                    while (updateDownloadState == DownloadState.InProgress)
                    {
                        if (!updateDownloadCancelling && (Cancelling || Cancelled))
                        {
                            Description = Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOAD_CANCELLED_DESC;
                            client.CancelAsync();
                            updateDownloadCancelling = true;
                        }

                        Thread.Sleep(SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS);
                    }

                    if (updateDownloadState == DownloadState.Cancelled)
                        throw new CancelledException();

                    if (updateDownloadState == DownloadState.Error)
                    {
                        needToRetry = true;

                        // this many errors so far - including this one
                        errorCount++;

                        // logging only, it will retry again.
                        log.ErrorFormat(
                            "Error while downloading from '{0}'. Number of errors so far (including this): {1}. Trying maximum {2} times.",
                            address, errorCount, MAX_NUMBER_OF_TRIES);

                        if (updateDownloadError == null)
                            log.Error("An unknown error occurred.");
                        else
                            log.Error(updateDownloadError);
                    }
                } while (errorCount < MAX_NUMBER_OF_TRIES && needToRetry);
            }
            finally
            {
                //deregister download events
                client.DownloadProgressChanged -= client_DownloadProgressChanged;
                client.DownloadFileCompleted -= client_DownloadFileCompleted;

                NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;

                client.Dispose();
            }

            //if this is still the case after having retried MAX_NUMBER_OF_TRIES number of times.
            if (updateDownloadState == DownloadState.Error)
            {
                log.ErrorFormat("Giving up - Maximum number of retries ({0}) has been reached.", MAX_NUMBER_OF_TRIES);

                MarkCompleted(updateDownloadError ?? new Exception(Messages.ERROR_UNKNOWN));
            }

        }

        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable && client != null && updateDownloadState == DownloadState.InProgress)
            {
                updateDownloadError = new WebException(Messages.NETWORK_CONNECTIVITY_ERROR);
                updateDownloadState = DownloadState.Error;
                client.CancelAsync();
            }
        }

        protected override void Run()
        {
            if (downloadUpdate)
            {
                log.InfoFormat("Downloading update '{0}' (from '{1}') to '{2}'", updateName, address, outputPathAndFileName);
                Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DESC, updateName);
                LogDescriptionChanges = false;
                DownloadFile();
                LogDescriptionChanges = true;

                if (IsCompleted || Cancelled)
                    return;

                if (Cancelling)
                    throw new CancelledException();
            }
            if (ValidMsi())
            {
                // Install the downloaded msi            
                try
                {
                    // Start the install process and end current 
                    if (File.Exists(outputPathAndFileName))
                    {
                        // Launch downloaded msi
                        Process.Start(outputPathAndFileName);
                        log.DebugFormat("Update {0} found and install started", updateName);
                    }
                }
                catch (Exception e)
                {
                    if (File.Exists(outputPathAndFileName))
                    {
                        File.Delete(outputPathAndFileName);
                    }
                    log.Error("Exception occurred when installing CHC.", e);
                    throw;
                }
            }

            Description = Messages.COMPLETED;
            MarkCompleted();
        }

        private bool ValidMsi()
        {
            using (FileStream stream = new FileStream(outputPathAndFileName, FileMode.Open, FileAccess.Read))
            {
                var hash = StreamUtilities.ComputeHash(stream, out var hashAlgorithm);
                //Convert to Hex string
                StringBuilder sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                var calculatedChecksum = sb.ToString();
                // Check if calculatedChecksum matches what is in chcupdates.xml
                if (checksum != calculatedChecksum)
                {
                    return false;
                }
            }

            return true;

            // TODO: Check digital signature of .msi

        }
        // Display the byte array in a readable format.
        public static void PrintByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write($"{array[i]:X2}");
                if ((i % 4) == 3) Console.Write(" ");
            }
            Console.WriteLine();
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int pc = (int)(95.0 * e.BytesReceived / e.TotalBytesToReceive);
            var descr = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DETAILS_DESC, updateName,
                                            Util.DiskSizeString(e.BytesReceived, "F1"),
                                            Util.DiskSizeString(e.TotalBytesToReceive));
            ByteProgressDescription = descr;
            Tick(pc, descr);
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled && updateDownloadState == DownloadState.Error) // cancelled due to network connectivity issue (see NetworkAvailabilityChanged)
                return;

            if (e.Cancelled) //user cancelled
            {
                updateDownloadState = DownloadState.Cancelled;
                log.DebugFormat("XenServer patch '{0}' download cancelled by the user", updateName);
                return;
            }

            if (e.Error != null) //failure
            {
                updateDownloadError = e.Error;
                log.DebugFormat("XenServer patch '{0}' download failed", updateName);
                updateDownloadState = DownloadState.Error;
                return;
            }

            //success
            updateDownloadState = DownloadState.Completed;
            log.DebugFormat("XenServer patch '{0}' download completed successfully", updateName);
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling && !IsCompleted && (updateDownloadState == DownloadState.InProgress);
        }

    }
}

