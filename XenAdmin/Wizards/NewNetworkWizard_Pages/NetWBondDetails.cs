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
using XenAdmin.Controls;
using XenAPI;


namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWBondDetails : XenTabPage
    {
        public NetWBondDetails()
        {
            InitializeComponent();
        }

        #region XentabPage overrides

        public override string Text => Messages.NETW_BOND_DETAILS_TEXT;

        public override string PageTitle => Messages.NETW_BOND_DETAILS_TITLE;

        public override bool EnableNext()
        {
            return Details.Valid;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
                cancel = !Details.CanCreateBond();
        }

        #endregion

        internal string BondName => Details.BondName;
        internal List<PIF> BondedPIFs => Details.BondedPIFs;
        internal bond_mode BondMode => Details.BondMode;
        internal Bond.hashing_algoritm HashingAlgorithm => Details.HashingAlgorithm;
        internal long MTU => Details.MTU;
        internal bool AutoPlug => Details.AutoPlug;

        internal void SetPool(Pool pool)
        {
            Connection = pool.Connection;
            Details.SetPool(pool);
            OnPageUpdated();
        }

        internal void SetHost(Host host)
        {
            Connection = host.Connection;
            Details.SetHost(host);
            OnPageUpdated();
        }

        private void Details_ValidChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
