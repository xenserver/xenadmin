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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Model;
using XenAPI;
using XenAdmin.XenSearch;
using XenAdmin.Core;
using System.Drawing;


namespace XenAdmin
{
    internal class MainWindowTreeBuilder
    {
        private readonly FlickerFreeTreeView _treeView;
        private readonly Color _treeViewForeColor;
        private readonly Color _treeViewBackColor;
        private object _highlightedDragTarget;
        private string _lastSearchText = string.Empty;
        private TreeSearchBox.Mode _lastSearchMode;
        private readonly List<VirtualTreeNode.PersistenceInfo> _infrastructureViewExpandedTags = new List<VirtualTreeNode.PersistenceInfo>();
        private readonly List<VirtualTreeNode.PersistenceInfo> _objectViewExpandedTags = new List<VirtualTreeNode.PersistenceInfo>();
        private readonly List<VirtualTreeNode.PersistenceInfo> _organizationViewExpandedTags = new List<VirtualTreeNode.PersistenceInfo>();
        private bool _rootExpanded;

        public MainWindowTreeBuilder(FlickerFreeTreeView treeView)
        {
            Util.ThrowIfParameterNull(treeView, "treeView");
            Program.AssertOnEventThread();

            _treeView = treeView;
            _treeViewForeColor = treeView.ForeColor;
            _treeViewBackColor = treeView.BackColor;

            SetDefaultObjectViewExpandedNodes();
            SetOrganizationViewExpandedNodes();
        }

        private void SetDefaultObjectViewExpandedNodes()
        {
            VirtualTreeNode dummyRootNode = new VirtualTreeNode(Messages.VIEW_OBJECTS);
            OrganizationalView.PopulateObjectView(CreateGroupAcceptor(dummyRootNode), TreeSearch.DefaultTreeSearch);

            foreach (VirtualTreeNode n in dummyRootNode.Nodes)
                _objectViewExpandedTags.Add(n.GetPersistenceInfo());
        }

        private void SetOrganizationViewExpandedNodes()
        {
            VirtualTreeNode dummyRootNode = new VirtualTreeNode(Messages.VIEW_ORGANIZATION);
            OrganizationalView.PopulateOrganizationView(CreateGroupAcceptor(dummyRootNode), TreeSearch.DefaultTreeSearch);

            foreach (VirtualTreeNode n in dummyRootNode.Nodes)
                _organizationViewExpandedTags.Add(n.GetPersistenceInfo());
         }

        /// <summary>
        /// Gets or sets an object that should be highlighted.
        /// </summary>
        public object HighlightedDragTarget
        {
            get { return _highlightedDragTarget; }
            set { _highlightedDragTarget = value; }
        }

        /// <summary>
        /// Updates the <see cref="TreeView"/> with the specified new root node. This is done by merging the specified
        /// root node with the existing root node to minimize updates to the treeview to reduce flicker.
        /// </summary>
        /// <param name="newRootNode">The new root node.</param>
        /// <param name="searchText">The search text for the currently active search.</param>
        public void RefreshTreeView(VirtualTreeNode newRootNode, string searchText, TreeSearchBox.Mode searchMode)
        {
            Util.ThrowIfParameterNull(newRootNode, "newRootNode");
            Util.ThrowIfParameterNull(searchText, "searchText");

            Program.AssertOnEventThread();

            _treeView.BeginUpdate();

            PersistExpandedNodes(searchText);

            _treeView.UpdateRootNodes(new[] { newRootNode });

            RestoreExpandedNodes(searchText, searchMode);

            bool searchTextCleared = (searchText.Length == 0 && searchText != _lastSearchText);

            _lastSearchText = searchText;
            _lastSearchMode = searchMode;
            _treeView.EndUpdate();

            // ensure that the selected nodes are visible when search text is cleared (CA-102127)
            if (searchTextCleared)
            {
                ExpandSelection();
            }
        }

        internal void ExpandSelection()
        {
            foreach (var node in _treeView.SelectedNodes)
            {
                node.EnsureVisible();
            }
        }

        public VirtualTreeNode CreateNewRootNode(Search search, TreeSearchBox.Mode mode)
        {
            VirtualTreeNode newRootNode;
            MainWindowTreeNodeGroupAcceptor groupAcceptor;

            switch (mode)
            {
                case TreeSearchBox.Mode.Objects:
                    newRootNode = new VirtualTreeNode(Messages.VIEW_OBJECTS);
                    groupAcceptor = CreateGroupAcceptor(_highlightedDragTarget, newRootNode);
                    OrganizationalView.PopulateObjectView(groupAcceptor, search);
                    break;
                case TreeSearchBox.Mode.Organization:
                    newRootNode = new VirtualTreeNode(Messages.VIEW_ORGANIZATION);
                    groupAcceptor = CreateGroupAcceptor(_highlightedDragTarget, newRootNode);
                    OrganizationalView.PopulateOrganizationView(groupAcceptor, search);
                    break;
                default:
                    Util.ThrowIfParameterNull(search, "search");
                    newRootNode = new VirtualTreeNode("XenCenter");
                    groupAcceptor = CreateGroupAcceptor(_highlightedDragTarget, newRootNode);
                    search.PopulateAdapters(groupAcceptor);
                    break;
            }

            return newRootNode;
        }

