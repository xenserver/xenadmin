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

namespace XenAdmin.Controls.XenSearch
{
    public class DropDownComboButton : Button
    {
        internal readonly NonReopeningContextMenuStrip contextMenuStrip = new NonReopeningContextMenuStrip();
        internal List<ToolStripItem> Items = new List<ToolStripItem>();
        private bool itemsChanged = true;
        private ToolStripItem selectedItem;
        public event EventHandler SelectedItemChanged;
        public event EventHandler ItemSelected;
        public event EventHandler BeforeItemSelected;

        public DropDownComboButton()
        {
            this.Text = "";
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.TextImageRelation = TextImageRelation.TextBeforeImage;
            this.Padding = new Padding(0, 0, 2, 0);
            this.Image = Properties.Resources.expanded_triangle;
            this.ImageAlign = ContentAlignment.MiddleRight;
        }

        public ToolStripItem SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
                OnBeforeItemSelected();

                ToolStripItem oldvalue = selectedItem;
                selectedItem = value;

                if (selectedItem == null)
                    return;

                OnItemSelected();

                bool areNotEqual = oldvalue == null ||
                     (selectedItem.Text != oldvalue.Text && selectedItem.Tag != oldvalue.Tag);

                if (areNotEqual)
                {
                    SetButtonTextFromItem(value);
                    OnSelectedItemChanged();
                }
            }
        }

        protected virtual void SetButtonTextFromItem(ToolStripItem item)
        {
            this.Text = item.Text;
        }

        public void AddItem(ToolStripItem item)
        {
            item.Click += new EventHandler(item_Click);
            item.AutoSize = true;
            Items.Add(item);
            itemsChanged = true;
        }

        public void ClearItems()
        {
            Items.Clear();
            itemsChanged = true;
        }

        private void item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
                return;

            SelectedItem = item;
        }

        protected virtual void OnBeforeItemSelected()
        {
            if (BeforeItemSelected != null)
                BeforeItemSelected(this, new EventArgs());
        }

        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new EventArgs());
        }

        protected virtual void OnItemSelected()
        {
            if (ItemSelected != null)
                ItemSelected(this, new EventArgs());
        }

        public event EventHandler BeforePopup;

        protected virtual void OnBeforePopup(EventArgs e)
        {
            if (BeforePopup != null)
                BeforePopup(this, e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (contextMenuStrip.CanOpen)
            {
                OnBeforePopup(e);
                if (itemsChanged)
                {
                    contextMenuStrip.Items.Clear();
                    contextMenuStrip.Items.AddRange(Items.ToArray());
                    itemsChanged = false;
                }
                contextMenuStrip.Show(this.Parent, this.Left + 1, this.Bottom - 1);
            }
        }

        public bool SelectItem<T>(Predicate<T> predicate)
        {
            foreach (ToolStripItem item in Items)
            {
                if (!item.CanSelect)
                    continue;

                if (predicate((T)item.Tag))
                {
                    SelectedItem = item;
                    return true;
                }
            }

            return false;  // nothing matched
        }
    }
}