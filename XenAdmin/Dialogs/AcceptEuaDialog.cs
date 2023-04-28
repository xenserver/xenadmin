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
using System.Linq;
using System.Threading;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class AcceptEuaDialog : XenDialogBase
    {
        private readonly List<Host> _hosts;
        private readonly Uri _targetUri;
        private readonly HashSet<string> _euas;
        private readonly HashSet<string> _errors;
        private readonly HashSet<string> _warnings;


        public AcceptEuaDialog(List<Host> hosts, Uri targetUri)
        {
            InitializeComponent();
            _hosts = hosts;
            _targetUri = targetUri;
            _euas = new HashSet<string>();
            _errors = new HashSet<string>();
            _warnings = new HashSet<string>();

            SetLoading(true);
            warningTableLayoutPanel.Visible = false;

            ThreadPool.QueueUserWorkItem(LoadEuas, null);
        }

        private void LoadEuas(object _)
        {
           _hosts.AsParallel().ForAll(FetchHostEua);

            Program.BeginInvoke(this, () =>
            {
                if (_euas.Count > 1)
                {
                    _warnings.Add(Messages.ACCEPT_EUA_MORE_THAN_ONE_FOUND);
                }

                if (_warnings.Count > 0 || _errors.Count > 0)
                {
                    warningTableLayoutPanel.Visible = true;
                    warningLabel.Text = string.Join(Environment.NewLine, _errors.Concat(_warnings));
                }
                else
                {
                    warningTableLayoutPanel.Visible = false;
                }

                euaTextBox.Text = string.Join($"{Environment.NewLine}___________________________{Environment.NewLine}", _euas);
                SetLoading(false);
            });
        }

        private void FetchHostEua(Host host)
        {
            if (!Helpers.TryLoadHostEua(host, _targetUri, out var eua) && Helpers.Post82X(host))
            {
                _errors.Add(string.Format(Messages.ACCEPT_EUA_CANNOT_FETCH, BrandManager.BrandConsole));
                return;
            }

            lock (_euas)
            {
                _euas.Add(eua);
            }
        }
        private void SetLoading(bool loading)
        {
            if (loading)
            {
                spinnerIcon.StartSpinning();
            }
            else
            {
                spinnerIcon.StopSpinning();
            }

            lock (_euas)
            {
                acceptButton.Enabled = euaTextBox.Visible = !loading && _euas.Count > 0 && _errors.Count == 0;
            }
        }

        private void button_click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