        private MainWindowTreeNodeGroupAcceptor CreateGroupAcceptor(VirtualTreeNode parent)
        {
            return CreateGroupAcceptor(null, parent);
        }

        private MainWindowTreeNodeGroupAcceptor CreateGroupAcceptor(object dragTarget, VirtualTreeNode parent)
        {
            return new MainWindowTreeNodeGroupAcceptor(dragTarget, _treeViewForeColor, _treeViewBackColor, parent);
        }

        private void AssignList(ref List<VirtualTreeNode.PersistenceInfo> list, TreeSearchBox.Mode mode)
        {
            switch (mode)
            {
                case TreeSearchBox.Mode.Objects:
                    list = _objectViewExpandedTags;
                    break;
                case TreeSearchBox.Mode.Organization:
                    list = _organizationViewExpandedTags;
                    break;
                default:
                    list = _infrastructureViewExpandedTags;
                    break;
            }
        }

        private void PersistExpandedNodes(string searchText)
        {
            // only persist the expansion state of nodes if there isn't an active search.
            //If there's a search then we're just going to expand everything later.

            // also because we wan't to restore the nodes back to their original state
            //after a search is removed, do the check on _lastSearchText and _lastSearchMode.

            if (searchText.Length == 0 && _lastSearchText.Length == 0 && _lastSearchMode != TreeSearchBox.Mode.SavedSearch)
            {
                List < VirtualTreeNode.PersistenceInfo > list = null;
                AssignList(ref list, _lastSearchMode);
                list.Clear();

                foreach (VirtualTreeNode node in _treeView.AllNodes.Where(n => n.Tag != null && n.IsExpanded))
                    list.Add(node.GetPersistenceInfo());
            }

            // persist the expansion state of the root node separately - it's a special case as its
            // expansion state isn't persisted differently depending on whether it's in org mode or not.
            _rootExpanded = _treeView.Nodes[0].IsExpanded || _treeView.Nodes[0].Nodes.Count == 0;
        }

        private void RestoreExpandedNodes(string searchText, TreeSearchBox.Mode searchMode)
        {
            if ((searchText != _lastSearchText && searchText.Length > 0) || (searchMode == TreeSearchBox.Mode.SavedSearch && _lastSearchMode != TreeSearchBox.Mode.SavedSearch))
            {
                // expand all nodes if there's a search and the search has changed.
                _treeView.ExpandAll();
            }

            if (searchText.Length == 0 && searchMode != TreeSearchBox.Mode.SavedSearch)
            {
                // if there isn't a search persist the user's expanded nodes.
                List < VirtualTreeNode.PersistenceInfo > list = null;
                AssignList(ref list, searchMode);

                var allNodes = new List<VirtualTreeNode>(_treeView.AllNodes);
                
                foreach (VirtualTreeNode.PersistenceInfo info in list)
                {
                    VirtualTreeNode match;

                    // First, look for the object in the same position
                    if (_treeView.TryExactMatch(info.Path, out match) >= info.Path.Count)
                    {
                        if (!match.IsExpanded)
                        {
                            match.Expand();
                        }
                        allNodes.Remove(match);
                        continue;
                    }

                    // Second, find the object in the maximal sub tree where it appeared only once
                    if (_treeView.TryExactMatch(info.PathToMaximalSubTree, out match) >= info.PathToMaximalSubTree.Count)
                    {
                        match = _treeView.FindNodeIn(match, info.Tag);
                        if (match != null)
                        {
                            if (!match.IsExpanded)
                            {
                                match.Expand();
                            }
                            allNodes.Remove(match);
                        }
                    }
                }

                // collapse everything else
                foreach (VirtualTreeNode n in allNodes)
                {
                    if (n.Tag != null && n.IsExpanded)
                    {
                        n.Collapse();
                    }
                }

                // special case for root node
                if (_rootExpanded)
                {
                    _treeView.Nodes[0].Expand();
                }
            }
        }

        #region MainWindowTreeNodeGroupAcceptor class

        private class MainWindowTreeNodeGroupAcceptor : IAcceptGroups
        {
            private readonly VirtualTreeNode _parent;
            private readonly Color _treeViewForeColor;
            private readonly Color _treeViewBackColor;
            private readonly object _highlightedDragTarget;
            private int _index;

