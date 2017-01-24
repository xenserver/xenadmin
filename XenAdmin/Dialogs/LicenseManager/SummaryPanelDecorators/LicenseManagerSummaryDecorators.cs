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
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Controls.SummaryPanel;
using XenAdmin.Core;

namespace XenAdmin.Dialogs
{
    public class LicenseManagerSummaryDecorator : SummaryTextDecorator
    {
        public LicenseManagerSummaryDecorator(SummaryTextComponent component, CheckableDataGridViewRow row)
            : base(component)
        {
            Row = row as LicenseDataGridViewRow;
            Debug.Assert(Row != null && row != null, "Failure to cast CheckableDataGridViewRow to LicenseDataGridViewRow");
        }
        protected LicenseDataGridViewRow Row { get; private set; }
    }

    public class LicenseManagerSummaryLicenseServerDecorator : LicenseManagerSummaryDecorator
    {
        public LicenseManagerSummaryLicenseServerDecorator(SummaryTextComponent component, CheckableDataGridViewRow row) : base(component, row) { }

        private LinkArea linkArea = new LinkArea(0, 0);

        public override StringBuilder BuildSummary()
        {
            StringBuilder sb = base.BuildSummary();
            
            if (String.IsNullOrEmpty(Row.LicenseServer))
                return sb;

            sb.AppendLine(Messages.LICENSE_MANAGER_SUMMARY_LICENSE_SERVER);
            linkArea.Start = sb.Length;
            sb.AppendLine(Row.LicenseServer);
            linkArea.Length = Row.LicenseServerAddress.ToLower() == "localhost" ? 0 : Row.LicenseServerAddress.Length;
            return sb.AppendLine();
        }

        public override LinkArea GetLinkArea()
        {
            return linkArea;
        }

        public override string GetLink()
        {
            if (string.IsNullOrEmpty(Row.LicenseServer) || Row.LicenseServerAddress.ToLower() == "localhost")
                return string.Empty;

            return string.Format(Messages.LICENSE_SERVER_WEB_CONSOLE_FORMAT, Row.LicenseServerAddress, XenAPI.Host.LicenseServerWebConsolePort);
        }
    }

    public class LicenseManagerSummaryLicenseTypeDecorator : LicenseManagerSummaryDecorator
    {
        public LicenseManagerSummaryLicenseTypeDecorator(SummaryTextComponent component, CheckableDataGridViewRow row) : base(component, row) { }

        public override StringBuilder BuildSummary()
        {
            StringBuilder sb = base.BuildSummary();
            sb.AppendLine(Messages.LICENSE_MANAGER_SUMMARY_LICENSE_TYPE);
            sb.AppendLine(Row.LicenseName);
            return sb.AppendLine();
        }
    }

    public class LicenseManagerSummaryLicenseSocketsDecorator : LicenseManagerSummaryDecorator
    {
        public LicenseManagerSummaryLicenseSocketsDecorator(SummaryTextComponent component, CheckableDataGridViewRow row) : base(component, row) { }

        public override StringBuilder BuildSummary()
        {
            StringBuilder sb = base.BuildSummary();
            sb.AppendLine(Messages.LICENSE_MANAGER_SUMMARY_LICENSE_SOCKETS);
            sb.AppendLine(Helpers.ClearwaterOrGreater(Row.XenObject.Connection)
                              ? Row.NumberOfSockets.ToString()
                              : Messages.UNKNOWN);
            return sb.AppendLine();
        }
    }

    public class LicenseManagerSummaryLicenseExpiresDecorator : LicenseManagerSummaryDecorator
    {
        public LicenseManagerSummaryLicenseExpiresDecorator(SummaryTextComponent component, CheckableDataGridViewRow row) : base(component, row) { }

        public override StringBuilder BuildSummary()
        {
            StringBuilder sb = base.BuildSummary();

            if(Helpers.ClearwaterOrGreater(Row.XenObject.Connection) && Row.CurrentLicenseState == LicenseStatus.HostState.Free)
                return sb;

            sb.AppendLine(Messages.LICENSE_MANAGER_SUMMARY_LICENSE_EXPIRES);

            if(Row.LicenseExpires.HasValue)
            {
                if(LicenseStatus.IsInfinite(Row.LicenseExpiresIn))
                {
                    sb.AppendLine(Messages.NEVER);
                }
                else
                {
                    string date = HelpersGUI.DateTimeToString(Row.LicenseExpires.Value, Messages.DATEFORMAT_DMY_LONG, true);
                    sb.AppendLine(date);
                }
            }
            else
                sb.AppendLine(Messages.GENERAL_UNKNOWN);
            
            return sb.AppendLine();
        }
    }
}
