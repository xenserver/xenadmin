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
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Resources;
using System.Collections;
using XenAdmin.XenSearch;
using System.Reflection;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Core;
using System.Diagnostics;
using XenAdmin.Controls;
using XenAdmin.Properties;
using System.Text.RegularExpressions;
using System.ComponentModel;
using XenAdmin.Network;
using System.Collections.ObjectModel;


namespace XenAdmin.Plugins
{
    internal enum PluginMenu { file = 0, view = 1, pool = 2, server = 3, vm = 4, storage = 5, templates = 6, tools = 7, window = 8, help = 9, none = 10 }
    internal enum PluginSerializationLevel { none, global, obj }
    internal enum PluginContextMenu { none = 0, pool, server, vm, storage, template, folder }

    public class PluginDescriptor : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Feature> _features = new List<Feature>();
        private string _error;
        private Dictionary<string, Search> _searches = new Dictionary<string, Search>();
        private Dictionary<string, RbacMethodList> _methodLists = new Dictionary<string, RbacMethodList>();
        private ResourceManager _resourceManager;
        private bool _enabled;

        private string _organization;          // the name of the company folder inside the "Plugins" folder
        private string _name;       // the name of the plugin folder and .xcplugin.xml file
        private string _label;        // optional - $plugin.label in resources file, 'label' in the XML, replaces _name for GUI display purposes
        private Version _version;     // required - 'version' in the XML, currently only 1 and 2 are accepted
        private string _description;  // optional - $plugin.description in resources file, 'description' in the XML
        private string _copyright;    // optional - $plugin.copyright in resources file, 'copyright' in the XML
        private string _link;         // optional - $plugin.link in resources file, 'link' in the XML

        public const string ATT_DESCRIPTION = "description";
        public const string ATT_COPYRIGHT = "copyright";
        public const string ATT_LINK = "link";
        public const string ATT_LABEL = "label";
        public const string ATT_VERSION = "version";

        public const string METHOD_LIST_ELEMENT = "MethodList";
        public const string METHOD_LIST_ATT_NAME = "name";

        public const string SEARCH_ELEMENT = "Search";

        public PluginDescriptor(XmlNode node, string name, string organization)
        {
            _name = name;
            _organization = organization;
            LoadAll(node);

            // this will initialize the features
            Enabled = Settings.IsPluginEnabled(Name, Organization) && Properties.Settings.Default.LoadPlugins;

            ValidateAll();
            
        }

        private void ValidateAll()
        {
            if (_error != null)
            {
                Enabled = false;
                log.Error(string.Format("Error loading plugin '{0}::{1}'. Displaying as disabled. Detail: '{2}'", Organization, Name, _error));
                return;
            }
            foreach (Feature f in _features)
            {
                _error = f.CheckForError();
                if (_error != null)
                {
                    Enabled = false;
                    log.Error(string.Format("Error loading plugin '{0}::{1}'. Displaying as disabled. Detail: '{2}'", Organization, Name, _error));
                    return;
                }
            }
            
        }

        private void LoadAll(XmlNode node)
        {
            string v = Helpers.GetStringXmlAttribute(node, ATT_VERSION, "");

            if (v == "2")
            {
                _version = new Version(2, 0);
            }
            else if (v == "1" || v == "1.0.0.0")
            {
                _version = new Version(1, 0);
            }
            else
            {
                _error = string.Format(Messages.UNRECOGNISED_PLUGIN_VERSION, v);
                return;
            }

            _description = Helpers.GetStringXmlAttribute(node, ATT_DESCRIPTION, "");
            _copyright = Helpers.GetStringXmlAttribute(node, ATT_COPYRIGHT, "");
            _link = Helpers.GetStringXmlAttribute(node, ATT_LINK, "");

            _resourceManager = LoadResources();

            // localize
            if (_resourceManager != null)
            {
                string label = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_LABEL));
                _label = string.IsNullOrEmpty(label) ? _label : label;