            public MainWindowTreeNodeGroupAcceptor(object highlightedDragTarget, Color treeViewForeColor, Color treeViewBackColor, VirtualTreeNode parent)
            {
                _parent = parent;
                _treeViewForeColor = treeViewForeColor;
                _treeViewBackColor = treeViewBackColor;
                _highlightedDragTarget = highlightedDragTarget;
            }

            #region IAcceptGroups Members

            public void FinishedInThisGroup(bool defaultExpand)
            {
            }

            public IAcceptGroups Add(Grouping grouping, Object group, int indent)
            {
                if (group != null)
                {
                    VirtualTreeNode node;

                    if (group is Pool)
                    {
                        node = AddPoolNode(_parent, _index, (Pool)group);
                    }
                    else if (group is Host)
                    {
                        node = AddHostNode(_parent, _index, (Host)group);
                    }
                    else if (group is VM)
                    {
                        node = AddVMNode(_parent, _index, (VM)group);
                    }
					else if (group is VM_appliance)
					{
						node = AddVmApplianceNode(_parent, _index, (VM_appliance)group);
					}
					else if (group is SR)
                    {
                        node = AddSRNode(_parent, _index, (SR)group);
                    }
                    else if (group is XenAPI.Network)
                    {
                        node = AddNetworkNode(_parent, _index, (XenAPI.Network)group);
                    }
                    else if (group is VDI)
                    {
                        node = AddVDINode(_parent, _index, (VDI)group);
                    }
                    else if (group is Folder)
                    {
                        node = AddFolderNode(_parent, _index, (Folder)group);
                    }
                    else if (group is StorageLinkServer)
                    {
                        node = AddStorageLinkServerNode(_parent, _index, (StorageLinkServer)group);
                    }
                    else if (group is StorageLinkSystem)
                    {
                        node = AddStorageLinkSystemNode(_parent, _index, (StorageLinkSystem)group);
                    }
                    else if (group is StorageLinkPool)
                    {
                        node = AddStorageLinkPoolNode(_parent, _index, (StorageLinkPool)group);
                    }
                    else if (group is StorageLinkVolume)
                    {
                        node = AddStorageLinkVolumeNode(_parent, _index, (StorageLinkVolume)group);
                    }
                    else if (group is StorageLinkRepository)
                    {
                        node = AddStorageLinkRepositoryNode(_parent, _index, (StorageLinkRepository)group);
                    }
                    else
                    {
                        node = AddNode(_parent, _index, grouping.GetGroupName(group), grouping.GetGroupIcon(group), false, new GroupingTag(grouping, FindGroupingParent(_parent.Tag), group));

                        if (group is DateTime)
                            Program.BeginInvoke(Program.MainWindow, () => { node.Text = HelpersGUI.DateTimeToString((DateTime)group, Messages.DATEFORMAT_DMY_HMS, true); });   // annoying: has to be on the event thread because of CA-46983
                    }

                    _index++;

                    return new MainWindowTreeNodeGroupAcceptor(_highlightedDragTarget, _treeViewForeColor, _treeViewBackColor, node);
                }
                return null;
            }

            #endregion

            private static object FindGroupingParent(object parentTag)
            {
                if (parentTag is GroupingTag)
                {
                    GroupingTag gt = (GroupingTag)parentTag;
                    // gt.Grouping is OrganizationalView means that we're at the Tags/Types/Custom Fields level.
                    // Custom field keys are the next level down, and values are the one after that.
                    return gt.Grouping is OrganizationalView ? null : gt.Group;
                }
                else
                {
                    return parentTag;
                }
            }

            private VirtualTreeNode AddStorageLinkServerNode(VirtualTreeNode parent, int index, StorageLinkServer storageLinkServer)
            {
                return AddNode(parent, index, storageLinkServer.Name, Images.GetIconFor(storageLinkServer), false, storageLinkServer);
            }

            private VirtualTreeNode AddStorageLinkSystemNode(VirtualTreeNode parent, int index, StorageLinkSystem storageLinkSystem)
            {
                return AddNode(parent, index, Helpers.GetName(storageLinkSystem), Images.GetIconFor(storageLinkSystem), false, storageLinkSystem);
            }

            private VirtualTreeNode AddStorageLinkPoolNode(VirtualTreeNode parent, int index, StorageLinkPool storageLinkPool)
            {
                return AddNode(parent, index, Helpers.GetName(storageLinkPool), Images.GetIconFor(storageLinkPool), false, storageLinkPool);
            }

