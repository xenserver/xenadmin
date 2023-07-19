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


using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Commands
{
    class InstallCertificateCommand : CertificateCommand
    {
        public InstallCertificateCommand()
        {
        }

        public InstallCertificateCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        public InstallCertificateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var host = selection.AsXenObjects<Host>().FirstOrDefault();

            if (host == null || !CheckRbacPermissions(host))
                return;

            MainWindowCommandInterface.ShowForm(typeof(InstallCertificateDialog), new object[] {host});
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            if (selection.Count != 1 || !(selection[0].XenObject is Host host) || !Helpers.StockholmOrGreater(host))
                return false;

            var pool = Helpers.GetPoolOfOne(host.Connection);
            return pool != null && !pool.ha_enabled;
        }

        protected override string GetCantRunReasonCore(IXenObject item)
        {
            if (item is Host host)
            {
                var pool = Helpers.GetPoolOfOne(host.Connection);
                if (pool != null && pool.ha_enabled)
                    return Messages.INSTALL_SERVER_CERTIFICATE_HA;
            }
            
            return base.GetCantRunReasonCore(item);
        }

        public override string ContextMenuText => MenuText;

        public override string MenuText => Messages.INSTALL_SERVER_CERTIFICATE_MENU;
    }


    class ResetCertificateCommand : CertificateCommand
    {
        public ResetCertificateCommand()
        {
        }

        public ResetCertificateCommand(IMainWindow mainWindow, IXenObject xenObject)
            : base(mainWindow, xenObject)
        {
        }
        
        public ResetCertificateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var host = selection.AsXenObjects<Host>().FirstOrDefault();

            if (host == null || !CheckRbacPermissions(host))
                return;

            using (var dialog = new WarningDialog(Messages.RESET_SERVER_CERTIFICATE_WARNING,
                ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
                if (dialog.ShowDialog(Program.MainWindow) != DialogResult.Yes)
                    return;

            new DelegatedAsyncAction(host.Connection,
                string.Format(Messages.RESET_SERVER_CERTIFICATE_TITLE, host.Name()),
                Messages.RESET_SERVER_CERTIFICATE_DESCRIPTION, Messages.COMPLETED,
                s => Host.reset_server_certificate(s, host.opaque_ref),
                "host.host_reset_server_certificate").RunAsync();
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.Count == 1 && selection[0].XenObject is Host host && Helpers.CloudOrGreater(host) && Helpers.XapiEqualOrGreater_1_290_0(host);
        }

        public override string ContextMenuText => MenuText;

        public override string MenuText => Messages.RESET_SERVER_CERTIFICATE_MENU;
    }


    class CertificateCommand : Command
    {
        public CertificateCommand()
        {
        }

        public CertificateCommand(IMainWindow mainWindow, IXenObject xenObject)
            : base(mainWindow, xenObject)
        {
        }

        public CertificateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected bool CheckRbacPermissions(Host host)
        {
            if (host != null && !host.Connection.Session.IsLocalSuperuser && !Registry.DontSudo &&
                host.Connection.Session.Roles.All(r => r.name_label != Role.MR_ROLE_POOL_ADMIN))
            {
                var currentRoles = host.Connection.Session.Roles;
                currentRoles.Sort();

                var msg = string.Format(Messages.CERTIFICATE_RBAC_RESTRICTION, currentRoles[0].FriendlyName(),
                    Role.FriendlyName(Role.MR_ROLE_POOL_ADMIN));

                using (var dlg = new ErrorDialog(msg))
                    dlg.ShowDialog(Parent);

                return false;
            }

            return true;
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return new InstallCertificateCommand().CanRunCore(selection) ||
                   new ResetCertificateCommand().CanRunCore(selection);
        }

        public override string MenuText => Messages.MAINWINDOW_CERTIFICATE_MENU_TEXT;

        public override string ContextMenuText => Messages.MAINWINDOW_CERTIFICATE_CONTEXT_MENU_TEXT;
    }
}
