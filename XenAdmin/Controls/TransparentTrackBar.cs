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

namespace XenAdmin.Controls
{
    public partial class TransparentTrackBar : UserControl
    {
        public TransparentTrackBar()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        public int Max = 8;
        public int Min = 0;
        public int Value = 0;
        public readonly int ThumbWidth = 11;
        public bool Hot = false;
        public bool Pressed = false;

        public new EventHandler Scroll;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Application.RenderWithVisualStyles)
            {
                ButtonRenderer.DrawParentBackground(e.Graphics, e.ClipRectangle, this);
            }
            else
            {
                base.OnPaintBackground(e);
            }

            if (Application.RenderWithVisualStyles)
            {
                TrackBarRenderer.DrawHorizontalTrack(e.Graphics, new Rectangle(e.ClipRectangle.Left + 5, 5 + (e.ClipRectangle.Height - 10 - 4) / 2, e.ClipRectangle.Width - 12, 4));
                TrackBarRenderer.DrawHorizontalTicks(e.Graphics, new Rectangle(e.ClipRectangle.Left + 5, (e.ClipRectangle.Height - 5), e.ClipRectangle.Width - 12, 4), (Max + 1 - Min), EdgeStyle.Bump);
                TrackBarRenderer.DrawBottomPointingThumb(e.Graphics, GetThumbRectangle(), !Enabled ? TrackBarThumbState.Disabled : Pressed ? TrackBarThumbState.Pressed : Hot ? TrackBarThumbState.Hot : TrackBarThumbState.Normal);
            }
            else
            {
                e.Graphics.DrawRectangle(SystemPens.ControlDark, new Rectangle(e.ClipRectangle.Left + 5, 5 + (e.ClipRectangle.Height - 10 - 4) / 2, e.ClipRectangle.Width - 12, 3));
                e.Graphics.DrawLine(SystemPens.ButtonHighlight, e.ClipRectangle.Left + 5, 5 + (e.ClipRectangle.Height - 8) / 2, e.ClipRectangle.Left + 5 + e.ClipRectangle.Width - 12, 5 + (e.ClipRectangle.Height - 8) / 2);
                e.Graphics.DrawLine(SystemPens.ControlDarkDark, e.ClipRectangle.Left + 6, 5 + (e.ClipRectangle.Height - 12) / 2, e.ClipRectangle.Left + 5 + e.ClipRectangle.Width - 13, 5 + (e.ClipRectangle.Height - 12) / 2);
                e.Graphics.DrawLine(SystemPens.ButtonFace, e.ClipRectangle.Left + 6, 5 + (e.ClipRectangle.Height - 14) / 2, e.ClipRectangle.Left + 5 + e.ClipRectangle.Width - 13, 5 + (e.ClipRectangle.Height - 14) / 2);

                Rectangle tickRect = new Rectangle(e.ClipRectangle.Left + 5, (e.ClipRectangle.Height - 5), e.ClipRectangle.Width - 10, 4);
                int tickLeft = tickRect.Left;
                int add = tickRect.Width / (Max - Min);

                using (Pen tickPen = new Pen(SystemColors.ControlText,2.0f))
                {
                    for (int i = 0; i < (Max - Min) + 1; i++)
                    {
                        if(i < (Max - Min))
                            add = (tickRect.Right - tickLeft) / (Max - (Min + i));
                        e.Graphics.DrawLine(tickPen,tickLeft,tickRect.Bottom - 4,tickLeft,tickRect.Bottom);
                        tickLeft += add;
                    }
                }

                Rectangle thumbRect = GetThumbRectangle();
                if (Pressed)
                {
                    e.Graphics.FillPolygon(SystemBrushes.ControlDarkDark, new Point[] { new Point(thumbRect.Left, thumbRect.Top), new Point(thumbRect.Right, thumbRect.Top), new Point(thumbRect.Right, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom), new Point(thumbRect.Left, thumbRect.Bottom - 5) });
                    e.Graphics.FillPolygon(SystemBrushes.ButtonHighlight, new Point[] { new Point(thumbRect.Left, thumbRect.Top), new Point(thumbRect.Right - 1, thumbRect.Top), new Point(thumbRect.Right - 1, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom - 1), new Point(thumbRect.Left - 1 + thumbRect.Width / 2, thumbRect.Bottom - 1), new Point(thumbRect.Left, thumbRect.Bottom - 5) });
                    e.Graphics.FillPolygon(SystemBrushes.ButtonShadow, new Point[] { new Point(thumbRect.Left + 1, thumbRect.Top + 1), new Point(thumbRect.Right - 1, thumbRect.Top + 1), new Point(thumbRect.Right - 1, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom - 1), new Point(thumbRect.Left + 1, thumbRect.Bottom - 5) });
                    e.Graphics.FillPolygon(SystemBrushes.ControlLightLight, new Point[] { new Point(thumbRect.Left + 1, thumbRect.Top + 1), new Point(thumbRect.Right - 2, thumbRect.Top + 1), new Point(thumbRect.Right - 2, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom - 2), new Point(thumbRect.Left - 1 + thumbRect.Width / 2, thumbRect.Bottom - 2), new Point(thumbRect.Left + 1, thumbRect.Bottom - 5) });
                }
                else
                {
                    e.Graphics.FillPolygon(SystemBrushes.ControlDarkDark, new Point[] { new Point(thumbRect.Left, thumbRect.Top), new Point(thumbRect.Right, thumbRect.Top), new Point(thumbRect.Right, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom), new Point(thumbRect.Left, thumbRect.Bottom - 5) });
                    e.Graphics.FillPolygon(SystemBrushes.ButtonHighlight, new Point[] { new Point(thumbRect.Left, thumbRect.Top), new Point(thumbRect.Right - 1, thumbRect.Top), new Point(thumbRect.Right - 1, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom - 1), new Point(thumbRect.Left - 1 + thumbRect.Width / 2, thumbRect.Bottom - 1), new Point(thumbRect.Left, thumbRect.Bottom - 5) });
                    e.Graphics.FillPolygon(SystemBrushes.ButtonShadow, new Point[] { new Point(thumbRect.Left + 1, thumbRect.Top + 1), new Point(thumbRect.Right - 1, thumbRect.Top + 1), new Point(thumbRect.Right - 1, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom - 1), new Point(thumbRect.Left + 1, thumbRect.Bottom - 5) });
                    e.Graphics.FillPolygon(SystemBrushes.ButtonFace, new Point[] { new Point(thumbRect.Left + 1, thumbRect.Top + 1), new Point(thumbRect.Right - 2, thumbRect.Top + 1), new Point(thumbRect.Right - 2, thumbRect.Bottom - 5), new Point(thumbRect.Left + thumbRect.Width / 2, thumbRect.Bottom - 2), new Point(thumbRect.Left - 1 + thumbRect.Width / 2, thumbRect.Bottom - 2), new Point(thumbRect.Left + 1, thumbRect.Bottom - 5) });
                }
            }

        }

        private Rectangle GetThumbRectangle()
        {
            float tickWidth = Convert.ToSingle(ClientSize.Width - 12) / Convert.ToSingle(Max - Min);
            return new Rectangle(Convert.ToInt32(Convert.ToSingle(Value) * tickWidth), 8, ThumbWidth, Height - 22);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None && GetThumbRectangle().Contains(PointToClient(MousePosition)))
            {
                if (!Hot)
                {
                    Hot = true;
                    Invalidate();
                }
            }
            else if (e.Button == MouseButtons.None && Hot)
            {
                Hot = false;
                Invalidate();
            }
            if (e.Button == MouseButtons.Left)
            {
                Pressed = true;
                if (SetNewValue(PointToClient(MousePosition)))
                {
                    Invalidate();
                    if (Scroll != null)
                        Scroll(null, null);
                }
            }
            else
            {
                if (Pressed)
                {
                    Pressed = false;
                    Invalidate();
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SetNewValue(PointToClient(MousePosition)))
                {
                    Pressed = true;
                    Invalidate();
                    if (Scroll != null)
                        Scroll(null, null);
                }
                else if (!Pressed)
                {
                    Pressed = true;
                    Invalidate();
                }
            }
            else
            {
                if (Pressed)
                {
                    Pressed = false;
                    Invalidate();
                }
            }
        }

        private bool SetNewValue(Point point)
        {
            float tickWidth = Convert.ToSingle(ClientSize.Width - 10) / Convert.ToSingle(Max - Min);
            int newVal = Convert.ToInt32(Convert.ToSingle(point.X - 5) / tickWidth);
            if (newVal > Max)
                newVal = Max;
            else if (newVal < Min)
                newVal = Min;

            if(newVal == Value)
                return false;
            Value = newVal;
            Refresh();
            return true;
        }
    }
}
