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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Dialogs.VMProtectionRecovery;
using XenAPI;


namespace XenAdmin.Dialogs
{
	public partial class DRConfigureDialog : XenDialogBase
	{
		#region Private fields
		private int m_numberOfCheckedSrs;
		private bool m_drOriginallyEnabled;
		
		/// <summary>
		/// The user cannot select more than a certain number of SRs
		/// </summary>
		private const int MAX_SR_SELECTED = 8;

		/// <summary>
		/// Style to use for checkable rows
		/// </summary>
		private DataGridViewCellStyle regStyle;
		/// <summary>
		/// Style to use for non-checkable rows (should appear grayed out)
		/// </summary>
		private DataGridViewCellStyle dimmedStyle;

        private List<SR> _availableSrs = new List<SR>();
		#endregion

		public DRConfigureDialog(Pool pool)
		{
			InitializeComponent();
			Text = String.Format(Messages.DR_CONFIGURE_TITLE, pool.Name);
            pictureBoxWarning.Image = SystemIcons.Warning.ToBitmap();
            pictureBoxInfo.Image = SystemIcons.Information.ToBitmap();
			HideAllWarnings();

			//setup cell styles
			regStyle = m_dataGridView.DefaultCellStyle.Clone();
			dimmedStyle = m_dataGridView.DefaultCellStyle.Clone();
			dimmedStyle.ForeColor = SystemColors.GrayText;

			Pool = pool;
		}

		#region Accessors
		public Pool Pool { get; private set; }

		public Dictionary<string, SR> SRtoEnable = new Dictionary<string, SR>();
		public Dictionary<string, SR> SRtoDisable = new Dictionary<string, SR>();
		#endregion

		#region Private methods

        /// <summary>
        /// First time population of the control
        /// </summary>
        private void PopulateSrDataGridView()
        {
            try
            {
                m_dataGridView.SuspendLayout();

                foreach (var sr in _availableSrs)
                {
                    var row = new SrRow(sr, Pool);

                    if (row.IsDrEnabled)
                    {
                        m_drOriginallyEnabled = true;
                        m_numberOfCheckedSrs++;
                    }

                    m_dataGridView.Rows.Add(row);
                    ToggleRowCheckable(row);
                }
            }
            finally
            {
                m_dataGridView.ResumeLayout();
            }
        }

		private void ToggleRowsCheckableState()
		{
			foreach (DataGridViewRow row in m_dataGridView.Rows)
				ToggleRowCheckable(row as SrRow);
		}

		private void ToggleRowCheckable(SrRow row)
		{
			if (row == null)
				return;

			bool checkable = row.HasSpace && m_numberOfCheckedSrs < MAX_SR_SELECTED;

			//if it's already checked do not consider it
			if (IsRowChecked(row))
				return;

			row.Cells[0].ReadOnly = !checkable;
			row.DefaultCellStyle = checkable ? regStyle : dimmedStyle;
		}

		private bool IsRowChecked(SrRow row)
		{
			if (row == null)
				return false;

			var cell = row.Cells[0] as DataGridViewCheckBoxCell;

			return cell == null ? false : (bool)cell.Value;
		}

		private bool IsRowChecked(int rowIndex, out SrRow row)
		{
			row = m_dataGridView.Rows[rowIndex] as SrRow;
			return IsRowChecked(row);
		}

		private void ToggleWarningsVisibleState()
		{
			ToggleWarningsWrapper(() =>
			                      	{
			                      		if (m_drOriginallyEnabled && m_numberOfCheckedSrs == 0)
			                      			ShowWarningOnDisable();
			                      		else if (!m_drOriginallyEnabled && m_numberOfCheckedSrs > 0)
			                      			ShowWarningOnEnable();
			                      		else
			                      			HideAllWarnings();
			                      	});
		}

		private void ToggleWarningsWrapper(Action action)
		{
			if (action == null)
				return;

			try
			{
				m_tableLpWarning.SuspendLayout();
				action.Invoke();
			}
			finally
			{
				m_tableLpWarning.ResumeLayout();
			}
		}

		/// <summary>
		/// Use the ToggleWarningsWrapper to call this to avoid flickering
		/// </summary>
		private void ShowWarningOnEnable()
		{
		    m_tableLpWarning.Visible = false;
            m_tableLpInfo.Visible = true;
		}

