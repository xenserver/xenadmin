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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public enum HostStatus { queued, compiling, downloading, succeeded, failed };
    public class HostWithStatus : IEquatable<HostWithStatus>
    {
        public Host Host;

        public Exception error;

        public HostStatus Status = HostStatus.queued;

        public long DataTransferred = 0;
        public long Size = 0;

        public HostWithStatus(Host host, long size)
        {
            Host = host;
            Size = size;
        }

        public string StatusString
        {
            get
            {
                if (error is CancelledException)
                    return Messages.CANCELLED_BY_USER;

                switch (Status)
                {
                    case HostStatus.failed:
                        {
                            if (error == null)
                                return Messages.BUGTOOL_HOST_STATUS_FAILED;

                            var searchTerms = new List<string> {"HTTP", "403", "Forbidden"};
                            if (error.Message != null && searchTerms.TrueForAll(s => error.Message.Contains(s)))
                            {
                                // RBAC Failure
                                var roles = Host.Connection.Session.Roles;
                                roles.Sort();
                                var msg = string.Format(Messages.BUGTOOL_RBAC_FAILURE, roles[0].FriendlyName);
                                return string.Format(Messages.BUGTOOL_HOST_STATUS_FAILED_WITH_ERROR, msg);
                            }

                            return string.Format(Messages.BUGTOOL_HOST_STATUS_FAILED_WITH_ERROR, error.Message);
                        }
                    case HostStatus.succeeded:
                        return Messages.COMPLETED;
                    case HostStatus.compiling:
                        return Messages.BUGTOOL_HOST_STATUS_COMPILING;
                    case HostStatus.downloading:
                        return string.Format(Messages.BUGTOOL_HOST_STATUS_DOWNLOADING, Util.MemorySizeStringSuitableUnits(DataTransferred, false));
                    default:
                        return Messages.BUGTOOL_HOST_STATUS_PENDING;
                }
            }
        }

        public override string ToString()
        {
            return Helpers.GetName(Host);
        }

        public bool Equals(HostWithStatus other)
        {
            return Host.opaque_ref == other.Host.opaque_ref;
        }
    }
}