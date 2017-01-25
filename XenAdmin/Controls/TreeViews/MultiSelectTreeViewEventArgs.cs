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
using System.ComponentModel;

namespace XenAdmin.Controls
{
    public class MultiSelectTreeViewItemDragEventArgs : EventArgs
    {
        private readonly ReadOnlyCollection<MultiSelectTreeNode> _nodes;
        private readonly MouseButtons _button;

        public MultiSelectTreeViewItemDragEventArgs(MouseButtons button)
            : this(button, new MultiSelectTreeNode[0])
        {
        }

        public MultiSelectTreeViewItemDragEventArgs(MouseButtons button, IEnumerable<MultiSelectTreeNode> nodes)
        {
            _nodes = new ReadOnlyCollection<MultiSelectTreeNode>(new List<MultiSelectTreeNode>(nodes));
            _button = button;
        }

        public MouseButtons Button
        {
            get
            {
                return _button;
            }
        }

        public ReadOnlyCollection<MultiSelectTreeNode> Nodes
        {
            get
            {
                return _nodes;
            }
        }
    }

    public class MultiSelectTreeViewCancelEventArgs : CancelEventArgs
    {
        private readonly TreeViewAction _action;
        private readonly MultiSelectTreeNode _node;

        public MultiSelectTreeViewCancelEventArgs(MultiSelectTreeNode node, bool cancel, TreeViewAction action)
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

        public MultiSelectTreeNode Node
        {
            get
            {
                return _node;
            }
        }
    }
}
