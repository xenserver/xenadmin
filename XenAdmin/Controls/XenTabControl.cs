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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Drawing2D;

namespace XenAdmin.Controls
{
    [Designer(typeof(ParentControlDesigner))]
    public partial class XenTabControl : Control
    {
        private XenTabPage selectedTab = null;

        public event EventHandler TabChanged;

        public int SelectedIndex
        {
            get
            {
                return
                    selectedTab != null && Controls.Contains(selectedTab) ?
                        Controls.GetChildIndex(selectedTab) :
                        -1;
            }

            set
            {
                SelectedTab =
                    value > -1 && value < Controls.Count ?
                         (XenTabPage)Controls[value] :
                         null;
            }
        }

        public XenTabPage SelectedTab
        {
            get
            {
                return selectedTab;
            }

            set
            {
                if (selectedTab == value)
                    return;
                if (selectedTab != null)
                    selectedTab.Visible = false;
                selectedTab = value;
                if (selectedTab != null)
                    selectedTab.Visible = true;
                OnTabChanged(new EventArgs());
                Invalidate();
                Update();
            }
        }

        public XenTabControl()
        {
            InitializeComponent();
        }

        protected override bool ScaleChildren
        {
            get { return false; }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.Visible = false;
        }

        private void OnTabChanged(EventArgs ea)
        {
            if (TabChanged != null)
                TabChanged(this, ea);
        }
    }
}
