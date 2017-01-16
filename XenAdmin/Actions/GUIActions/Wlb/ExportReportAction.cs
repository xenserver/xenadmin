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
using Microsoft.Reporting.WinForms;
using XenAPI;


namespace XenAdmin.Actions.Wlb
{
    /// <summary>
    /// This class exports the reports and stores the result into a byte array
    /// </summary>
    class ExportReportAction : AsyncAction
    {
        private byte[] _reportData;
        private string _format;
        private ReportViewer _reportViewer;

        public byte[] ReportData
        {
            get { return _reportData; }
        }

        public ExportReportAction(string format, ref ReportViewer reportViewer)
            : base(null, String.Format(Messages.WLB_REPORT_EXPORTING, format + "..."))
        {
            this._format = format;
            this._reportViewer = reportViewer;
        }

        protected override void Run()
        {
            SafeToExit = false;
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            this._reportData = _reportViewer.LocalReport.Render(
               this._format, null, out mimeType, out encoding, out extension,
               out streamids, out warnings);
        }
        
    }
}
