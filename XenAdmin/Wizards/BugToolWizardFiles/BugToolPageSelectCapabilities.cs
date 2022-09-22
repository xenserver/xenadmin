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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenCenterLib;


namespace XenAdmin.Wizards.BugToolWizardFiles
{
    public partial class BugToolPageSelectCapabilities : XenTabPage
    {
        #region Private fields
        private List<Host> _hostList = new List<Host>();
        private List<Capability> _capabilities = new List<Capability>();

        private Dictionary<Host, List<Capability>> hostCapabilities;
        private List<GetSystemStatusCapabilities> actions;
        private ActionProgressDialog dialog;
        private bool cancelled;
        #endregion

        public BugToolPageSelectCapabilities()
        {
            InitializeComponent();
            linkLabel1.Text = string.Format(linkLabel1.Text, BrandManager.CompanyNameShort);
            linkLabel1.Visible = !HiddenFeatures.LinkLabelHidden;
        }

        public override string Text => Messages.BUGTOOL_PAGE_CAPABILITIES_TEXT;

        public override string PageTitle => Messages.BUGTOOL_PAGE_CAPABILITIES_PAGETITLE;

        public override string HelpID => "SelectReportContents";

        public override bool EnableNext()
        {
            return wizardCheckAnyChecked();
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                ClearButton.Enabled = wizardCheckAnyChecked();
                SelectButton.Enabled = wizardCheckAnyUnchecked();
            }
        }

        private bool wizardCheckAnyChecked()
        {
            foreach (DataGridViewRow row in dataGridViewItems.Rows)
            {
                var capRow = row as CapabilityRow;
                if (capRow != null && capRow.Capability.Checked)
                    return true;
            }
            return false;
        }

        private bool wizardCheckAnyUnchecked()
        {
             foreach (DataGridViewRow row in dataGridViewItems.Rows)
             {
                 var capRow = row as CapabilityRow;
                 if (capRow != null && !capRow.Capability.Checked)
                     return true;
             }
             return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HostList"></param>
        /// <returns>If success of not - false with stop moving to next page</returns>
        public bool GetCommonCapabilities(List<Host> HostList)
        {
            if (!cancelled && Helpers.ListsContentsEqual<Host>(_hostList, HostList))
                return true;

            _hostList = HostList;
            dataGridViewItems.Rows.Clear();

            if (dialog != null && dialog.Visible)
                dialog.Close();

            hostCapabilities = new Dictionary<Host, List<Capability>>();
            actions = new List<GetSystemStatusCapabilities>();
            cancelled = false;
            dialog = new ActionProgressDialog(Messages.SERVER_STATUS_REPORT_GET_CAPABILITIES);
            dialog.ShowCancel = true;
            dialog.CancelClicked += new EventHandler(dialog_CancelClicked);

            foreach (Host host in _hostList)
            {
                GetSystemStatusCapabilities action = new GetSystemStatusCapabilities(host);
                actions.Add(action);
                action.Completed += Common_action_Completed;
                action.RunAsync();
            }

            if (_hostList.Count > 0)
                dialog.ShowDialog(this);

            return !cancelled;
        }

        private void dialog_CancelClicked(object sender, EventArgs e)
        {
            dialog.Close();
            cancelled = true;

            // Need to unhook events on current actions, to stop them
            // interfering when they return in 20mins
            foreach (GetSystemStatusCapabilities action in actions)
                action.Completed -= Common_action_Completed;
        }

        private void Common_action_Completed(ActionBase sender)
        {
            Program.Invoke(this, delegate()
            {
                if (cancelled)
                    return;

                if (!(sender is GetSystemStatusCapabilities action))
                    return;

                hostCapabilities[action.Host] = action.Succeeded ? ParseXmlToList(action.Result) : null;

                if (hostCapabilities.Count >= _hostList.Count)
                {
                    dialog.Close();
                    CombineCapabilitiesLists();
                    BuildList();
                }
            });
        }

        private static List<Capability> ParseXmlToList(string xml)
        {
            List<Capability> capabilities = new List<Capability>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode node in doc.GetElementsByTagName("capability"))
            {
                if (node.Attributes == null)
                    continue;

                Capability c = new Capability();
                foreach (XmlAttribute a in node.Attributes)
                {
                    string name = a.Name;
                    string value = a.Value;

                    if (name == "content-type")
                        c.ContentType = value == "text/plain" ? ContentType.text_plain : ContentType.application_data;
                    else if (name == "default-checked")
                        c.DefaultChecked = value == "yes";
                    else if (name == "key")
                        c.Key = value;
                    else if (name == "max-size")
                        c.MaxSize = long.Parse(value);
                    else if (name == "min-size")
                        c.MinSize = long.Parse(value);
                    else if (name == "pii")
                    {
                        switch (value)
                        {
                            case "yes":
                                c.PII = PrivateInformationIncluded.yes;
                                break;
                            case "no":
                                c.PII = PrivateInformationIncluded.no;
                                break;
                            case "maybe":
                                c.PII = PrivateInformationIncluded.maybe;
                                break;
                            default:
                                c.PII = PrivateInformationIncluded.customized;
                                break;
                        }
                    }
                }

                capabilities.Add(c);
            }

            return capabilities;
        }

