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

namespace XenAPI
{
    public partial class VBD : IComparable<VBD>
    {

        public bool IsCDROM
        {
            get { return this.type == XenAPI.vbd_type.CD; }
        }

        public override string ToString()
        {
            if (VDI != null)
                return Connection.Resolve<VDI>(VDI).name_label;

            return "";
        }

        // TODO: If we get floppy disk support extend the enum and fix this check to enable floppy disk drives in MultipleDvdIsoList.cs
        public bool IsFloppyDrive
        {
            get { return false; }
        }

        public VBD FindVMCDROM(VM vm)
        {
            if (vm == null)
                return null;

            List<VBD> vbds =
                vm.Connection.ResolveAll(vm.VBDs).FindAll(delegate(VBD vbd) { return vbd.IsCDROM; });

            if (vbds.Count > 0)
            {
                vbds.Sort();
                return vbds[0];
            }
            else
            {
                return null;
            }
        }

        public bool IsOwner
        {
            get { return other_config != null && other_config.ContainsKey("owner"); }
            set
            {
                if (value != IsOwner)
                {
                    Dictionary<string, string> new_other_config =
                        other_config == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(other_config);
                    if (value)
                        new_other_config["owner"] = "true";
                    else
                        new_other_config.Remove("owner");
                    other_config = new_other_config;
                }
            }
        }

        public int IONice
        {
            get
            {
                if (qos_algorithm_params != null && qos_algorithm_params.ContainsKey("class"))
                    return int.Parse(qos_algorithm_params["class"]);
                else
                    return 0;
            }
            set
            {
                if (value != IONice)
                {
                    Dictionary<string, string> new_qos_algorithm_params =
                        qos_algorithm_params == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(qos_algorithm_params);
                    new_qos_algorithm_params["class"] = value.ToString();

                    // set the IO scheduling algorithm to use
                    qos_algorithm_type = "ionice";
                    // which scheduling class ionice should use
                    // best-effort for now (other options are 'rt' and 'idle')
                    new_qos_algorithm_params["sched"] = "be";

                    qos_algorithm_params = new_qos_algorithm_params;
                }
            }
        }

        public bool read_only
        {
            get { return mode == vbd_mode.RO; }
            set { mode = value ? vbd_mode.RO : vbd_mode.RW; }
        }

        public override int CompareTo(VBD other)
        {
            return userdevice.CompareTo(other.userdevice);
        }
    }
}
