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
using System.Drawing;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAdmin.XenSearch;
using System.Windows.Forms;
using System.Diagnostics;
using XenAdmin.Commands;

namespace XenAdmin.Plugins
{
    internal class MenuItemFeatureCommand : Command
    {
        private readonly static List<MenuItemFeature> GlobalPlugins = new List<MenuItemFeature>();
        private readonly static Dictionary<MenuItemFeature, List<IXenObject>> ObjectPlugins = new Dictionary<MenuItemFeature, List<IXenObject>>();
        private readonly MenuItemFeature _menuItemFeature;
        private readonly Search _search;
        private readonly PluginSerializationLevel _serialization;
        private readonly List<IXenObject> _heldXenModelObjects = new List<IXenObject>();

        public MenuItemFeatureCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, MenuItemFeature menuItemFeature, Search search, PluginSerializationLevel serialization)
            : base(mainWindow, selection)
        {
            Util.ThrowIfParameterNull(menuItemFeature, "menuItemFeature");
            _menuItemFeature = menuItemFeature;
            _search = search;
            _serialization = serialization;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (!_menuItemFeature.Enabled)
            {
                return false;
            }

            if (_serialization == PluginSerializationLevel.global && GlobalPlugins.Contains(_menuItemFeature))
            {
                return false;
            }

            if (_serialization == PluginSerializationLevel.obj && ObjectPlugins.ContainsKey(_menuItemFeature))
            {
                foreach (IXenObject xenObject in GetAllXenObjectsAndExpandFolders(selection))
                {
                    if (ObjectPlugins[_menuItemFeature].Contains(xenObject))
                    {
                        return false;
                    }
                }
            }

            foreach (SelectedItem item in selection)
            {
                if (item.GroupingTag != null)
                {
                    return false;
                }
            }

            if (_search == null)
            {
                return true;
            }

            foreach (IXenObject xenObject in GetAllXenObjectsAndExpandFolders(selection))
            {
                if (!_search.Query.Match(xenObject))
                {
                    return false;
                }
            }

            return selection.Count > 0;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            if (_serialization == PluginSerializationLevel.global && !GlobalPlugins.Contains(_menuItemFeature))
            {
                GlobalPlugins.Add(_menuItemFeature);
            }

            if (_serialization == PluginSerializationLevel.obj)
            {
                if (!ObjectPlugins.ContainsKey(_menuItemFeature))
                {
                    ObjectPlugins[_menuItemFeature] = new List<IXenObject>();
                }

                foreach (IXenObject xenObject in GetAllXenObjectsAndExpandFolders(selection))
                {
                    _heldXenModelObjects.Add(xenObject);
                    ObjectPlugins[_menuItemFeature].Add(xenObject);
                }
            }

            ExternalPluginAction action;

            if (selection[0].XenObject == null && selection[0].GroupingTag == null)
            {
                // root node selected
                action = new ExternalPluginAction(_menuItemFeature, GetAllXenObjectsAndExpandFolders(selection), true);
            }
            else
            {
                action = new ExternalPluginAction(_menuItemFeature, GetAllXenObjectsAndExpandFolders(selection), false);

            }

            action.Completed += ExternalPluginAction_Completed;
            action.RunAsync();
        }

        /// <summary>
        /// Get all IXenObjects for the specified selection. The content of Folders is returned instead of the folders themselves.
        /// </summary>
        /// <param name="selection">The selection.</param>
        private List<IXenObject> GetAllXenObjectsAndExpandFolders(SelectedItemCollection selection)
        {
            List<IXenObject> output = new List<IXenObject>();
            foreach (SelectedItem item in selection)
            {
                Folder folder = item.XenObject as Folder;

                if (folder != null)
                {
                    foreach (IXenObject x in folder.RecursiveXenObjects)
                    {
                        output.Add(x);
                    }
                }
                else if (item.XenObject != null)
                {
                    output.Add(item.XenObject);
                }
            }
            return output;
        }

        private void ExternalPluginAction_Completed(ActionBase sender)
        {
            MethodInvoker method = delegate
            {
                if (_serialization == PluginSerializationLevel.global)
                {
                    GlobalPlugins.Remove(_menuItemFeature);
                }
                else if (_serialization == PluginSerializationLevel.obj)
                {
                    foreach (IXenObject xenObject in _heldXenModelObjects)
                    {
                        ObjectPlugins[_menuItemFeature].Remove(xenObject);
                    }

                    if (ObjectPlugins[_menuItemFeature].Count == 0)
                    {
                        ObjectPlugins.Remove(_menuItemFeature);
                    }

                }
            };

            if (Program.MainWindow != null)
            {
                Program.Invoke(Program.MainWindow, method);
            }
            else
            {
                // for tests
                method();
            }
        }

        public override string MenuText
        {
            get
            {
                return _menuItemFeature.ToString();
            }
        }

        public override Image MenuImage
        {
            get
            {
                return _menuItemFeature.Icon;
            }
        }

        public override string ToolTipText
        {
            get
            {
                return _menuItemFeature.Tooltip ?? string.Empty;
            }
        }
    }
}
