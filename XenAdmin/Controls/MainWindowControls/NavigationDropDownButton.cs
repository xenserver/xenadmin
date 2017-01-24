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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XenAdmin.Controls.MainWindowControls
{
    class NavigationDropDownButton : ToolStripDropDownButton, INavigationItem
    {
        private ToolStripItem[] m_itemList;
        private bool m_resettingItemList;

        protected NavigationDropDownButton()
        {
            Overflow = ToolStripItemOverflow.Never;
        }

        public INavigationItem PairedItem { get; set; }
        public event Action<object> NavigationViewChanged;

        public void SetItemList(ToolStripItem[] items)
        {
            if (m_resettingItemList)
                return;

            m_resettingItemList = true;
            
            m_itemList = items;

            var pairedButton = PairedItem as NavigationDropDownButton;
            if (pairedButton != null)
                pairedButton.SetItemList(items);
            
            m_resettingItemList = false;
        }

        protected override void OnDropDownShow(EventArgs e)
        {
            DropDownItems.Clear();
            DropDownItems.AddRange(m_itemList);
            base.OnDropDownShow(e);
        }

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            var curItem = e.ClickedItem as ToolStripMenuItem;

            if (curItem != null && !curItem.Checked)
            {
                foreach (ToolStripMenuItem item in DropDownItems)
                {
                    if (item != null)
                        item.Checked = item == curItem;
                }

                if (NavigationViewChanged != null)
                    NavigationViewChanged(curItem.Tag);
            }
            base.OnDropDownItemClicked(e);
        }
    }


    class NavigationDropDownButtonBig : NavigationDropDownButton
    {
        public NavigationDropDownButtonBig()
        {
            ImageAlign = ContentAlignment.MiddleLeft;
            ImageScaling = ToolStripItemImageScaling.None;
        }
    }


    class NavigationDropDownButtonSmall : NavigationDropDownButton
    {
        public NavigationDropDownButtonSmall()
        {
            Alignment = ToolStripItemAlignment.Right;
            ImageScaling = ToolStripItemImageScaling.None;
        }
    }
}
