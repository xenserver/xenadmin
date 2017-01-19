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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using XenCenterLib.Archive;

using XenCenterLib.Compression;

namespace XenAdmin.Dialogs
{
	public partial class DecompressApplianceDialog : XenDialogBase
	{
		private readonly BackgroundWorker m_worker;
		private readonly string m_compressedFile;
		private readonly string m_decompressedFile;

		public DecompressApplianceDialog(string compresssedFile, string decompressedFile)
		{
			InitializeComponent();
			m_pictureBoxError.Image = SystemIcons.Error.ToBitmap();
			m_labelNotice.Text = string.Format(Messages.UNCOMPRESS_APPLIANCE_DESCRIPTION, Path.GetFileName(decompressedFile), Path.GetFileName(compresssedFile));
			m_tlpNotice.Visible = true;
			m_tlpProgress.Visible = false;
			HideError();

			m_compressedFile = compresssedFile;
			m_decompressedFile = decompressedFile;

			m_worker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
			m_worker.DoWork += m_worker_DoWork;
			m_worker.ProgressChanged += m_worker_ProgressChanged;
			m_worker.RunWorkerCompleted += m_worker_RunWorkerCompleted;
		}
		
		#region Private methods

		private void ShowError(string error)
		{
			m_tlpError.Visible = true;
			m_labelError.Text = error;
			m_buttonDecompress.Text = Messages.RETRY;
			m_buttonDecompress.Enabled = true;
		}

		private void HideError()
		{
			m_tlpError.Visible = false;
		}

		#endregion

		#region Event handlers

		private void m_worker_DoWork(object sender, DoWorkEventArgs e)
		{
			FileInfo info = new FileInfo(m_compressedFile);
			long length = info.Length;

			using (Stream inStream = File.Open(m_compressedFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
			using (Stream outStream = File.Open(m_decompressedFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
			{
				try
				{
                    using (CompressionStream bzis = CompressionFactory.Reader( CompressionFactory.Type.Gz, inStream))
					{
						byte[] buffer = new byte[4*1024];

						int bytesRead;
						while ((bytesRead = bzis.Read(buffer, 0, buffer.Length)) > 0)
						{
							outStream.Write(buffer, 0, bytesRead);

							int percentage = (int)Math.Floor((double)bzis.Position*100/length);
							
							if (percentage < 0)
								m_worker.ReportProgress(0);
							else if (percentage > 100)
								m_worker.ReportProgress(100);
							else
								m_worker.ReportProgress(percentage);

							if (m_worker.CancellationPending)
							{
								e.Cancel = true;
								return;
							}
						}
					}
				}
				finally
				{
					outStream.Flush();
				}
			}
		}

		private void m_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
		    Program.Invoke(this, () => m_progressBar.Value = e.ProgressPercentage);
		}

		private void m_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)//failure
			{
				Program.Invoke(this, () =>
				                     	{
											m_tlpProgress.Visible = false;
				                     		ShowError(e.Error.Message);
				                     		m_tlpProgress.Visible = false;
											//delete what has been unzipped so far
											//if file does not exist, no exception is thrown, so no need to check
											File.Delete(m_decompressedFile);
				                     	});
			}
			else if (e.Cancelled)//user cancelled
			{
				Program.Invoke(this, () =>
				                     	{
											File.Delete(m_decompressedFile);
				                     		DialogResult = DialogResult.Cancel;
				                     	});
			}
			else//success
			{
				Program.Invoke(this, () =>
				                     	{
											m_progressBar.Value = 100;
											File.Delete(m_compressedFile);
				                     		DialogResult = DialogResult.Yes;
				                     	});
			}
			
		}

		private void m_buttonDecompress_Click(object sender, EventArgs e)
		{
			HideError();
			m_tlpNotice.Visible = false;
			m_tlpProgress.Visible = true;
			m_progressBar.Value = 0;
			m_buttonDecompress.Enabled = false;
			m_worker.RunWorkerAsync();
		}

		private void m_buttonCancel_Click(object sender, EventArgs e)
		{
			if (m_worker.IsBusy)
				m_worker.CancelAsync();
			else
				DialogResult = DialogResult.Cancel;
		}

		#endregion
	}
}
