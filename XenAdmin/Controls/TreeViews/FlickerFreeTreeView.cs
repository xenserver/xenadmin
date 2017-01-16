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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Core;
using System.Collections.Generic;
using XenAdmin.Network;
using XenAdmin.XenSearch;

using XenAPI;
using Message = System.Windows.Forms.Message;

namespace XenAdmin.Controls
{
    /**
     * Flicker-free TreeView, from
     * http://www.codeguru.com/forum/archive/index.php/t-182326.html
     * 
     * This has been specialised for the mainwindow treeview
     * (persistent selection, scroll etc) so it should be used 
     * with caution for other things 
     */
    [FormFontFixer.PreserveFonts(true)]
    public class FlickerFreeTreeView : VirtualTreeView
    {
        private List<VirtualTreeNode.PersistenceInfo> _persistedSelectionInfo;
        private IList<object> _persistedTopNode;

        protected override void OnBeforeExpand(VirtualTreeViewCancelEventArgs e)
        {
            if (TopNode == null)
            {
                _persistedTopNode = new List<object>();
            }
            else
            {
                _persistedTopNode = TopNode.GetPersistenceInfo().Path;
            }
            
            base.OnBeforeExpand(e);
        }

        protected override void OnAfterExpand(VirtualTreeViewEventArgs e)
        {
            base.OnAfterExpand(e);
            if (_persistedTopNode != null)
            {
                int hPos = HScrollPos;
                TopNode = ClosestMatch(_persistedTopNode);
                HScrollPos = hPos;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Core.Win32.WM_ERASEBKGND)
            {
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Apps)
                OnKeyPress(new KeyPressEventArgs((char)keyData));
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region Persist selection across BeginUpdate() / EndUpdate()

        /// <summary>
        /// We have to be a bit more clever now, as things can appear
        /// more than once in the treeview (ie Group by tag)
        /// 
        /// Therefore we try and keep selection on the correct
        /// object, even during migration or moving between folders.
        /// In the case we can't, we default to its parent and its
        /// parent's parent and so on.
        /// </summary>
        public new void BeginUpdate()
        {
            // Save the selection...
            _persistedSelectionInfo = new List<VirtualTreeNode>(SelectedNodes).ConvertAll(n => n.GetPersistenceInfo());

            // Save the scroll position..
            if (TopNode == null)
            {
                _persistedTopNode = new List<object>();
            }
            else
            {
                _persistedTopNode = TopNode.GetPersistenceInfo().Path;
            }
        }

        public new void EndUpdate()
        {
            base.EndUpdate();

            int hPos = HScrollPos;
            VirtualTreeNode restoredTopNode = ClosestMatch(_persistedTopNode);

            if (restoredTopNode != TopNode)
            {
                TopNode = restoredTopNode;

                // Restore the scroll position...
                // Setting TopNode alters _both_ scrollbars. This sets the vertical one to the old position
                // and the horizontal one to a different one depending on the width on the TopNode.
                HScrollPos = hPos;
            }

            // Restore the selection...
            if (_persistedSelectionInfo == null || _persistedSelectionInfo.Count == 0)
            {
                SelectedNode = Nodes[0];
            }
            else
            {
                RestoreSelection();
            }

            // if the selected nodes haven't change, but the selected tags have
            // then a selections changed event still needs to be fired.
            foreach (VirtualTreeNode node in SelectedNodes)
            {
                VirtualTreeNode.PersistenceInfo info = new VirtualTreeNode.PersistenceInfo(node);

                if (!_persistedSelectionInfo.Contains(info))
                {
                    // selection is different to old one. So fire an event.

                    ForceSelectionsChanged();
                    break;
                }
            }
        }

        /// <summary>
        /// Try and restore the selection.
        /// First, look for the object in the same position
        /// Second, find the object in the maximal sub tree where it appeared only once
        /// Finally, just select one of the parents
        /// </summary>
        private void RestoreSelection()
        {
            List<VirtualTreeNode> newSelectedNodes = new List<VirtualTreeNode>();

            foreach (VirtualTreeNode.PersistenceInfo info in _persistedSelectionInfo)
            {
                VirtualTreeNode match;

                // First, look for the object in the same position
                if (TryExactMatch(info.Path, out match) >= info.Path.Count)
                {
                    TryToSelectNode(newSelectedNodes, match);
                    continue;
                }

                // Second, find the object in the maximal sub tree where it appeared only once
                if (TryExactMatch(info.PathToMaximalSubTree, out match) >= info.PathToMaximalSubTree.Count)
                {
                    match = FindNodeIn(match, info.Tag);
                    if (match != null)
                    {
                        // since node has moved, make sure it's visible.
                        match.EnsureVisible();
                        TryToSelectNode(newSelectedNodes, match);
                    }
                }
            }

            if (newSelectedNodes.Count == 0)
            {
                foreach (VirtualTreeNode.PersistenceInfo info in _persistedSelectionInfo)
                {
                    // Finally, just select one of the parents
                    TryToSelectNode(newSelectedNodes, ClosestMatch(info.Path));
                    break;
                }
            }

            // restore selection
            SelectedNodes.SetContents(newSelectedNodes);
        }

        private void TryToSelectNode(List<VirtualTreeNode> nodes, VirtualTreeNode node)
        {
            if (!CanSelectNode(node))
            {
                TryToSelectNode(nodes, node.Parent);
            }
            else if (!nodes.Contains(node))
            {
                nodes.Add(node);
            }
        }

        public bool CanSelectNode(VirtualTreeNode node)
        {
            return node.Tag == null || node.Tag is IXenObject || node.Tag is GroupingTag || node.Tag is Search;
        }

        /// <summary>
        /// Try and find a node by following the path in selection
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="match"></param>
        /// <returns>How far it got along the path before it bailed.  
        /// If it returns >= selection.Count, then it succeded</returns>
        public int TryExactMatch(IList<Object> selection, out VirtualTreeNode match)
        {
            int i = 0;
            match = Nodes[0];

            while (i < selection.Count)
            {
                bool found = false;

                foreach (VirtualTreeNode child in match.Nodes)
                {
                    if (child.Tag.Equals(selection[i]))
                    {
                        match = child;
                        found = true;
                        i++;
                        break;
                    }
                }

                if (!found)
                    break;
            }

            return i;
        }

        public VirtualTreeNode FindNodeIn(VirtualTreeNode match, Object o)
        {
            if (match.Tag == o)
                return match;

            foreach (VirtualTreeNode child in match.Nodes)
            {
                VirtualTreeNode result = FindNodeIn(child, o);
                if (result != null)
                    return result;
            }

            return null;
        }

        private VirtualTreeNode ClosestMatch(IList<Object> selection)
        {
            VirtualTreeNode currentNode;
            int i = TryExactMatch(selection, out currentNode);

            // We never got down the first step;
            // this usually means selection changing over
            // connection / disconnect. Skank it up
            if (i == 0 && selection.Count > 0)
            {
                IXenObject o = selection[0] as IXenObject;
                if (o != null)
                {
                    IXenConnection connection = o.Connection;

                    foreach (VirtualTreeNode child in currentNode.Nodes)
                    {
                        IXenObject o2 = child.Tag as IXenObject;
                        if (o2 == null)
                            continue;

                        if (o2.Connection == connection)
                        {
                            currentNode = child;
                            break;
                        }
                    }
                }
            }

            return currentNode;
        }

        #endregion

        public bool TryToSelectNewNode(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            foreach (VirtualTreeNode node in AllNodes)
            {
                if (tagMatch(node.Tag))
                {
                    if (selectNode)
                        SelectedNode = node;
                    
                    if (expandNode)
                    node.Expand();
                    
                    if (ensureNodeVisible)
                    node.EnsureVisible();
                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Selects the specified object in the tree.
        /// </summary>
        /// <param name="o">The object to be selected.</param>
        /// <param name="node">The node at which to start.</param>
        /// <param name="expand">Expand the node when it's found.</param>
        /// <param name="cancelled">if set to <c>true</c> then the node for the
        /// specified object was not allowed to be selected.</param>
        /// <returns>A value indicating whether selection was successful.</returns>
        public bool SelectObject(IXenObject o, VirtualTreeNode node, bool expand, ref bool cancelled)
        {
            IXenObject candidate = node.Tag as IXenObject;

            if (o == null || (candidate != null && candidate.opaque_ref == o.opaque_ref))
            {
                if (!CanSelectNode(node))
                {
                    cancelled = true;
                    return false;
                }

                SelectedNode = node;

                if (expand)
                    node.Expand();

                return true;
            }

            foreach (VirtualTreeNode child in node.Nodes)
            {
                if (SelectObject(o, child, expand, ref cancelled))
                    return true;
            }

            return false;
        }
    }
}
