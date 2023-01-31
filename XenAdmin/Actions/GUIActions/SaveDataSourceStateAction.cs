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
using XenAdmin.Controls.CustomDataGraph;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class SaveDataSourceStateAction : AsyncAction
    {
        private readonly List<DataSourceItem> _dataSourceItems;
        private readonly List<DesignedGraph> _graphs;
        private readonly IXenObject _xenObject;

        public SaveDataSourceStateAction(IXenConnection connection, IXenObject xmo, List<DataSourceItem> items, List<DesignedGraph> graphs)
            : base(connection, Messages.ACTION_SAVE_DATASOURCES, Messages.ACTION_SAVING_DATASOURCES, true)
        {
            _dataSourceItems = items;
            _xenObject = xmo;
            _graphs = graphs;

            if (xmo is Host)
                ApiMethodsToRoleCheck.AddRange("host.record_data_source", "host.forget_data_source_archives");
            else if (xmo is VM)
                ApiMethodsToRoleCheck.AddRange("VM.record_data_source", "VM.forget_data_source_archives");

            ApiMethodsToRoleCheck.Add("pool.set_gui_config");
        }

        protected override void Run()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            var guiConfig = pool?.gui_config;

            if (_dataSourceItems != null)
            {
                foreach (DataSourceItem ds in _dataSourceItems)
                {
                    if (ds.DataSource.enabled != ds.Enabled)
                    {
                        if (_xenObject is Host host)
                        {
                            if (ds.Enabled)
                                Host.record_data_source(Session, host.opaque_ref, ds.DataSource.name_label);
                            else
                                Host.forget_data_source_archives(Session, host.opaque_ref, ds.DataSource.name_label);
                        }
                        else if (_xenObject is VM vm)
                        {
                            if (ds.Enabled)
                                VM.record_data_source(Session, vm.opaque_ref, ds.DataSource.name_label);
                            else
                                VM.forget_data_source_archives(Session, vm.opaque_ref, ds.DataSource.name_label);
                        }
                    }

                    if (ds.ColorChanged && guiConfig != null)
                    {
                        string key = Palette.GetColorKey(ds.DataSource.name_label, _xenObject);
                        string value = ds.Color.ToArgb().ToString();

                        if (!guiConfig.ContainsKey(key))
                            guiConfig.Add(key, value);
                        else
                            guiConfig[key] = value;
                    }
                }
            }

            var newGuiConfig = new Dictionary<string, string>();
            string uuid = Helpers.GetUuid(_xenObject);

            // build new gui config dictionary:
            // add keys not related to current XenObject
            if (guiConfig != null)
            {
                foreach (string key in guiConfig.Keys)
                {
                    bool isMatch = Palette.LayoutKey.IsMatch(key) || Palette.GraphNameKey.IsMatch(key);
                    if (isMatch && key.Contains(uuid))
                        continue;
                    newGuiConfig.Add(key, guiConfig[key]);
                }
            }

            if (_graphs != null)
            {
                // add current XenObject keys
                for (int i = 0; i < _graphs.Count; i++)
                {
                    string key = Palette.GetLayoutKey(i, _xenObject);
                    string value = _graphs[i].ToString();

                    // 'key' should not exist in the new gui config dictionary
                    newGuiConfig.Add(key, value);

                    key = Palette.GetGraphNameKey(i, _xenObject);
                    value = _graphs[i].DisplayName;
                    if (value != string.Empty)
                    {
                        newGuiConfig.Add(key, value);
                    }
                }
            }

            if (pool != null)
                Pool.set_gui_config(Session, pool.opaque_ref, newGuiConfig);
        }
    }
}
