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
using System.Linq;


namespace XenAdmin.Commands
{
    internal class RemoveFromFolderCommand : Command
    {
        private readonly IList<VirtualTreeNode> _nodes;

        public RemoveFromFolderCommand(IMainWindow mainWindow, IList<VirtualTreeNode> nodes)
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
                    if (node.Parent == null || node.Tag is Folder || !(node.Tag is IXenObject))
                    {
                        return false;
                    }

                    Folder f = node.Parent.Tag as Folder;

                    if (f == null || f.IsRootFolder)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var objectsToBeRemoved = (from VirtualTreeNode node in _nodes
                                    let xenObject = node.Tag as IXenObject
                                    where xenObject != null
                                    select xenObject).ToList();

            new DeleteFolderAction(objectsToBeRemoved).RunAsync();
        }

        public override string MenuText
        {
            get
            {
                return Messages.REMOVE_FROM_FOLDER;
            }
        }
    }
}
