/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    public class TagButton : UserControl
    {
        private const int VerticalInteriorPadding = 8;
        private readonly int HorizontalInteriorPadding = 14;
        private bool m_IsSelected = false;

        public TagButton()
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        public bool IsSelected
        {
            get { return m_IsSelected; }
            set
            {
                m_IsSelected = value;
                Invalidate();
            }
        }


        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Size sizeText = TextRenderer.MeasureText(this.Text, this.Font, new Size(Int32.MaxValue, Int32.MaxValue),
                                                     TextFormatFlags.NoPrefix);
            this.Size = new Size(sizeText.Width + HorizontalInteriorPadding, sizeText.Height + VerticalInteriorPadding);
            Refresh();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            Size sizeText = TextRenderer.MeasureText(this.Text, this.Font, new Size(Int32.MaxValue, Int32.MaxValue),
                                                     TextFormatFlags.NoPrefix);
            this.Size = new Size(sizeText.Width + HorizontalInteriorPadding, sizeText.Height + VerticalInteriorPadding);
            Refresh();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Cursor = Cursors.Hand;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Graphics graphics = this.CreateGraphics();
            Rectangle rectangle = new Rectangle(ClientRectangle.X + 3, ClientRectangle.Y + 3, ClientRectangle.Width - 6, ClientRectangle.Height - 6);
            ControlPaint.DrawFocusRectangle(graphics, rectangle,this.ForeColor,this.BackColor);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }


        private static GraphicsPath GetRoundedPath(Rectangle rect, float radius)
        {
            float diameter = 2 * radius;

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }



        protected override void OnPaint(PaintEventArgs pevent)
        {
            //Debug.WriteLine("Painting "+this.Text);
            Graphics graphics = pevent.Graphics;

            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle ballRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            using (GraphicsPath outerPath = GetRoundedPath(ballRectangle, 9f))
            {
                //this.Region = new Region(outerPath);

                Color tagColor = IsSelected ?  Color.Gray:Color.WhiteSmoke;
                this.ForeColor = tagColor;
                this.BackColor = Color.Transparent;
                using (SolidBrush outerBrush = new SolidBrush(tagColor))
                {
                    graphics.FillPath(outerBrush, outerPath);
                }

            }

            Rectangle textRectangle = ballRectangle;
            textRectangle.Location = new Point(textRectangle.X + 5, textRectangle.Y);
            Color textColor = IsSelected ? Color.White : Color.Black;
            TextRenderer.DrawText(graphics, Text, Font, textRectangle, textColor, Color.Transparent, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            if (this.Focused)
            {
                Rectangle rectangle = new Rectangle(ClientRectangle.X + 3, ClientRectangle.Y + 3, ClientRectangle.Width - 6, ClientRectangle.Height - 6);
                ControlPaint.DrawFocusRectangle(graphics, rectangle,this.ForeColor,this.BackColor);
            }
        }

    }
}