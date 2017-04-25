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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.ExportWizard
{
	/// <summary>
	/// Class representing the page of the ExportAppliance wizard where the user specifies
	/// any EULA documents to be included in the exported appliance
	/// </summary>
	internal partial class ExportEulaPage : XenTabPage
	{
		/// <summary>
		/// Maximum number of EULA documents allowed
		/// </summary>
		private const int MAX_EULA_DOCS = 25;

		/// <summary>
		/// Maximum number of documents allowed to be viewed at the same time
		/// </summary>
		private const int MAX_VIEW_DOCS = 1;

		public ExportEulaPage()
		{
			InitializeComponent();
			m_toolTip.SetToolTip(m_buttonAdd, String.Format(Messages.EXPORT_EULA_PAGE_TOOLTIP, MAX_EULA_DOCS));
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.EXPORT_EULA_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.EULAS; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "ExportEula"; } }

		public override void PageLoaded(PageLoadedDirection direction)
		{
			base.PageLoaded(direction);
			SetAddButtonEnabledState();
			SetRemoveButtonEnabledState();
			SetViewButtonEnabledState();
		}

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

		#endregion

		#region Accessors
		/// <summary>
		/// Gets a collection of the paths to the EULA documents that will be included in the appliance
		/// </summary>
		public IEnumerable<string> Eulas
		{
			get
			{
				var eulas = new List<string>();

				foreach (ListViewItem item in m_listViewEulaFiles.Items)
					eulas.Add(item.Text);

				return eulas;
			}
		}
		#endregion

		#region Private methods

		private void ViewEulaDocuments()
		{
			foreach (ListViewItem item in m_listViewEulaFiles.SelectedItems)
			        Process.Start(item.Text);
		}

		private void SetAddButtonEnabledState()
		{
			//The add button should be disabled once the maximum number of EULA documents has been reached
			m_buttonAdd.Enabled = m_listViewEulaFiles.Items.Count < MAX_EULA_DOCS;
		}

		private void SetRemoveButtonEnabledState()
		{
			m_buttonRemove.Enabled = m_listViewEulaFiles.SelectedItems.Count > 0;
		}

		private void SetViewButtonEnabledState()
		{
			m_buttonView.Enabled = m_listViewEulaFiles.SelectedItems.Count == MAX_VIEW_DOCS;
		}

		private void AddEulaDocuments(string[] filePaths, out string invalidFile)
		{
			bool itemsAdded = false;
			invalidFile = string.Empty;

			foreach (string filePath in filePaths)
			{
				if (m_listViewEulaFiles.Items.Count == MAX_EULA_DOCS) //maximum number of eulas has been reached
					break;

				if (m_listViewEulaFiles.Items.ContainsKey(filePath)) //do not allow duplicate entries
					continue;

				var fileInfo = new FileInfo(filePath); //CA-55247: do not allow empty files
				if (fileInfo.Length <= 0)
				{
					invalidFile = filePath;
					return;
				}

				var item = new ListViewItem {Text = filePath, Name = filePath}; //set the Name so they're searchable by key
				m_listViewEulaFiles.Items.Add(item);

				if (!itemsAdded)
					itemsAdded = true;
			}

			if (itemsAdded) //only call these if item list has changed
			{
				m_listViewEulaFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				SetAddButtonEnabledState();
				IsDirty = true;
			}
		}

		private void RemoveEulaDocuments()
		{
			bool itemsRemoved = false;

			foreach (ListViewItem item in m_listViewEulaFiles.SelectedItems)
			{
				m_listViewEulaFiles.Items.Remove(item);

				if (!itemsRemoved)
					itemsRemoved = true;
			}

			if (itemsRemoved)//only call these if item list has changed
			{
				m_listViewEulaFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				SetAddButtonEnabledState();
				IsDirty = true;
			}
		}

		#endregion

		#region Control event handlers

		private void m_listViewEulaFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetRemoveButtonEnabledState();
			SetViewButtonEnabledState();
		}

		private void m_listViewEulaFiles_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ViewEulaDocuments();
		}

		private void m_buttonAdd_Click(object sender, EventArgs e)
		{
			using (var openFileDlog = new OpenFileDialog
			                             	{
			                             		CheckFileExists = true,
			                             		CheckPathExists = true,
			                             		DereferenceLinks = true,
			                             		Filter = Messages.EXPORT_EULA_PAGE_FILETYPES,
			                             		Multiselect = true,
			                             		RestoreDirectory = true
			                             	})
			{
				openFileDlog.FileOk += openFileDlog_FileOk;
				openFileDlog.ShowDialog();
			}
		}

		private void openFileDlog_FileOk(object sender, CancelEventArgs e)
		{
			OpenFileDialog dlog = (OpenFileDialog)sender;
			if (dlog == null)
				return;

			//check whether we exceed the allow number of documents and warn the user
			int numAllowedEulas = MAX_EULA_DOCS - m_listViewEulaFiles.Items.Count;

			if (dlog.FileNames.Length > numAllowedEulas)
			{
				var warnResult = MessageBox.Show(this,
				                                 String.Format(Messages.EXPORT_EULA_PAGE_FILE_LIMIT_WARNING, MAX_EULA_DOCS),
				                                 Messages.EXPORT_EULA_PAGE_FILE_LIMIT_WARNING_CAPTION,
				                                 MessageBoxButtons.OKCancel,
				                                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

				if (warnResult == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}

			string invalidFile;
			AddEulaDocuments(dlog.FileNames, out invalidFile);

			if (!String.IsNullOrEmpty(invalidFile))
			{
				e.Cancel = true;
				MessageBox.Show(this,
				                String.Format(Messages.EXPORT_EULA_PAGE_INVALID_FILE, invalidFile),
				                Messages.XENCENTER,
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
			}
		}

		private void m_buttonRemove_Click(object sender, EventArgs e)
		{
			RemoveEulaDocuments();
		}

		private void m_buttonView_Click(object sender, EventArgs e)
		{
			ViewEulaDocuments();
		}

		#endregion
	}
}
