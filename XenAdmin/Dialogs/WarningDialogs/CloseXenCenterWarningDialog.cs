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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs.WarningDialogs
{
    public partial class CloseXenCenterWarningDialog : XenDialogBase
    {
        public CloseXenCenterWarningDialog()
        {
            InitializeComponent();
            customHistoryContainer1.AutoScroll = true;

            buildList();
        }

        public CloseXenCenterWarningDialog(IXenConnection connection)
        {
            InitializeComponent();
            this.connection = connection;

            label2.Text = String.Format(Messages.DISCONNECT_WARNING, Helpers.GetName(connection).Ellipsise(50));
            ExitButton.Text = Messages.DISCONNECT_ANYWAY;
            DontExitButton.Text = Messages.DISCONNECT_CANCEL;
            buildSingleConnectionList();
        }

        private void buildSingleConnectionList()
        {
            foreach (ActionBase action in ConnectionsManager.History)
            {
                AsyncAction a = action as AsyncAction;
                if (!action.IsCompleted && (a == null || !a.Cancelling))
                {
                    IXenObject xo = (action.Pool as IXenObject) ?? (action.Host as IXenObject) ?? (action.VM as IXenObject) ?? (action.SR as IXenObject);
                    if (xo == null || xo.Connection != connection)
                        continue;

                    AddRow(action);
                }
            }
        }

        private void buildList()
        {
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (!action.IsCompleted)
                {
                    AddRow(action);
                }
            }
        }

        private void AddRow(ActionBase action)
            {
                ActionBase a = action;
                ActionRow row = new ActionRow(action);
                row.ShowCancel = false;
                action.Completed += delegate
                    {
                        Program.Invoke(this, delegate
                            {
                                row.Visible = !a.IsCompleted;
                                row.ShowCancel = false;
                                bool canEnd = true;
                                foreach (ActionRow r in customHistoryContainer1.CustomHistoryPanel.Rows)
                                {
                                    if (!r.Action.IsCompleted || r.Action.Exception != null)
                                    {
                                        canEnd = false;
                                        break;
                                    }
                                }
                                customHistoryContainer1.Refresh();
                                if (canEnd)
                                    DialogResult = DialogResult.Ignore;
                            });
                    };
                action.Changed += (obj => Program.Invoke(this, delegate
                    {
                        row.Visible = !a.IsCompleted;
                        row.ShowCancel = false;
                        customHistoryContainer1.Refresh();
                    }));
                customHistoryContainer1.CustomHistoryPanel.AddItem(row);
            }

        internal override string HelpName
        {
            get
            {
                return connection == null ? Name : "DisconnectServerWarningDialog";
            }
        }
    }
}