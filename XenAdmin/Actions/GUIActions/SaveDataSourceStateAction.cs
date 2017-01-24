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
using XenAdmin.Network;
using XenAdmin.Controls.CustomDataGraph;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Actions
{
    public class SaveDataSourceStateAction : PureAsyncAction
    {
        List<DataSourceItem> DataSourceItems;
        List<DesignedGraph> Graphs;
        IXenObject XenObject;

        private Pool GetPool()
        {
            return Helpers.GetPoolOfOne(Connection);
        }

        private Dictionary<string, string> GetGuiConfig()
        {
            Pool pool = GetPool();
            return pool != null ? Helpers.GetGuiConfig(pool) : null;
        }

        public SaveDataSourceStateAction(IXenConnection connection, IXenObject xmo, List<DataSourceItem> items, List<DesignedGraph> graphs)
            : base(connection, Messages.ACTION_SAVE_DATASOURCES, Messages.ACTION_SAVING_DATASOURCES, true)
        {
            DataSourceItems = items;
            XenObject = xmo;
            Graphs = graphs;
        }

        protected override void Run()
        {
            Dictionary<string, string> gui_config = GetGuiConfig();

            if (DataSourceItems != null)
            {
                foreach (DataSourceItem ds in DataSourceItems)
                {
                    if (ds.DataSource.enabled != ds.Enabled)
                    {
                        Host host = XenObject as Host;
                        VM vm = XenObject as VM;
                        if (host != null)
                        {
                            if (ds.Enabled)
                                XenAPI.Host.record_data_source(Session, host.opaque_ref, ds.DataSource.name_label);
                            else
                                XenAPI.Host.forget_data_source_archives(Session, host.opaque_ref, ds.DataSource.name_label);
                        }
                        else if (vm != null)
                        {
                            if (ds.Enabled)
                                XenAPI.VM.record_data_source(Session, vm.opaque_ref, ds.DataSource.name_label);
                            else
                                XenAPI.VM.forget_data_source_archives(Session, vm.opaque_ref, ds.DataSource.name_label);
                        }
                    }

                    if (ds.ColorChanged)
                    {
                        if (gui_config != null)
                        {
                            string key = Palette.GetColorKey(ds.DataSource.name_label, XenObject);
                            string value = ds.Color.ToArgb().ToString();

                            if (!gui_config.ContainsKey(key))
                                gui_config.Add(key, value);
                            else
                                gui_config[key] = value;
                        }
                    }
                }
            }

            Dictionary<string, string> new_gui_config = new Dictionary<string, string>();
            string uuid = Helpers.GetUuid(XenObject);

            // build new gui config dictionary:
            // add keys not related to current XenObject
            if (gui_config != null)
            {
                foreach (string key in gui_config.Keys)
                {
                    bool isMatch = (Palette.LayoutKey.IsMatch(key) || Palette.GraphNameKey.IsMatch(key));
                    if (isMatch && key.Contains(uuid))
                        continue;
                    new_gui_config.Add(key, gui_config[key]);
                }
            }

            if (Graphs != null)
            {
                // add current XenObject keys
                for (int i = 0; i < Graphs.Count; i++)
                {
                    string key = Palette.GetLayoutKey(i, XenObject);
                    string value = Graphs[i].ToString();

                    // 'key' should not exist in the new gui config dictionary
                    new_gui_config.Add(key, value);

                    key = Palette.GetGraphNameKey(i, XenObject);
                    value = Graphs[i].DisplayName;
                    if (value != String.Empty)
                    {
                        new_gui_config.Add(key, value);
                    }
                }
            }

            XenAPI.Pool.set_gui_config(Session, GetPool().opaque_ref, new_gui_config);
        }
    }
}