        private void CombineCapabilitiesLists()
        {
            List<Capability> combination = null;

            foreach (var kvp in hostCapabilities)
            {
                var host = kvp.Key;
                var capabilities = kvp.Value;

                if (capabilities == null)
                    continue;

                if (Helpers.FeatureForbidden(host, Host.RestrictVtpm))
                    capabilities.RemoveAll(c => c.Key.ToLower() == "vtpm");

                combination = combination == null
                    ? capabilities
                    : combination.Intersect(capabilities).ToList();
            }

            if (combination == null || combination.Count <= 0)
            {
                using (var dlg = new ErrorDialog(Messages.SERVER_STATUS_REPORT_CAPABILITIES_FAILED))
                    dlg.ShowDialog(this);

                cancelled = true;
                return;
            }
                
            _capabilities = combination;
            _capabilities.Add(GetClientCapability());
            _capabilities.Sort(CapabilityComparer);
        }

        private static Capability GetClientCapability()
        {
            var logSize = GetClientLogSize();

            Capability clientCap = new Capability
            {
                ContentType = ContentType.text_plain,
                DefaultChecked = true,
                Key = "client-logs",
                MaxSize = logSize,
                MinSize = logSize,
                PII = PrivateInformationIncluded.yes
            };

            return clientCap;
        }

        private int CapabilityComparer(Capability obj1, Capability obj2)
        {
            var result = obj1.PII.CompareTo(obj2.PII);
            
            if (result == 0)
                result = obj1.CompareTo(obj2);
            
            return result;
        }

        private void OnCheckedCapabilitiesChanged()
        {
            string totalSize;
            CalculateTotalSize(out totalSize);
            TotalSizeValue.Text = totalSize;
            ClearButton.Enabled = wizardCheckAnyChecked();
            SelectButton.Enabled = wizardCheckAnyUnchecked();
            OnPageUpdated();
        }

        private void BuildList()
        {
            try
            {
                dataGridViewItems.SuspendLayout();
                dataGridViewItems.Rows.Clear();

                var list = new List<DataGridViewRow>();
                foreach (Capability c in _capabilities)
                    list.Add(new CapabilityRow(c));

                dataGridViewItems.Rows.AddRange(list.ToArray());
            }
            finally
            {
                dataGridViewItems.ResumeLayout();
            }

            OnCheckedCapabilitiesChanged();
        }

