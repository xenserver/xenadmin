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
    class NavigationToolStrip : ToolStripEx
    {
        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            var button = e.Item as ToolStripButton;

            if (button != null)
                button.CheckedChanged += button_CheckedChanged;

            var dropDownButton = e.Item as ToolStripDropDownButton;

            if (dropDownButton != null)
                dropDownButton.DropDownItemClicked += dropDownButton_DropDownItemClicked;

            base.OnItemAdded(e);
        }

        private void button_CheckedChanged(object sender, EventArgs e)
        {
            var checkedButton = sender as ToolStripButton;

            if (checkedButton == null || !checkedButton.Checked)
                return;

            foreach (var toolStripItem in Items)
            {
                var buttonItem = toolStripItem as ToolStripButton;

                if (buttonItem != null && buttonItem != checkedButton)
                    buttonItem.Checked = false;

                var dropDownButton = toolStripItem as ToolStripDropDownButton;

                if (dropDownButton != null)
                {
                    foreach (ToolStripMenuItem item in dropDownButton.DropDownItems)
                        item.Checked = false;
                }
            }
            
            Invalidate();
        }

        private void dropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var curItem = e.ClickedItem as ToolStripMenuItem;

            if (curItem == null || !curItem.Checked)
                return;

            foreach (var toolStripItem in Items)
            {
                var buttonItem = toolStripItem as ToolStripButton;

                if (buttonItem != null && buttonItem.Checked)
                    buttonItem.Checked = false;

                var dropDownButton = toolStripItem as ToolStripDropDownButton;

                if (dropDownButton != null && dropDownButton != curItem.OwnerItem)
                {
                    foreach (ToolStripMenuItem item in dropDownButton.DropDownItems)
                        item.Checked = false;
                }
            }

            Invalidate();
        }
    }

    class NavigationToolStripBig : NavigationToolStrip
    {
        public NavigationToolStripBig()
        {
            RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Renderer = new BigNavigationToolStripRenderer();
        }
    }

    class NavigationToolStripSmall : NavigationToolStrip
    {
        public NavigationToolStripSmall()
        {
            RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Renderer = new SmallNavigationToolStripRenderer();
        }
    }
}
