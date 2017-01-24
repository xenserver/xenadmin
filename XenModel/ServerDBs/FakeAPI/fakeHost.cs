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
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal class fakeHost : fakeXenObject
    {
        public fakeHost(DbProxy proxy)
            : base("host", proxy)
        {

        }

        // For IXenConnection.Heartbeat() 
        public Response<DateTime> get_servertime()
        {
            return new Response<DateTime>(DateTime.UtcNow);
        }


        public Response<DateTime> get_server_localtime(string session, string host)
        {
            return new Response<DateTime>(DateTime.Now);
        }

        public Response<string> set_license_server(string dummy, string hostOpaqueRef, Hashtable serverDetails)
        {
            if (proxy.db.GetValue("host", hostOpaqueRef, "license_server") != null)
            {
                proxy.EditObject_(DbProxy.EditTypes.Replace, "host", hostOpaqueRef, "license_server", serverDetails);
                return new Response<string>("");
            }
            return new Response<string>(true, new string[] { Failure.LICENSE_CHECKOUT_ERROR });
        }

        public Response<string> license_apply(string dummy, string hostOpaqueRef, string licFileBase64)
        {
            ActivationKeyParser parser = new ActivationKeyParser(licFileBase64);

            Hashtable license_params = (Hashtable)proxy.db.GetValue("host", hostOpaqueRef, "license_params");

            license_params["sku_type"] = parser.sku_type;
            license_params["version"] = parser.version;
            license_params["productcode"] = parser.productcode;
            license_params["serialnumber"] = parser.serialnumber;
            license_params["sockets"] = parser.sockets;
            DateTime expiry = DateTime.Parse(parser.human_readable_expiry, CultureInfo.InvariantCulture);
            license_params["expiry"] = string.Format("{0}{1}{2}T00:00:00Z", expiry.Year, expiry.Month.ToString("00"), expiry.Day.ToString("00"));
            license_params["name"] = parser.name;
            license_params["address1"] = parser.address1;
            license_params["address2"] = parser.address2;
            license_params["city"] = parser.city;
            license_params["state"] = parser.state;
            license_params["postalcode"] = parser.postalcode;
            license_params["country"] = parser.country;
            license_params["company"] = parser.company;

            proxy.EditObject_(DbProxy.EditTypes.Replace, "host", hostOpaqueRef, "license_params", license_params);

            return new Response<string>("");
        }

        public Response<string> apply_edition(string dummy, string hostOpaqueRef, string edition)
        {
            if (proxy.db.GetValue("host", hostOpaqueRef, "edition") != null)
            {
                proxy.EditObject_(DbProxy.EditTypes.Replace, "host", hostOpaqueRef, "edition", edition);
                return new Response<string>("");
            }
            return new Response<string>(true, new string[] { Failure.LICENSE_CHECKOUT_ERROR });
        }

        public Response<string> reboot(string session, string opaque_ref)
        {
            string metrics_ref = (string)proxy.db.GetValue("host", opaque_ref, "metrics");
            proxy.EditObject_(DbProxy.EditTypes.Replace, "host_metrics", metrics_ref, "live", false);

            string master_ref = "";
            foreach (string pool in proxy.db.Tables["pool"].Rows.Keys)
            {
                master_ref = (string)proxy.db.GetValue("pool", pool, "master");
                break;
            }

            if (opaque_ref == master_ref)
            {
                proxy.MarkToDisconnect = true;
            }

            // run in background so that reboot command returns before connection is lost.
            Thread t = new Thread(delegate()
            {
                while (proxy.MarkToDisconnect)
                    Thread.Sleep(1000);

                Thread.Sleep(4000);

                proxy.EditObject_(DbProxy.EditTypes.Replace, "host_metrics", metrics_ref, "live", true);
                proxy.EditObject_(DbProxy.EditTypes.AddToDict, "host", opaque_ref, "other_config", "boot_time", Util.ToUnixTime(DateTime.Now).ToString());

                Thread.Sleep(2000);

                enable(session, opaque_ref);
            });
            t.IsBackground = true;
            t.Start();

            return new Response<string>("");
        }

        public Response<bool> get_enabled(string session, string opaque_ref)
        {
            return new Response<bool>((bool)proxy.db.GetValue("host", opaque_ref, "enabled"));
        }

        public Response<object> migrate_receive(string session, string host, string network, Hashtable options)
        {
            //Just return some data that this call was sent - doesn't reflect what xapi will send
            //as xapi's data from this call should be opaque
            return new Response<object>(new Hashtable(new Dictionary<string,string>
            {
                {"session", session}, 
                {"host", host},
                {"network", network},
            }));
        }
        

        public Response<string> enable(string session, string opaque_ref)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "host", opaque_ref, "enabled", true);
            return new Response<string>("");
        }

        public Response<string> disable(string session, string opaque_ref)
        {
            proxy.EditObject_(DbProxy.EditTypes.Replace, "host", opaque_ref, "enabled", false);
            return new Response<string>("");
        }

        public Response<string> evacuate(string session, string opaque_ref)
        {
            Host host = proxy.connection.Resolve(new XenRef<Host>(opaque_ref));
            if (host == null)
                return new Response<string>(true, new string[] { Failure.INTERNAL_ERROR });
            int i = 0;
            Host[] hosts = host.Connection.Cache.Hosts;
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (vm.power_state != vm_power_state.Running)
                    continue;

                Host dest_host = FindDestHost(hosts, host, ref i);
                if (dest_host == null)
                {
                    fakeVM.hard_shutdown(proxy, vm.opaque_ref);
                }
                else
                {
                    fakeVM.pool_migrate(proxy, vm.opaque_ref, opaque_ref, dest_host.opaque_ref);
                }
            }
            return new Response<string>("");
        }

        private static Host FindDestHost(Host[] hosts, Host src_host, ref int i)
        {
            int old_i = i;
            do
            {
                i++;
                if (i >= hosts.Length)
                    i = 0;
                if (hosts[i] != src_host)
                    return hosts[i];
            } while (i != old_i);

            return null;
        }

        public Response<string> get_by_uuid(string session, string uuid)
        {
            return new Response<string>((string)proxy.db.ObjectWithFieldValue("host", "uuid", uuid));
        }

        public Response<string> shutdown(string session, string opaque_ref)
        {
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "host", opaque_ref, "allowed_operations", "vm_migrate");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "host", opaque_ref, "allowed_operations", "evacuate");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "host", opaque_ref, "allowed_operations", "provision");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "host", opaque_ref, "allowed_operations", "vm_resume");
            proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "host", opaque_ref, "allowed_operations", "vm_start");
            proxy.EditObject_(DbProxy.EditTypes.AddToArray, "host", opaque_ref, "allowed_operations", "power_on");
            //proxy.EditObject_(DbProxy.EditTypes.Replace, "host", opaque_ref, "power_on_mode", "wake-on-lan");

            string metricsOpaqueRef = proxy.db.Tables["host"].Rows[opaque_ref].Props["metrics"].XapiObjectValue.ToString();
            proxy.EditObject_(DbProxy.EditTypes.Replace, "host_metrics", metricsOpaqueRef, "live", false);

            return new Response<string>("");

        }

        public Response<string> call_plugin(string session, string master_opaque_ref, string arg1, string arg2, Hashtable arg3)
        {
            if (arg1 == "power-on-host")
            {
                string hostToPowerOnUuid = (string)arg3["remote_host_uuid"];
                string hostToPowerOnOpaqueRef = null;

                foreach (string oRef in proxy.db.Tables["host"].Rows.Keys)
                {
                    if (proxy.db.Tables["host"].Rows[oRef].Props["uuid"].XapiObjectValue.ToString() == hostToPowerOnUuid)
                    {
                        hostToPowerOnOpaqueRef = oRef;
                        break;
                    }
                }

                proxy.EditObject_(DbProxy.EditTypes.AddToArray, "host", hostToPowerOnOpaqueRef, "allowed_operations", "vm_migrate");
                proxy.EditObject_(DbProxy.EditTypes.AddToArray, "host", hostToPowerOnOpaqueRef, "allowed_operations", "evacuate");
                proxy.EditObject_(DbProxy.EditTypes.AddToArray, "host", hostToPowerOnOpaqueRef, "allowed_operations", "provision");
                proxy.EditObject_(DbProxy.EditTypes.AddToArray, "host", hostToPowerOnOpaqueRef, "allowed_operations", "vm_resume");
                proxy.EditObject_(DbProxy.EditTypes.AddToArray, "host", hostToPowerOnOpaqueRef, "allowed_operations", "vm_start");
                proxy.EditObject_(DbProxy.EditTypes.RemoveFromArray, "host", hostToPowerOnOpaqueRef, "allowed_operations", "power_on");
                proxy.EditObject_(DbProxy.EditTypes.Replace, "host", hostToPowerOnOpaqueRef, "power_on_mode", "wake-on-lan");
                proxy.EditObject_(DbProxy.EditTypes.Replace, "host", hostToPowerOnOpaqueRef, "enabled", true);

                string metricsOpaqueRef = proxy.db.Tables["host"].Rows[hostToPowerOnOpaqueRef].Props["metrics"].XapiObjectValue.ToString();
                proxy.EditObject_(DbProxy.EditTypes.Replace, "host_metrics", metricsOpaqueRef, "live", true);

                return new Response<string>("True");
            }
            else
            {
                return new Response<String>(true, new String[] { Failure.INTERNAL_ERROR });
            }

        }
    }
}
