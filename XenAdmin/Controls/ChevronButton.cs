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

namespace XenAdmin.Controls
{
    public partial class ChevronButton : UserControl
    {
        public ChevronButton()
        {
            InitializeComponent();

        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Bindable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }

        public Image Image
        {
            get
            {
                return pictureBoxButton.Image;
            }
            set
            {
                pictureBoxButton.Image = value;
            }
        }


        public event EventHandler ButtonClick
        {
            add { pictureBoxButton.Click += value; }
            remove { pictureBoxButton.Click -= value; }
        }

        private bool hasFocus = false;
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            hasFocus = true;
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            hasFocus = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle focusRect = this.ClientRectangle;
            focusRect.Inflate(-2,-2);
            if(hasFocus)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, focusRect, ForeColor, BackColor);
            }
            else
            {
                using (var pen = new Pen(BackColor, 1))
                    e.Graphics.DrawRectangle(pen, focusRect);
            }

        }
        protected override void OnEnter(EventArgs e)
        {
            this.Invalidate();
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            this.Invalidate();
            base.OnLeave(e);
        }




    }
}
