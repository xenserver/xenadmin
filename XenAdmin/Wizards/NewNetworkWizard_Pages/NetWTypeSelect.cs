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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWTypeSelect : XenTabPage
    {
        public NetWTypeSelect()
        {
            InitializeComponent();
            this.rbtnExternalNetwork.Checked = true;
        }

        public override string Text { get { return Messages.NETW_TYPE_SELECT_TEXT; } }

        public override string PageTitle { get { return Messages.NETW_TYPE_SELECT_TITLE; } }

        public override void PopulatePage()
        {
            Update(Connection);
        }

        public DeprecationBanner Banner { set; private get; }

        public NetworkTypes SelectedNetworkType
        {
            get
            {
                return rbtnBondedNetwork.Checked
                           ? NetworkTypes.Bonded
                           : rbtnExternalNetwork.Checked
                                 ? NetworkTypes.External
                                 : rbtnCHIN.Checked
                                       ? NetworkTypes.CHIN
                                       : NetworkTypes.Internal;
            }
        }

        private void Update(IXenConnection connection)
        {
            Host master = Helpers.GetMaster(connection);
            if (master == null)
                return;

            if (master.RestrictVLAN)
            {
                rbtnExternalNetwork.Checked = false;
                panelExternal.Enabled = false;

                toolTipContainerExternal.SetToolTip(Messages.EXTERNAL_NETWORKS_REQUIRE_STANDARD);

                rbtnInternalNetwork.Checked = true;
            }
            else
            {
                RemoveFromToolTip(toolTipContainerExternal, panelExternal);
            }

            Pool pool = Helpers.GetPoolOfOne(connection);

            if (!pool.vSwitchController)
            {
                rbtnCHIN.Checked = false;
                panelCHIN.Enabled = false;

                toolTipContainerCHIN.SetToolTip(
                    !Helpers.CowleyOrGreater(connection) || Helpers.FeatureForbidden(connection, Host.RestrictVSwitchController) ?
                    string.Format(Messages.FEATURE_NOT_AVAILABLE_NEED_COWLEY_ENTERPRISE_OR_PLATINUM_PLURAL, Messages.CHINS) :
                    Messages.CHINS_NEED_VSWITCHCONTROLLER);

                if (master.RestrictVLAN)
                    rbtnInternalNetwork.Checked = true;
                else
                    rbtnExternalNetwork.Checked = true;
            }
            else
            {
                RemoveFromToolTip(toolTipContainerCHIN, panelCHIN);
            }
        }

        public void SetDeprecationBanner(bool visible)
        {
            if(Banner != null)
            {
                Banner.AppliesToVersion = Messages.XENSERVER_6_2;
                Banner.BannerType = DeprecationBanner.Type.Deprecation;
                Banner.FeatureName = Messages.DVSCS;
                Banner.LinkUri = new Uri(InvisibleMessages.DVSC_DEPRECATION_URL);
                Banner.Visible = visible;
            }
        }

        private void RemoveFromToolTip(ToolTipContainer container, Panel panel)
        {
            // We have to remove the controls from the panel (rather than just
            // the panel from the container) in order to make all the radio buttons
            // on the page into one group.
            List<Control> controls = new List<Control>();  // need to make a copy because we're about to mess with it
            foreach (Control control in panel.Controls)
                controls.Add(control);
            foreach (Control control in controls)
            {
                Point location = container.Location;
                location.Offset(control.Location);

                panel.Controls.Remove(control);
                control.Visible = true;
                control.Dock = DockStyle.None;

                Controls.Add(control);
                control.Location = location;
            }

            Controls.Remove(container);
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            Pool pool = Helpers.GetPoolOfOne(Connection);
            SetDeprecationBanner(false);
            base.PageLoaded(direction);
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            SetDeprecationBanner(false);
            base.PageLeave(direction, ref cancel);
        }
    }
}
