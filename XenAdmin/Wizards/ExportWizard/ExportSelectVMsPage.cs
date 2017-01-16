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
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.ExportWizard
{
	/// <summary>
	/// Class representing the page of the ExportAppliance wizard where the user specifies
	/// the VMs to be included in the exported appliance
	/// </summary>
	internal partial class ExportSelectVMsPage : XenTabPage
	{
		#region Nested classes

		private static class NativeMethods
		{
			[DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
			internal static extern bool GetDiskFreeSpaceEx(string lpszPath, ref long lpFreeBytesAvailable, ref long lpTotalNumberOfBytes, ref long lpTotalNumberOfFreeBytes);
		}

		#endregion

        private bool m_buttonNextEnabled;

		public ExportSelectVMsPage()
		{
			InitializeComponent();
			m_tlpWarning.Visible = false;
			VMsToExport = new List<VM>();
		}

		#region Accessors

		/// <summary>
		/// Gets a list of the VMs that will be included in the exported appliance
		/// </summary>
		public List<VM> VMsToExport { get; private set; }

		public string ApplianceDirectory { get; set; }

		/// <summary>
		/// The items selected on the main window treeview when the wizard was launched.
		/// These determine the VMs selected by default.
		/// </summary>
		public SelectedItemCollection SelectedItems { private get; set; }

		public bool ExportAsXva { private get; set; }

		#endregion

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return ExportAsXva ? Messages.EXPORT_SELECTVMS_PAGE_TITLE_XVA : Messages.EXPORT_SELECTVMS_PAGE_TITLE_OVF; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.EXPORT_SELECTVMS_PAGE_TEXT; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
        public override string HelpID { get { return ExportAsXva ? "SelectVMsXva" : "SelectVMsOvf"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

		public override void PageLoaded(PageLoadedDirection direction)
		{
			base.PageLoaded(direction); //call first so the page gets populated
            m_ctrlError.HideError();
			EnableButtons();
		}

		public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
					cancel = !PerformCheck(CheckSpaceRequirements);

			base.PageLeave(direction, ref cancel);
		}

        public override void PopulatePage()
		{
			var pool = Helpers.GetPoolOfOne(Connection);
            label2.Text = string.Format(Helpers.IsPool(pool.Connection) ? Messages.VMS_IN_POOL : Messages.VMS_IN_SERVER,
                                                pool.Name.Ellipsise(60));
			VMsToExport.Clear();

			try
			{
				m_dataGridView.SuspendLayout();
				m_dataGridView.Rows.Clear();

				var applianceVMs = new List<XenRef<VM>>();
                if (SelectedItems != null && SelectedItems.FirstIs<VM_appliance>())
					applianceVMs.AddRange(((VM_appliance)SelectedItems.FirstAsXenObject).VMs);

				foreach (var vm in Connection.Cache.VMs.Where(vm => IsVmExportable(vm) && MatchesSearchText(vm)))
				{
					VM curVm = vm;//closure below
					bool selected = SelectedItems != null
					                && (SelectedItems.AsXenObjects().Contains(vm) || applianceVMs.FirstOrDefault(xenref => xenref.opaque_ref == curVm.opaque_ref) != null);

					m_dataGridView.Rows.Add(GetDataGridViewRow(vm, selected));

					if (selected)
						VMsToExport.Add(vm);
				}
				m_dataGridView.Sort(columnTick, ListSortDirection.Descending);
			}
			finally
			{
				m_dataGridView.ResumeLayout();
			}
			UpdateCounterLabel();
			EnableButtons();
		}

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Private methods

        /// <summary>
        /// Performs certain checks on the pages's input data and shows/hides an error accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        private bool PerformCheck(params CheckDelegate[] checks)
        {
            m_buttonNextEnabled = m_ctrlError.PerformCheck(checks);
            OnPageUpdated();
            return m_buttonNextEnabled;
        }
		
		private bool IsVmExportable(VM vm)
		{
			if (!vm.is_a_real_vm)
				return false;

			if (!vm.Show(Properties.Settings.Default.ShowHiddenVMs))
				return false;

			if (vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Suspended)
				return false;

			if (vm.Locked)
				return false;

			return vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.export);
		}

		private bool MatchesSearchText(VM vm)
		{
			return m_searchTextBox.Matches(vm.Name);
		}

		private DataGridViewRow GetDataGridViewRow(VM vm, bool selected)
		{
			var row = new DataGridViewRow {Tag = vm};
			var vmAppliance = Connection.Resolve(vm.appliance);
            long totalVmSize = GetTotalVmSize(vm);
			row.Cells.AddRange(new DataGridViewCell[]
			                   	{
			                   		new DataGridViewCheckBoxCell {Value = selected},
			                   		new DataGridViewTextBoxCell {Value = vm.Name},
			                   		new DataGridViewTextBoxCell {Value = vm.Description},
			                   		new DataGridViewTextBoxCell {Value = Util.DiskSizeString(totalVmSize), Tag = totalVmSize},
			                   		new DataGridViewTextBoxCell {Value = vmAppliance == null ? Messages.NONE : vmAppliance.Name}
			                   	});
			return row;
		}

        private long GetTotalVmSize(VM vm)
        {
            long virtualSize = 0;

            foreach (var vbdRef in vm.VBDs)
            {
                var vbd = Connection.Resolve(vbdRef);
                if (vbd == null)
                    continue;

                if (vbd.type == vbd_type.CD)
                    continue;

                VDI vdi = Connection.Resolve(vbd.VDI);
                if (vdi == null)
                    continue;

                virtualSize += vdi.virtual_size;
            }

            return virtualSize;
        }

	    private bool CheckSpaceRequirements(out string errorMsg)
		{
			errorMsg = string.Empty;
			long spaceNeeded = 0;

			foreach (DataGridViewRow row in m_dataGridView.Rows)
			{
				if ((bool)row.Cells[0].Value)
					spaceNeeded += (long)row.Cells[3].Tag;
			}

			long availableSpace = GetFreeSpace(ApplianceDirectory);
			if (spaceNeeded > availableSpace)
			{
			    errorMsg = String.Format(Messages.EXPORT_SELECTVMS_PAGE_ERROR_TARGET_SPACE_NOT_ENOUGH, Util.DiskSizeString(availableSpace), Util.DiskSizeString(spaceNeeded));
				//Log.Error(Messages.EXPORT_SELECTVMS_PAGE_ERROR_TargeSpaceNotEnough);
				return false;
			}

			return true;
		}

		//TODO: improve method
		private long GetFreeSpace(string drivename)
		{
			if (!drivename.EndsWith(@"\"))
				drivename += @"\";

			long space = 0;
			long lpTotalNumberOfBytes = 0;
			long lpTotalNumberOfFreeBytes = 0;

			if (NativeMethods.GetDiskFreeSpaceEx(drivename, ref space, ref lpTotalNumberOfBytes, ref lpTotalNumberOfFreeBytes))
				return space;

			return -1;
		}

		private void EnableButtons()
		{
			var count = VMsToExport.Count;
			m_tlpWarning.Visible = ExportAsXva && count > 1;
            m_buttonNextEnabled = ExportAsXva ? count == 1 : count > 0;
		    m_buttonClearAll.Enabled = count > 0;
		    m_buttonSelectAll.Enabled = count < m_dataGridView.RowCount;
            OnPageUpdated();
		}

		private void UpdateCounterLabel()
		{
			var count = VMsToExport.Count;
			m_labelCounter.Text = count == 1 ? Messages.ONE_VM_SELECTED : string.Format(Messages.MOREONE_VM_SELECTED, count);
		}

		#endregion

		#region Control event handlers

		private void m_buttonSelectAll_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow dataGridViewRow in m_dataGridView.Rows)
			{
				dataGridViewRow.Cells[0].Value = true;

				var vm = dataGridViewRow.Tag as VM;
				if (vm != null && !VMsToExport.Contains(vm))
					VMsToExport.Add(vm);
			}
		}

		private void m_buttonClearAll_Click(object sender, EventArgs e)
		{
			//clear all visible ones

			foreach (DataGridViewRow dataGridViewRow in m_dataGridView.Rows)
			{
				dataGridViewRow.Cells[0].Value = false;

				var vm = dataGridViewRow.Tag as VM;
				if (vm != null)
					VMsToExport.Remove(vm);
			}
		}

		private void m_searchTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				var sortedColumn = m_dataGridView.SortedColumn;
				var sortOrder = m_dataGridView.SortOrder;

				m_dataGridView.SuspendLayout();
				m_dataGridView.Rows.Clear();

				foreach (var vm in Connection.Cache.VMs.Where(IsVmExportable))
				{
					if (MatchesSearchText(vm))
						m_dataGridView.Rows.Add(GetDataGridViewRow(vm, VMsToExport.Contains(vm)));
				}

				if (sortOrder != SortOrder.None && sortedColumn != null)
					m_dataGridView.Sort(sortedColumn, (sortOrder == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);
			}
			finally
			{
				m_dataGridView.ResumeLayout();
			}
		}

		private void m_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (m_dataGridView.IsCurrentCellDirty)
				m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}

		private void m_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex >= m_dataGridView.RowCount)
				return;

			var row = m_dataGridView.Rows[e.RowIndex];
			var vm = row.Tag as VM;

			if (vm != null)
			{
				if ((bool)row.Cells[0].Value)
				{
					if (!VMsToExport.Contains(vm))
						VMsToExport.Add(vm);
				}
				else
					VMsToExport.Remove(vm);
			}

            m_ctrlError.HideError();
			UpdateCounterLabel();
			IsDirty = true;
			EnableButtons();
		}	

		private void m_dataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
		{
			if ((e.Row.State & DataGridViewElementStates.Selected) != 0)
				e.Row.Selected = false;
		}

        private void m_dataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column == columnSize)
            {
                e.SortResult = StringUtility.NaturalCompare(e.CellValue1.ToString(), e.CellValue2.ToString());
                e.Handled = true;
            }
        }
        
        #endregion
	}
}
