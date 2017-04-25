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

using XenAdmin.Core;

namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public class FibreChannelDevice : IComparable<FibreChannelDevice>
    {
        public readonly String Serial;
        public readonly String Path;
        public readonly long Size;
        public readonly String Vendor;
        public readonly String SCSIid;
        public readonly String adapter;
        public readonly String channel;
        public readonly String id;
        public readonly String lun;
        public readonly String name_label;
        public readonly String name_description;
        public readonly bool pool_metadata_detected;
        public readonly String eth;

        public FibreChannelDevice(String serial, String path,
            String vendor, long size, String SCSIid, String adapter,
            String channel, String id, String lun)
        {
            this.Serial = serial;
            this.Path = path;
            this.Vendor = vendor;
            this.Size = size;
            this.SCSIid = SCSIid;
            this.adapter = adapter;
            this.channel = channel;
            this.id = id;
            this.lun = lun;
            this.name_label = "";
            this.name_description = "";
            this.pool_metadata_detected = false;
        }

        public FibreChannelDevice(String serial, String path,
            String vendor, long size, String SCSIid, String adapter,
            String channel, String id, String lun,
            String name_label, String name_description, bool pool_metadata_detected,
            String eth)
        {
            this.Serial = serial;
            this.Path = path;
            this.Vendor = vendor;
            this.Size = size;
            this.SCSIid = SCSIid;
            this.adapter = adapter;
            this.channel = channel;
            this.id = id;
            this.lun = lun;
            this.name_label = name_label;
            this.name_description = name_description;
            this.pool_metadata_detected = pool_metadata_detected;
            this.eth = eth;
        }

        public int CompareTo(FibreChannelDevice other)
        {
            long n = Size - other.Size;
            return n == 0 ? StringUtility.NaturalCompare(Serial, other.Serial) : (int)n;
        }
    }
}
