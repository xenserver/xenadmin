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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Wizards.NewNetworkWizard_Pages;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Actions;
using XenAdmin.Controls;

namespace XenAdmin.Wizards
{
    public enum NetworkTypes
    {
        Internal,  // old-style (host-only) private networks
        CHIN,      // cross-host internal (or private) networks
        External,
        Bonded
    }

    public partial class NewNetworkWizard : XenWizardBase
    {
        private readonly NetWTypeSelect pageNetworkType;
        private readonly NetWName pageName;
        private readonly NetWDetails pageNetworkDetails;
        private readonly NetWBondDetails pageBondDetails;
        private readonly NetWChinDetails pageChinDetails;

        /// <summary>
        /// May be null.
        /// </summary>
        private readonly Pool Pool;
        private readonly Host Host;
        private NetworkTypes? m_networkType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pool">May be null.</param>
        /// <param name="host">Never null. In the case of a pool, "host" is set to the master.</param>
        public NewNetworkWizard(Network.IXenConnection connection, Pool pool, Host host)
            : base(connection)
        {
            InitializeComponent();

            pageNetworkType = new NetWTypeSelect();
            pageName = new NetWName();
            pageNetworkDetails = new NetWDetails();
            pageBondDetails = new NetWBondDetails();
            pageChinDetails = new NetWChinDetails();

            System.Diagnostics.Trace.Assert(host != null);
            Pool = pool;
            Host = host;

            if (Pool != null)
                pageBondDetails.SetPool(Pool);
            else
                pageBondDetails.SetHost(Host);

            pageNetworkDetails.Host = host;
            pageChinDetails.Host = host;
            pageChinDetails.Pool = pool;

            AddPage(pageNetworkType);
            AddPage(new XenTabPage { Text = "" });
        }

        protected override void OnShown(EventArgs e)
        {
            Text = string.Format(Messages.NEWNETWORKWIZARD_TITLE, Helpers.GetName(xenConnection));
            base.OnShown(e);
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(NetWTypeSelect))
            {
                var oldNetworkType = m_networkType;
                m_networkType = pageNetworkType.SelectedNetworkType;//value non-null, so safe to cast further below

                RemovePagesFrom(1);

                if (m_networkType == NetworkTypes.Bonded)
                {
                    AddPage(pageBondDetails);
                }
                else
                {
                    AddPage(pageName);
                    pageName.SelectedNetworkType = (NetworkTypes)m_networkType;

                    if (oldNetworkType != m_networkType)
                        NotifyNextPagesOfChange(pageName);

                    if (m_networkType == NetworkTypes.CHIN)
                        AddPage(pageChinDetails);
                    else
                    {
                        AddPage(pageNetworkDetails);
                        pageNetworkDetails.SelectedNetworkType = (NetworkTypes)m_networkType;

                        if (oldNetworkType != m_networkType)
                            NotifyNextPagesOfChange(pageNetworkDetails);
                    }
                }
            }
        }

        protected override void FinishWizard()
        {
            NetworkTypes network_type = pageNetworkType.SelectedNetworkType;

            if (network_type == NetworkTypes.Bonded)
            {
                if (!CreateBonded())
                {
                    FinishCanceled();
                    return;
                }
            }
            else if (network_type == NetworkTypes.CHIN)
            {
                CreateCHIN();
            }
            else
            {
                CreateNonBonded();
            }

            base.FinishWizard();
        }

        private bool CreateBonded()
        {
            BondDetails details = pageBondDetails.Details;

            if (details.ShowCreationWarning() == DialogResult.OK)
            {
                new CreateBondAction(details.Connection, details.BondName, details.BondedPIFs, details.AutoPlug, details.MTU, details.BondMode, details.HashingAlgorithm).RunAsync();
                return true;
            }
            
            return false;
        }

        private void CreateCHIN()
        {
            XenAPI.Network network = PopulateNewNetworkObj();
            XenAPI.Network theInterface = pageChinDetails.SelectedInterface;
            (new CreateChinAction(xenConnection, network, theInterface)).RunAsync();
        }

        private void CreateNonBonded()
        {
            XenAPI.Network network = PopulateNewNetworkObj();
            PIF nic = pageNetworkDetails.SelectedHostNic;
            long vlan = pageNetworkDetails.VLAN;

            NetworkAction action = pageNetworkType.SelectedNetworkType == NetworkTypes.External
                                       ? new NetworkAction(xenConnection, network, nic, vlan)
                                       : new NetworkAction(xenConnection, network, true);
            action.RunAsync();
        }

        private XenAPI.Network PopulateNewNetworkObj()
        {
            XenAPI.Network result = new XenAPI.Network();
            result.name_label = pageName.NetworkName;
            result.name_description = pageName.NetworkDescription;
            result.AutoPlug = (pageNetworkType.SelectedNetworkType == NetworkTypes.CHIN ? pageChinDetails.isAutomaticAddNicToVM : pageNetworkDetails.isAutomaticAddNicToVM);
            if (pageNetworkType.SelectedNetworkType == NetworkTypes.CHIN)
                result.MTU = pageChinDetails.MTU;
            else if (pageNetworkDetails.MTU.HasValue) //Custom MTU may not be allowed if we are making a virtual network or something
                result.MTU = pageNetworkDetails.MTU.Value;
            return result;
        }

        protected override string WizardPaneHelpID()
        {
            return "NewNetworkWizard";
        }
    }
}
