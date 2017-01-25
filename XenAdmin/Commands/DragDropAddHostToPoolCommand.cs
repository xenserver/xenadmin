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
using XenAdmin.Core;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace XenAdmin.Commands
{
    internal class DragDropAddHostToPoolCommand : DragDropCommand
    {
        public DragDropAddHostToPoolCommand(IMainWindow mainWindow, VirtualTreeNode targetNode, IDataObject dragData)
            : base(mainWindow, targetNode, dragData)
        {
        }

        protected override bool CanExecuteCore()
        {
            Pool targetPool = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Pool>();
            
            if (targetPool != null)
            {
                AddHostToPoolCommand cmd = new AddHostToPoolCommand(MainWindowCommandInterface, GetDraggedItemsAsXenObjects<Host>(), targetPool, true);
                return cmd.CanExecute();
            }
            return false;
        }

        public override string StatusBarText
        {
            get
            {
                if (CanExecute())
                {
                    Pool targetPool = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Pool>();
                    List<Host> draggedHosts = GetDraggedItemsAsXenObjects<Host>();

                    if (draggedHosts.Count > 0)
                    {
                        foreach (Host draggedHost in draggedHosts)
                        {
                            PoolJoinRules.Reason reason = PoolJoinRules.CanJoinPool(draggedHost.Connection, targetPool.Connection, true, true, true);
                            if (reason != PoolJoinRules.Reason.Allowed)
                            {
                                string reasonString = PoolJoinRules.ReasonMessage(reason);
                                if (draggedHosts.Count == 1)
                                    return reasonString;
                                else
                                    return string.Format("{0}: {1}", draggedHost, reasonString);
                            }
                        }
                    }
                }

                return null;
            }
        }

        public override VirtualTreeNode HighlightNode
        {
            get
            {
                return CanExecute() ? GetTargetNodeAncestor<Pool>() : null;
            }
        }

        protected override void ExecuteCore()
        {
            Pool targetPool = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Pool>();
            new AddHostToPoolCommand(MainWindowCommandInterface, GetDraggedItemsAsXenObjects<Host>(), targetPool, true).Execute();
        }
    }
}
