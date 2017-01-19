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
using XenAdmin.Model;
using XenAPI;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Actions;

namespace XenAdmin.Controls
{
    public class FolderEditor : DoubleBufferedPanel
    {
        private readonly FolderListItem _folderListItem;
        public FolderEditor(string folder)
            : base()
        {
            _folderListItem = new FolderListItem(folder, FolderListItem.AllowSearch.None, true);
            _folderListItem.PathChanged += folderListItem_PathChanged;
            _folderListItem.Parent = this;
        }

        public string Path
        {
            get { return _folderListItem.Path; }
        }

        private void folderListItem_PathChanged(object sender, EventArgs e)
        {
            RefreshBuffer();
            Refresh();
            Height = _folderListItem.PreferredSize.Height;
        }

        protected override void OnDrawToBuffer(PaintEventArgs e)
        {
            // Hack.  See Program.TransparentUsually.
            e.Graphics.TextContrast = 5;
            _folderListItem.DrawSelf(e.Graphics, new Rectangle(0, 0, Width, Height), false);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _folderListItem.OnMouseMove(e.Location);
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            _folderListItem.OnMouseClick(e, e.Location);
            base.OnMouseClick(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _folderListItem.OnMouseLeave();
            base.OnMouseLeave(e);
        }
    }
}
