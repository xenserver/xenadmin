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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenAdmin.Wizards.ExportWizard.ApplianceChecks;
using XenAPI;
using XenCenterLib;
using XenModel;

namespace XenAdmin.Wizards.ExportWizard
{
	/// <summary>
	/// Class representing the page of the ExportAppliance wizard where the user defines
	/// the name and location where the appliance will be exported
	/// </summary>
	internal partial class ExportAppliancePage : XenTabPage
	{
		public ExportAppliancePage()
		{
			InitializeComponent();
            try
            {
                m_textBoxFolderName.Text = Win32.GetKnownFolderPath(Win32.KnownFolders.Downloads);
            }
            catch
            {
                //ignore
            }

            m_ctrlError.HideError();
		}

		#region Accessors

		/// <summary>
		/// Gets the parent directory of the exported appliance folder. It's not the appliance folder because
		/// this property is used to deduce space requirements on the target drive before the wizard closes
		/// and the appliance folder is only created once the wizard finishes.
		/// </summary>
		public string ApplianceDirectory => m_textBoxFolderName.Text.Trim();

		/// <summary>
		/// Gets or sets the exported appliance name (ovf/ova filename without extension).
		/// </summary>
        public string ApplianceFileName => m_textBoxApplianceName.Text.Trim();

        public bool ExportAsXva => radioButtonXva.Checked;

        public List<VM> VMsToExport { private get; set; }

        public bool IncludeMemorySnapshot { private get; set; }

		#endregion

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
        public override string PageTitle => Messages.EXPORT_APPLIANCE_PAGE_TITLE;

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
        public override string Text => Messages.EXPORT_APPLIANCE_PAGE_TEXT;

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
        public override string HelpID => ExportAsXva ? "ApplianceXva" : "ApplianceOvf";

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
			{
				m_textBoxFolderName.Text = m_textBoxFolderName.Text.Trim();
				m_textBoxApplianceName.Text = m_textBoxApplianceName.Text.Trim();
                cancel = !m_ctrlError.PerformCheck(CheckPathValid, CheckDestinationFolderExists, CheckApplianceExists, CheckPermissions, CheckSpaceRequirements);
                OnPageUpdated();
			}
		}

        public override void PopulatePage()
        {
            m_ctrlError.HideError();

            var bigDiskExists = BigDiskExists();
            var bigDiskMsg = string.Format(Messages.EXPORT_ERROR_EXCEEDS_MAX_SIZE_VDI_OVA_OVF,
                Util.DiskSizeString(SR.DISK_MAX_SIZE, 0));

            if (VMsToExport.Count == 1)
            {
                m_textBoxApplianceName.Text = VMsToExport[0].Name();
                
                radioButtonXva.Enabled = radioButtonXva.Checked = true;
                radioButtonOvf.Enabled = !IncludeMemorySnapshot && !bigDiskExists;

                if (IncludeMemorySnapshot)
                    labelOvf.Text = Messages.EXPORT_ERROR_INCLUDES_SNAPSHOT;
                else if (bigDiskExists)
                    labelOvf.Text = bigDiskMsg;

                _tlpInfoXva.Visible = false;
                _tlpInfoOvf.Visible = !radioButtonOvf.Enabled;
            }
            else
            {
                var vAppRef = VMsToExport.Select(v => v.appliance).Distinct().FirstOrDefault();
                var vApp = Connection.Resolve(vAppRef);
                if (vApp != null)
                    m_textBoxApplianceName.Text = vApp.Name();

                radioButtonXva.Enabled = false;
                radioButtonOvf.Enabled = radioButtonOvf.Checked = !bigDiskExists;
                
                if (bigDiskExists)
                    labelOvf.Text = bigDiskMsg;
                
                _tlpInfoXva.Visible = true;
                _tlpInfoOvf.Visible = !radioButtonOvf.Enabled;
            }

            CheckVtpm();
            OnPageUpdated();
        }

        private void CheckVtpm()
        {
            if (Helpers.FeatureForbidden(Connection, Host.RestrictVtpm) ||
                !Helpers.XapiEqualOrGreater_22_26_0(Connection) ||
                !VMsToExport.Any(v => v.VTPMs.Count > 0))
            {
                _tlpWarning.Visible = false;
            }
            else if (Helpers.XapiEqualOrGreater_23_9_0(Connection))
            {
                labelWarning.Text = Messages.VTPM_EXPORT_UNSUPPORTED_FOR_OVF;
                _tlpWarning.Visible = radioButtonOvf.Checked;
            }
            else
            {
                labelWarning.Text = Messages.VTPM_EXPORT_UNSUPPORTED_FOR_ALL;
                _tlpWarning.Visible = true;
            }
        }

        public override bool EnableNext()
        {
            return (radioButtonXva.Enabled && radioButtonXva.Checked ||
                    radioButtonOvf.Enabled && radioButtonOvf.Checked) &&
                   !m_ctrlError.Visible;
        }

		#endregion

		#region Private methods

