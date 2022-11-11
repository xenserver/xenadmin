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
using System.Windows.Forms;

using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Model;


namespace XenAdmin.Controls
{
    public class FolderListItem
    {
        public event Action PathChanged;

        private const int INNER_PADDING = 9;
        private const int RIGHT_PADDING = 20;
        private const int IMAGE_OFFSET = 4;

        public enum AllowSearch { None, AllButLast, All };
        private AllowSearch allowSearch;

        private List<FLIControl> controls;
        public Control Parent;
        private bool hasChangeButton;
        private LinkLabel changeLabel;
        private Padding itemBorder;

        /// <summary>
        /// Color.Transparent on most platforms; SystemColors.Window when ClearType is enabled.
        /// This is to work around an issue in TextRenderer.DrawText which causes text written to a double-buffer
        /// using ClearType to anti-alias onto black rather than onto the background colour.  In this case,
        /// we use Window, because by luck those labels are always on top of that colour on Vista and XP.
        /// We indicate that we're writing to a buffer (rather than the screen) by setting Graphics.TextContrast
        /// to 5 (the default is 4). This hack was needed because there's no easy way to add info to
        /// a Graphics object. (CA-22938).
        /// </summary>
        private static readonly Color TransparentUsually;

        static FolderListItem()
        {
            try
            {
                if (SystemInformation.FontSmoothingType == 2) // ClearType
                    TransparentUsually = SystemColors.Window;
            }
            catch (NotSupportedException)
            {
                // This is an old platform without FontSmoothingType support.
                TransparentUsually = Color.Transparent;
            }

        }

        public FolderListItem(string path, AllowSearch allowSearch, bool changeButton)
        {
            Path = path;
            this.controls = new List<FLIControl>();
            this.allowSearch = allowSearch;
            this.hasChangeButton = changeButton;
            this.itemBorder = new Padding(0);
        }

        public string Path { get; private set; }

        public Color ForeColor { get; set; } = SystemColors.ControlText;

        private Font Font => Program.DefaultFont;

        public int MaxWidth { get; set; }

        public void DrawSelf(Graphics g, Rectangle bounds, bool selected)
        {
            controls = new List<FLIControl>();
            Size bigSize = new Size(int.MaxValue, int.MaxValue);

            Point p = new Point(bounds.Left + itemBorder.Left, bounds.Top + itemBorder.Top);
            Size sz;

            var backColor = g.TextContrast == 5 ? TransparentUsually : Color.Transparent;

            if (Path == "")
            {
                Drawing.DrawText(g, Messages.NONE, Font, p, ForeColor, backColor);
                sz = Drawing.MeasureText(Messages.NONE, Font);
                p.X += sz.Width;
            }
            else
            {
                p.X += 3;  // this is necessary to compensate for TextFormatFlags.NoPadding below: all the other rows have padding
                String[] pathParts = Folders.PointToPath(Path);
                CalcSizeAndTrunc(g, bounds.Width - itemBorder.Horizontal, out var trunc1, out var trunc2);
                if (trunc1 > 0)
                {
                    Drawing.DrawText(g, Messages.ELLIPSIS, Font, p, ForeColor, backColor, TextFormatFlags.NoPadding);
                    sz = Drawing.MeasureText(g, Messages.ELLIPSIS, Font, bigSize, TextFormatFlags.NoPadding);
                    p.X += sz.Width + INNER_PADDING;
                }
                for (int i = trunc1; i < pathParts.Length; ++i)
                {
                    if (i > 0)
                    {
                        g.DrawImage(Images.StaticImages.folder_separator, p + new Size(0, IMAGE_OFFSET));
                        p.X += Images.StaticImages.folder_separator.Width + INNER_PADDING;
                    }

                    string s = pathParts[i];
                    if (i == pathParts.Length - 1 && trunc2 > 0)  // need to truncate final component
                        s = s.Ellipsise(trunc2);

                    bool doSearch = (allowSearch == AllowSearch.All ||
                        (allowSearch == AllowSearch.AllButLast && i != pathParts.Length - 1));

                    Drawing.DrawText(g, s, Font, p, doSearch ? Color.Blue : ForeColor, backColor, TextFormatFlags.NoPadding);
                    sz = Drawing.MeasureText(g, s, Font, bigSize, TextFormatFlags.NoPadding);
                    if (doSearch)
                        controls.Add(new FLIControl(Folders.PathToPoint(pathParts, i + 1),
                            p.X - bounds.Left, p.Y - bounds.Top, sz.Width, sz.Height));
                    p.X += sz.Width + INNER_PADDING;
                }
            }

            if (hasChangeButton)
            {
                p.X += RIGHT_PADDING;
                if (changeLabel != null)
                {
                    changeLabel.LinkClicked -= changeLabel_LinkClicked;
                    changeLabel.Dispose();
                }
                changeLabel = new LinkLabel();
                changeLabel.Text = Messages.CHANGE;
                changeLabel.TabStop = true;
                changeLabel.LinkClicked += changeLabel_LinkClicked;
                changeLabel.Parent = this.Parent;
                changeLabel.Left = p.X - bounds.Left;
                changeLabel.Top = p.Y - bounds.Top;
            }
        }

        public Size PreferredSize
        {
            get
            {
                using (var g = Parent.CreateGraphics())
                    return CalcSizeAndTrunc(g, int.MaxValue, out _, out _);
            }
        }

