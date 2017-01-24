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

using System.Linq;

namespace XenAPI
{
    public partial class VM_guest_metrics
    {
        /// <summary>
        /// List of distro values that we treat as Linux/Non-Windows (written by Linux Guest Agent, evaluating xe-linux-distribution)
        /// </summary>
        private readonly string[] linuxDistros = { "debian", "rhel", "fedora", "centos", "scientific", "oracle", "sles", "lsb", "boot2docker", "freebsd" };
        
        //This will tell if they are install (but they may not be upto date!)
        public bool PV_drivers_installed
        {
            get
            {
                return PV_drivers_version.ContainsKey("major") && PV_drivers_version.ContainsKey("minor");
            }
        }

        /// <summary>
        /// Returns true if (it is known that) this VM_guest_metrics belongs to a non-Windows VM.
        /// (Returns false if the VM is Windows or unknown)
        /// </summary>
        /// <param name="gm"></param>
        /// <returns></returns>
        public bool IsVmNotWindows
        {
            get
            {
                if (this.os_version != null)
                {
                    if (this.os_version.ContainsKey("distro") && !string.IsNullOrEmpty(this.os_version["distro"]) && linuxDistros.Contains(this.os_version["distro"].ToLowerInvariant()))
                        return true;

                    if (this.os_version.ContainsKey("uname") && this.os_version["uname"].ToLowerInvariant().Contains("netscaler"))
                        return true;
                }

                return false;
            }
        }
    }
}
