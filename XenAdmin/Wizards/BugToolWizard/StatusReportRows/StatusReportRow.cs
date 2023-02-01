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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Wizards.BugToolWizard
{
    partial class BugToolPageRetrieveData
    {
        private abstract class StatusReportRow : DataGridViewRow
        {
            private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            protected readonly DataGridViewExImageCell cellHostImg = new DataGridViewExImageCell();
            protected readonly DataGridViewTextBoxCell cellHost = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellStatus = new DataGridViewTextBoxCell();
            private readonly DataGridViewExImageCell cellResultImg = new DataGridViewExImageCell();

            public event Action<StatusReportRow> RowStatusChanged;
            public event Action<StatusReportRow> RowStatusCompleted;

            protected StatusReportRow()
            {
                Cells.AddRange(cellHostImg, cellHost, cellStatus, cellResultImg);
                cellResultImg.Value = new Bitmap(1, 1);
                UpdateCells(0);
            }

            public abstract StatusReportAction Action { get; }
            public int PercentComplete { get; private set; }

            public bool IsCompleted => Action != null && Action.IsCompleted;

            public bool IsSuccessful => Action != null && Action.IsCompleted && Action.Status == ReportStatus.succeeded;

            public void CancelAction()
            {
                if (Action == null)
                {
                    CreateAction(null, null);
                    if (Action != null)
                    {
                        Action.Changed += Action_Changed;
                        Action.Completed += Action_Completed;
                        CancelAction();
                    }
                    else
                    {
                        Log.Debug("Could not instantiate the requested action.");
                    }
                }
                if (Action != null && !Action.IsCompleted)
                    Action.Cancel();
            }

            public void RunAction(string path, string time)
            {
                CreateAction(path, time);
                Action.Changed += Action_Changed;
                Action.Completed += Action_Completed;
                Action.RunAsync();
            }

            public void DeRegisterEvents()
            {
                if (Action == null)
                    return;

                Action.Changed -= Action_Changed;
                Action.Completed -= Action_Completed;
            }

            private void Action_Changed(ActionBase action)
            {
                Program.Invoke(DataGridView, () =>
                {
                    UpdateCells(action.PercentComplete);
                    RowStatusChanged?.Invoke(this);
                });
            }

            private void Action_Completed(ActionBase action)
            {
                DeRegisterEvents();

                Program.Invoke(DataGridView, () =>
                {
                    UpdateCells(100);
                    RowStatusCompleted?.Invoke(this);
                });
            }

            protected abstract void CreateAction(string path, string time);

            private void UpdateCells(int percentComplete)
            {
                cellStatus.Value = GetStatus(out Image statusImage);
                PercentComplete = percentComplete;

                if (statusImage != null)
                    cellResultImg.Value = statusImage;
            }

            protected virtual string GetStatus(out Image img)
            {
                img = null;
                if (Action == null)
                    return Messages.BUGTOOL_REPORTSTATUS_QUEUED;

                switch (Action.Status)
                {
                    case ReportStatus.compiling:
                    return Messages.BUGTOOL_REPORTSTATUS_COMPILING;

                    case ReportStatus.succeeded:
                    img = Images.StaticImages._000_Tick_h32bit_16;
                    return Messages.COMPLETED;

                    case ReportStatus.failed:
                    img = Images.StaticImages._000_Abort_h32bit_16;
                    return Messages.BUGTOOL_REPORTSTATUS_FAILED;

                    case ReportStatus.cancelled:
                    img = Images.StaticImages.cancelled_action_16;
                    return Messages.BUGTOOL_REPORTSTATUS_CANCELLED;

                    case ReportStatus.queued:
                    return Messages.BUGTOOL_REPORTSTATUS_QUEUED;

                    case ReportStatus.downloading:
                    if (Action is IDataTransferStatusReportAction actionDownloading)
                    {
                        return string.Format(Messages.BUGTOOL_REPORTSTATUS_DOWNLOADING,
                                            Util.MemorySizeStringSuitableUnits(actionDownloading.DataTransferred, false));
                    }
                    return Messages.BUGTOOL_REPORTSTATUS_DOWNLOADING_NO_DATA;

                    case ReportStatus.packaging:
                    if (Action is IDataTransferStatusReportAction actionPackaging)
                    {
                        return string.Format(Messages.BUGTOOL_REPORTSTATUS_PACKAGING,
                            Util.MemorySizeStringSuitableUnits(actionPackaging.DataTransferred, false));
                    }
                    return Messages.BUGTOOL_REPORTSTATUS_PACKAGING_NO_DATA;
                    default:
                    return string.Empty;
                }
            }
        }
    }
}
