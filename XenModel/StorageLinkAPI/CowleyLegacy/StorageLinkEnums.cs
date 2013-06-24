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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using XenAdmin;


namespace XenAPI
{
    public static class StorageLinkEnums
    {
        /// <summary>
        /// Gets the text to display in the UI for the specified enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum for which the UI displayable text should be shown.</typeparam>
        /// <param name="value">The value of the num of which the UI displayable text should be shown.</param>
        /// <returns>The text.</returns>
        public static string GetDisplayText<TEnum>(TEnum value)
        {
            // can't seem to do compile-time checking for enums in C# generics.
            Type enumType = typeof(TEnum);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("TEnum is not an enum.", "TEnum");
            }

            uint iValue = Convert.ToUInt32(value);

            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                object val = field.GetValue(null);
                uint iVal = (uint)val;

                if ((iValue == 0 && iVal == 0) || (iVal & iValue) != 0)
                {
                    string text = Messages.ResourceManager.GetString("STORAGELINK_" + enumType.Name.ToUpper() + "_" + val);
                    return string.IsNullOrEmpty(text) ? val.ToString() : text;
                }
            }
            return value.ToString();
        }

        public enum JobState
        {
            Autodetect = 0,
            Created = 1,
            Pending = 2,
            Queued = 3,
            Activating = 4,
            Running = 5,
            Successful = 6,
            Failed = 7,
            Cancelled = 8
        };

        public enum LicenseType
        {
            LICENSE_ALL,
            LICENSE_ENTERPRISE,
            LICENSE_PLATINUM,
        };

        [Flags]
        public enum ModuleCredInputOptions : uint
        {
            STORAGE_ADAPTER_CRED_SUPPORTS_USERNAME = 0x000001,
            STORAGE_ADAPTER_CRED_SUPPORTS_PASSWORD = 0x000002,
            STORAGE_ADAPTER_CRED_SUPPORTS_IPADDRESS = 0x000004,
            STORAGE_ADAPTER_CRED_SUPPORTS_PORTNUM = 0x000008,
            STORAGE_ADAPTER_CRED_SUPPORTS_NAMESPACE = 0x000010,
            STORAGE_ADAPTER_CRED_SUPPORTS_CONTEXT = 0x000020,
            STORAGE_ADAPTER_CRED_REQUIRES_USERNAME = 0x001000,
            STORAGE_ADAPTER_CRED_REQUIRES_PASSWORD = 0x002000,
            STORAGE_ADAPTER_CRED_REQUIRES_IPADDRESS = 0x004000,
            STORAGE_ADAPTER_CRED_REQUIRES_PORTNUM = 0x008000,
            STORAGE_ADAPTER_CRED_REQUIRES_NAMESPACE = 0x010000,
            STORAGE_ADAPTER_CRED_REQUIRES_CONTEXT = 0x020000
        };

        //Indicates the type of storage hardware or software that is being managed by the module
        [Flags]
        public enum FlagsType
        {
            NONE = 0x0000,
            REFRESH_CACHE = 0x0001,
            ASYNC = 0x0002,
            XML_OUTPUT = 0x0004,
            VERBOSE = 0x0008,
            FORCE = 0x0010,
            MINIMAL = 0x0020,
            HOST_GROUP = 0x0040,
            SR_BRIDGE = 0x0080,
            DEEP_REFRESH_CACHE = 0x0101,
            PV_VM = 0x0200,
            RESTARTABLE = 0x0400,
            NONRECURSIVE_CANCEL = 0x0800,
            LIC_PLATINUM = 0x1000,
            LIC_ENTERPRISE = 0x2000,
            HIDDEN = 0x4000,
            LIVE = 0x8000
        };

        [Flags]
        public enum ProvisioningType : uint
        {
            THICK = 0x000100,
            THIN = 0x000200
        };

        [Flags]
        public enum ProvisioningOptions : uint
        {
            NONE = 0x000000,
            DEDUPLICATION = 0x002000
        };

        [Flags]
        public enum RaidType : uint
        {
            RAID_NONE = 0x0001,
            RAID_JBOD = 0x0002,
            RAID0_CONCAT = 0x0004,
            RAID0 = 0x0008,
            RAID1 = 0x0010,
            RAID3 = 0x0020,
            RAID4 = 0x0040,
            RAID5 = 0x0080,
            RAID6 = 0x0100,
            RAID0_1 = 0x0200,
            RAID10 = 0x0400,
            RAID15 = 0x0800,
            RAID50 = 0x1000,
            RAID60 = 0x2000,
            RAID_NET = 0x4000
        };

        public enum HypervisorType
        {
            Autodetect,
            HYPERV,
            XEN
        };

        [Flags]
        public enum StorageSystemCapabilities : uint
        {
            AUTO = 0x00000001,
            ISCSI = 0x00000002,
            FIBRE_CHANNEL = 0x00000004,
            NFS = 0x00000008,
            CIFS = 0x00000010,
            SAS = 0x00000020,
            VM_CUSTOM1 = 0x00000040,
            VM_CUSTOM2 = 0x00000080,
            PROVISION_FULL = 0x00000100,
            PROVISION_THIN = 0x00000200,
            MAPPING = 0x00000400,
            MULTIPLE_STORAGE_POOLS = 0x00000800,
            LUN_GROUPING = 0x00001000,
            POOL_LEVEL_DEDUPLICATION = 0x00002000,
            VOLUME_LEVEL_DEDUPLICATION = 0x00004000,
            DIFF_SNAPSHOT = 0x00010000,
            PLEX_SNAPSHOT = 0x00020000,
            REMOTE_REPLICATION = 0x00040000,
            CLONE = 0x00080000,
            RESIZE = 0x00100000,
            STORAGE_POOL_CLEANUP = 0x00200000,
            CUSTOM_ENDPOINTS = 0x00400000,
            OPTIMIZED_ISCSI_LOGIN = 0x00800000,
            LUN_PER_IQN = 0x01000000,
            INSTANT_CLONE = 0x02000000,
            CLONE_OF_SNAPSHOT = 0x04000000,
            ISCSI_PRE_LOGIN = 0x08000000,
            SNAPSHOT_OF_SNAPSHOT = 0x10000000,
            GROUP_BASED_ASSIGNMENT = 0x20000000
        };
    }
}
