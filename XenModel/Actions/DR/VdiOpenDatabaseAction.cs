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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.DR
{
    public class VdiOpenDatabaseAction : AsyncAction
    {
        private XenRef<Session> _metadataSessionRef;
        public Session MetadataSession;

        private VDI _vdi;

        public VDI Vdi
        {
            get { return _vdi; }
        }

        public XenRef<Session> MetadataSessionRef
        {
            get { return _metadataSessionRef; }
        }

        public VdiOpenDatabaseAction(IXenConnection connection, VDI vdi)
            : base(connection, String.Format(Messages.ACTION_VDI_OPEN_DATABASE_TITLE, connection.Resolve(vdi.SR).Name))
        {
            _vdi = vdi;
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("VDI.open_database");
            ApiMethodsToRoleCheck.Add("Session.get_record");
            #endregion
        }

        protected override void Run()
        {
            Description = String.Format(Messages.ACTION_VDI_OPEN_DATABASE_STATUS, Connection.Resolve(_vdi.SR).Name); 
            _metadataSessionRef = VDI.open_database(Session, _vdi.opaque_ref);
            if (MetadataSessionRef != null)
            {
                MetadataSession = Session.get_record(Session, MetadataSessionRef);
            }
            PercentComplete = 100;
            
            Description = Messages.ACTION_VDI_OPEN_DATABASE_DONE;
        }
    }
}
