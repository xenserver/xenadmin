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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Commands
{
    class DestroyBondCommand:Command
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DestroyBondCommand(IMainWindow mainWindow, XenAPI.Network network)
            : base(mainWindow, network)
        {
            
        }
        private static List<string> GetAllNetworkNames(IXenConnection connection)
        {
            List<string> result = new List<string>();
            foreach (XenAPI.Network network in connection.Cache.Networks)
            {
                result.Add(network.Name);
            }
            return result;
        }

        protected sealed override void ExecuteCore(SelectedItemCollection selection)
        {
            //It only supports one item selected for now
            Trace.Assert(selection.Count==1);

            XenAPI.Network network = (XenAPI.Network) selection.FirstAsXenObject;
            List<PIF> pifs = network.Connection.ResolveAll(network.PIFs);
            if (pifs.Count == 0)
            {
                // Should never happen as long as the caller is enabling the button correctly, but
                // it's possible in a tiny window across disconnecting.
                log.Error("Network has no PIFs");
                return;
            }

            // We just want one, so that we can name it.
            PIF pif = pifs[0];

            string msg = string.Format(Messages.DELETE_BOND_MESSAGE, pif.Name);

            bool will_disturb_primary = NetworkingHelper.ContainsPrimaryManagement(pifs);
            bool will_disturb_secondary = NetworkingHelper.ContainsSecondaryManagement(pifs);

            if (will_disturb_primary)
            {
                Pool pool = Helpers.GetPool(network.Connection);
                if (pool != null && pool.ha_enabled)
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Error,
                            string.Format(Messages.BOND_DELETE_HA_ENABLED, pif.Name, pool.Name),
                            Messages.DELETE_BOND)))
                    {
                        dlg.ShowDialog(Parent);
                    }
                    return;
                }

                string message = string.Format(will_disturb_secondary ? Messages.BOND_DELETE_WILL_DISTURB_BOTH : Messages.BOND_DELETE_WILL_DISTURB_PRIMARY, msg);

                DialogResult result;
                using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, message, Messages.DELETE_BOND),
                            "NetworkingConfigWarning",
                            new ThreeButtonDialog.TBDButton(Messages.BOND_DELETE_CONTINUE, DialogResult.OK),
                            ThreeButtonDialog.ButtonCancel))
                {
                    result = dlg.ShowDialog(Parent);
                }
                if (DialogResult.OK != result)
                    return;
            }
            else if (will_disturb_secondary)
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, string.Format(Messages.BOND_DELETE_WILL_DISTURB_SECONDARY, msg), Messages.XENCENTER),
                        ThreeButtonDialog.ButtonOK,
                        ThreeButtonDialog.ButtonCancel))
                {
                    dialogResult = dlg.ShowDialog(Parent);
                }
                if (DialogResult.OK != dialogResult)
                    return;
            }
            else
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, msg, Messages.XENCENTER),
                        new ThreeButtonDialog.TBDButton(Messages.OK, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT, true),
                        ThreeButtonDialog.ButtonCancel))
                {
                    dialogResult = dlg.ShowDialog(Parent);
                }
                if (DialogResult.OK != dialogResult)
                    return;
            }

            // The UI shouldn't offer deleting a bond in this case, but let's make sure we've
            // done the right thing and that the bond hasn't been deleted in the meantime. (CA-27436).
            Bond bond = pif.BondMasterOf;
            if (bond != null)
                new Actions.DestroyBondAction(bond).RunAsync();
        }
    }
}
