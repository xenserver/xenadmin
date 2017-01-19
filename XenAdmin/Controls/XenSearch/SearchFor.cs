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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.XenSearch;


namespace XenAdmin.Controls.XenSearch
{
    public partial class SearchFor : UserControl
    {
        public event Action QueryChanged;

        private const ObjectTypes CUSTOM = ObjectTypes.None;  // We use None as a special signal value for "Custom..."

        private readonly Dictionary<ObjectTypes, String> typeNames = new Dictionary<ObjectTypes, String>();
        private readonly Dictionary<ObjectTypes, Image> typeImages = new Dictionary<ObjectTypes, Image>();

        private ObjectTypes customValue;
        private ObjectTypes savedTypes;
        private bool autoSelecting = false;

        public SearchFor()
        {
            InitializeComponent();
            InitializeDictionaries();
            PopulateSearchForComboButton();
        }

        public void BlankSearch()
        {
            QueryScope = new QueryScope(ObjectTypes.None);
            OnQueryChanged();
        }

        private void OnQueryChanged()
        {
            if (QueryChanged != null)
                QueryChanged();
        }

        private void InitializeDictionaries()
        {
            // add all single types, names and images
            Dictionary<String, ObjectTypes> dict = (Dictionary<String, ObjectTypes>)PropertyAccessors.Geti18nFor(PropertyNames.type);
            ImageDelegate<ObjectTypes> images = (ImageDelegate<ObjectTypes>)PropertyAccessors.GetImagesFor(PropertyNames.type);
            foreach (KeyValuePair<String, ObjectTypes> kvp in dict)
            {
                typeNames[kvp.Value] = kvp.Key;
                typeImages[kvp.Value] = Images.GetImage16For(images(kvp.Value));
            }

            // add all combo types, mostly names only
            typeNames[ObjectTypes.LocalSR | ObjectTypes.RemoteSR] = Messages.ALL_SRS;
            typeImages[ObjectTypes.LocalSR | ObjectTypes.RemoteSR] = Images.GetImage16For(images(ObjectTypes.LocalSR | ObjectTypes.RemoteSR));
            typeNames[ObjectTypes.Server | ObjectTypes.DisconnectedServer | ObjectTypes.VM] = Messages.SERVERS_AND_VMS;
            typeNames[ObjectTypes.Server | ObjectTypes.DisconnectedServer | ObjectTypes.VM | ObjectTypes.UserTemplate | ObjectTypes.RemoteSR] = Messages.SERVERS_AND_VMS_AND_CUSTOM_TEMPLATES_AND_REMOTE_SRS;
            typeNames[ObjectTypes.Server | ObjectTypes.DisconnectedServer | ObjectTypes.VM | ObjectTypes.UserTemplate | ObjectTypes.RemoteSR | ObjectTypes.LocalSR] = Messages.SERVERS_AND_VMS_AND_CUSTOM_TEMPLATES_AND_ALL_SRS;
            typeNames[ObjectTypes.AllExcFolders] = Messages.ALL_TYPES;
            typeNames[ObjectTypes.AllIncFolders] = Messages.ALL_TYPES_AND_FOLDERS;
            //typeNames[ObjectTypes.DockerContainer] = "Docker containers";
        }

        private void AddItemToSearchFor(ObjectTypes type)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = typeNames[type];
            item.Tag = type;
            if (typeImages.ContainsKey(type))
                item.Image = typeImages[type];
            searchForComboButton.AddItem(item);
        }

