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
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace XenAdmin.Controls
{
    /// <summary>
    /// This interface is used to hide these members from general use.
    /// </summary>
    internal interface IMultiSelectTreeNode
    {
        void SetSelected(bool selected);
        void SetEnabled(bool enabled);
    }

    public class MultiSelectTreeNode : TreeNode, IMultiSelectTreeNodeCollectionOwner, IMultiSelectTreeNode
    {
        private readonly MultiSelectTreeNodeCollection _nodes;
        private Color _backColor;
        private Color _foreColor;
        private bool _selected;
        private bool _enabled;
        private Color _highlightForeColor = SystemColors.HighlightText;
        private Color _highlightBackColor = SystemColors.Highlight;
        private bool _showCheckBox;

        public MultiSelectTreeNode()
            : this(string.Empty)
        {
        }

        public MultiSelectTreeNode(string text)
            : base(text)
        {
            _nodes = new MultiSelectTreeNodeCollection(this);
            _backColor = BackColor;
            _foreColor = ForeColor;
        }

        /// <summary>
        /// Gets or sets the back-color of the treeNode from the base class. If this is done by setting the BackColor of the base class, then
        /// the entire treeview is invalidated which can cause flicker. It is therefore achieved by setting the internal propbag and then invalidating
        /// just this node.
        /// </summary>
        private Color BaseBackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (value != base.BackColor)
                {
                    if (TreeView != null && PropBag != null && TreeView.IsHandleCreated)
                    {
                        PropBag.BackColor = value;
                        TreeView.Invalidate(Bounds);
                    }
                    else
                    {
                        base.BackColor = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the fore-color of the treeNode from the base class. If this is done by setting the BackColor of the base class, then
        /// the entire treeview is invalidated which can cause flicker. It is therefore achieved by setting the internal propbag and then invalidating
        /// just this node.
        /// </summary>
        private Color BaseForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                if (value != base.ForeColor)
                {
                    if (TreeView != null && PropBag != null && TreeView.IsHandleCreated)
                    {
                        PropBag.ForeColor = value;
                        TreeView.Invalidate(Bounds);
                    }
                    else
                    {
                        base.ForeColor = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the internal prop bag for this node. Users of this property should always check for null after calling this property.
        /// </summary>
        private OwnerDrawPropertyBag PropBag
        {
            get
            {
                FieldInfo fieldInfo = GetType().GetField("propBag", BindingFlags.NonPublic | BindingFlags.Instance);

                if (fieldInfo != null)
                {
                    return (OwnerDrawPropertyBag)fieldInfo.GetValue(this);
                }
                
                return null;
            }
        }

        private void UpdateColors()
        {
            if (_enabled && _selected)
            {
                BaseBackColor = _highlightBackColor;
                BaseForeColor = _highlightForeColor;
            }
            else
            {
                BaseBackColor = _backColor;
                BaseForeColor = _foreColor;
            }
        }

        public new Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
                if (!_selected)
                {
                    BaseBackColor = value;
                }
            }
        }

        public new Color ForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                _foreColor = value;

                if (!_selected)
                {
                    BaseForeColor = value;
                }
            }
        }

        public Color HighlightForeColor
        {
            get
            {
                return _highlightForeColor;
            }
            set
            {
                _highlightForeColor = value;
                UpdateColors();
            }
        }

        public Color HighlightBackColor
        {
            get
            {
                return _highlightBackColor;
            }
            set
            {
                _highlightBackColor = value;
                UpdateColors();
            }
        }

        public new MultiSelectTreeNode NextVisibleNode
        {
            get
            {
                return (MultiSelectTreeNode)base.NextVisibleNode;
            }
        }

        public new MultiSelectTreeNode PrevVisibleNode
        {
            get
            {
                return (MultiSelectTreeNode)base.PrevVisibleNode;
            }
        }

        public new MultiSelectTreeNode Parent
        {
            get
            {
                return (MultiSelectTreeNode)base.Parent;
            }
        }

        private bool IsTreeViewEnabled()
        {
            // iterate through ancestors to find treeview as this node might have only just been
            // added and therefore the TreeView property will still return null.

            MultiSelectTreeNode node = this;

            while (node != null)
            {
                if (node.TreeView != null)
                {
                    return node.TreeView.Enabled;
                }
                node = node.Parent;
            }
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the tree node is in the selected state.
        /// </summary>
        /// <value></value>
        /// <returns>true if the tree node is in the selected state; otherwise, false.
        /// </returns>
        public new bool IsSelected
        {
            get
            {
                return _selected;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a CheckBox should be shown on this node. This
        /// only takes effect if TreeView.ShowCheckBoxes is set to true.
        /// </summary>
        public bool ShowCheckBox
        {
            get
            {
                return _showCheckBox;
            }
            set
            {
                if (_showCheckBox != value)
                {
                    _showCheckBox = value;
                    if (TreeView != null)
                    {
                        TreeView.UpdateCheckboxVisibility(this);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates through all the descendants of this <see cref="VirtualTreeNode"/>.
        /// </summary>
        public IEnumerable<MultiSelectTreeNode> Descendants
        {
            get
            {
                foreach (MultiSelectTreeNode node in Nodes)
                {
                    yield return node;

                    foreach (MultiSelectTreeNode n in node.Descendants)
                    {
                        yield return n;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the parent tree view that the tree node is assigned to.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="MultiSelectTreeView"/> that represents the parent tree view that the tree node is assigned to, or null if the node has not been assigned to a tree view.
        /// </returns>
        public new MultiSelectTreeView TreeView
        {
            get
            {
                return (MultiSelectTreeView)base.TreeView;
            }
        }

        #region IMultiSelectTreeNodeCollectionOwner Members

        public new MultiSelectTreeNodeCollection Nodes
        {
            get { return _nodes; }
        }

        #endregion

        #region IMultiSelectTreeNode Members

        void IMultiSelectTreeNode.SetSelected(bool selected)
        {
            _enabled = IsTreeViewEnabled();
            _selected = selected;
            UpdateColors();
        }

        void IMultiSelectTreeNode.SetEnabled(bool enabled)
        {
            _enabled = enabled;
            UpdateColors();
        }

        #endregion
    }
}
