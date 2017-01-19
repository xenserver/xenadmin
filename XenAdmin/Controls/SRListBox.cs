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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Wizards.NewSRWizard_Pages;
using XenAdmin.Core;


namespace XenAdmin.Controls
{
    /// <summary>
    /// A flicker-free ListBox for SRs. 
    /// Add to this control ToStringWrapper<SR.SRInfo>s, which will
    /// be drawn 'UUID (size)' or will be drawn gray and unselectable.
    /// </summary>
    public class SRListBox : FlickerFreeListBox
    {
        private static readonly Font MonospaceFont = new Font(FontFamily.GenericMonospace, 9);
        private String MustSelectUUID = null;

        public SRListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += SRListBox_DrawItem;
            // Since we are doing OwnerDraw, we need to set the ItemHeight
            this.ItemHeight = Drawing.MeasureText("()01234567890abcdefABCDEFKTGMB", this.Font).Height + 2;
        }

        public void SetMustSelectUUID(String uuid)
        {
            MustSelectUUID = uuid;
        }

        public void TryAndSelectUUID()
        {
            foreach (SR.SRInfo info in Items)
                if (info.UUID == MustSelectUUID && CanSelectSRInfo(info))
                {
                    SelectedItem = info;
                    return;
                }

            SelectedIndex = -1;
        }

        private bool CanSelectSRInfo(SR.SRInfo info)
        {
            SR sr = SrWizardHelpers.SrInUse(info.UUID);

            if (sr != null && sr.HasPBDs)
                return false;

            if (!String.IsNullOrEmpty(MustSelectUUID) && info.UUID != MustSelectUUID)
                return false;

            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool movingUp = e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.PageUp;
            bool movingDown = e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.PageDown;
            bool movingPage = e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown;

            if (Items.Count <= 0 || !(movingUp || movingDown))
            {
                base.OnKeyDown(e);
                return;
            }

            int index = SelectedIndex;
            int bottomIndex = TopIndex + ClientSize.Height / ItemHeight;
            int firstAvailable = -1;
            int lastAvailableInView = -1;
            int lastAvailableOffView = -1;

            if (movingUp)
            {
                while (--index >= 0)
                {
                    SR.SRInfo nextInfo = Items[index] as SR.SRInfo;
                    if (nextInfo == null || CanSelectSRInfo(nextInfo))
                    {
                        if (firstAvailable == -1)
                            firstAvailable = index;

                        if (movingPage)
                        {
                            if (index >= TopIndex)
                                lastAvailableInView = index;
                            else
                                lastAvailableOffView = index;
                        }
                        else
                            break;
                    }
                }
            }

            if (movingDown)
            {
                while (++index <= Items.Count - 1)
                {
                    SR.SRInfo nextInfo = Items[index] as SR.SRInfo;
                    if (nextInfo == null || CanSelectSRInfo(nextInfo))
                    {
                        if (firstAvailable == -1)
                            firstAvailable = index;

                        if (movingPage)
                        {
                            if (index <= bottomIndex)
                                lastAvailableInView = index;
                            else
                                lastAvailableOffView = index;
                        }
                        else
                            break;
                    }
                }
            }

            if (movingPage)
            {
                if (lastAvailableInView > -1)
                    SelectedIndex = lastAvailableInView;
                else if (lastAvailableOffView > -1)
                    SelectedIndex = lastAvailableOffView;
            }
            else if (firstAvailable > -1)
                SelectedIndex = firstAvailable;

            e.Handled = true;
            base.OnKeyDown(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            SR.SRInfo info = SelectedItem as SR.SRInfo;
            if (info == null || CanSelectSRInfo(info))
            {
                base.OnSelectedIndexChanged(e);
                return;
            }

            //If we cannot select any SRInfos - give up
            if (!Items.Cast<SR.SRInfo>().Any(CanSelectSRInfo))
                return;

            TryAndSelectUUID();
        }

        private void SRListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            SR.SRInfo info = Items[e.Index] as SR.SRInfo;
            if (info == null)
                return;

            SR sr = SrWizardHelpers.SrInUse(info.UUID);
          
            Color fore, back;
            if (!Enabled)
            {
                fore = SystemColors.GrayText;
                back = SystemColors.Control;
            }
            else if (!CanSelectSRInfo(info))
            {
                // Draw in-use SRs grayed out
                fore = SystemColors.GrayText;
                back = SystemColors.Window;
            }
            else if ((e.State & DrawItemState.Selected) > 0)
            {
                // When control enabled, draw the selected item blue
                fore = SystemColors.HighlightText;
                back = SystemColors.Highlight;
            }
            else
            {
               fore = SystemColors.ControlText;
               back = SystemColors.Window;
            }

            // Fill in background
            using (SolidBrush backBrush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }
            
            if (sr == null)
            {
                // Draw SR UUID in monospace, but size in normal font

                Size s = Drawing.MeasureText(info.UUID + " ", MonospaceFont);
                Drawing.DrawText(e.Graphics, info.UUID + " ", MonospaceFont, e.Bounds.Location, fore);
                if(!String.IsNullOrEmpty(info.Aggr))
                    Drawing.DrawText(e.Graphics, info.Aggr + " ", e.Font, new Point(e.Bounds.Left + s.Width, e.Bounds.Top), fore);
            }
            else
            {
                String text = String.Format(Messages.SR_X_ON_Y, sr.Name, Helpers.GetName(sr.Connection));

                if (!sr.HasPBDs)
                    text = String.Format(Messages.DETACHED_BRACKETS, text);

                Drawing.DrawText(e.Graphics, text, e.Font, e.Bounds.Location, fore);
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
            switch (msg.Msg)
            {
                case Win32.WM_LBUTTONDOWN:
                case Win32.WM_LBUTTONDBLCLK:
                    int i = IndexFromPoint(PointToClient(MousePosition));
                    if (i < 0 || i >= Items.Count)
                        break;

                    SR.SRInfo info = Items[i] as SR.SRInfo;
                    if (info == null)
                        break;

                    // If cannot select item, ignore message
                    if (!CanSelectSRInfo(info))
                        return;

                    break;
            }

            base.WndProc(ref msg);
        }     
    }
}
