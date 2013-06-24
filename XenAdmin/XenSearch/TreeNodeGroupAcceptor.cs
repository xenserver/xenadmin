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
using System.Text;
using System.Windows.Forms;

using XenAdmin.Controls;

namespace XenAdmin.XenSearch
{
    internal class TreeNodeGroupAcceptor : IAcceptGroups
    {
        protected readonly IHaveNodes parent;
        protected readonly bool? needToBeExpanded;

        protected int index = 0;

        public TreeNodeGroupAcceptor(IHaveNodes parent, bool? needToBeExpanded)
        {   
            this.parent = parent;
            this.needToBeExpanded = needToBeExpanded;
        }

        public virtual IAcceptGroups Add(Grouping grouping, Object group, int indent)
        {
            if (group == null)
                return null;

            bool? expandNode = ShouldExpandNode(group);
            VirtualTreeNode node = AddNode(index, grouping, group);
            index++;

            return newAcceptor(node, expandNode);
        }

        protected virtual bool? ShouldExpandNode(Object group)
        {
            return null;
        }

        protected virtual VirtualTreeNode AddNode(int index, Grouping grouping, Object obj)
        {
            VirtualTreeNode node = new VirtualTreeNode(obj.ToString());
            node.Tag = obj;
            parent.Nodes.Insert(index, node);
            return node;
        }

        protected virtual TreeNodeGroupAcceptor newAcceptor(VirtualTreeNode node, bool? expanded)
        {
            return new TreeNodeGroupAcceptor(node, expanded);
        }

        public virtual void FinishedInThisGroup(bool defaultExpand)
        {
            if (needToBeExpanded.HasValue ? needToBeExpanded.Value : defaultExpand)
                parent.Expand();
            else
                parent.Collapse();
        }
    }
}
