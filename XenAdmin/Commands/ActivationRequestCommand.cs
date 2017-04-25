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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    class ActivationRequestCommand : Command
    {
        private string _request;

        public ActivationRequestCommand(IMainWindow mainWindow,string request)
        {
            _request = request;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            ActivationRequestAction action = new ActivationRequestAction(_request);
            action.Completed += action_Completed;
            action.RunAsync();
        }

        void action_Completed(ActionBase sender)
        {
            ActivationRequestAction action = (ActivationRequestAction)sender;
            if (action.Succeeded)
                Program.OpenURL(string.Format(InvisibleMessages.ACTIVATION_FORM_URL, InvisibleMessages.ACTIVATION_SERVER, action.Result));
            else
            {
                if (DialogResult.Cancel == ShowSaveDialog())
                    throw action.Exception;

                SaveFileDialog fd = new SaveFileDialog();
                Program.Invoke(Program.MainWindow,
                    delegate()
                    {
                        fd.AddExtension = true;
                        fd.DefaultExt = "txt";
                        fd.Filter = string.Format("{0} (*.*)|*.*", Messages.ALL_FILES);
                        fd.FilterIndex = 0;
                        fd.RestoreDirectory = true;
                        if (DialogResult.Cancel == fd.ShowDialog(Program.MainWindow))
                            throw action.Exception;

                        using (FileStream fs = File.Open(fd.FileName, FileMode.Create))
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(_request);
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    });

                //Description = string.Format(Messages.ACTIVATION_REQUEST_SAVED, fd.FileName);
            }
        }

        private delegate DialogResult DialogInvoker();

        private DialogResult ShowSaveDialog()
        {
            return (DialogResult)Program.Invoke(Program.MainWindow,
                (DialogInvoker)delegate()
                {
                    DialogResult dialogResult;
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Error, string.Format(Messages.ACTIVATION_FAILED_MESSAGE, InvisibleMessages.ACTIVATION_SERVER)),
                        "ActivationServerUnavailable",
                        new ThreeButtonDialog.TBDButton(Messages.ACTIVATION_SAVE, DialogResult.Yes),
                        ThreeButtonDialog.ButtonCancel))
                    {
                        dialogResult = dlg.ShowDialog(Program.MainWindow);
                    }
                    return dialogResult;
                });
        }

    }
}
