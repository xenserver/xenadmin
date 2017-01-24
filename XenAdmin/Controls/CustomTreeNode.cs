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
using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace XenAdmin.Controls
{
    public class CustomTreeNode : IComparable<CustomTreeNode>, IEquatable<CustomTreeNode>
    {
        private bool selectable = true;
        private bool _expanded = true;

        public bool Enabled = true;
        public bool PreferredExpanded = true;

        /// <summary>
        /// DO NOT ADD NODES DIRECTLY TO OTHER NODES!!
        /// use CustomTreeView.AddChildNode
        /// </summary>
        public readonly List<CustomTreeNode> ChildNodes = new List<CustomTreeNode>();

        private CheckState _state;
        public string Text = "new_node";
        public string Description = "a_node";
        public int Level = -1;
        public int ChildNumber = -1;
        public CustomTreeNode ParentNode = null;
        public Image Image;
        public object Tag;
        public bool HideCheckbox;

        public CustomTreeNode()
            : this(true)
        {
        }

        public CustomTreeNode(bool selectable)
        {
            this.selectable = selectable;
        }

        public bool Selectable
        {
            get
            {
                return selectable;
            }
        }

        public CheckState State
        {
            get
            {
                return _state;
            }
            set
            {
                if(value == _state && Level != -1)
                    return;

                _state = value;

                if (value != CheckState.Indeterminate)
                    foreach (CustomTreeNode child in ChildNodes)
                        if (child.State != value && child.Enabled)
                            child.State = value;

                if (Level == -1)
                    return;

                CheckState initial = value;
                if (initial != CheckState.Indeterminate)
                {
                    foreach (CustomTreeNode c in ParentNode.ChildNodes)
                    {
                        if (c.State != initial && c.Enabled)
                        {
                            initial = CheckState.Indeterminate;
                            break;
                        }
                    }
                }

                ParentNode.State = initial;
            }                
        }

        public bool CheckedIfdisabled = true;

        public bool Expanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                _expanded = value;
                foreach (CustomTreeNode child in ChildNodes)
                {
                    if (!value)
                        child.Expanded = value;
                    else
                        child.Expanded = child.PreferredExpanded;
                }
            }
        }

        public void AddChild(CustomTreeNode child)
        {
            child.Level = this.Level + 1;
            child.ParentNode = this;
            child.ChildNumber = ChildNodes.Count;
            this.ChildNodes.Add(child);
            if (this.State == CheckState.Indeterminate)
                // This line is necessary. Apparently.
                child.State = child.State;
        }

        public bool isDescendantOf(CustomTreeNode parent)
        {
            if (this.Level <= parent.Level || this.ParentNode == null)
            {
                return false;
            }
            else if (this.ParentNode == parent)
            {
                return true;
            }
            else
            {
                return this.ParentNode.isDescendantOf(parent);
            }
        }

        public bool Equals(CustomTreeNode other)
        {
            if (this.Level != other.Level)
                return false;
            else if (this.Level == -1 && other.Level == -1)
                return true;
            else if (this.ChildNumber != other.ChildNumber)
                return false;
            else
                return this.ParentNode.Equals(other.ParentNode);
        }

        /// <summary>
        /// DO NOT OVERRIDE TO SET SORT ORDER
        /// override SameLevelSortOrder(CutstomTreeNode other) instead
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(CustomTreeNode other)
        {
            if (this == other)
                return 0;
            else if (other.isDescendantOf(this))
                return -1;
            else if (this.isDescendantOf(other))
                return 1;
            else if (this.ParentNode == other.ParentNode && SameLevelSortOrder(other) == 0)
                return this.ChildNumber - other.ChildNumber;
            else if (this.ParentNode == other.ParentNode)
                return SameLevelSortOrder(other);
            else if (this.Level < other.Level)
                return this.CompareTo(other.ParentNode);
            else if (this.Level > other.Level)
                return this.ParentNode.CompareTo(other);
            else
                return this.ParentNode.CompareTo(other.ParentNode);
        }

        protected virtual int SameLevelSortOrder(CustomTreeNode other)
        {
            return this.ToString().CompareTo(other.ToString());
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
