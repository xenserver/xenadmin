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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAdmin.XenSearch;
using XenAPI;
using XenAdmin.Commands;


namespace XenAdmin.Dialogs
{
    public partial class FolderChangeDialog : XenDialogBase
    {
        private readonly string orig_folder;

        public FolderChangeDialog(string orig_folder)
            : base(null)
        {
            this.orig_folder = orig_folder;

            InitializeComponent();
            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
            XenConnections_CollectionChanged(null, null);

            treeView.BeginUpdate();
            treeView.ImageList = new ImageList();
            treeView.ImageList.Images.Add("folder", Properties.Resources._000_Folder_open_h32bit_16);  // if there is only one image, all nodes will get it
            treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            treeView.ImageList.TransparentColor = Color.Transparent;
            treeView.expandOnDoubleClick = false;
            InitState();
            PopulateTree();
            treeView.EndUpdate();
        }

        protected override void OnClosed(EventArgs e)
        {
            ConnectionsManager.XenConnections.CollectionChanged -= XenConnections_CollectionChanged;
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                connection.Cache.DeregisterBatchCollectionChanged<Folder>(foldersChanged);
        }
        
        private void EmptyTree()
        {
            treeView.Nodes.Clear();
        }

        private void PopulateTree()
        {
            VirtualTreeNode selectedNode = treeView.SelectedNode;
            Folder selectedFolder = selectedNode==null?null: selectedNode.Tag as Folder;

            EmptyTree();

            TreeNodeGroupAcceptor tnga = new TreeNodeGroupAcceptor(treeView, true);
            Search.SearchForAllFolders().PopulateAdapters(tnga);

            if (selectedFolder != null)
            {
                // try the one we were on: if that's no longer present, back off to the one we started with
                if (!SelectNode(selectedFolder.opaque_ref))
                {
                    string folder = orig_folder;
                    SelectNode(folder);
                }
            }
            else
                SelectNode(null);
        }

        private void InitState()
        {
            SelectNode(orig_folder);
            if (orig_folder == "")
            {
                radioButtonNone.Checked = true;
            }
            else
            {
                radioButtonChoose.Checked = true;
                ActiveControl = treeView;
            }
        }

        // Select the named node. If found it, return true.
        // If not, select the first node (if there are any) and return false.
        private bool SelectNode(string tag)
        {
            VirtualTreeNode node = FindNode(tag);
            if (node != null)
            {
                treeView.SelectedNode = node;
                return true;
            }
            else if (treeView.Nodes.Count != 0)
                treeView.SelectedNode = treeView.Nodes[0];
            return false;
        }

        private void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                connection.Cache.RegisterBatchCollectionChanged<Folder>(foldersChanged);
        }

        private void foldersChanged(object sender, EventArgs e)
        {
            PopulateTree();
        }

        private VirtualTreeNode FindNode(string tag)
        {
            foreach (VirtualTreeNode node in treeView.Nodes)
            {
                VirtualTreeNode found = FindNode(tag, node);
                if (found != null)
                    return found;
            }
            return null;
        }

        private VirtualTreeNode FindNode(string tag, VirtualTreeNode node)
        {
            Folder folder = node.Tag as Folder;

            if (folder != null && folder.opaque_ref == tag)
                return node;

            foreach (VirtualTreeNode subNode in node.Nodes)
            {
                VirtualTreeNode found = FindNode(tag, subNode);
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
                else
                    return null;
            }
        }

        public bool FolderChanged
        {
            get
            {
                Folder folder = CurrentFolder;
                string folderName = (folder == null ? String.Empty : folder.opaque_ref);
                return (orig_folder != folderName);
            }
        }

        private void EnableTreeView(bool enabled)
        {
            treeView.Enabled = enabled;
            newButton.Enabled = enabled;
        }

        private void SetOKButtonEnablement()
        {
            okButton.Enabled = FolderChanged;
        }

        private void radioButtonNone_Click(object sender, EventArgs e)
        {
            EnableTreeView(false);
            SetOKButtonEnablement();
        }

        private void radioButtonChoose_Click(object sender, EventArgs e)
        {
            EnableTreeView(true);
            SetOKButtonEnablement();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            new NewFolderCommand(Program.MainWindow, null, this).Execute();
        }

        private void newMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
                return;

            Folder folder = item.Tag as Folder;
            if (folder == null)
                return;

            new NewFolderCommand(Program.MainWindow, folder, this).Execute();
        }

        private void treeView_NodeMouseClick(object sender, VirtualTreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.Node == null)
                return;

            Folder folder = e.Node.Tag as Folder;
            if (folder == null)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item = new ToolStripMenuItem(Messages.NEW_FOLDER, Resources._000_Folder_open_h32bit_16, newMenuItem_Click);
            item.Tag = folder;
            menu.Items.Add(item);
            menu.Show(treeView, e.Location);
        }

        private void treeView_AfterSelect(object sender, VirtualTreeViewEventArgs e)
        {
            SetOKButtonEnablement();
        }

        private void treeView_NodeMouseDoubleClick(object sender, VirtualTreeNodeMouseClickEventArgs e)
        {
            if (okButton.Enabled)
                okButton_Click(sender, null);
        }
    }
}