/* Copyright (c) Citrix Systems Inc. 
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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using XenAdmin.Core;



namespace XenAdmin.Controls
{
    public abstract class CustomHistoryRow : IDisposable
    {
        public static readonly Padding Margin = new Padding(3);

        private static readonly string DescriptionLabel = Messages.HISTORYROW_DETAILS;
        private static readonly string ErrorLabel = Messages.HISTORYROW_ERROR;
        private static readonly string TimeLabel = Messages.HISTORYROW_TIME;
        private static readonly string ProgressLabel = Messages.HISTORYROW_PROGRESS;

        private static readonly Font TitleFont = Program.DefaultFontBold;
        private static readonly Font Font = Program.DefaultFont;
        private static readonly Color TextColor = SystemColors.ControlText;
        private static readonly Color TitleColor = Color.Navy;
        private static readonly Color ErrorColor = Color.Red;
        private static readonly Brush BackBrush = SystemBrushes.Control;
        private static readonly Pen BorderPen = new Pen(SystemColors.ControlDarkDark, 1);
        private static readonly Pen ErrorPenThin = new Pen(ErrorColor, 1);
        private static readonly Pen ErrorPenThick = new Pen(ErrorColor, 3);

        private static readonly Padding ItemPadding = new Padding(3);
        private static readonly Padding InternalPadding = new Padding(3, 3, 3, 10);

        public string Title;
        public string Description;
        public string TimeOccurred;
        public CustomHistoryPanel ParentPanel = null;
        public bool ButtonPressed = false;
        public bool Visible = true;
        public bool ShowCancel = true;

        protected Image Image = null;
        protected abstract string TimeTaken
        {
            get;
        }
        protected int Progress;
        protected bool ShowProgress = true;
        protected bool ShowTime = true;
        public bool Error = false;
        protected bool CancelEnabled = true;
        protected EventHandler<EventArgs> CancelButtonClicked;

        private int RowHeight = 0;
        private int RowWidth = 0;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public int DrawSelf(Graphics g, Rectangle Bounds, int visibleTop, int visibleBottom)
        {
            if (Bounds.Width == RowWidth && !(Bounds.Top < visibleBottom && Bounds.Top + RowHeight > visibleTop))
                return RowHeight;

            int top = Bounds.Top + ItemPadding.Top + InternalPadding.Top;
            int left = Bounds.Left + InternalPadding.Left;

            int rowheight1 = HeightRow1() + ItemPadding.Vertical;
            int rowheight2 = HeightRow2() + ItemPadding.Vertical;
            int rowheight3 = ShowTime ? HeightRow3() + ItemPadding.Vertical : 0;
            int rowheight4 = ShowProgress ? 17 + ItemPadding.Vertical : 0;
            RowHeight = rowheight1 + rowheight2 + rowheight3 + rowheight4 + InternalPadding.Vertical;
            RowWidth = Bounds.Width;

            if (g != null && Bounds.Top < visibleBottom && Bounds.Top + RowHeight > visibleTop && Bounds.Top + RowHeight <= Int16.MaxValue)
            {
                int left1 = left + CustomHistoryPanel.col1;
                int left2 = left1 + ParentPanel.col2;
                int left3 = left2 + ParentPanel.col3;
                int top1 = top + rowheight1;
                int top2 = top1 + rowheight2;
                int top3 = top2 + rowheight3;

                g.FillRectangle(BackBrush, Bounds.Left, Bounds.Top, RowWidth, RowHeight);
                if (!Error)
                {
                    g.DrawRectangle(BorderPen, Bounds.Left, Bounds.Top, RowWidth, RowHeight);
                }
                else
                {
                    g.DrawLine(ErrorPenThin,  Bounds.Left,            Bounds.Top,             Bounds.Left + RowWidth, Bounds.Top);
                    g.DrawLine(ErrorPenThin,  Bounds.Left,            Bounds.Top + RowHeight, Bounds.Left + RowWidth, Bounds.Top + RowHeight);
                    g.DrawLine(ErrorPenThick, Bounds.Left + RowWidth, Bounds.Top,             Bounds.Left + RowWidth, Bounds.Top + RowHeight + 1);
                    g.DrawLine(ErrorPenThick, Bounds.Left,            Bounds.Top,             Bounds.Left,            Bounds.Top + RowHeight + 1);
                }

                g.DrawImage(Image, left + ((CustomHistoryPanel.col1 - Image.Width) / 2), top, Image.Width, Image.Height);
                Drawing.DrawText(g, Title, TitleFont, new Rectangle(left1, top, ParentPanel.col2 + ParentPanel.col3, rowheight1), TitleColor, TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
                Drawing.DrawText(g, TimeOccurred, Font, new Point(left3, top), TextColor);
                if(!Error)
                    Drawing.DrawText(g, DescriptionLabel, Font, new Point(left1, top1), TextColor);
                else
                    Drawing.DrawText(g, ErrorLabel, Font, new Point(left1, top1), ErrorColor);

                Drawing.DrawText(g, Description, Font, new Rectangle(left2, top1, ParentPanel.col3, rowheight2), TextColor, TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

                if (ShowCancel)
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        Rectangle r = new Rectangle(left3, top1, 75, 23);
                        PushButtonState state =
                            CancelEnabled && ButtonPressed ? PushButtonState.Pressed :
                            CancelEnabled                  ? PushButtonState.Normal :
                                                             PushButtonState.Disabled;
                        ButtonRenderer.DrawButton(g, r, Messages.CANCEL, Font, false, state);
                    }
                    else
                    {
                        if (CancelEnabled && !ButtonPressed)
                        {
                            g.FillRectangle(SystemBrushes.ControlText, new Rectangle(left3, top1, 75, 23));
                            g.FillRectangle(SystemBrushes.ButtonHighlight, new Rectangle(left3, top1, 74, 22));
                            g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(left3 + 1, top1 + 1, 73, 21));
                            g.FillRectangle(SystemBrushes.ButtonFace, new Rectangle(left3 + 1, top1 + 1, 72, 20));
                            Drawing.DrawText(g, Messages.CANCEL, Font, new Rectangle(left3 + 1, top1 + 1, 72, 20), SystemColors.ControlText, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                        }
                        else if (CancelEnabled)
                        {
                            g.FillRectangle(SystemBrushes.ControlText, new Rectangle(left3, top1, 75, 23));
                            g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(left3 + 1, top1 + 1, 73, 21));
                            g.FillRectangle(SystemBrushes.ButtonFace, new Rectangle(left3 + 2, top1 + 2, 71, 19));
                            Drawing.DrawText(g, Messages.CANCEL, Font, new Rectangle(left3 + 2, top1 + 2, 71, 19), SystemColors.ControlText, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                        }
                        else
                        {
                            g.FillRectangle(SystemBrushes.ControlText, new Rectangle(left3, top1, 75, 23));
                            g.FillRectangle(SystemBrushes.ButtonHighlight, new Rectangle(left3, top1, 74, 22));
                            g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(left3 + 1, top1 + 1, 73, 21));
                            g.FillRectangle(SystemBrushes.ButtonFace, new Rectangle(left3 + 1, top1 + 1, 72, 20));
                            Drawing.DrawText(g, Messages.CANCEL, Font, new Rectangle(left3 + 1, top1 + 1, 72, 20), SystemColors.GrayText, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                        }
                    }
                }

                if (ShowTime)
                {
                    Drawing.DrawText(g, TimeLabel, Font, new Point(left1, top2), TextColor);
                    Drawing.DrawText(g, TimeTaken, Font, new Rectangle(left2, top2, ParentPanel.col3, rowheight3), TextColor, TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
                }

                if (ShowProgress)
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        Drawing.DrawText(g, ProgressLabel, Font, new Point(left1, top3), TextColor);
                        ProgressBarRenderer.DrawHorizontalBar(g, new Rectangle(left2, top3, ParentPanel.col3, 17));
                        ProgressBarRenderer.DrawHorizontalChunks(g, new Rectangle(left2 + 4, top3 + 3, (Progress * (ParentPanel.col3 - 8) / 100), 12));
                    }
                    else
                    {
                        Drawing.DrawText(g, ProgressLabel, Font, new Point(left1, top3), TextColor);
                        g.FillRectangle(SystemBrushes.ButtonShadow, new Rectangle(left2, top3, ParentPanel.col3, 17));
                        g.FillRectangle(SystemBrushes.ButtonHighlight, new Rectangle(left2 + 1, top3 + 1, ParentPanel.col3 - 1, 16));
                        g.FillRectangle(SystemBrushes.ButtonFace, new Rectangle(left2 + 1, top3 + 1, ParentPanel.col3 - 2, 15));
                        int barwidth = (Progress * (ParentPanel.col3 - 4) / 100);
                        int chunkwidth = 7;
                        int chunkgap = 2;
                        int progleft = 0;
                        while (true)
                        {
                            if (progleft + chunkwidth + chunkgap < barwidth)
                            {
                                g.FillRectangle(SystemBrushes.ActiveCaption, new Rectangle(left2 + 2 + progleft, top3 + 2, chunkwidth, 13));
                                progleft += chunkwidth + chunkgap;
                            }
                            else
                                break;
                        }
                        g.FillRectangle(SystemBrushes.ActiveCaption, new Rectangle(left2 + 2 + progleft, top3 + 2, chunkwidth - progleft, 13));
                    }
                    /*int barwidth = (Progress * (ParentPanel.col3 - 8) / 100);
                    int chunkwidth = 7;
                    int chunkgap = 1;
                    int progleft = 0;
                    while (true)
                    {
                        if (progleft + chunkwidth + chunkgap < barwidth)
                        {
                            ChunkRenderer.DrawBackground(g, new Rectangle(left2 + 4 + progleft, t + rowheight2 + rowheight3 + 3, chunkwidth, 12));
                            progleft += chunkwidth + chunkgap;
                        }
                        else
                            break;
                    }
                    ChunkRenderer.DrawBackground(g, new Rectangle(left2 + 4 + progleft, t + rowheight2 + rowheight3 + 3, chunkwidth - progleft, 12));*/
                }
            }

            return RowHeight;
        }

        private int HeightOf(string text, Font font, int width)
        {
            int h = Drawing.MeasureText(text, font, new Size(width, Int32.MaxValue), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak).Height;
            return h > 0 ? h : 12;
        }

        private int HeightRow3()
        {
            return HeightOf(TimeTaken, Font, ParentPanel.col3);
        }

        private int HeightRow2()
        {
            return HeightOf(Description, Font, ParentPanel.col3);
        }

        private int HeightRow1()
        {
            return HeightOf(Title, TitleFont, ParentPanel.col2 + ParentPanel.col3);
        }

        internal void Click(Point p, MouseButtons button)
        {
            if (button == MouseButtons.Left && ShowCancel && CancelEnabled)
            {
                Rectangle rect = CancelButtonRect();
                if (rect.Contains(p))
                {
                    ButtonPressed = true;
                    ParentPanel.Refresh();
                }
            }
        }

        public void MouseUp(Point p, MouseButtons button)
        {
            if (button == MouseButtons.Left &&
                ShowCancel && CancelEnabled && ButtonPressed && CancelButtonClicked != null)
            {
                Rectangle rect = CancelButtonRect();
                if (rect.Contains(p))
                {
                    CancelButtonClicked(null, null);
                    CancelEnabled = false;
                }
            }
            ButtonPressed = false;
        }

        private Rectangle CancelButtonRect()
        {
            return new Rectangle(Margin.Left + InternalPadding.Left + CustomHistoryPanel.col1 + ParentPanel.col2 + ParentPanel.col3,
                                 Margin.Left + InternalPadding.Top + ItemPadding.Top + ItemPadding.Vertical + HeightRow1(), 75, 23);
        }
    }
}
