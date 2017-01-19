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
using XenAdmin.Controls;
using System.Drawing;
using XenAdmin.XenSearch;
using System.Xml;
using System.Resources;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Plugins
{
    /// <summary>
    /// Abstract base class for a ui addition added via a plugin
    /// </summary>
    public abstract class Feature : IDisposable
    {
        private readonly PluginDescriptor _pluginDescriptor;
        private readonly ResourceManager _resourceManager;
        protected readonly XmlNode node;

        private readonly Search _search;           // optional - "search" attribute, specifies a search defined in the config file to set enablement
        private readonly string _name;             // required - "name" attribute on the feature's tag
        private readonly string _label;            // optional - "label" attribute, used as a display string instead of 'name' if set.
        private readonly string _description;  // optional - $feature.description in resources file
        private readonly string _tooltip;          // optional - $feature.tooltip in resources file
        private readonly Image _icon;         // optional - $feature.icon in resources file

        public const string ATT_NAME = "name";
        public const string ATT_LABEL = "label";
        public const string ATT_DESCRIPTION = "description";
        public const string ATT_TOOLTIP = "tooltip";
        public const string ATT_ICON = "icon";
        public const string ATT_SEARCH = "search";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for localization</param>
        /// <param name="node">The XML node from the plugin config file</param>
        /// <param name="pluginDescriptor">The plugin descriptor.</param>
        public Feature(ResourceManager resourceManager, XmlNode node, PluginDescriptor pluginDescriptor)
        {
            Util.ThrowIfParameterNull(node, "node");
            Util.ThrowIfParameterNull(pluginDescriptor, "pluginDescriptor");

            this.node = node;
            _resourceManager = resourceManager;
            _pluginDescriptor = pluginDescriptor;

            _name = Helpers.GetStringXmlAttribute(node, ATT_NAME, "");
            _label = Helpers.GetStringXmlAttribute(node, ATT_LABEL, "");
            _description = Helpers.GetStringXmlAttribute(node, ATT_DESCRIPTION, "");
            _tooltip = Helpers.GetStringXmlAttribute(node, ATT_TOOLTIP, "");
            _icon = LoadIcon(Helpers.GetStringXmlAttribute(node, ATT_ICON, ""));
            _search = PluginDescriptor.GetSearch(Helpers.GetStringXmlAttribute(node, ATT_SEARCH, ""));

            // localize
            if (_resourceManager != null)
            {
                string label = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_LABEL));
                _label = string.IsNullOrEmpty(label) ? _label : label;

                string description = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_DESCRIPTION));
                _description = string.IsNullOrEmpty(description) ? _description : description;

                string tooltip = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_TOOLTIP));
                _tooltip = string.IsNullOrEmpty(tooltip) ? _tooltip : tooltip;

                string icon = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_ICON));
                _icon = string.IsNullOrEmpty(icon) ? _icon : LoadIcon(icon);
            }
        }

        public virtual string CheckForError()
        {
            if (_name == "")
                return string.Format(Messages.CANNOT_PARSE_NODE_PARAM, node.Name, ATT_NAME);

            return null;
        }

        private static Image LoadIcon(string path)
        {
            return string.IsNullOrEmpty(path) ? null : Image.FromFile(path);
        }

        public virtual void Initialize()
        {
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Label))
            {
                return Label;
            }

            return Name;
        }

        public string Label
        {
            get
            {
                return _label;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Tooltip
        {
            get
            {
                return _tooltip;
            }
        }

        public Image Icon
        {
            get
            {
                return _icon;
            }
        }

        public string PluginName
        {
            get
            {
                return PluginDescriptor.Name;
            }
        }

        protected Search Search
        {
            get
            {
                return _search;
            }
        }

        public RbacMethodList GetMethodList(string name)
        {
            return PluginDescriptor.GetMethodList(name);
        }

        public bool Enabled
        {
            get
            {
                return PluginDescriptor.Enabled;
            }
        }

        protected PluginDescriptor PluginDescriptor
        {
            get
            {
                return _pluginDescriptor;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
