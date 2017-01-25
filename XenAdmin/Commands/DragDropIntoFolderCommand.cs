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
using XenAdmin.Model;
using XenAPI;
using System.Windows.Forms;
using System.Threading;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    internal class DragDropIntoFolderCommand : DragDropCommand
    {
        public DragDropIntoFolderCommand(IMainWindow mainWindow, VirtualTreeNode targetNode, IDataObject dragData)
            : base(mainWindow, targetNode, dragData)
        {
        }

        protected override bool CanExecuteCore()
        {
            Folder targetFolder = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Folder>();

            return targetFolder != null && 
                GetDraggedItemsAsXenObjects<IXenObject>().Count > 0 && 
                DraggedObjectsAreValid();
        }

        public override VirtualTreeNode HighlightNode
        {
            get
            {
                return GetTargetNodeAncestor<Folder>();
            }
        }

        private bool DraggedObjectsAreValid()
        {
            Folder targetFolder = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Folder>();
            List<IXenObject> draggedItems = GetDraggedItemsAsXenObjects<IXenObject>();

            // all selected folders must have the same parent
            string commonParent = null;
            foreach (IXenObject obj in draggedItems)
            {
                Folder folder = obj as Folder;

                // if all items are already in the target folder then cancel.
                if (GetItemsNotAlreadyInTargetFolder().Count == 0)
                {
                    return false;
                }

                // can't drag non-folder items to the root folder
                if (targetFolder.IsRootFolder && !(obj is Folder))
                {
                    return false;
                }
                
                if (folder != null)
                {
                    // can't drag root folder
                    if (folder.IsRootFolder)
                    {
                        return false;
                    }

                    // can't drag folder to itself
                    if (targetFolder.opaque_ref == folder.opaque_ref)
                    {
                        return false;
                    }

                    // can't drag to direct parent
                    if (targetFolder.opaque_ref == Folders.GetParent(folder.opaque_ref))
                    {
                        return false;
                    }

                    // can't drag to a child folder
                    if (targetFolder.opaque_ref.StartsWith(folder.opaque_ref + "/"))
                    {
                        return false;
                    }

                    string parent = Folders.GetParent(folder.opaque_ref);

                    // all folders must have same parent folder
                    if (commonParent != null && parent != commonParent)
                    {
                        return false;
                    }
                    commonParent = parent;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the items to be moved. Any items which are already in the target folder won't be added to the returned list.
        /// </summary>
        private List<IXenObject> GetItemsNotAlreadyInTargetFolder()
        {
            List<IXenObject> output = new List<IXenObject>();
            Folder targetFolder = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Folder>();
            
            foreach (IXenObject draggedItem in GetDraggedItemsAsXenObjects<IXenObject>())
            {
                bool valid = true;

                // check obj isn't already in target-folder
                foreach (IXenObject objectAlreadyInTargetFolder in targetFolder.XenObjects)
                {
                    if (draggedItem.opaque_ref == objectAlreadyInTargetFolder.opaque_ref)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    output.Add(draggedItem);
                }
            }

            return output;
        }

        protected override void ExecuteCore()
        {
            Folder targetFolder = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Folder>();

            if (DraggedNodes != null)
            {
                foreach (VirtualTreeNode draggedNode in DraggedNodes)
                {
                    if (GetItemsNotAlreadyInTargetFolder().Contains((IXenObject)draggedNode.Tag))
                    {
                        GreyAll(draggedNode);
                    }
                }
            }

            new MoveToFolderAction(GetItemsNotAlreadyInTargetFolder(), targetFolder).RunAsync();

            // need to now wait until the operation has finished... then we can expand the node.
            ThreadPool.QueueUserWorkItem(delegate
            {
                for (int i = 0; i < 20 && TargetNode.Nodes.Count == 0; i++)
                {
                    Thread.Sleep(100);
                }

                MainWindowCommandInterface.Invoke(delegate { TargetNode.Expand(); });
            });
        }

        /// <summary>
        /// Grey out all the folders from the given node downwards.
        /// </summary>
        private static void GreyAll(VirtualTreeNode node)
        {
            Folder folder = node.Tag as Folder;
            if (folder != null)
            {
                folder.Grey = true;
                node.SelectedImageIndex = (int)Icons.FolderGrey;
                node.ImageIndex = node.SelectedImageIndex;

                foreach (VirtualTreeNode child in node.Nodes)
                {
                    GreyAll(child);
                }
            }
        }
    }
}
