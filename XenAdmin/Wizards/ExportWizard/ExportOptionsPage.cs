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
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenOvf;
using XenAdmin.Controls;

namespace XenAdmin.Wizards.ExportWizard
{
	/// <summary>
	/// Class representing the page of the ExportAppliance wizard where the user specifies
	/// whether to create a manifest, sign the appliance or encrypt files and whether to
	/// create an OVA package or compress the OVF files
	/// </summary>
	internal partial class ExportOptionsPage : XenTabPage
	{
		private const int MIN_PASSWORD_STRENGTH = 1;
		private int m_passwordStrength;
		private bool m_isEncryptionOk = true;
		private bool m_isSignatureOk = true;
        private bool m_buttonNextEnabled;

		public ExportOptionsPage()
		{
			InitializeComponent();
		    m_ctrlErrorCert.HideError();
			m_buttonValidate.Enabled = false;
			m_labelStrength.Visible = false;
			m_pictureBoxTick.Visible = false;
			m_pictureBoxTickValidate.Visible = false;
			m_tableLayoutPanelManifest.Enabled = m_checkBoxManifest.Checked;

			//CA-59159: encryption is not supported for Boston
			sectionHeaderLabel2.Visible = false;
			m_tableLayoutPanelEncryption.Visible = false;
		}

		#region Accessors

		/// <summary>
		/// Gets a value indicating whether to create a manifest file
		/// </summary>
		public bool CreateManifest { get { return m_checkBoxManifest.Checked; } }

		/// <summary>
		/// Gets a value indicating whether to sign the appliance
		/// </summary>
		public bool SignAppliance { get { return m_checkBoxSign.Checked; } }

		/// <summary>
		/// Gets the certificate used to create the signature
		/// </summary>
		public X509Certificate2 Certificate { get; private set; }

		/// <summary>
		/// Gets a value indicating whether to encrypt the files reference in the OVF package.
		/// </summary>
		public bool EncryptFiles { get { return m_checkBoxEncrypt.Checked; } }

		/// <summary>
		/// Gets the password used for the encryption of the OVF files
		/// </summary>
		public string EncryptPassword { get { return m_textBoxPwd.Text; } }

		/// <summary>
		/// Gets a value indicating whether the appliance will be exproted as a single OVA file
		/// </summary>
		public bool CreateOVA { get { return m_checkBoxCreateOVA.Checked; } }

		/// <summary>
		/// Gets a value indicating whether the OVF files should be compressed
		/// </summary>
		public bool CompressOVFfiles { get { return m_checkBoxCompressFiles.Checked; } }

