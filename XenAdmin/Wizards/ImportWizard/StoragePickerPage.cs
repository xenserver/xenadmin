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
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.ImportWizard
{
	public partial class StoragePickerPage : XenTabPage
	{
		#region Private fields
		private volatile Task m_importTask;
		private bool m_alreadyFoundVM;
		private ActionProgressDialog m_actionDialog;
        private string m_buttonNextText = Messages.WIZARD_BUTTON_NEXT;
        private bool m_buttonPreviousEnabled = true;
        private bool m_buttonNextEnabled;
		#endregion

		public event Action ImportVmCompleted;

		public StoragePickerPage()
		{
			InitializeComponent();
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle => Messages.IMPORT_SELECT_STORAGE_PAGE_TITLE;

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text => Messages.IMPORT_SELECT_STORAGE_PAGE_TEXT;

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID => "StoragePicker";

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            m_buttonNextText = (direction == PageLoadedDirection.Forward) ? Messages.IMPORT_VM_IMPORT : Messages.WIZARD_BUTTON_NEXT;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
			{
				cancel = true;
				IsDirty = false;
				SetButtonNextEnabled(false);
                m_buttonPreviousEnabled = false;
                OnPageUpdated();
                ImportXvaAction = new ImportVmAction(TargetHost == null ? TargetConnection : TargetHost.Connection,
                    TargetHost, FilePath, SR,
                    VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm);
				ImportXvaAction.Completed += m_importXvaAction_Completed;

				m_actionDialog = new ActionProgressDialog(ImportXvaAction, ProgressBarStyle.Blocks) {ShowCancel = true};
				// Now we want to start a scanning thread to search for the task ref on the vms
				ThreadPool.QueueUserWorkItem(ScanTask, null);
                m_alreadyFoundVM = false;
				m_actionDialog.Show(this);
			}
		}

        public override string NextText(bool isLastPage)
        {
            return m_buttonNextText;
        }

        public override void PopulatePage()
        {
            SetButtonNextEnabled(false);
            m_srPicker.Populate(SrPicker.SRPickerType.VM, TargetConnection, TargetHost, null, null);
            IsDirty = true;
        }

        public override bool EnablePrevious()
        {
            return m_buttonPreviousEnabled;
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

        internal IXenConnection TargetConnection { get; set; }

        internal Host TargetHost { get; set; }

		public ImportVmAction ImportXvaAction { get; private set; }

		public string FilePath { private get; set; }

		public SR SR => m_srPicker.SR;

		public VM ImportedVm { get; private set; }

		#endregion

		#region Private methods

        private void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }

		private bool ImportInProgress()
		{
			// Check if the upload action has started
			if (ImportXvaAction != null)
			{
				// if the VM has been uploaded we can continue
				if (m_alreadyFoundVM)
					return false;

				// if it has failed or been cancelled we let them try again
				if (ImportXvaAction.IsCompleted)
					return false;

				return true;
			}
			return false;
		}

		private void ScanTask(object obj)
		{
			Program.AssertOffEventThread();

			if (ImportXvaAction == null)
				return;

			while (ImportXvaAction.RelatedTask == null)
				Thread.Sleep(100);

			while ((m_importTask = TargetConnection.Resolve(ImportXvaAction.RelatedTask)) == null)
				Thread.Sleep(100);

            // We register a XenObjectsUpdated event handler where we check that the import task has the object creation phase marked as "complete"; 
            // Once the object creation is complete, we look for the vm; When we found the vm we unregister this event handler;
            TargetConnection.XenObjectsUpdated += targetConnection_XenObjectsUpdated;

			Program.Invoke(this, CheckTask);
		}

		private void CheckTask()
		{
			Program.AssertOnEventThread();

			// We need to wait for the task to make the object creation phase as complete

            if (m_importTask == null || m_importTask.other_config == null)
                return;

			if (!m_importTask.other_config.ContainsKey("object_creation"))
				return;

			if (m_importTask.other_config["object_creation"] != "complete")
				return;

            // Once this is done, look for the vm
            GetVm();
        }

        private void GetVm()
        {
            Program.AssertOnEventThread();

            if (m_alreadyFoundVM)
            {
                // Should never get here (as we unregister XenObjectsUpdated event handler after we find the vm) but just in case,
                // unregister XenObjectsUpdated event handler
                TargetConnection.XenObjectsUpdated -= targetConnection_XenObjectsUpdated;
                return;
            }

            foreach (VM vm in TargetConnection.Cache.VMs)
            {
                if (!vm.other_config.ContainsKey(ImportVmAction.IMPORT_TASK))
                    continue;

                string importTaskRef = vm.other_config[ImportVmAction.IMPORT_TASK];

                if (m_importTask.opaque_ref == importTaskRef)
                {
                    m_alreadyFoundVM = true;
                    ImportedVm = vm;

                    // We found the VM, unregister XenObjectsUpdated event handler
                    TargetConnection.XenObjectsUpdated -= targetConnection_XenObjectsUpdated;

                    // And close the dialog, flick to next page.
                    m_actionDialog.Close();
                    if (ImportVmCompleted != null)
                        ImportVmCompleted();
                    return;
                }
            }
		}

		#endregion

		#region Event handlers

        private void targetConnection_XenObjectsUpdated(object sender, EventArgs e)
        {
            CheckTask();
        }

        private void m_importXvaAction_Completed(ActionBase sender)
		{
			Program.Invoke(this, () =>
			{
				Program.AssertOnEventThread();

				if (!(ImportXvaAction.Succeeded) || ImportXvaAction.Cancelled)
				{
                    // task failed or has been cancelled, unregister XenObjectsUpdated event handler
                    TargetConnection.XenObjectsUpdated -= targetConnection_XenObjectsUpdated;

					// Give the user a chance to correct any errors
					m_actionDialog = null;
                    m_buttonPreviousEnabled = true;
                    OnPageUpdated();
				}
			});
		}

        private void m_srPicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ImportInProgress())
				return;

			SetButtonNextEnabled(m_srPicker.SR != null);
			IsDirty = true;
		}

        private void m_srPicker_CanBeScannedChanged()
        {
            buttonRescan.Enabled = m_srPicker.CanBeScanned;
            SetButtonNextEnabled(m_srPicker.SR != null);
        }

        private void buttonRescan_Click(object sender, EventArgs e)
        {
            m_srPicker.ScanSRs();
        }

        #endregion
    }
}
