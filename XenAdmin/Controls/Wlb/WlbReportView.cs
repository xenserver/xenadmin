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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows;
using System.Xml;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.Wlb;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Help;
using System.Collections;

// Report viewer control dependencies
using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices;
using System.IO;



namespace XenAdmin.Controls.Wlb
{
    public partial class WlbReportView : UserControl
    {
       
        #region Variables

        public event CustomRefreshEventHandler OnChangeOK;
        public event DrillthroughEventHandler ReportDrilledThrough;
        public event BackEventHandler ReportBack;
        public event EventHandler Close;
        public event EventHandler PoolConnectionLost;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _bDisplayedError;
        private bool _resetReportViewer;
        private int _currentOffsetMinutes;
        private int _startLine;
        private int _endLine;
        private int _currentReportSection;

        // To mitigate the WLB server memory pressure to generate pool audit trail report
        // and decrease the latency to display the report results on XenCenter,
        // split large pool audit trail report in sections, each contains at most 200 pages.
        // For the first page, there are 43 lines of record.
        // For other pages, each contains 45 lines of record.
        private const int _lineLimit = 9000 - 2; 
        private bool _userComboSet;
        private bool _objectComboSet;
        private string _startTime = String.Empty;
        private string _endTime = String.Empty;
        private bool _isAuditReport;
        private bool _isCreedenceOrLater;

        private WlbReportInfo _reportInfo;
        private LocalReport _localReport;
        private IEnumerable<Host> _hosts;
        private Pool _pool;
        private List<string> _selectedCustomFilters; 
        private static string DELIMETER = ",";

        // for report subscription
        private Dictionary<string, string> _reportParameters;

        #endregion


        #region Properties

        public WlbReportInfo ViewerReportInfo
        {
            get { return _reportInfo; }
            set { _reportInfo = value; }
        }

        public LocalReport ViewerLocalReport
        {
            get { return _localReport; }
            set { _localReport = value; }
        }

        public Pool Pool
        {
            get { return _pool; }
            set { _pool = value; }
        }


        public bool ResetReportViewer
        {
            get { return _resetReportViewer; }
            set { _resetReportViewer = value; }
        }

        public IEnumerable<Host> Hosts
        {
            get { return _hosts; }
            set
            {
                _hosts = value;
                SetHostComboBox();
            }
        }

        public bool IsCreedenceOrLater
        {
            get { return _isCreedenceOrLater; }
            set { _isCreedenceOrLater = value; }
        }

        #endregion


        #region Constructor

        public WlbReportView()
        {
            InitializeComponent();
            FixSearchTextField(); //Fix for CA-59482: don't let the text length be more that 32 chars
            
            //CA-67888: For localizing export menu items
            ToolStrip toolStrip = (ToolStrip)reportViewer1.Controls.Find("toolStrip1", true)[0];
            ToolStripDropDownButton exportButton = (ToolStripDropDownButton)toolStrip.Items["export"];

            //Internally Microsoft ReportViewer populates "Export" ToolStripDropDownButton on DropDownOpening event.
            //That means DropDownOpening has two event hanlders including ours below. Although they'll be executed in the order they were added
            //for now -- so these localized Texts will replace old Texts -- the .NET Framework specification doesn't say anything about the
            //order of execution of event handlers. So although highly unlikely, the order may not be proper in the future.
            exportButton.DropDownOpening += (sender, e) =>
            {
                if (exportButton.DropDownItems.Count == 2)
                {
                    exportButton.DropDownItems[0].Text = Messages.FILE_XLS;
                    exportButton.DropDownItems[1].Text = Messages.FILE_PDF;
                }
            };
        }

