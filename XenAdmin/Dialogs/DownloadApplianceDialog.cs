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
using System.IO;
using System.Net;
using System.Windows.Forms;
using XenAdmin.Core;

using XenOvf;

namespace XenAdmin.Dialogs
{
	public partial class DownloadApplianceDialog : XenDialogBase
	{
		private readonly WebClient m_webClient;
		private readonly Uri m_uri;
		private readonly Queue<ApplianceFile> m_filesToDownload = new Queue<ApplianceFile>();

        public DownloadApplianceDialog(Uri uri)
		{
			InitializeComponent();
			m_pictureBoxError.Image = SystemIcons.Error.ToBitmap();
			m_tlpProgress.Visible = false;
			HideDownloadError();
			m_ctrlError.Visible = false;
			m_uri = uri;

			m_webClient = new WebClient();
			m_webClient.DownloadFileCompleted += webclient_DownloadFileCompleted;
			m_webClient.DownloadProgressChanged += webclient_DownloadProgressChanged;
		}

		public string DownloadedPath { get; private set; }

		#region Private methods

		private void ShowDownloadError(string error)
		{
			m_tlpError.Visible = true;
			m_labelError.Text = error;
			m_buttonDownload.Text = Messages.RETRY;
			m_buttonDownload.Enabled = true;
		}

		private void HideDownloadError()
		{
			m_tlpError.Visible = false;
		}

		private bool CheckPathValid(out string error)
		{
			error = string.Empty;

			if (String.IsNullOrEmpty(m_textBoxWorkspace.Text))
				return false;

			if (!PathValidator.IsPathValid(m_textBoxWorkspace.Text))
			{
				error = Messages.IMPORT_SELECT_APPLIANCE_PAGE_ERROR_INVALID_PATH;
				return false;
			}

			return true;
		}

		private bool CheckWorkspaceExists(out string error)
		{
			error = string.Empty;

			if (Directory.Exists(m_textBoxWorkspace.Text))
				return true;

			error = Messages.EXPORT_APPLIANCE_PAGE_ERROR_NON_EXIST_DIR;
			return false;
		}

		#endregion

		#region Event handlers

		private void webclient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			Program.Invoke(this, () => m_progressBar.Value = e.ProgressPercentage);
		}

		private void webclient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Error != null) //failure
			{
				Program.Invoke(this, () =>
				                     	{
				                     		DownloadedPath = null;
				                     		m_tlpProgress.Visible = false;
				                     	    var appfile = (ApplianceFile)e.UserState;
				                     	    ShowDownloadError(string.Format(Messages.IMPORT_DOWNLOAD_ERROR, appfile.RemoteUri, e.Error.Message));
				                     	});
			}
			else if (e.Cancelled) //user cancelled
			{
				Program.Invoke(this, () =>
				                     	{
				                     		DownloadedPath = null;
				                     		DialogResult = DialogResult.Cancel;
				                     	});
			}
			else //success
			{
				Program.Invoke(this, () =>
				                     	{
				                     		var appfile = (ApplianceFile)e.UserState;
											
											if (m_filesToDownload.Peek() == appfile)
												m_filesToDownload.Dequeue();

											if (appfile.LocalPath == DownloadedPath && DownloadedPath.ToLower().EndsWith(".ovf"))
				                     		{
				                     			var envType = OVF.Load(DownloadedPath);

				                     			if (envType != null)
				                     			{
				                     				int index = m_uri.OriginalString.LastIndexOf('/') + 1;
				                     				var remoteDir = m_uri.OriginalString.Substring(0, index);

				                     				foreach (var file in envType.References.File)
				                     				{
				                     					var remoteUri = new Uri(remoteDir + file.href);
				                     					var localPath = Path.Combine(m_textBoxWorkspace.Text, file.href);
				                     					m_filesToDownload.Enqueue(new ApplianceFile(remoteUri, localPath));
				                     				}
				                     			}
				                     		}

				                     		if (m_filesToDownload.Count == 0)
				                     		{
				                     			m_progressBar.Value = 100;
				                     			DialogResult = DialogResult.Yes;
				                     		}
				                     		else
				                     		{
				                     			var file = m_filesToDownload.Peek();
				                     			m_webClient.DownloadFileAsync(file.RemoteUri, file.LocalPath, file);
				                     		}
				                     	});
			}
		}

		#endregion

		#region Control event handlers

		private void m_textBoxWorkspace_TextChanged(object sender, EventArgs e)
		{
            m_ctrlError.PerformCheck(CheckPathValid);
		}

		private void m_buttonBrowse_Click(object sender, EventArgs e)
		{
			using (var dlog = new FolderBrowserDialog {Description = Messages.FOLDER_BROWSER_DOWNLOAD_APPLIANCE})
			{
				if (dlog.ShowDialog() == DialogResult.OK)
					m_textBoxWorkspace.Text = dlog.SelectedPath;
			}
		}
		
		private void m_buttonDownload_Click(object sender, EventArgs e)
		{
			if (m_ctrlError.PerformCheck(CheckWorkspaceExists))
			{
				HideDownloadError();
				m_tlpProgress.Visible = true;
				m_progressBar.Value = 0;
				m_buttonDownload.Enabled = false;

				Uri proxyUri = m_webClient.Proxy.GetProxy(m_uri);

				if (proxyUri != m_uri)
				{
					DownloadedPath = null;
					m_tlpProgress.Visible = false;
					ShowDownloadError(Messages.PROXY_SERVERS_NOT_SUPPORTED);
					return;
				}

				DownloadedPath = Path.Combine(m_textBoxWorkspace.Text, Path.GetFileName(m_uri.AbsolutePath));
				
				//CA-41747
				if (string.IsNullOrEmpty(Path.GetExtension(DownloadedPath)))
					DownloadedPath += ".ovf";

				var file = new ApplianceFile(m_uri, DownloadedPath);
				m_filesToDownload.Enqueue(file);
				m_webClient.DownloadFileAsync(m_uri, DownloadedPath, file);
			}
		}

		private void m_buttonCancel_Click(object sender, EventArgs e)
		{
			if (m_webClient.IsBusy)
				m_webClient.CancelAsync();
			else
				DialogResult = DialogResult.Cancel;
		}
		
		#endregion

		private class ApplianceFile
		{
			public ApplianceFile(Uri remoteUri, string localPath)
			{
				RemoteUri = remoteUri;
				LocalPath = localPath;
			}

			public Uri RemoteUri { get; private set; }

			public string LocalPath { get; private set; }
		}
	}
}