		/// <summary>
		/// Use the ToggleWarningsWrapper to call this to avoid flickering
		/// </summary>
		private void ShowWarningOnDisable()
        {
            m_tableLpWarning.Visible = true;
            m_tableLpInfo.Visible = false;
		}

		/// <summary>
		/// Use the ToggleWarningsWrapper to call this to avoid flickering
		/// </summary>
		private void HideAllWarnings()
		{
            m_tableLpWarning.Visible = false;
            m_tableLpInfo.Visible = true;
		}

        #endregion       
		
		#region Control event handlers

        private void DRConfigureDialog_Load(object sender, EventArgs e)
        {
            m_buttonOK.Enabled = false;
            spinnerIcon1.StartSpinning();
            _worker.RunWorkerAsync();
        }

        private void DRConfigureDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            _worker.CancelAsync();
        }

        private void m_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex > m_dataGridView.RowCount - 1)
                return;

            m_dataGridView.Rows[e.RowIndex].Cells[0].Value = !(bool)m_dataGridView.Rows[e.RowIndex].Cells[0].Value;
        }

	    private void m_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex > m_dataGridView.RowCount - 1)
				return;

			SrRow srRow;
			if (IsRowChecked(e.RowIndex, out srRow))
			{
				m_numberOfCheckedSrs++;

				if (srRow != null && !srRow.IsDrEnabled && !SRtoEnable.ContainsKey(srRow.SR.opaque_ref))
					SRtoEnable.Add(srRow.SR.opaque_ref, srRow.SR);
                if (srRow != null && SRtoDisable.ContainsKey(srRow.SR.opaque_ref))
                    SRtoDisable.Remove(srRow.SR.opaque_ref);
			}
			else
			{
				m_numberOfCheckedSrs--;

				if (srRow != null && srRow.IsDrEnabled && !SRtoDisable.ContainsKey(srRow.SR.opaque_ref))
					SRtoDisable.Add(srRow.SR.opaque_ref, srRow.SR);
                if (srRow != null && SRtoEnable.ContainsKey(srRow.SR.opaque_ref))
                    SRtoEnable.Remove(srRow.SR.opaque_ref);
			}

			ToggleWarningsVisibleState();
			ToggleRowsCheckableState();
		}
		
        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var srs = new List<SR>(Pool.Connection.Cache.SRs);
            for (int i = 0; i < srs.Count; i++)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var sr = srs[i];
                SR checkedSr = SR.SupportsDatabaseReplication(sr.Connection, sr) ? sr : null;
                int percentage = (i + 1) * 100 / srs.Count;
                _worker.ReportProgress(percentage, checkedSr);
            }
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SR sr = e.UserState as SR;
            if (sr != null)
                _availableSrs.Add(sr);
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            spinnerIcon1.StopSpinning();
            m_buttonOK.Enabled = true;
            PopulateSrDataGridView();
        }
        
        #endregion

		#region Nested Items

		private class SrRow : DataGridViewRow
		{
			public SrRow(SR sr, Pool pool)
			{
				SR = sr;
				var availableSpace = sr.FreeSpace;
				HasSpace = availableSpace > 0;

				foreach (var vdi in pool.Connection.Cache.VDIs)
				{
					if (vdi.type == vdi_type.metadata && vdi.metadata_of_pool.opaque_ref == pool.opaque_ref && vdi.SR.opaque_ref == sr.opaque_ref)
					{
						IsDrEnabled = true;
						break;
					}
				}

				var cellTick = new DataGridViewCheckBoxCell { Value = IsDrEnabled };
				var cellName = new DataGridViewTextAndImageCell { Value = sr.Name, Image = Images.GetImage16For(sr.GetIcon) };
				var cellDesc = new DataGridViewTextBoxCell { Value = sr.Description };
				var cellType = new DataGridViewTextBoxCell { Value = sr.FriendlyTypeName };
				var cellSpace = new DataGridViewTextBoxCell { Value = Util.DiskSizeString(availableSpace) };
				Cells.AddRange(cellTick, cellName, cellDesc, cellType, cellSpace);
			}

			public bool IsDrEnabled { get; private set; }

			public SR SR { get; private set; }

			public bool HasSpace { get; private set; }
		}

		#endregion
    }
}
