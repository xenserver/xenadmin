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

using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace XenAdmin.Controls
{
    /// <summary>
    /// A <see cref="TreeView"/> that only adds <see cref="TreeNode"/>s when they become visible.
    /// </summary>
    public partial class VirtualTreeView : MultiSelectTreeView, IVirtualTreeNodeCollectionOwner, IHaveNodes
    {
        private readonly VirtualTreeNodeCollection _nodes;
        private readonly VirtualTreeSelectedNodeCollection _selectedNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualTreeView"/> class.
        /// </summary>
        public VirtualTreeView()
        {
            _nodes = new VirtualTreeNodeCollection(this);
            _selectedNodes = new VirtualTreeSelectedNodeCollection(this);
        }

        /// <summary>
        /// Gets or sets the tree node that is currently selected in the tree view control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="VirtualTreeNode"/> that is currently selected in the tree view control.
        /// </returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new VirtualTreeNode SelectedNode
        {
            get
            {
                return (VirtualTreeNode)base.SelectedNode;
            }
            set
            {
                if (value != null)
                {
                    Devirtualise(value);
                }
                base.SelectedNode = value;
            }
        }

        /// <summary>
        /// Gets or sets the first fully-visible tree node in the tree view control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="VirtualTreeNode"/> that represents the first fully-visible tree node in the tree view control.
        /// </returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new VirtualTreeNode TopNode
        {
            get { return (VirtualTreeNode)base.TopNode; }
            set
            {
                Util.ThrowIfParameterNull(value, "value");
                Devirtualise(value);
                base.TopNode = value;
            }
        }

        private static List<VirtualTreeNode> GetParents(VirtualTreeNode node)
        {
            List<VirtualTreeNode> parents = new List<VirtualTreeNode>();

            VirtualTreeNode nn = node.Parent;
            while (nn != null)
            {
                parents.Add(nn);
                nn = nn.Parent;
            }
            parents.Reverse();
            return parents;
        }

        /// <summary>
        /// Populates the real nodes for every node from the root down to this node.
        /// </summary>
        private void Devirtualise(VirtualTreeNode node)
        {
            foreach (VirtualTreeNode parent in GetParents(node))
            {
                Devirtualise(parent.Nodes);
            }
        }

        /// <summary>
        /// Ensures that all real nodes are up-to-date with the virtual ones.
        /// </summary>
        private void Devirtualise(VirtualTreeNodeCollection nodes)
        {
            if (NodeCollectionRequiresUpdate(nodes.Owner))
            {
                nodes.Owner.RealNodes.Clear();
                nodes.Owner.RealNodes.AddRange(new List<VirtualTreeNode>(nodes).ToArray());
            }
        }

        private static bool NodeCollectionRequiresUpdate(IVirtualTreeNodeCollectionOwner owner)
        {
            if (owner.RealNodes.Count == 1 && owner.RealNodes[0] is DummyTreeNode)
            {
                return true;
            }

            if (owner.RealNodes.Count != owner.Nodes.Count)
            {
                return true;
            }

            for (int i = 0; i < owner.Nodes.Count; i++)
            {
                if (owner.RealNodes[i] != owner.Nodes[i])
                {
                    return true;
                }
            }
            return false;
        }

        public new VirtualTreeSelectedNodeCollection SelectedNodes
        {
            get { return _selectedNodes; }
        }

        #region IVirtualTreeNodeCollectionOwner Members

        public new VirtualTreeNodeCollection Nodes
        {
            get { return _nodes; }
        }

        MultiSelectTreeNodeCollection IVirtualTreeNodeCollectionOwner.RealNodes
        {
            get { return base.Nodes; }
        }

        #endregion

        #region IHaveNodes Members

        VirtualTreeView.VirtualTreeNodeCollection IHaveNodes.Nodes
        {
            get { return Nodes; }
        }

        object IHaveNodes.Tag
        {
            get { return Tag; }
        }

        void IHaveNodes.Expand()
        {
        }

        void IHaveNodes.Collapse()
        {
        }

        #endregion

        /// <summary>
        /// Retrieves the tree node that is at the specified point.
        /// </summary>
        public new VirtualTreeNode GetNodeAt(Point pt)
        {
            return (VirtualTreeNode)base.GetNodeAt(pt);
        }

        /// <summary>
        /// Retrieves the tree node that is at the specified point.
        /// </summary>
        public new VirtualTreeNode GetNodeAt(int x, int y)
        {
            return (VirtualTreeNode)base.GetNodeAt(x, y);
        }

        /// <summary>
        /// Merges the specified node tree with the existing one. Automatically does a BeginUpdate
        /// if the nodes change.
        /// </summary>
        /// <param name="newRootNodes">The new root nodes.</param>
        public void UpdateRootNodes(IEnumerable<VirtualTreeNode> newRootNodes)
        {
            Util.ThrowIfParameterNull(newRootNodes, "newRootNodes");

            foreach (VirtualTreeNode node in newRootNodes)
            {
                if (node.TreeView != null)
                {
                    throw new ArgumentException("newRootNodes should not be attached to a TreeView.", "newRootNodes");
                }
            }

            bool doneBeginUpdate = false;
            UpdateNodes(Nodes, new List<VirtualTreeNode>(newRootNodes), ref doneBeginUpdate);
        }

        private void UpdateNodes(VirtualTreeNodeCollection oldNodes, IList<VirtualTreeNode> newNodes, ref bool doneBeginUpdate)
        {
            for (int i = 0; i < Math.Max(oldNodes.Count, newNodes.Count); )
            {
                if (oldNodes.Count > i && newNodes.Count > i)
                {
                    if (oldNodes[i].Nodes.Count > 0 && newNodes[i].Nodes.Count == 0 && oldNodes[i].IsExpanded)
                    {
                        // fix for issues CA-34486 and CA-36409
                        // see VirtualTreeViewTests.TestUpdateWhenRemoveAllChildNodesOfExandedParent

                        // When merging sets of tree-nodes using UpdateRootNodes, if you remove all of
                        // the child-nodes from a node, the node still remembers its IsExpanded state
                        // from before the nodes were removed regardless of whether you call Collapse()
                        // or Expand() after the nodes were removed.

                        // This causes problems for the Virtual treeview as it
                        // relies the BeforeExpanded event to convert DummyTreeNodes into VirtualTreeNodes
                        // on population.

                        oldNodes[i].Collapse();
                    }

                    UpdateNode(newNodes[i], oldNodes[i], ref doneBeginUpdate);
                    UpdateNodes(oldNodes[i].Nodes, newNodes[i].Nodes, ref doneBeginUpdate);
                    i++;
                }
                else if (oldNodes.Count <= i && newNodes.Count > i)
                {
                    LogTreeView("Adding node " + newNodes[i].Text);
                    DoBeginUpdateIfRequired(ref doneBeginUpdate);
                    oldNodes.Add(newNodes[i]);
                    UpdateNodes(oldNodes[i].Nodes, newNodes[i].Nodes, ref doneBeginUpdate);
                    i++;
                }
                else
                {
                    LogTreeView("Removing node " + oldNodes[i].Text);
                    DoBeginUpdateIfRequired(ref doneBeginUpdate);
                    oldNodes.RemoveAt(i);
                }
            }
        }

        private void DoBeginUpdateIfRequired(ref bool doneBeginUpdate)
        {
            if (!doneBeginUpdate)
            {
                doneBeginUpdate = true;
                BeginUpdate();
            }
        }

        [Conditional("DEBUG")]
        private void LogTreeView(string s)
        {
            //Debug.WriteLine(s);
        }

        private void UpdateNode(VirtualTreeNode src, VirtualTreeNode dest, ref bool doneBeginUpdate)
        {
            if (dest.ImageIndex != src.ImageIndex)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.ImageIndex = src.ImageIndex;
            }

            if (dest.SelectedImageIndex != src.SelectedImageIndex)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.SelectedImageIndex = src.SelectedImageIndex;
            }

            if (dest.Text != src.Text)
            {
                LogTreeView("Overwriting " + src.Text + " with " + dest.Text);
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.Text = src.Text;
            }

            if (dest.Tag != src.Tag)
            {
                dest.Tag = src.Tag;
            }

            if (dest.NodeFont != src.NodeFont)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.NodeFont = src.NodeFont;
            }

            if (dest.BackColor != src.BackColor)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.BackColor = src.BackColor;
            }

            if (dest.ForeColor != src.ForeColor)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.ForeColor = src.ForeColor;
            }

            if (dest.Name != src.Name)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.Name = src.Name;
            }

            if (dest.ShowCheckBox != src.ShowCheckBox)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.ShowCheckBox = src.ShowCheckBox;
            }

            if (dest.Checked != src.Checked)
            {
                DoBeginUpdateIfRequired(ref doneBeginUpdate);
                dest.Checked = src.Checked;
            }
        }

        /// <summary>
        /// Iterates through every node of this <see cref="VirtualTreeView"/>.
        /// </summary>
        public new IEnumerable<VirtualTreeNode> AllNodes
        {
            get
            {
                foreach (VirtualTreeNode node in Nodes)
                {
                    yield return node;

                    foreach (VirtualTreeNode n in node.Descendants)
                    {
                        yield return n;
                    }
                }
            }
        }

        #region Virtual overrides

        protected sealed override void OnAfterCheck(TreeViewEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnAfterCheck(new VirtualTreeViewEventArgs(node, e.Action));
        }

        protected sealed override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            VirtualTreeViewCancelEventArgs args = new VirtualTreeViewCancelEventArgs(node, e.Cancel, e.Action);
            OnBeforeCheck(args);
            e.Cancel = args.Cancel;
        }

        protected sealed override void OnAfterCollapse(TreeViewEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnAfterCollapse(new VirtualTreeViewEventArgs(node, e.Action));
        }

        protected sealed override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            VirtualTreeViewCancelEventArgs args = new VirtualTreeViewCancelEventArgs(node, e.Cancel, e.Action);
            OnBeforeCollapse(args);
            e.Cancel = args.Cancel;
        }

        protected sealed override void OnAfterExpand(TreeViewEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnAfterExpand(new VirtualTreeViewEventArgs(node, e.Action));
        }

        protected sealed override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            Devirtualise(node.Nodes);
            VirtualTreeViewCancelEventArgs args = new VirtualTreeViewCancelEventArgs(node, e.Cancel, e.Action);
            OnBeforeExpand(args);
            e.Cancel = args.Cancel;
        }

        protected sealed override void OnAfterDeselect(TreeViewEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnAfterDeselect(new VirtualTreeViewEventArgs(node));
        }

        protected sealed override void OnBeforeDeselect(TreeViewEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnBeforeDeselect(new VirtualTreeViewEventArgs(node));
        }

        protected sealed override void OnAfterSelect(TreeViewEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnAfterSelect(new VirtualTreeViewEventArgs(node, e.Action));
        }

        protected sealed override void OnBeforeSelect(MultiSelectTreeViewCancelEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            VirtualTreeViewCancelEventArgs args = new VirtualTreeViewCancelEventArgs(node, e.Cancel, e.Action);
            OnBeforeSelect(args);
            e.Cancel = args.Cancel;
        }

        protected sealed override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnNodeMouseClick(new VirtualTreeNodeMouseClickEventArgs(node, e.Button, e.Clicks, e.X, e.Y));
        }

        protected sealed override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            OnNodeMouseDoubleClick(new VirtualTreeNodeMouseClickEventArgs(node, e.Button, e.Clicks, e.X, e.Y));
        }

        protected sealed override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            VirtualNodeLabelEditEventArgs args = new VirtualNodeLabelEditEventArgs(node, e.Label);
            OnAfterLabelEdit(args);
            e.CancelEdit = args.CancelEdit;
        }

        protected sealed override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            VirtualTreeNode node = (VirtualTreeNode)e.Node;
            VirtualNodeLabelEditEventArgs args = new VirtualNodeLabelEditEventArgs(node, e.Label);
            OnBeforeLabelEdit(args);
            e.CancelEdit = args.CancelEdit;
        }

        protected sealed override void OnItemDrag(MultiSelectTreeViewItemDragEventArgs e)
        {
            List<VirtualTreeNode> nodes = new List<VirtualTreeNode>();

            foreach (MultiSelectTreeNode n in e.Nodes)
            {
                nodes.Add((VirtualTreeNode)n);
            }

            OnItemDrag(new VirtualTreeViewItemDragEventArgs(e.Button, nodes));
        }

        protected virtual void OnAfterCheck(VirtualTreeViewEventArgs e)
        {
            EventHandler<VirtualTreeViewEventArgs> handler = AfterCheck;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnBeforeCheck(VirtualTreeViewCancelEventArgs e)
        {
            EventHandler<VirtualTreeViewCancelEventArgs> handler = BeforeCheck;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAfterCollapse(VirtualTreeViewEventArgs e)
        {
            EventHandler<VirtualTreeViewEventArgs> handler = AfterCollapse;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnBeforeCollapse(VirtualTreeViewCancelEventArgs e)
        {
            EventHandler<VirtualTreeViewCancelEventArgs> handler = BeforeCollapse;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAfterExpand(VirtualTreeViewEventArgs e)
        {
            EventHandler<VirtualTreeViewEventArgs> handler = AfterExpand;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnBeforeExpand(VirtualTreeViewCancelEventArgs e)
        {
            EventHandler<VirtualTreeViewCancelEventArgs> handler = BeforeExpand;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAfterDeselect(VirtualTreeViewEventArgs e)
        {
            EventHandler<VirtualTreeViewEventArgs> handler = AfterDeselect;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnBeforeDeselect(VirtualTreeViewEventArgs e)
        {
            EventHandler<VirtualTreeViewEventArgs> handler = BeforeDeselect;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAfterSelect(VirtualTreeViewEventArgs e)
        {
            EventHandler<VirtualTreeViewEventArgs> handler = AfterSelect;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnBeforeSelect(VirtualTreeViewCancelEventArgs e)
        {
            EventHandler<VirtualTreeViewCancelEventArgs> handler = BeforeSelect;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnBeforeLabelEdit(VirtualNodeLabelEditEventArgs e)
        {
            EventHandler<VirtualNodeLabelEditEventArgs> handler = BeforeLabelEdit;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnAfterLabelEdit(VirtualNodeLabelEditEventArgs e)
        {
            EventHandler<VirtualNodeLabelEditEventArgs> handler = AfterLabelEdit;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        protected virtual void OnNodeMouseClick(VirtualTreeNodeMouseClickEventArgs e)
        {
            EventHandler<VirtualTreeNodeMouseClickEventArgs> handler = NodeMouseClick;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnNodeMouseDoubleClick(VirtualTreeNodeMouseClickEventArgs e)
        {
            EventHandler<VirtualTreeNodeMouseClickEventArgs> handler = NodeMouseDoubleClick;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemDrag(VirtualTreeViewItemDragEventArgs e)
        {
            EventHandler<VirtualTreeViewItemDragEventArgs> handler = ItemDrag;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs after the tree node check box is checked.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node check box is checked.")]
        public new event EventHandler<VirtualTreeViewEventArgs> AfterCheck;

        /// <summary>
        /// Occurs before the tree node check box is checked.
        /// </summary>
        [Category("Behavior"), Description("Occurs before the tree node check box is checked.")]
        public new event EventHandler<VirtualTreeViewCancelEventArgs> BeforeCheck;

        /// <summary>
        /// Occurs after the tree node is collapsed.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node is collapsed.")]
        public new event EventHandler<VirtualTreeViewEventArgs> AfterCollapse;

        /// <summary>
        /// Occurs before the tree node is collapsed.
        /// </summary>
        [Category("Behavior"), Description("Occurs before the tree node is collapsed.")]
        public new event EventHandler<VirtualTreeViewCancelEventArgs> BeforeCollapse;

        /// <summary>
        /// Occurs after the tree node is expanded.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node is expanded.")]
        public new event EventHandler<VirtualTreeViewEventArgs> AfterExpand;

        /// <summary>
        /// Occurs before the tree node is expanded.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node is expanded.")]
        public new event EventHandler<VirtualTreeViewCancelEventArgs> BeforeExpand;

        /// <summary>
        /// Occurs after the tree node is deselected.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node is deselected.")]
        public new event EventHandler<VirtualTreeViewEventArgs> AfterDeselect;

        /// <summary>
        /// Occurs before the tree node is deselected.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node is deselected.")]
        public new event EventHandler<VirtualTreeViewEventArgs> BeforeDeselect;

        /// <summary>
        /// Occurs after the tree node is selected.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node is selected.")]
        public new event EventHandler<VirtualTreeViewEventArgs> AfterSelect;

        /// <summary>
        /// Occurs before the tree node is selected.
        /// </summary>
        [Category("Behavior"), Description("Occurs before the tree node is selected.")]
        public new event EventHandler<VirtualTreeViewCancelEventArgs> BeforeSelect;

        /// <summary>
        /// Occurs after the node is clicked.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the node is clicked.")]
        public new event EventHandler<VirtualTreeNodeMouseClickEventArgs> NodeMouseClick;

        /// <summary>
        /// Occurs after the node is double clicked.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the node is double clicked.")]
        public new event EventHandler<VirtualTreeNodeMouseClickEventArgs> NodeMouseDoubleClick;

        /// <summary>
        /// Occurs after the tree node label is edited.
        /// </summary>
        [Category("Behavior"), Description("Occurs after the tree node label is edited.")]
        public new event EventHandler<VirtualNodeLabelEditEventArgs> AfterLabelEdit;

        /// <summary>
        /// Occurs before the tree node label is edited.
        /// </summary>
        [Category("Behavior"), Description("Occurs before the tree node label is edited.")]
        public new event EventHandler<VirtualNodeLabelEditEventArgs> BeforeLabelEdit;

        /// <summary>
        /// Occurs when a node starts being dragged.
        /// </summary>
        [Category("Behavior"), Description("Occurs when a node starts being dragged.")]
        public new event EventHandler<VirtualTreeViewItemDragEventArgs> ItemDrag;

        #endregion

        /// <summary>
        /// Used to ensure that the '+' expander thingy is visible when a node has children.
        /// </summary>
        private class DummyTreeNode : MultiSelectTreeNode
        {
        }
    }

    internal interface IVirtualTreeNodeCollectionOwner
    {
        /// <summary>
        /// Gets a complete collection of nodes owned.
        /// </summary>
        VirtualTreeView.VirtualTreeNodeCollection Nodes { get;}

        /// <summary>
        /// Gets a collection of the nodes which are actually added to the base <see cref="System.Windows.Forms.TreeView"/>.
        /// </summary>
        MultiSelectTreeNodeCollection RealNodes { get;}
    }

    internal interface IHaveNodes
    {
        VirtualTreeView.VirtualTreeNodeCollection Nodes { get; }
        object Tag { get; }
        void Expand();
        void Collapse();
    }

    #region VirtualTreeView EventArgs classes

    public class VirtualTreeViewEventArgs : EventArgs
    {
        private readonly TreeViewAction _action;
        private readonly VirtualTreeNode _node;

        public VirtualTreeViewEventArgs(VirtualTreeNode node)
        {
            Util.ThrowIfParameterNull(node, "node");

            _node = node;
        }

        public VirtualTreeViewEventArgs(VirtualTreeNode node, TreeViewAction action)
            : this(node)
        {
            _action = action;
        }

        public TreeViewAction Action
        {
            get
            {
                return _action;
            }
        }

        public VirtualTreeNode Node
        {
            get
            {
                return _node;
            }
        }
    }

    public class VirtualTreeViewCancelEventArgs : CancelEventArgs
    {
        private readonly TreeViewAction _action;
        private readonly VirtualTreeNode _node;

        public VirtualTreeViewCancelEventArgs(VirtualTreeNode node, bool cancel, TreeViewAction action)
            : base(cancel)
        {
            _node = node;
            _action = action;
        }

        public TreeViewAction Action
        {
            get
            {
                return _action;
            }
        }

        public VirtualTreeNode Node
        {
            get
            {
                return _node;
            }
        }
    }

    public class VirtualTreeNodeMouseClickEventArgs : MouseEventArgs
    {
        private readonly VirtualTreeNode _node;

        public VirtualTreeNodeMouseClickEventArgs(VirtualTreeNode node, MouseButtons button, int clicks, int x, int y)
            : base(button , clicks , x , y, 0)
        {
            _node = node;
        }

        public VirtualTreeNode Node
        {
            get
            {
                return _node;
            }
        }
    }

    public class VirtualNodeLabelEditEventArgs : EventArgs
    {
        private bool _cancelEdit;
        private readonly string _label;
        private readonly VirtualTreeNode _node;

        public VirtualNodeLabelEditEventArgs(VirtualTreeNode node)
        {
            Util.ThrowIfParameterNull(node, "node");

            _node = node;
            _label = null;
        }

        public VirtualNodeLabelEditEventArgs(VirtualTreeNode node, string label)
            : this(node)
        {
            _label = label;
        }

        public bool CancelEdit
        {
            get
            {
                return _cancelEdit;
            }
            set
            {
                _cancelEdit = value;
            }
        }

        public string Label
        {
            get
            {
                return _label;
            }
        }

        public VirtualTreeNode Node
        {
            get
            {
                return _node;
            }
        }
    }

    public class VirtualTreeViewItemDragEventArgs : EventArgs
    {
        private readonly ReadOnlyCollection<VirtualTreeNode> _nodes;
        private readonly MouseButtons _button;

        public VirtualTreeViewItemDragEventArgs(MouseButtons button)
            : this(button, new VirtualTreeNode[0])
        {
        }

        public VirtualTreeViewItemDragEventArgs(MouseButtons button, IEnumerable<VirtualTreeNode> nodes)
        {
            _nodes = new ReadOnlyCollection<VirtualTreeNode>(new List<VirtualTreeNode>(nodes));
            _button = button;
        }

        public MouseButtons Button
        {
            get
            {
                return _button;
            }
        }

        public ReadOnlyCollection<VirtualTreeNode> Nodes
        {
            get
            {
                return _nodes;
            }
        }
    }

    #endregion

}
