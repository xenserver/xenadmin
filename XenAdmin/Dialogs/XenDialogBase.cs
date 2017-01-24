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
using System.Windows.Forms;
using XenAdmin.Network;
using XenAdmin.Core;

namespace XenAdmin.Dialogs
{
    public partial class XenDialogBase : Form
    {
        private static Dictionary<IXenConnection, List<XenDialogBase>> instances = new Dictionary<IXenConnection, List<XenDialogBase>>();

        private static void AddInstance(IXenConnection connection, XenDialogBase dialog)
        {
            lock(instances)
            {
                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new List<XenDialogBase>());

                instances[connection].Add(dialog);
            }
        }

        public static void CloseAll(IXenConnection connection)
        {
            Program.AssertOnEventThread();

            lock (instances)
            {
                if (!instances.ContainsKey(connection))
                    return;

                List<XenDialogBase> dialogs = new List<XenDialogBase>(instances[connection]);

                foreach (XenDialogBase dialog in dialogs)
                {
                    if (dialog is AddServerDialog || dialog is ConnectingToServerDialog)
                    {
                        // These dialogs are managed by the connection itself
                        continue;
                    }
                    dialog.Close();
                }

                instances.Remove(connection);
            }
        }

        private IXenConnection _connection;
        protected IXenConnection connection
        {
            get { return _connection; }
            set
            {
                _connection = value;
                if (_connection != null)
                {
                    AddInstance(_connection, this);
                }
            }
        }

        public XenDialogBase(IXenConnection connection)
            : this()
        {
            this.connection = connection;
        }

        /// <summary>
        /// Only use this ctor if you don't want your dialog to be 
        /// closed when a connection disconnects.
        /// </summary>
        public XenDialogBase()
        {
            InitializeComponent();
        }

        private bool ownerActivatedOnClosed = true;
        /// <summary>
        /// Allow the XenDialogBase.OnClosed to set Owner.Activate() - this will push the Owner
        /// to the top of the windows stack stealing focus.
        /// </summary>
        protected bool OwnerActivatedOnClosed
        {
            get { return ownerActivatedOnClosed; }
            set { ownerActivatedOnClosed = value; }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            lock (instances)
            {
                if (connection != null && instances.ContainsKey(connection))
                {
                    instances[connection].Remove(this);
                }
            }

            if (OwnerActivatedOnClosed && Owner != null)
                Owner.Activate();
        }

        private void XenDialogBase_Load(object sender, EventArgs e)
        {
            //this.Owner = Program.MainWindow;
            this.CenterToParent();
            FormFontFixer.Fix(this);
        }

        public bool HasHelp()
        {
            return Help.HelpManager.HasHelpFor(HelpName);
        }

        private void XenDialogBase_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.HelpManager.Launch(HelpName);
            e.Cancel = true;
        }

        private void XenDialogBase_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.HelpManager.Launch(HelpName);
            hlpevent.Handled = true;
        }

        /// <summary>
        /// override if the reference in the help differs to the dialogs name
        /// </summary>
        internal virtual string HelpName
        {
            get
            {
                return Name;
            }
        }

        private void XenDialogBase_Shown(object sender, EventArgs e)
        {
            if (Modal && Owner != null && Owner.WindowState == FormWindowState.Minimized)
            {
                Owner.WindowState = FormWindowState.Normal;
                CenterToParent();
            }
        }
    }
}
