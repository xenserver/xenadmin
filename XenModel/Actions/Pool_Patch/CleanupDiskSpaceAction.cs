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

using System.Collections.Generic;
using XenAPI;


namespace XenAdmin.Actions
{
    public class CleanupDiskSpaceAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pool_patch excludedPatch;

        public CleanupDiskSpaceAction(Host host, Pool_patch excludedPatch, bool suppressHistory)
            : base(host.Connection, Messages.ACTION_CLEANUP_DISK_SPACE_TITLE, "", suppressHistory)
        {
            Host = host;
            this.excludedPatch = excludedPatch;
            ApiMethodsToRoleCheck.Add("host.call_plugin");
        }

        protected override void Run()
        {
            Description = string.Format(Messages.ACTION_CLEANUP_DISK_SPACE_DESCRIPTION, Host.Name());
            try
            {
                var args = new Dictionary<string, string>();
                if (excludedPatch != null)
                    args.Add("exclude", excludedPatch.uuid);

                Result = Host.call_plugin(Session, Host.opaque_ref, "disk-space", "cleanup_disk_space", args);
                if (Result.ToLower() == "true")
                    Description = string.Format(Messages.ACTION_CLEANUP_DISK_SPACE_SUCCESS, Host.Name());
            }
            catch (Failure failure)
            {
                log.WarnFormat("Plugin call disk-space.cleanup_disk_space() on {0} failed with {1}", Host.Name(),
                               failure.Message);
                throw;
            }
        }
    }
}