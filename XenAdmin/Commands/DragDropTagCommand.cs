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
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Model;
using System.Windows.Forms;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    internal class DragDropTagCommand : DragDropCommand
    {
        public DragDropTagCommand(IMainWindow mainWindow, VirtualTreeNode targetNode, IDataObject dragData)
            : base(mainWindow, targetNode, dragData)
        {
        }

        public override VirtualTreeNode HighlightNode
        {
            get
            {
                return GetTargetNodeAncestor<GroupingTag>();
            }
        }

        protected override bool CanExecuteCore()
        {
            List<IXenObject> draggedObjects = GetDraggedItemsAsXenObjects<IXenObject>();
            GroupingTag gt = GetTargetNodeAncestorAsXenObjectOrGroupingTag<GroupingTag>();
            
            if (gt != null && draggedObjects.Count > 0)
            {
                foreach (IXenObject xenObject in draggedObjects)
                {
                    // can't tag folders.
                    if (xenObject is Folder)
                        return false;
                }

                return gt.Grouping.GroupingName == Messages.TAGS && !string.IsNullOrEmpty(gt.Group as string);
            }
            return false;
        }

        protected override void ExecuteCore()
        {
            string tag = (string)(GetTargetNodeAncestorAsXenObjectOrGroupingTag<GroupingTag>().Group);
            List<IXenObject> objs = GetDraggedItemsAsXenObjects<IXenObject>();

            var actions = new List<AsyncAction>();

            foreach (IXenObject obj in objs)
                actions.Add(Tags.AddTagAction(obj, tag));

            if (actions.Count != 0)
                RunMultipleActions(actions,
                    string.Format(Messages.ADD_TAG, tag),
                    string.Format(Messages.ADDING_TAG, tag),
                    string.Format(Messages.ADDED_TAG, tag), true);
        }
    }
}
