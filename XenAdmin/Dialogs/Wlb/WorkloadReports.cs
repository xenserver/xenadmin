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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml;
using System.Web;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Controls.Wlb;
// Report viewer control dependencies
using Microsoft.Reporting.WinForms;


namespace XenAdmin.Dialogs.Wlb
{
    public partial class WorkloadReports : XenDialogBase
    {
        #region Private Variables

        private IEnumerable<Host> _hosts;
        private Pool _pool;
        private WlbReportSubscriptionCollection _subscriptionCollection;
        private readonly string _reportFile = string.Empty;
        private bool _runReport;
        private bool _isCreedenceOrLater;
        private XmlNodeList _currentNodes;

        #endregion


        #region Properties

        /// <summary>
        /// Object set to current  pool
        /// </summary>
        public Pool Pool
        {
            get { return _pool; }
            set { _pool = value; }
        }

        /// <summary>
        /// Collection of hosts for the current pool
        /// </summary>
        public IEnumerable<Host> Hosts
        {
            get { return _hosts; }
            set { _hosts = value; }
        }


        #endregion


        #region Constructor

        public WorkloadReports()
        {
            InitializeComponent();

            splitContainerLeftPane.Panel2Collapsed = true;
            lblSubscriptions.Visible = false;
            treeViewSubscriptionList.Visible = false;
            wlbReportView1.ButtonSubscribeVisible = false;
            wlbReportView1.ButtonLaterReportVisible = false;
            wlbReportView1.Visible = false;
            subscriptionView1.Visible = false;
        }

        /// <summary>
        /// Overloaded constructor provides access to a specific report upon load
        /// </summary>
        public WorkloadReports(string reportFile, bool run)
            : this()
        {
            _reportFile = reportFile;
            _runReport = run;
        }

        #endregion

        internal override string HelpName => "WLBReports";

        #region Private Methods

