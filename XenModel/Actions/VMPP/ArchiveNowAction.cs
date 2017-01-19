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
using System.Collections.Generic;
using System.Text;
using XenAPI;


namespace XenAdmin.Actions
{
    public class ArchiveNowAction : PureAsyncAction
    {
        private VM _snapshot;
        public ArchiveNowAction(VM snapshot)
            : base(snapshot.Connection, string.Format(Messages.ARCHIVE_SNAPSHOT_X, snapshot.Name))
        {
            _snapshot = snapshot;
            VM = snapshot.Connection.Resolve(snapshot.snapshot_of);
        }
        protected override void Run()
        {
            try
            {
                Description = string.Format(Messages.ARCHIVING_SNAPSHOT_X, _snapshot.Name);
                RelatedTask=new XenRef<Task>(VMPP.archive_now(Session, _snapshot.opaque_ref));
                PollToCompletion();
                Description = string.Format(Messages.ARCHIVED_SNAPSHOT_X, _snapshot.Name);
            }
            catch (Exception e)
            {
                Failure f = e as Failure;
                if (f != null)
                {
                    string msg = "";
                    if (f.ErrorDescription.Count > 3)
                    {
                        msg = XenAPI.Message.FriendlyName(f.ErrorDescription[3]);
                    }

                    throw new Exception(msg);

                }
                throw;
            }
        }
    }
}