            private VirtualTreeNode AddStorageLinkVolumeNode(VirtualTreeNode parent, int index, StorageLinkVolume storageLinkVolume)
            {
                string name = storageLinkVolume.FriendlyName;
                VDI vdi = storageLinkVolume.VDI(ConnectionsManager.XenConnectionsCopy);

                if (vdi != null)
                {
                    name = string.Format("{0} ({1})", vdi.Name, name);
                }

                return AddNode(parent, index, name, Images.GetIconFor(storageLinkVolume), false, storageLinkVolume);
            }

            private VirtualTreeNode AddStorageLinkRepositoryNode(VirtualTreeNode parent, int index, StorageLinkRepository storageLinkRepository)
            {
                string name = storageLinkRepository.Name;
                StorageLinkPool pool = storageLinkRepository.StorageLinkPool;

                if (pool != null && !string.IsNullOrEmpty(pool.ParentStorageLinkPoolId))
                {
                    name = string.Format("{0} ({1})", storageLinkRepository.Name, pool);
                }

                return AddNode(parent, index, name, Images.GetIconFor(storageLinkRepository), false, storageLinkRepository);
            }

            private VirtualTreeNode AddPoolNode(VirtualTreeNode parent, int index, Pool pool)
            {
                return AddNode(parent, index, Helpers.GetName(pool), Images.GetIconFor(pool), false, pool);
            }

            private VirtualTreeNode AddVMNode(VirtualTreeNode parent, int index, VM vm)
            {
                bool hidden = vm.IsHidden;
                string name = hidden ? String.Format(Messages.X_HIDDEN, vm.Name) : vm.Name;
                return AddNode(parent, index, name, Images.GetIconFor(vm), hidden, vm);
            }

			private VirtualTreeNode AddVmApplianceNode(VirtualTreeNode parent, int index, VM_appliance appliance)
			{
				return AddNode(parent, index, appliance.Name, Images.GetIconFor(appliance), false, appliance);
			}

            private VirtualTreeNode AddHostNode(VirtualTreeNode parent, int index, Host host)
            {
                return AddNode(parent, index, host.Name, Images.GetIconFor(host), false, host);
            }

            private VirtualTreeNode AddSRNode(VirtualTreeNode parent, int index, SR sr)
            {
                bool hidden = sr.IsHidden;
                String name = hidden ? String.Format(Messages.X_HIDDEN, sr.NameWithoutHost) : sr.NameWithoutHost;
                return AddNode(parent, index, name, Images.GetIconFor(sr), hidden, sr);
            }

            private VirtualTreeNode AddNetworkNode(VirtualTreeNode parent, int index, XenAPI.Network network)
            {
                bool hidden = network.IsHidden;
                bool slave = network.IsSlave;
                string rawName = network.Name;
                String name = slave ? String.Format(Messages.NIC_SLAVE, rawName) :
                    hidden ? String.Format(Messages.X_HIDDEN, rawName) :
                    rawName;
                return AddNode(parent, index, name, Images.GetIconFor(network), slave || hidden, network);
            }

            private VirtualTreeNode AddVDINode(VirtualTreeNode parent, int index, VDI vdi)
            {
                String name = String.IsNullOrEmpty(vdi.Name) ? Messages.NO_NAME : vdi.Name;
                return AddNode(parent, index, name, Images.GetIconFor(vdi), false, vdi);
            }

            private VirtualTreeNode AddFolderNode(VirtualTreeNode parent, int index, Folder folder)
            {
                return AddNode(parent, index, folder.Name, Images.GetIconFor(folder), false, folder);
            }

            private VirtualTreeNode AddNode(VirtualTreeNode parent, int index, string name, Icons icon, bool grayed, object obj)
            {
                VirtualTreeNode result = new VirtualTreeNode(name.Ellipsise(1000));

                IXenObject xenObject = obj as IXenObject;
                result.Tag = obj;
                parent.Nodes.Insert(index, result);
                result.ImageIndex = (int)icon;
                result.SelectedImageIndex = (int)icon;
                bool error = xenObject != null && xenObject.InError;
                bool highlighted = _highlightedDragTarget != null && obj != null && _highlightedDragTarget.Equals(obj);

                if (highlighted)
                {
                    result.BackColor = SystemColors.Highlight;
                    result.ForeColor = SystemColors.HighlightText;
                    result.NodeFont = Program.DefaultFont;
                }
                else if (error)
                {
                    result.BackColor = Program.ErrorBackColor;
                    result.ForeColor = Program.ErrorForeColor;
                    result.NodeFont = Program.DefaultFont;
                }
                else if (grayed)
                {
                    result.BackColor = _treeViewBackColor;
                    result.ForeColor = SystemColors.GrayText;
                    result.NodeFont = Program.DefaultFont;
                }
                else
                {
                    result.BackColor = _treeViewBackColor;
                    result.ForeColor = _treeViewForeColor;
                    result.NodeFont = Program.DefaultFont;
                }

                return result;
            }
        }

        #endregion
    }
}