		#endregion

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.EXPORT_OPTIONS_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.EXPORT_OPTIONS_PAGE_TEXT; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "ExportOptions"; } }

		public override void PageLoaded(PageLoadedDirection direction)
		{
			base.PageLoaded(direction);
			SetButtonNextEnabled(true);
		}

		public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
				cancel = !(m_isSignatureOk && m_isEncryptionOk);

			base.PageLeave(direction, ref cancel);
		}

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Private methods

        /// <summary>
        /// Performs certain checks on the pages's input data and shows/hides an error accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        private bool PerformCheck(params CheckDelegate[] checks)
        {
            bool success = m_ctrlErrorCert.PerformCheck(checks);
            SetButtonNextEnabled(success);
            return m_buttonNextEnabled;
        }

        private void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }

		private void ToggleEncryptionCheckBoxCheckedState()
		{
			if (!string.IsNullOrEmpty(m_textBoxPwd.Text) || !string.IsNullOrEmpty(m_textBoxReEnterPwd.Text))
				m_checkBoxEncrypt.Checked = true;
		}

		private void OnCertificateInfoChanged()
		{
			if (!string.IsNullOrEmpty(m_textBoxCertificate.Text) || !string.IsNullOrEmpty(m_textBoxPrivateKeyPwd.Text))
				m_checkBoxSign.Checked = true;

		    m_ctrlErrorCert.HideError();
			m_buttonValidate.Enabled = !string.IsNullOrEmpty(m_textBoxCertificate.Text) && !string.IsNullOrEmpty(m_textBoxPrivateKeyPwd.Text);
			m_pictureBoxTickValidate.Visible = false;
			m_isSignatureOk = false;
			SetButtonNextEnabled(m_isEncryptionOk && m_isSignatureOk);
		}

		private void OnEncryptionChanged()
		{
			CheckEncryption();
			SetButtonNextEnabled(m_isEncryptionOk && m_isSignatureOk);
		}

		private void OnSignatureChanged()
		{
			CheckSignature();
			SetButtonNextEnabled(m_isEncryptionOk && m_isSignatureOk);
		}

		private void CalculatePassordStrength()
		{
			if (String.IsNullOrEmpty(m_textBoxPwd.Text))
			{
				m_passwordStrength = 0;
				m_labelStrength.Visible = false;
			}
			else
			{
				m_passwordStrength = OVF.CalculateStrength(m_textBoxPwd.Text);

				string strength;
				Color foreColor;

				switch (m_passwordStrength)
				{
					case 0:
						strength = Messages.PASSPHRASE_STRENGTH_LOW;
						foreColor = Color.Red;
						break;
					case 1:
						strength = Messages.PASSPHRASE_STRENGTH_FAIR;
						foreColor = Color.DarkOrange;
						break;
					case 2:
						strength = Messages.PASSPHRASE_STRENGTH_GOOD;
						foreColor = Color.Blue;
						break;
					case 3:
						strength = Messages.PASSPHRASE_STRENGTH_STRONG;
						foreColor = Color.Green;
						break;
					default:
						strength = Messages.PASSPHRASE_STRENGTH_UNKNOWN;
						foreColor = Color.FromKnownColor(KnownColor.WindowText);
						break;
				}

				m_labelStrength.Visible = true;
				m_labelStrength.Text = string.Format(Messages.PASSPHRASE_STRENGTH_PROMPT, strength);
				m_labelStrength.ForeColor = foreColor;
			}
		}

		private void CheckEncryption()
		{
			if (m_checkBoxEncrypt.Checked)
			{
				if (String.IsNullOrEmpty(m_textBoxPwd.Text) || String.IsNullOrEmpty(m_textBoxReEnterPwd.Text))
				{
					m_isEncryptionOk = false;
					m_pictureBoxTick.Visible = false;
					return;
				}

				if (m_textBoxPwd.Text != m_textBoxReEnterPwd.Text)
				{
					m_isEncryptionOk = false;
					m_pictureBoxTick.Visible = false;
					return;
				}

				m_pictureBoxTick.Visible = true;

				if (m_passwordStrength < MIN_PASSWORD_STRENGTH)
				{
					m_isEncryptionOk = false;
					return;
				}
			}
			
			m_isEncryptionOk = true;
		}

		private void CheckSignature()
		{
			if (m_checkBoxManifest.Checked && m_checkBoxSign.Checked)
			{
				if (String.IsNullOrEmpty(m_textBoxCertificate.Text) || String.IsNullOrEmpty(m_textBoxPrivateKeyPwd.Text)
					|| !PerformCheck(CheckCertificatePathValid, CheckCertificatePathExists, CheckSignPasswordValid, CheckCertificateValid))
				{
					m_pictureBoxTickValidate.Visible = false;
					m_isSignatureOk = false;
					return;
				}

				m_pictureBoxTickValidate.Visible = true;
				m_buttonValidate.Enabled = false;
			}

			m_isSignatureOk = true;
		}

		private bool CheckCertificatePathValid(out string error)
		{
			error = string.Empty;

			if (!PathValidator.IsPathValid(m_textBoxCertificate.Text))//includes null check
			{
				error = Messages.EXPORT_SECURITY_PAGE_ERROR_INVALID_CERT;
				return false;
			}

			return true;
		}

		private bool CheckCertificatePathExists(out string error)
		{
			error = string.Empty;

			if (!File.Exists(m_textBoxCertificate.Text))
			{
				error = Messages.EXPORT_SECURITY_PAGE_ERROR_CERT_NON_EXIST;
				return false;
			}
			return true;
		}

		private bool CheckSignPasswordValid(out string error)
		{
			error = string.Empty;

			try
			{
				Certificate = new X509Certificate2(m_textBoxCertificate.Text, m_textBoxPrivateKeyPwd.Text);
			}
			catch (CryptographicException)
			{
				error = Messages.EXPORT_SECURITY_PAGE_ERROR_SIGN_INVALID;
				return false;
			}

			return true;
		}

        /// <summary>
        /// Verify the certificate used to sign the package.
        /// </summary>
        private bool CheckCertificateValid(out string error)
        {
            error = string.Empty;

            try
            {
                if (Certificate != null && Certificate.Verify())
                    return true;
            }
            catch (CryptographicException)
            { }

            error = Messages.EXPORT_SECURITY_PAGE_ERROR_CERTIFICATE_INVALID;
            return false;
        }

		#endregion

		#region Control event handlers

		private void m_checkBoxManifest_CheckedChanged(object sender, EventArgs e)
		{
			m_tableLayoutPanelManifest.Enabled = m_checkBoxManifest.Checked;
			OnSignatureChanged();
			IsDirty = true;
		}

		private void m_checkBoxSign_CheckedChanged(object sender, EventArgs e)
		{
			OnSignatureChanged();
			IsDirty = true;
		}

		private void m_buttonBrowseCert_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDlog = new OpenFileDialog
													{
														CheckFileExists = true,
														CheckPathExists = true,
														DereferenceLinks = true,
														Filter = Messages.EXPORT_SECURITY_PAGE_FILETYPES,
														Multiselect = false,
														RestoreDirectory = true
													})
			{
				if (openFileDlog.ShowDialog() == DialogResult.OK)
					m_textBoxCertificate.Text = openFileDlog.FileName;
			}
		}

		private void m_textBoxCertificate_TextChanged(object sender, EventArgs e)
		{
			OnCertificateInfoChanged();
			IsDirty = true;
		}

		private void m_textBoxPrivateKeyPwd_TextChanged(object sender, EventArgs e)
		{
			OnCertificateInfoChanged();
			//no need to notify IsDirty here, this is only for data validation
		}

		private void m_buttonValidate_Click(object sender, EventArgs e)
		{
			OnSignatureChanged();
		}

		private void m_checkBoxEncrypt_CheckedChanged(object sender, EventArgs e)
		{
			OnEncryptionChanged();
			IsDirty = true;
		}

		private void m_textBoxPwd_TextChanged(object sender, EventArgs e)
		{
			ToggleEncryptionCheckBoxCheckedState();
			CalculatePassordStrength();
			OnEncryptionChanged();
			IsDirty = true;
		}

		private void m_textBoxReEnterPwd_TextChanged(object sender, EventArgs e)
		{
			ToggleEncryptionCheckBoxCheckedState();
			OnEncryptionChanged();
			//no need to notify IsDirty here, this is only for data validation
		}

		private void m_checkBoxCreateOVA_CheckedChanged(object sender, System.EventArgs e)
		{
			IsDirty = true;
		}

		private void m_checkBoxCompressFiles_CheckedChanged(object sender, System.EventArgs e)
		{
			IsDirty = true;
		}

		#endregion
	}
}
