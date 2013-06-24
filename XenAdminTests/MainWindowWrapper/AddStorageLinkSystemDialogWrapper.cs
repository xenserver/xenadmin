/* Copyright (c) Citrix Systems Inc. 
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
using System.Text;
using XenAdmin.Dialogs;
using XenAdmin.Core;
using System.Windows.Forms;

namespace XenAdminTests
{
    internal class AddStorageLinkSystemDialogWrapper : TestWrapper<AddStorageLinkSystemDialog>
    {
        public AddStorageLinkSystemDialogWrapper(Win32Window window)
            : base(window)
        {
        }

        public Button CancelButton
        {
            get
            {
                return GetField<Button>("btnCancel");
            }
        }
        
        public Button btnOK
        {
            get
            {
                return GetField<Button>("btnOK");
            }
        }
       
        public ComboBox StorageAdapterComboBox
        {
            get
            {
                return GetField<ComboBox>("StorageAdapterComboBox");
            }
        }
        
        public TextBox NamespaceTextBox
        {
            get
            {
                return GetField<TextBox>("NamespaceTextBox");
            }
        }
        
        public TextBox PasswordTextBox
        {
            get
            {
                return GetField<TextBox>("PasswordTextBox");
            }
        }
        
        public TextBox UsernameTextBox
        {
            get
            {
                return GetField<TextBox>("UsernameTextBox");
            }
        }
        
        public TextBox IPAddressTextBox
        {
            get
            {
                return GetField<TextBox>("IPAddressTextBox");
            }
        }
        
        public TextBox PortNumberTextBox
        {
            get
            {
                return GetField<TextBox>("PortNumberTextBox");
            }
        }
    }
}
