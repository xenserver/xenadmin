﻿/* Copyright (c) Citrix Systems, Inc. 
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
using System.Windows.Forms;

namespace XenAdmin.Controls.Common
{
    public delegate bool CheckDelegate(out string error);

    public partial class PasswordFailure : UserControl
    {
        public PasswordFailure()
        {
            InitializeComponent();
        }

        [Localizable(true)]
        public string Error
        {
            get
            {
                return errorLabel.Text;
            }
            set
            {
                errorLabel.Text = value;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            errorLabel.MaximumSize = new Size(Width - errorLabel.Margin.Left - errorLabel.Margin.Right -
                                              errorPictureBox.Width - errorPictureBox.Margin.Left - errorPictureBox.Margin.Right, 0);
        }

        public void ShowError(string errorMsg)
        {
            Visible = true;
            Error = errorMsg;
        }

        public void HideError()
        {
            Visible = false;
        }

        /// <summary>
        /// Performs certain checks on the pages's input data and shows/hides itself accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        /// <returns></returns>
        public bool PerformCheck(params CheckDelegate[] checks)
        {
            foreach (var check in checks)
            {
                string errorMsg;

                if (!check.Invoke(out errorMsg))
                {
                    if (string.IsNullOrEmpty(errorMsg))
                        HideError();
                    else
                        ShowError(errorMsg);

                    return false;
                }
            }

            HideError();
            return true;
        }
    }
}
