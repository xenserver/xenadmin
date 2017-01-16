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
using System.Xml;
using System.Windows.Forms;
using XenAdmin.Core;
using System.Resources;
using System.Collections.ObjectModel;
using XenAdmin.Commands;


namespace XenAdmin.Plugins
{
    /// <summary>
    /// Class to describe a menu item which is added to Xencenter via a plugin and is used to collect under submenus the menu items that launch the plugin commands.
    /// </summary>
    internal class ParentMenuItemFeature : Feature
    {
        private readonly List<MenuItemFeature> _features = new List<MenuItemFeature>();
        private readonly PluginMenu _menu;
        private readonly PluginContextMenu _contextMenu = PluginContextMenu.none;

        public const string ELEMENT_NAME = "GroupMenuItem";

        public ParentMenuItemFeature(ResourceManager resourceManager, XmlNode node, PluginDescriptor plugin)
            : base(resourceManager, node, plugin)
        {
            _menu = Helpers.GetEnumXmlAttribute<PluginMenu>(node, MenuItemFeature.ATT_MENU, PluginMenu.none);
            _contextMenu = Helpers.GetEnumXmlAttribute<PluginContextMenu>(node, MenuItemFeature.ATT_CONTEXT_MENU, MenuItemFeature.GetContextMenuFromMenu(_menu));

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    if (child.Name == MenuItemFeature.ELEMENT_NAME)
                        _features.Add(new MenuItemFeature(resourceManager, child, PluginDescriptor, this));
                }
            }
        }

        public override string CheckForError()
        {
            if (_menu == PluginMenu.none)
                return string.Format(Messages.CANNOT_PARSE_NODE_PARAM, node.Name, MenuItemFeature.ATT_MENU);

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element && child.Name != MenuItemFeature.ELEMENT_NAME)
                    return string.Format(Messages.PLUGINS_UNRECOGNISED_XML_NODE, child.Name, node.Name);
            }

            foreach (Feature f in _features)
            {
                string s = f.CheckForError();
                if (s != null)
                    return s;
            }

            return base.CheckForError();
        }

        public override void Initialize()
        {
            foreach (MenuItemFeature feature in _features)
            {
                feature.Initialize();
            }
        }

        public ParentMenuItemFeatureCommand GetCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
        {
            return new ParentMenuItemFeatureCommand(mainWindow, selection, this, Search);
        }

        public PluginMenu Menu
        {
            get
            {
                return _menu;
            }
        }

        public PluginContextMenu ContextMenu
        {
            get
            {
                return _contextMenu;
            }
        }

        public ReadOnlyCollection<MenuItemFeature> Features
        {
            get
            {
                return new ReadOnlyCollection<MenuItemFeature>(_features);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (MenuItemFeature feature in _features)
                {
                    feature.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
