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


using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    class EnableTlsVerificationCommand : Command
    {
        private readonly bool _confirm = true;

        public EnableTlsVerificationCommand()
        {
            
        }

        public EnableTlsVerificationCommand(IMainWindow window, Pool pool, bool confirm = true)
            : base(window, pool)
        {
            _confirm = confirm;
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var connection = selection.GetConnectionOfFirstItem();

            if (connection != null && !connection.Session.IsLocalSuperuser && !Registry.DontSudo &&
                connection.Session.Roles.All(r => r.name_label != Role.MR_ROLE_POOL_ADMIN))
            {
                var currentRoles = connection.Session.Roles;
                currentRoles.Sort();

                var msg = string.Format(Messages.ENABLE_TLS_VERIFICATION_RBAC_RESTRICTION, currentRoles[0].FriendlyName(),
                    Role.FriendlyName(Role.MR_ROLE_POOL_ADMIN));

                using (var dlg = new ErrorDialog(msg))
                    dlg.ShowDialog(Parent);

                return;
            }

            var pool = Helpers.GetPoolOfOne(connection);

            if (_confirm)
            {
                var msg = $"{Messages.MESSAGEBOX_ENABLE_TLS_VERIFICATION_WARNING}\n\n{Messages.CONFIRM_CONTINUE}";

                using (var dlg = new WarningDialog(msg,
                    new ThreeButtonDialog.TBDButton(Messages.MESSAGEBOX_ENABLE_TLS_VERIFICATION_BUTTON,
                        DialogResult.Yes, ThreeButtonDialog.ButtonType.ACCEPT, true),
                    ThreeButtonDialog.ButtonNo))
                    if (dlg.ShowDialog(Parent) != DialogResult.Yes)
                        return;
            }

            new DelegatedAsyncAction(connection,
                string.Format(Messages.ACTION_ENABLING_TLS_VERIFICATION_ON, Helpers.GetName(pool)),
                Messages.ACTION_ENABLING_TLS_VERIFICATION, Messages.COMPLETED,
                Pool.enable_tls_verification, "pool.enable_tls_verification").RunAsync();
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            if (selection == null || selection.Count != 1 ||
                selection.Any(i => !(i.XenObject is Host) && !(i.XenObject is Pool)))
                return false;

            var conn = selection.GetConnectionOfAllItems();
            if (conn == null || !Helpers.Post82X(conn) ||
                !Helpers.XapiEqualOrGreater_1_290_0(conn) ||
                conn.Cache.Hosts.Any(Host.RestrictCertificateVerification))
                return false;

            var pool = Helpers.GetPoolOfOne(conn);
            return pool != null &&
                   !pool.tls_verification_enabled &&
                   !pool.ha_enabled &&
                   !pool.current_operations.Values.Contains(pool_allowed_operations.ha_enable) &&
                   !pool.current_operations.Values.Contains(pool_allowed_operations.ha_disable) &&
                   !pool.current_operations.Values.Contains(pool_allowed_operations.cluster_create) &&
                   !pool.current_operations.Values.Contains(pool_allowed_operations.designate_new_master) &&
                   !pool.RollingUpgrade();
        }

        protected override string GetCantRunReasonCore(IXenObject item)
        {
            var pool = item == null ? null : Helpers.GetPoolOfOne(item.Connection);
            
            if (pool != null)
            {
                if (pool.ha_enabled)
                    return Messages.ENABLE_TLS_VERIFICATION_HA_ENABLED;

                if (pool.current_operations.Values.Contains(pool_allowed_operations.ha_enable))
                    return Messages.ENABLE_TLS_VERIFICATION_HA_ENABLING;

                if (pool.current_operations.Values.Contains(pool_allowed_operations.ha_disable))
                    return Messages.ENABLE_TLS_VERIFICATION_HA_DISABLING;

                if (pool.current_operations.Values.Contains(pool_allowed_operations.cluster_create))
                    return Messages.ENABLE_TLS_VERIFICATION_CLUSTERING;
            
                if (pool.current_operations.Values.Contains(pool_allowed_operations.designate_new_master))
                    return Messages.ENABLE_TLS_VERIFICATION_NEW_COORDINATOR;

                if (pool.RollingUpgrade())
                    return Messages.ENABLE_TLS_VERIFICATION_RPU;
            }

            return base.GetCantRunReasonCore(item);
        }

        public override string MenuText => Messages.ENABLE_TLS_VERIFICATION_MENU;
    }
}
