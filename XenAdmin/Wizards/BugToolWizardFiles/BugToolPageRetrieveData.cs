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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Actions;


namespace XenAdmin.Wizards.BugToolWizardFiles
{
    public partial class BugToolPageRetrieveData : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SystemStatusAction _action;
        private List<HostWithStatus> _hostList = new List<HostWithStatus>();

        public BugToolPageRetrieveData()
        {
            InitializeComponent();
        }

        public override string Text { get { return Messages.BUGTOOL_PAGE_RETRIEVEDATA_TEXT; } }

        public override string PageTitle { get { return Messages.BUGTOOL_PAGE_RETRIEVEDATA_PAGE_TITLE; } }

        public override string HelpID { get { return "CompileReport"; } }

        public override bool EnableNext()
        {
            if (_action == null || !_action.IsCompleted)
                return false;

            if (_action.SomethingToSave)
                return true;

            return false;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (direction == PageLoadedDirection.Forward)
                RunAction(CapabilityList, SelectedHosts);
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (OutputFolder == null)
            {
                using (var dlog = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.BUGTOOL_PAGE_RETRIEVEDATA_CONFIRM_CANCEL, Messages.BUGTOOL_PAGE_RETRIEVEDATA_PAGE_TITLE),
                        ThreeButtonDialog.ButtonYes,
                        ThreeButtonDialog.ButtonNo))
                {
                    if (dlog.ShowDialog(this) == DialogResult.Yes)
                        CancelAction();
                    else
                        cancel = true;
                }
            }
            base.PageLeave(direction, ref cancel);
        }

        public override void PageCancelled()
        {
            if (_action != null)
                CancelAction();
        }

        public override void SelectDefaultControl()
        {
            flickerFreeListBox1.Select();
        }

        public List<Host> SelectedHosts { private get; set; }
        public IEnumerable<Capability> CapabilityList { private get; set; }

        /// <summary>
        /// Must be called on the event thread.
        /// </summary>
        private void CancelAction()
        {
            Program.AssertOnEventThread();
            OnPageUpdated();
            _action.Changed -= _action_Changed;
            _action.Completed -= _action_Completed;
            _action.Cancel();
            _action = null;
        }

        private void RunAction(IEnumerable<Capability> capabilities, List<Host> hosts)
        {
            OnPageUpdated();
            _hostList.Clear();
            flickerFreeListBox1.Items.Clear();
            label1.Text = "";
            long size = 0;
            foreach (Capability c in capabilities)
                if (c.Key != "client-logs")
                    size += c.MinSize;
            foreach(Host host in hosts)
            {
                HostWithStatus hostWithStatus = new HostWithStatus(host,size);
                _hostList.Add(hostWithStatus);
                flickerFreeListBox1.Items.Add(hostWithStatus);
            }
            List<string> strings = new List<string>();
            foreach(Capability c in capabilities)
                    strings.Add(c.Key);
            _action = new SystemStatusAction(_hostList, strings);
            _action.Changed += _action_Changed;
            _action.Completed += _action_Completed;
            _action.RunAsync();
        }

        private void finish()
        {
            if (_action.Exception == null)
            {
                label1.Text = _action.Description == Messages.ACTION_SYSTEM_STATUS_NONE_SUCCEEDED
                                  ? Messages.ACTION_SYSTEM_STATUS_NONE_SUCCEEDED_GUI
                                  : _action.Description;

                flickerFreeListBox1.Refresh();

                //Update buttons to enable next iff none of the hosts have failed
                //The next button will remain disabled should there be a failing host
                if (!_hostList.TrueForAll(host => host.Status == HostStatus.failed))
                    OnPageUpdated();

                progressBar1.Value = 100;
            }
            else
            {
                // we probably will get here
                log.Debug("Server status report finished with errors",_action.Exception);
            }
        }

        public string OutputFolder
        {
            get
            {
                if (_action.IsCompleted)
                {
                    if (_action.Cancelled)
                        return "";
                    return _action.Result ?? "";
                }
                return null;
            }
        }

        private void actionchanged()
        {
            progressBar1.Value = _action.PercentComplete;
            label1.Text = _action.Description;
            flickerFreeListBox1.Refresh();
            OnPageUpdated();
        }

        private void _action_Completed(ActionBase sender)
        {
            Program.Invoke(this, finish);
        }

        private void _action_Changed(object sender)
        {
            Program.Invoke(this, actionchanged);
        }

        private void flickerFreeListBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            HostWithStatus host = flickerFreeListBox1.Items[e.Index] as HostWithStatus;
            using (SolidBrush backBrush = new SolidBrush(flickerFreeListBox1.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            e.Graphics.DrawImage(Images.GetImage16For(host.Host), e.Bounds.Left, e.Bounds.Top);

            int width = Drawing.MeasureText(host.StatusString, flickerFreeListBox1.Font).Width;
            Drawing.DrawText(e.Graphics, host.ToString(), flickerFreeListBox1.Font, new Rectangle(e.Bounds.Left + Properties.Resources._000_Server_h32bit_16.Width, e.Bounds.Top, e.Bounds.Right - (width + Properties.Resources._000_Server_h32bit_16.Width), e.Bounds.Height), flickerFreeListBox1.ForeColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            if (host.Status == HostStatus.queued)
                Drawing.DrawText(e.Graphics, host.StatusString, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Right - width, e.Bounds.Top, width, e.Bounds.Height), flickerFreeListBox1.ForeColor, flickerFreeListBox1.BackColor);
            else if (host.Status == HostStatus.compiling || host.Status == HostStatus.downloading)
                Drawing.DrawText(e.Graphics, host.StatusString, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Right - width, e.Bounds.Top, width, e.Bounds.Height), Color.Blue, flickerFreeListBox1.BackColor);
            else if (host.Status == HostStatus.succeeded)
                Drawing.DrawText(e.Graphics, host.StatusString, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Right - width, e.Bounds.Top, width, e.Bounds.Height), Color.Green, flickerFreeListBox1.BackColor);
            if (host.Status == HostStatus.failed)
                Drawing.DrawText(e.Graphics, host.StatusString, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Right - width, e.Bounds.Top, width, e.Bounds.Height), Color.Red, flickerFreeListBox1.BackColor);
        }
    }

}
