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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using XenAdmin.Core;
using System.IO;


namespace XenAdmin.Dialogs
{
    /*
     * This code is based on the page:
     * http://www.codeproject.com/csharp/GetSaveFileName.asp?df=100&forumid=96342&exp=0&select=962159
     * 
     * It uses win32 to extend a save file dialog.
     */
    public class ExportVMDialog
    {
        public string DefaultExt = "";
        public string Filter = "";
        public string FileName = Messages.BACKUP_NAME;
        public string Title = "";
        public bool Verify = true;

        private IntPtr ComboHandle = IntPtr.Zero;
        private IWin32Window Owner;

        private IntPtr HookProc(IntPtr hdlg, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case Win32.WM_INITDIALOG:
                    // Center the dialog on its owner
                    Win32.RECT sr = new Win32.RECT();
                    Win32.RECT cr = new Win32.RECT();
                    IntPtr parent = Win32.GetParent(hdlg);
                    Win32.GetWindowRect(parent, ref cr);

                    Win32.GetWindowRect(Owner.Handle, ref sr);

                    int x = (sr.Right + sr.Left - (cr.Right - cr.Left)) / 2;
                    int y = (sr.Bottom + sr.Top - (cr.Bottom - cr.Top)) / 2;

                    Win32.SetWindowPos(parent, IntPtr.Zero, x, y, cr.Right - cr.Left, cr.Bottom - cr.Top + 32, Win32.SWP_NOZORDER);

                    IntPtr fileTypeWindow = Win32.GetDlgItem(parent, 0x441);
                    IntPtr fontHandle = Win32.SendMessage(fileTypeWindow, Win32.WM_GETFONT, IntPtr.Zero, IntPtr.Zero);

                    //we now need to find the combo-box to position the new tick box under

                    IntPtr fileComboWindow = Win32.GetDlgItem(parent, 0x470);
                    Win32.RECT aboveRect = new Win32.RECT();
                    Win32.GetWindowRect(fileComboWindow, ref aboveRect);

                    Win32.POINT point = new Win32.POINT();
                    point.X = aboveRect.Left;
                    point.Y = aboveRect.Bottom;
                    Win32.ScreenToClient(parent, ref point);

                    Win32.POINT rightPoint = new Win32.POINT();
                    rightPoint.X = aboveRect.Right;
                    rightPoint.Y = aboveRect.Top;

                    Win32.ScreenToClient(parent, ref rightPoint);

                    //we create the new combobox

                    IntPtr comboHandle = Win32.CreateWindowEx(0, "BUTTON", "", Win32.WS_VISIBLE | Win32.WS_CHILD | Win32.WS_TABSTOP | Win32.BS_AUTOCHECKBOX, point.X, point.Y + 8, rightPoint.X - point.X, 16, parent, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    fontHandle = Win32.SendMessage(fileTypeWindow, Win32.WM_GETFONT, IntPtr.Zero, IntPtr.Zero);

                    Win32.SendMessage(comboHandle, Win32.WM_SETFONT, fontHandle, IntPtr.Zero);
                    Win32.SendMessage(comboHandle, Win32.WM_SETTEXT, IntPtr.Zero, Messages.EXPORT_VM_VERIFY_POST_INSTALL);
                    Win32.SendMessage(comboHandle, Win32.BM_SETCHECK, (IntPtr)(Verify ? Win32.BST_CHECKED : Win32.BST_UNCHECKED), IntPtr.Zero);

                    //remember the handles of the controls we have created so we can destroy them after
                    ComboHandle = comboHandle;
                    break;

                case Win32.WM_DESTROY:
                    //destroy the handles we have created
                    if (ComboHandle != IntPtr.Zero)
                    {
                        Win32.DestroyWindow(ComboHandle);
                    }
                    break;

                case Win32.WM_NOTIFY:

                    //we need to intercept the CDN_FILEOK message
                    //which is sent when the user selects a filename

                    Win32.NMHDR nmhdr = (Win32.NMHDR)Marshal.PtrToStructure(lParam, typeof(Win32.NMHDR));

                    if (nmhdr.code == Win32.CDN_FILEOK)
                    {
                        Verify = Win32.SendMessage(ComboHandle, Win32.BM_GETCHECK, IntPtr.Zero, IntPtr.Zero) == (IntPtr)Win32.BST_CHECKED;
                    }
                    else if (nmhdr.code == Win32.CDN_HELP)
                    {
                        Help.HelpManager.Launch("ExportVMDialog");
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            //set up the struct and populate it

            Win32.OPENFILENAME ofn = new Win32.OPENFILENAME();

            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.lpstrFilter = Filter.Replace('|', '\0') + '\0';

            ofn.lpstrFile = FileName+ new string(' ', 512);
            ofn.lpTemplateName = FileName;
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = System.IO.Path.GetFileName(FileName) + new string(' ', 512);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = Title;
            ofn.lpstrDefExt = DefaultExt;

            //position the dialog above the active window
            Owner = owner;
            ofn.hwndOwner = owner.Handle;

            //set up some sensible flags
            ofn.Flags = Win32.OFN_EXPLORER | Win32.OFN_PATHMUSTEXIST | Win32.OFN_NOTESTFILECREATE | Win32.OFN_ENABLEHOOK | Win32.OFN_HIDEREADONLY | Win32.OFN_OVERWRITEPROMPT | Win32.OFN_SHOWHELP;

            //this is where the hook is set. Note that we can use a C# delegate in place of a C function pointer
            ofn.lpfnHook = new Win32.OFNHookProcDelegate(HookProc);

            //show the dialog

            if (!Win32.GetSaveFileName(ref ofn))
            {
                int ret = Win32.CommDlgExtendedError();

                if (ret != 0)
                {
                    throw new ApplicationException(string.Format(Messages.EXPORTVM_COULD_NOT_SHOW, ret.ToString()));
                }

                return DialogResult.Cancel;
            }

            FileName = ofn.lpstrFile;

            return DialogResult.OK;
        }
    }
}
