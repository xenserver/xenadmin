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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Dialogs;


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
        private int oldIndex = -1;
        #endregion

        public BugToolPageSelectCapabilities()
        {
            InitializeComponent();

            //set this here due to a framework bug
            splitContainer1.Panel1MinSize = 250;
            splitContainer1.Panel2MinSize = 200;

            // Due to a Visual Studio bug, ToolTipTitle is not put in the resx, even though the form is Localizable=true
            // so we have to set it here instead.
            PiiTooltip.ToolTipTitle = Messages.PRIVACY_WARNING;
        }

        public override string Text{get { return Messages.BUGTOOL_PAGE_CAPABILITIES_TEXT; }}

        public override string PageTitle { get { return Messages.BUGTOOL_PAGE_CAPABILITIES_PAGETITLE; } }

        public override string HelpID { get { return "SelectReportContents"; } }

        public override bool EnableNext()
        {
            return wizardCheckAnyChecked();
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            if (direction == PageLoadedDirection.Forward)
            {
                ClearButton.Enabled = wizardCheckAnyChecked();
                SelectButton.Enabled = wizardCheckAnyUnchecked();
            }
        }

        private bool wizardCheckAnyChecked()
        {
            foreach (Capability c in _capabilities)
                if (c.Checked)
                    return true;
            return false;
        }

        private bool wizardCheckAnyUnchecked()
        {
            foreach (Capability c in _capabilities)
                if (!c.Checked)
                    return true;
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
            CapabilitiesCheckedListBox.Items.Clear();

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
            // interfeering when they return in 20mins
            foreach (GetSystemStatusCapabilities action in actions)
                action.Completed -= Common_action_Completed;
        }

        private void Common_action_Completed(object sender, EventArgs e)
        {
            Program.Invoke(this, delegate()
            {
                if (cancelled)
                    return;

                GetSystemStatusCapabilities caps = sender as GetSystemStatusCapabilities;
                if (caps == null)
                    return;

                hostCapabilities[caps.Host] = 
                    caps.Succeeded ? parseXMLToList(caps) : null;

                if (hostCapabilities.Count >= _hostList.Count)
                {
                    dialog.Close();
                    CombineCapabilitiesLists();
                }
            });
        }

        private List<Capability> parseXMLToList(Actions.GetSystemStatusCapabilities action)
        {
            List<Capability> capabilities = new List<Capability>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(action.Result);
            foreach (XmlNode node in doc.GetElementsByTagName("capability"))
            {
                Capability c = new Capability();
                foreach (XmlAttribute a in node.Attributes)
                {
                    string name = a.Name;
                    string value = a.Value;

                    if (name == "content-type")
                        c.ContentType = value == "text/plain" ? ContentType.text_plain : ContentType.application_data;
                    else if (name == "default-checked")
                        c.DefaultChecked = value == "yes" ? true : false;
                    else if (name == "key")
                        c.Key = value;
                    else if (name == "max-size")
                        c.MaxSize = Int64.Parse(value);
                    else if (name == "min-size")
                        c.MinSize = Int64.Parse(value);
                    else if (name == "max-time")
                        c.MaxTime = Int64.Parse(value);
                    else if (name == "min-time")
                        c.MinTime = Int64.Parse(value);
                    else if (name == "pii")
                        c.PII = value == "yes" ? PrivateInformationIncluded.yes :
                            value == "no" ? PrivateInformationIncluded.no :
                            value == "maybe" ? PrivateInformationIncluded.maybe : PrivateInformationIncluded.customized;
                }

                capabilities.Add(c);
            }

            return capabilities;
        }

        private void CombineCapabilitiesLists()
        {
            List<Capability> combination = null;

            foreach (List<Capability> capabilities in hostCapabilities.Values)
            {
                if (capabilities == null)
                    continue;

                if (combination == null)
                {
                    combination = capabilities;
                    continue;
                }

                combination = Helpers.ListsCommonItems<Capability>(combination, capabilities);
            }

            if (combination == null || combination.Count <= 0)
            {
                new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        Messages.SERVER_STATUS_REPORT_CAPABILITIES_FAILED,
                        Messages.SERVER_STATUS_REPORT)).ShowDialog(this);

                cancelled = true;
                return;
            }
                
            _capabilities = combination;

            _capabilities.Add(GetClientCapability());

            Sort_capabilities();

            buildList();
        }

        private Capability GetClientCapability()
        {
            Capability clientCap = new Capability();
            clientCap.ContentType = ContentType.text_plain;
            clientCap.DefaultChecked = true;
            clientCap.Key = "client-logs";
            clientCap.MaxSize = getLogSize();
            clientCap.MinSize = clientCap.MaxSize;
            clientCap.MaxTime = -1;
            clientCap.MinTime = -1;
            clientCap.PII = PrivateInformationIncluded.yes;

            return clientCap;
        }

        private void Sort_capabilities()
        {
            _capabilities.Sort(new Comparison<Capability>(delegate(Capability obj1, Capability obj2)
            {
                int piicompare = obj1.PII.CompareTo(obj2.PII);
                if (piicompare == 0)
                    return StringUtility.NaturalCompare(obj1.ToString(), obj2.ToString());
                else
                    return piicompare;
            }));
        }

        private void OnCheckedCapabilitiesChanged(bool refreshList)
        {
            TotalSizeValue.Text = getShoppingCartTotalSize();
            TotalTimeValue.Text = getShoppingCartTotalTime();
            OnPageUpdated();
            ClearButton.Enabled = wizardCheckAnyChecked();
            SelectButton.Enabled = wizardCheckAnyUnchecked();

            if (refreshList)
                CapabilitiesCheckedListBox.Refresh();
        }

        private void buildList()
        {
            CapabilitiesCheckedListBox.Items.Clear();

            foreach (Capability c in _capabilities)
            {
                CapabilitiesCheckedListBox.Items.Add(c);
                if (CapabilitiesCheckedListBox.SelectedIndex == -1)
                    CapabilitiesCheckedListBox.SelectedItem = c;    
            }
            OnCheckedCapabilitiesChanged(false);
        }

        private long getLogSize()
        {
            String path = Program.GetLogFile_();
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

        private string getShoppingCartTotalSize()
        {
            List<long> minList = new List<long>();
            List<long> maxList = new List<long>();
            foreach (Capability c in CapabilitiesCheckedListBox.Items)
            {
                if (c.Checked)
                {
                    int m = c.Key == "client-logs" ? 1 : _hostList.Count;
                    minList.Add(c.MinSize * m);
                    maxList.Add(c.MaxSize * m);
                }
            }
            return Helpers.StringFromMaxMinSizeList(minList, maxList);
        }

        private string getShoppingCartTotalTime()
        {
            List<long> minList = new List<long>();
            List<long> maxList = new List<long>();
            foreach (Capability c in CapabilitiesCheckedListBox.Items)
            {
                if (c.Checked)
                {
                    int m = c.Key == "client-logs" ? 1 : _hostList.Count;
                    minList.Add(c.MinTime * m);
                    maxList.Add(c.MaxTime * m);
                }
            }
            return Helpers.StringFromMaxMinTimeList(minList, maxList);
        }

        public List<Capability> Capabilities
        {
            get
            {
                List<Capability> list = new List<Capability>();
                foreach (Capability c in CapabilitiesCheckedListBox.Items)
                {
                    if(c.Checked)
                        list.Add(c);
                }
                return list;
            }
        }

        #region Control event handlers

        private void CapabilitiesCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CapabilitiesCheckedListBox.SelectedIndex == -1)
                return;

            Capability c = CapabilitiesCheckedListBox.SelectedItem as Capability;

            DescriptionValue.Text = c.Description;
            SizeValue.Text = c.EstimatedSize;
            TimeValue.Text = c.EstimatedTime;
        }

        private void CapabilitiesCheckedListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            Capability c = CapabilitiesCheckedListBox.Items[e.Index] as Capability;
            using (SolidBrush backBrush = new SolidBrush(e.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.Left + 1, e.Bounds.Top + 1), c.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
                Drawing.DrawText(e.Graphics, c.ToString(), Font, new Point(e.Bounds.Left + e.Bounds.Height, e.Bounds.Top + 1), e.ForeColor);
                e.Graphics.FillRectangle(backBrush, new Rectangle(e.Bounds.Right - 15, e.Bounds.Top, 15, e.Bounds.Height));
            }
            if (c.PII == PrivateInformationIncluded.customized)
            {
                e.Graphics.DrawImage(Properties.Resources._000_PiiCustomised_h32bit_16, new Rectangle(e.Bounds.Right - 15, e.Bounds.Top, 15, 15));
            }
            else if (c.PII == PrivateInformationIncluded.maybe)
            {
                e.Graphics.DrawImage(Properties.Resources._000_PiiMaybe_h32bit_16, new Rectangle(e.Bounds.Right - 15, e.Bounds.Top, 15, 15));
            }
            else if (c.PII == PrivateInformationIncluded.no)
            {
                e.Graphics.DrawImage(Properties.Resources._000_PiiNo_h32bit_16, new Rectangle(e.Bounds.Right - 15, e.Bounds.Top, 15, 15));
            }
            else
            {
                e.Graphics.DrawImage(Properties.Resources._000_PiiYes_h32bit_16, new Rectangle(e.Bounds.Right - 15, e.Bounds.Top, 15, 15));
            }
        }

        private void CapabilitiesCheckedListBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = CapabilitiesCheckedListBox.PointToClient(MousePosition);
            int imageLeft = CapabilitiesCheckedListBox.Height > CapabilitiesCheckedListBox.ItemHeight * CapabilitiesCheckedListBox.Items.Count ? 19 : 37;
            if (point.X < CapabilitiesCheckedListBox.Width - imageLeft)
            {
                PiiTooltip.RemoveAll();
                return;
            }
            int hoverIndex = CapabilitiesCheckedListBox.IndexFromPoint(point);
            if (hoverIndex >= 0 && hoverIndex < CapabilitiesCheckedListBox.Items.Count)
            {
                if ((CapabilitiesCheckedListBox.Items[hoverIndex] as Capability).PiiText != PiiTooltip.GetToolTip(CapabilitiesCheckedListBox) || hoverIndex != oldIndex)
                {
                    oldIndex = hoverIndex;
                    PiiTooltip.Active = false;
                    PiiTooltip.SetToolTip(CapabilitiesCheckedListBox, (CapabilitiesCheckedListBox.Items[hoverIndex] as Capability).PiiText);
                    PiiTooltip.Active = true;
                }
            } 
        }

        private void CapabilitiesCheckedListBox_MouseUp(object sender, MouseEventArgs e)
        {
            Point point = CapabilitiesCheckedListBox.PointToClient(MousePosition);
            if (point.X < 15)
            {
                Capability c = CapabilitiesCheckedListBox.SelectedItem as Capability;
                c.Checked = !c.Checked;
                OnCheckedCapabilitiesChanged(true);
            }
        }

        private void CapabilitiesCheckedListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Capability c = CapabilitiesCheckedListBox.SelectedItem as Capability;
                if (c != null)
                {
                    c.Checked = !c.Checked;
                    OnCheckedCapabilitiesChanged(true);
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(InvisibleMessages.PRIVACY);
            }
            catch
            {
                new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       Messages.HOMEPAGE_ERROR_MESSAGE,
                       Messages.XENCENTER)).ShowDialog(this);
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            foreach (Capability c in _capabilities)
                c.Checked = true;

            OnCheckedCapabilitiesChanged(true);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            foreach (Capability c in _capabilities)
                c.Checked = false;

            OnCheckedCapabilitiesChanged(true);
        }

        #endregion
    }

    public enum ContentType { text_plain, application_data };

    public enum PrivateInformationIncluded { yes, maybe, customized, no};

    public class Capability : IComparable<Capability>, IEquatable<Capability>
    {
        public ContentType ContentType;
        bool _defaultChecked;
        public string Key;
        public long MaxSize;
        public long MinSize;
        public long MaxTime;
        public long MinTime;
        public PrivateInformationIncluded PII;

        public bool Checked;

        public bool DefaultChecked
        {
            get
            {
                return _defaultChecked;
            }
            set
            {
                _defaultChecked = value;
                Checked = value;
            }
        }

        public string PiiText
        {
            get
            {
                if (PII == PrivateInformationIncluded.yes)
                    return Messages.PII_YES;
                else if(PII == PrivateInformationIncluded.maybe)
                    return Messages.PII_MAYBE;
                else if(PII == PrivateInformationIncluded.customized)
                    return Messages.PII_CUSTOMISED;
                else
                    return Messages.PII_NO;

            }
        }

        public string Description
        {
            get
            {
                return Core.PropertyManager.GetFriendlyName(
                    string.Format("Description-host.system_status-{0}", Key));
            }
        }

        public string EstimatedSize
        {
            get
            {
                return Helpers.StringFromMaxMinSize(MinSize, MaxSize);
            }
        }

        public string EstimatedTime
        {
            get
            {
                return Helpers.StringFromMaxMinTime(MinTime, MaxTime);
            }
        }

        public override string ToString()
        {
            string result = Core.PropertyManager.GetFriendlyName(
               string.Format("Label-host.system_status-{0}", Key));
            return result == null ? Key : result;
        }

        public int CompareTo(Capability other)
        {
            return StringUtility.NaturalCompare(this.Key, other.Key);
        }

        public bool Equals(Capability other)
        {
            return this.Key.Equals(other.Key);
        }
    }
}