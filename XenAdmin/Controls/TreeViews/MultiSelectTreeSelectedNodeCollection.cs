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
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace XenAdmin.Controls
{
    partial class MultiSelectTreeView
    {
        public sealed class MultiSelectTreeSelectedNodeCollection : IList<MultiSelectTreeNode>
        {
            private readonly MultiSelectTreeView _parent;

            public MultiSelectTreeSelectedNodeCollection(MultiSelectTreeView parent)
            {
                Util.ThrowIfParameterNull(parent, "parent");
                _parent = parent;
            }

            public void SetContents(IEnumerable<MultiSelectTreeNode> items)
            {
                Util.ThrowIfParameterNull(items, "items");
                IList<MultiSelectTreeNode> itemsAsList = items as IList<MultiSelectTreeNode>;
                int itemsCount = itemsAsList != null ? itemsAsList.Count : (new List<MultiSelectTreeNode>(items).Count);
                bool different = itemsCount != Count;

                if (!different)
                {
                    List<MultiSelectTreeNode> existing = new List<MultiSelectTreeNode>(this);
                    
                    // check if contents are different.
                    foreach (MultiSelectTreeNode node in items)
                    {
                        int index = existing.IndexOf(node);
                        
                        if (index < 0)
                        {
                            different = true;
                            break;
                        }
                        else
                        {
                            existing.RemoveAt(index);
                        }
                    }

                    different |= existing.Count > 0;
                }
                
                if (different)
                {
                    _parent._selectionChanged = false;
                    _parent.UnselectAllNodes(TreeViewAction.Unknown);

                    foreach (MultiSelectTreeNode item in items)
                    {
                        _parent.SelectNode(item, true, TreeViewAction.Unknown);
                    }

                    _parent.OnSelectionsChanged();
                }
            }

            #region IList<TreeNode> Members

            public int IndexOf(MultiSelectTreeNode item)
            {
                return _parent._selectedNodes.IndexOf(item);
            }

            public void Insert(int index, MultiSelectTreeNode item)
            {
                _parent._selectionChanged = false;
                _parent.SelectNode(item, true, TreeViewAction.Unknown);
                _parent.OnSelectionsChanged();
            }

            public void RemoveAt(int index)
            {
                MultiSelectTreeNode item = _parent._selectedNodes[index];
                _parent._selectionChanged = false;
                _parent.SelectNode(item, false, TreeViewAction.Unknown);
                _parent.OnSelectionsChanged();
            }

            public MultiSelectTreeNode this[int index]
            {
                get
                {
                    return _parent._selectedNodes[index];
                }
                set
                {
                    throw new InvalidOperationException();
                }
            }

            #endregion

            #region ICollection<TreeNode> Members

            public void Add(MultiSelectTreeNode item)
            {
                _parent._selectionChanged = false;
                _parent.SelectNode(item, true, TreeViewAction.Unknown);
                _parent.OnSelectionsChanged();
            }

            public void Clear()
            {
                _parent._selectionChanged = false;
                _parent.UnselectAllNodes(TreeViewAction.Unknown);
                _parent.OnSelectionsChanged();
            }

            public bool Contains(MultiSelectTreeNode item)
            {
                return _parent._selectedNodes.Contains(item);
            }

            public void CopyTo(MultiSelectTreeNode[] array, int arrayIndex)
            {
                _parent._selectedNodes.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _parent._selectedNodes.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(MultiSelectTreeNode item)
            {
                bool ret = Contains(item);
                _parent._selectionChanged = false;
                _parent.SelectNode(item, false, TreeViewAction.Unknown);
                _parent.OnSelectionsChanged();
                return ret;
            }

            #endregion

            #region IEnumerable<MultiSelectTreeNode> Members

            public IEnumerator<MultiSelectTreeNode> GetEnumerator()
            {
                return _parent._selectedNodes.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        private class InternalSelectedNodeCollection : Collection<MultiSelectTreeNode>
        {
            protected override void ClearItems()
            {
                List<MultiSelectTreeNode> nodes = new List<MultiSelectTreeNode>(this);

                base.ClearItems();

                foreach (MultiSelectTreeNode node in nodes)
                {
                    ((IMultiSelectTreeNode)node).SetSelected(false);
                }
            }

            protected override void InsertItem(int index, MultiSelectTreeNode item)
            {
                base.InsertItem(index, item);
                ((IMultiSelectTreeNode)item).SetSelected(true);
            }

            protected override void RemoveItem(int index)
            {
                MultiSelectTreeNode node = this[index];
                base.RemoveItem(index);
                ((IMultiSelectTreeNode)node).SetSelected(false);
            }

            protected override void SetItem(int index, MultiSelectTreeNode item)
            {
                MultiSelectTreeNode toRemove = this[index];

                base.SetItem(index, item);

                ((IMultiSelectTreeNode)toRemove).SetSelected(false);
                ((IMultiSelectTreeNode)item).SetSelected(true);
            }
        }
    }
}