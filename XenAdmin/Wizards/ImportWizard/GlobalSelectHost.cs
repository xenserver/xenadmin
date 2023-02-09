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
using XenAdmin.Controls;
using XenAdmin.Commands;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.ImportWizard
{
	internal partial class GlobalSelectHost : XenTabPage
    {
		private IXenObject m_selectedObject;
        private bool m_buttonNextEnabled;

        public event Action<IXenConnection> ConnectionSelectionChanged;

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

            var selectedHost = m_selectedObject as Host;
            if (selectedHost != null)
                m_poolHostPicker.SelectHost(selectedHost);
			else if (m_selectedObject != null  && m_selectedObject.Connection != null)
                m_poolHostPicker.SelectConnection(m_selectedObject.Connection);

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
            if (ConnectionSelectionChanged != null)
                ConnectionSelectionChanged(SelectedHost != null ? SelectedHost.Connection : SelectedConnection);
        }

		private void m_buttonAddNewServer_Click(object sender, EventArgs e)
        {
            new AddHostCommand(Program.MainWindow, this).Run();
		}

		#endregion

		public Host SelectedHost
		{
			get
			{
                return m_poolHostPicker.ChosenHost;
			}
		}

		public IXenConnection SelectedConnection
		{
			get
			{
                return m_poolHostPicker.ChosenConnection;
			}
		}

        public void SetDefaultTarget(IXenObject xenObject)
        {
            m_selectedObject = xenObject;
        }
	}
}
