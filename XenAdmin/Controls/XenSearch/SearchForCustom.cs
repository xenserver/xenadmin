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
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.XenSearch;

namespace XenAdmin.Controls.XenSearch
{
    public partial class SearchForCustom : XenDialogBase
    {
        private readonly Dictionary<ObjectTypes, String> typeNames;

        public SearchForCustom(Dictionary<ObjectTypes, String> typeNames, ObjectTypes initialState)
            : base()
        {
            InitializeComponent();
            this.typeNames = typeNames;
            InitializeList(initialState);
        }

        public ObjectTypes Selected
        {
            get
            {
                ObjectTypes t = ObjectTypes.None;
                foreach (SearchForCustomItem o in checkedListBox.CheckedItems)
                    t |= o.Type;
                return t;
            }
        }

        private void AddItemToSearchFor(ObjectTypes o, ObjectTypes initialState)
        {
            SearchForCustomItem item = new SearchForCustomItem(typeNames[o], o);
            bool on = ((initialState & o) == o);
            checkedListBox.Items.Add(item, on);
        }

        private void InitializeList(ObjectTypes initialState)
        {
            AddItemToSearchFor(ObjectTypes.Pool, initialState);
            AddItemToSearchFor(ObjectTypes.Server, initialState);
            AddItemToSearchFor(ObjectTypes.DisconnectedServer, initialState);
            AddItemToSearchFor(ObjectTypes.VM, initialState);
            AddItemToSearchFor(ObjectTypes.UserTemplate, initialState);
            AddItemToSearchFor(ObjectTypes.DefaultTemplate, initialState);
            AddItemToSearchFor(ObjectTypes.Snapshot, initialState);
            AddItemToSearchFor(ObjectTypes.RemoteSR, initialState);
            AddItemToSearchFor(ObjectTypes.LocalSR, initialState);
            AddItemToSearchFor(ObjectTypes.VDI, initialState);
            AddItemToSearchFor(ObjectTypes.Network, initialState);
            AddItemToSearchFor(ObjectTypes.Folder, initialState);
            //AddItemToSearchFor(ObjectTypes.DockerContainer, initialState);

            // The item check change event only fires before the check state changes
            // so to reuse the logic we have to pretend that something has changed as the enablement code expects to deal with a new value from the args
            checkedListBox_ItemCheck(null, new ItemCheckEventArgs(0, checkedListBox.GetItemCheckState(0), checkedListBox.GetItemCheckState(0)));
        }

        private void OnOK(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnCancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++) 
            {
                checkedListBox.SetItemCheckState(i, CheckState.Checked);
            }
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool allChecked = true;
            bool allUnchecked = true;
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                // this event fires before the checked state changes
                CheckState s = i == e.Index ? e.NewValue : checkedListBox.GetItemCheckState(i);
                if (s == CheckState.Checked)
                    allUnchecked = false;
                else
                    allChecked = false;
            }
            buttonClearAll.Enabled = !allUnchecked;
            buttonSelectAll.Enabled = !allChecked;
            // Stops the focus dancing because we click and disable the same button
            okButton.Focus();
        }
    }

    public class SearchForCustomItem
    {
        String name;
        ObjectTypes type;

        public SearchForCustomItem(String name, ObjectTypes type)
        {
            this.name = name;
            this.type = type;
        }

        public ObjectTypes Type
        {
            get { return type; }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}