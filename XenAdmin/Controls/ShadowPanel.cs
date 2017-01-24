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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using XenAdmin.Properties;

namespace XenAdmin.Controls
{
    public class ShadowPanel : Panel
    {
        private Color _panelColor = Color.Transparent;

        public Color PanelColor
        {
            get { return _panelColor; }
            set { _panelColor = value; }
        }

        private Color _borderColor = Color.Transparent;

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        private int shadowSize = 5;
        private int shadowMargin = 2;

        // static for good perfomance 
        private static Image shadowDownRight = Resources.tshadowdownright;
        static Image shadowDownLeft = Resources.tshadowdownleft;
        static Image shadowDown = Resources.tshadowdown;
        static Image shadowRight = Resources.tshadowright;
        static Image shadowTopRight = Resources.tshadowtopright;

        public ShadowPanel()
        {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Get the graphics object. We need something to draw with ;-)
            Graphics g = e.Graphics;

            // Create tiled brushes for the shadow on the right and at the bottom.
            TextureBrush shadowRightBrush = new TextureBrush(shadowRight, WrapMode.Tile);
            TextureBrush shadowDownBrush = new TextureBrush(shadowDown, WrapMode.Tile);

            // Translate (move) the brushes so the top or left of the image matches the top or left of the
            // area where it's drawed. If you don't understand why this is necessary, comment it out. 
            // Hint: The tiling would start at 0,0 of the control, so the shadows will be offset a little.
            shadowDownBrush.TranslateTransform(0, Height - shadowSize);
            shadowRightBrush.TranslateTransform(Width - shadowSize, 0);

            // Define the rectangles that will be filled with the brush.
            // (where the shadow is drawn)
            Rectangle shadowDownRectangle = new Rectangle(
                shadowSize + shadowMargin,                      // X
                Height - shadowSize,                            // Y
                Width - (shadowSize * 2 + shadowMargin),        // width (stretches)
                shadowSize                                      // height
                );

            Rectangle shadowRightRectangle = new Rectangle(
                Width - shadowSize,                             // X
                shadowSize + shadowMargin,                      // Y
                shadowSize,                                     // width
                Height - (shadowSize * 2 + shadowMargin)        // height (stretches)
                );

            // And draw the shadow on the right and at the bottom.
            g.FillRectangle(shadowDownBrush, shadowDownRectangle);
            g.FillRectangle(shadowRightBrush, shadowRightRectangle);

            // Now for the corners, draw the 3 5x5 pixel images.
            g.DrawImage(shadowTopRight, new Rectangle(Width - shadowSize, shadowMargin, shadowSize, shadowSize));
            g.DrawImage(shadowDownRight, new Rectangle(Width - shadowSize, Height - shadowSize, shadowSize, shadowSize));
            g.DrawImage(shadowDownLeft, new Rectangle(shadowMargin, Height - shadowSize, shadowSize, shadowSize));

            // Fill the area inside with the color in the PanelColor property.
            // 1 pixel is added to everything to make the rectangle smaller. 
            // This is because the 1 pixel border is actually drawn outside the rectangle.
            Rectangle fullRectangle = new Rectangle(
                1,                                              // X
                1,                                              // Y
                Width - (shadowSize + 2),                       // Width
                Height - (shadowSize + 2)                       // Height
                );

            if (PanelColor != Color.Transparent)
            {
                using (SolidBrush bgBrush = new SolidBrush(_panelColor))
                    g.FillRectangle(bgBrush, fullRectangle);
            }

            // Draw a nice 1 pixel border it a BorderColor is specified
            if (_borderColor != Color.Transparent)
            {
                using (Pen borderPen = new Pen(BorderColor))
                    g.DrawRectangle(borderPen, fullRectangle);
            }

            // Memory efficiency
            shadowDownBrush.Dispose();
            shadowRightBrush.Dispose();

            shadowDownBrush = null;
            shadowRightBrush = null;
        }

        // Correct resizing
        protected override void OnResize(EventArgs e)
        {
            base.Invalidate();
            base.OnResize(e);
        }
    }
}