        // Given the MaxWidth, or the passed-in width if the MaxWidth has not been specified,
        // calculate the amount of truncation of the name required,
        // and the size of the resultant truncated name.
        // trunc1 is the number of components to *remove* from the front of the name
        // trunc2 is the number of characters to *retain* in the final component (inc ellipsis if necessary), or zero if not required
        private Size CalcSizeAndTrunc(Graphics g, int width, out int trunc1, out int trunc2)
        {
            Size bigSize = new Size(int.MaxValue, int.MaxValue);

            Size theSize = new Size(0, 0);

            String[] pathParts = Folders.PointToPath(Path);
            if (pathParts == null)
            {
                theSize = Drawing.MeasureText(g, Messages.NONE, Font);
            }
            else
            {
                theSize.Width += 3;
                for (int i = 0; i < pathParts.Length; ++i)
                {
                    if (i > 0)
                        theSize.Width += Images.StaticImages.folder_separator.Width + INNER_PADDING;
                    string s = pathParts[i];
                    Size sz = Drawing.MeasureText(g, s, Font, bigSize, TextFormatFlags.NoPadding);
                    theSize.Width += sz.Width + INNER_PADDING;
                    theSize.Height = Math.Max(theSize.Height, sz.Height);
                }
            }

            if (hasChangeButton)
            {
                theSize.Width += RIGHT_PADDING;
                using (var underlineFont = new Font(Font, FontStyle.Underline))
                {
                    Size size2 = Drawing.MeasureText(g, Messages.CHANGE, underlineFont, bigSize, TextFormatFlags.NoPadding);
                    theSize.Width += size2.Width;
                    theSize.Height = Math.Max(theSize.Height, size2.Height);
                }
            }
            else
            {
                theSize.Width -= INNER_PADDING;
            }

            theSize.Width += this.itemBorder.Left + this.itemBorder.Right;
            theSize.Height += this.itemBorder.Top + this.itemBorder.Bottom;

            // theSize is now the width of the item without any truncation.
            // Calculate any truncation required.
            trunc1 = trunc2 = 0;
            if (pathParts != null)
            {
                int w = MaxWidth;
                if (w == 0)
                    w = width;
                for (; theSize.Width > w && trunc1 < pathParts.Length - 1; ++trunc1)
                {
                    // To truncate the first component, we replace the component with an ellipsis.
                    // Subsequently we delete a separator image and two paddings and the component. (Draw it!).
                    if (trunc1 == 0)
                        theSize.Width += Drawing.MeasureText(g, Messages.ELLIPSIS, Font, bigSize, TextFormatFlags.NoPadding).Width;
                    else
                        theSize.Width -= Images.StaticImages.folder_separator.Width + 2 * INNER_PADDING;
                    theSize.Width -= Drawing.MeasureText(g, pathParts[trunc1], Font, bigSize, TextFormatFlags.NoPadding).Width;
                }

                // It may still be that the text is too long, if the last component is very long.
                // In that case we need to ellipsise the final component.
                // (We just remove one character at a time: binary chop is excessive for normal folder lengths).
                if (theSize.Width > w)
                {
                    String s = pathParts[trunc1];
                    int needToLose = theSize.Width - w;
                    int widthBefore = Drawing.MeasureText(g, s, Font, bigSize, TextFormatFlags.NoPadding).Width;

                    for (trunc2 = s.Length + Messages.ELLIPSIS.Length - 1;
                        trunc2 > Messages.ELLIPSIS.Length &&
                            widthBefore - Drawing.MeasureText(g, s.Ellipsise(trunc2), Font, bigSize, TextFormatFlags.NoPadding).Width < needToLose;
                        --trunc2)
                        ;
                }
            }

            return theSize;
        }

        private FLIControl OnControl(Point point)
        {
            foreach (FLIControl control in controls)
            {
                if (control.rect.Contains(point))
                    return control;
            }
            return null;
        }

        public void OnMouseMove(Point point)
        {
            if (OnControl(point) != null)
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;
        }

        private Cursor Cursor
        {
            set
            {
                if (Parent != null)
                    Parent.Cursor = value;
            }
        }

        public void OnMouseLeave()
        {
            Cursor = Cursors.Default;
        }

        public void OnMouseClick(MouseEventArgs e, Point point)
        {
            if (e.Button != MouseButtons.Left)
                return;

            FLIControl control = OnControl(point);
            if (control == null)
                return;

            Program.MainWindow.SearchForFolder(control.tag);
        }

        private void changeLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            LaunchFolderChangeDlg();
        }

        private void LaunchFolderChangeDlg()
        {
            using (var dialog = new FolderChangeDialog(Path))
                if (dialog.ShowDialog(Program.MainWindow) == DialogResult.OK)
                {
                    if (!dialog.FolderChanged)
                        return;

                    Folder selectedFolder = dialog.CurrentFolder;

                    Path = selectedFolder == null
                        ? string.Empty
                        : Folders.AppendPath(selectedFolder.Path, selectedFolder.ToString());

                    OnPathChanged();
                }
        }

        private void OnPathChanged()
        {
            if (PathChanged != null)
                PathChanged();
        }

        private class FLIControl
        {
            public string tag;
            public Rectangle rect;

            public FLIControl(string tag, Rectangle rect)
            {
                this.tag = tag;
                this.rect = rect;
            }

            public FLIControl(string tag, int left, int top, int width, int height)
            {
                this.tag = tag;
                this.rect = new Rectangle(left, top, width, height);
            }
        }
    }
}
