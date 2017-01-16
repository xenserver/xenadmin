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
using System.Text;

using XenAPI;

using XenAdmin.Model;

using XenAdmin.Network;

namespace XenAdmin.Actions
{
    class NetworkingActionHelpers
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static void ReconfigureSinglePrimaryManagement(AsyncAction action, PIF src, PIF dest, int hi)
        {
            PIF new_dest = NetworkingHelper.CopyIPConfig(src, dest);

            // We put an IP address on the destination interface, then we reconfigure the management interface onto it.

            int mid = (hi + action.PercentComplete) / 2;

            BringUp(action, new_dest, new_dest.IP, dest, mid);
            ReconfigureManagement(action, src, new_dest, true, false, hi, true);
        }

        internal static void ReconfigurePrimaryManagement(AsyncAction action, PIF src, PIF dest, int hi)
        {
            PIF new_dest = NetworkingHelper.CopyIPConfig(src, dest);

            // We bring up the slave interfaces on the slave hosts, without plugging them.
            // Then the slave interface on the master host.
            // Then we reconfigure the management interface on the slaves.
            // Then the master.

            int lo = action.PercentComplete;
            int inc = (hi - lo) / 4;
            lo += inc;

            BringUp(action, new_dest, false, lo);
            lo += inc;
            BringUp(action, new_dest, true, lo);
            lo += inc;
            ReconfigureManagement(action, src, new_dest, false, false, lo, true);
            ReconfigureManagement(action, src, new_dest, true, false, hi, true);
        }

        //internal static void DeconfigureSecondaryManagement(AsyncAction action, PIF pif, int hi)
        //{
        //    int mid = (action.PercentComplete + hi) / 2;
        //    BringDown(action, pif, false, mid);
        //    BringDown(action, pif, true, hi);
        //}

        //internal static void ReconfigureSecondaryManagement(AsyncAction action, PIF pif, int hi)
        //{
        //    // We reconfigure the interfaces on the slaves, then the master.
        //    int mid = (action.PercentComplete + hi) / 2;

        //    BringUp(action, pif, false, mid);
        //    BringUp(action, pif, true, hi);
        //}

        private static void BringUp(AsyncAction action, PIF new_pif, bool this_host, int hi)
        {
            ForSomeHosts(action, new_pif, this_host, false, hi,
                delegate(AsyncAction a, PIF p, int h)
                {
                    BringUp(a, new_pif, new_pif.IP, p, h);
                });
        }

        /// <summary>
        /// Configure an IP address, management purpose, and set the disallow_unplug flag on the given pif, then plug it.
        /// </summary>
        internal static void BringUp(AsyncAction action, PIF new_pif, PIF existing_pif, int hi)
        {
            BringUp(action, new_pif, new_pif.IP, existing_pif, hi);
        }

        /// <summary>
        /// Configure an IP address, management purpose, and set the disallow_unplug flag on the given existing_pif.
        /// </summary>
        /// <param name="new_pif">The source of the new IP details</param>
        /// <param name="existing_pif">The PIF to configure</param>
        internal static void BringUp(AsyncAction action, PIF new_pif, string new_ip, PIF existing_pif, int hi)
        {
            bool primary = string.IsNullOrEmpty(new_pif.ManagementPurpose);

            int lo = action.PercentComplete;
            int inc = (hi - lo) / (primary ? 2 : 3);

            log.DebugFormat("Bringing PIF {0} {1} up as {2}/{3}, {4}, {5}...", existing_pif.Name, existing_pif.uuid,
                new_ip, new_pif.netmask, new_pif.gateway, new_pif.DNS);
            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_BRINGING_UP, existing_pif.Name);

            PIF p = (PIF)existing_pif.Clone();
            p.disallow_unplug = !primary;
            p.ManagementPurpose = new_pif.ManagementPurpose;
            p.SaveChanges(action.Session);

            action.PercentComplete = lo + inc;

