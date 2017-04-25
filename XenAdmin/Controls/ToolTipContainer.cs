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
using System.ComponentModel.Design;

namespace XenAdmin.Controls
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class ToolTipContainer : Panel
    {
        /// <summary>
        /// If true, prevents the tooltip from appearing.
        /// </summary>
        public bool SuppressTooltip = false;

        EmptyPanel emptyPanel1;
        public ToolTipContainer()
        {
            InitializeComponent();
        }

        public Control TheControl;

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is EmptyPanel)
                return;
            TheControl = e.Control;
            e.Control.Dock = DockStyle.Fill;
            emptyPanel1 = new EmptyPanel();
            emptyPanel1.Dock = DockStyle.Fill;
            Controls.Add(emptyPanel1);
            emptyPanel1.BringToFront();
            emptyPanel1.Enabled = !TheControl.Enabled;
            TheControl.EnabledChanged += new EventHandler(TheControl_EnabledChanged);
        }

        private void TheControl_EnabledChanged(object sender, EventArgs e)
        {
            emptyPanel1.Enabled = !TheControl.Enabled;
        }

        public void SetToolTip(string text)
        {
            toolTip1.RemoveAll();
            toolTip1.Popup += new PopupEventHandler(toolTip1_Popup);
            toolTip1.SetToolTip(emptyPanel1, text);
        }

        public void RemoveAll()
        {
            toolTip1.RemoveAll();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            if (!e.AssociatedControl.ClientRectangle.Contains(e.AssociatedControl.PointToClient(MousePosition))
                || SuppressTooltip)
            {
                e.Cancel = true;
            }
        }
    }
}
