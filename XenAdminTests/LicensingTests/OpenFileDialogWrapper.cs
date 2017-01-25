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
using XenAdmin;
using XenAdmin.Core;

namespace XenAdminTests.LicensingTests
{
    internal class OpenFileDialogWrapper
    {
        private readonly string _initialWindowText;
        
        public OpenFileDialogWrapper(string initialWindowText)
        {
            _initialWindowText = initialWindowText;
        }

        public bool Visible
        {
            get
            {
                return DialogWindow != null;
            }
        }

        private Win32Window DialogWindow
        {
            get
            {
                return Win32Window.GetWindowWithText(_initialWindowText);
            }
        }
            
        public void SetFilename(string filename)
        {
            Util.ThrowIfStringParameterNullOrEmpty(filename, "filename");

            if (!Visible)
            {
                throw new InvalidOperationException("Install License Key OpenFileDialog not visible.");
            }

            foreach (Win32Window w in DialogWindow.Children)
            {
                if (w.WindowClass == "ComboBoxEx32")
                {
                    w.Text = filename;
                    return;
                }
            }
            throw new InvalidOperationException("Could not set filename");
        }

        public void ClickOK()
        {
            if (!Visible)
            {
                throw new InvalidOperationException("Install License Key OpenFileDialog not visible.");
            }

            foreach (Win32Window w in DialogWindow.Children)
            {
                if (w.Text == "&Open")
                {
                    const int WM_LBUTTONDOWN = 0x201;
                    const int WM_LBUTTONUP = 0x202;

                    w.SendMessage(WM_LBUTTONDOWN, 1, 0x00010001);
                    w.SendMessage(WM_LBUTTONUP, 0, 0x00010001);
                    return;
                }
            }

            throw new InvalidOperationException("Could not click OK.");
        }
    }
}
