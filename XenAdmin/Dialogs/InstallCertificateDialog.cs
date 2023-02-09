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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Dialogs
{
    partial class InstallCertificateDialog : XenDialogBase
    {
        private InstallCertificateAction _action;
        private readonly Host _host;

        public InstallCertificateDialog()
        {
            InitializeComponent();
        }

        public InstallCertificateDialog(Host host)
        {
            InitializeComponent();
            _host = host;
        }

        internal override string HelpName => "InstallCertificate";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = string.Format(Text, _host.Name());
            HideAllErrors();
            UpdateButtons();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            CancelInstallation();
            base.OnClosing(e);
        }

        private void HideAllErrors()
        {
            labelKeyError.Visible = false;
            labelCertificateError.Visible = false;
            labelChainError.Visible = false;
            tlpActionProgress.Visible = false;
            progressBar1.Visible = false;
        }

        private void UpdateButtons()
        {
            buttonInstall.Enabled = !string.IsNullOrEmpty(textBoxKey.Text) &&
                                    !string.IsNullOrEmpty(textBoxCertificate.Text) &&
                                    (_action == null || _action.IsCompleted);

            buttonRemove.Enabled = dataGridViewCertificates.SelectedRows.Count > 0 &&
                                   (_action == null || _action.IsCompleted);
        }

        private void CancelInstallation()
        {
            if (_action != null && !_action.IsCompleted)
                _action?.Cancel();
        }

        private void UpdateProgress()
        {
            if (_action == null)
                return;

            Program.Invoke(this, () =>
            {
                labelActionProgress.Text = _action.Description;

                if (_action.PercentComplete < 0)
                    progressBar1.Value = 0;
                else if (_action.PercentComplete > 100)
                    progressBar1.Value = 100;
                else
                    progressBar1.Value = _action.PercentComplete;
            });
        }

        private string DateConverter(string dateString)
        {
            string date = string.Empty;

            if (!Util.TryParseIso8601DateTime(dateString, out DateTime result))
                return dateString;

            Program.Invoke(this, () => { date = HelpersGUI.DateTimeToString(result.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true); });

            return date;
        }

        #region Event handlers

        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {
            HideAllErrors();
            UpdateButtons();
        }

        private void textBoxCertificate_TextChanged(object sender, EventArgs e)
        {
            HideAllErrors();
            UpdateButtons();
        }

        private void dataGridViewCertificates_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void buttonBrowseKey_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Multiselect = false,
                Title = Messages.CERTIFICATE_SELECT_KEY_TITLE,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = Messages.CERTIFICATE_KEY_FILETYPES
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    textBoxKey.Text = dlg.FileName;
            }
        }

        private void buttonBrowseCertificate_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title = Messages.CERTIFICATE_SELECT_CERTIFICATE_TITLE,
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = Messages.CERTIFICATE_FILETYPES
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    textBoxCertificate.Text = dlg.FileName;
            }
        }

        private void buttonAddCertificate_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title = Messages.CERTIFICATE_SELECT_CERTIFICATE_TITLE,
                Multiselect = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = Messages.CERTIFICATE_FILETYPES
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var rows = new List<DataGridViewRow>();

                    foreach (var name in dlg.FileNames)
                    {
                        var row = new CertificateRow(name);

                        if (dataGridViewCertificates.Rows.Cast<CertificateRow>().All(r => r.FileName != row.FileName))
                            rows.Add(row);
                        //All() returns true for empty collection which is correct for this predicate
                    }

                    dataGridViewCertificates.Rows.AddRange(rows.ToArray());
                    HideAllErrors();
                    UpdateButtons();
                }
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dataGridViewCertificates.SelectedRows.Count != 1)
                return;

            var row = dataGridViewCertificates.SelectedRows[0];
            dataGridViewCertificates.Rows.Remove(row);

            HideAllErrors();
            UpdateButtons();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            var certificateFiles = new List<string>();
            foreach (var gridRow in dataGridViewCertificates.Rows)
            {
                if (gridRow is CertificateRow row)
                    certificateFiles.Add(row.FileName);
            }

            _action = new InstallCertificateAction(_host, textBoxKey.Text.Trim(), textBoxCertificate.Text.Trim(),
                certificateFiles, DateConverter);

            _action.RequestReconnection += _action_RequestReconnection;
            _action.Changed += _action_Changed;
            _action.Completed += _action_Completed;

            labelKeyError.Visible = false;
            labelCertificateError.Visible = false;
            labelChainError.Visible = false;

            UpdateProgress();
            pictureBox1.Visible = false;
            progressBar1.Visible = true;
            tlpActionProgress.Visible = true;
            
            _action.RunAsync();
            UpdateButtons();
        }

        private void _action_RequestReconnection(IXenConnection conn)
        {
            Program.Invoke(Program.MainWindow, () => XenConnectionUI.BeginConnect(conn, false, null, false));
        }

        private void _action_Changed(ActionBase action)
        {
            UpdateProgress();
        }

        private void _action_Completed(ActionBase action)
        {
            _action.Completed -= _action_Completed;
            _action.Changed -= _action_Changed;

            Program.Invoke(this, () =>
            {
                UpdateProgress();
                UpdateButtons();

                if (_action.Succeeded)
                {
                    pictureBox1.Image = Images.StaticImages._000_Tick_h32bit_16;
                    buttonInstall.Enabled = false;
                    buttonCancel.Text = Messages.CLOSE;
                }
                else if (_action.IsError)
                {
                    pictureBox1.Image = Images.StaticImages._000_error_h32bit_16;

                    if (!string.IsNullOrEmpty(_action.KeyError))
                    {
                        labelKeyError.Text = _action.KeyError;
                        labelKeyError.Visible = true;
                        labelActionProgress.Text = Messages.CERTIFICATE_INSTALLATION_FAILURE;
                    }
                    else if (!string.IsNullOrEmpty(_action.CertificateError))
                    {
                        labelCertificateError.Text = _action.CertificateError;
                        labelCertificateError.Visible = true;
                        labelActionProgress.Text = Messages.CERTIFICATE_INSTALLATION_FAILURE;
                    }
                    else if (!string.IsNullOrEmpty(_action.ChainError))
                    {
                        labelChainError.Text = _action.ChainError;
                        labelChainError.Visible = true;
                        labelActionProgress.Text = Messages.CERTIFICATE_INSTALLATION_FAILURE;
                    }
                    else
                        labelActionProgress.Text = string.Format(Messages.CERTIFICATE_INSTALLATION_FAILURE_GENERIC,
                            _action.Exception.Message.Ellipsise(500));
                }

                pictureBox1.Visible = true;
            });
        }

        #endregion


        private class CertificateRow : DataGridViewRow
        {
            private readonly DataGridViewTextBoxCell _cellName = new DataGridViewTextBoxCell();

            public CertificateRow(string fileName)
            {
                FileName = fileName;

                Cells.Add(_cellName);
                _cellName.Value = FileName;
            }

            public string FileName { get; }
        }
    }
}
