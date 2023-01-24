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
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWTypeSelect : XenTabPage
    {
        public NetWTypeSelect()
        {
            InitializeComponent();
            rbtnExternalNetwork.Checked = true;
        }

        public override string Text => Messages.NETW_TYPE_SELECT_TEXT;

        public override string PageTitle => Messages.NETW_TYPE_SELECT_TITLE;

        public NetworkTypes SelectedNetworkType =>
            rbtnBondedNetwork.Checked
                ? NetworkTypes.Bonded
                : rbtnExternalNetwork.Checked
                    ? NetworkTypes.External
                    : rbtnCHIN.Checked
                        ? NetworkTypes.CHIN
                        : rbtnSriov.Checked
                            ? NetworkTypes.SRIOV
                            : NetworkTypes.Internal;


        public override void PopulatePage()
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                return;

            if (HiddenFeatures.CrossServerPrivateNetworkHidden || Helpers.StockholmOrGreater(Connection))
            {
                rbtnCHIN.Visible = labelCHIN.Visible = false;
                warningTableChin.Visible = false;
            }
            else if (!pool.vSwitchController())
            {
                rbtnCHIN.Visible = labelCHIN.Visible = true;
                rbtnCHIN.Enabled = labelCHIN.Enabled = false;

                labelWarningChinOption.Text = Helpers.FeatureForbidden(Connection, Host.RestrictVSwitchController)
                    ? string.Format(Messages.FEATURE_DISABLED, Messages.CHIN)
                    : Messages.CHINS_NEED_VSWITCHCONTROLLER;

                warningTableChin.Visible = true;
            }
            else
            {
                rbtnCHIN.Visible = labelCHIN.Visible = true;
                rbtnCHIN.Enabled = labelCHIN.Enabled = true;
                warningTableChin.Visible = false;
            }

            bool hasNicCanEnableSriov = pool.Connection.Cache.PIFs.Any(pif => pif.IsPhysical() && pif.SriovCapable() && !pif.IsSriovPhysicalPIF());
            bool sriovFeatureForbidden = Helpers.FeatureForbidden(Connection, Host.RestrictSriovNetwork);

            if (!Helpers.KolkataOrGreater(pool.Connection))
            {
                rbtnSriov.Visible = labelSriov.Visible = false;
                warningTableSriov.Visible = false;
            }
            else if (Helpers.FeatureForbidden(pool.Connection, Host.SriovNetworkDisabled) ||
                     sriovFeatureForbidden || !pool.HasSriovNic() || !hasNicCanEnableSriov)
            {
                rbtnSriov.Visible = labelSriov.Visible = true;
                rbtnSriov.Enabled = labelSriov.Enabled = false;

                labelWarningSriovOption.Text =
                    Helpers.FeatureForbidden(pool.Connection, Host.SriovNetworkDisabled)
                        ? string.Format(Messages.FEATURE_EXPERIMENTAL, Messages.NETWORK_SRIOV)
                        : sriovFeatureForbidden
                            ? string.Format(Messages.FEATURE_DISABLED, Messages.NETWORK_SRIOV)
                            : pool.HasSriovNic()
                                ? Messages.NICS_ARE_SRIOV_ENABLED
                                : Messages.SRIOV_NEED_NICSUPPORT;

                warningTableSriov.Visible = true;
            }
            else
            {
                rbtnSriov.Visible = labelSriov.Visible = true;
                rbtnSriov.Enabled = labelSriov.Enabled = true;
                warningTableSriov.Visible = false;
            }
        }
    }
}
