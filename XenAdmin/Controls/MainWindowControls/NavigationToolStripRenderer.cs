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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;

namespace XenAdmin.Controls.MainWindowControls
{
    class NavigationToolStripRenderer : ToolStripProfessionalRenderer
    {
        public NavigationToolStripRenderer()
            : base(new NavigationColourTable())
        {
            RoundedEdges = false;
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            var bounds = new Rectangle(Point.Empty, e.ToolStrip.Size);
            
            using (Pen pen = new Pen(NavigationColourTable.BACK_COLOR))
                e.Graphics.DrawRectangle(pen, bounds);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderButtonBackground(e);

            var button = (NavigationButton)e.Item;
            if (button == null)
                return;

            var gradTop = ColorTable.ToolStripPanelGradientBegin;
            var gradBottom = ColorTable.ToolStripPanelGradientBegin;

            if (button.Pressed || button.Checked)
            {
                gradTop = NavigationColourTable.CHECKED_GRADIENT_BEGIN;
                gradBottom = NavigationColourTable.CHECKED_GRADIENT_END;
            }
            else if (button.Selected)//hover
            {
                gradTop = NavigationColourTable.HOVER_GRADIENT_BEGIN;
                gradBottom = NavigationColourTable.HOVER_GRADIENT_END;
            }

            var bounds = new Rectangle(Point.Empty, e.Item.Size);
            var g = e.Graphics;

            DrawItemBackGround(g, bounds, gradTop, gradBottom);

            if (button.Pressed || button.Checked || button.Selected)
                DrawItemBorder(g, bounds);
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderDropDownButtonBackground(e);

            var dropdown = e.Item as NavigationDropDownButton;
            if (dropdown == null)
                return;

            var gradTop = ColorTable.ToolStripPanelGradientBegin;
            var gradBottom = ColorTable.ToolStripPanelGradientBegin;

            var dropDownItems = dropdown.DropDownItems;
            if (dropDownItems.Count == 0)
            {
                var pairedItem = dropdown.PairedItem as NavigationDropDownButton;
                if (pairedItem != null)
                    dropDownItems = pairedItem.DropDownItems;
            }

            bool itemChecked = false;
            foreach (ToolStripMenuItem menuItem in dropDownItems)
            {
                if (menuItem.Checked)
                {
                    itemChecked = true;
                    break;
                }
            }

            if (itemChecked)
            {
                gradTop = NavigationColourTable.CHECKED_GRADIENT_BEGIN;
                gradBottom = NavigationColourTable.CHECKED_GRADIENT_END;
            }
            else if (dropdown.Pressed || dropdown.Selected)//mouse down or hover
            {
                gradTop = NavigationColourTable.HOVER_GRADIENT_BEGIN;
                gradBottom = NavigationColourTable.HOVER_GRADIENT_END;
            }

            var bounds = new Rectangle(Point.Empty, e.Item.Size);
            var g = e.Graphics;

            DrawItemBackGround(g, bounds, gradTop, gradBottom);

            if (itemChecked || dropdown.Pressed || dropdown.Selected)
                DrawItemBorder(g, bounds);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            base.OnRenderItemText(e);

            var notifyButton = e.Item as NotificationButtonBig;
            if (notifyButton == null || notifyButton.UnreadEntries == 0)
                return;

            using (Font blobFont = new Font(e.TextFont, FontStyle.Bold))
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var blobText = notifyButton.UnreadEntries.ToString();
                var textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

                var contRect = e.Item.ContentRectangle;
                var blobSize = Drawing.MeasureText(g, blobText, blobFont, contRect.Size, textFlags);

                int horizPadding = 5;
                int vertPadding = 1;
                int rightMargin = 2;

                var blobRect = new Rectangle(e.TextRectangle.Right + rightMargin + horizPadding + 1,
                                     contRect.Top + ((contRect.Height - blobSize.Height) / 2),
                                     blobSize.Width,
                                     blobSize.Height);

                var redRect = new Rectangle(blobRect.Location, blobRect.Size);
                var whiteRect = new Rectangle(blobRect.Location, blobRect.Size);
                redRect.Inflate(horizPadding, vertPadding);
                whiteRect.Inflate(horizPadding + 1, vertPadding + 1);

                using (GraphicsPath path = new GraphicsPath())
                {
                    int diameter = redRect.Height;
                    var arc = new Rectangle(redRect.Location, new Size(diameter, diameter));

                    //top left corner
                    path.AddArc(arc, 180, 90);

                    // top right corner
                    arc.X = redRect.Right - diameter;
                    path.AddArc(arc, 270, 90);

                    // bottom right corner 
                    arc.Y = redRect.Bottom - diameter;
                    path.AddArc(arc, 0, 90);

                    // bottom left corner
                    arc.X = redRect.Left;
                    path.AddArc(arc, 90, 90);
 
                    path.CloseFigure();

                    using (var brush = new SolidBrush(NavigationColourTable.NOTIFICATION_BACKCOLOR))
                        g.FillPath(brush, path);
                }

                using (GraphicsPath path = new GraphicsPath())
                {
                    int diameter = whiteRect.Height;
                    var arc = new Rectangle(whiteRect.Location, new Size(diameter, diameter));

                    //top left corner
                    path.AddArc(arc, 180, 90);

                    // top right corner
                    arc.X = whiteRect.Right - diameter;
                    path.AddArc(arc, 270, 90);

                    // bottom right corner 
                    arc.Y = whiteRect.Bottom - diameter;
                    path.AddArc(arc, 0, 90);

                    // bottom left corner
                    arc.X = whiteRect.Left;
                    path.AddArc(arc, 90, 90);

                    path.CloseFigure();

                    using (var pen = new Pen(NavigationColourTable.NOTIFICATION_FORECOLOR, 1))
                        g.DrawPath(pen, path);
                }

                Drawing.DrawText(g, blobText, blobFont, blobRect, NavigationColourTable.NOTIFICATION_FORECOLOR, textFlags);
            }
        }

