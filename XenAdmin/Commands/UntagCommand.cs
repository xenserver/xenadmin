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
using System.Collections.ObjectModel;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Model;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Untags the specified treenodes.
    /// </summary>
    internal class UntagCommand : Command
    {
        private readonly IList<VirtualTreeNode> _nodes;

        public UntagCommand(IMainWindow mainWindow, IList<VirtualTreeNode> nodes)
            : base(mainWindow)
        {
            Util.ThrowIfParameterNull(nodes, "nodes");
            _nodes = nodes;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (_nodes.Count > 0)
            {
                foreach (VirtualTreeNode node in _nodes)
                {
                    if (!IsTagInOrgMode(node.Parent))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private static bool IsTagInOrgMode(VirtualTreeNode node)
        {
            if (node != null && node.Parent != null)//exclude the top node
            {
                GroupingTag gt = node.Tag as GroupingTag;
                return gt != null && gt.Grouping.GroupingName == Messages.TAGS;
            }
            return false;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var actions = new List<AsyncAction>();

            foreach (VirtualTreeNode node in _nodes)
            {
                string tag = node.Parent.Text;

                IXenObject xenObject = node.Tag as IXenObject;

                if (!string.IsNullOrEmpty(tag) && xenObject != null)
                    actions.Add(Tags.RemoveTagAction(xenObject, tag));
            }

            if (actions.Count != 0)
                RunMultipleActions(actions, Messages.DELETE_TAGS, Messages.DELETING_TAGS, Messages.DELETED_TAGS, true);
        }

        public override string MenuText
        {
            get
            {
                if (_nodes.Count > 1)
                {
                    return Messages.UNTAG_OBJECTS;
                }
                return Messages.UNTAG;
            }
        }
    }
}
