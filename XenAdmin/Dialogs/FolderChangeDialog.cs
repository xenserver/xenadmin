/* Copyright (c) Cloud Software Group, Inc. 
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAdmin.XenSearch;
using XenAdmin.Commands;


namespace XenAdmin.Dialogs
{
    public partial class FolderChangeDialog : XenDialogBase
    {
        private readonly string originalFolderRef;
        private string selectedFolderRef;

        public FolderChangeDialog(string originalFolderRef = null)
            : base(null)
        {
            InitializeComponent();

            this.originalFolderRef = originalFolderRef;

            var imgList = new ImageList {ColorDepth = ColorDepth.Depth32Bit, TransparentColor = Color.Transparent};
            imgList.Images.Add("folder", Resources._000_Folder_open_h32bit_16);
            treeView.ImageList = imgList;

            treeView.expandOnDoubleClick = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
            XenConnections_CollectionChanged(null, null);

            selectedFolderRef = originalFolderRef;

            if (string.IsNullOrEmpty(selectedFolderRef))
            {
                radioButtonNone.Checked = true;
            }
            else
            {
                radioButtonChoose.Checked = true;
                ActiveControl = treeView;
            }

            PopulateTree();
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            ConnectionsManager.XenConnections.CollectionChanged -= XenConnections_CollectionChanged;
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                connection.Cache.DeregisterBatchCollectionChanged<Folder>(FoldersChanged);
        }

        private void PopulateTree()
        {
            try
            {
                treeView.BeginUpdate();

                //repopulate the tree
                treeView.Nodes.Clear();
                var tnga = new TreeNodeGroupAcceptor(treeView, true);
                Search.SearchForAllFolders().PopulateAdapters(tnga);
                    
                //restore selection
                SelectNodeByRef(selectedFolderRef);
            }
            finally
            {
                treeView.EndUpdate();
            }
        }

        private void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                connection.Cache.RegisterBatchCollectionChanged<Folder>(FoldersChanged);
        }

        private void FoldersChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, PopulateTree);
        }

        private void SelectNodeByRef(string folderRef)
        {
            var node = FindNodeByRef(folderRef);
            if (node == null)
            {
                treeView.SelectedNode = null;
            }
            else
            {
                treeView.SelectedNode = node;

                var folder = treeView.SelectedNode.Tag as Folder;
                if (folder != null)
                    selectedFolderRef = folder.opaque_ref;
                
                treeView.SelectedNode.EnsureVisible();
            }
        }

        private VirtualTreeNode FindNodeByRef(string folderRef)
        {
            if (string.IsNullOrEmpty(folderRef))
                return null;

            foreach (VirtualTreeNode node in treeView.Nodes)
            {
                VirtualTreeNode found = FindNodeByRef(folderRef, node);
                if (found != null)
                    return found;
            }
            return null;
        }

        private VirtualTreeNode FindNodeByRef(string folderRef, VirtualTreeNode node)
        {
            if (string.IsNullOrEmpty(folderRef))
                return null;

            Folder folder = node.Tag as Folder;

            if (folder != null && folder.opaque_ref == folderRef)
                return node;

            foreach (VirtualTreeNode subNode in node.Nodes)
            {
                VirtualTreeNode found = FindNodeByRef(folderRef, subNode);
                if (found != null)
                    return found;
            }

            return null;
        }

        public Folder CurrentFolder
        {
            get
            {
                if (radioButtonChoose.Checked && treeView.SelectedNode != null)
                    return treeView.SelectedNode.Tag as Folder;
                
                return null;
            }
        }

        public bool FolderChanged
        {
            get
            {
                Folder folder = CurrentFolder;
                if (folder == null)
                    return !string.IsNullOrEmpty(originalFolderRef);

                return folder.opaque_ref != originalFolderRef;
            }
        }

        private void CreateNewFolder(Folder folder = null)
        {
            var cmd = new NewFolderCommand(Program.MainWindow, folder, this);
            cmd.FoldersCreated += cmd_FoldersCreated;
            cmd.Run();
        }

        private void cmd_FoldersCreated(string[] obj)
        {
            if (obj != null && obj.Length > 0)
                Program.Invoke(this, () => SelectNodeByRef(obj[0]));
        }

        private void RenameFolder()
        {
            if (treeView.SelectedNode != null)
            {
                var folder = treeView.SelectedNode.Tag as Folder;
                if (folder == null)
                    return;

                using (var dialog = new InputPromptDialog {
                    Text = Messages.RENAME_FOLDER_TITLE,
                    PromptText = Messages.NEW_FOLDER_NAME,
                    InputText = folder.Name(),
                    HelpID = "NewFolderDialog"
                })
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;
                    selectedFolderRef = Folders.AppendPath(folder.Path, dialog.InputText);
                    new RenameFolderCommand(Program.MainWindow, folder, dialog.InputText).Run();
                }
            }
        }

        private void DeleteFolder()
        {
            if (treeView.SelectedNode != null)
            {
                var folder = treeView.SelectedNode.Tag as Folder;
                selectedFolderRef = folder == null || folder.Parent == null ? null : folder.Parent.opaque_ref;
                new DeleteFolderCommand(Program.MainWindow, folder).Run();
            }
        }

        private void EnableButtons()
        {
            okButton.Enabled = radioButtonNone.Checked ||
                               (radioButtonChoose.Checked && treeView.SelectedNode != null);

            buttonRename.Enabled = buttonDelete.Enabled = radioButtonChoose.Checked && treeView.SelectedNode != null;
            toolStripMenuItemRename.Visible = buttonRename.Enabled;
            toolStripMenuItemDelete.Visible = buttonDelete.Enabled;
        }

        #region Control event Handlers

        private void radioButtonNone_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNone.Checked)
            {
                treeView.SelectedNode = null;
                EnableButtons();
            }
        }

        private void radioButtonChoose_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonChoose.Checked)
            {
                SelectNodeByRef(selectedFolderRef);
                EnableButtons();
            }
        }

        private void radioButtonNone_TabStopChanged(object sender, EventArgs e)
        {
            if (!radioButtonNone.TabStop)
                radioButtonNone.TabStop = true;
        }

        private void radioButtonChoose_TabStopChanged(object sender, EventArgs e)
        {
            if (!radioButtonChoose.TabStop)
                radioButtonChoose.TabStop = true;
        }


        private void buttonNew_Click(object sender, EventArgs e)
        {
            CreateNewFolder();
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            RenameFolder();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeleteFolder();
        }


        private void toolStripMenuItemRename_Click(object sender, EventArgs e)
        {
            RenameFolder();
        }

        private void toolStripMenuItemNew_Click(object sender, EventArgs e)
        {
            Folder folder = null;
            if (treeView.SelectedNode != null)
                folder = treeView.SelectedNode.Tag as Folder;

            CreateNewFolder(folder);
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            DeleteFolder();
        }


        private void treeView_Enter(object sender, EventArgs e)
        {
            radioButtonChoose.Checked = true;
        }

        private void treeView_NodeMouseDoubleClick(object sender, VirtualTreeNodeMouseClickEventArgs e)
        {
            if (radioButtonChoose.Checked && treeView.SelectedNode != null && e.Node == treeView.SelectedNode)
                DialogResult = DialogResult.OK;
        }

        private void treeView_SelectionsChanged(object sender, EventArgs e)
        {
            // if the SelecteNode is null, it could be a result of the user
            // either having clicked radioButtonNone or whitespace in the treeview,
            // in which case do not deselect the radioButtonChoose

            if (treeView.SelectedNode != null)
            {
                radioButtonChoose.Checked = true;

                var folder = treeView.SelectedNode.Tag as Folder;
                if (folder != null)
                    selectedFolderRef = folder.opaque_ref;
            }

            EnableButtons();
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteFolder();
            else if (e.KeyCode == Keys.F2)
                RenameFolder();
        }

        #endregion
    }
}
