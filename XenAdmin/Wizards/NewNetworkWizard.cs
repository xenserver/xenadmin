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

using System;
using System.Collections.Generic;
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
        Bonded,
        SRIOV
    }

    public partial class NewNetworkWizard : XenWizardBase
    {
        private readonly NetWTypeSelect pageNetworkType;
        private readonly NetWName pageName;
        private readonly NetWDetails pageNetworkDetails;
        private readonly NetWBondDetails pageBondDetails;
        private readonly NetWChinDetails pageChinDetails;
        private readonly NetWSriovDetails pageSriovDetails;

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
        /// <param name="host">Never null. In the case of a pool, "host" is set to the coordinator.</param>
        public NewNetworkWizard(Network.IXenConnection connection, Pool pool, Host host)
            : base(connection)
        {
            InitializeComponent();

            pageNetworkType = new NetWTypeSelect();
            pageName = new NetWName();
            pageNetworkDetails = new NetWDetails();
            pageBondDetails = new NetWBondDetails();
            pageChinDetails = new NetWChinDetails();
            pageSriovDetails = new NetWSriovDetails();

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
            pageSriovDetails.Host = host;

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
                    else if (m_networkType == NetworkTypes.SRIOV)
                    {
                        AddPage(pageSriovDetails);
                    }
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

            switch (network_type)
            {
                case NetworkTypes.Bonded:
                    CreateBonded();
                    break;
                case NetworkTypes.CHIN:
                    CreateCHIN();
                    break;
                case NetworkTypes.SRIOV:
                    CreateSRIOV();
                    break;
                default:
                    CreateNonBonded();
                    break;
            }

            base.FinishWizard();
        }

        private void CreateBonded()
        {
            new CreateBondAction(pageBondDetails.Connection, pageBondDetails.BondName,
                pageBondDetails.BondedPIFs, pageBondDetails.AutoPlug, pageBondDetails.MTU,
                pageBondDetails.BondMode, pageBondDetails.HashingAlgorithm).RunAsync();
        }

        private void CreateCHIN()
        {
            XenAPI.Network network = CreateNetworkObject();
            XenAPI.Network theInterface = pageChinDetails.SelectedInterface;
            new CreateChinAction(xenConnection, network, theInterface).RunAsync();
        }

        private void CreateSRIOV()
        {
            List<PIF> sriovSelectedPifs = new List<PIF>();
            Pool pool = Helpers.GetPoolOfOne(Host.Connection);
            if (pool == null)
                return;

            if (pageSriovDetails.SelectedHostNic == null)
                return ;

            foreach (PIF thePIF in pool.Connection.Cache.PIFs)
            {
                if (thePIF.IsPhysical() && !thePIF.IsBondNIC() && thePIF.SriovCapable() && thePIF.device == pageSriovDetails.SelectedHostNic.device && !thePIF.IsSriovPhysicalPIF())
                    sriovSelectedPifs.Add(thePIF);
            }

            XenAPI.Network network = CreateNetworkObject();

            if (sriovSelectedPifs.Count != 0)
                new CreateSriovAction(xenConnection, network, sriovSelectedPifs).RunAsync();
        }

        private void CreateNonBonded()
        {
            XenAPI.Network network = CreateNetworkObject();

            PIF nic;
            if (pageNetworkDetails.CreateVlanOnSriovNetwork)
            {
                if (pageNetworkDetails.SelectedHostNic == null || !pageNetworkDetails.SelectedHostNic.IsSriovPhysicalPIF())
                    return;

                var sriovPhysicalPif = xenConnection.Resolve(pageNetworkDetails.SelectedHostNic.sriov_physical_PIF_of[0]);
                if (sriovPhysicalPif == null)
                    return;

                nic = xenConnection.Resolve(sriovPhysicalPif.logical_PIF);
                if (nic == null)
                    return;
            }
            else
            {
                nic = pageNetworkDetails.SelectedHostNic;
            }

            NetworkAction action = pageNetworkType.SelectedNetworkType == NetworkTypes.External
                                       ? new NetworkAction(xenConnection, network, nic, pageNetworkDetails.VLAN)
                                       : new NetworkAction(xenConnection, network, true);
            action.RunAsync();
        }

        private XenAPI.Network CreateNetworkObject()
        {
            XenAPI.Network result = new XenAPI.Network
            {
                name_label = pageName.NetworkName,
                name_description = pageName.NetworkDescription,
                managed = true
            };

            var autoPlug = pageNetworkType.SelectedNetworkType == NetworkTypes.CHIN
                ? pageChinDetails.isAutomaticAddNicToVM
                : pageNetworkType.SelectedNetworkType == NetworkTypes.SRIOV
                    ? pageSriovDetails.AddNicToVmsAutomatically
                    : pageNetworkDetails.AddNicToVmsAutomatically;
            result.SetAutoPlug(autoPlug);
            
            if (pageNetworkType.SelectedNetworkType == NetworkTypes.CHIN)
                result.MTU = pageChinDetails.MTU;
            else
            {
                int mtu = pageNetworkDetails.MTU;
                if (mtu != -1) //Custom MTU may not be allowed if we are making a virtual network or something
                    result.MTU = mtu;
            }

            return result;
        }

        protected override string WizardPaneHelpID()
        {
            return "NewNetworkWizard";
        }
    }
}
