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
using XenAPI;
using System.Collections;
using System.Collections.Generic;

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal class fakeVM : fakeXenObject
    {
        public fakeVM(DbProxy proxy)
            : base("vm", proxy)
        {

        }

        #region invoke
        public Response<string> start(string session, string opaque_ref)
        {
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "start");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "start_on");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "copy");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "clone");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "destroy");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "make_into_template");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "export");

            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "power_state", "Running");

            string affinity = (string)proxy.db.GetValue("vm", opaque_ref, "affinity");
            if (string.IsNullOrEmpty(affinity) || affinity == Helper.NullOpaqueRef)
                affinity = proxy.GetRandomRef("host");
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "resident_on", affinity);

            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "hard_shutdown");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "hard_reboot");
            return new Response<string>("");
        }

        public Response<string> clean_shutdown(string session, string opaque_ref)
        {
            ThrowIfReadOnly();
            return hard_shutdown(session, opaque_ref);
        }

        public Response<string> hard_shutdown(string session, string opaque_ref)
        {
            ThrowIfReadOnly();
            hard_shutdown(proxy, opaque_ref);
            return new Response<string>("");
        }

        internal static void hard_shutdown(DbProxy proxy, string opaque_ref)
        {
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "hard_shutdown");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "hard_reboot");

            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "power_state", "Halted");
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "resident_on", Helper.NullOpaqueRef);

            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "start");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "start_on");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "copy");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "clone");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "destroy");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "make_into_template");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "export");
        }

        public Response<string> suspend(string session, string opaque_ref)
        {
            ThrowIfReadOnly();
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "hard_reboot");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "suspend");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "clean_shutdown");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "clean_reboot");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "pool_migrate");

            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "power_state", "Suspended");
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "resident_on", Helper.NullOpaqueRef);

            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "resume");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "resume_on");
            return new Response<string>("");
        }

        public Response<string> resume(string session, string opaque_ref, bool paused, bool force)
        {
            ThrowIfReadOnly();
            string affinity = (string)proxy.db.GetValue("vm", opaque_ref, "affinity");
            if (string.IsNullOrEmpty(affinity) || affinity == Helper.NullOpaqueRef)
                affinity = proxy.GetRandomRef("host");

            return resume_on(session, opaque_ref, affinity, paused, force);
        }

        public Response<string> resume_on(string session, string opaque_ref, string host_ref, bool paused, bool force)
        {
            ThrowIfReadOnly();
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "resume");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "vm", opaque_ref, "allowed_operations", "resume_on");

            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "power_state", "Running");
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "resident_on", host_ref);

            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "hard_reboot");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "suspend");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "clean_shutdown");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "clean_reboot");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "vm", opaque_ref, "allowed_operations", "pool_migrate");
            return new Response<string>("");
        }

        public Response<string> copy(string session, string opaque_ref, string name, string dest_sr_opaque_ref)
        {
            ThrowIfReadOnly();
            return clone(session, opaque_ref, name);
        }

        public Response<string> clone(string session, string opaque_ref, string name)
        {
            ThrowIfReadOnly();
            string new_vm = proxy.CopyObject("vm", opaque_ref); // we need to give this vm a metrics object;
            string new_metrics = proxy.CopyObject("vm_metrics", (string)proxy.db.GetValue("vm", opaque_ref, "metrics"));
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", new_vm, "metrics", new_metrics);
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", new_vm, "name_label", name);
            proxy.SendCreateObject("vm", new_vm);
            return new Response<string>(new_vm);
        }

        public Response<string> pool_migrate(string session, string opaque_ref, string host_ref, Hashtable options)
        {
            ThrowIfReadOnly();
            VM vm = proxy.connection.Resolve(new XenRef<VM>(opaque_ref));
            if (vm == null)
                return new Response<string>(true, new string[] { Failure.INTERNAL_ERROR });
            pool_migrate(proxy, opaque_ref, vm.resident_on, host_ref);
            return new Response<string>("");
        }

        internal static void pool_migrate(DbProxy proxy, string vm, string src, string dest)
        {
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "host", src, "resident_VMs", vm);
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "host", dest, "resident_VMs", vm);
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", vm, "resident_on", dest);
        }

        public Response<string> assert_can_migrate(string session, string vm, Hashtable receiveMapping, bool live, Hashtable srMap, Hashtable networkMap, Hashtable options)
        {
            //Tell the method to return an error - hopefully the opaque ref will not be set to this in real life
            if (vm == Failure.INTERNAL_ERROR)
                return new Response<string>(true, new []{ Failure.INTERNAL_ERROR });
            return new Response<string>("");
        }

        public Response<string> provision(string session, string opaque_ref)
        {
            ThrowIfReadOnly();
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "is_a_template", false);
            return new Response<string>("");
        }

        public Response<string> destroy(string session, string opaque_ref)
        {
            ThrowIfReadOnly();
            return destroyObj("vm", opaque_ref);
        }

        public Response<string> copy_bios_strings(string session, string opaque_ref, string host)
        {
            Hashtable src = (Hashtable)proxy.db.Tables["host"].Rows[host].Props["bios_strings"].XapiObjectValue;
            Db.Row destVm = proxy.db.Tables["vm"].Rows[opaque_ref];
            destVm.Props["bios_strings"] = new Db.Prop(destVm, "bios_strings", new Hashtable(src));
            proxy.SendModObject("vm", opaque_ref);
            return new Response<string>("");
        }

        #endregion

        #region get
        
        public Response<object> get_is_a_template(string session, string opaque_ref)
        {
            return new Response<object>(proxy.db.GetValue("vm", opaque_ref, "is_a_template"));
        }

        public Response<string[]> get_allowed_vif_devices(string session, string opaque_ref)
        {
            List<string> used = new List<string>();
            List<string> devices = new List<string>();
            
            foreach (KeyValuePair<string, Db.Row> vif_row in proxy.db.Tables["vif"].Rows)
            {
                if ((string)proxy.db.GetValue("vif", vif_row.Key, "VM") == opaque_ref)
                    used.Add((string)proxy.db.GetValue("vif", vif_row.Key, "device"));
            }

            for (int i = 0; i < 7; i++)
            {
                if (used.Contains(i.ToString()))
                    continue;

                devices.Add(i.ToString());
            }
            return new Response<string[]>(devices.ToArray());
        }
        #endregion

        #region set

        public Response<string> set_is_a_template(string session, string opaque_ref, bool template)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "is_a_template", template);
            return new Response<string>("");
        }

        public Response<string> set_name_label(string session, string opaque_ref, string name)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "name_label", name);
            return new Response<string>("");
        }

        public Response<string> set_name_description(string session, string opaque_ref, string description)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "name_description", description);
            return new Response<string>("");
        }

        public Response<string> set_VCPUs_max(string session, string opaque_ref, string vcpus)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "VCPUs_max", vcpus);
            return new Response<string>("");
        }

        public Response<string> set_VCPUs_at_startup(string session, string opaque_ref, string vcpus)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "VCPUs_at_startup", vcpus);
            return new Response<string>("");
        }

        public Response<string> set_memory_dynamic_min(string session, string opaque_ref, string memory)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "memory_dynamic_min", memory);
            return new Response<string>("");
        }

        public Response<string> set_memory_dynamic_max(string session, string opaque_ref, string memory)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "memory_dynamic_max", memory);
            return new Response<string>("");
        }

        public Response<string> set_memory_static_max(string session, string opaque_ref, string memory)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "memory_static_max", memory);
            return new Response<string>("");
        }

        public Response<string> set_HVM_boot_params(string session, string opaque_ref, Hashtable boot_params)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "HVM_boot_params", boot_params);
            return new Response<string>("");
        }

        public Response<string> set_PV_args(string session, string opaque_ref, string pv_args)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "PV_args", pv_args);
            return new Response<string>("");
        }

        public Response<string> set_affinity(string session, string opaque_ref, string affinity)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "vm", opaque_ref, "affinity", affinity);
            return new Response<string>("");
        }

        #endregion
    }
}
