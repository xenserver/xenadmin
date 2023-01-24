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
using XenAdmin.Core;
using XenAdmin.XCM;
using XenAdmin.Wizards.GenericPages;


namespace XenAdmin.Wizards.ConversionWizard
{
    class ConversionNetworkPage : SelectMultipleVMNetworkPage
    {
        public ConversionClient ConversionClient { private get; set; }
        public ServerInfo VmwareCredInfo { private get; set; }

        private NetworkInstance[] VmwareNetworks;

        public override string Text => Messages.CONVERSION_NETWORK_PAGE_TEXT;

        public override string PageTitle => String.Format(Messages.CONVERSION_NETWORK_PAGE_TITLE, BrandManager.ProductBrand);

        public override string HelpID => "NetworkOptions";

        public override bool EnableNext()
        {
            return true;
        }

        protected override bool LoadsRemoteData => true;
        protected override string NetworkColumnHeaderText => Messages.CONVERSION_NETWORK_PAGE_COLUMN_HEADER;
        protected override bool ShowReserveMacAddressesCheckBox => true;
        protected override bool ShowRefreshButton => true;

        protected override string IntroductionText => Messages.CONVERSION_NETWORK_PAGE_BLURB;
        protected override string TableIntroductionText => string.Empty;

        protected override NetworkResourceContainer NetworkData(string sysId)
        {
            return new ConversionNetworkResourceContainer(VmwareNetworks);
        }

        protected override void LoadNetworkData()
        {
            VmwareNetworks = ConversionClient.GetNetworks(VmwareCredInfo);
        }

        protected override void FillTableRows()
        {
            FillTableRow(null, null, null);
        }
    }

    class ConversionNetworkResource : INetworkResource
    {
        public ConversionNetworkResource(NetworkInstance network)
        {
            NetworkName = network.Name;
            NetworkID = network.Id;
        }

        public string VmNameOverride  => null;
        public string NetworkName { get; }
        public string MACAddress  => null;
        public string NetworkID { get; }
    }

    class ConversionNetworkResourceContainer : NetworkResourceContainer
    {
        private int _counter;
        private readonly NetworkInstance[] _networks;

        public ConversionNetworkResourceContainer(NetworkInstance[] networks)
        {
            _networks = networks;
        }

        public override INetworkResource Next()
        {
            var res = new ConversionNetworkResource(_networks[_counter]);
            _counter++;
            return res;
        }

        public override bool IsNext => _counter < _networks.Length;
    }
}
