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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Alerts;
using XenAdmin.Model;
using System.Drawing;

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.XenSearch;
using System.IO;
using XenAdmin.Network.StorageLink;

namespace XenAdmin
{
    class Images
    {
        public static readonly ImageList ImageList16 = new ImageList();
        /// <summary>
        /// This is the same images as in ImageList16, but in an array.
        /// Reading from the ImageList is strangely slow.
        /// </summary>
        private static readonly Image[] ImageArray16 = new Image[Enum.GetValues(typeof(Icons)).Length];

        static Images()
        {
            ImageList16.ColorDepth = ColorDepth.Depth32Bit;
            ImageList16.TransparentColor = Color.Transparent;

            // Initialize our ImageList. We do this programatically (as opposed to using the designer) because
            // of a known bug in ImageList with transparency. 
            // See http://www.codeproject.com/cs/miscctrl/AlphaImageImagelist.asp?df=100&forumid=137678&exp=0&select=1392020#xx1392020xx
            // To workaround the bug, we just add the images from the resource file.
            // Note that this list is in the same order as the enum in Icons.cs.
            ImageList16.Images.Add("Logo.png", XenAdmin.Properties.Resources.Logo);
            ImageList16.Images.Add("000_ServerInProgress_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerInProgress_h32bit_16);
            ImageList16.Images.Add("000_TreeConnected_h32bit_16.png", XenAdmin.Properties.Resources._000_TreeConnected_h32bit_16);
            ImageList16.Images.Add("000_ServerDisconnected_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerDisconnected_h32bit_16);
            ImageList16.Images.Add("000_ServerMaintenance_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerMaintenance_h32bit_16);
            ImageList16.Images.Add("000_HostUnpatched_h32bit_16.png", XenAdmin.Properties.Resources._000_HostUnpatched_h32bit_16);
            ImageList16.Images.Add("server_up_16.png", XenAdmin.Properties.Resources.server_up_16);
            ImageList16.Images.Add("000_ServerErrorFile_h32bit_16.png", XenAdmin.Properties.Resources._000_ServerErrorFile_h32bit_16);
            ImageList16.Images.Add("000_StartVM_h32bit_16.png", XenAdmin.Properties.Resources._000_StartVM_h32bit_16);
            ImageList16.Images.Add("000_VMDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMDisabled_h32bit_16);
            ImageList16.Images.Add("000_StoppedVM_h32bit_16.png", XenAdmin.Properties.Resources._000_StoppedVM_h32bit_16);
            ImageList16.Images.Add("000_VMStoppedDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMStoppedDisabled_h32bit_16);
            ImageList16.Images.Add("000_SuspendVM_h32bit_16.png", XenAdmin.Properties.Resources._000_SuspendVM_h32bit_16);
            ImageList16.Images.Add("000_VMPausedDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMPausedDisabled_h32bit_16);
            ImageList16.Images.Add("000_VMStarting_h32bit_16.png", XenAdmin.Properties.Resources._000_VMStarting_h32bit_16);
            ImageList16.Images.Add("000_VMStartingDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_VMStartingDisabled_h32bit_16);
            ImageList16.Images.Add("000_VMTemplate_h32bit_16.png", XenAdmin.Properties.Resources._000_VMTemplate_h32bit_16);
            ImageList16.Images.Add("000_TemplateDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_TemplateDisabled_h32bit_16);
            ImageList16.Images.Add("000_UserTemplate_h32bit_16.png", XenAdmin.Properties.Resources._000_UserTemplate_h32bit_16);
            ImageList16.Images.Add("000_VMSession_h32bit_16.png", XenAdmin.Properties.Resources._000_VMSession_h32bit_16);
            ImageList16.Images.Add("000_VMSnapShotDiskOnly_h32bit_16.png", XenAdmin.Properties.Resources._000_VMSnapShotDiskOnly_h32bit_16);
            ImageList16.Images.Add("000_VMSnapshotDiskMemory_h32bit_16.png", XenAdmin.Properties.Resources._000_VMSnapshotDiskMemory_h32bit_16);
            ImageList16.Images.Add("_000_ScheduledVMsnapshotDiskOnly_h32bit_16.png", XenAdmin.Properties.Resources._000_ScheduledVMsnapshotDiskOnly_h32bit_16);
            ImageList16.Images.Add("_000_ScheduledVMsnapshotDiskMemory_h32bit_16.png", XenAdmin.Properties.Resources._000_ScheduledVMsnapshotDiskMemory_h32bit_16);
            ImageList16.Images.Add("000_PoolConnected_h32bit_16.png", XenAdmin.Properties.Resources._000_PoolConnected_h32bit_16);
            ImageList16.Images.Add("pool_up_16.png", XenAdmin.Properties.Resources.pool_up_16);

            ImageList16.Images.Add("000_Storage_h32bit_16.png", XenAdmin.Properties.Resources._000_Storage_h32bit_16);
            ImageList16.Images.Add("000_StorageBroken_h32bit_16.png", XenAdmin.Properties.Resources._000_StorageBroken_h32bit_16);
            ImageList16.Images.Add("000_StorageDefault_h32bit_16.png", XenAdmin.Properties.Resources._000_StorageDefault_h32bit_16);
            ImageList16.Images.Add("000_StorageDisabled_h32bit_16.png", XenAdmin.Properties.Resources._000_StorageDisabled_h32bit_16);
            ImageList16.Images.Add("000_StorageNeedsUpgrading_h32bit_16.png", XenAdmin.Properties.Resources._000_StorageNeedsUpgrading_h32bit_16);

            ImageList16.Images.Add("001_ShutDown_h32bit_16.png", Properties.Resources._001_ShutDown_h32bit_16);
            ImageList16.Images.Add("000_paused_h32bit_16.png", Properties.Resources._000_paused_h32bit_16);
            ImageList16.Images.Add("001_PowerOn_h32bit_16.png", Properties.Resources._001_PowerOn_h32bit_16);
            ImageList16.Images.Add("000_HelpIM_h32bit_16.png", Properties.Resources._000_HelpIM_h32bit_16);

            ImageList16.Images.Add("000_Network_h32bit_16.png", Properties.Resources._000_Network_h32bit_16);
            ImageList16.Images.Add("000_defaultSpyglass_h32bit_16.png", Properties.Resources._000_defaultSpyglass_h32bit_16);
            ImageList16.Images.Add("000_Search_h32bit_16.png", Properties.Resources._000_Search_h32bit_16);
            
            #region Server message images

            ImageList16.Images.Add("001_PowerOn_h32bit_16.png", Properties.Resources._001_PowerOn_h32bit_16);
            ImageList16.Images.Add("001_ShutDown_h32bit_16.png", Properties.Resources._001_ShutDown_h32bit_16);
            ImageList16.Images.Add("001_Reboot_h32bit_16.png", Properties.Resources._001_Reboot_h32bit_16);
            ImageList16.Images.Add("000_paused_h32bit_16.png", Properties.Resources._000_paused_h32bit_16);
            ImageList16.Images.Add("000_Resumed_h32bit_16.png", Properties.Resources._000_Resumed_h32bit_16);
            ImageList16.Images.Add("clonevm_16.png", Properties.Resources.clonevm_16);

            ImageList16.Images.Add("Logo.png", Properties.Resources.Logo);

            ImageList16.Images.Add("alert1_16.png", Properties.Resources.alert1_16);
            ImageList16.Images.Add("alert2_16.png", Properties.Resources.alert2_16);
            ImageList16.Images.Add("alert3_16.png", Properties.Resources.alert3_16);
            ImageList16.Images.Add("alert4_16.png", Properties.Resources.alert4_16);
            ImageList16.Images.Add("alert5_16.png", Properties.Resources.alert5_16);

            #endregion

            ImageList16.Images.Add("centos_16x.png", Properties.Resources.centos_16x);
            ImageList16.Images.Add("debian_16x.png", Properties.Resources.debian_16x);
            ImageList16.Images.Add("oracle_16x.png", Properties.Resources.oracle_16x);
            ImageList16.Images.Add("redhat_16x.png", Properties.Resources.redhat_16x);
            ImageList16.Images.Add("suse_16x.png", Properties.Resources.suse_16x);
            ImageList16.Images.Add("windows_h32bit_16.png", Properties.Resources.windows_h32bit_16);
            ImageList16.Images.Add("coreos-globe-icon.png", Properties.Resources.coreos_globe_icon);

            ImageList16.Images.Add("tools_uptodate_16x.png", Properties.Resources.tools_uptodate_16x);
            ImageList16.Images.Add("tools_notinstalled_16x.png", Properties.Resources.tools_notinstalled_16x);
            ImageList16.Images.Add("tools_outofdate_16x.png", Properties.Resources.tools_outofdate_16x);

            ImageList16.Images.Add("000_VM_h32bit_16.png", Properties.Resources._000_VM_h32bit_16);
            ImageList16.Images.Add("000_Server_h32bit_16.png", Properties.Resources._000_Server_h32bit_16);
            ImageList16.Images.Add("000_Pool_h32bit_16.png", Properties.Resources._000_Pool_h32bit_16);

            ImageList16.Images.Add("000_VirtualStorage_h32bit_16.png", Properties.Resources._000_VirtualStorage_h32bit_16);
            ImageList16.Images.Add("virtualstorage_snapshot_16.png", Properties.Resources.virtualstorage_snapshot_16);

            ImageList16.Images.Add("000_Folder_open_h32bit_16.png", Properties.Resources._000_Folder_open_h32bit_16);
            ImageList16.Images.Add("folder_grey.png", Properties.Resources.folder_grey);

            ImageList16.Images.Add("000_Tag_h32bit_16.png", Properties.Resources._000_Tag_h32bit_16);
            ImageList16.Images.Add("000_Fields_h32bit_16.png", Properties.Resources._000_Fields_h32bit_16);
            ImageList16.Images.Add("ha_16.png", Properties.Resources.ha_16);

            ImageList16.Images.Add("sl_16.png", Properties.Resources.sl_16);
            ImageList16.Images.Add("sl_connected_16.png", Properties.Resources.sl_connected_16);
            ImageList16.Images.Add("sl_disconnected_16.png", Properties.Resources.sl_disconnected_16);
            ImageList16.Images.Add("sl_connecting_16.png", Properties.Resources.sl_connecting_16);
            ImageList16.Images.Add("sl_system_16.png", Properties.Resources.sl_system_16);
            ImageList16.Images.Add("sl_pool_16.png", Properties.Resources.sl_pool_16);
            ImageList16.Images.Add("sl_pools_16.png", Properties.Resources.sl_pools_16);
            ImageList16.Images.Add("sl_lun_16.png", Properties.Resources.sl_lun_16);
            ImageList16.Images.Add("sl_luns_16.png", Properties.Resources.sl_luns_16);
            ImageList16.Images.Add("000_Storage_h32bit_16.png", Properties.Resources._000_Storage_h32bit_16);

            ImageList16.Images.Add("virtualappliance_16.png", Properties.Resources._000_VirtualAppliance_h32bit_16);

            ImageList16.Images.Add("000_MigrateVM_h32bit_16.png", Properties.Resources._000_MigrateVM_h32bit_16);

            ImageList16.Images.Add("_000_ManagementInterface_h32bit_16.png", Properties.Resources._000_ManagementInterface_h32bit_16);
            ImageList16.Images.Add("000_TCP_IPGroup_h32bit_16.png", Properties.Resources._000_TCP_IPGroup_h32bit_16);
            ImageList16.Images.Add("infra_view_16_textured.png", Properties.Resources.infra_view_16_textured);
            ImageList16.Images.Add("objects_16_textured.png", Properties.Resources.objects_16_textured);

            ImageList16.Images.Add("RunningDC_16.png", Properties.Resources.RunningDC_16);
            ImageList16.Images.Add("StoppedDC_16.png", Properties.Resources.StoppedDC_16);
            ImageList16.Images.Add("PausedDC_16.png", Properties.Resources.PausedDC_16);


            System.Diagnostics.Trace.Assert(ImageList16.Images.Count == Enum.GetValues(typeof(Icons)).Length,
                "Programmer error - you must add an entry to the image list when you add a new icon to the enum");

            int i = 0;
            foreach (Image im in ImageList16.Images)
            {
                ImageArray16[i++] = im;
            }
        }

        public static Image GetImage16For(IXenConnection connection)
        {
            Icons icon = GetIconFor(connection);
            return ImageArray16[(int)icon];
        }



        public static Image GetImage16For(IXenObject o)
        {
            Icons icon = GetIconFor(o);
            return ImageArray16[(int)icon];
        }

        public static Image GetImage16For(Search search)
        {
            return GetImage16For(GetIconFor(search));
        }

        public static Image GetImage16For(XenAPI.Message.MessageType type)
        {
            return GetImage16For(GetIconFor(type));
        }

        public static Image GetImage16For(AlertPriority priority)
        {
            var icon = GetIconFor(priority);
            return icon == Icons.MessageUnknown ? null : GetImage16For(icon);
        }

        public static Image GetImage32For(IXenObject o)
        {
            return new Bitmap(GetImage16For(o), 32, 32);
        }

        public static Image GetImage16For(Icons icon)
        {
            return ImageArray16[(int)icon];
        }

     

        public static Icons GetIconFor(IXenObject o)
        {
            VM vm = o as VM;
            if (vm != null)
                return GetIconFor(vm);

			VM_appliance appl = o as VM_appliance;
			if (appl != null)
				return GetIconFor(appl);

            SR sr = o as SR;
            if (sr != null)
                return GetIconFor(sr);

            Host host = o as Host;
            if (host != null)
                return GetIconFor(host);

            Pool pool = o as Pool;
            if (pool != null)
                return GetIconFor(pool);

            XenAPI.Network network = o as XenAPI.Network;
            if (network != null)
                return GetIconFor(network);

            VDI vdi = o as VDI;
            if (vdi != null)
                return GetIconFor(vdi);

            VBD vbd = o as VBD;
            if (vbd != null)
                return GetIconFor(vbd);

            Folder folder = o as Folder;
            if (folder != null)
                return GetIconFor(folder);

            PIF pif = o as PIF;
            if (pif != null)
                return GetIconFor(pif);

            StorageLinkServer storageLinkServer = o as StorageLinkServer;
            if (storageLinkServer != null)
                return GetIconFor(storageLinkServer);

            StorageLinkSystem storageLinkSystem = o as StorageLinkSystem;
            if (storageLinkSystem != null)
                return GetIconFor(storageLinkSystem);

            StorageLinkPool storageLinkPool = o as StorageLinkPool;
            if (storageLinkPool != null)
                return GetIconFor(storageLinkPool);

            StorageLinkVolume storageLinkVolume = o as StorageLinkVolume;
            if (storageLinkVolume != null)
                return GetIconFor(storageLinkVolume);
            
            StorageLinkRepository storageLinkRepository = o as StorageLinkRepository;
            if (storageLinkRepository != null)
                return GetIconFor(storageLinkRepository);

            DockerContainer dockerContainer = o as DockerContainer;
            if (dockerContainer != null)
                return GetIconFor(dockerContainer);

            System.Diagnostics.Trace.Assert(false,
                "You asked for an icon for a type I don't recognise!");

            return Icons.XenCenter;
        }

        public static Icons GetIconFor(Search search)
        {
            return search.DefaultSearch ? Icons.DefaultSearch : Icons.Search;
        }

        public static Icons GetIconFor(IXenConnection connection)
        {
            Pool pool = Helpers.GetPool(connection);
            if (pool != null)
                return GetIconFor(pool);

            Host host = Helpers.GetMaster(connection);
            if (host != null)
                return GetIconFor(host);

            if (connection.InProgress)
                // Yellow connection in progress icon
                return Icons.HostConnecting;
            else
                // Red disconnected icon
                return Icons.HostDisconnected;
        }

        public static Icons GetIconFor(StorageLinkSystem storageLinkSystem)
        {
            return Icons.StorageLinkSystem;
        }

        public static Icons GetIconFor(StorageLinkServer storageLinkServer)
        {
            if (storageLinkServer.StorageLinkConnection.ConnectionState == StorageLinkConnectionState.Connecting)
            {
                return Icons.StorageLinkServerConnecting;
            }
            else if (storageLinkServer.StorageLinkConnection.ConnectionState == StorageLinkConnectionState.Connected)
            {
                return Icons.StorageLinkServerConnected;
            }
            return Icons.StorageLinkServerDisconnected;
        }

        public static Icons GetIconFor(StorageLinkPool storageLinkPool)
        {
            return Icons.StorageLinkPool;
        }

        public static Icons GetIconFor(StorageLinkVolume storageLinkVolume)
        {
            VDI vdi = storageLinkVolume.VDI(ConnectionsManager.XenConnectionsCopy);
            if (vdi != null)
            {
                return GetIconFor(vdi);
            }
            return Icons.StorageLinkVolume;
        }

        public static Icons GetIconFor(StorageLinkRepository storageLinkRepository)
        {
            return Icons.StorageLinkRepository;
        }

        public static Icons GetIconFor(Folder folder)
        {
            return folder.Grey ? Icons.FolderGrey : Icons.Folder;
        }

		public static Icons GetIconFor(VM_appliance appl)
		{
			return Icons.VmAppliance;
		}

        public static Icons GetIconFor(VM vm)
        {
            bool disabled = vm.IsHidden;

            if (vm.is_a_snapshot)
            {
                if (vm.is_snapshot_from_vmpp)
                {
                    if (vm.power_state == vm_power_state.Suspended)
                        return Icons.ScheduledSnapshotDiskMemory;
                    else
                        return Icons.ScheduledSnapshotDiskOnly;
                }
                else
                {
                    if (vm.power_state == vm_power_state.Suspended)
                        return Icons.SnapshotWithMem;
                    else
                        return Icons.SnapshotDisksOnly;
                }
            }

            if (vm.is_a_template && vm.DefaultTemplate)
                return disabled ? Icons.TemplateDisabled : Icons.Template;

            if (vm.is_a_template && !vm.DefaultTemplate)
                return disabled ? Icons.TemplateDisabled : Icons.TemplateUser;

            if (!vm.ExistsOnServer)
                return disabled ? Icons.VmStoppedDisabled : Icons.VmStopped;

            if (vm.current_operations.ContainsValue(vm_operations.migrate_send))
                return Icons.VmCrossPoolMigrate; 

            // If a VM lifecycle operation is in progress, show the orange "starting" icon
            foreach (vm_operations op in vm.current_operations.Values)
            {
                if (VM.is_lifecycle_operation(op))
                    return disabled ? Icons.VmStartingDisabled : Icons.VmStarting;
            }

            switch (vm.power_state)
            {
                case XenAPI.vm_power_state.Suspended:
                    return disabled ? Icons.VmSuspendedDisabled : Icons.VmSuspended;
                case XenAPI.vm_power_state.Running:
                    return disabled ? Icons.VmRunningDisabled : Icons.VmRunning;
                case XenAPI.vm_power_state.Paused:
                    return disabled ? Icons.VmSuspendedDisabled : Icons.VmSuspended;
                default:
                    return disabled ? Icons.VmStoppedDisabled : Icons.VmStopped;
            }
        }

        public static Icons GetIconFor(SR sr)
        {
            return sr.GetIcon;
        }

        public static Icons GetIconFor(Host host)
        {

            Host_metrics metrics =host.Connection.Resolve<Host_metrics>(host.metrics);
            bool host_is_live = metrics != null && metrics.live;

            if (host_is_live)
            {
                if (host.HasCrashDumps)
                {
                    return Icons.HostHasCrashDumps;
                }
                if (host.current_operations.ContainsValue(host_allowed_operations.evacuate)
                    || !host.enabled)
                {
                    return Icons.HostEvacuate;
                }
                else if (!XenAPI.Host.IsFullyPatched(host,ConnectionsManager.XenConnectionsCopy))
                {
                    return Icons.HostUnpatched;
                }
                else if (Helpers.IsOlderThanMaster(host))
                {
                    return Icons.HostOlderThanMaster;
                }
                else
                {
                    return Icons.HostConnected;
                }
            }
            else
            {
                // XenAdmin.XenSearch.Group puts a fake host in the treeview for disconnected
                // XenConnections. So here we give the yellow 'connection in progress' icon which formerly
                // applied only to XenConnections.
                if (host.Connection.InProgress && !host.Connection.IsConnected)
                    return Icons.HostConnecting;
                else
                    return Icons.HostDisconnected;
            }
        }


        public static Icons GetIconFor(Pool pool)
        {
            return pool.Connection.IsConnected
                       ? pool.IsPoolFullyUpgraded
                             ? Icons.PoolConnected
                             : Icons.PoolNotFullyUpgraded
                       : Icons.HostDisconnected;
        }

        public static Icons GetIconFor(XenAPI.Network network)
        {
            return Icons.Network;
        }

        public static Icons GetIconFor(VDI vdi)
        {
            if (vdi.is_a_snapshot)
                return Icons.VDISnapshot;
            else
                return Icons.VDI;
        }

        public static Icons GetIconFor(VBD vbd)
        {
            return Icons.VDI;
        }


        public static Icons GetIconFor(XenAPI.Message.MessageType type)
        {
            switch (type)
            {
                case XenAPI.Message.MessageType.VM_STARTED:
                    return Icons.VmStart;

                case XenAPI.Message.MessageType.VM_SHUTDOWN:
                    return Icons.VmShutdown;

                case XenAPI.Message.MessageType.VM_REBOOTED:
                    return Icons.VmReboot;

                case XenAPI.Message.MessageType.VM_SUSPENDED:
                    return Icons.VmSuspend;

                case XenAPI.Message.MessageType.VM_RESUMED:
                    return Icons.VmResumed;

                case XenAPI.Message.MessageType.VM_CLONED:
                    return Icons.VmCloned;

                default:
                    return Icons.MessageUnknown;
            }
        }

        public static Icons GetIconFor(AlertPriority priority)
        {
            switch (priority)
            {
                case AlertPriority.Priority1:
                    return Icons.MessagePriority1;
                case AlertPriority.Priority2:
                    return Icons.MessagePriority2;
                case AlertPriority.Priority3:
                    return Icons.MessagePriority3;
                case AlertPriority.Priority4:
                    return Icons.MessagePriority4;
                case AlertPriority.Priority5:
                    return Icons.MessagePriority5;
                default:
                    return Icons.MessageUnknown;
            }
        }

        public static void GenImageHTML()
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter("images.htm");
            tw.WriteLine("<html><head><title>XenCenter Images</title></head>");

            tw.WriteLine("<body><table>");

            foreach (Icons icon in Enum.GetValues(typeof(Icons)))
            {
                tw.WriteLine("<tr><td>{0}</td><td><image src='{1}'></td></tr>", icon, ImageList16.Images.Keys[(int)icon]);
            }

            tw.WriteLine("</table></body></html>");

            // close the stream
            tw.Close();
        }

        public static Icons GetIconFor(PIF pif)
        {
            return pif.IsPrimaryManagementInterface() ? Icons.PifPrimary : Icons.PifSecondary;
        }

        public static Icons GetIconFor(DockerContainer dockerContainer)
        {
            switch (dockerContainer.power_state)
            {
                case vm_power_state.Paused:
                    return Icons.DCPaused;
                case vm_power_state.Running:
                    return Icons.DCRunning;
                default:
                    return Icons.DCStopped;
            }
        }
    }
}