        /// <summary>
        /// Sets the maximum number of characters allowed in the find textbox to 32
        /// </summary>
        private void FixSearchTextField()
        {
            const int maxLen = 32;
            ToolStrip toolStrip = (ToolStrip)reportViewer1.Controls.Find("toolStrip1", true)[0];
            //text box
            ToolStripTextBox findTextBox = (ToolStripTextBox)toolStrip.Items["textToFind"];
            findTextBox.MaxLength = maxLen;
            //find button
            ToolStripLabel findButton = (ToolStripLabel)toolStrip.Items["find"];
            findButton.Click += (s, e) =>
            {
                if (findTextBox.Text.Length > maxLen)
                {
                    findTextBox.Text = findTextBox.Text.Remove(maxLen);
                }
            };
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Implements all IReportViewerMessages to provide localization support to
        /// the report toolbar for tooltips and other misc items
        /// </summary>
        public class CCustomMessageClass : IReportViewerMessages
        {

            public string BackButtonToolTip
            {
                get { return (Messages.WLB_REPORT_BACKTOPARENTREPORT); }
            }

            public string BackMenuItemText
            {
                get { return (Messages.WLB_REPORT_BACKTOPARENTREPORT); }
            }

            public string ChangeCredentialsText
            {
                get { return (Messages.WLB_REPORT_CHANGECREDENTIALSTEXT); }
            }

            public string CurrentPageTextBoxToolTip
            {
                get { return (Messages.WLB_REPORT_CURRENTPAGETEXTBOXTOOLTIP); }
            }

            public string DocumentMapButtonToolTip
            {
                get { return (Messages.WLB_REPORT_DOCUMENTMAP); }
            }

            public string DocumentMapMenuItemText
            {
                get { return (Messages.WLB_REPORT_DOCUMENTMAP); }
            }

            public string ExportButtonToolTip
            {
                get { return (Messages.WLB_REPORT_EXPORT); }
            }

            public string ExportMenuItemText
            {
                get { return (Messages.WLB_REPORT_EXPORT); }
            }

            public string FalseValueText
            {
                get { return (Messages.WLB_REPORT_FALSEVALUETEXT); }
            }

            public string FindButtonText
            {
                get { return (Messages.WLB_REPORT_FIND); }
            }

            public string FindButtonToolTip
            {
                get { return (Messages.WLB_REPORT_FIND); }
            }

            public string FindNextButtonText
            {
                get { return (Messages.WLB_REPORT_NEXT); }
            }

            public string FindNextButtonToolTip
            {
                get { return (Messages.WLB_REPORT_FINDNEXT); }
            }

            public string FirstPageButtonToolTip
            {
                get { return (Messages.WLB_REPORT_FIRSTPAGE); }
            }

            public string LastPageButtonToolTip
            {
                get { return (Messages.WLB_REPORT_LASTPAGE); }
            }

            public string NextPageButtonToolTip
            {
                get { return (Messages.WLB_REPORT_NEXTPAGE); }
            }

            public string NoMoreMatches
            {
                get { return (Messages.WLB_REPORT_NOMOREMATCHES); }
            }

            public string NullCheckBoxText
            {
                get { return (Messages.WLB_REPORT_FALSEVALUETEXT); }
            }

            public string NullCheckBoxToolTip
            {
                get { return (Messages.WLB_REPORT_FALSEVALUETEXT); }
            }

            public string NullValueText
            {
                get { return (Messages.WLB_REPORT_FALSEVALUETEXT); }
            }

            public string PageOf
            {
                get { return (Messages.WLB_REPORT_PAGEOF); }
            }

            public string PageSetupButtonToolTip
            {
                get { return (Messages.WLB_REPORT_PAGESETUP); }
            }

            public string PageSetupMenuItemText
            {
                get { return (Messages.WLB_REPORT_PAGESETUP); }
            }

            public string ParameterAreaButtonToolTip
            {
                get { return (Messages.WLB_REPORT_PARAMETERS); }
            }

            public string PasswordPrompt
            {
                get { return (Messages.WLB_REPORT_PASSWORDPROMPT); }
            }

            public string PreviousPageButtonToolTip
            {
                get { return (Messages.WLB_REPORT_PREVIOUSPAGE); }
            }

            public string PrintButtonToolTip
            {
                get { return (Messages.WLB_REPORT_PRINT); }
            }

            public string PrintLayoutButtonToolTip
            {
                get { return (Messages.WLB_REPORT_PRINTLAYOUT); }
            }

            public string PrintLayoutMenuItemText
            {
                get { return (Messages.WLB_REPORT_PRINTLAYOUT); }
            }

            public string PrintMenuItemText
            {
                get { return (Messages.WLB_REPORT_PRINT); }
            }

            public string ProgressText
            {
                get { return (Messages.WLB_REPORT_PROCESSING); }
            }

            public string RefreshButtonToolTip
            {
                get { return (Messages.WLB_REPORT_REFRESH); }
            }

            public string RefreshMenuItemText
            {
                get { return (Messages.WLB_REPORT_REFRESH); }
            }

            public string SearchTextBoxToolTip
            {
                get { return (Messages.WLB_REPORT_SEARCH); }
            }

            public string SelectAValue
            {
                get { return (Messages.WLB_REPORT_SELECT); }
            }

            public string SelectAll
            {
                get { return (Messages.WLB_REPORT_SELECTALL); }
            }

            public string StopButtonToolTip
            {
                get { return (Messages.WLB_REPORT_STOPRENDERING); }
            }

            public string StopMenuItemText
            {
                get { return (Messages.WLB_REPORT_STOPRENDERING); }
            }

            public string TextNotFound
            {
                get { return (Messages.WLB_REPORT_TEXTNOTFOUND); }
            }

            public string TotalPagesToolTip
            {
                get { return (Messages.WLB_REPORT_TOTALPAGES); }
            }

            public string TrueValueText
            {
                get { return (""); }
            }

            public string UserNamePrompt
            {
                get { return (Messages.WLB_REPORT_USERNAME); }
            }

            public string ViewReportButtonText
            {
                get { return (Messages.WLB_REPORT_VIEWREPORT); }
            }

            public string ViewReportButtonToolTip
            {
                get { return (Messages.WLB_REPORT_VIEWREPORT); }
            }

            public string ZoomControlToolTip
            {
                get { return (Messages.WLB_REPORT_ZOOM); }
            }

            public string ZoomMenuItemText
            {
                get { return (Messages.WLB_REPORT_ZOOM); }
            }

            public string ZoomToPageWidth
            {
                get { return (Messages.WLB_REPORT_PAGEWIDTH); }
            }

            public string ZoomToWholePage
            {
                get { return (Messages.WLB_REPORT_WHOLEPAGE); }
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Run report
        /// </summary>
        public void ExecuteReport()
        {
            try
            {
                // Make sure the pool is okay
                if (!_pool.Connection.IsConnected)
                {
                    PoolConnectionLost(this, EventArgs.Empty);
                }
                else if (StartDatePicker.Value.CompareTo(EndDatePicker.Value) > 0)
                {
                    using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Warning,
                           Messages.WLB_REPORT_DATE_ORDERING_MESSAGE,
                           Messages.WLB_REPORT_DATE_ORDERING_CAPTION)))
                    {
                        dlg.ShowDialog(this);
                    }
                }
                else
                {

                    _bDisplayedError = false;

                    byte[] rdlBytes = Encoding.UTF8.GetBytes(_reportInfo.ReportDefinition);
                    System.IO.MemoryStream stream = new System.IO.MemoryStream(rdlBytes);

                    this.reportViewer1.LocalReport.LoadReportDefinition(stream);

                    _localReport = this.reportViewer1.LocalReport;
                    _localReport.DisplayName = _reportInfo.ReportName;

                    RunReport();

                    if (!_bDisplayedError)
                        this.reportViewer1.RefreshReport();

                    this.btnSubscribe.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       ex.Message,
                       Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
            }
        }


        /// <summary>
        /// Reset reportViewer after reportInfo is changed
        /// </summary>
        /// <param name="reportInfo">ReportInfo instance</param>
        public void SynchReportViewer(WlbReportInfo reportInfo)
        {
            if(reportInfo.ReportFile.StartsWith("pool_audit_history"))
            {
                _isAuditReport = true;
                InitializeAuditReport();
            }
            else
            {
                _isAuditReport = false;
            }

            this.btnLaterReport.Visible = false;
            this.ViewerReportInfo = reportInfo;

            if (ResetReportViewer == true)
                this.reportViewer1.Reset();

            ResetReportViewer = true;

            // Enable the run and disable subscribe buttons 
            this.btnRunReport.Enabled = true;
            this.btnLaterReport.Enabled = false;
            this.btnSubscribe.Enabled = false;

            // Set the run button text
            this.btnRunReport.Text = Messages.RUN_REPORT;

            // If host is a parameter for the selected report, show it
            if (reportInfo.DisplayHosts == true)
            {
                // Some serious hackage to get the correct host selected in the dropdown if the
                // parameter is being forced through other means (subreport or drillthrough)                      
                if (this.ViewerLocalReport != null)
                {
                    string currentHostID;

                    for (int i=0; i < this.ViewerLocalReport.OriginalParametersToDrillthrough.Count; i++)
                    {                         
                        if (this.ViewerLocalReport.OriginalParametersToDrillthrough[i].Name == "HostID")
                        {    
                            currentHostID = this.ViewerLocalReport.OriginalParametersToDrillthrough[i].Values[0];

                            for (int j = 0; j < this.hostComboBox.Items.Count; j++)
                            {
                                if (((Host)this.hostComboBox.Items[j]).uuid == currentHostID)
                                {
                                    this.hostComboBox.SelectedIndex = j;
                                    break;
                                }
                                    
                            }
                        }
                    }
                }

                // If none of the above worked out, we set it here                    
                if ((this.hostComboBox.SelectedIndex < 0) && (this.hostComboBox.Items.Count > 0))
                    this.hostComboBox.SelectedIndex = 0;

                // Set control items accordingly
                this.panelHosts.Visible = true;
            }
            else
            {
                // Host dropdown does not need to be displayed
                this.panelHosts.Visible = false;
            }

            if (reportInfo.DisplayFilter)
            {
                this.panelShow.Visible = true;
                SetViewComboBox(reportInfo.ReportFile);
                this.comboBoxView.SelectedIndex = 0;
            }
            else
            {
                this.panelShow.Visible = false;
            }


            if (reportInfo.DisplayUsers)
            {
                // User dropdown needs to be displayed
                this.panelUsers.Visible = true;
                if(!_userComboSet)
                {
                    // Fill user dropdown with user list retrieved from WLB server
                    string audit_users_string = reportInfo.ReportQueryParameterNames["AuditUser"].ToString();
                    string[] audit_users = audit_users_string.Length > 0 ? audit_users_string.Split(',') : (new string[0]);
                    Array.Sort(audit_users, StringComparer.InvariantCulture);
                    this.SetUserComboBox(audit_users);
                    _userComboSet = true;
                }

            }
            else
            {
                // Users dropdown does not need to be displayed
                this.panelUsers.Visible = false;
            }


            if (reportInfo.DisplayAuditObjects)
            {
                // Object dropdown needs to be displayed
                this.panelObjects.Visible = true;
                if(!_objectComboSet)
                {
                    // Fill object dropdown with object list retrieved from WLB server
                    string audit_objects_string = reportInfo.ReportQueryParameterNames["AuditObject"].ToString();
                    string[] audit_objects = audit_objects_string.Length > 0 ? audit_objects_string.Split(',') : (new string[0]);
                    Array.Sort(audit_objects, StringComparer.InvariantCulture);
                    this.SetObjectComboBox(audit_objects);
                    _objectComboSet = true;
                }

            }
            else
            {
                // Objects dropdown does not need to be displayed
                this.panelObjects.Visible = false;
            }


            this.Visible = true;
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Populates and configures date picker values
        /// </summary>
        private void SetPickerDateValues()
        {

            DateTime dt = DateTime.Now;
            EndDatePicker.Value = dt;

            dt = dt.AddDays(-6);
            StartDatePicker.Value = dt;

            StartDatePicker.Format = DateTimePickerFormat.Custom; // DateTimePickerFormat.Short;
            StartDatePicker.CustomFormat = Messages.DATEFORMAT_DMY; //"dd-MMM-yyyy";
            EndDatePicker.Format = DateTimePickerFormat.Custom; //.Short;
            EndDatePicker.CustomFormat = Messages.DATEFORMAT_DMY;
        }


        /// <summary>
        /// Populates the hosts drop down with hosts from the current pool
        /// </summary>        
        private void SetHostComboBox()
        {
            if (_hosts != null)
            {
                foreach (Host host in _hosts)
                {
                    hostComboBox.Items.Add(host);
                }
            }

            hostComboBox.Sorted = true;
        }

        /// <summary>
        /// Populates the users drop down with users retrieved from WLB server
        /// </summary>
        private void SetUserComboBox(string[] _users)
        {
            if (_users != null)
            {
                // Add ALL in dropdown to represent all users
                userComboBox.Items.Add(Messages.WLB_AUDIT_LOG_USER_OBJECT_ALL);

                foreach (string user in _users)
                {
                    userComboBox.Items.Add(user);
                }
            }

            userComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates the objects drop down with objects retrieved from WLB server
        /// </summary>
        private void SetObjectComboBox(string[] _objects)
        {
            if (_objects != null)
            {
                // Add ALL in dropdown to represent all objects
                objectComboBox.Items.Add(Messages.WLB_AUDIT_LOG_USER_OBJECT_ALL);

                foreach (string auditObject in _objects)
                {
                    objectComboBox.Items.Add(auditObject);
                }
            }

            objectComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates the hosts drop down with hosts from the current pool
        /// </summary>        
        private void SetViewComboBox(string reportFile)
        {
            this.comboBoxView.Items.Clear();

            if(_isAuditReport)
            {
                this.comboBoxView.Items.Add(Messages.WLB_REPORT_VIEW_BASIC);
                this.comboBoxView.Items.Add(Messages.WLB_REPORT_VIEW_VERBOSE);
            }
            else
            {
                this.comboBoxView.Items.Add(Messages.WLB_REPORT_VIEW_ALL);
                this.comboBoxView.Items.Add(Messages.WLB_REPORT_VIEW_CUSTOM);
            }
        }


        /// <summary>
        /// Sets the local report parameters, post-query.
        /// </summary>
        private void SetReportParameters()
        {
            try
            {
                ReportParameterInfoCollection currentParams = _localReport.GetParameters();
                ReportParameter rpCurrentParam;
                string paramValue = String.Empty;
                bool addParam = false;
                if (this._reportParameters == null)
                    this._reportParameters = new Dictionary<string, string>();
                else
                    this._reportParameters.Clear();

                foreach (ReportParameterInfo rp in currentParams)
                {
                    addParam = false;
                    switch (rp.Name)
                    {
                        case "LocaleCode":
                            paramValue = currentParams["LocaleCode"].Values.Count == 0 ? Program.CurrentLanguage : currentParams["LocaleCode"].Values[0];
                            addParam = true; 
                            break;

                        case "Start":
                            paramValue = currentParams["Start"].Values.Count == 0 ? GetDateOffset(HelpersGUI.DateTimeToString(StartDatePicker.Value, Messages.DATEFORMAT_DMY, false)) : currentParams["Start"].Values[0];
                            addParam = true;
                            break;

                        case "End":
                            paramValue = currentParams["End"].Values.Count == 0 ? GetDateOffset(HelpersGUI.DateTimeToString(EndDatePicker.Value, Messages.DATEFORMAT_DMY, false)) : currentParams["End"].Values[0];    
                            addParam = true;
                            break;

                        case "StartTime":
                            paramValue = currentParams["StartTime"].Values.Count == 0 ? GetDateOffset(HelpersGUI.DateTimeToString(StartDatePicker.Value, Messages.DATEFORMAT_DMY, false)) : currentParams["StartTime"].Values[0];
                            // For audit report enhancement, need to set time in seconds
                            if(String.IsNullOrEmpty(_startTime))
                            {
                                int offSet = Convert.ToInt32(paramValue);
                                // One day is 86400000 milliseconds or 86400 seconds
                                // Set start time as 00:00:00 of the day
                                paramValue = ((Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds) / 86400000 + offSet) * 86400).ToString();
                                _startTime = paramValue;                                    
                            }
                            else
                            {
                                // To retrieve more report of pool audit trail,
                                // use the already set _startTime
                                paramValue = _startTime;
                            }

                            addParam = true;
                            break;

                        case "EndTime":
                            paramValue = currentParams["EndTime"].Values.Count == 0 ? GetDateOffset(HelpersGUI.DateTimeToString(EndDatePicker.Value, Messages.DATEFORMAT_DMY, false)) : currentParams["EndTime"].Values[0];    
                            // For audit report enhancement, need to set time in seconds
                            if(String.IsNullOrEmpty(_endTime))
                            {
                                if(paramValue == "0")
                                {
                                    // If today is selected, set the current time in seconds
                                    paramValue = Convert.ToInt64(((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds) / 1000).ToString();
                                }
                                else
                                {
                                    int offSet = Convert.ToInt32(paramValue) + 1;
                                    // One day is 86400000 milliseconds or 86400 seconds
                                    // Set end time as 23:59:59 of the day
                                    paramValue = (((Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds) / 86400000 + offSet) * 86400) - 1).ToString();
                                }
                                _endTime = paramValue;

                            }
                            else
                            {
                                // To retrieve more report of pool audit trail,
                                // use the already set _endTime
                                paramValue = _endTime;
                            }

                            addParam = true;
                            break;

                        case "GroupBy1":
                            paramValue = currentParams["GroupBy1"].Values.Count == 0 ? "day" : currentParams["GroupBy1"].Values[0];
                            addParam = true;
                            break;

                        case "TopN":
                            paramValue = currentParams["TopN"].Values.Count == 0 ? "100000" : currentParams["TopN"].Values[0];
                            addParam = true;
                            break;

                        case "PoolID":
                            paramValue = currentParams["PoolID"].Values.Count == 0 ? _pool.uuid : currentParams["PoolID"].Values[0];
                            addParam = true;
                            break;

                        case "PoolName":
                            paramValue = currentParams["PoolName"].Values.Count == 0 ? Helpers.GetName(_pool) : currentParams["PoolName"].Values[0];
                            addParam = true;
                            break;

                        case "HostID":
                            paramValue = currentParams["HostID"].Values.Count == 0 ? ((Host)hostComboBox.SelectedItem).uuid : currentParams["HostID"].Values[0];
                            addParam = true;
                            break;

                        case "HostName":
                            paramValue = currentParams["HostName"].Values.Count == 0 ? ((Host)hostComboBox.SelectedItem).name_label : currentParams["HostName"].Values[0];
                            addParam = true;
                            break;

                        case "Filter":
                            paramValue = currentParams["Filter"].Values.Count == 0 ?
                                (_selectedCustomFilters != null && _selectedCustomFilters.Count > 0 ? String.Join(DELIMETER, _selectedCustomFilters.ToArray()) : comboBoxView.SelectedIndex.ToString()) : 
                                currentParams["Filter"].Values[0];
                            addParam = true;
                            break;

                        case "UTCOffset":
                            paramValue = currentParams["UTCOffset"].Values.Count == 0 ? _currentOffsetMinutes.ToString(): currentParams["UTCOffset"].Values[0];
                            addParam = true;
                            break;

                        case "AuditUser":
                            if (userComboBox.SelectedIndex == 0)
                            {
                                // The first item is ALL in localized characters,
                                // set it as "ALL" so WLB server could recognize.
                                paramValue = "ALL";
                            }
                            else
                            {
                                paramValue = currentParams["AuditUser"].Values.Count == 0 ? userComboBox.SelectedItem.ToString(): currentParams["AuditUser"].Values[0];
                            }
                            addParam = true;
                            break;

                        case "AuditObject":
                            if(objectComboBox.SelectedIndex == 0)
                            {
                                // The first item is ALL in localized characters,
                                // set it as "ALL" so WLB server could recognize.
                                paramValue = "ALL";
                            }
                            else
                            {
                                paramValue = currentParams["AuditObject"].Values.Count == 0 ? objectComboBox.SelectedItem.ToString(): currentParams["AuditObject"].Values[0];
                            }
                            addParam = true;
                            break;

                        case "StartLine":
                            paramValue = _startLine.ToString();
                            addParam = true;
                            break;

                        case "EndLine":
                            paramValue = _endLine.ToString();
                            addParam = true;
                            break;

                        case "ReportVersion":
                            paramValue = "Creedence";
                            addParam = true;
                            break;

                    }
                    if (addParam)
                    {
                        rpCurrentParam = new ReportParameter(rp.Name, paramValue);
                        _localReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter[] { rpCurrentParam });
                        _reportParameters.Add(rp.Name, paramValue);
                    }
                }
            }
            catch(LocalProcessingException ex)
            {
                log.Debug(ex, ex);
                throw new Exception(String.Format(Messages.WLB_REPORT_SET_PARAMS, ex.InnerException.InnerException.Message));
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
                throw new Exception(String.Format(Messages.WLB_REPORT_SET_PARAMS, ex.Message));
            }
        }


        /// <summary>
        /// Sets the date offset to an integer for UTC offset handling
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns>string representing the offset from the current date</returns>
        private string GetDateOffset(string dateValue)
        {
            DateTime passedDate = DateTime.Parse(dateValue, CultureInfo.InvariantCulture);
            TimeSpan diff = passedDate - DateTime.Today;
            int diffDays = diff.Days;
            return diffDays.ToString();

        }


        /// <summary>
        /// Invokes and executes a call to the Kirkwood database via Xapi to obtain report data.
        /// </summary>
        /// <param name="reportKey"></param>
        /// <param name="currentParams"></param>
        /// <returns></returns>
        private string GetReportData(string reportKey, ReportParameterInfoCollection currentParams)
        {
            string returnValue = string.Empty;
            List<string> reportQueryParams;

            reportQueryParams = new List<String>();
            ICollection keyCollections = _reportInfo.ReportQueryParameterNames.Keys;
            int cnt = _reportInfo.ReportQueryParameterNames.Count;
            String[] paramKeys = new String[cnt];
            keyCollections.CopyTo(paramKeys, 0);
            
            for (int i = 0; i <cnt; i++)
            {
                reportQueryParams.Add(paramKeys[i]);
            }

            reportQueryParams.Add("UTCOffset");
            reportQueryParams.Add("LocaleCode");

            Dictionary<string, string> parms = new Dictionary<string, string>();

            for (int i = 0; i < reportQueryParams.Count; i++)
            {
                parms[reportQueryParams[i].ToString()] = currentParams[reportQueryParams[i].ToString()].Values[0].ToString();
            }

            AsyncAction a = new WlbReportAction(Pool.Connection, 
                                                Helpers.GetMaster(Pool.Connection),
                                                reportKey, 
                                                _reportInfo.ReportName, 
                                                false,
                                                parms);
            using (var dlg = new ActionProgressDialog(a, ProgressBarStyle.Marquee))
                dlg.ShowDialog();

            if (a.Succeeded)
            {
                returnValue = a.Result;
            }
            else
            {
                _bDisplayedError = true;
            }

            return returnValue;

        }


        /// <summary>
        /// Responsible for marshalling the string data returned back from Xapi->Kirkwood into XML and 
        /// then into a dataset object.  Once the dataset is obtained, set the rdlc path and then bind a local
        /// datasource to the datasource container within the rdlc. 
        /// </summary>
        private void PopulateReportData()
        {
            string reportLabelNames = "None";
            string reportLabelValues = "None";
            string reportKey = _reportInfo.ReportFile.Replace(".rdlc", "");

            DataSet reportDS = new DataSet();

            string xmlData = GetReportData(reportKey, _localReport.GetParameters());

            try
            {
                if ((!String.IsNullOrEmpty(xmlData)) && (!_bDisplayedError))
                {

                    // Read the XML document into the DataSet.  
                    System.IO.StringReader xmlSR = new System.IO.StringReader(xmlData);
                    reportDS.ReadXml(xmlSR, XmlReadMode.ReadSchema);
                    xmlSR.Close();

                    // Create a Data Source and add it to reportviewer.  This is to allow the RDLC fields to bind
                    ReportDataSource reportDataSource1 = new ReportDataSource();
                    reportDataSource1.Name = "KirkwoodDBDataSetLocal";
                    reportDataSource1.Value = reportDS.Tables[0];
                    _localReport.DataSources.Add(reportDataSource1);

                    if(_isAuditReport && _isCreedenceOrLater)
                    {
                        int cnt = reportDS.Tables[0].Rows.Count;

                        if (cnt >= _lineLimit)
                        {
                            // If audit trail report from WLB server exceeds the line limit,
                            // change the run button text to "More",
                            // display the label to indicate there's more report.

                            this.btnRunReport.Text = Messages.FETCH_EARLIER_DATA;
                            this.btnRunReport.Enabled = true;

                        }
                        else
                        {
                            // If audit trail report from WLB server doesn't exceed the line limit,
                            // and this is the first section of report,
                            // change the run button text back to "Run Report",
                            // or disable the run report button and just keep the later report button.
                            if(btnLaterReport.Visible)
                            {
                                this.btnRunReport.Enabled = false;
                            }
                            else
                            {
                                this.btnRunReport.Text = Messages.RUN_REPORT;
                            }
                            
                        }

                        if(_currentReportSection > 1)
                        {
                            this.btnLaterReport.Visible = true;
                            this.btnLaterReport.Enabled = true;
                            if (!this.btnRunReport.Enabled)
                            {
                                this.btnLaterReport.Select();
                            }
                        }
                        else if (_currentReportSection == 1 && 
                                 this.btnRunReport.Text == Messages.FETCH_EARLIER_DATA)
                        {
                            this.btnLaterReport.Visible = true;
                            this.btnLaterReport.Enabled = false;                            
                            this.btnRunReport.Select();
                        }
                        else
                        {
                            this.btnLaterReport.Visible = false;
                            this.btnLaterReport.Enabled = false;
                        }
                    }
                    

                    // Set the report label names and values
                    if (reportDS.Tables[1].Rows.Count > 0)
                    {
                        DataRow dr = reportDS.Tables[1].Rows[0];
                        reportLabelNames = dr["name"].ToString();
                        reportLabelValues = dr["description"].ToString();
                    }

                    PopulateReportLabelData(reportLabelNames, reportLabelValues);
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
                throw new Exception(String.Format(Messages.WLB_REPORT_BIND_DATASOURCE, ex.Message));
            }
        }


        /// <summary>
        /// Jams labels and values into two separate parameters that is extracted programatically within 
        /// the rdlc code block.
        /// </summary>
        /// <param name="reportLabelNames"></param>
        /// <param name="reportLabelValues"></param>
        private void PopulateReportLabelData(string reportLabelNames, string reportLabelValues)
        {
            try
            {
                // Set the parameter values
                Microsoft.Reporting.WinForms.ReportParameter rpParamLabels = new Microsoft.Reporting.WinForms.ReportParameter("ParamLabels", reportLabelNames);
                Microsoft.Reporting.WinForms.ReportParameter rpParamValues = new Microsoft.Reporting.WinForms.ReportParameter("ParamValues", reportLabelValues);

                // Add report parameters array
                _localReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter[] { 
                    rpParamLabels,
                    rpParamValues});
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
                throw new Exception(String.Format(Messages.WLB_REPORT_ERROR_LOCALIZED_PARAMS, ex.Message));
            }
        }


        /// <summary>
        /// Performs common report execution steps such as populating datasets, labels and defaults
        /// </summary>
        private void RunReport()
        {
            try
            {
                // Event handler for SubreportProcessing
                _localReport.SubreportProcessing += new SubreportProcessingEventHandler(reportViewer1SubReportEventHandler);

                // Set the parameters
                SetReportParameters();

                // Bind the report to a datasource and set label values
                PopulateReportData();

                // Go ahead and show the toolbar now that a report has been executed
                CCustomMessageClass customMessages = new CCustomMessageClass();
                this.reportViewer1.Messages = customMessages;

                this.reportViewer1.ShowToolBar = true;

            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
                throw new Exception(ex.Message);
            }
        }

        #endregion


        #region Event Handler

        /// <summary>
        /// Clean up tool tip for Host combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(this);
        }
        /// <summary>
        /// Owner Draw for Host combobox, showing tooltips.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox thisComboBox = (ComboBox)sender;

                //CA-85278: XenCenter/WLB | XenCenter error when click on Pool Audit Trail report
                //  Be sure we have a valid item index
                if ((e.Index > -1) &&
                    (thisComboBox.Items.Count > e.Index))
                {
                    string text = thisComboBox.GetItemText(thisComboBox.Items[e.Index]);
                    e.DrawBackground();
                    using (SolidBrush br = new SolidBrush(e.ForeColor))
                    {
                        e.Graphics.DrawString(text, e.Font, br, e.Bounds);
                    }
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && thisComboBox.DroppedDown)
                    {
                        toolTip1.Show(text, thisComboBox, e.Bounds.Right, e.Bounds.Bottom);
                    }
                    e.DrawFocusRectangle();
                }
            }
        }

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportView_Load(object sender, EventArgs e)
        {
            this.btnRunReport.Enabled = false;
            this.btnLaterReport.Enabled = false;
            this.btnSubscribe.Enabled = false;

            this.hostComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Hide the toolbar until a report is executed
            this.reportViewer1.ShowToolBar = false;

            SetPickerDateValues();

            // Hosts dropdown and label is invisible until we need it
            this.panelShow.Visible = false;
            this.panelHosts.Visible = false;

            // Users and Objects dropdowns and labels are invisible until we need it
            this.panelUsers.Visible = false;
            this.panelObjects.Visible = false;

            // Set UTC Offset
            TimeZone tz = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;

            int currentOffsetHours = tz.GetUtcOffset(currentDate).Hours;
            int currentOffsetMinutes = tz.GetUtcOffset(currentDate).Minutes;
            _currentOffsetMinutes = (currentOffsetHours * 60) + (currentOffsetMinutes);
                        
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentDrillthroughEventArgs"></param>
        private void OnReportDrilledThrough(DrillthroughEventArgs currentDrillthroughEventArgs)
        {
            if (ReportDrilledThrough != null)
            {
                ReportDrilledThrough(this, currentDrillthroughEventArgs);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentBackEventArgs"></param>
        private void OnReportBack(BackEventArgs currentBackEventArgs)
        {
            if (ReportBack != null)
            {
                ReportBack(this, currentBackEventArgs);
            }
        }


        /// <summary>
        /// Event handler for the "Run Report"/"Next Section" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void btnRunReport_Click(object sender, EventArgs e)
        {
            if(_isAuditReport && _isCreedenceOrLater)
            {
                // If the button displays "Run Report",
                // that means the 1st report section is requested.
                // Or else the button text is "Next Section",
                // next report section is requested.

                if(this.btnRunReport.Text == Messages.RUN_REPORT)
                {
                    _currentReportSection = 1;
                    _startLine = 1;
                    _endLine = _lineLimit;
                    _endTime = String.Empty;
                }
                else
                {
                    _currentReportSection += 1;
                    _startLine += _lineLimit;
                    _endLine += _lineLimit;
                }
            }
            this.reportViewer1.Reset();
            this.ExecuteReport();
        }


        /// <summary>
        /// Event handler for the "Previous Section" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void btnLaterReport_Click(object sender, EventArgs e)
        {
            if(_isAuditReport && _isCreedenceOrLater)
            {
                // The previous report section is requested.

                _currentReportSection -= 1;
                _startLine -= _lineLimit;
                _endLine -= _lineLimit;
            }
            this.reportViewer1.Reset();
            this.ExecuteReport();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            // Make sure the pool is okay
            if (!_pool.Connection.IsConnected)
            {
                PoolConnectionLost(this, EventArgs.Empty);
            }
            else
            {
                if (this._reportParameters.ContainsKey("reportName"))
                {
                    this._reportParameters.Remove("reportName");
                }
                this._reportParameters.Add("reportName", this._reportInfo.ReportFile.Split('.')[0]);
                WlbReportSubscriptionDialog rpSubDialog = new WlbReportSubscriptionDialog(this._reportInfo.ReportName, this._reportParameters, _pool);
                if (rpSubDialog.ShowDialog() == DialogResult.OK)
                {
                    OnChangeOK(this, e);
                }
            }
        }


        /// <summary>
        /// Handles the render subreport event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportViewer1SubReportEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            string reportPath = e.ReportPath.Remove(0, _localReport.ReportPath.LastIndexOf("\\") + 1);
        }


        /// <summary>
        /// Handles drilldown report events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportViewer1_Drillthrough(object sender, DrillthroughEventArgs e)                        
        {

            _localReport = (LocalReport)e.Report;

            OnReportDrilledThrough(e);

            byte[] rdlBytes = Encoding.UTF8.GetBytes(_reportInfo.ReportDefinition);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(rdlBytes);
            _localReport.LoadReportDefinition(stream);

            RunReport();

            if (!_bDisplayedError)
                _localReport.Refresh(); 

        }


        /// <summary>
        /// Handles back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportViewer1_Back(object sender, BackEventArgs e)
        {
            _localReport = (LocalReport)e.ParentReport;
            OnReportBack(e);
        }


        /// <summary>
        /// Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (Close != null)
            {
                Close(this, e);
            }
        }


        private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedCustomFilters = null;

            if (this.comboBoxView.SelectedItem.ToString() == Messages.WLB_REPORT_VIEW_CUSTOM)
            {
                if (this.comboBoxView.SelectedIndex == 1)
                {
                    Dialogs.Wlb.WlbReportCustomFilter customFilterDialog = new Dialogs.Wlb.WlbReportCustomFilter(_pool.Connection);
                    customFilterDialog.InitializeFilterTypeIndex();
                    if (customFilterDialog.ShowDialog() == DialogResult.OK)
                    {
                        _selectedCustomFilters = customFilterDialog.GetSelectedFilters();
                        this.btnSubscribe.Enabled = false;
                        this.reportViewer1.Reset();
                        this.ExecuteReport();
                    }
                    else
                    {
                        this.comboBoxView.SelectedIndex = 0;
                    }
                }
                else
                {
                    this.btnSubscribe.Enabled = false;
                    this.reportViewer1.Reset();
                }
            }
        }

         delegate byte[] ReportExporterDelgate(string fileExtension);

        private void reportViewer1_ReportExport(object sender, ReportExportEventArgs e)
        {
            //cancel the default process
            e.Cancel = true;

            //the dropdown menu wasn't hiding itself
            //so this hack was needed to hide the menu
            //find the export button
            ToolStrip toolStrip = (ToolStrip)reportViewer1.Controls.Find("toolStrip1", true)[0];

            ToolStripDropDownButton exportButton = (ToolStripDropDownButton)toolStrip.Items["export"];
            exportButton.DropDown.Close();

            reportViewer1.Cursor = Cursors.WaitCursor;

            //and run our own code to export

            ExportReportAction action = new ExportReportAction(e.Extension.Name, ref reportViewer1);
            using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dlg.ShowDialog();

            //ReportExporterDelgate exp = new ReportExporterDelgate(RunExportReport);


            //IAsyncResult result = exp.BeginInvoke(e.Extension.Name, null, null);

            ////new ActionProgressDialog(result, ProgressBarStyle.Marquee).ShowDialog();

            //result.AsyncWaitHandle.WaitOne();

            //byte[] bytes = exp.EndInvoke(result);

            reportViewer1.Cursor = Cursors.Default;

            string fileExtension = "";
            if (e.Extension.Name == "Excel")
                fileExtension = "xls";
            else if (e.Extension.Name == "PDF")
                fileExtension = "pdf";
            
            SaveFileDialog sfDialog = new SaveFileDialog();
            
            sfDialog.Filter = (fileExtension == "pdf" ? Messages.FILE_PDF + "|*.pdf|" : Messages.FILE_XLS + "|*.xls|")
                + Messages.FILE_ALL + "|*.*";

            sfDialog.AddExtension = true;
            if (sfDialog.ShowDialog() == DialogResult.OK)
            {                
                FileStream fs = null;
                try
                {
                    fs = new FileStream(sfDialog.FileName, FileMode.Create);
                    fs.Write(action.ReportData, 0, action.ReportData.Length);
                    lblExported.Text = Messages.WLBREPORT_EXPORT_SUCC;
                    lblExported.Visible = true;                    
                    timer1.Start();
                }
                catch (Exception ex)
                {
                    log.Debug(ex, ex);
                    using (var dlg = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Error,
                           ex.Message,
                           Messages.XENCENTER)))
                    {
                        dlg.ShowDialog(this);
                    }
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }

            //bytes = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            lblExported.Visible = false;
        }


        #endregion //Event Handler

        private void comboBox_SelectionChanged(object sender, EventArgs e)
        {
            if(_isAuditReport)
            {
                InitializeAuditReport();
            }
        }

        private void InitializeAuditReport()
        {
            _startTime = String.Empty;
            _endTime = String.Empty;
            _startLine = 1;
            _endLine = _lineLimit;
            _currentReportSection = 0;
            btnRunReport.Text = Messages.RUN_REPORT;
            btnRunReport.Enabled = true;
            btnLaterReport.Visible = false;
        }

    }
}
