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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAPI;
using XenOvf;

namespace XenAdmin.Wizards.ImportWizard
{
	/// <summary>
	/// Class representing the page of the ImportAppliance wizard where the user specifies
	/// whether to run OS fixups.
	/// </summary>
	internal partial class ImportOptionsPage : XenTabPage
	{
        private SR m_selectedIsoSr;
		private string m_isoFilename;
		private IEnumerable<Host> m_hostTargets;
        private bool m_buttonNextEnabled;
	    private bool m_fixupISOInLibrary;
        private List<SR> m_srsWithRegisteredEvents = new List<SR>();

		public ImportOptionsPage()
		{
			InitializeComponent();
            m_ctrlError.HideError();
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle
		{
			get { return Messages.IMPORT_OPTIONS_PAGE_TITLE; }
		}

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text
		{
			get { return Messages.IMPORT_OPTIONS_PAGE_TEXT; }
		}

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        public override void PopulatePage()
		{
            var isoName = Path.GetFileName(XenOvf.Properties.Settings.Default.xenLinuxFixUpDisk);
			VDI theVdi = Connection.Cache.VDIs.FirstOrDefault(vdi => vdi != null && vdi.name_label == isoName);

            m_fixupISOInLibrary = theVdi != null;
            if (m_fixupISOInLibrary) //found existing fixup ISO on an ISO SR
            {
                PopulateComboBox(Connection.Resolve(theVdi.SR));
            }
            else //iso has not been placed on an ISO SR
			{
                GetFixupIsoInfo();
                PopulateComboBox();
			}

            ResetSelectedIsoSr();
            m_radioButtonDontRunOSFixups.Checked = true;
		}

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

		public bool RunFixups { get { return m_radioButtonRunOSFixups.Checked; } }

		public SR SelectedIsoSR { get { return m_selectedIsoSr; } }

		public Dictionary<string, VmMapping> VmMappings { set { m_hostTargets = from VmMapping mapping in value.Values select Connection.Resolve(mapping.XenRef as XenRef<Host>); } }

		#endregion

		#region Private methods

        /// <summary>
        /// Performs certain checks on the pages's input data and shows/hides an error accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        private void PerformCheck(params CheckDelegate[] checks)
        {
            m_buttonNextEnabled = m_ctrlError.PerformCheck(checks);
            OnPageUpdated();
        }

		private bool IsIsoSrSelectable(SR sr)
		{
			return sr.content_type.ToLower() == "iso"
			       && sr.type.ToLower() == "iso"
			       && !sr.IsToolsSR
			       && m_hostTargets.All(sr.CanBeSeenFrom); //returns true if the list is empty
		}

        private void DeregisterEvents()
        {
            foreach (var sr in m_srsWithRegisteredEvents)
            {
                sr.PropertyChanged -= sr_PropertyChanged;
            }
        }

        private string GetSRDropDownItemDisplayString(SR sr)
        {
            return string.Format("{0} ({1})", sr.Name, Util.DiskSizeString(sr.FreeSpace));
        }

        private void PopulateComboBox()
        {
            PopulateComboBox(null);
        }

        private void PopulateComboBox(SR selectedSR)
		{
			var availableSRs = new List<SR>();

            foreach (var sr in Connection.Cache.SRs)
			{
                if (IsIsoSrSelectable(sr))
                {
                    if (!m_srsWithRegisteredEvents.Contains(sr))
                        m_srsWithRegisteredEvents.Add(sr);

                    sr.PropertyChanged -= sr_PropertyChanged;
                    sr.PropertyChanged += sr_PropertyChanged;
                    
                    availableSRs.Add(sr);
                }
			}

			try
			{
                m_comboBoxISOLibraries.SuspendLayout();
                m_comboBoxISOLibraries.Items.Clear();
                m_comboBoxISOLibraries.Items.Add(new ToStringWrapper<SR>(null, Messages.IMPORT_OPTIONS_PAGE_CHOOSE_ISO_SR));
                SetSelectedIndexWithoutEvent(0);

				foreach (var sr in availableSRs)
				{
				    int index = m_comboBoxISOLibraries.Items.Add(new ToStringWrapper<SR>(sr, GetSRDropDownItemDisplayString));
                    if (selectedSR != null && selectedSR.Equals(sr))
                        SetSelectedIndexWithoutEvent(index);
				}

                m_comboBoxISOLibraries.Items.Add(new ToStringWrapper<SR>(null, Messages.IMPORT_OPTIONS_PAGE_NEW_ISO_LIBRARY));
			}
			finally
			{
                m_comboBoxISOLibraries.ResumeLayout();
			}
		}

		private void GetFixupIsoInfo()
		{
            m_isoFilename = OVF.GetISOFixupFileName();
        }

		/// <summary>
		/// Checks fixup ISO exists in the XenCenter installation directory
		/// </summary>
		private bool CheckFixupIsoInXencenterInstallation(out string error)
		{
			error = string.Empty;

            if (m_radioButtonDontRunOSFixups.Checked || m_fixupISOInLibrary || !string.IsNullOrEmpty(m_isoFilename))
                return true;
            
			error = Messages.IMPORT_OPTIONS_PAGE_ERROR_NO_FIXUP_ISO_INSTALLED;
		    return false;
		}

		private bool CheckSelectedFixupValid(out string error)
		{
			error = String.Empty;

		    return m_radioButtonDontRunOSFixups.Checked || m_selectedIsoSr != null;
		}

		private void sr_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
            if (e.PropertyName != "name_label" && e.PropertyName != "physical_size" &&
                e.PropertyName != "physical_utilisation")
                return;

			var sr = sender as SR;
			if (sr == null)
				return;

			bool selectable = IsIsoSrSelectable(sr);
		    var existingItems =
		        m_comboBoxISOLibraries.Items.OfType<ToStringWrapper<SR>>().Where(wrapper => sr.Equals(wrapper.item));
            if (existingItems.Count() == 1)
			{
                if (selectable)
                    Program.Invoke(this, () =>
                                             {
                                                 var wrapper = existingItems.First();
                                                 m_comboBoxISOLibraries.SuspendLayout();
                                                 try
                                                 {
                                                     int index = m_comboBoxISOLibraries.Items.IndexOf(wrapper);
                                                     int selectedIndex = m_comboBoxISOLibraries.SelectedIndex;
                                                     m_comboBoxISOLibraries.Items.RemoveAt(index);
                                                     m_comboBoxISOLibraries.Items.Insert(index, new ToStringWrapper<SR>(sr, GetSRDropDownItemDisplayString));
                                                     SetSelectedIndexWithoutEvent(selectedIndex);
                                                 }
                                                 finally
                                                 {
                                                     m_comboBoxISOLibraries.ResumeLayout();
                                                 }
                                             });
                else
                    Program.Invoke(this, PopulateComboBox);
			}
			else if (selectable)
			{
				Program.Invoke(this, PopulateComboBox);
			}
		}

