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
using XenAdmin.Properties;


namespace XenAdmin.Controls
{
    public partial class SearchTextBox : UserControl
    {
    	/// <summary>
    	/// If true the TextChanged event should not be fired. This is the case when the text
		/// changes from Messages.SEARCH_TEXT_BOX_INITIAL_TEXT to string.Empty and vice-versa.
    	/// </summary>
		private bool m_isTextChangeSilent;

        public SearchTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            InitializeComponent();
            
            textBox1.Text = Messages.SEARCH_TEXT_BOX_INITIAL_TEXT;
            textBox1.LostFocus += textBox1_LostFocus;
            textBox1.GotFocus += textBox1_GotFocus;
            SetTextGrey(true);
        }

        public void Reset()
        {
            textBox1_LostFocus(this, EventArgs.Empty);
        }

        private void SetTextGrey(bool grey)
        {
            if (grey)
            {
                textBox1.ForeColor = Color.Gray;
                textBox1.Font = new Font(textBox1.Font, FontStyle.Italic);
            }
            else
            {
                textBox1.ForeColor = SystemColors.WindowText;
                textBox1.Font = new Font(textBox1.Font, FontStyle.Regular);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            var borderRect = new Rectangle(0, 0, Width, Height);

            if (Application.RenderWithVisualStyles)
            {
                TextBoxRenderer.DrawTextBox(g, borderRect, TextBoxState.Selected);
            }
            else
            {
                ControlPaint.DrawBorder3D(g, borderRect);
                g.FillRectangle(Brushes.White, new Rectangle(2, 2, Width - 4, Height - 4));
            }

            g.TextRenderingHint = Drawing.TextRenderingHint;

            // Draw magnifying glass or cross icon
            Image image = textBox1.Text.Length > 0 && textBox1.Text != Messages.SEARCH_TEXT_BOX_INITIAL_TEXT ? Resources.cross : Resources._000_Search_h32bit_16;

            g.DrawImage(image, new Rectangle(textBox1.Width + textBox1.Left, Height / 2 - image.Height / 2, image.Width, image.Height));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            const int leftMargin = 4;
            const int iconWidth = 20;
            textBox1.Location = new Point(leftMargin, Height / 2 - textBox1.Height / 2);
            textBox1.Size = new Size(Width - leftMargin - iconWidth, textBox1.Height);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Invalidate();
			if (!m_isTextChangeSilent)
				OnTextChanged(EventArgs.Empty);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Refresh();
        }

        private void textBox1_LostFocus(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
				m_isTextChangeSilent = true;
                textBox1.Text = Messages.SEARCH_TEXT_BOX_INITIAL_TEXT;
				m_isTextChangeSilent = false;
                SetTextGrey(true);
            }

            Refresh();
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == Messages.SEARCH_TEXT_BOX_INITIAL_TEXT)
            {
            	m_isTextChangeSilent = true;
                textBox1.Text = string.Empty;
            	m_isTextChangeSilent = false;
                SetTextGrey(false);
            }
            Refresh();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override string Text
        {
            get
            {
                return textBox1.Text != Messages.SEARCH_TEXT_BOX_INITIAL_TEXT ? textBox1.Text : string.Empty;
            }
            set
            {
                if (value == "" && !this.Focused)
                {
                    textBox1.Text = Messages.SEARCH_TEXT_BOX_INITIAL_TEXT;
                    SetTextGrey(true);
                }
                else
                {
                    textBox1.Text = value;
                    SetTextGrey(false);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Matches(string name)
        {
            return name.IndexOf(this.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            var handler = this.TextChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.X > textBox1.Left + textBox1.Width)
            {
                if (textBox1.Text.Length > 0)
                {
                    textBox1.Text = string.Empty;
                }
                else
                {
                    textBox1.Focus();
                }
                Refresh();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Cursor = e.X > textBox1.Left + textBox1.Width ? Cursors.Hand : Cursors.IBeam;
        }

        [Browsable(true)]
        public new event EventHandler TextChanged;

        private bool wasFocused;
        private int cursorLoc;

        public void SaveState()
        {
            wasFocused = textBox1.ContainsFocus;
            cursorLoc = textBox1.SelectionStart;
        }

        public void RestoreState()
        {
            if (!wasFocused || ContainsFocus)
                return;

            textBox1.Select();
            textBox1.Select(cursorLoc, 0);
        }
    }
}
