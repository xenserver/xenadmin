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
using System.Collections.ObjectModel;
using XenAPI;
using XenAdmin.Controls.CustomGridView;
using System.Windows.Forms;
using XenAdmin.Actions;

namespace XenAdmin.Commands
{
    internal abstract class DragDropCommand
    {
        private readonly VirtualTreeNode _targetNode;
        private readonly ReadOnlyCollection<VirtualTreeNode> _draggedNodes;
        private readonly GridRowCollection _draggedGridRows;
        private readonly IMainWindow _mainWindow;

        protected DragDropCommand(IMainWindow mainWindow, VirtualTreeNode targetNode, IDataObject dragData)
        {
            Util.ThrowIfParameterNull(mainWindow, "mainWindow");
            Util.ThrowIfParameterNull(dragData, "dragData");
            _mainWindow = mainWindow;
            _targetNode = targetNode;

            if (dragData.GetDataPresent(typeof(GridRowCollection)))
            {
                _draggedGridRows = (GridRowCollection)dragData.GetData(typeof(GridRowCollection));
            }
            else if (dragData.GetDataPresent(typeof(VirtualTreeNode[])))
            {
                _draggedNodes = new ReadOnlyCollection<VirtualTreeNode>(new List<VirtualTreeNode>((VirtualTreeNode[])dragData.GetData(typeof(VirtualTreeNode[]))));
            }
        }

        /// <summary>
        /// Gets the main window to be used by the Command.
        /// </summary>
        public IMainWindow MainWindowCommandInterface
        {
            get { return _mainWindow; }
        }

        /// <summary>
        /// Determines whether this drop-drop operation can execute with the specified target-node and drag-data.
        /// </summary>
        public bool CanExecute()
        {
            if ((DraggedNodes == null || DraggedNodes.Count == 0) && DraggedGridRows == null)
            {
                return false;
            }
            if (_targetNode == null)
            {
                return false;
            }

            return CanExecuteCore();
        }

        /// <summary>
        /// Determines whether this drop-drop operation can execute with the specified target-node and drag-data.
        /// </summary>
        protected virtual bool CanExecuteCore()
        {
            return false;
        }

        /// <summary>
        /// Executes this drag-drop operation with the specified target-node and drag data.
        /// </summary>
        public void Execute()
        {
            ExecuteCore();
        }

        /// <summary>
        /// Executes this drag-drop operation with the specified target-node and drag data.
        /// </summary>
        protected virtual void ExecuteCore()
        {
        }

        /// <summary>
        /// Gets the node that the mouse is over during the drag-drop operation.
        /// </summary>
        protected VirtualTreeNode TargetNode
        {
            get
            {
                return _targetNode;
            }
        }

        /// <summary>
        /// Gets the node that should be highlighted during the drag-drop operation.
        /// </summary>
        public virtual VirtualTreeNode HighlightNode
        {
            get
            {
                return _targetNode;
            }
        }

        /// <summary>
        /// Gets the nearest ancestor of the target node that has a tag of the specified xen object or grouping tag type and returns that tag.
        /// </summary>
        protected T GetTargetNodeAncestorAsXenObjectOrGroupingTag<T>() where T : class
        {
            VirtualTreeNode node = GetTargetNodeAncestor<T>();

            if (node != null)
            {
                return (T)node.Tag;
            }
            return default(T);
        }

        /// <summary>
        /// Gets the nearest ancestor of the target node that has a tag of the specified xen object or grouping-tag type.
        /// </summary>
        protected VirtualTreeNode GetTargetNodeAncestor<T>() where T : class
        {
            VirtualTreeNode node = TargetNode;
            while (node != null)
            {
                if (node.Tag is T)
                {
                    return node;
                }

                node = node.Parent;
            }
            return null;
        }

        /// <summary>
        /// Gets the dragged grid rows. Returns null if grid rows weren't dragged.
        /// </summary>
        protected GridRowCollection DraggedGridRows
        {
            get
            {
                return _draggedGridRows;
            }
        }

        /// <summary>
        /// Gets the dragged tree nodes. Returns null if tree nodes weren't dragged.
        /// </summary>
        protected ReadOnlyCollection<VirtualTreeNode> DraggedNodes
        {
            get
            {
                return _draggedNodes;
            }
        }

        /// <summary>
        /// Gets the status bar text tat should be displayed during the drag-drop operation. Returns null
        /// if no text should be displayed.
        /// </summary>
        public virtual string StatusBarText
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the dragged items as Xen objects. If any items isn't a XenObject then an empty list is returned.
        /// </summary>
        protected List<T> GetDraggedItemsAsXenObjects<T>() where T : IXenObject
        {
            List<T> output = new List<T>();
            if (DraggedNodes != null)
            {
                foreach (VirtualTreeNode node in DraggedNodes)
                {
                    if (node != null)
                    {
                        if (node.Tag is T)
                        {
                            output.Add((T)node.Tag);
                        }
                        else
                        {
                            return new List<T>();
                        }
                    }
                    else
                    {
                        return new List<T>();
                    }
                }
            }
            else if (DraggedGridRows != null)
            {
                foreach (GridRow row in DraggedGridRows)
                {
                    if (row.Tag is T)
                    {
                        output.Add((T)row.Tag);
                    }
                    else
                    {
                        return new List<T>();
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Runs the specified <see cref="AsyncAction"/>s such that they are synchronous per connection but asynchronous across connections.
        /// </summary>
        public void RunMultipleActions(IEnumerable<AsyncAction> actions, string title, string startDescription, string endDescription, bool runActionsInParallel)
        {
            MultipleActionLauncher launcher = new MultipleActionLauncher(actions, title, startDescription, endDescription, runActionsInParallel);
            launcher.Run();
        }
    }
}