		private bool CheckPathValid(out string error)
		{
			error = string.Empty;

			if (string.IsNullOrEmpty(ApplianceFileName))
				return false;

			if (!PathValidator.IsFileNameValid(ApplianceFileName, out string invalidNameMsg))
			{
				error = string.Join(" ", Messages.EXPORT_APPLIANCE_PAGE_ERROR_INALID_APP, invalidNameMsg);
				return false;
			}

			if (string.IsNullOrEmpty(ApplianceDirectory))
				return false;

			string path = $"{ApplianceDirectory}\\{ApplianceFileName}";

			if (!PathValidator.IsPathValid(path, out string invalidPathMsg))
			{
				error = string.Join(" ", Messages.EXPORT_APPLIANCE_PAGE_ERROR_INVALID_DIR, invalidPathMsg);
				return false;
			}

			return true;
		}

        private bool CheckPermissions(out string error)
        {
            error = string.Empty;
            var touchFile = Path.Combine(ApplianceDirectory, "_");

            try
            {
                //attempt writing in the destination directory
                FileStream fs = File.Create(touchFile);
                fs.Close();
                File.Delete(touchFile);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                error = Messages.EXPORT_APPLIANCE_PAGE_ERROR_PERMISSIONS;
                return false;
            }
        }

		private bool CheckDestinationFolderExists(out string error)
		{
			error = string.Empty;

			if (Directory.Exists(ApplianceDirectory))
				return true;

			error = Messages.ERROR_DESTINATION_DIR_NON_EXIST;
			return false;
		}

		private bool CheckApplianceExists(out string error)
		{
            ApplianceCheck.FileExtension extension = radioButtonXva.Checked
                ? ApplianceCheck.FileExtension.xva
                : ApplianceCheck.FileExtension.ovaovf;

		    ApplianceCheck check = new ApplianceExistsCheck(ApplianceDirectory, ApplianceFileName, extension);
		    check.Validate();
		    error = check.ErrorReason;
		    return check.IsValid;
		}

        private bool CheckSpaceRequirements(out string errorMsg)
        {
            errorMsg = string.Empty;
            ulong spaceNeeded = 0;

            foreach (var vm in VMsToExport)
            {
                spaceNeeded += vm.GetTotalSize();

                if (IncludeMemorySnapshot)
                {
                    var vdi = vm.Connection.Resolve(vm.suspend_VDI);
                    if (vdi != null)
                        spaceNeeded += (ulong)vdi.virtual_size;
                }
            }

            ulong availableSpace = GetFreeSpace(ApplianceDirectory);

            if (spaceNeeded > availableSpace)
            {
                errorMsg = string.Format(Messages.EXPORT_SELECTVMS_PAGE_ERROR_TARGET_SPACE_NOT_ENOUGH,
                    Util.DiskSizeString(availableSpace), Util.DiskSizeString(spaceNeeded));

                return false;
            }

            return true;
        }

        private bool BigDiskExists()
        {
            foreach (var vm in VMsToExport)
            {
                foreach (var vbdRef in vm.VBDs)
                {
                    var vbd = Connection.Resolve(vbdRef);
                    if (vbd == null)
                        continue;

                    var vdi = Connection.Resolve(vbd.VDI);
                    if (vdi == null)
                        continue;

                    if (vdi.virtual_size > SR.DISK_MAX_SIZE)
                        return true;
                }
            }

            return false;
        }

        private ulong GetFreeSpace(string drivename)
        {
            if (!drivename.EndsWith(@"\"))
                drivename += @"\";

            long space = 0;
            long lpTotalNumberOfBytes = 0;
            long lpTotalNumberOfFreeBytes = 0;

            if (Win32.GetDiskFreeSpaceEx(drivename, ref space, ref lpTotalNumberOfBytes, ref lpTotalNumberOfFreeBytes))
                return (ulong)space;

            return 0;
        }

		#endregion

		#region Control event handlers

		private void m_buttonBrowse_Click(object sender, EventArgs e)
		{
            using (var dlog = new FolderBrowserDialog
            {
                Description = Messages.FOLDER_BROWSER_EXPORT_APPLIANCE,
                SelectedPath = ApplianceDirectory
            })
			{
				if (dlog.ShowDialog() == DialogResult.OK)
					m_textBoxFolderName.Text = dlog.SelectedPath;
			}
		}

		private void m_textBoxApplianceName_TextChanged(object sender, EventArgs e)
		{
            IsDirty = true;
            m_ctrlError.PerformCheck(CheckPathValid);
            OnPageUpdated();
		}

		private void m_textBoxFolderName_TextChanged(object sender, EventArgs e)
		{
            IsDirty = true;
            m_ctrlError.PerformCheck(CheckPathValid);
            OnPageUpdated();
		}

        private void radioButtonOvf_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOvf.Checked)
            {
                IsDirty = true;
                CheckVtpm();
            }
        }

        private void radioButtonXva_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonXva.Checked)
            {
                IsDirty = true;
                CheckVtpm();
            }
        }

        #endregion
    }
}
