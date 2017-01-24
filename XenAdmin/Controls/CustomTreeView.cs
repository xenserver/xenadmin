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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Controls
{
    public partial class CustomTreeView : FlickerFreeListBox
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// SURGEON GENERAL'S WARNING: This collection contains the infamous 'secret node'.
        /// To iterate only through items that you have explicity added to the treeview, use
        /// the Items collection instead.
        /// </summary>
        public readonly List<CustomTreeNode> Nodes = new List<CustomTreeNode>();
        private VisualStyleRenderer plusRenderer;
        private VisualStyleRenderer minusRenderer;
        private CustomTreeNode lastSelected;
        private bool _inUpdate = false;
        
        /// <summary>
        /// If you want to make this into a regular listbox, set this to a smaller value, like 5 or something
        /// </summary>
        private int _nodeIndent = 19;

        [Browsable(true)]
        public int NodeIndent
        {
            get { return _nodeIndent; }
            set { _nodeIndent = value; }
        }
        
        public CustomTreeNode SecretNode = new CustomTreeNode();

        private bool _showCheckboxes = true;
        [Browsable(true)]
        public bool ShowCheckboxes
        {
            get { return _showCheckboxes; }
            set { _showCheckboxes = value; }
        }

        private bool _showDescription = true;
        [Browsable(true)]
        public bool ShowDescription
        {
            get { return _showDescription; }
            set { _showDescription = value; }
        }

        private bool _showImages = false;
        [Browsable(true)]
        public bool ShowImages
        {
            get { return _showImages; }
            set { _showImages = value; }
        }

        /// <summary>
        /// The font used in descriptions.
        /// </summary>
        private Font _descriptionFont = null;

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                if (_descriptionFont != null)
                    _descriptionFont.Dispose();
                _descriptionFont = new Font(value.FontFamily, value.Size - 1);
                RecalculateWidth();
            }
        }

        private bool _showRootLines = true;

        [Browsable(true)]
        public bool ShowRootLines
        {
            get { return _showRootLines; }
            set { _showRootLines = value; }
        }

        private bool _rootAlwaysExpanded = false;
        [Browsable(true)]
        public bool RootAlwaysExpanded
        {
            get { return _rootAlwaysExpanded; }
            set { _rootAlwaysExpanded = value; }
        }

		public override int ItemHeight { get { return 17; } }
		
		public CustomTreeView()
        {
            InitializeComponent();
            _descriptionFont = new Font(base.Font.FontFamily, base.Font.Size - 2);

            if (Application.RenderWithVisualStyles)
            {
                plusRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                minusRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
            }
        }

        public new void BeginUpdate()
        {
            _inUpdate = true;
            base.BeginUpdate();
        }

        public new void EndUpdate()
        {
            _inUpdate = false;
            base.EndUpdate();
            RecalculateWidth();
            Resort();
            Refresh();
        }

        public new void Invalidate()
        {
            RecalculateWidth();
            base.Invalidate();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
       
            if(Enabled)
            {
                using (SolidBrush backBrush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            }

            if (e.Index == -1 || Items.Count <= e.Index)
                return;

            CustomTreeNode node = this.Items[e.Index] as CustomTreeNode;

            if (node == null)
                return;

            //int indent = (node.Level + 1) * NodeIndent;
            int indent = node.Level * NodeIndent + (ShowRootLines ? NodeIndent : 2);

            int TextLength = Drawing.MeasureText(node.ToString(), e.Font).Width + 2;
            int TextLeft = indent + (ShowCheckboxes && !node.HideCheckbox ? ItemHeight : 0) + (ShowImages ? ItemHeight : 0);

			//CA-59618: add top margin to the items except the first one when rendering with
			//visual styles because in this case there is already one pixel of margin.
			int topMargin = Application.RenderWithVisualStyles && e.Index == 0 ? 0 : 1;

            if (Enabled && node.Selectable)
            {
                Color nodeBackColor = node.Enabled
                                          ? e.BackColor
                                          : (e.BackColor == BackColor ? BackColor : SystemColors.ControlLight);

                using (SolidBrush backBrush = new SolidBrush(nodeBackColor))
                {
                    e.Graphics.FillRectangle(backBrush, new Rectangle(e.Bounds.Left + TextLeft + 1, e.Bounds.Top + topMargin, TextLength - 4, e.Bounds.Height));
                }
            }

            //draw expander
            if (node.ChildNodes.Count > 0 && (ShowRootLines || node.Level > 0))
            {
                if (!node.Expanded)
                {
                    if(Application.RenderWithVisualStyles)
						plusRenderer.DrawBackground(e.Graphics, new Rectangle(e.Bounds.Left + indent - ItemHeight, e.Bounds.Top + 3 + topMargin, 9, 9));
                    else
						e.Graphics.DrawImage(Properties.Resources.tree_plus, new Rectangle(e.Bounds.Left + indent - ItemHeight, e.Bounds.Top + 3 + topMargin, 9, 9));
                }
                else
                {
                    if (Application.RenderWithVisualStyles)
						minusRenderer.DrawBackground(e.Graphics, new Rectangle(e.Bounds.Left + indent - ItemHeight, e.Bounds.Top + 3 + topMargin, 9, 9));
                    else
						e.Graphics.DrawImage(Properties.Resources.tree_minus, new Rectangle(e.Bounds.Left + indent - ItemHeight, e.Bounds.Top + 3 + topMargin, 9, 9));
                }
            }

            //draw checkboxes
            if (ShowCheckboxes && !node.HideCheckbox)
            {
                var checkedState = CheckBoxState.UncheckedDisabled;

                if (node.State == CheckState.Checked)
                {
                    if (node.Enabled && Enabled)
                        checkedState = CheckBoxState.CheckedNormal;
                    else if (node.CheckedIfdisabled)
                        checkedState = CheckBoxState.CheckedDisabled;
                }
                else if (node.State == CheckState.Indeterminate)
                {
                    checkedState = node.Enabled && Enabled
                                       ? CheckBoxState.MixedNormal
                                       : CheckBoxState.MixedDisabled;
                }
                else if (node.State == CheckState.Unchecked)
                {
                    checkedState = node.Enabled && Enabled
                                       ? CheckBoxState.UncheckedNormal
                                       : CheckBoxState.UncheckedDisabled;
                }

                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.Left + indent, e.Bounds.Top + 1 + topMargin), checkedState);
                indent += ItemHeight;
            }

            //draw images
            if (ShowImages && node.Image != null)
            {
                var rectangle = new Rectangle(e.Bounds.Left + indent, e.Bounds.Top + topMargin, node.Image.Width, node.Image.Height);

                if (node.Enabled && Enabled)
                    e.Graphics.DrawImage(node.Image, rectangle);
                else
                    e.Graphics.DrawImage(node.Image, rectangle, 0, 0, node.Image.Width, node.Image.Height, GraphicsUnit.Pixel, Drawing.GreyScaleAttributes);

                indent += ItemHeight;
            }

            //draw item's main text
            Color textColor = node.Enabled && Enabled
                                  ? (node.Selectable ? e.ForeColor : ForeColor)
                                  : SystemColors.GrayText;

            Drawing.DrawText(e.Graphics, node.ToString(), e.Font, new Point(e.Bounds.Left + indent, e.Bounds.Top + topMargin), textColor);
            indent += TextLength;

            //draw item's description
            if (ShowDescription)
            {
				Drawing.DrawText(e.Graphics, node.Description, _descriptionFont, new Point(e.Bounds.Left + indent, e.Bounds.Top + 1 + topMargin), SystemColors.GrayText);
            }
        }

        public List<CustomTreeNode> CheckedItems()
        {
            List<CustomTreeNode> nodes = new List<CustomTreeNode>();
            foreach (CustomTreeNode node in Nodes)
                if (node.Level >= 0 && node.State == CheckState.Checked && node.Enabled)
                    nodes.Add(node);
            return nodes;
        }

        public List<CustomTreeNode> CheckableItems()
        {
            List<CustomTreeNode> nodes = new List<CustomTreeNode>();
            foreach (CustomTreeNode node in Nodes)
                if (node.Level >= 0 && node.State != CheckState.Checked && node.Enabled)
                    nodes.Add(node);
            return nodes;
        }
        
        public void AddNode(CustomTreeNode node)
        {
            if (Nodes.Count == 0)
                Nodes.Add(SecretNode);
            SecretNode.AddChild(node);
            Nodes.Add(node);
            if (!_inUpdate)
            {
                RecalculateWidth();
                Resort();
                Refresh();
            }
        }

        public void RemoveNode(CustomTreeNode node)
        {
            Nodes.Remove(node);
            if (!_inUpdate)
            {
                RecalculateWidth();
                Resort();
                Refresh();
            }
        }

        public void AddChildNode(CustomTreeNode parent, CustomTreeNode child)
        {
            parent.AddChild(child);
            Nodes.Add(child);
            if (!_inUpdate)
            {
                RecalculateWidth();
                Resort();
                Refresh();
            }
        }

        public void ClearAllNodes()
        {
            Nodes.Clear();
            if (!_inUpdate)
            {
                RecalculateWidth();
                Resort();
                Refresh();
            }
        }

        public void Resort()
        {
            try
            {
                lastSelected = SelectedItem as CustomTreeNode;
            }
            catch (IndexOutOfRangeException)
            {
                // Accessing ListBox.SelectedItem sometimes throws an IndexOutOfRangeException (See CA-24396)
                log.Warn("IndexOutOfRangeException in ListBox.SelectedItem");
                lastSelected = null;
            }
            Nodes.Sort();
            Items.Clear();
            foreach (CustomTreeNode node in Nodes)
            {
                if (node.Level != -1 && node.ParentNode.Expanded)
                    Items.Add(node);
            }
            SelectedItem = lastSelected;
            // I've yet to come across the above assignement working. If we fail to restore the selection, select something so the user can see focus feedback
            // (the color of the selected item is the only indication as to whether it is focused or not)
            // Iterating through and using CustomTreeNode.equals is useless here as it compares based on index, which I think is why the above call almost never works
            if (SelectedItem == null && lastSelected != null && Items.Count > 0)
            {
                SelectedItem = Items[0];
            }
        }

        // Adjusts the width of the control to that of the widest row
        private void RecalculateWidth()
        {
            int maxWidth = 0;
            foreach (CustomTreeNode node in this.Nodes)
            {
                int indent = (node.Level + 1) * NodeIndent;
                int checkbox = ShowCheckboxes && !node.HideCheckbox ? ItemHeight : 0;
                int image = ShowImages ? ItemHeight : 0;
                int text = Drawing.MeasureText(node.ToString(), this.Font).Width + 2;
                int desc = ShowDescription ? Drawing.MeasureText(node.Description, _descriptionFont).Width : 0;
                int itemWidth = indent + checkbox + image + text + desc + 10;
                maxWidth = Math.Max(itemWidth, maxWidth);
            }
            // Set horizontal extent and enable scrollbar if necessary
            this.HorizontalExtent = maxWidth;
            this.HorizontalScrollbar = this.HorizontalExtent > this.Width && Enabled;
        }

        /// <summary>
        /// Finds next/previous node in Items collection.
        /// </summary>
        /// <param name="currentNode">Node where the search for next/previous node will start.</param>
        /// <param name="searchForward">Determines direction of search (search for next or previous node).</param>
        /// <returns></returns>
        protected CustomTreeNode GetNextNode(CustomTreeNode currentNode, bool searchForward)
        {
            if (currentNode == null)
                return null;

            int index = Items.IndexOf(currentNode);
            if (searchForward)
            {
                index++;
                if (index >= Items.Count)
                    index = -1;
            }
            else
                index--;

            if (index < 0)
                return null;
            return (CustomTreeNode)Items[index];
        }

        /// <summary>
        /// Finds next/previous enabled node in Items collection.
        /// </summary>
        /// <param name="currentNode">Node where the search for next/previous enabled node will start.</param>
        /// <param name="searchForward">Determines direction of search (search for next or previous node).</param>
        /// <returns></returns>
        protected CustomTreeNode GetNextEnabledNode(CustomTreeNode currentNode, bool searchForward)
        {
            if (currentNode == null)
                return null;
            CustomTreeNode nextNode = GetNextNode(currentNode, searchForward);
            if (nextNode == null)
                return null;
            if (nextNode.Enabled)
                return nextNode;
            return GetNextEnabledNode(nextNode, searchForward);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            bool anythingChanged = false;
            bool orderChanged = false;
            Point loc = this.PointToClient(MousePosition);
            int index = this.IndexFromPoint(loc);
            if (index < 0 || index > Items.Count)
                return;

            CustomTreeNode node = this.Items[index] as CustomTreeNode;

            if (node == null)
                return;

            int indent = node.Level * NodeIndent + (ShowRootLines ? NodeIndent : 2);

            if (node.ChildNodes.Count > 0 && loc.X < indent - (ItemHeight - 9) && loc.X > indent - ItemHeight &&
                (ShowRootLines || node.Level > 0))
            {
                node.Expanded = !node.Expanded;
                node.PreferredExpanded = node.Expanded;
                anythingChanged = true;
                orderChanged = true;
            }
            else if (ShowCheckboxes && !node.HideCheckbox && node.Enabled && loc.X > indent && loc.X < indent + ItemHeight)
            {
                if (node.State == CheckState.Unchecked || node.State == CheckState.Indeterminate)
                    node.State = CheckState.Checked;
                else
                    node.State = CheckState.Unchecked;
                anythingChanged = true;
            }
            if (orderChanged)
                Resort();
            if (anythingChanged)
            {
                if(ItemCheckChanged != null)
                    ItemCheckChanged(node, new EventArgs());
                Refresh();
            }
            base.OnMouseUp(e);
        }

        public event EventHandler<EventArgs> ItemCheckChanged;
        public event EventHandler DoubleClickOnRow;

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            bool anythingChanged = false;
            Point loc = this.PointToClient(MousePosition);
            int index = this.IndexFromPoint(loc);
            if (index < 0 || index > Items.Count)
                return;

            CustomTreeNode node = this.Items[index] as CustomTreeNode;

            if (node == null)
                return;

            int indent = node.Level * NodeIndent + (ShowRootLines ? NodeIndent : 2);

            if (node.ChildNodes.Count > 0 && loc.X < indent - (ItemHeight - 9) && loc.X > indent - ItemHeight &&
                (ShowRootLines || node.Level > 0))
            {
                return;
            }
            else if (ShowCheckboxes && !node.HideCheckbox && loc.X > indent && loc.X < indent + ItemHeight)
            {
                return;
            }
            else if (node.ChildNodes.Count > 0 && (node.Level > 0 || !_rootAlwaysExpanded))
            {
                node.Expanded = !node.Expanded;
                node.PreferredExpanded = node.Expanded;
                anythingChanged = true;
            }

            if (anythingChanged)
            {
                Resort();
                Refresh();
            }

            if (DoubleClickOnRow != null)
                DoubleClickOnRow(this, e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            var node = SelectedItem as CustomTreeNode;

            switch (e.KeyCode)
            {
                case Keys.Space:
                    {
                        if (!ShowCheckboxes)
                            break;
                        if (node == null || node.HideCheckbox || !node.Enabled)
                            break;

                        //checked => uncheck it; unchecked or indeterminate => check it
                        node.State = node.State == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;
                        Refresh();

                        if (ItemCheckChanged != null)
                            ItemCheckChanged(node, new EventArgs());

                        break;
                    }
            }
            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var node = SelectedItem as CustomTreeNode;

            switch (e.KeyCode)
            {
                case Keys.Right:
                    {
                        if (node != null && node.ChildNodes.Count > 0 && (node.Level > 0 || !_rootAlwaysExpanded) && !node.Expanded)
                        {
                            node.Expanded = true;
                            Resort();
                            Refresh();
                            e.Handled = true;
                        }
                        break;
                    }
                case Keys.Left:
                    {
                        if (node != null && node.ChildNodes.Count > 0 && (node.Level > 0 || !_rootAlwaysExpanded) && node.Expanded)
                        {
                            node.Expanded = false;
                            Resort();
                            Refresh();
                            e.Handled = true;
                        }
                        break;
                    }
            }
            base.OnKeyDown(e);
        }
    }
}
