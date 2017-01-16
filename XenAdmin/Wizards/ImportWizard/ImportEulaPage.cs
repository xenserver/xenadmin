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
using XenAdmin.Controls.Common;
using XenOvf;
using XenOvf.Definitions;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.ImportWizard
{
	/// <summary>
	/// Class representing the page of the ImportAppliance wizard where the user accepts
	/// any EULAs included in the imported appliance.
	/// </summary>
	internal partial class ImportEulaPage : XenTabPage
	{
		private const int MAX_EULA_DOCS = 25;
        private bool m_buttonNextEnabled;

		public ImportEulaPage()
		{
			InitializeComponent();
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.IMPORT_EULA_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.EULAS; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "ImportEula"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

		public override void PageLoaded(PageLoadedDirection direction)
		{
			base.PageLoaded(direction);//call first so the page gets populated
			CheckEulaAccepted();
		}

		public override void CheckPageDisabled()
		{
			bool eulaExists = OVF.HasEula(SelectedOvfEnvelope);
			DisableStep = !eulaExists;
		}

        public override void PopulatePage()
		{
			if (DisableStep) //defensive check
				return;

			if (!m_checkBoxAccept.Enabled)
				m_checkBoxAccept.Enabled = true;

			m_tabControlEULA.TabPages.Clear();
			m_checkBoxAccept.Checked = false;

			EulaSection_Type[] eulas = OVF.FindSections<EulaSection_Type>(SelectedOvfEnvelope);

			if (!PerformCheck((out string error) => CheckNumberOfEulas(eulas.Length, out error)))
			{
				m_checkBoxAccept.Enabled = false;
				return;
			}

			foreach (EulaSection_Type eula in eulas)
			{
				RichTextBox rtb = new RichTextBox {ReadOnly = true, Dock = DockStyle.Fill, BackColor = SystemColors.Window, BorderStyle = BorderStyle.None};
				string licText = OVF.FindStringsMessage(SelectedOvfEnvelope, eula.License);

				if (licText.StartsWith(@"{\rtf"))
					rtb.Rtf = licText;
				else
					rtb.Text = licText;

				//RichTextBox does not support BorderStyle.FixedSingle, therefore as a workaround use
				//BorderStyle.None and put the RichTextBox in a panel with BorderStyle.FixedSingle
				Panel panel = new Panel {Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle};
				panel.Controls.Add(rtb);

				TabPage tp = new TabPage {Text = Messages.EULA};
				tp.Controls.Add(panel);

				m_tabControlEULA.TabPages.Add(tp);
			}
		}

        public override void SelectDefaultControl()
        {
            m_tabControlEULA.Select();
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }
		#endregion

		#region Accessors

		public EnvelopeType SelectedOvfEnvelope { private get; set; }

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

		private bool CheckNumberOfEulas(int number, out string error)
		{
			error = string.Empty;

			if (number > MAX_EULA_DOCS)
			{
				error = Messages.IMPORT_EULA_PAGE_MAX_TABS;
				return false;
			}

			return true;
		}

		private void CheckEulaAccepted()
		{
			m_buttonNextEnabled = m_checkBoxAccept.Enabled && m_checkBoxAccept.Checked;
            OnPageUpdated();
		}

		#endregion

		#region Control event handlers
		
		private void m_checkBoxAccept_CheckedChanged(object sender, EventArgs e)
		{
			CheckEulaAccepted();
		}
		
		#endregion
    }
}