                string description = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_DESCRIPTION));
                _description = string.IsNullOrEmpty(description) ? _description : description;

                string copyright = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_COPYRIGHT));
                _copyright = string.IsNullOrEmpty(copyright) ? _copyright : copyright;

                string link = _resourceManager.GetString(string.Format("{0}.{1}", Name, ATT_LINK));
                _link = string.IsNullOrEmpty(link) ? _link : link;
            }

            LoadMethodLists(node); // features depend on the list of methods
            LoadSearches(node); // features depend on the list of searches
            LoadFeatures(node);
        }

        public Search GetSearch(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid) && _searches.ContainsKey(uuid))
            {
                return _searches[uuid];
            }
            return null;
        }

        public RbacMethodList GetMethodList(string name)
        {
            if (!string.IsNullOrEmpty(name) && _methodLists.ContainsKey(name))
            {
                return _methodLists[name];
            }
            return null;
        }

        public static string PowerShellExecutable
        {
            get
            {
                string text = System.Environment.GetFolderPath(Environment.SpecialFolder.System);
                return Path.Combine(text, @"WindowsPowerShell\v1.0\powershell.exe");
            }
        }

        public ReadOnlyCollection<Feature> Features
        {
            get
            {
                return new ReadOnlyCollection<Feature>(_features);
            }
        }

        private ResourceManager LoadResources()
        {
            string resources = string.Format("{0}\\Plugins\\{1}\\{2}\\{2}.resources.dll", Application.StartupPath, Organization, Name);
            string cultured_resources = string.Format("{0}\\Plugins\\{1}\\{2}\\{3}\\{2}.resources.dll", Application.StartupPath, Organization, Name, Program.CurrentLanguage);

            if (File.Exists(cultured_resources))
            {
                resources = cultured_resources;
            }
            try
            {
                if (File.Exists(resources))
                {
                    // We load this "unsafely" because of CA-144950: the plugin is almost certainly
                    // downloaded from the web and won't install without this. I considered adding
                    // a confirmation step, but as all we do with the resources is to extract some
                    // strings, there is no security implication. (This doesn't affect security
                    // confirmations on programs called by the plugin).
                    return new ResourceManager(Name, Assembly.UnsafeLoadFrom(resources));
                }
            }
            catch (Exception e)
            {
                _error = e.Message;
            }

            return null;
        }

        private void LoadMethodLists(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element && child.Name == METHOD_LIST_ELEMENT)
                {
                    string name = Helpers.GetStringXmlAttribute(child, METHOD_LIST_ATT_NAME);
                    if (name == null)
                    {
                        _error = string.Format(Messages.CANNOT_PARSE_NODE_PARAM, node.Name, METHOD_LIST_ATT_NAME);
                    }
                    string[] list = child.InnerText.Trim().Split(',');
                    _methodLists.Add(name, new RbacMethodList(list));
                }
            }
        }

        private void LoadSearches(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element && child.Name == SEARCH_ELEMENT)
                {
                    try
                    {
                        Search s = SearchMarshalling.LoadSearch(child);
                        _searches.Add(s.UUID, s);
                    }
                    catch (Exception e)
                    {
                        _error = e.Message;
                        return;
                    }   
                }
            }
        }

        private void LoadFeatures(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;

                switch (child.Name)
                {
                    case MenuItemFeature.ELEMENT_NAME:
                        _features.Add(new MenuItemFeature(_resourceManager, child, this));
                        break;
                    case TabPageFeature.ELEMENT_NAME:
                        _features.Add(new TabPageFeature(_resourceManager, child, this));
                        break;
                    case ParentMenuItemFeature.ELEMENT_NAME:
                        _features.Add(new ParentMenuItemFeature(_resourceManager, child, this));
                        break;
                    case SEARCH_ELEMENT:
                        break;
                    case METHOD_LIST_ELEMENT:
                        break;
                    default:
                        _error = string.Format("XML node '{0}' not recognised", child.Name);
                        return;
                }
            }
        }

        public void DisposeURLs(IXenObject xmo)
        {
            foreach (Feature f in Features)
            {
                TabPageFeature tabPageFeature = f as TabPageFeature;

                if (tabPageFeature != null)
                {
                    tabPageFeature.DisposeURL(xmo);
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (value == _enabled)
                    return;

                _enabled = value;

                foreach (Feature f in Features)
                {
                    if (!value)
                    {
                        f.Dispose();
                    }
                    else if (value)
                    {
                        f.Initialize();
                    }
                }
            }
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

        public Version Version
        {
            get
            {
                return _version;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Copyright
        {
            get
            {
                return _copyright;
            }
        }

        public string Link
        {
            get
            {
                return _link;
            }
        }

        public string Error
        {
            get
            {
                return _error;
            }
        }

        public string Organization
        {
            get
            {
                return _organization;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Feature feature in Features)
                {
                    feature.Dispose();
                }
            }
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
