using System;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Wizards.BugToolWizardFiles.StatusReportRows
{
    internal abstract class StatusReportRow : DataGridViewRow
    {
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

                default:
                    return string.Empty;
            }
        }
    }
}
