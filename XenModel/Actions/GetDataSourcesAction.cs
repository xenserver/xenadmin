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

using System.Linq;
using System.Collections.Generic;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public class GetDataSourcesAction : AsyncAction
    {
        public readonly List<Data_source> DataSources = new List<Data_source>();
        public readonly IXenObject IXenObject;

        public GetDataSourcesAction(IXenConnection connection, IXenObject xmo)
            : base(connection, Messages.ACTION_GET_DATASOURCES, Messages.ACTION_GETTING_DATASOURCES, true)
        {
            IXenObject = xmo;
        }

        protected override void Run()
        {
            List<Data_source> sources;

            Host host = IXenObject as Host;
            VM vm = IXenObject as VM;
            if (vm != null)
            {
                sources = XenAPI.VM.get_data_sources(Session, vm.opaque_ref);
            }
            else if (host != null)
            {
                sources = XenAPI.Host.get_data_sources(Session, host.opaque_ref);
            }
            else
            {
                return;
            }
            DataSources.AddRange(sources);
            // add custom datasources

            //CA-89512: We are provided the Avg CPU for server >= Tampa - otherwise work it out for ourselves
            if(!DataSources.Any(d=>d.name_label=="cpu_avg"))
            {
                Data_source avg_cpu_source = new Data_source("avg_cpu", "", true, false, "percentage", 0d, double.MaxValue, 0d);
                DataSources.Add(avg_cpu_source);
            }
        }
    }
}
