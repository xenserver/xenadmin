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

using System.Threading;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Action to Save the downloaded Server Status Report to local disk and remote CIS server
    /// </summary>
    public class SaveServerStatusReportAction : AsyncAction
    { 
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Parameters
        private SaveReportInfo info = null;

        /// lock to sync Actions
        private object syncFinishMonitorLock = new object(); 

        // Two sub actions
        ZipStatusReportAction zipAction = null;
        UploadServerStatusReportAction uploadAction = null;


        public class SaveReportInfo
        {
            public string TempFolder { get; set; } // Temp folder containing the downloaded report
            public string DestFile { get; set; } // The destination of the local disk file to save the report
            public string UploadToken { get; set; } // Token to upload the report
            public string CaseId { get; set; } // Optional case ID
            public string DomainName { get; set; } // domain name to upload the report
            public bool NeedUpload { get; set; } // Whether need to upload the report to remote server

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="tmpFolder"></param>
            /// <param name="destFile"></param>
            /// <param name="needUpload"></param>
            /// <param name="uploadToken"></param>
            /// <param name="caseId"></param>
            /// <param name="domainName"></param>
            public SaveReportInfo(string tmpFolder, string destFile, bool needUpload=false,string uploadToken=null, string caseId=null, string domainName=null)
            {
                this.TempFolder = tmpFolder;
                this.DestFile = destFile;
                this.NeedUpload = needUpload;
                this.UploadToken = uploadToken;
                this.CaseId = caseId;
                this.DomainName = domainName;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"> contain the information to save and upload files</param>
        public SaveServerStatusReportAction(SaveReportInfo info) :base(null,Messages.ACTION_SAVE_UPLOAD_SERVER_STATUS_REPORT)
        {
            this.info = info;
        }

        /// <summary>
        /// Sync this action and sub-action. This save action need to wait until the sub-actions finished, one by one
        /// </summary>
        /// <param name="action">sub-aciton to sync with</param>
        private void syncAction(AsyncAction action)
        {
            // Wait until zipAciton finish
            lock (syncFinishMonitorLock)
            {
                while (!action.IsCompleted && !Cancelling && !Cancelled)
                {
                    Monitor.Wait(syncFinishMonitorLock);
                }

                if (Cancelling || Cancelled)
                    throw new CancelledException();
            }
        }

        /// <summary>
        /// Override funciton to redefine how to cancel this Action
        /// Also cancel the sub-actions
        /// </summary>
        protected override void CancelRelatedTask()
        {
            if (null != zipAction) zipAction.Cancel();

            if (null != uploadAction) uploadAction.Cancel();

            lock (syncFinishMonitorLock)
                Monitor.PulseAll(syncFinishMonitorLock);

            Description = Messages.CANCEL;
        }

        /// <summary>
        /// Override the function to redefine whether this action can be cancelled
        /// Can be cancelled if one of the sub-action not finished
        /// </summary>
        public override void RecomputeCanCancel()
        {
            if (this.IsCompleted)
            {
                CanCancel = false;
                return;
            }

            if(null != zipAction && !zipAction.IsCompleted || null != uploadAction && !uploadAction.IsCompleted)
            {
                CanCancel = true;
                return;
            }

            CanCancel = false;
        }

        /// <summary>
        /// Whether got error during run action
        /// side effect: Description will be set, exception will be loged
        /// </summary>
        /// <param name="action"></param>
        /// <returns>
        ///     true ---> error detected
        ///     false ---> no error
        /// </returns>

        private bool gotErrorOnAction(AsyncAction action)
        {
            // Error occur, will not continue
            if (null != action.Exception)
            {
                log.Debug(action.Exception);
                this.Description = action.Description;
                this.Exception = action.Exception;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Release the sync lock, used as the callback to sync between Action and sub-action
        /// The sub-action need to register this function to the callback to release the lock
        /// </summary>
        /// <param name="sender"></param>
        private void ReleaseSyncFinishMonitorLock(ActionBase sender)
        {
            lock (syncFinishMonitorLock)
            {
                Monitor.PulseAll(syncFinishMonitorLock);
            }
        }

        protected override void Run()
        {

            // zip up the report files and save them to the chosen file
            zipAction = new ZipStatusReportAction(info.TempFolder, info.DestFile);
            zipAction.Changed += delegate
            {
                if (info.NeedUpload)
                {
                    this.PercentComplete = zipAction.PercentComplete / 2;
                }
                else
                {
                    this.PercentComplete = zipAction.PercentComplete;
                }
                this.Description = zipAction.Description;
               
            };
            zipAction.Completed += ReleaseSyncFinishMonitorLock;
            zipAction.RunAsync();

            syncAction(zipAction);
            if (gotErrorOnAction(zipAction))
            {
                return;
            }

            // upload the report files
            if (info.NeedUpload)
            {
                uploadAction = new UploadServerStatusReportAction(info.DestFile, info.UploadToken, info.CaseId,
                                                                       info.DomainName, true);
                uploadAction.Changed += delegate
                {
                    this.PercentComplete = 50 + uploadAction.PercentComplete / 2;
                    this.Description = uploadAction.Description;
                };

                uploadAction.Completed += ReleaseSyncFinishMonitorLock;

                uploadAction.RunAsync();
                syncAction(uploadAction);
                if (gotErrorOnAction(uploadAction))
                {
                    return;
                }
            }
            this.Description = Messages.COMPLETED;
        }
    }

}
