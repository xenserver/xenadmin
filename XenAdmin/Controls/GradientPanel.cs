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


using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace XenAdmin.Controls.GradientPanel
{
    public abstract class GradientPanel : Panel
    {
        protected GradientPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected abstract LinearGradientMode GradientMode { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected abstract Color GradientStartColor { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected abstract Color GradientEndColor { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected abstract Color BorderColor { get; }

        #endregion

        protected void PaintPanel(Graphics g, bool drawBorder = false)
        {
            if (Size.Width == 0 || Size.Height == 0)
                return;

            Rectangle bounds = new Rectangle(Point.Empty, Size);

            if (!Application.RenderWithVisualStyles)
            {
                g.FillRectangle(SystemBrushes.Control, bounds);
            }
            else
            {
                using (Brush b = new LinearGradientBrush(bounds, GradientStartColor, GradientEndColor, GradientMode))
                {
                    ButtonRenderer.DrawParentBackground(g, bounds, this);
                    g.FillRectangle(b, bounds);
                }
            }

            if (drawBorder)
            {
                using (var p = new Pen(BorderColor))
                    g.DrawRectangle(p, new Rectangle(bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            PaintPanel(e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing
        }
    }

    public class HorizontalGradientPanel : GradientPanel
    {
        public static Color TextColor =>
            Application.RenderWithVisualStyles ? Color.White : SystemColors.ControlText;

        protected override LinearGradientMode GradientMode => LinearGradientMode.Horizontal;
        protected override Color GradientStartColor => Color.FromArgb(57, 109, 140);
        protected override Color GradientEndColor => Color.FromArgb(63, 139, 137);
        protected override Color BorderColor => ProfessionalColors.OverflowButtonGradientEnd;
    }

    public class VerticalGradientPanel : GradientPanel
    {
        public static Color TextColor =>
            Application.RenderWithVisualStyles ? Color.FromArgb(60, 60, 60) : SystemColors.ControlText;

        protected override LinearGradientMode GradientMode => LinearGradientMode.Vertical;
        protected override Color GradientStartColor => ProfessionalColors.OverflowButtonGradientBegin;
        protected override Color GradientEndColor => ProfessionalColors.OverflowButtonGradientEnd;
        protected override Color BorderColor => ProfessionalColors.OverflowButtonGradientEnd;

        protected override void OnPaint(PaintEventArgs e)
        {
            PaintPanel(e.Graphics, true);
        }
    }
}
