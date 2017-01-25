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

namespace XenAdmin.Controls
{
    public class VirtualTreeSelectedNodeCollection : IList<VirtualTreeNode>
    {
        private readonly MultiSelectTreeView _parent;

        public VirtualTreeSelectedNodeCollection(VirtualTreeView parent)
        {
            Util.ThrowIfParameterNull(parent, "parent");
            _parent = parent;
        }

        public void SetContents(IEnumerable<VirtualTreeNode> nodes)
        {
            List<MultiSelectTreeNode> nodeList = new List<MultiSelectTreeNode>();
            foreach (VirtualTreeNode node in nodes)
            {
                nodeList.Add(node);
            }

            _parent.SelectedNodes.SetContents(nodeList);
        }

        #region IList<VirtualTreeNode> Members

        public int IndexOf(VirtualTreeNode item)
        {
            return _parent.SelectedNodes.IndexOf(item);
        }

        public void Insert(int index, VirtualTreeNode item)
        {
            _parent.SelectedNodes.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _parent.SelectedNodes.RemoveAt(index);
        }

        public VirtualTreeNode this[int index]
        {
            get
            {
                return (VirtualTreeNode)_parent.SelectedNodes[index];
            }
            set
            {
                _parent.SelectedNodes[index] = value;
            }
        }

        #endregion

        #region ICollection<VirtualTreeNode> Members

        public void Add(VirtualTreeNode item)
        {
            _parent.SelectedNodes.Add(item);
        }

        public void Clear()
        {
            _parent.SelectedNodes.Clear();
        }

        public bool Contains(VirtualTreeNode item)
        {
            return _parent.SelectedNodes.Contains(item);
        }

        public void CopyTo(VirtualTreeNode[] array, int arrayIndex)
        {
            _parent.SelectedNodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _parent.SelectedNodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(VirtualTreeNode item)
        {
            return _parent.SelectedNodes.Remove(item);
        }

        #endregion

        #region IEnumerable<VirtualTreeNode> Members

        public IEnumerator<VirtualTreeNode> GetEnumerator()
        {
            return new VirtualTreeSelectedNodeCollectionEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Enumerator

        private class VirtualTreeSelectedNodeCollectionEnumerator : IEnumerator<VirtualTreeNode>
        {
            private readonly VirtualTreeSelectedNodeCollection _owner;
            private int _current = -1;

            public VirtualTreeSelectedNodeCollectionEnumerator(VirtualTreeSelectedNodeCollection owner)
            {
                _owner = owner;
            }

            #region IEnumerator<MultiSelectTreeNode> Members

            public VirtualTreeNode Current
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
