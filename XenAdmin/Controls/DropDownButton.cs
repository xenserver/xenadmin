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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using XenAdmin.Properties;

namespace XenAdmin.Controls
{
    public partial class DropDownButton : Button
    {
        private ContextMenuStrip _contextMenuStrip;
        private bool _ignoreNextClick;

        public DropDownButton()
        {
            InitializeComponent();

            base.Image = Resources.expanded_triangle;
            base.ImageAlign = ContentAlignment.MiddleRight;
            DoubleBuffered = true;
        }

        protected override void OnContextMenuStripChanged(EventArgs e)
        {
            base.OnContextMenuStripChanged(e);

            if (_contextMenuStrip != null)
            {
                _contextMenuStrip.Closed -= _contextMenuStrip_Closed;
                _contextMenuStrip.Opening -= _contextMenuStrip_Opening;
            }

            _contextMenuStrip = ContextMenuStrip;

            if (_contextMenuStrip != null)
            {
                _contextMenuStrip.Closed += _contextMenuStrip_Closed;
                _contextMenuStrip.Opening += _contextMenuStrip_Opening;

                // need to add dummy so context menu strip opens even it there aren't any items.
                if (_contextMenuStrip.Items.Count == 0)
                {
                    _contextMenuStrip.Items.Add(new ToolStripMenuItem { Name = "dummy" });
                }
            }
        }

        private void _contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            _contextMenuStrip.Items.RemoveByKey("dummy");
        }

        private void _contextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            // ensure that if the user clicks on the button when the menu is open, then the menu is closed,
            // (not opened again.)

            _ignoreNextClick = ClientRectangle.Contains(PointToClient(Cursor.Position));
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (_contextMenuStrip != null && !_ignoreNextClick)
            {
                _contextMenuStrip.Show(this, new Point(0, Height), ToolStripDropDownDirection.Default);
            }

            _ignoreNextClick = false;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image
        {
            get
            {
                return base.Image;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ContentAlignment ImageAlign
        {
            get
            {
                return base.ImageAlign;
            }
        }
    }
}
