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
using System.Net;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class DestroyBondAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// All bonds equivalent to the one given in the constructor (i.e. one per host).
        /// </summary>
        private readonly List<Bond> Bonds = new List<Bond>();

        /// <summary>
        /// The interfaces of all the bonds in Bonds.
        /// </summary>
        private readonly List<PIF> Interfaces = new List<PIF>();

        /// <summary>
        /// All members under each bond in Bonds.  These will be plugged at the end, in order to
        /// get metrics like the carrier flag, if they haven't already been brought up as a
        /// management interface.
        /// </summary>
        private readonly List<PIF> Members = new List<PIF>();

        /// <summary>
        /// The first member (ordered by name) under each interface.
        /// </summary>
        private readonly Dictionary<PIF, PIF> FirstMembers = new Dictionary<PIF, PIF>();

        /// <summary>
        /// The network that we're either going to destroy or rename.
        /// </summary>
        private readonly XenAPI.Network Network;

        /// <summary>
        /// The bond's name.
        /// </summary>
        private readonly string Name;

        public DestroyBondAction(Bond bond)
            : base(bond.Connection, string.Format(Messages.ACTION_DESTROY_BOND_TITLE, bond.Name()),
                   string.Format(Messages.ACTION_DESTROY_BOND_DESCRIPTION, bond.Name()))
        {
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("host.management_reconfigure");
            ApiMethodsToRoleCheck.Add("network.destroy");
            ApiMethodsToRoleCheck.Add("vif.plug");
            ApiMethodsToRoleCheck.Add("vif.unplug");
            ApiMethodsToRoleCheck.Add("pif.reconfigure_ip");
            ApiMethodsToRoleCheck.Add("pif.plug");
            ApiMethodsToRoleCheck.Add("bond.destroy");
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(XenAPI.Role.CommonTaskApiList);
            #endregion

            Name = bond.Name();

            Pool = Helpers.GetPoolOfOne(Connection);

            foreach (Host host in Connection.Cache.Hosts)
            {
                Bond b = NetworkingHelper.FindBond(host, bond);
                if (b != null)
                {
                    Bonds.Add(b);

                    b.Locked = true;

                    PIF bondInterface = Connection.Resolve(b.master);
                    if (bondInterface != null)
                    {
                        Interfaces.Add(bondInterface);
                        bondInterface.Locked = true;

                        List<PIF> members = Connection.ResolveAll(b.slaves);
                        NetworkingHelper.Sort(members);
                        foreach (PIF pif in members)
                        {
                            Members.Add(pif);
                            pif.Locked = true;
                        }

                        FirstMembers[bondInterface] = Connection.Resolve(b.primary_slave);

                        if (!FirstMembers.ContainsKey(bondInterface) && members.Count != 0)
                            FirstMembers[bondInterface] = members[0];
                    }

                    AppliesTo.Add(host.opaque_ref);
                }
            }

            PIF coordinator_bond_interface = Connection.Resolve(bond.master);
            if (coordinator_bond_interface != null)
            {
                Network = Connection.Resolve(coordinator_bond_interface.network);
                Network.Locked = true;
            }
        }

        protected override void Run()
        {
            PercentComplete = 0;
            Connection.ExpectDisruption = true;

            int incr = Interfaces.Count > 0 ? 50 / Interfaces.Count : 0;

            try
            {
                foreach (PIF bondInterface in Interfaces)
                {
                    NetworkingActionHelpers.MoveManagementInterfaceName(this, bondInterface, FirstMembers[bondInterface]);
                    PercentComplete += incr;
                }
            }
            catch (WebException we)
            {
                //ignore keep-alive failure since disruption is expected
                if (we.Status != WebExceptionStatus.KeepAliveFailure)
                    throw;
            }

            PercentComplete = 50;

            var caughtExceptions = new List<Exception>();

            int inc = Bonds.Count > 0 ? 40 / Bonds.Count : 0;

            foreach (Bond bond in Bonds)
            {
                Bond bond1 = bond;
                try
                {
                    RelatedTask = Bond.async_destroy(Session, bond1.opaque_ref);
                }
                catch (Exception exn)
                {
                    if (Connection != null && Connection.ExpectDisruption &&
                        exn is WebException webEx && webEx.Status == WebExceptionStatus.KeepAliveFailure)
                    {
                        //ignore
                    }
                    else
                    {
                        log.Error($"Failed to destroy bond {bond1.opaque_ref}", exn);
                        caughtExceptions.Add(exn);
                    }
                }
                PollToCompletion(PercentComplete, PercentComplete + inc);
            }

            PercentComplete = 90;

            if (Network != null)
            {
                string oldNetworkName = Network.Name();

                log.DebugFormat("Destroying network {0} ({1})...", oldNetworkName, Network.uuid);

                try
                {
                    XenAPI.Network.destroy(Session, Network.opaque_ref);
                    log.DebugFormat("Network {0} ({1}) destroyed.", oldNetworkName, Network.uuid);
                }
                catch (Exception exn)
                {
                    if (Connection != null && Connection.ExpectDisruption &&
                        exn is WebException webEx && webEx.Status == WebExceptionStatus.KeepAliveFailure)
                    {
                        //ignore
                    }
                    else
                    {
                        log.Error($"Failed to destroy bond {Network.opaque_ref}", exn);
                        caughtExceptions.Add(exn);
                    }
                }
            }

            if (caughtExceptions.Count > 0)
                throw caughtExceptions[0];

            Description = string.Format(Messages.ACTION_DESTROY_BOND_DONE, Name);
        }

        private void UnlockAll()
        {
            foreach (Bond bond in Bonds)
                bond.Locked = false;

            foreach (PIF bondInterface in Interfaces)
                bondInterface.Locked = false;

            foreach (PIF pif in Members)
                pif.Locked = false;

            if (Network != null)
                Network.Locked = false;
        }

        
        protected override void Clean()
        {
            Connection.ExpectDisruption = false;
            UnlockAll();
        }

    }
}
