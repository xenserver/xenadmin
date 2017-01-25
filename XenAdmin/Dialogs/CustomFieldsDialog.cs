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
using XenAdmin.CustomFields;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAdmin.XenSearch;
using XenAPI;
using XenAdmin.Actions;


namespace XenAdmin.Dialogs
{
    public partial class CustomFieldsDialog : XenDialogBase
    {
        public CustomFieldsDialog(IXenConnection connection) : base(connection)
        {
            InitializeComponent();

            Build();

            CustomFieldsManager.CustomFieldsChanged += CustomFields_CustomFieldsChanged;
        }

        void CustomFields_CustomFieldsChanged()
        {
            Build();
        }

        private void Build()
        {
            lbCustomFields.BeginUpdate();

            try
            {
                lbCustomFields.Items.Clear();

                lbCustomFields.Items.AddRange(CustomFieldsManager.GetCustomFields(connection).ToArray());
        
                btnDelete.Enabled = false;
            }
            finally
            {
                lbCustomFields.EndUpdate();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            NewCustomFieldDialog dialog = new NewCustomFieldDialog(connection);
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            CustomFieldDefinition definition = dialog.Definition;

            DelegatedAsyncAction action = new DelegatedAsyncAction(connection,
                String.Format(Messages.ADD_CUSTOM_FIELD, definition.Name),
                String.Format(Messages.ADDING_CUSTOM_FIELD, definition.Name),
                String.Format(Messages.ADDED_CUSTOM_FIELD, definition.Name),
                delegate(Session session)
                {
                    CustomFieldsManager.AddCustomField(session, connection, definition);
                });
            action.RunAsync();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CustomFieldDefinition customFieldDefinition = lbCustomFields.SelectedItem as CustomFieldDefinition;
            if (customFieldDefinition == null)
                return;

            string name = customFieldDefinition.Name.Ellipsise(50);

            if (!MainWindow.Confirm(connection, Program.MainWindow, Messages.MESSAGEBOX_CONFIRM, Messages.MESSAGEBOX_DELETE_CUSTOM_FIELD, name))
                return;

            int selIdx = lbCustomFields.SelectedIndex;

            lbCustomFields.Items.RemoveAt(selIdx);
            DelegatedAsyncAction action = new DelegatedAsyncAction(connection,
                String.Format(Messages.DELETE_CUSTOM_FIELD, name),
                String.Format(Messages.DELETING_CUSTOM_FIELD, name),
                String.Format(Messages.DELETED_CUSTOM_FIELD, name),
                delegate(Session session)
                {
                    CustomFieldsManager.RemoveCustomField(session, connection, customFieldDefinition);
                });
            action.RunAsync();
        }

        private void lbCustomFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = lbCustomFields.SelectedIndex >= 0;
        }
    }
}