        private static long GetClientLogSize()
        {
            String path = Program.GetLogFile();
            if (path != null)
            {
                // Size of XenCenter.log
                FileInfo logFileInfo = new FileInfo(path);
                long total = logFileInfo.Length;

                // Add size of any XenCenter.log.x
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(path));
                Regex regex = new Regex(Regex.Escape(Path.GetFileName(path)) + @"\.\d+");
                foreach (FileInfo file in di.GetFiles())
                {
                    if (regex.IsMatch(file.Name))
                        total += file.Length;
                }
                return total;
            }
            else
            {
                return 50 * Util.BINARY_MEGA;
            }
        }

        private void CalculateTotalSize(out string totalSize)
        {
            var sizeMinList = new List<long>();
            var sizeMaxList = new List<long>();

            foreach (var row in dataGridViewItems.Rows)
            {
                var capRow = row as CapabilityRow;
                if (capRow != null && capRow.Capability.Checked)
                {
                    var c = capRow.Capability;
                    int m = c.Key == "client-logs" ? 1 : _hostList.Count;

                    sizeMinList.Add(c.MinSize * m);
                    sizeMaxList.Add(c.MaxSize * m);
                }
            }
            totalSize = Helpers.StringFromMaxMinSizeList(sizeMinList, sizeMaxList);
        }

        public List<Capability> CheckedCapabilities =>
            (from DataGridViewRow row in dataGridViewItems.Rows
                let capRow = row as CapabilityRow
                where capRow != null && capRow.Capability.Checked
                select capRow.Capability).ToList();

        #region Control event handlers

        private void dataGridViewItems_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            var row1 = dataGridViewItems.Rows[e.RowIndex1] as CapabilityRow;
            var row2 = dataGridViewItems.Rows[e.RowIndex2] as CapabilityRow;

            if (row1 != null && row2 != null && e.Column.Index == columnImage.Index)
            {
                e.SortResult = row1.Capability.PII.CompareTo(row2.Capability.PII);
                e.Handled = true;
            }
        }

        private void dataGridViewItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewItems.SelectedRows.Count > 0)
            {
                var row = dataGridViewItems.SelectedRows[0] as CapabilityRow;
                if (row == null)
                    return;

                DescriptionValue.Text = row.Capability.Description;
                SizeValue.Text = row.Capability.EstimatedSize;
            }
        }

        private void dataGridViewItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != columnCheck.Index)
                return;
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewItems.RowCount)
                return;

            var row = dataGridViewItems.Rows[e.RowIndex] as CapabilityRow;
            if (row == null)
                return;

            row.Capability.Checked = !row.Capability.Checked;
            row.Update();
            OnCheckedCapabilitiesChanged();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(InvisibleMessages.PRIVACY);
            }
            catch
            {
                using (var dlg = new ErrorDialog(Messages.HOMEPAGE_ERROR_MESSAGE))
                    dlg.ShowDialog(this);
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewItems.Rows)
            {
                var capRow = row as CapabilityRow;
                if (capRow != null && !capRow.Capability.Checked)
                {
                    capRow.Capability.Checked = true;
                    capRow.Update();
                }
            }
            OnCheckedCapabilitiesChanged();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewItems.Rows)
            {
                var capRow = row as CapabilityRow;
                if (capRow != null && capRow.Capability.Checked)
                {
                    capRow.Capability.Checked = false;
                    capRow.Update();
                }
            }
            OnCheckedCapabilitiesChanged();
        }

        #endregion

        private class CapabilityRow : DataGridViewRow
        {
            public readonly Capability Capability;

            private readonly DataGridViewCheckBoxCell cellCheck = new DataGridViewCheckBoxCell();
            private readonly DataGridViewTextBoxCell cellItem = new DataGridViewTextBoxCell();
            private readonly DataGridViewImageCell cellImage = new DataGridViewImageCell();

            public CapabilityRow(Capability capability)
            {
                Capability = capability;
                Cells.AddRange(cellCheck, cellItem, cellImage);
                Update();
            }

            public void Update()
            {
                cellCheck.Value = Capability.Checked;
                cellItem.Value = Capability.Name;

                switch(Capability.PII)
                {
                    case PrivateInformationIncluded.maybe:
                        cellImage.Value = Images.StaticImages.alert2_16;
                        cellImage.ToolTipText = Messages.PII_MAYBE;
                        break;
                    case PrivateInformationIncluded.customized:
                        cellImage.Value = Images.StaticImages.alert3_16;
                        cellImage.ToolTipText = Messages.PII_CUSTOMISED;
                        break;
                    case PrivateInformationIncluded.no:
                        cellImage.Value = Images.StaticImages.alert4_16;
                        cellImage.ToolTipText = Messages.PII_NO;
                        break;
                    case PrivateInformationIncluded.yes:
                    default:
                        cellImage.Value = Images.StaticImages.alert1_16;
                        cellImage.ToolTipText = Messages.PII_YES;
                        break;
                }
            }
        }
    }

    public enum ContentType { text_plain, application_data }

    public enum PrivateInformationIncluded { yes, maybe, customized, no}

    public class Capability : IComparable<Capability>, IEquatable<Capability>
    {
        public ContentType ContentType;
        bool _defaultChecked;
        public long MaxSize;
        public long MinSize;
        public PrivateInformationIncluded PII;
        private string _key;
        private string _name;
        private string _description;

        public bool Checked;

        public bool DefaultChecked
        {
            get => _defaultChecked;
            set
            {
                _defaultChecked = value;
                Checked = value;
            }
        }

        public string Key
        {
            get => _key;
            set
            {
                _key = value;

                _name = FriendlyNameManager.GetFriendlyName($"Label-host.system_status-{_key}");
                _description = FriendlyNameManager.GetFriendlyName($"Description-host.system_status-{_key}");

                if (string.IsNullOrEmpty(_name))
                    _name = _key;
                else if (_key == "client-logs")
                    _name = string.Format(_name, BrandManager.BrandConsole);
                else if (_key == "CVSM")
                {
                    _name = string.Format(_name, BrandManager.CompanyNameShort);
                    _description = string.Format(_description, BrandManager.CompanyNameShort);
                }
            }
        }

        public string Name => _name;

        public string Description => _description;

        public string EstimatedSize => Helpers.StringFromMaxMinSize(MinSize, MaxSize);

        public override string ToString()
        {
            return _name;
        }

        public override int GetHashCode()
        {
            return Key == null ? 0 : Key.GetHashCode();
        }

        public int CompareTo(Capability other)
        {
            return StringUtility.NaturalCompare(Key, other?.Key);
        }

        public bool Equals(Capability other)
        {
            return Key.Equals(other?.Key);
        }
    }
}