        private void SetSelectedIndexWithoutEvent(int newIndex)
        {
            m_comboBoxISOLibraries.SelectedIndexChanged -= m_comboBoxISOLibraries_SelectedIndexChanged;
            try
            {
                m_comboBoxISOLibraries.SelectedIndex = newIndex;
            }
            finally
            {
                m_comboBoxISOLibraries.SelectedIndexChanged += m_comboBoxISOLibraries_SelectedIndexChanged;
            }
        }

        private bool IsNewISOLibraryItem(ToStringWrapper<SR> wrapper)
        {
            return (wrapper.Equals(m_comboBoxISOLibraries.Items[m_comboBoxISOLibraries.Items.Count - 1]) &&
                    wrapper.ToStringProperty.Equals(Messages.IMPORT_OPTIONS_PAGE_NEW_ISO_LIBRARY));
        }

        private void OpenNewSRWizard()
        {
            SetSelectedIndexWithoutEvent(0);
            ResetControlsAfterSelectedIndexChanged();

            NewSRWizard wizard = new NewSRWizard(Connection);
			wizard.CheckNFSISORadioButton();

            if (wizard.ShowDialog() == DialogResult.OK)
                PopulateComboBox();
        }

        private void ResetSelectedIsoSr()
        {
            ToStringWrapper<SR> wrapper = m_comboBoxISOLibraries.SelectedItem as ToStringWrapper<SR>;
            m_selectedIsoSr = wrapper != null ? wrapper.item : null;
        }

        private void ResetControlsAfterSelectedIndexChanged()
        {
            ResetSelectedIsoSr();
            PerformCheck(CheckFixupIsoInXencenterInstallation, CheckSelectedFixupValid);

            if (!m_ctrlError.Visible)
            {
                m_pictureBoxInfo.Visible = m_selectedIsoSr != null;
                m_labelFixupISOInfo.Visible = m_selectedIsoSr != null;
                if (m_labelFixupISOInfo.Visible)
                    m_labelFixupISOInfo.Text = Messages.IMPORT_OPTIONS_PAGE_USE_SELECTED_ISO_LIBRARY;
            }
        }

        #endregion

        #region Control event Handlers

        private void m_comboBoxISOLibraries_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToStringWrapper<SR> wrapper = m_comboBoxISOLibraries.SelectedItem as ToStringWrapper<SR>;
            if (wrapper == null)
                return;

            if (IsNewISOLibraryItem(wrapper))
            {
                OpenNewSRWizard();
                return;
            }

            ResetControlsAfterSelectedIndexChanged();
        }

        private void m_radioButtonRunOSFixups_CheckedChanged(object sender, EventArgs e)
        {
            if (m_radioButtonRunOSFixups.Checked)
            {
                m_labelLocationFixupISO.Enabled = true;
                PerformCheck(CheckFixupIsoInXencenterInstallation, CheckSelectedFixupValid);

                if (!m_ctrlError.Visible)
                {
                    SR sr = m_selectedIsoSr;
                    m_comboBoxISOLibraries.Enabled = !m_fixupISOInLibrary || sr == null;
                    if (sr != null)
                    {
                        m_pictureBoxInfo.Visible = true;
                        m_labelFixupISOInfo.Visible = true;
                        m_labelFixupISOInfo.Text = m_fixupISOInLibrary
                                                       ? Messages.IMPORT_OPTIONS_PAGE_FOUND_EXISTING_ISO
                                                       : Messages.IMPORT_OPTIONS_PAGE_USE_SELECTED_ISO_LIBRARY;
                    }
                }
                IsDirty = true;
            }
        }

        private void m_radioButtonDontRunOSFixups_CheckedChanged(object sender, EventArgs e)
        {
            if (m_radioButtonDontRunOSFixups.Checked)
            {
                m_labelLocationFixupISO.Enabled = false;
                m_comboBoxISOLibraries.Enabled = false;
                m_pictureBoxInfo.Visible = false;
                m_labelFixupISOInfo.Visible = false;

                PerformCheck(CheckFixupIsoInXencenterInstallation);
            }
        }

        #endregion
    }
}
