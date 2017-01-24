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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Controls.SummaryPanel;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public interface ILicenseManagerView
    {
        LicenseManagerController Controller { set; }
        void DrawRowsInGrid(List<CheckableDataGridViewRow> itemsToShow);
        void DrawSelectedRowsAsChecked(List<CheckableDataGridViewRow> rows);
        void DrawSummaryForHighlightedRow(CheckableDataGridViewRow row, SummaryTextComponent component, Action runOnUrlClick);
        void DrawHighlightedRow(CheckableDataGridViewRow row);
        void DrawRowStatusIcon(int rowIndex, LicenseDataGridViewRow.Status rowStatus);
        void DrawAssignButtonAsDisabled(bool isDisabled);
        void DrawReleaseButtonAsDisabled(bool isDisabled);
        void DrawActivateButtonAsDisabled(bool isDisabled);
        void DrawActivateButtonAsHidden(bool isHidden);
        void DrawRequestButtonAsDisabled(bool isDisabled);
        void DrawApplyButtonAsDisabled(bool isDisabled, string disabledReason);
        List<CheckableDataGridViewRow> GetCheckedRows { get; }
        void ClearAllGridRows();
        Control Parent { get; }
        void DrawSummaryInformation(string info, bool show);
        void SetRowDisabledRowInfo(int rowIndex, string info, bool disabled);
        void DrawViewAsReadOnly(bool isReadOnly);
    }
}