            ReconfigureIP(action, new_pif, existing_pif, new_ip, action.PercentComplete + inc);
            if (!primary)
                Plug(action, existing_pif, hi);

            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_BRINGING_UP_DONE, existing_pif.Name);
            log.DebugFormat("Brought PIF {0} {1} up.", existing_pif.Name, existing_pif.uuid);
        }

        internal static void ReconfigureIP(AsyncAction action, PIF new_pif, PIF existing_pif, string ip, int hi)
        {
            log.DebugFormat("Reconfiguring IP on {0} {1} ...", existing_pif.Name, existing_pif.uuid);

            action.RelatedTask = PIF.async_reconfigure_ip(action.Session, existing_pif.opaque_ref,
                new_pif.ip_configuration_mode, ip, new_pif.netmask, new_pif.gateway, new_pif.DNS);
            action.PollToCompletion(action.PercentComplete, hi);

            log.DebugFormat("Reconfiguring IP on {0} {1} done.", existing_pif.Name, existing_pif.uuid);
        }
        
        internal static void Plug(AsyncAction action, PIF pif, int hi)
        {
            if (!PIF.get_currently_attached(action.Session, pif.opaque_ref))
            {
                log.DebugFormat("Plugging {0} {1} ...", pif.Name, pif.uuid);
                action.RelatedTask = PIF.async_plug(action.Session, pif.opaque_ref);
                action.PollToCompletion(action.PercentComplete, hi);
                log.DebugFormat("Plugging {0} {1} done.", pif.Name, pif.uuid);
            }
        }

        //private static void BringDown(AsyncAction action, PIF master, bool this_host, int hi)
        //{
        //    ForSomeHosts(action, master, this_host, false, hi, BringDown);
        //}

        /// <summary>
        /// Depurpose (set disallow_unplug=false) and remove the IP address from the given PIF.
        /// </summary>
        internal static void BringDown(AsyncAction action, PIF pif, int hi)
        {
            int mid = (action.PercentComplete + hi) / 2;

            Depurpose(action, pif, mid);
            ClearIP(action, pif, hi);
        }

        internal static void ReconfigureManagement(AsyncAction action, PIF down_pif, PIF up_pif, bool this_host,
            bool lock_pif, int hi, bool bring_down_down_pif)
        {
            System.Diagnostics.Trace.Assert(down_pif.host.opaque_ref == up_pif.host.opaque_ref);

            // To change the management interface on a host, you need to:
            //   1.  Clear the disallow_unplug flag from the current management PIF (we clear ManagementPurpose at the same time);
            //   2.  Use Host.management_reconfigure to switch to the new PIF;
            //   3.  Clear the IP address from the old management PIF.

            int lo = action.PercentComplete;
            int inc = (hi - lo) / 3;

            lo += inc;
            if (bring_down_down_pif)
                ForSomeHosts(action, down_pif, this_host, lock_pif, lo, Depurpose);
            lo += inc;
            ForSomeHosts(action, up_pif, this_host, lock_pif, lo, ReconfigureManagement_);
            if (bring_down_down_pif)
                ForSomeHosts(action, down_pif, this_host, lock_pif, hi, ClearIP);
        }

        /// <summary>
        /// If the "from" PIF is a secondary management interface,
        /// move the management interface name to the "to" PIF instead.
        /// </summary>
        internal static void MoveManagementInterfaceName(AsyncAction action, PIF from, PIF to)
        {
            string managementPurpose = from.ManagementPurpose;

            if (string.IsNullOrEmpty(managementPurpose))
                return;

            log.DebugFormat("Moving management interface name from {0} to {1}...", from.uuid, to.uuid);

            PIF to_clone = (PIF)to.Clone();
            to_clone.ManagementPurpose = from.ManagementPurpose;
            to_clone.SaveChanges(action.Session);

            PIF from_clone = (PIF)from.Clone();
            from_clone.ManagementPurpose = null;
            from_clone.SaveChanges(action.Session);

            log.DebugFormat("Moving management interface name from {0} to {1} done.", from.uuid, to.uuid);
        }

        /// <summary>
        /// Clear the disallow_unplug and ManagementPurpose on the given NIC.
        /// </summary>
        private static void Depurpose(AsyncAction action, PIF pif, int hi)
        {
            log.DebugFormat("Depurposing PIF {0} {1}...", pif.Name, pif.uuid);
            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_DEPURPOSING, pif.Name);

            PIF p = (PIF)pif.Clone();
            p.disallow_unplug = false;
            p.ManagementPurpose = null;
            p.SaveChanges(action.Session);

            action.PercentComplete = hi;

            log.DebugFormat("Depurposed PIF {0} {1}.", pif.Name, pif.uuid);
            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_DEPURPOSED, pif.Name);
        }

        /// <summary>
        /// Switch the host's management interface from its current setting over to the given PIF.
        /// </summary>
        private static void ReconfigureManagement_(AsyncAction action, PIF pif, int hi)
        {
            log.DebugFormat("Switching to PIF {0} {1} for management...", pif.Name, pif.uuid);
            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_MANAGEMENT_RECONFIGURING, pif.Name);

            int mid = (hi + action.PercentComplete) / 2;

            PIF p = (PIF)pif.Clone();
            p.disallow_unplug = false;
            p.ManagementPurpose = null;
            p.SaveChanges(action.Session);

            action.PercentComplete = mid;

            action.RelatedTask = XenAPI.Host.async_management_reconfigure(action.Session, pif.opaque_ref);
            action.PollToCompletion(mid, hi);

            log.DebugFormat("Switched to PIF {0} {1} for management.", pif.Name, pif.uuid);
            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_MANAGEMENT_RECONFIGURED, pif.Name);
        }

        /// <summary>
        /// Remove the IP address from the given PIF.
        /// </summary>
        private static void ClearIP(AsyncAction action, PIF pif, int hi)
        {
            log.DebugFormat("Removing IP address from {0} {1}...", pif.Name, pif.uuid);
            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_BRINGING_DOWN, pif.Name);

            action.RelatedTask = PIF.async_reconfigure_ip(action.Session, pif.opaque_ref, ip_configuration_mode.None, "", "", "", "");
            action.PollToCompletion(action.PercentComplete, hi);

            action.Description = string.Format(Messages.ACTION_CHANGE_NETWORKING_BRINGING_DOWN_DONE, pif.Name);
            log.DebugFormat("Removed IP address from {0} {1}.", pif.Name, pif.uuid);
        }

        internal delegate void PIFMethod(AsyncAction action, PIF pif, int hi);

        internal static void ForSomeHosts(AsyncAction action, PIF pif, bool this_host, bool lock_pif, int hi, PIFMethod pif_method)
        {
            System.Diagnostics.Trace.Assert(pif != null);

            XenAPI.Network network = action.Connection.Resolve(pif.network);
            if (network == null)
            {
                log.Warn("Network has gone away");
                return;
            }

            List<PIF> to_reconfigure =
                action.Connection.ResolveAll(network.PIFs).FindAll(
                    (Predicate<PIF>)delegate(PIF p)
                    {
                        return this_host == (p.host.opaque_ref == pif.host.opaque_ref);
                    });

            if (to_reconfigure.Count == 0)
                return;

            int lo = action.PercentComplete;
            int inc = (hi - lo) / to_reconfigure.Count;
            foreach (PIF p in to_reconfigure)
            {
                if (lock_pif)
                    p.Locked = true;
                try
                {
                    lo += inc;
                    pif_method(action, p, lo);
                }
                finally
                {
                    if (lock_pif)
                        p.Locked = false;
                }
            }
        }
    }
}
