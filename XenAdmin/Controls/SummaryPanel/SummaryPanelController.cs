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

namespace XenAdmin.Controls.SummaryPanel
{
    public class SummaryPanelController
    {
        public ISummaryPanelView View { private get; set; }

        public SummaryPanelController(){}

        public SummaryPanelController(ISummaryPanelView view)
        {
            View = view;
            View.DrawInformationIcon = Images.StaticImages._000_Info3_h32bit_16;
            View.DrawInformationVisible = false;
        }

        public string Title
        {
           set { View.DrawTitle = value; }
        }

        public string LicenseHelperUrlText
        {
            set { View.DrawLicenseHelperUrlText = value; }
        }

        public string SupportHelperUrlText
        {
            set { View.DrawSupportHelperUrlText = value; }
        }

        public bool LicenseHelperUrlVisible
        {
            set { View.DrawLicenseUrlVisible = value; }
        }

        public bool SupportHelperUrlVisible
        {
            set { View.DrawSupportUrlVisible = value; }
        }

        public string LicenseWarningMessage
        {
            set { View.DrawLicenseWarningMessage = value; }
        }

        public string SupportWarningMessage
        {
            set
            {
                View.DrawSupportWarningMessage = value;
            }
        }

        public bool InformationVisible
        {
            set { View.DrawInformationVisible = value; }
        }

        public string InformationText
        {
            set { View.DrawInformationText = value; }
        }

        public Bitmap LicenseWarningIcon
        {
            set { View.DrawLicenseWarningIcon = value; }
        }

        public Bitmap SupportWarningIcon
        {
            set { View.DrawSupportWarningIcon = value; }
        }
        
        public SummaryTextComponent TextSummary
        {
            set
            {
                View.DrawSummaryText = value.BuildSummary().ToString();
                View.DrawSummaryLinkArea = value.GetLinkArea();
                View.DrawSummaryLink = value.GetLink();
            }
        }

        public Action RunOnLicenseUrlClick { private get; set; }

        public Action RunOnSupportUrlClick { private get; set; }

        public void LicenseUrlClicked()
        {
            RunOnLicenseUrlClick?.Invoke();
        }

        public void SupportUrlClicked()
        {
            RunOnSupportUrlClick?.Invoke();
        }

        public bool DisplayLicenseWarning
        {
            set
            {
                View.LicenseWarningIconVisibility = value;
                View.LicenseWarningTextVisibility = value;
            }
        }

        public bool DisplaySupportWarning
        {
            set
            {
                View.SupportWarningIconVisibility = value;
                View.SupportWarningTextVisibility = value;
            }
        }
    }
}
