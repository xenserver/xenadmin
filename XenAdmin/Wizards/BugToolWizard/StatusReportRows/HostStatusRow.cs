﻿/* Copyright (c) Citrix Systems, Inc. 
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

using System.Collections.Generic;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.BugToolWizard
{
    partial class BugToolPageRetrieveData
    {
        private class HostStatusRow : StatusReportRow
        {
            private readonly long _size;
            private readonly List<string> _capabilityKeys;
            private readonly Host _host;
            private SingleHostStatusAction _action;

            public HostStatusRow(Host host, long size, List<string> capabilityKeys)
            {
                _host = host;
                _size = size;
                _capabilityKeys = capabilityKeys;
                cellHostImg.Value = Images.GetImage16For(_host);
                cellHost.Value = _host.Name();
            }

            public IXenConnection Connection => _host.Connection;

            public override StatusReportAction Action => _action;

            protected override void CreateAction(string path, string time)
            {
                _action = new SingleHostStatusAction(_host, _size, _capabilityKeys, path, time);
            }
        }
    }
}
