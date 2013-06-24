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
using System.Threading;
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
        /// The masters of all the bonds in Bonds.
        /// </summary>
        private readonly List<PIF> Masters = new List<PIF>();

        /// <summary>
        /// Masters that held secondary management interfaces.  These are discovered when bringing masters down.
        /// </summary>
        private readonly List<PIF> Secondaries = new List<PIF>();

        /// <summary>
        /// All slaves under each bond in Bonds.  These will be plugged at the end, in order to
        /// get metrics like the carrier flag, if they haven't already been brought up as a
        /// management interface.
        /// </summary>
        private readonly List<PIF> Slaves = new List<PIF>();

        /// <summary>
        /// The first slave (ordered by name) under each master.
        /// </summary>
        private readonly Dictionary<PIF, PIF> FirstSlaves = new Dictionary<PIF, PIF>();

        /// <summary>
        /// A dictionary of copies of the equivalent entries in FirstSlaves, but with the IP details that we're carrying across.
        /// </summary>
        private readonly Dictionary<PIF, PIF> NewFirstSlaves = new Dictionary<PIF, PIF>();

        /// <summary>
        /// The network that we're either going to destroy or rename.
        /// </summary>
        private readonly XenAPI.Network Network;

        /// <summary>
        /// The network's new name.  May be null, in which case the network will be destroyed.
        /// </summary>
        private readonly string NewNetworkName;

        /// <summary>
        /// The bond's name.
        /// </summary>
        private readonly string Name;

        /// <summary>
        /// In Boston, most network configuration is done automatically by xapi (PR-1006)
        /// </summary>
        private readonly bool bostonOrGreater;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bond"></param>
        /// <param name="new_network_name">May be null, in which case the network will be destroyed.</param>
        public DestroyBondAction(Bond bond, string new_network_name)
            : base(bond.Connection, string.Format(Messages.ACTION_DESTROY_BOND_TITLE, bond.Name),
                   string.Format(Messages.ACTION_DESTROY_BOND_DESCRIPTION, bond.Name))
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

            NewNetworkName = new_network_name;
            Name = bond.Name;

            Pool = Helpers.GetPoolOfOne(Connection);
            bostonOrGreater = Helpers.BostonOrGreater(Connection);

            foreach (Host host in Connection.Cache.Hosts)
            {
                Bond b = NetworkingHelper.FindBond(host, bond);
                if (b != null)
                {
                    Bonds.Add(b);

                    b.Locked = true;

                    PIF master = Connection.Resolve(b.master);
                    if (master != null)
                    {
                        Masters.Add(master);
                        master.Locked = true;

                        List<PIF> slaves = Connection.ResolveAll(b.slaves);
                        NetworkingHelper.Sort(slaves);
                        foreach (PIF pif in slaves)
                        {
                            Slaves.Add(pif);
                            pif.Locked = true;
                        }

                        if (bostonOrGreater)
                        {
                            FirstSlaves[master] = Connection.Resolve(b.primary_slave);
                        }

                        if (!FirstSlaves.ContainsKey(master) && slaves.Count != 0)
                            FirstSlaves[master] = slaves[0];
                    }

                    AppliesTo.Add(host.opaque_ref);
                }
            }

            PIF master_master = Connection.Resolve(bond.master);
            if (master_master != null)
            {
                Network = Connection.Resolve(master_master.network);
                Network.Locked = true;
            }
        }

        protected override void Run()
        {
            Connection.ExpectDisruption = true;
            List<VIF> unplugged_vifs = new List<VIF>();
            string old_network_name = Network == null ? "" : Network.Name;
            Exception e = null;

            if (!bostonOrGreater)
            {
                if (Network != null && NewNetworkName != null)
                {
                    // Unplug all active VIFs, because we can't fiddle with the PIFs on the network while the
                    // VIFs are active (xapi won't let us).
                    foreach (VIF vif in Connection.ResolveAll(Network.VIFs))
                    {
                        if (vif.currently_attached)
                        {
                            log.DebugFormat("Unplugging VIF {0} from network {1}...", vif.uuid, old_network_name);
                            VIF.unplug(Session, vif.opaque_ref);
                            unplugged_vifs.Add(vif);
                            log.DebugFormat("VIF {0} unplugged from network {1}.", vif.uuid, old_network_name);
                        }
                    }
                }
            }

            BestEffort(ref e, ReconfigureManagementInterfaces);

            if (e != null)
                throw e;

            PercentComplete = 50;

            int inc = 40 / (Bonds.Count + Masters.Count + Slaves.Count);

            int lo = PercentComplete;
            foreach (Bond bond in Bonds)
            {
                Bond bond1 = bond;
                int lo1 = lo;
                BestEffort(ref e, delegate() { RelatedTask = Bond.async_destroy(Session, bond1.opaque_ref);});
                PollToCompletion(lo1, lo1 + inc);

                lo += inc;
            }

            foreach (PIF master in Secondaries)
            {
                if (!bostonOrGreater)
                    ReconfigureSecondaryManagement(master, PercentComplete + inc);
                PercentComplete += inc;
            }

            if (!bostonOrGreater)
            {
                foreach (PIF pif in Slaves)
                    NetworkingActionHelpers.Plug(this, pif, PercentComplete + inc);
            }

            if (Network != null)
            {
                if (NewNetworkName == null)
                {
                    // We can delete the whole network.
                    log.DebugFormat("Destroying network {0} ({1})...", old_network_name, Network.uuid);
                    BestEffort(ref e, delegate()
                        {
                            XenAPI.Network.destroy(Session, Network.opaque_ref);
                            log.DebugFormat("Network {0} ({1}) destroyed.", old_network_name, Network.uuid);
                        });
                }
                else
                {
                    // Rename the network, so that the VIFs still have somewhere to live.
                    log.DebugFormat("Renaming network {0} ({1}) to {2}...", old_network_name, Network.uuid, NewNetworkName);

                    XenAPI.Network n = (XenAPI.Network)Network.Clone();
                    n.name_label = NewNetworkName;
                    BestEffort(ref e, delegate()
                        {
                            n.SaveChanges(Session);
                            log.DebugFormat("Renaming network {0} ({1}) done.", NewNetworkName, n.uuid);
                        });

                    // Replug all the VIFs that we unplugged before.
                    if (!bostonOrGreater)
                    {
                        foreach (VIF vif in unplugged_vifs)
                        {
                            log.DebugFormat("Replugging VIF {0} into network {1}...", vif.opaque_ref, NewNetworkName);
                            BestEffort(ref e, delegate()
                                {
                                    VIF.plug(Session, vif.opaque_ref);
                                    log.DebugFormat("Replugging VIF {0} into network {1} done.", vif.opaque_ref, NewNetworkName);
                                });
                        }
                    }
                }
            }

            if (e != null)
                throw e;

            Description = string.Format(Messages.ACTION_DESTROY_BOND_DONE, Name);
        }

        private void ReconfigureManagementInterfaces()
        {
            int progress = 0;
            int inc = 50 / Masters.Count;

            foreach (PIF master in Masters)
            {
                progress += inc;
                if (bostonOrGreater)
                {
                    NetworkingActionHelpers.MoveManagementInterfaceName(this, master, FirstSlaves[master]);
                }
                else if (master.management)
                {
                    ReconfigurePrimaryManagement(master, progress);
                }
                else if (master.IsSecondaryManagementInterface(true))
                {
                    DeconfigureSecondaryManagement(master, progress);
                }
            }
        }

        private void ReconfigurePrimaryManagement(PIF master, int hi)
        {
            System.Diagnostics.Trace.Assert(!bostonOrGreater);
            NetworkingActionHelpers.ReconfigureSinglePrimaryManagement(this, master, FirstSlaves[master], hi);
        }

        private void DeconfigureSecondaryManagement(PIF master, int hi)
        {
            System.Diagnostics.Trace.Assert(!bostonOrGreater);
            NewFirstSlaves[master] = NetworkingHelper.CopyIPConfig(master, FirstSlaves[master]);
            NetworkingActionHelpers.BringDown(this, master, hi);
            Secondaries.Add(master);
        }

        private void ReconfigureSecondaryManagement(PIF master, int hi)
        {
            System.Diagnostics.Trace.Assert(!bostonOrGreater);
            NetworkingActionHelpers.BringUp(this, NewFirstSlaves[master], FirstSlaves[master], hi);
        }

        private void UnlockAll()
        {
            foreach (Bond bond in Bonds)
            {
                bond.Locked = false;
            }

            foreach (PIF master in Masters)
            {
                master.Locked = false;
            }

            if (Network != null)
            {
                Network.Locked = false;
            }
        }

        
        protected override void Clean()
        {
            Connection.ExpectDisruption = false;
            UnlockAll();
        }

    }
}
