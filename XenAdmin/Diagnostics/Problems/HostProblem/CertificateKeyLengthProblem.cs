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

using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public class CertificateKeyLengthProblem : ProblemWithMoreInfo
    {
        private readonly Host _host;

        public CertificateKeyLengthProblem(Check check, Host host)
            : base(check)
        {
            _host = host;
        }

        public override string Description => string.Format(Messages.STRING_COLON_SPACE_STRING,
            _host.Name(), Messages.CERTIFICATE_KEY_LENGTH_PROBLEM_DESCRIPTION);

        public override string Message => Messages.CERTIFICATE_KEY_LENGTH_PROBLEM_MORE_INFO;

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                if (Helpers.StockholmOrGreater(_host))
                {
                    using (var dlg = new ErrorDialog(Message,
                        new ThreeButtonDialog.TBDButton(Messages.INSTALL_SERVER_CERTIFICATE_MENU, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT, true),
                        ThreeButtonDialog.ButtonCancel
                    ))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            var cmd = new InstallCertificateCommand(Program.MainWindow, _host);
                            if (cmd.CanRun())
                                cmd.Run();
                        }
                    }
                }
                else
                {
                    //this is only for safety in case version evaluation fails; it shouldn't happen under normal conditions
                    using (var dlg = new ErrorDialog(Message))
                        dlg.ShowDialog();
                }
            });

            cancelled = true;
            return null;
        }
    }


    public class CertificateKeyLengthWarningUrl : WarningWithMoreInfo
    {
        private readonly Host _host;

        public CertificateKeyLengthWarningUrl(Check check, Host host)
            : base(check)
        {
            _host = host;
        }

        public override string Description => string.Format(Messages.STRING_COLON_SPACE_STRING,
            _host.Name(), Messages.CERTIFICATE_KEY_LENGTH_PROBLEM_DESCRIPTION);

        public override string Message => string.Format(Messages.CERTIFICATE_KEY_LENGTH_WARNING_MORE_INFO,
            BrandManager.ProductBrand, BrandManager.ProductVersionPost82);

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                if (Helpers.StockholmOrGreater(_host))
                {
                    using (var dlg = new WarningDialog(Message,
                        new ThreeButtonDialog.TBDButton(Messages.INSTALL_SERVER_CERTIFICATE_MENU, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT, true),
                        ThreeButtonDialog.ButtonCancel
                    ))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            var cmd = new InstallCertificateCommand(Program.MainWindow, _host);
                            if (cmd.CanRun())
                                cmd.Run();
                        }
                    }
                }
                else
                {
                    //this is only for safety in case version evaluation fails; it shouldn't happen under normal conditions
                    using (var dlg = new WarningDialog(Message))
                        dlg.ShowDialog();
                }
            });

            cancelled = true;
            return null;
        }
    }
}
