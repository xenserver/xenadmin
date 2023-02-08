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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    class RotatePoolSecretCommand : Command
    {
        protected override void RunCore(SelectedItemCollection selection)
        {
            var connection = selection.GetConnectionOfFirstItem();

            if (connection != null && !connection.Session.IsLocalSuperuser && !Registry.DontSudo &&
                connection.Session.Roles.All(r => r.name_label != Role.MR_ROLE_POOL_ADMIN))
            {
                var currentRoles = connection.Session.Roles;
                currentRoles.Sort();

                var msg = string.Format(Messages.ROTATE_POOL_SECRET_RBAC_RESTRICTION, currentRoles[0].FriendlyName(),
                    Role.FriendlyName(Role.MR_ROLE_POOL_ADMIN));

                using (var dlg = new ErrorDialog(msg))
                    dlg.ShowDialog(Parent);

                return;
            }

            if (Properties.Settings.Default.RemindChangePassword)
                using (var dlg = new InformationDialog(Messages.ROTATE_POOL_SECRET_REMIND_CHANGE_PASSWORD)
                    {ShowCheckbox = true, CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE})
                {
                    dlg.ShowDialog(Parent);
                    Properties.Settings.Default.RemindChangePassword = !dlg.IsCheckBoxChecked;
                }

            new DelegatedAsyncAction(connection, Messages.ROTATE_POOL_SECRET_TITLE,
                Messages.ROTATE_POOL_SECRET_TITLE, Messages.COMPLETED,
                Pool.rotate_secret, "pool.rotate_secret").RunAsync();
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            if (selection.Any(i => !(i.XenObject is Host) && !(i.XenObject is Pool)))
                return false;

            var conn = selection.GetConnectionOfAllItems();
            if (conn == null || !Helpers.StockholmOrGreater(conn) || conn.Cache.Hosts.Any(Host.RestrictPoolSecretRotation))
                return false;

            var pool = Helpers.GetPoolOfOne(conn);
            return pool != null && !pool.ha_enabled && !pool.RollingUpgrade();
        }

        protected override string GetCantRunReasonCore(IXenObject item)
        {
            var pool = item == null ? null : Helpers.GetPoolOfOne(item.Connection);
            
            if (pool != null)
            {
                if (pool.ha_enabled)
                    return Messages.ROTATE_POOL_SECRET_HA;
                
                if (pool.RollingUpgrade())
                    return Messages.ROTATE_POOL_SECRET_RPU;
            }

            return base.GetCantRunReasonCore(item);
        }

        public override string MenuText => Messages.ROTATE_POOL_SECRET_MENU;
    }
}
