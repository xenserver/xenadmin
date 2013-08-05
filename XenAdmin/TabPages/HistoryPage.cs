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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.TabPages
{
    public partial class HistoryPage : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<IXenObject> _xenObjects = new List<IXenObject>();

        public bool ShowAll { get; set; }

        public HistoryPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;

            label1.ForeColor = Program.HeaderGradientForeColor;
            label1.Font = Program.HeaderGradientFont;

            actionsCheckBox.ForeColor = Program.HeaderGradientForeColor;
            errorsCheckBox.ForeColor = Program.HeaderGradientForeColor;
            informationCheckBox.ForeColor = Program.HeaderGradientForeColor;
        }

        public void SetXenObjects(IEnumerable<IXenObject> xenObjects)
        {
            Util.ThrowIfParameterNull(xenObjects, "xenObjects");
            Program.AssertOnEventThread();
            _xenObjects.Clear();
            _xenObjects.AddRange(xenObjects);

            // Refilter (but don't rebuild) list
            foreach (ActionRow row in customHistoryContainer1.CustomHistoryPanel.Rows)
            {
                row.Visible = FilterRowVisible(row);
            }

            removeAllButtonUpdate();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (ConnectionsManager.History.Count > 0 && (Program.RunInAutomatedTestMode ||
                new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.MESSAGEBOX_LOGS_DELETE, Messages.MESSAGEBOX_LOGS_DELETE_TITLE),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo).ShowDialog(this) == DialogResult.Yes))
            {
                customHistoryContainer1.CustomHistoryPanel.Rows.RemoveAll(delegate(CustomHistoryRow item)
                {
                    ActionRow row = item as ActionRow;
                    if (row != null && item.Visible && row.Action.IsCompleted)
                    {
                        ConnectionsManager.History.Remove(row.Action);
                        return true;
                    }
                    return false;
                });
            }
            customHistoryContainer1.CustomHistoryPanel.Refresh();
        }

        private void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(Program.MainWindow, () =>
                                                        {
                                                            ActionBase action = (ActionBase) e.Element;
                                                            ActionRow row = null;
                                                            switch (e.Action)
                                                            {
                                                                case CollectionChangeAction.Add:
                                                                    row = AddRow(action);
                                                                    row.Visible = FilterRowVisible(row);
                                                                    break;
                                                                case CollectionChangeAction.Remove:
                                                                    row = FindRowFromAction(action);
                                                                    if (row != null)
                                                                        customHistoryContainer1.CustomHistoryPanel.Rows.
                                                                            Remove(row);
                                                                    break;
                                                                case CollectionChangeAction.Refresh:
                                                                    BuildRowList();
                                                                    break;
                                                            }
                                                            removeAllButtonUpdate();
                                                        });
        }

        private void removeAllButtonUpdate()
        {
            button1.Enabled = customHistoryContainer1.CustomHistoryPanel.Rows.Find(r => r.Visible) != null;
        }

        private ActionRow FindRowFromAction(ActionBase action)
        {
            foreach (ActionRow row in customHistoryContainer1.CustomHistoryPanel.Rows)
            {
                if (row.Action == action)
                    return row;
            }
            log.Warn("Somehow an Action is being removed from the collection before we had an add event for it");
            return null;
        }

        public void BuildRowList()
        {
            customHistoryContainer1.CustomHistoryPanel.SuspendDraw = true;
            customHistoryContainer1.CustomHistoryPanel.Rows.Clear();
            foreach (ActionBase a in ConnectionsManager.History)
            {
                ActionRow row = AddRow(a);
                row.Visible = FilterRowVisible(row);
                a.Changed -= action_Changed;
                a.Completed -= action_Changed; 
                a.Changed += action_Changed;
                a.Completed += action_Changed;
            }
            customHistoryContainer1.CustomHistoryPanel.SuspendDraw = false;
            customHistoryContainer1.CustomHistoryPanel.Refresh();
            removeAllButtonUpdate();
        }

        private ActionRow AddRow(ActionBase action)
        { 
            ActionRow row = new ActionRow(action);

            Debug.Assert(!string.IsNullOrEmpty(row.Title), "Entries in the logs tab should have a title.");
            Debug.Assert(!row.Title.Contains("{0}"), "Has a string.Format() been forgotten?");

            customHistoryContainer1.CustomHistoryPanel.AddItem(row);
            return row;
        }

        private bool FilterRowVisible(ActionRow row)
        {
            if (!ShowAll && _xenObjects.Find(x => row.AppliesTo.Contains(x.opaque_ref)) == null)
                return false;

            if (!actionsCheckBox.Checked && (row.Type == ActionType.Action || row.Type == ActionType.Meddling))
                return false;

            if (!errorsCheckBox.Checked && (row.Type == ActionType.Error || row.Error))
                return false;

            if (!informationCheckBox.Checked && row.Type == ActionType.Information)
                return false;

            return true;
        }

        private void actionsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BuildRowList();
        }

        private void alertsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BuildRowList();
        }

        private void informationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BuildRowList();
        }

        private void errorsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BuildRowList();
        }

        private void action_Changed(ActionBase sender)
        {
            Program.Invoke(this, delegate
            {
                ActionRow row = FindRowFromAction(sender);
                if (row != null && row.Visible)
                    customHistoryContainer1.Invalidate();
            });
        }
    }
}
