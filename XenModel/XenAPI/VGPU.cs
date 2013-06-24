/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;

using CookComputing.XmlRpc;


namespace XenAPI
{
    public partial class VGPU : XenObject<VGPU>
    {
        public VGPU()
        {
        }

        public VGPU(string uuid,
            XenRef<VM> VM,
            XenRef<GPU_group> GPU_group,
            string device,
            bool currently_attached,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.VM = VM;
            this.GPU_group = GPU_group;
            this.device = device;
            this.currently_attached = currently_attached;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new VGPU from a Proxy_VGPU.
        /// </summary>
        /// <param name="proxy"></param>
        public VGPU(Proxy_VGPU proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VGPU update)
        {
            uuid = update.uuid;
            VM = update.VM;
            GPU_group = update.GPU_group;
            device = update.device;
            currently_attached = update.currently_attached;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_VGPU proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            GPU_group = proxy.GPU_group == null ? null : XenRef<GPU_group>.Create(proxy.GPU_group);
            device = proxy.device == null ? null : (string)proxy.device;
            currently_attached = (bool)proxy.currently_attached;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_VGPU ToProxy()
        {
            Proxy_VGPU result_ = new Proxy_VGPU();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.VM = (VM != null) ? VM : "";
            result_.GPU_group = (GPU_group != null) ? GPU_group : "";
            result_.device = (device != null) ? device : "";
            result_.currently_attached = currently_attached;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new VGPU from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VGPU(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            VM = Marshalling.ParseRef<VM>(table, "VM");
            GPU_group = Marshalling.ParseRef<GPU_group>(table, "GPU_group");
            device = Marshalling.ParseString(table, "device");
            currently_attached = Marshalling.ParseBool(table, "currently_attached");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(VGPU other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._GPU_group, other._GPU_group) &&
                Helper.AreEqual2(this._device, other._device) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, VGPU server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    VGPU.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static VGPU get_record(Session session, string _vgpu)
        {
            return new VGPU((Proxy_VGPU)session.proxy.vgpu_get_record(session.uuid, (_vgpu != null) ? _vgpu : "").parse());
        }

        public static XenRef<VGPU> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VGPU>.Create(session.proxy.vgpu_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _vgpu)
        {
            return (string)session.proxy.vgpu_get_uuid(session.uuid, (_vgpu != null) ? _vgpu : "").parse();
        }

        public static XenRef<VM> get_VM(Session session, string _vgpu)
        {
            return XenRef<VM>.Create(session.proxy.vgpu_get_vm(session.uuid, (_vgpu != null) ? _vgpu : "").parse());
        }

        public static XenRef<GPU_group> get_GPU_group(Session session, string _vgpu)
        {
            return XenRef<GPU_group>.Create(session.proxy.vgpu_get_gpu_group(session.uuid, (_vgpu != null) ? _vgpu : "").parse());
        }

        public static string get_device(Session session, string _vgpu)
        {
            return (string)session.proxy.vgpu_get_device(session.uuid, (_vgpu != null) ? _vgpu : "").parse();
        }

        public static bool get_currently_attached(Session session, string _vgpu)
        {
            return (bool)session.proxy.vgpu_get_currently_attached(session.uuid, (_vgpu != null) ? _vgpu : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _vgpu)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vgpu_get_other_config(session.uuid, (_vgpu != null) ? _vgpu : "").parse());
        }

        public static void set_other_config(Session session, string _vgpu, Dictionary<string, string> _other_config)
        {
            session.proxy.vgpu_set_other_config(session.uuid, (_vgpu != null) ? _vgpu : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _vgpu, string _key, string _value)
        {
            session.proxy.vgpu_add_to_other_config(session.uuid, (_vgpu != null) ? _vgpu : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _vgpu, string _key)
        {
            session.proxy.vgpu_remove_from_other_config(session.uuid, (_vgpu != null) ? _vgpu : "", (_key != null) ? _key : "").parse();
        }

        public static XenRef<VGPU> create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
            return XenRef<VGPU>.Create(session.proxy.vgpu_create(session.uuid, (_vm != null) ? _vm : "", (_gpu_group != null) ? _gpu_group : "", (_device != null) ? _device : "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        public static XenRef<Task> async_create(Session session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
            return XenRef<Task>.Create(session.proxy.async_vgpu_create(session.uuid, (_vm != null) ? _vm : "", (_gpu_group != null) ? _gpu_group : "", (_device != null) ? _device : "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.vgpu_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vgpu_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<XenRef<VGPU>> get_all(Session session)
        {
            return XenRef<VGPU>.Create(session.proxy.vgpu_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VGPU>, VGPU> get_all_records(Session session)
        {
            return XenRef<VGPU>.Create<Proxy_VGPU>(session.proxy.vgpu_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<VM> _VM;
        public virtual XenRef<VM> VM {
             get { return _VM; }
             set { if (!Helper.AreEqual(value, _VM)) { _VM = value; Changed = true; NotifyPropertyChanged("VM"); } }
         }

        private XenRef<GPU_group> _GPU_group;
        public virtual XenRef<GPU_group> GPU_group {
             get { return _GPU_group; }
             set { if (!Helper.AreEqual(value, _GPU_group)) { _GPU_group = value; Changed = true; NotifyPropertyChanged("GPU_group"); } }
         }

        private string _device;
        public virtual string device {
             get { return _device; }
             set { if (!Helper.AreEqual(value, _device)) { _device = value; Changed = true; NotifyPropertyChanged("device"); } }
         }

        private bool _currently_attached;
        public virtual bool currently_attached {
             get { return _currently_attached; }
             set { if (!Helper.AreEqual(value, _currently_attached)) { _currently_attached = value; Changed = true; NotifyPropertyChanged("currently_attached"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
