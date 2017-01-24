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

namespace XenAdmin.Controls.SummaryPanel
{
    public partial class SummaryPanel : UserControl, ISummaryPanelView
    {
        public SummaryPanelController Controller { private get; set; }
        public SummaryPanel()
        {
            InitializeComponent();
            helperLink.LinkClicked += helperLink_LinkClicked;
            information.LinkClicked += information_LinkClicked;
        }

        public string Title
        {
            set{ Controller.Title = value; }
        }

        public string HelperUrl
        {
            set { Controller.HelperUrl = value; }
        }

        public bool HelperUrlVisible
        {
            set { Controller.HelperUrlVisible = value; }
        }

        public bool WarningVisible
        {
            set { Controller.DisplayWarning = value; }
        }

        public bool InformationVisible
        {
            set { Controller.InformationVisible = value; }
        }

        public string WarningText
        {
            set { Controller.WarningMessage = value; }
        }

        public Action RunOnUrlClick
        {
            set { Controller.RunOnUrlClick = value; }
        }

        public SummaryTextComponent SummaryText
        {
            set { Controller.TextSummary = value; }
        }

        public string InformationText
        {
            set { Controller.InformationText = value; }
        }

        public Bitmap WarningIcon
        {
            set { Controller.WarningIcon = value; }
        }

        private void helperLink_LinkClicked(object sender, EventArgs e)
        {
            Controller.UrlClicked();
        }

        private string summaryLink;

        private void information_LinkClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(summaryLink))
                Program.OpenURL(summaryLink);
        }
        
        #region ISummaryPanelView Members
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DrawTitle
        {
            set { titleLabel.Text = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DrawWarningMessage
        {
            set { warningText.Text = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Bitmap DrawWarningIcon
        {
            set { warningIcon.Image = value;  }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Bitmap DrawInformationIcon
        {
            set { informationImage.Image = value;  }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DrawHelperUrl
        {
            set { helperLink.Text = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool WarningTextVisiblity
        {
            set { warningText.Visible = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool WarningIconVisiblity
        {
            set { warningIcon.Visible = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DrawSummaryText
        {
            set
            {
                information.Text = value;
                information.LinkArea = new LinkArea(0, 0);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DrawSummaryLink
        {
            set { summaryLink = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public LinkArea DrawSummaryLinkArea
        {
            set { information.LinkArea = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool DrawHelperUrlVisible
        {
            set { helperLink.Visible = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool DrawInformationVisible
        {
            set { informationLayoutPanel.Visible = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DrawInformationText
        {
            set { informationMessage.Text = value; }
        }

        #endregion
    }
}
