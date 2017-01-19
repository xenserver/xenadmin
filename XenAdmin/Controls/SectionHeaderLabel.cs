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
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    public partial class SectionHeaderLabel : UserControl
    {
        /// <summary>
        /// Vertical Alignment enumeration.  This doesn;t appear in .NET until 4.0
        /// </summary>
        public enum VerticalAlignment
        {
            /// <summary>
            /// Align to the Top of the control
            /// </summary>
            Top,
            /// <summary>
            /// Align to the vertical center of the control
            /// </summary>
            Middle,
            /// <summary>
            /// Align to the Bottom of the control
            /// </summary>
            Bottom
        }

        /// <summary>
        /// Horizontal alignment of the control text
        /// </summary>
        [Localizable(true)]
        public HorizontalAlignment LabelHorizontalAlignment { get; set; }

        /// <summary>
        /// Text to display
        /// </summary>
        [Localizable(true)]
        public string LabelText { get; set; }

        /// <summary>
        /// Padding around the text
        /// </summary>
        [Localizable(true)]
        public Padding LabelPadding { get; set; }

        /// <summary>
        /// Vertical alignment of the header line
        /// </summary>
        public VerticalAlignment LineLocation { get; set; }

        /// <summary>
        /// Color of the header line
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// Padding around the header line
        /// </summary>
        [Localizable(true)]
        public Padding LinePadding { get; set; }

        /// <summary>
        /// Use mnemonic for setting focus on a control
        /// </summary>
        [Localizable(true)]
        public bool UseMnemonic { get; set; }

        /// <summary>
        /// The control on which to set focus when mnemonic key is pressed
        /// </summary>
        [Localizable(true)]
        public Control FocusControl { get; set; }

        /// <summary>
        /// Public constructor
        /// </summary>
        public SectionHeaderLabel()
        {
            InitializeComponent();

            this.LineColor = Color.Black;
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// Custom OnPoint method
        /// </summary>
        /// <param name="e">PaintEventArgs object</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawLine(e.Graphics);
            DrawLabel(e.Graphics);
            Size baseHeight = new Size(0, this.Padding.Vertical +
                                      (this.LinePadding.Vertical + 1) +
                                      this.LabelPadding.Vertical +
                                      (int)e.Graphics.MeasureString(this.LabelText, this.Font).Height);
            this.MinimumSize = baseHeight;
            base.OnPaint(e);
        }

        /// <summary>
        /// Handle mnemonic events to set focus to the FocusControl
        /// </summary>
        /// <param name="charCode"></param>
        /// <returns></returns>
        protected override bool ProcessMnemonic(char charCode)
        {
            if (this.UseMnemonic && Control.IsMnemonic(charCode, this.LabelText) && (Control.ModifierKeys == Keys.Alt) && null != this.FocusControl)
            {
                this.FocusControl.Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Draws the text label of the control
        /// </summary>
        /// <param name="graphics">Current graphics drawing object</param>
        private void DrawLabel(Graphics graphics)
        {
            StringFormat sf = new StringFormat();
            if (this.UseMnemonic)
            {
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                if (!this.ShowKeyboardCues)
                    sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
            }
            else
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
            sf.FormatFlags = StringFormatFlags.NoWrap;

            //Make sure our brush is disposed
            using (SolidBrush brush = new SolidBrush(this.ForeColor))
            {
                PointF pointF = GetTextPointF(graphics);
                //Draw the actual text
                graphics.DrawString(this.LabelText, this.Font, brush, pointF, sf);
            }
        }

        /// <summary>
        /// Draw the header line
        /// </summary>
        /// <param name="graphics">Current graphics drawing object</param>
        private void DrawLine(Graphics graphics)
        {
            //These are used a lot in here...
            SizeF textSizeF = graphics.MeasureString(this.LabelText, this.Font);
            float lineY = GetLineVerticalLocation();

            //Make sure our pen is disposed
            using (Pen pen = new Pen(this.LineColor))
            {
                if (LineLocation == VerticalAlignment.Middle)
                {
                    switch (LabelHorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            {
                                //Draw a line from the right end of the text to the right end of the control
                                DrawLeftLine(graphics, textSizeF, lineY, pen);
                                break;
                            }
                        case HorizontalAlignment.Right:
                            {
                                //Draw a line from the left end of the control to the left end of the text
                                DrawRightLine(graphics, textSizeF, lineY, pen);
                                break;
                            }
                        case HorizontalAlignment.Center:
                            {
                                //Draw a line from the left of the control, to the left of the label, 
                                // then from the right of the label to the right of the control
                                DrawLeftLine(graphics, textSizeF, lineY, pen);
                                DrawRightLine(graphics, textSizeF, lineY, pen);
                                break;
                            }
                    }
                }

                else
                {
                    float startX = (float)this.Padding.Left +           //Control left padding
                                  (float)LinePadding.Left;             //Line left padding
                    float endX = (float)this.Width -                    //Control width
                                 (float)this.Padding.Right -            //Control right padding
                                 (float)this.LinePadding.Right;         //Line right padding
                    graphics.DrawLine(pen, startX, lineY, endX, lineY);
                }
            }
        }

        private void DrawRightLine(Graphics graphics, SizeF textSizeF, float lineY, Pen pen)
        {
            float startX = (float)this.Padding.Left +           //Control left padding
                           (float)LinePadding.Left;             //Line left padding
            float endX = (float)this.Width -                    //Control width
                         (float)this.Padding.Right -            //Control right padding
                         (float)this.LinePadding.Right -        //Line right padding
                         (float)this.LabelPadding.Horizontal -  //Label horizontal padding
                         textSizeF.Width;                       //Width of the label text
            graphics.DrawLine(pen, startX, lineY, endX, lineY);
        }

        private void DrawLeftLine(Graphics graphics, SizeF textSizeF, float lineY, Pen pen)
        {
            float startX = (float)this.Padding.Left +           //Control left padding
                           (float)LabelPadding.Horizontal +     //Label left+right padding
                           (float)LinePadding.Left +            //Line left padding
                           textSizeF.Width;                     //Width of the label text
            float endX = (float)this.Width -                    //Control width
                         (float)this.Padding.Right -            //Control right padding
                         (float)this.LinePadding.Right;         //Line right padding
            graphics.DrawLine(pen, startX, lineY, endX, lineY);
        }

        /// <summary>
        /// Determines the left point of the line
        /// </summary>
        /// <param name="graphics">Current graphics drawing object</param>
        /// <returns>Returns the left PointF</returns>
        private PointF GetLeftLinePoint(Graphics graphics)
        {
            PointF pointF = new PointF();

            pointF.X = this.Padding.Left + this.LinePadding.Left;

            pointF.Y = GetLineVerticalLocation();

            return pointF;
        }

        /// <summary>
        /// Determines the right point of the line
        /// </summary>
        /// <param name="graphics">Current graphics drawing object</param>
        /// <returns>Returns the right PointF</returns>
        private PointF GetRightLinePoint(Graphics graphics)
        {
            PointF pointF = new PointF();

            pointF.X = this.Width - this.Padding.Right - this.LinePadding.Right;

            pointF.Y = GetLineVerticalLocation();

            return pointF;
        }

        /// <summary>
        /// Determines the vertical location of the line
        /// </summary>
        /// <returns>the float location for the line Y</returns>
        private float GetLineVerticalLocation()
        {
            float Y = 0f;
            switch (this.LineLocation)
            {
                case VerticalAlignment.Top:
                    {
                        Y = this.Padding.Top + this.LinePadding.Top + 1;
                        break;
                    }
                case VerticalAlignment.Middle:
                    {
                        Y = (this.Height - this.Padding.Vertical) / 2;
                        break;
                    }
                case VerticalAlignment.Bottom:
                    {
                        Y = (this.Height - this.Padding.Bottom - this.LinePadding.Bottom - 1);
                        break;
                    }
            }
            return Y;
        }

        /// <summary>
        /// Determines the point location for the label text
        /// </summary>
        /// <param name="graphics">Current graphics drawing object</param>
        /// <returns>The PointF location of the label text</returns>
        private PointF GetTextPointF(Graphics graphics)
        {
            SizeF textSize = graphics.MeasureString(this.LabelText, this.Font);
            PointF pointF = new PointF();

            pointF.Y = this.Padding.Top + this.LabelPadding.Top + 1;
            //If both the line and the label are top aligned,
            // bump the text down, including the line's padding
            if (LineLocation == VerticalAlignment.Top)
            {
                pointF.Y += this.LinePadding.Vertical + 1;
            }

            switch (this.LabelHorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        pointF.X = this.Padding.Left + this.LabelPadding.Left;
                        break;
                    }
                case HorizontalAlignment.Center:
                    {
                        pointF.X = (this.Width - this.Padding.Horizontal) / 2 - (int)textSize.Width / 2;
                        break;
                    }
                case HorizontalAlignment.Right:
                    {
                        pointF.X = (this.Width - this.Padding.Right - this.LabelPadding.Right) - (int)textSize.Width;
                        break;
                    }
            }

            return pointF;
        }
    }
}

