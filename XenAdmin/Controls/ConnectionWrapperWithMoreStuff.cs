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
using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAdmin.Core;

namespace XenAdmin.Controls
{
    public class ConnectionWrapperWithMoreStuff : CustomTreeNode, IComparable<ConnectionWrapperWithMoreStuff>
    {
        public IXenConnection Connection, masterConnection;
        PoolJoinRules.Reason reason;

        public ConnectionWrapperWithMoreStuff(IXenConnection connection)
        {
            Connection = connection;
            Refresh();
        }

        public ConnectionWrapperWithMoreStuff TheMaster
        {
            set
            {
                masterConnection = (value == null ? null : value.Connection);
            }
        }

        public bool WillBeMaster
        {
            get
            {
                return reason == PoolJoinRules.Reason.WillBeMaster;
            }
        }

        public bool CanBeMaster
        {
            get
            {
                return reason != PoolJoinRules.Reason.Connecting
                    && reason != PoolJoinRules.Reason.NotConnected
                    && reason != PoolJoinRules.Reason.LicenseRestriction
                    && reason != PoolJoinRules.Reason.IsAPool;
            }
        }

        public bool AllowedAsSlave
        {
            get
            {
                return reason == PoolJoinRules.Reason.Allowed;
            }
        }

        public override string ToString()
        {
            string name = Helpers.GetName(Helpers.GetMaster(Connection));
            if (name == "")
                return Connection.Hostname;
            else
                return name;
        }

        public int CompareTo(ConnectionWrapperWithMoreStuff other)
        {
            return base.CompareTo(other);
        }

        protected override int SameLevelSortOrder(CustomTreeNode other)
        {
            if (!(other is ConnectionWrapperWithMoreStuff))
                return -1;
            ConnectionWrapperWithMoreStuff other2 = other as ConnectionWrapperWithMoreStuff;
            int diff = (int)(this.reason) - (int)(other2.reason);
            if (diff < 0)
                return -1;
            else if (diff > 0)
                return 1;
            else
                return StringUtility.NaturalCompare(this.ToString(), other2.ToString());
        }

        internal void Refresh()
        {
            reason = PoolJoinRules.CanJoinPool(Connection, masterConnection, true, true, true);
            this.Description = PoolJoinRules.ReasonMessage(reason);
            this.Enabled = (reason == PoolJoinRules.Reason.Allowed);
            this.CheckedIfdisabled = (reason == PoolJoinRules.Reason.WillBeMaster);
            if (reason == PoolJoinRules.Reason.WillBeMaster)
                this.State = CheckState.Checked;
        }
    }
}