        /// <summary>
        /// Populates the treeview with ReportInfo and SubscriptionInfo nodes
        /// </summary>
        private void PopulateTreeViewReportList()
        {
            treeViewReportList.BeginUpdate();

            try
            {
                treeViewReportList.Nodes.Clear();
                treeViewReportList.ImageList = CreateReportImageList();

                if (_currentNodes == null || _currentNodes.Count == 0)
                    return;

                TreeNode nodeToSelect = null;
                
                for (int i = 0; i < _currentNodes.Count; i++)
                {
                    TreeNode currentReportTreeNode = GetReportTreeNode(_currentNodes[i]);
                    treeViewReportList.Nodes.Add(currentReportTreeNode);

                    if (nodeToSelect != null)
                        continue;

                    string currentReportFile = ((WlbReportInfo)currentReportTreeNode.Tag).ReportFile.Split('.')[0];
                        
                    if (string.Compare(currentReportFile, _reportFile, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                        nodeToSelect = currentReportTreeNode;
                }

                treeViewReportList.Sort();
                treeViewReportList.SelectedNode = nodeToSelect ?? treeViewReportList.Nodes[0];
            }
            finally
            {
                treeViewReportList.EndUpdate();
            }
        }


        /// <summary>
        /// Populates the subscription list with items of type WlbReportSubscription
        /// </summary>
        private void PopulateTreeViewSubscriptionList()
        {
            treeViewSubscriptionList.BeginUpdate();

            try
            {
                treeViewSubscriptionList.Nodes.Clear();
                treeViewSubscriptionList.ImageList = CreateReportImageList();

                if (_subscriptionCollection != null)
                {
                    lblSubscriptions.Visible = true;
                    treeViewSubscriptionList.Visible = true;

                    foreach (string key in _subscriptionCollection.Keys)
                        treeViewSubscriptionList.Nodes.Add(GetReportSubscriptionTreeNode(_subscriptionCollection[key]));
                }
                else
                {
                    lblSubscriptions.Visible = false;
                    treeViewSubscriptionList.Visible = false;
                }
            }
            finally
            {
                treeViewSubscriptionList.EndUpdate();
            }
        }


        /// <summary>
        /// Used by drilldown and back events from the reportviewer to maintain treeview state
        /// </summary>
        private void SynchTreeViewReportList(string reportName)
        {
            WlbReportInfo reportInfo;
            wlbReportView1.ResetReportViewer = false;

            string reportFile = String.Format("{0}.{1}", reportName, "rdlc");

            for (int i = 0; i < treeViewReportList.Nodes.Count; i++)
            {
                if ((treeViewReportList.Nodes[i].Tag != null) &&
                    (treeViewReportList.Nodes[i].Tag.GetType() == typeof(WlbReportInfo)))
                {
                    reportInfo = (WlbReportInfo)treeViewReportList.Nodes[i].Tag;

                    if (reportInfo.ReportFile == reportFile)
                    {
                        treeViewReportList.SelectedNode = treeViewReportList.Nodes[i];
                        treeViewReportList.Select();
                        wlbReportView1.ViewerReportInfo = reportInfo;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Update report treeView when edit/add/delete report subscriptions
        /// </summary>
        private void UpdateReportTreeView()
        {
            // Begin update report treeView
            treeViewReportList.BeginUpdate();

            // If there is a selected subscription node, need to save it before it got cleared
            // There is no need to save selected report node since it never got cleared
            WlbReportSubscription selectedSub = null;
            String selectedSubReportFile = String.Empty;
            if (this.treeViewReportList.SelectedNode != null && this.treeViewReportList.SelectedNode.Tag != null && this.treeViewReportList.SelectedNode.Tag.GetType() == typeof(WlbReportSubscription))
            {
                // Save selected subscription node for later
                selectedSub = (WlbReportSubscription)this.treeViewReportList.SelectedNode.Tag;

                // Save the report file of the selected subscription node for later
                selectedSubReportFile = ((WlbReportInfo)this.treeViewReportList.SelectedNode.Parent.Parent.Tag).ReportFile;
            }

            // Update subscription nodes
            foreach (TreeNode reportNode in this.treeViewReportList.Nodes)
            {
                // Get subscriptions under the report node
                Dictionary<string, WlbReportSubscription> subscriptionList = _subscriptionCollection.GetReportSubscriptionByReportName(((WlbReportInfo)reportNode.Tag).ReportFile);

                // If there's subscriptions for the current report, add them
                if (subscriptionList.Count > 0)
                {
                    // Add a subscriptions folder if there isn't one
                    if (reportNode.GetNodeCount(true) == 0)
                    {
                        TreeNode subscriptionFolderNode = new TreeNode(Messages.WLB_SUBSCRIPTIONS, 2, 2);
                        reportNode.Nodes.Add(subscriptionFolderNode);
                    }

                    // Delete all the nodes to build the updated list
                    reportNode.Nodes[0].Nodes.Clear();

                    // Add in the latest nodes
                    if (selectedSub != null && String.Compare(selectedSubReportFile, ((WlbReportInfo)reportNode.Tag).ReportFile, true) == 0)
                    {
                        // Add new subscription node and retrieve selected node if there is one
                        AddNewSubscriptionNode(reportNode.Nodes[0], subscriptionList, ((WlbReportInfo)reportNode.Tag).ReportName, selectedSub);
                    }
                    else
                    {
                        // Add new subscription node
                        AddNewSubscriptionNode(reportNode.Nodes[0], subscriptionList, ((WlbReportInfo)reportNode.Tag).ReportName, null);
                    }
                }
                else
                {
                    // Force to select reportNode before clear reportNode's children node
                    // only if the original selected subscription has been deleted 
                    // and there are no more subscriptions under the report
                    if (selectedSub != null && String.Compare(selectedSubReportFile, ((WlbReportInfo)reportNode.Tag).ReportFile, true) == 0)
                    {
                        this.treeViewReportList.SelectedNode = reportNode;
                    }

                    // Clear reportNode's children node (subscription folder node) 
                    // if there is no subscription under the reportNode
                    reportNode.Nodes.Clear();
                }
            }

            // End updating report treeView
            treeViewReportList.EndUpdate();
        }


        /// <summary>
        /// Update subscriptionTreeView when Add/Edit/Delete subscription
        /// </summary>
        private void UpdateSubscriptionTreeView()
        {
            // Begin update subscription treeView
            treeViewSubscriptionList.BeginUpdate();

            // Save selected node if there is one
            WlbReportSubscription selectedSub = null;
            if (treeViewSubscriptionList.SelectedNode != null)
            {
                selectedSub = (WlbReportSubscription)treeViewSubscriptionList.SelectedNode.Tag;
            }

            // Delete all nodes
            treeViewSubscriptionList.Nodes.Clear();

            // Add subscription nodes if there is any
            if (_subscriptionCollection.Count > 0)
            {
                foreach (string key in _subscriptionCollection.Keys)
                {
                    TreeNode subNode = GetReportSubscriptionTreeNode(_subscriptionCollection[key]);
                    treeViewSubscriptionList.Nodes.Add(subNode);

                    // Retrieve selected node if there is one
                    if (selectedSub != null && String.Compare(_subscriptionCollection[key].Id, selectedSub.Id, true) == 0)
                    {
                        treeViewSubscriptionList.SelectedNode = subNode;
                    }
                }

                // Force to set selected node to the first node 
                // only if treeViewSubscriptionList has selected subscription node before update
                // but the selected subscription doesn't exist anymore
                if (selectedSub != null && treeViewSubscriptionList.SelectedNode == null)
                {
                    treeViewSubscriptionList.SelectedNode = treeViewSubscriptionList.Nodes[0];
                }
            }
            else if (selectedSub != null)
            {
                // No nodes left in the tree so select the current report in the reportviewer tree
                foreach (TreeNode reportNode in this.treeViewReportList.Nodes)
                {
                    if (((WlbReportInfo)reportNode.Tag).ReportName == selectedSub.ReportDisplayName)
                    {
                        treeViewReportList.SelectedNode = reportNode;
                        break;
                    }
                }
            }

            // End update subscription treeView
            treeViewSubscriptionList.EndUpdate();
        }


        /// <summary>
        /// Adds an image list to the current treeview for use by each of the nodes types
        /// </summary>
        private ImageList CreateReportImageList()
        {
            ImageList reportImageList;

            reportImageList = new ImageList();

            reportImageList.Images.Add(Images.StaticImages._000_GetServerReport_h32bit_16);
            reportImageList.Images.Add(Images.StaticImages.subscribe);
            reportImageList.Images.Add(Images.StaticImages._000_Folder_open_h32bit_16);

            return reportImageList;
        }


        /// <summary>
        /// Retrieve subscriptions and set _subscriptionCollection
        /// </summary>
        private void RetrieveSubscriptionCollection()
        {
            _subscriptionCollection = null;

            RetrieveWlbConfigurationAction action = new RetrieveWlbConfigurationAction(_pool);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks) {ShowCancel = true})
                dialog.ShowDialog(this);

            if (action.Succeeded)
            {
                var poolConfiguration = new WlbPoolConfiguration(action.WlbConfiguration);
                _isCreedenceOrLater = poolConfiguration.IsCreedenceOrLater;
            }
            else
            {
                Close();
            }
        }


        /// <summary>
        /// Attempts to get the latest set of reports from the WLB server version 2.0
        /// and beyond (it invokes and runs a call to the Kirkwood database via Xapi 
        /// to obtain report configuration data including the actual
        /// rdlc report definitions).
        /// If there are no report definitions on WLB server, it obtains them
        /// from local XML file (the old way).
        /// </summary>
        private XmlNodeList GetReportsConfig()
        {
            string reportName = "get_report_definitions";

            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("LocaleCode", Program.CurrentLanguage);
            if (_isCreedenceOrLater)
            {
                parms.Add("ReportVersion", "Creedence");
                parms.Add("PoolId", _pool.uuid);
            }

            AsyncAction action = new WlbReportAction(_pool.Connection,
                                                Helpers.GetCoordinator(_pool.Connection),
                                                reportName,
                                                Messages.WLB_REPORT_DEFINITIONS,
                                                true,
                                                parms);

            using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dlg.ShowDialog();

            string returnValue = action.Result;
            XmlDocument xmlReportsDoc = new XmlDocument();

            if (action.Succeeded && !string.IsNullOrEmpty(returnValue))
            {
                try
                {
                    xmlReportsDoc.LoadXml(returnValue);
                }
                catch
                {
                    //ignore
                }
            }

            if (!xmlReportsDoc.HasChildNodes || xmlReportsDoc.DocumentElement == null ||
                !xmlReportsDoc.DocumentElement.HasChildNodes)
            {
                try
                {
                    xmlReportsDoc.Load($@"{Application.StartupPath}\{"reports.xml"}");
                }
                catch
                {
                    //ignore
                }
            }

            return xmlReportsDoc.SelectNodes(@"Reports/Report");
        }

        // To enhance pool audit trail report, WLB server updates RDLC and would send user and object lists.
        // These values are sent along with parameter keys "AuditUser", "AuditObject".
        // The GetCustomXmlElement functions are used to retrieve the parameter values from WLB server.
        private static XmlElement GetCustomXmlElement(XmlElement root)
        {
            XmlNodeList labelNodes = root.GetElementsByTagName("Custom");
            if (labelNodes != null && labelNodes.Count > 0)
            {
                string xmlCustom = labelNodes[0].InnerXml;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<root>" + xmlCustom + "</root>");
                return xmlDoc.DocumentElement;
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                string searchStart = "Function Custom() As String";
                string searchEnd = "\nEnd Function";
                string xmlDocString = root.InnerXml;
                int startPos = xmlDocString.IndexOf(searchStart, 0);
                if (startPos > -1)
                {
                    int endPos = xmlDocString.IndexOf(searchEnd, startPos);
                    string customString = xmlDocString.Substring(startPos + searchStart.Length, endPos - startPos - searchStart.Length);
                    customString = customString.Trim().Substring("Return".Length).Trim().Trim("\"".ToCharArray());
                    customString = HttpUtility.HtmlDecode(customString);
                    xmlDoc.LoadXml(customString);
                    return xmlDoc.DocumentElement;
                }
                else
                {
                    return null;
                }
            }
        }

        private static XmlElement GetCustomXmlElement(XmlElement root, string version)
        {
            XmlElement customXml = GetCustomXmlElement(root);
            if (customXml != null)
            {
                XmlNodeList versionNodes = customXml.GetElementsByTagName("Version");
                foreach (XmlNode thisNode in versionNodes)
                {
                    if (thisNode.Attributes["value"].Value == version)
                    {
                        XmlDocument thisDoc = new XmlDocument();
                        thisDoc.LoadXml("<root>" + thisNode.InnerXml + "</root>");
                        return thisDoc.DocumentElement;
                    }
                }
            }
            return null;
        }

        private static XmlElement GetCustomXmlElement(XmlElement root, string version, string tagName)
        {
            XmlElement customXml = GetCustomXmlElement(root, version);
            if (customXml != null)
            {
                XmlNodeList tagNodes = customXml.GetElementsByTagName(tagName);
                if (tagNodes != null && tagNodes.Count > 0)
                {
                    XmlDocument thisDoc = new XmlDocument();
                    thisDoc.LoadXml("<root>" + tagNodes[0].InnerXml + "</root>");
                    return thisDoc.DocumentElement;
                }
            }
            return null;
        }

        private static XmlElement GetCustomXmlElement(XmlNode root, string version, string tagName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<root>" + root.InnerXml + "</root>");
            return GetCustomXmlElement(xmlDoc.DocumentElement, version, tagName);
        }


        /// <summary>
        /// Creates a node of type ReportTreeNode based on information specified from the XML report 
        /// configuration file.
        /// </summary>
        /// <param name="currentNode">Current report XML node from config file</param>
        /// <returns></returns>
        private TreeNode GetReportTreeNode(XmlNode currentNode)
        {
            // If the report definition node doesn't exist (old WLB version), load the definition from the 
            // local file system. Otherwise, the definition is present in the config from the WLB server

            string nodeReportDefinition;
            var rdlc = currentNode.SelectSingleNode(@"Rdlc");
            var nodeFileName = currentNode.Attributes?["File"].Value;

            if (rdlc == null)
            {
                XmlDocument xmlReportDefinition = new XmlDocument();
                xmlReportDefinition.Load($@"{Application.StartupPath}\{nodeFileName}");
                nodeReportDefinition = xmlReportDefinition.OuterXml;
            }
            else
            {
                nodeReportDefinition = rdlc.InnerText;
            }

            string nodeNameLabel = string.Empty;

            if (currentNode.Attributes?["Name"] != null)
                nodeNameLabel = currentNode.Attributes["Name"].Value;
            else if (currentNode.Attributes?["NameLabel"].Value != null)
            {
                var obj = Messages.ResourceManager.GetObject(currentNode.Attributes?["NameLabel"].Value);
                if (obj != null)
                    nodeNameLabel = obj.ToString();
            }

            var nodeDisplayFilter = currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='Filter']") != null;

            var nodeDisplayHosts = currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='HostID']") != null;

            var nodeDisplayUsers = currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='AuditUser']") != null;

            var nodeDisplayAuditObjects = currentNode.SelectSingleNode(@"QueryParameters/QueryParameter[@Name='AuditObject']") != null;

            XmlElement queryParametersXmlElement = GetCustomXmlElement(currentNode, "Creedence", "QueryParameters");
            var nodeParamDict = GetSQLQueryParamNames(currentNode, queryParametersXmlElement);

            var reportInfo = new WlbReportInfo(nodeNameLabel,
                nodeFileName,
                nodeReportDefinition,
                nodeDisplayHosts,
                nodeDisplayFilter,
                nodeDisplayUsers,
                nodeDisplayAuditObjects,
                nodeParamDict);

            return new TreeNode
            {
                Tag = reportInfo,
                Text = nodeNameLabel,
                ImageIndex = 0,
                SelectedImageIndex = 0
            };
        }


        /// <summary>
        /// Creates a treeNode for give report name and subscription object
        /// </summary>
        /// <param name="subscription">Instance of a subscription</param>
        /// <returns>Return a instance of TreeNode</returns>
        private TreeNode GetReportSubscriptionTreeNode(WlbReportSubscription subscription)
        {
            TreeNode subscriptionTreeNode = new TreeNode();

            subscriptionTreeNode.Text = subscription.Name;
            subscriptionTreeNode.Tag = subscription;
            subscriptionTreeNode.ImageIndex = 1;
            subscriptionTreeNode.SelectedImageIndex = 1;

            return subscriptionTreeNode;
        }


        /// <summary>
        /// Returns a list of parameters names whose values will be required for reports' SQL query
        /// to run. These names are specified in the Report XML configuration file.
        /// </summary>
        /// <param name="currentNode">Current report XML node from config file</param>
        /// <param name="queryParametersXmlElement">Current report XML node from config file</param>
        /// <returns>OrderedDictionary of parameter names and contents whose values are required by the report SQL query</returns>
        private OrderedDictionary GetSQLQueryParamNames(XmlNode currentNode, XmlElement queryParametersXmlElement)
        {
            OrderedDictionary paramNames = new OrderedDictionary();
            XmlNodeList paramNameNodes = currentNode.SelectNodes("QueryParameters/QueryParameter");

            foreach (XmlNode paramNode in paramNameNodes)
            {
                if (paramNode.Attributes["Name"] != null)
                {
                    string value = paramNode.Attributes["Name"].Value;
                    // To implement the AuditUser and AuditObject dropdowns, the values are set
                    // along with parameter "AuditUser' and "AuditObject" from WLB server.
                    // If the query parameters contain values, store them in dictionary.
                    // Else just store the parameter as key with "" value.
                    if(_isCreedenceOrLater && queryParametersXmlElement != null)
                    {
                        XmlNodeList valueTagNodes = queryParametersXmlElement.GetElementsByTagName(value);
                        if (valueTagNodes != null && valueTagNodes.Count > 0)
                        {
                            paramNames.Add(value, valueTagNodes[0].InnerText);
                        }
                        else
                        {
                            paramNames.Add(value, "");
                        }
                    }
                    else
                    {
                        paramNames.Add(value, "");
                    }
                }
            }

            return paramNames;
        }


        /// <summary>
        /// Add subscription nodes if a report has subscriptions
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <param name="reportName">Report display name that can be added to WlbReportSubscription instance</param>
        /// <param name="selectedSub">The original selected node before update</param>
        /// <param name="subscriptionFolder"></param>
        private void AddNewSubscriptionNode(TreeNode subscriptionFolder, Dictionary<string, WlbReportSubscription> subscriptions, string reportName, WlbReportSubscription selectedSub)
        {
            foreach (WlbReportSubscription sub in subscriptions.Values)
            {
                sub.ReportDisplayName = reportName;
                TreeNode subNode = GetReportSubscriptionTreeNode(sub);
                subscriptionFolder.Nodes.Add(subNode);

                // Retrieve selected subscription node if there is one
                if (selectedSub != null && String.Compare(selectedSub.Id, sub.Id, true) == 0)
                {
                    this.treeViewReportList.SelectedNode = subNode;
                }
            }

            // Force to set selected node to the first subscription node of the report 
            // only if the original selected subscription node of the report has been deleted.
            if (selectedSub != null && subscriptions.Count > 0 && (this.treeViewReportList.SelectedNode == null || this.treeViewReportList.SelectedNode.Tag == null))
            {
                this.treeViewReportList.SelectedNode = subscriptionFolder.Nodes[0];
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Load report form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkloadReports_Load(object sender, EventArgs e)
        {
            subscriptionView1.OnChangeOK += OnChangeOK_Refresh;
            wlbReportView1.OnChangeOK += OnChangeOK_Refresh;

            RetrieveSubscriptionCollection();
            _currentNodes = GetReportsConfig();
        }


        /// <summary>
        /// When the report window is displayed, the current pool and a list of hosts that
        /// correlate to the pool need to be set in the viewer control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkloadReports_Shown(object sender, EventArgs e)
        {
            subscriptionView1.Pool = _pool;
            wlbReportView1.Pool = _pool;
            wlbReportView1.Hosts = _hosts;
            wlbReportView1.IsCreedenceOrLater = _isCreedenceOrLater;

            // Populate report treeview with report and subscription on the top of left panel
            PopulateTreeViewReportList();

            // Populate subscription treeview with subscription on the bottom of the left panel
            PopulateTreeViewSubscriptionList();
        }


        /// <summary>
        /// Update report treeView and subscription treeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChangeOK_Refresh(object sender, EventArgs e)
        {
            RetrieveSubscriptionCollection();

            if (_subscriptionCollection != null)
            {
                // Update subscription treeView must be before updating report treeView
                UpdateSubscriptionTreeView();
                UpdateReportTreeView();

                if (sender is WlbReportSubscriptionView)
                    subscriptionView1.RefreshSubscriptionView();
            }
        }


        private void treeViewReportList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewReportList.SelectedNode.Tag is WlbReportInfo report)
            {
                subscriptionView1.Visible = false;
                wlbReportView1.RefreshReportViewer(report);
                wlbReportView1.Visible = true;

                if (_runReport)
                {
                    wlbReportView1.RunReport();
                    _runReport = false;
                }
            }
            else if (treeViewReportList.SelectedNode.Tag is WlbReportSubscription subscription)
            {
                wlbReportView1.Visible = false;
                subscriptionView1.RefreshSubscriptionView(subscription);
                subscriptionView1.Visible = true;
            }
            else
                return;

            treeViewSubscriptionList.SelectedNode = null;
        }


        private void treeViewSubscriptionList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewSubscriptionList.SelectedNode.Tag is WlbReportSubscription subscription)
            {
                wlbReportView1.Visible = false;
                subscriptionView1.RefreshSubscriptionView(subscription);
                subscriptionView1.Visible = true;

                treeViewReportList.SelectedNode = null;
            }
        }


        /// <summary>
        /// Event handler for when a report is drilled in to from the MS Viewer inside the ReportView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wlbReportView1_ReportDrilledThrough(object sender, DrillthroughEventArgs e)
        {
            SynchTreeViewReportList(e.ReportPath.ToString());
        }

        private void wlbReportView1_Close(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }


        /// <summary>
        /// Event handler for a lost pool connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wlbReportView1_PoolConnectionLost(object sender, EventArgs e)
        {
            using (var dlg = new InformationDialog(String.Format(Messages.WLB_REPORT_POOL_CONNECTION_LOST, _pool.Name()))
                {WindowTitle = Messages.WLBREPORT_POOL_CONNECTION_LOST_CAPTION})
            {
                dlg.ShowDialog(this);
            }

            this.Close();
            this.Dispose();
        }

        #endregion
    }
}
