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

namespace XenAPI
{
    public partial class VBD : IComparable<VBD>
    {

        public bool IsCDROM()
        {
            return this.type == XenAPI.vbd_type.CD;
        }

        public override string ToString()
        {
            if (VDI != null)
                return Connection.Resolve<VDI>(VDI).name_label;

            return "";
        }

        // TODO: If we get floppy disk support extend the enum and fix this check to enable floppy disk drives in MultipleDvdIsoList.cs
        public bool IsFloppyDrive()
        {
            return false;
        }

        public bool GetIsOwner()
        {
            return other_config != null && other_config.ContainsKey("owner");
        }

        public void SetIsOwner(bool value)
        {
            _other_config = SetDictionaryKey(other_config, "owner", value ? "true" : null);
        }

        public int GetIoNice()
        {
            if (qos_algorithm_params != null && qos_algorithm_params.ContainsKey("class"))
                return int.Parse(qos_algorithm_params["class"]);
            else
                return 0;
        }

        public void SetIoNice(int value)
        {
            // set the IO scheduling algorithm to use
            qos_algorithm_type = "ionice";

            // which scheduling class ionice should use
            // best-effort for now (other options are 'rt' and 'idle')

            qos_algorithm_params = SetDictionaryKeys(qos_algorithm_params,
                new KeyValuePair<string, string>("class", value.ToString()),
                new KeyValuePair<string, string>("sched", "be"));
        }

        public bool IsReadOnly()
        {
            return mode == vbd_mode.RO;
        }

        public override int CompareTo(VBD other)
        {
            return userdevice.CompareTo(other.userdevice);
        }
    }
}
