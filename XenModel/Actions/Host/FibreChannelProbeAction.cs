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

using System.Collections.Generic;
using XenAPI;


namespace XenAdmin.Actions
{
    public class FibreChannelProbeAction : PureAsyncAction
    {

        public FibreChannelProbeAction(Host master)
            : base(master.Connection, string.Format(Messages.PROBING_HBA_TITLE, master.Name), null, true)
        {
            Host = master;
        }

        private SR.SRTypes srType = SR.SRTypes.lvmohba;

        public FibreChannelProbeAction(Host master, SR.SRTypes srType)
            : this(master)
        {
            this.srType = srType;
        }

        protected override void Run()
        {
            this.Description = Messages.PROBING_HBA;
            try
            {
                Result = XenAPI.SR.probe(Session, Host.opaque_ref, new Dictionary<string, string>(), srType.ToString(), new Dictionary<string, string>());
            }
            catch (XenAPI.Failure f)
            {
                if (f.ErrorDescription[0] == "SR_BACKEND_FAILURE_90" 
                 || f.ErrorDescription[0] == "SR_BACKEND_FAILURE_107")
                    Result = f.ErrorDescription[3];
                else
                    throw;
            }
            this.Description = Messages.PROBED_HBA;
        }
    }
}
