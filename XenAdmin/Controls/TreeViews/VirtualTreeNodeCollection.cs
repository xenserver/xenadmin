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

using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace XenAdmin.Controls
{
    public partial class VirtualTreeView
    {
        public class VirtualTreeNodeCollection : Collection<VirtualTreeNode>
        {
            private readonly IVirtualTreeNodeCollectionOwner _owner;

            internal VirtualTreeNodeCollection(IVirtualTreeNodeCollectionOwner owner)
            {
                Util.ThrowIfParameterNull(owner, "owner");
                _owner = owner;
            }

            internal IVirtualTreeNodeCollectionOwner Owner
            {
                get { return _owner; }
            }

            protected override void InsertItem(int index, VirtualTreeNode item)
            {
                base.InsertItem(index, item);
                item.Owner = _owner;

                if (item.AllParentsAreExpanded)
                {
                    _owner.RealNodes.Insert(index, item);
                }
                else
                {
                    // this node is not visible. just add a dummy node so the "+" icon is visible.

                    if (_owner.RealNodes.Count == 0)
                    {
                        _owner.RealNodes.Add(new DummyTreeNode());
                    }
                }
            }

            protected override void ClearItems()
            {
                base.ClearItems();
                _owner.RealNodes.Clear();
            }

            protected override void RemoveItem(int index)
            {
                VirtualTreeNode node = this[index];

                base.RemoveItem(index);

                if (Count == 0)
                {
                    _owner.RealNodes.Clear();
                }
                else if (_owner.RealNodes.Contains(node))
                {
                    _owner.RealNodes.Remove(node);
                }
            }

            protected override void SetItem(int index, VirtualTreeNode item)
            {
                // the treeview is peculiar in that it uses the set indexer to *insert* an item... not to set an item -- we'll do the same.
                InsertItem(index, item);
            }

            public VirtualTreeNode[] Find(Predicate<VirtualTreeNode> match, bool searchAllDescendants)
            {
                var output = new List<VirtualTreeNode>();
                Find(this, match, output, searchAllDescendants);
                return output.ToArray();
            }

            private void Find(IEnumerable<VirtualTreeNode> nodes, Predicate<VirtualTreeNode> match, List<VirtualTreeNode> output, bool searchAllDescendants)
            {
                foreach (VirtualTreeNode node in nodes)
                {
                    if (match(node))
                    {
                        output.Add(node);
                    }
                    if (searchAllDescendants)
                    {
                        Find(node.Nodes, match, output, searchAllDescendants);
                    }
                }
            }
        }
    }
}
