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
using System.Linq;
using System.Collections.Generic;
using XenAPI;


namespace XenAdmin.Actions
{
    public class GetDataSourcesAction : AsyncAction
    {
        public List<Data_source> DataSources { get; } = new List<Data_source>();
        public IXenObject XenObject { get; }

        public GetDataSourcesAction(IXenObject xmo)
            : base(xmo.Connection, Messages.ACTION_GET_DATASOURCES, Messages.ACTION_GETTING_DATASOURCES, true)
        {
            XenObject = xmo;
        }

        protected override void Run()
        {
            List<Data_source> sources;

            if (XenObject is VM vm)
                sources = VM.get_data_sources(Session, vm.opaque_ref);
            else if (XenObject is Host host)
                sources = Host.get_data_sources(Session, host.opaque_ref);
            else
                return;

            DataSources.AddRange(sources);
        }
    }


    public class EnableDataSourceAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Data_source _dataSource;
        private readonly string _dataSourceFriendlyName;

        public EnableDataSourceAction(IXenObject xenObject, Data_source dataSource, string dataSourceFriendlyName)
            : base(xenObject.Connection, string.Format(Messages.ACTION_ENABLING_DATASOURCE, dataSourceFriendlyName), true)
        {
            _dataSource = dataSource;
            _dataSourceFriendlyName = dataSourceFriendlyName;

            if (xenObject is Host host)
            {
                Host = host;
                ApiMethodsToRoleCheck.Add("host.record_data_source");
            }
            else if (xenObject is VM vm)
            {
                VM = vm;
                ApiMethodsToRoleCheck.Add("VM.record_data_source");
            }
        }

        public List<Data_source> DataSources { get; private set; }

        protected override void Run()
        {
            try
            {
                if (Host != null)
                {
                    Host.record_data_source(Session, Host.opaque_ref, _dataSource.name_label);
                    DataSources = Host.get_data_sources(Session, Host.opaque_ref);
                }
                else if (VM != null)
                {
                    VM.record_data_source(Session, VM.opaque_ref, _dataSource.name_label);
                    DataSources = VM.get_data_sources(Session, VM.opaque_ref);
                }
            }
            catch (Exception e)
            {
                Description = string.Format(Messages.ACTION_ENABLING_DATASOURCE_ERROR, _dataSourceFriendlyName);
                log.Error($"Failed to enable data source {_dataSource.name_label}", e);
                throw;
            }
        }
    }
}