        private void AddCustom()
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = Messages.CUSTOM;
            item.Tag = CUSTOM;
            searchForComboButton.AddItem(item);
        }

        private void AddSeparator()
        {
            searchForComboButton.AddItem(new ToolStripSeparator());
        }

        // The order here is not the same as the order in the Organization View,
        // which is determined by the ObjectTypes enum (CA-28418).
        private void PopulateSearchForComboButton()
        {
            AddItemToSearchFor(ObjectTypes.Pool);
            AddItemToSearchFor(ObjectTypes.Server);
            AddItemToSearchFor(ObjectTypes.DisconnectedServer);
            AddItemToSearchFor(ObjectTypes.VM);
            AddItemToSearchFor(ObjectTypes.Snapshot);
            AddItemToSearchFor(ObjectTypes.UserTemplate);
            AddItemToSearchFor(ObjectTypes.DefaultTemplate);
            AddItemToSearchFor(ObjectTypes.RemoteSR);
            AddItemToSearchFor(ObjectTypes.RemoteSR | ObjectTypes.LocalSR);  // local SR on its own is pretty much useless
            AddItemToSearchFor(ObjectTypes.VDI);
            AddItemToSearchFor(ObjectTypes.Network);
            AddItemToSearchFor(ObjectTypes.Folder);
            //AddItemToSearchFor(ObjectTypes.DockerContainer);
            AddSeparator();
            AddItemToSearchFor(ObjectTypes.Server | ObjectTypes.DisconnectedServer | ObjectTypes.VM);
            AddItemToSearchFor(ObjectTypes.Server | ObjectTypes.DisconnectedServer | ObjectTypes.VM | ObjectTypes.UserTemplate | ObjectTypes.RemoteSR);
            AddItemToSearchFor(ObjectTypes.Server | ObjectTypes.DisconnectedServer | ObjectTypes.VM | ObjectTypes.UserTemplate | ObjectTypes.RemoteSR | ObjectTypes.LocalSR);
            AddSeparator();
            AddItemToSearchFor(ObjectTypes.AllExcFolders);
            AddItemToSearchFor(ObjectTypes.AllIncFolders);
            AddSeparator();
            AddCustom();
        }

        void searchForComboButton_BeforePopup(object sender, System.EventArgs e)
        {
            savedTypes = GetSelectedItemTag();
        }

        private void searchForComboButton_itemSelected(object sender, System.EventArgs e)
        {
            ObjectTypes types = GetSelectedItemTag();
            if (types != CUSTOM)
                return;

            if (!autoSelecting)
            {
                // Launch custom dlg. Dependent on OK/Cancel, save result and continue, or quit
                SearchForCustom sfc = new SearchForCustom(typeNames, customValue);
                sfc.ShowDialog(Program.MainWindow);
                if (sfc.DialogResult == DialogResult.Cancel)
                {
                    autoSelecting = true;
                    SetFromScope(savedTypes);  // reset combo button to value before Custom...
                    autoSelecting = false;
                    return;
                }
                customValue = sfc.Selected;
            }

            OnQueryChanged();
        }

        private void searchForComboButton_selChanged(object sender, EventArgs e)
        {
            ObjectTypes types = GetSelectedItemTag();
            if (types == CUSTOM)
                return;  // CUSTOM is dealt with in searchForComboButton_itemSelected instead

            OnQueryChanged();
        }

        public QueryScope QueryScope
        {
            get
            {
                return GetAsScope();
            }
            set
            {
                autoSelecting = true;
                customValue = value.ObjectTypes;
                SetFromScope(value);
                autoSelecting = false;
            }
        }

        private ObjectTypes GetSelectedItemTag()
        {
            return (searchForComboButton.SelectedItem == null ? ObjectTypes.None :
                (ObjectTypes)searchForComboButton.SelectedItem.Tag);
        }

        private QueryScope GetAsScope()
        {
            ObjectTypes types = GetSelectedItemTag();
            if (types == CUSTOM)  // if we're on Custom..., look it up from the custom setting
                return new QueryScope(customValue);
            else  // else just report what we're showing
                return new QueryScope(types);
        }

        private void SetFromScope(QueryScope value)
        {
            // Can we represent it by one of the fixed types?
            bool bDone = searchForComboButton.SelectItem<ObjectTypes>(
                delegate(ObjectTypes types)
                {
                    return (types == value.ObjectTypes);
                }
            );

            // If not, set it to "Custom..."
            if (!bDone)
                bDone = searchForComboButton.SelectItem<ObjectTypes>(
                    delegate(ObjectTypes types)
                    {
                        return (types == CUSTOM);
                    }
                );
        }

        private void SetFromScope(ObjectTypes types)
        {
            SetFromScope(new QueryScope(types));
        }
    }
}