        private void DrawItemBackGround(Graphics g, Rectangle bounds, Color gradTop, Color gradBottom)
        {
            using (var brush = new LinearGradientBrush(bounds, Color.Black, Color.Black, LinearGradientMode.Vertical))
            {
                ColorBlend blend = new ColorBlend
                    {
                        Positions = new[] { 0, 1 / 2f, 1 },
                        Colors = new[] { gradTop, gradBottom, gradTop }
                    };

                brush.InterpolationColors = blend;
                g.FillRectangle(brush, bounds);
            }
        }

        private void DrawItemBorder(Graphics g, Rectangle bounds)
        {
            using (Pen pen = new Pen(NavigationColourTable.ITEM_BORDER_COLOR))
                g.DrawRectangle(pen, bounds.X + 1, bounds.Y, bounds.Width - 2, bounds.Height - 1);
        }
    }

    class SmallNavigationToolStripRenderer : NavigationToolStripRenderer
    {
        
    }

    class BigNavigationToolStripRenderer : NavigationToolStripRenderer
    {
        
    }

    class NavigationColourTable : ProfessionalColorTable
    {
        internal static readonly Color CHECKED_GRADIENT_BEGIN = Color.Silver;
        internal static readonly Color CHECKED_GRADIENT_END = Color.WhiteSmoke;
        internal static readonly Color HOVER_GRADIENT_BEGIN = Color.WhiteSmoke;
        internal static readonly Color HOVER_GRADIENT_END = Color.White;
        internal static readonly Color BACK_COLOR = SystemColors.Control;
        internal static readonly Color ITEM_BORDER_COLOR = Color.SlateGray;
        internal static readonly Color NOTIFICATION_BACKCOLOR = Color.FromArgb(204, 0, 0);
        internal static readonly Color NOTIFICATION_FORECOLOR = Color.White;

        public override Color ToolStripGradientBegin
        {
            get { return BACK_COLOR; }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return BACK_COLOR; }
        }

        public override Color ToolStripGradientEnd
        {
            get { return BACK_COLOR; }
        }
    }
}
