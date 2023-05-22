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


namespace XenAdmin.Dialogs
{
    public partial class AcceptEuaDialog : XenDialogBase
    {
        private readonly List<string> _euas;
        private readonly HashSet<string> _warnings;

        public AcceptEuaDialog(List<string> euas)
        {
            InitializeComponent();
            _euas = euas;
            _warnings = new HashSet<string>();
            warningTableLayoutPanel.Visible = false;

            LoadEuas();
        }

        private void LoadEuas()
        {
            if (_euas.Count > 1)
            {
                _warnings.Add(Messages.ACCEPT_EUA_MORE_THAN_ONE_FOUND);
            }

            if (_warnings.Count > 0)
            {
                warningTableLayoutPanel.Visible = true;
                warningLabel.Text = string.Join(Environment.NewLine, _warnings);
            }
            else
            {
                warningTableLayoutPanel.Visible = false;
            }

            euaTextBox.Text = string.Join($"{Environment.NewLine}___________________________{Environment.NewLine}", _euas);
        }
    }
}
