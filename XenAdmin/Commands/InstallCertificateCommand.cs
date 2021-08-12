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


using System.Collections.Generic;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Commands
{
    class InstallCertificateCommand : Command
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

            if (host != null && !host.Connection.Session.IsLocalSuperuser && !Registry.DontSudo &&
                host.Connection.Session.Roles.All(r => r.name_label != Role.MR_ROLE_POOL_ADMIN))
            {
                var currentRoles = host.Connection.Session.Roles;
                currentRoles.Sort();

                var msg = string.Format(Messages.CERTIFICATE_RBAC_RESTRICTION, currentRoles[0].FriendlyName(),
                    Role.FriendlyName(Role.MR_ROLE_POOL_ADMIN));

                using (var dlg = new ErrorDialog(msg))
                    dlg.ShowDialog(Parent);

                return;
            }

            MainWindowCommandInterface.ShowForm(typeof(InstallCertificateDialog), new object[] {host});
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            if (!selection.AllItemsAre<Host>() || selection.Count > 1)
                return false;

            var host = selection.AsXenObjects<Host>().FirstOrDefault();
            if (host == null)
                return false;

            if (!Helpers.StockholmOrGreater(host))
                return false;

            var pool = Helpers.GetPoolOfOne(host.Connection);
            return pool != null && !pool.ha_enabled;
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            if (item is Host host)
            {
                var pool = Helpers.GetPoolOfOne(host.Connection);
                if (pool != null && pool.ha_enabled)
                    return Messages.INSTALL_SERVER_CERTIFICATE_HA;
            }
            
            return base.GetCantExecuteReasonCore(item);
        }

        public override string ContextMenuText => Messages.INSTALL_SERVER_CERTIFICATE_CONTEXT_MENU;

        public override string MenuText => Messages.INSTALL_SERVER_CERTIFICATE_MENU;
    }
}
