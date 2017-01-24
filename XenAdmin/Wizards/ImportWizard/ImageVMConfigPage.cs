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
using XenAdmin.Controls;
using XenAdmin.Core;


namespace XenAdmin.Wizards.ImportWizard
{
	internal partial class ImageVMConfigPage : XenTabPage
	{
		private const ulong KB = 1024;
		private const ulong MB = (KB * 1024);
		private const ulong GB = (MB * 1024);
        
        private bool m_buttonNextEnabled;

		public ImageVMConfigPage()
		{
			InitializeComponent();
            m_ctrlError.HideError();
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.IMAGE_DEFINITION_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.IMAGE_DEFINITION_PAGE_TEXT; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "VMConfig"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        public override void PopulatePage()
		{
			//CA-61385: remove wim support for Boston
			m_groupBoxAddSpace.Visible = false;
			m_groupBoxAddSpace.Enabled = IsWim;
			m_textBoxVMName.Text = string.Empty;
			m_upDownMemory.Value = m_upDownMemory.Minimum;
			m_upDownCpuCount.Value = m_upDownCpuCount.Minimum;
			m_upDownAddSpace.Value = m_upDownAddSpace.Minimum;
		}

        public override void SelectDefaultControl()
        {
            m_textBoxVMName.Select();
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

		public bool IsWim { private get; set; }

		public string VmName { get { return m_textBoxVMName.Text; } }

		public ulong CpuCount { get { return (ulong)m_upDownCpuCount.Value; } }

		public ulong Memory { get { return (ulong)m_upDownMemory.Value; } }

		public ulong AdditionalSpace { get { return m_groupBoxAddSpace.Enabled ? (ulong)m_upDownAddSpace.Value * GB : 0; } }

		#endregion

		#region Private Methods

        private bool CheckVmNameValid(string name, out string error)
		{
			error = string.Empty;

			if (String.IsNullOrEmpty(name))
				return false;

			if (!PathValidator.IsFileNameValid(name))
			{
				error = Messages.IMPORT_SELECT_APPLIANCE_PAGE_ERROR_INVALID_PATH;
				return false;
			}
			return true;
		}

		#endregion

		#region Control event handlers

		private void m_textBoxVMName_TextChanged(object sender, EventArgs e)
		{
            m_buttonNextEnabled = m_ctrlError.PerformCheck((out string error) => CheckVmNameValid(m_textBoxVMName.Text, out error));
            OnPageUpdated();
			IsDirty = true;
		}
		
		private void m_upDownMemory_ValueChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		private void m_upDownCpuCount_ValueChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		private void m_upDownAddSpace_ValueChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}

		#endregion
    }
}
