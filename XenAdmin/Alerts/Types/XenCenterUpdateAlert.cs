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
using System.Text;
using XenAdmin.Core;


namespace XenAdmin.Alerts
{
    public class XenCenterUpdateAlert : Alert
    {
        public readonly XenCenterVersion NewVersion;

        public XenCenterUpdateAlert(XenCenterVersion version)
        {
            NewVersion = version;
            _timestamp = NewVersion.TimeStamp;
        }

        public override AlertPriority Priority { get { return AlertPriority.Priority5; } }

        public override string WebPageLabel
        {
            get { return Messages.AVAILABLE_UPDATES_DOWNLOAD_TEXT; }
        }

        public override string Name
        {
            get { return NewVersion.Name; }
        }

        public override string Title
        {
            get { return Messages.ALERT_NEW_VERSION; }
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.ALERT_NEW_VERSION_DETAILS, NewVersion.Name);
            }
        }

        public override Action FixLinkAction
        {
            get { return () => Program.OpenURL(NewVersion.Url); }
        }

        public override string FixLinkText
        {
            get
            {
                return Messages.ALERT_NEW_VERSION_DOWNLOAD;
            }
        }

        public override string AppliesTo
        {
            get
            {
                return Messages.XENCENTER;
            }
        }

        public override string HelpID
        {
            get
            {
                return "XenCenterUpdateAlert";
            }
        }

        public override void Dismiss()
        {            
            Properties.Settings.Default.LatestXenCenterSeen = NewVersion.VersionAndLang;
            Settings.TrySaveSettings();
            Updates.RemoveUpdate(this);
        }

        public override bool IsDismissed()
        {
            return Properties.Settings.Default.LatestXenCenterSeen == NewVersion.VersionAndLang;
        }

        public override bool Equals(Alert other)
        {
            if (other is XenCenterUpdateAlert)
            {
                return NewVersion.VersionAndLang == ((XenCenterUpdateAlert)other).NewVersion.VersionAndLang;
            }
            return base.Equals(other);
        }
    }
}
