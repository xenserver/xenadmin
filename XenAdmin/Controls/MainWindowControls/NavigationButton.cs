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

namespace XenAdmin.Controls.MainWindowControls
{
    class NavigationButton : ToolStripButton, INavigationItem
    {
        protected NavigationButton()
        {
            ImageScaling = ToolStripItemImageScaling.None;
            Overflow = ToolStripItemOverflow.Never;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (!Checked)
                Checked = true;
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            var pairedButton = PairedItem as ToolStripButton;

            if (pairedButton != null && pairedButton.Checked != Checked)
            {
                pairedButton.Checked = Checked;

                if (Checked && NavigationViewChanged != null)
                    NavigationViewChanged(Tag);
            }
        }

        public void SetTag(object tag)
        {
            Tag = tag;

            var pairedButton = PairedItem as ToolStripButton;
            if (pairedButton != null)
                pairedButton.Tag = tag;
        }

        public INavigationItem PairedItem { get; set; }
        public event Action<object> NavigationViewChanged;
    }


    class NavigationButtonBig : NavigationButton
    {
        public NavigationButtonBig()
        {
            ImageAlign = ContentAlignment.MiddleLeft;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            TextAlign = ContentAlignment.MiddleLeft;
        }
    }


    class NavigationButtonSmall : NavigationButton
    {
        public NavigationButtonSmall()
        {
            Alignment = ToolStripItemAlignment.Right;
        }
    }

    class NotificationButtonBig : NavigationButtonBig, INotificationItem
    {
        private int unreadEntries;

        public int UnreadEntries
        {
            get { return unreadEntries; }
            set
            {
                unreadEntries = value;
                
                ToolTipText = unreadEntries > 0
                     ? string.Format(Messages.NOTIFICATIONS_TOTAL, unreadEntries)
                     : Messages.NOTIFICATIONS_TOTAL_ZERO;
                
                Invalidate();
            }
        }
    }

    class NotificationButtonSmall : NavigationButtonSmall, INotificationItem
    {
        private int unreadEntries;

        public int UnreadEntries
        {
            get { return unreadEntries; }
            set
            {
                unreadEntries = value;

                if (unreadEntries > 0)
                {
                    Text = ToolTipText = string.Format(Messages.NOTIFICATIONS_TOTAL, unreadEntries);
                    Image = Properties.Resources.notif_alerts_16;
                }
                else
                {
                    Text = ToolTipText = Messages.NOTIFICATIONS_TOTAL_ZERO;
                    Image = Properties.Resources.notif_none_16;
                }

                Invalidate();
            }
        }
    }
}
