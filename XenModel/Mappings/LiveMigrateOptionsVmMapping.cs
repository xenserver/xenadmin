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
using System.Linq;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Mappings
{
    public class LiveMigrateOptionsVmMapping
    {
        private readonly VmMapping vmMapping;
        private readonly VM vm;

        public LiveMigrateOptionsVmMapping(VmMapping vmMapping, IXenObject vm)
        {
            this.vmMapping = vmMapping;
            this.vm = vm as VM;

            if(vm==null)
                throw new NullReferenceException("VM passed to ctor was null");
        }

        /// <summary>
        /// VDI ref to SR ref Map
        /// </summary>
        public Dictionary<XenRef<VDI>, XenRef<SR>> VdiMap
        { 
            get
            {
                return vmMapping.Storage.ToDictionary(pair => new XenRef<VDI>(pair.Key), 
                                                      pair => new XenRef<SR>(pair.Value));
            }
        }

        /// <summary>
        /// VIF to remote network
        /// </summary>
        public Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>> VifMap 
        { 
            get
            {
                //VM mapping is network to network
                Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>> map = new Dictionary<XenRef<VIF>, XenRef<XenAPI.Network>>();

                foreach (var pair in vmMapping.Networks)
                {
                    XenAPI.Network net = vm.Connection.Resolve(new XenRef<XenAPI.Network>(pair.Key));

                    List<XenRef<VIF>> selectedVIFs =  (from vifRef in net.VIFs
                                                       where vm.VIFs.Contains(vifRef)
                                                       select vifRef).ToList();

                    KeyValuePair<string, XenAPI.Network> closurePair = pair;
                    selectedVIFs.ForEach(v => map.Add(new XenRef<VIF>(v.opaque_ref),
                                         new XenRef<XenAPI.Network>(closurePair.Value.opaque_ref)));

                }

                return map;
            }
        }

        public bool Live
        {
            get { return true; }
        }

        public Dictionary<string, string> Options 
        {
            get { return new Dictionary<string, string>(); }
        }
    }
}
