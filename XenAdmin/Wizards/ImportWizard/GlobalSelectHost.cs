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
using XenAdmin.Commands;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.ImportWizard
{
	internal partial class GlobalSelectHost : XenTabPage
    {
		private Host m_selectedHost;
		private IXenConnection m_selectedConnection;
        private bool m_buttonNextEnabled;

		public GlobalSelectHost()
		{
			InitializeComponent();
			m_poolHostPicker.AllowPoolSelect = true;
			m_poolHostPicker.SelectedItemChanged += SelectedItemChanged;
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.IMPORT_SELECT_HOST_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.HOME_SERVER; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "GlobalSelectHost"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        public override void PopulatePage()
		{
			m_poolHostPicker.buildList();

			if (m_selectedHost != null)
				m_poolHostPicker.SelectHost(m_selectedHost);
			else if (m_selectedConnection != null)
				m_poolHostPicker.SelectConnection(m_selectedConnection);

			IsDirty = true;
		}

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Control event handlers

		private void SelectedItemChanged(object sender, SelectedItemEventArgs e)
        {
			m_buttonNextEnabled = e.SomethingSelected;
            OnPageUpdated();
			IsDirty = true;
        }

		private void m_buttonAddNewServer_Click(object sender, EventArgs e)
        {
            new AddHostCommand(Program.MainWindow, this).Execute();
		}

		#endregion

		public Host SelectedHost
		{
			get
			{
				m_selectedHost = m_poolHostPicker.ChosenHost;
				return m_selectedHost;
			}
			set { m_selectedHost = value; }
		}

		public IXenConnection SelectedConnection
		{
			get
			{
				m_selectedConnection = m_poolHostPicker.ChosenConnection;
				return m_selectedConnection;
			}
			set { m_selectedConnection = value; }
		}
	}
}
