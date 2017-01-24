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
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Dialogs
{
    public partial class ConnectingToServerDialog : XenDialogBase
    {
        private readonly IXenConnection _connection;
        private Form ownerForm;

        public ConnectingToServerDialog(IXenConnection connection)
            : base(connection)
        {
            this._connection = connection;
            InitializeComponent();
            Icon = Properties.Resources.AppIcon;

            FormFontFixer.Fix(this);

            this.lblStatus.Text = String.Format(Messages.LABEL_ATTEMPT, _connection.Hostname);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConnectingToServer_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        /// <summary>
        /// May be called off the event thread. Will do the necessary invoke.
        /// </summary>
        public void SetText(string text)
        {
            Program.Invoke(Program.MainWindow, delegate()
            {
                this.lblStatus.Text = text;
            });
        }

        internal override string HelpName
        {
            get { return "AddServerDialog"; }
        }

        private bool endBegun = false;
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_connection.InProgress && !_connection.IsConnected && !endBegun)
            {
                //if we are still trying to connect then we need to end it. This call will close this dialog as well,
                //so set a guard to skip this block when we re-enter.
                endBegun = true;
                _connection.EndConnect();
                e.Cancel = true;

                //CA-65004: if this dialog is closed by closing the parent form, returning
                //without calling the base class method causes the parent form to stay open
                if (e.CloseReason == CloseReason.FormOwnerClosing && getOwnerForm() == ownerForm)
                    ownerForm.Close();
                return;
            }

            if (_connection != null)
                XenConnectionUI.connectionDialogs.Remove(_connection);

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Gets the Form set as this Connection's owner in BeginConnect. All the dialogs spawned during the connection attempt
        /// are displayed with this Form as their Owner. If no Owner has been set, will return Program.MainWindow.
        /// </summary>
        /// <returns></returns>
        private Form getOwnerForm()
        {
            return ownerForm != null && !ownerForm.IsDisposed &&
                !ownerForm.Disposing && !Program.Exiting ? ownerForm : Program.MainWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">The form that connecting dialogs will be displayed in front of.
        /// May be null, in which case Program.MainWindow is used.</param>
        /// <param name="initiateMasterSearch">If true, if connection to the master fails we will start trying to connect to
        /// each remembered slave in turn.</param>
        protected internal void BeginConnect(Form owner, bool initiateMasterSearch)
        {
            if (_connection != null && _connection is XenConnection)
            {
                ownerForm = owner;

                RegisterEventHandlers();
                ((XenConnection) _connection).BeginConnect(initiateMasterSearch, HideAndPromptForNewPassword);

                if (_connection.InProgress)
                {
                    if (!Visible)
                        Show(getOwnerForm());

                    Focus();
                }
            }
        }

        private void RegisterEventHandlers()
        {
            // unregister default ConnectionResult event handler
            _connection.ConnectionResult -= XenConnectionUI.Connection_ConnectionResult; 

            _connection.ConnectionResult += Connection_ConnectionResult;
            _connection.ConnectionClosed += Connection_ConnectionClosed;
            _connection.BeforeConnectionEnd += Connection_BeforeConnectionEnd;
            _connection.ConnectionMessageChanged += Connection_ConnectionMessageChanged;
        }

        private void UnregisterEventHandlers()
        {
            // re-register default ConnectionResult event handler (delete and add, just to make sure it is only registered once)
            _connection.ConnectionResult -= XenConnectionUI.Connection_ConnectionResult; 
            _connection.ConnectionResult += XenConnectionUI.Connection_ConnectionResult;            
            
            _connection.ConnectionResult -= Connection_ConnectionResult;
            _connection.ConnectionClosed -= Connection_ConnectionClosed;
            _connection.BeforeConnectionEnd -= Connection_BeforeConnectionEnd;
            _connection.ConnectionMessageChanged -= Connection_ConnectionMessageChanged;
        }

        private void Connection_ConnectionResult(object sender, ConnectionResultEventArgs e)
        {
            CloseConnectingDialog();

            // show connection error
            if (e.Connected || e.Error == null)
                return;

            IXenConnection connection = (IXenConnection)sender;

            Program.Invoke(Program.MainWindow,
                           delegate()
                           {
                               XenConnectionUI.ShowConnectingDialogError_(getOwnerForm(), connection, e.Error);
                           });
        }

        private void Connection_ConnectionClosed(object sender, EventArgs e)
        {
            CloseConnectingDialog();
        }

        private void Connection_BeforeConnectionEnd(object sender, EventArgs e)
        {
            CloseConnectingDialog();
        }

        private void Connection_ConnectionMessageChanged(object sender, ConnectionMessageChangedEventArgs e)
        {
            if (Visible)
            {
                SetText(e.Message);
            }
        }

        private void CloseConnectingDialog()
        {
            // de-register event handlers
            UnregisterEventHandlers();

            Program.Invoke(Program.MainWindow, () =>
                                                   {
                                                       if (Visible)
                                                       {
                                                           OwnerActivatedOnClosed = false;
                                                       }
                                                       Close();
                                                   });
        }

        private bool HideAndPromptForNewPassword(IXenConnection xenConnection, string oldPassword)
        {
            bool result = false;
            Program.Invoke(Program.MainWindow, delegate()
                                                   {
                                                       bool wasVisible = Visible;
                                                       if (wasVisible)
                                                       {
                                                           Hide();
                                                       }

                                                       // show an altered version of the add server dialog with the hostname greyed out
                                                       AddServerDialog dialog = new AddServerDialog(xenConnection, true);
                                                       result = dialog.ShowDialog(getOwnerForm()) == DialogResult.OK;

                                                       if (result && wasVisible)
                                                       {
                                                           Show(getOwnerForm());
                                                       }
                                                   });
            return result;
        }
    }
}
