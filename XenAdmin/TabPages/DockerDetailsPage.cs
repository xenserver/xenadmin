/* Copyright (c) Citrix Systems Inc. 
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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Model;
using System.Xml;
using System.Collections;

namespace XenAdmin.TabPages
{
    public partial class DockerDetailsPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int REFRESH_INTERVAL = 20000;

        private IXenObject _xenObject;
        private DockerContainer _container;
        private VM _vmResideOn;
        private Host _hostResideOn;
        private string _resultCache;

        public IXenObject XenObject
        {
            get
            {
                Program.AssertOnEventThread();
                return _xenObject;
            }
            set
            {
                Program.AssertOnEventThread();

                if (value == null)
                    return;

                if (_xenObject != value)
                {
                    _xenObject = value;
                    if (_xenObject is DockerContainer)
                    {
                        _container = _xenObject as DockerContainer;

                        _vmResideOn = _container.Parent;
                        if (_vmResideOn.resident_on == null || string.IsNullOrEmpty(_vmResideOn.resident_on.opaque_ref) || (_vmResideOn.resident_on.opaque_ref.ToLower().Contains("null")))
                            return;

                        _hostResideOn = _container.Connection.Resolve(_vmResideOn.resident_on);
                        Rebuild();
                    }
                }
            }
        }

        private void CreateTree(XmlNode node, TreeNode rootNode)
        {
            Program.AssertOnEventThread();

            if (node.NodeType == XmlNodeType.Text)
                rootNode.Text = node.Value;
            else
            {
                if (node.Name == "SPECIAL_XS_ENCODED_ELEMENT")
                    rootNode.Text = node.Attributes["name"].Value;
                else
                    rootNode.Text = node.Name;
            }
            IEnumerator ienum = node.GetEnumerator();
            while (ienum.MoveNext())
            {
                XmlNode current = (XmlNode)ienum.Current;
                TreeNode currentNode = new TreeNode();
                CreateTree(current, currentNode);
                rootNode.Nodes.Add(currentNode);
            }
        }

        public void Rebuild()
        {
            Program.AssertOnEventThread();
            if (_xenObject is DockerContainer)
            {
                RefreshTime.Text = String.Format(Messages.LAST_REFRESH_SUCCESS, DateTime.Now.ToString("HH:mm:ss"));
                try
                {
                    string expectResult = "True";
                    var args = new Dictionary<string, string>{};
                    args["vmuuid"] = _vmResideOn.uuid;
                    args["object"] = _container.uuid;
                    Session session = _container.Connection.DuplicateSession();
                    string CurrentResult = XenAPI.Host.call_plugin(session, _hostResideOn.opaque_ref, "xscontainer", "get_inspect", args);
                    if (_resultCache == CurrentResult)
                        return;
                    else
                        _resultCache = CurrentResult;
                    DetailtreeView.Nodes.Clear();
                    if (CurrentResult.StartsWith(expectResult))
                    {
                        CurrentResult = CurrentResult.Substring(expectResult.Length);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(CurrentResult);
                        IEnumerator ienum = doc.GetEnumerator();
                        XmlNode docker_inspect;
                        while (ienum.MoveNext())
                        {
                            docker_inspect = (XmlNode)ienum.Current;
                            if (docker_inspect.NodeType != XmlNodeType.XmlDeclaration)
                            {
                                TreeNode rootNode = new TreeNode();
                                CreateTree(docker_inspect, rootNode);
                                DetailtreeView.Nodes.Add(rootNode);
                            }
                        }
                    }
                    else
                    {
                        RefreshTime.Text = Messages.LAST_REFRESH_FAIL;
                    }
                }
                catch (Failure failure)
                {
                    RefreshTime.Text = Messages.LAST_REFRESH_FAIL;
                    log.WarnFormat("Plugin call xscontainer.get_inspect({0}) on {1} failed with {2}", _container.uuid, _hostResideOn.Name,
                        failure.Message);
                    throw;
                }
            }
        }

        public DockerDetailsPage()
        {
            InitializeComponent();
            base.Text = Messages.DOCKER_DETAIL_TAB_TITLE;
            RefreshTimer.Interval = REFRESH_INTERVAL;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            Rebuild();
        }

        public void PauseRefresh()
        {
            RefreshTimer.Enabled = false;
        }

        public void ResumeRefresh()
        {
            RefreshTimer.Enabled = true;
        }
        
    }
}
