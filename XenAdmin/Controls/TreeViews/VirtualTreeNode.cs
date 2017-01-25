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

using System.Windows.Forms;
using System.Collections.Generic;

namespace XenAdmin.Controls
{
    /// <summary>
    /// A node of a <see cref="VirtualTreeView"/>.
    /// </summary>
    public partial class VirtualTreeNode : MultiSelectTreeNode, IVirtualTreeNodeCollectionOwner, IHaveNodes
    {
        private readonly VirtualTreeView.VirtualTreeNodeCollection _nodes;
        private IVirtualTreeNodeCollectionOwner _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualTreeNode"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public VirtualTreeNode(string text)
            : base(text)
        {
            _nodes = new VirtualTreeView.VirtualTreeNodeCollection(this);
        }

        internal IVirtualTreeNodeCollectionOwner Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <summary>
        /// Gets a value indicating that all the parents of this node are expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if all parents are expanded; otherwise, <c>false</c>.
        /// </value>
        public bool AllParentsAreExpanded
        {
            get
            {
                bool isRoot = Owner is VirtualTreeView;
                VirtualTreeNode parentNode = Owner as VirtualTreeNode;

                while (!isRoot)
                {
                    if (parentNode == null)
                    {
                        return false;
                    }

                    if (!parentNode.IsExpanded)
                    {
                        return false;
                    }

                    isRoot = parentNode.Owner is VirtualTreeView;
                    parentNode = parentNode.Owner as VirtualTreeNode;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the first child tree node in the tree node collection.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The first child <see cref="VirtualTreeNode"/> in the <see cref="VirtualTreeNode.Nodes"/> collection.
        /// </returns>
        public new VirtualTreeNode FirstNode
        {
            get { return _nodes.Count == 0 ? null : _nodes[0]; }
        }

        /// <summary>
        /// Gets the last child tree node.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="VirtualTreeNode"/> that represents the last child tree node.
        /// </returns>
        public new VirtualTreeNode LastNode
        {
            get { return _nodes.Count == 0 ? null : _nodes[_nodes.Count - 1]; }
        }

        /// <summary>
        /// Gets the next sibling tree node.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="VirtualTreeNode"/> that represents the next sibling tree node.
        /// </returns>
        public new VirtualTreeNode NextNode
        {
            get
            {
                if (Owner != null)
                {
                    int index = Owner.Nodes.IndexOf(this);

                    if (Owner.Nodes.Count > index + 1)
                    {
                        return Owner.Nodes[index + 1];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the previous sibling tree node.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="VirtualTreeNode"/> that represents the previous sibling tree node.
        /// </returns>
        public new VirtualTreeNode PrevNode
        {
            get
            {
                if (Owner != null)
                {
                    int index = Owner.Nodes.IndexOf(this);

                    if (index > 0)
                    {
                        return Owner.Nodes[index - 1];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Removes the current tree node from the tree view control.
        /// </summary>
        public new void Remove()
        {
            if (Owner != null)
            {
                Owner.Nodes.Remove(this);
            }
        }

        /// <summary>
        /// Gets the parent tree view that the tree node is assigned to.
        /// </summary>
        public new VirtualTreeView TreeView
        {
            get
            {
                // since this node might actually be virtual at this point, iterate up the nodes until a real node is found.

                VirtualTreeNode node = this;

                while (node != null)
                {
                    TreeNode tn = node;
                    if (tn.TreeView != null)
                    {
                        return (VirtualTreeView)tn.TreeView;
                    }
                    node = node.Parent;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the parent tree node of the current tree node.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="VirtualTreeNode"/> that represents the parent of the current tree node.
        /// </returns>
        public new VirtualTreeNode Parent
        {
            get
            {
                return Owner as VirtualTreeNode;
            }
        }

        /// <summary>
        /// Ensures that the tree node is visible, expanding tree nodes and scrolling the tree view control as necessary.
        /// </summary>
        public new void EnsureVisible()
        {
            if (((TreeNode)this).TreeView == null)
            {
                VirtualTreeNode n = Parent;
                while (n != null)
                {
                    n.Expand();
                    n = n.Parent;
                }
            }
            base.EnsureVisible();
        }

        /// <summary>
        /// Iterates through all the ancestors of this <see cref="VirtualTreeNode"/>.
        /// </summary>
        public IEnumerable<VirtualTreeNode> Ancestors
        {
            get
            {
                VirtualTreeNode n = Parent;

                while (n != null)
                {
                    yield return n;
                    n = n.Parent;
                }
            }
        }

        public PersistenceInfo GetPersistenceInfo()
        {
            return new PersistenceInfo(this);
        }

        /// <summary>
        /// Iterates through all the descendants of this <see cref="VirtualTreeNode"/>.
        /// </summary>
        public new IEnumerable<VirtualTreeNode> Descendants
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

        #region IVirtualTreeNodeCollectionOwner Members

        public new VirtualTreeView.VirtualTreeNodeCollection Nodes
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
            Expand();
        }

        void IHaveNodes.Collapse()
        {
            Collapse();
        }

        #endregion
    }
}
