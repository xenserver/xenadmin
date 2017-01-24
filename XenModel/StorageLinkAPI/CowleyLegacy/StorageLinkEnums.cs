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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using XenAdmin;


namespace XenAPI
{
    public static class StorageLinkEnums
    {
        [Flags]
        public enum ProvisioningType : uint
        {
            THICK = 0x000100,
            THIN = 0x000200
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
    }
}
