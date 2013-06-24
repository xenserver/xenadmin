/* Copyright (c) Citrix Systems Inc. 
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
using System.Collections.Generic;
using System.Text;
using XenAdmin.Network.StorageLink;
using System.Threading;
using System.Web.Services.Protocols;
using System.Net;
using XenAPI;

namespace XenAdmin.Actions
{
    public class StorageLinkDelegatedAsyncAction : AsyncAction
    {
        private readonly Func<StorageLinkJobInfo> _jobInvoker;
        private bool _cancel;
        private readonly string _endDescription;
        private readonly string _title;
        private readonly string _startDescription;
        private StorageLinkJobInfo _jobInfo;

        public StorageLinkDelegatedAsyncAction(Func<StorageLinkJobInfo> jobInvoker, string title, string startDescription, string endDescription)
            : base(null, title, startDescription)
        {
            Util.ThrowIfParameterNull(jobInvoker, "jobInvoker");
            _jobInvoker = jobInvoker;
            _endDescription = endDescription;
            _title = title;
            _startDescription = startDescription;
        }

        public StorageLinkDelegatedAsyncAction(Func<StorageLinkJobInfo> jobInvoker)
            : base(null, "")
        {
            Util.ThrowIfParameterNull(jobInvoker, "jobInvoker");
            _jobInvoker = jobInvoker;
        }

        protected override void Run()
        {
            _jobInfo = _jobInvoker();

            while (_jobInfo != null)
            {
                if (_cancel)
                {
                    _jobInfo.StorageLinkConnection.CancelJob(_jobInfo.JobId);
                    return;
                }

                _jobInfo = _jobInfo.StorageLinkConnection.GetJobInfo(_jobInfo.JobId);

                if (_title == null)
                {
                    Title = _jobInfo.Title;
                }

                if (_startDescription == null)
                {
                    Description = _jobInfo.Description;
                }

                PercentComplete = _jobInfo.Progress;

                if (_jobInfo.State == StorageLinkEnums.JobState.Failed)
                {
                    throw new InvalidOperationException(_jobInfo.ErrorMessage);
                }

                if(_jobInfo.State == StorageLinkEnums.JobState.Cancelled || _jobInfo.State == StorageLinkEnums.JobState.Successful)
                {
                    PercentComplete = 100;
                    Description = _endDescription;
                    Thread.Sleep(2000);
                    return;
                }
            }
        }

        protected override void CancelRelatedTask()
        {
            _cancel = true;
        }

        public override void RecomputeCanCancel()
        {
            bool result = false;
            
            if (_jobInfo != null)
            {
                if (_jobInfo.State == StorageLinkEnums.JobState.Running)
                {
                    result = true;
                }
                if (_jobInfo.State == StorageLinkEnums.JobState.Queued)
                {
                    result = true;
                }
                if (_jobInfo.State == StorageLinkEnums.JobState.Pending)
                {
                    result = true;
                }
                if (_jobInfo.State == StorageLinkEnums.JobState.Created)
                {
                    result = true;
                }
            }
            CanCancel = result;
        }
    }
}
