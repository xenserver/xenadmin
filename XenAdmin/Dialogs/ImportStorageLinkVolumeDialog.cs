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
using System.Collections.ObjectModel;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using System.Diagnostics;
using XenAdmin.Properties;
using System.Drawing;

namespace XenAdmin.Dialogs
{
    public partial class ImportStorageLinkVolumeDialog : XenDialogBase
    {
        private readonly StorageLinkRepository _slr;
        private readonly List<VirtualTreeNode> _canonRootNodes;

        public ImportStorageLinkVolumeDialog(StorageLinkRepository slr)
        {
            Util.ThrowIfParameterNull(slr, "slr");
            _slr = slr;

            InitializeComponent();

            treeView.ImageList = Images.ImageList16;
            listView.SmallImageList = Images.ImageList16;

            _canonRootNodes = PopulateCanonRootNodes(_slr);

            treeView.UpdateRootNodes(GetNewRootNodes());
            treeView.EndUpdate();

            // expand storage-system and top level pools by default
            new List<VirtualTreeNode>(treeView.Nodes).ForEach(n => n.Expand());

            if (treeView.Nodes.Count > 0)
            {
                treeView.Nodes[0].EnsureVisible();

                treeView.AfterCheck += treeView_AfterCheck;
                listView.ItemChecked += listView_ItemChecked;

                // check clipboard. If it matches any node, then select that node
                string text = null;
                bool selected = false;

                try
                {
                    text = Clipboard.GetText().Trim();
                }
                catch (Exception)
                {
                }

                if (text != null)
                {
                    var node = new List<VirtualTreeNode>(treeView.AllNodes).Find(n => n.Text.ToLower() == text.ToLower());
                    if (node != null)
                    {
                        treeView.SelectedNode = node;
                        node.EnsureVisible();
                        selected = true;
                    }
                }

                if (!selected)
                {
                    // select the pool that the SR is in?
                }
            }
        }


        private static VirtualTreeNode CreateCanonNode(IStorageLinkObject o)
        {
            return new VirtualTreeNode(o.Name)
            {
                Tag = o,
                Name = o.opaque_ref,
                ImageIndex = (int)Images.GetIconFor(o),
                SelectedImageIndex = (int)Images.GetIconFor(o),
                ShowCheckBox = o is StorageLinkVolume
            };
        }

        private static ListViewItem CreateListViewItem(StorageLinkVolume vol)
        {
            var item = new ListViewItem(vol.Name)
            {
                Tag = vol,
                ImageIndex = (int)Images.GetIconFor(vol),
                Name = vol.opaque_ref,
                Checked = true
            };

            item.SubItems.Add(Util.DiskSizeString(vol.Capacity * 1024L * 1024L));

            return item;
        }

        private static List<VirtualTreeNode> PopulateCanonRootNodes(StorageLinkRepository slr)
        {
            List<VirtualTreeNode> output = new List<VirtualTreeNode>();

            foreach (StorageLinkPool pool in slr.StorageLinkConnection.Cache.StoragePools)
            {
                if (pool.StorageLinkSystemId == slr.StorageLinkSystemId && string.IsNullOrEmpty(pool.ParentStorageLinkPoolId))
                {
                    output.Add(CreateCanonNode(pool));
                }
            }

            var dontShow = new List<string>();
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy.FindAll(c => c.IsConnected && Helpers.MidnightRideOrGreater(c)))
            {
                foreach (VDI vdi in c.Cache.VDIs)
                {
                    var otherConfig = new Dictionary<string, string>(vdi.sm_config);
                    if (otherConfig.ContainsKey("SVID"))
                    {
                        dontShow.Add(otherConfig["SVID"]);
                    }
                }
            }

            foreach (StorageLinkVolume volume in slr.StorageLinkConnection.Cache.StorageVolumes)
            {
                if (volume.StorageLinkSystemId == slr.StorageLinkSystemId && !dontShow.Contains(volume.opaque_ref))
                {
                    output.Find(n => n.Name == volume.RootStorageLinkPool.opaque_ref).Nodes.Add(CreateCanonNode(volume));
                }
            }

            return output;
        }

        private void treeView_AfterCheck(object sender, VirtualTreeViewEventArgs e)
        {
            var volume = (StorageLinkVolume)e.Node.Tag;
            if (!e.Node.Checked)
            {
                listView.Items.RemoveByKey(volume.opaque_ref);
            }
            else
            {
                listView.ItemChecked -= listView_ItemChecked;
                listView.Items.Add(CreateListViewItem(volume));
                listView.ItemChecked += listView_ItemChecked;
            }
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!e.Item.Checked)
            {
                treeView.AfterCheck -= treeView_AfterCheck;

                var nodes = treeView.Nodes.Find(n => n.Name == e.Item.Name, true);
                Debug.Assert(nodes != null && nodes.Length == 1);
                if (nodes != null && nodes.Length > 0)
                {
                    nodes[0].Checked = false;
                }
                treeView.AfterCheck += treeView_AfterCheck;

                listView.Items.Remove(e.Item);
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            // take copy of root-node with search text
            treeView.UpdateRootNodes(GetNewRootNodes());

            // set everything that matches the search to be expanded.
            new List<VirtualTreeNode>(treeView.AllNodes).FindAll(NodeMatches).ForEach(n => n.Expand());

            treeView.EndUpdate();
        }

        private List<VirtualTreeNode> GetNewRootNodes()
        {
            return _canonRootNodes.ConvertAll(n =>
                {
                    var nn = new VirtualTreeNode(string.Empty);
                    CopyNode(n, nn);
                    return nn;
                }).FindAll(NodeMatches);
        }

        private void CopyNode(VirtualTreeNode src, VirtualTreeNode dest)
        {
            StorageLinkVolume volume = src.Tag as StorageLinkVolume;

            dest.Text = src.Text;
            dest.Tag = src.Tag;
            dest.Name = src.Name;
            dest.ImageIndex = src.ImageIndex;
            dest.SelectedImageIndex = src.SelectedImageIndex;
            dest.ShowCheckBox = src.ShowCheckBox;
            dest.Checked = dest.ShowCheckBox && listView.Items.Find(src.Name, false).Length > 0;

            foreach (VirtualTreeNode n in src.Nodes)
            {
                if (NodeMatches(n))
                {
                    dest.Nodes.Add(new VirtualTreeNode(string.Empty));
                    CopyNode(n, dest.Nodes[dest.Nodes.Count - 1]);
                }
            }
        }

        private bool NodeTextMatches(VirtualTreeNode node)
        {
            return searchTextBox.Matches(node.Text);
        }

        private bool NodeMatches(VirtualTreeNode node)
        {
            if (searchTextBox.Text.Length == 0 ||
                NodeTextMatches(node) ||
                new List<VirtualTreeNode>(node.Ancestors).Find(NodeTextMatches) != null)
            {
                return true;
            }

            foreach (VirtualTreeNode n in node.Nodes)
            {
                if (NodeMatches(n))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            Image image = Resources.gripper;

            e.Graphics.DrawImageUnscaled(Resources.gripper, new Point(splitContainer1.Width / 2 - image.Width / 2, splitContainer1.Panel1.Height - 2));
        }

        public ReadOnlyCollection<string> Volumes
        {
            get
            {
                return new ReadOnlyCollection<string>(new List<string>(Util.PopulateList<ListViewItem>(listView.Items).ConvertAll(i => i.Name)));
            }
        }
    }
}
