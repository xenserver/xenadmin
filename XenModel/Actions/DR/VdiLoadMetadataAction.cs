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
    public class VdiLoadMetadataAction : AsyncAction
    {
        public XenRef<Session> MetadataSessionRef;
        public Session MetadataSession;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PoolMetadata _poolMetadata;
        public PoolMetadata PoolMetadata
        {
            get
            {
                if (_poolMetadata.VmAppliances.Count > 0 || _poolMetadata.Vms.Count > 0)
                {
                    return _poolMetadata;
                }
                return null;
            }
        }

        public VdiLoadMetadataAction(IXenConnection connection, VDI vdi)
            : base(connection, String.Format(Messages.ACTION_VDI_LOAD_METADATA_TITLE, connection.Resolve(vdi.SR).Name))
        {
            _poolMetadata = new PoolMetadata(null, vdi);

            Pool = Helpers.GetPoolOfOne(connection);

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("VDI.open_database");
            ApiMethodsToRoleCheck.Add("Session.get_record");
            ApiMethodsToRoleCheck.Add("Pool.get_all_records");
            ApiMethodsToRoleCheck.Add("VM_appliance.get_all_records");
            ApiMethodsToRoleCheck.Add("VM.get_all_records");
            #endregion
        }

        protected override void Run()
        {
            Description = String.Format(Messages.ACTION_VDI_LOAD_METADATA_STATUS, Connection.Resolve(_poolMetadata.Vdi.SR).Name);
            log.DebugFormat("Loading metadata from VDI '{0}' on SR '{1}'", _poolMetadata.Vdi.Name,
                            Connection.Resolve(_poolMetadata.Vdi.SR).Name);

            MetadataSessionRef = VDI.open_database(Session, _poolMetadata.Vdi.opaque_ref);
            PercentComplete = 30;

            if (MetadataSessionRef != null)
            {
                MetadataSession = null;
                try
                {
                    MetadataSession = Session.get_record(Session, MetadataSessionRef);

                    #region FIND POOL 
                    Dictionary<XenRef<XenAPI.Pool>, XenAPI.Pool> pools = XenAPI.Pool.get_all_records(MetadataSession);
                    foreach (var pool in pools.Values)
                    {
                        _poolMetadata.Pool = pool;
                        string poolName = String.IsNullOrEmpty(pool.name_label) && pool.master != null
                                              ? XenAPI.Host.get_name_label(MetadataSession, pool.master.opaque_ref)
                                              : pool.name_label;
                        _poolMetadata.Pool.name_label = poolName;
                        log.DebugFormat("Found metadata of pool '{0}' (UUID={1})", _poolMetadata.Pool.Name,
                                        _poolMetadata.Pool.uuid);
                        break;
                    }
                    #endregion

                    /*if (_poolMetadata.Pool.uuid == Pool.uuid) // metadata of current pool
                    {
                        return;
                    }*/

                    _poolMetadata.VmAppliances = VM_appliance.get_all_records(MetadataSession);
                    foreach (var vmAppRef in _poolMetadata.VmAppliances.Keys)
                    {
                        _poolMetadata.VmAppliances[vmAppRef].opaque_ref = vmAppRef.opaque_ref;
                    }

                    PercentComplete = 50;

                    Dictionary<XenRef<VM>, VM> vms = VM.get_all_records(MetadataSession);
                    foreach (var vmRef in vms.Keys)
                    {
                        VM vm = vms[vmRef];
                        if (vm.not_a_real_vm)
                            continue;
                        vm.opaque_ref = vmRef.opaque_ref;
                        _poolMetadata.Vms.Add(vmRef, vm);
                    }
                }
                catch (Exception)
                {
                }
            }
            PercentComplete = 100;
            Description = Messages.ACTION_VDI_LOAD_METADATA_DONE;
        }
    }

    public class PoolMetadata : IComparable<PoolMetadata>, IEquatable<PoolMetadata>
    {
        public XenAPI.Pool Pool;
        public VDI Vdi;
        public Dictionary<XenRef<VM_appliance>, VM_appliance> VmAppliances;
        public Dictionary<XenRef<VM>, VM> Vms;

        public PoolMetadata(XenAPI.Pool pool, VDI vdi)
        {
            Pool = pool;
            Vdi = vdi;
            VmAppliances = new Dictionary<XenRef<VM_appliance>, VM_appliance>();
            Vms = new Dictionary<XenRef<VM>, VM>();
        }

        public int CompareTo(PoolMetadata other)
        {
            // Sort by the PoolName
            return StringUtility.NaturalCompare(Pool.Name, other.Pool.Name); 
        }

        public bool Equals(PoolMetadata other)
        {
            return this.Pool.opaque_ref == other.Pool.opaque_ref;
        }
    }
}
