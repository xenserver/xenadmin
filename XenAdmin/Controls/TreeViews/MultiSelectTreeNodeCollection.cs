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
using System.Runtime.InteropServices;
using XenAdmin.Core;

namespace XenAdmin.Controls
{
    public class MultiSelectTreeNodeCollection : IList<MultiSelectTreeNode>
    {
        private readonly IMultiSelectTreeNodeCollectionOwner _owner;

        public MultiSelectTreeNodeCollection(IMultiSelectTreeNodeCollectionOwner owner)
        {
            Util.ThrowIfParameterNull(owner, "owner");
            _owner = owner;
        }

        public void AddRange(MultiSelectTreeNode[] nodes)
        {
            _owner.Nodes.AddRange(nodes);
            Array.ForEach(nodes, UpdateCheckBoxVisiblity);
        }

        private void UpdateCheckBoxVisiblity(MultiSelectTreeNode node)
        {
            if (node.TreeView != null && node.TreeView.CheckBoxes && node.Handle != IntPtr.Zero && node.TreeView != null && node.TreeView.Handle != IntPtr.Zero)
            {
                node.TreeView.UpdateCheckboxVisibility(node);

                // recurse down all descendants
                node.Nodes.ForEach(UpdateCheckBoxVisiblity);
            }
        }

        public void ForEach(Action<MultiSelectTreeNode> action)
        {
            foreach (MultiSelectTreeNode node in this)
            {
                action(node);
            }
        }

        #region IList<MultiSelectTreeNode> Members

        public int IndexOf(MultiSelectTreeNode item)
        {
            return _owner.Nodes.IndexOf(item);
        }

        public void Insert(int index, MultiSelectTreeNode item)
        {
            _owner.Nodes.Insert(index, item);
            UpdateCheckBoxVisiblity(item);
        }

        public void RemoveAt(int index)
        {
            _owner.Nodes.RemoveAt(index);
        }

        public MultiSelectTreeNode this[int index]
        {
            get
            {
                return (MultiSelectTreeNode)_owner.Nodes[index];
            }
            set
            {
                _owner.Nodes[index] = value;
                UpdateCheckBoxVisiblity(value);
            }
        }

        #endregion

        #region ICollection<MultiSelectTreeNode> Members

        public void Add(MultiSelectTreeNode item)
        {
            _owner.Nodes.Add(item);
            UpdateCheckBoxVisiblity(item);
        }

        public void Clear()
        {
            _owner.Nodes.Clear();
        }

        public bool Contains(MultiSelectTreeNode item)
        {
            return _owner.Nodes.Contains(item);
        }

        public void CopyTo(MultiSelectTreeNode[] array, int arrayIndex)
        {
            _owner.Nodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _owner.Nodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(MultiSelectTreeNode item)
        {
            bool ret = _owner.Nodes.Contains(item);
            _owner.Nodes.Remove(item);
            return ret;
        }

        #endregion

        #region IEnumerable<MultiSelectTreeNode> Members

        public IEnumerator<MultiSelectTreeNode> GetEnumerator()
        {
            return new MultiSelectTreeNodeCollectionEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Enumerator

        private class MultiSelectTreeNodeCollectionEnumerator : IEnumerator<MultiSelectTreeNode>
        {
            private readonly MultiSelectTreeNodeCollection _owner;
            private int _current = -1;

            public MultiSelectTreeNodeCollectionEnumerator(MultiSelectTreeNodeCollection owner)
            {
                _owner = owner;
            }

            #region IEnumerator<MultiSelectTreeNode> Members

            public MultiSelectTreeNode Current
            {
                get
                {
                    if (_current == -1)
                    {
                        return null;
                    }
                    return _owner[_current];
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public bool MoveNext()
            {
                if (_current < _owner.Count - 1)
                {
                    _current++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _current = -1;
            }

            #endregion
        }

        #endregion
    }
}
