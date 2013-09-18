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
    public partial class VGPU_type : XenObject<VGPU_type>
    {
        public VGPU_type()
        {
        }

        public VGPU_type(string uuid,
            string vendor_name,
            string model_name,
            long framebuffer_size,
            long max_heads,
            List<XenRef<PGPU>> supported_on_PGPUs,
            List<XenRef<PGPU>> enabled_on_PGPUs,
            List<XenRef<VGPU>> VGPUs)
        {
            this.uuid = uuid;
            this.vendor_name = vendor_name;
            this.model_name = model_name;
            this.framebuffer_size = framebuffer_size;
            this.max_heads = max_heads;
            this.supported_on_PGPUs = supported_on_PGPUs;
            this.enabled_on_PGPUs = enabled_on_PGPUs;
            this.VGPUs = VGPUs;
        }

        /// <summary>
        /// Creates a new VGPU_type from a Proxy_VGPU_type.
        /// </summary>
        /// <param name="proxy"></param>
        public VGPU_type(Proxy_VGPU_type proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VGPU_type update)
        {
            uuid = update.uuid;
            vendor_name = update.vendor_name;
            model_name = update.model_name;
            framebuffer_size = update.framebuffer_size;
            max_heads = update.max_heads;
            supported_on_PGPUs = update.supported_on_PGPUs;
            enabled_on_PGPUs = update.enabled_on_PGPUs;
            VGPUs = update.VGPUs;
        }

        internal void UpdateFromProxy(Proxy_VGPU_type proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            vendor_name = proxy.vendor_name == null ? null : (string)proxy.vendor_name;
            model_name = proxy.model_name == null ? null : (string)proxy.model_name;
            framebuffer_size = proxy.framebuffer_size == null ? 0 : long.Parse((string)proxy.framebuffer_size);
            max_heads = proxy.max_heads == null ? 0 : long.Parse((string)proxy.max_heads);
            supported_on_PGPUs = proxy.supported_on_PGPUs == null ? null : XenRef<PGPU>.Create(proxy.supported_on_PGPUs);
            enabled_on_PGPUs = proxy.enabled_on_PGPUs == null ? null : XenRef<PGPU>.Create(proxy.enabled_on_PGPUs);
            VGPUs = proxy.VGPUs == null ? null : XenRef<VGPU>.Create(proxy.VGPUs);
        }

        public Proxy_VGPU_type ToProxy()
        {
            Proxy_VGPU_type result_ = new Proxy_VGPU_type();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.vendor_name = (vendor_name != null) ? vendor_name : "";
            result_.model_name = (model_name != null) ? model_name : "";
            result_.framebuffer_size = framebuffer_size.ToString();
            result_.max_heads = max_heads.ToString();
            result_.supported_on_PGPUs = (supported_on_PGPUs != null) ? Helper.RefListToStringArray(supported_on_PGPUs) : new string[] {};
            result_.enabled_on_PGPUs = (enabled_on_PGPUs != null) ? Helper.RefListToStringArray(enabled_on_PGPUs) : new string[] {};
            result_.VGPUs = (VGPUs != null) ? Helper.RefListToStringArray(VGPUs) : new string[] {};
            return result_;
        }

        /// <summary>
        /// Creates a new VGPU_type from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VGPU_type(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            vendor_name = Marshalling.ParseString(table, "vendor_name");
            model_name = Marshalling.ParseString(table, "model_name");
            framebuffer_size = Marshalling.ParseLong(table, "framebuffer_size");
            max_heads = Marshalling.ParseLong(table, "max_heads");
            supported_on_PGPUs = Marshalling.ParseSetRef<PGPU>(table, "supported_on_PGPUs");
            enabled_on_PGPUs = Marshalling.ParseSetRef<PGPU>(table, "enabled_on_PGPUs");
            VGPUs = Marshalling.ParseSetRef<VGPU>(table, "VGPUs");
        }

        public bool DeepEquals(VGPU_type other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._vendor_name, other._vendor_name) &&
                Helper.AreEqual2(this._model_name, other._model_name) &&
                Helper.AreEqual2(this._framebuffer_size, other._framebuffer_size) &&
                Helper.AreEqual2(this._max_heads, other._max_heads) &&
                Helper.AreEqual2(this._supported_on_PGPUs, other._supported_on_PGPUs) &&
                Helper.AreEqual2(this._enabled_on_PGPUs, other._enabled_on_PGPUs) &&
                Helper.AreEqual2(this._VGPUs, other._VGPUs);
        }

        public override string SaveChanges(Session session, string opaqueRef, VGPU_type server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }

        public static VGPU_type get_record(Session session, string _vgpu_type)
        {
            return new VGPU_type((Proxy_VGPU_type)session.proxy.vgpu_type_get_record(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        public static XenRef<VGPU_type> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VGPU_type>.Create(session.proxy.vgpu_type_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _vgpu_type)
        {
            return (string)session.proxy.vgpu_type_get_uuid(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        public static string get_vendor_name(Session session, string _vgpu_type)
        {
            return (string)session.proxy.vgpu_type_get_vendor_name(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        public static string get_model_name(Session session, string _vgpu_type)
        {
            return (string)session.proxy.vgpu_type_get_model_name(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        public static long get_framebuffer_size(Session session, string _vgpu_type)
        {
            return long.Parse((string)session.proxy.vgpu_type_get_framebuffer_size(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        public static long get_max_heads(Session session, string _vgpu_type)
        {
            return long.Parse((string)session.proxy.vgpu_type_get_max_heads(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        public static List<XenRef<PGPU>> get_supported_on_PGPUs(Session session, string _vgpu_type)
        {
            return XenRef<PGPU>.Create(session.proxy.vgpu_type_get_supported_on_pgpus(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        public static List<XenRef<PGPU>> get_enabled_on_PGPUs(Session session, string _vgpu_type)
        {
            return XenRef<PGPU>.Create(session.proxy.vgpu_type_get_enabled_on_pgpus(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        public static List<XenRef<VGPU>> get_VGPUs(Session session, string _vgpu_type)
        {
            return XenRef<VGPU>.Create(session.proxy.vgpu_type_get_vgpus(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        public static List<XenRef<VGPU_type>> get_all(Session session)
        {
            return XenRef<VGPU_type>.Create(session.proxy.vgpu_type_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VGPU_type>, VGPU_type> get_all_records(Session session)
        {
            return XenRef<VGPU_type>.Create<Proxy_VGPU_type>(session.proxy.vgpu_type_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private string _vendor_name;
        public virtual string vendor_name {
             get { return _vendor_name; }
             set { if (!Helper.AreEqual(value, _vendor_name)) { _vendor_name = value; Changed = true; NotifyPropertyChanged("vendor_name"); } }
         }

        private string _model_name;
        public virtual string model_name {
             get { return _model_name; }
             set { if (!Helper.AreEqual(value, _model_name)) { _model_name = value; Changed = true; NotifyPropertyChanged("model_name"); } }
         }

        private long _framebuffer_size;
        public virtual long framebuffer_size {
             get { return _framebuffer_size; }
             set { if (!Helper.AreEqual(value, _framebuffer_size)) { _framebuffer_size = value; Changed = true; NotifyPropertyChanged("framebuffer_size"); } }
         }

        private long _max_heads;
        public virtual long max_heads {
             get { return _max_heads; }
             set { if (!Helper.AreEqual(value, _max_heads)) { _max_heads = value; Changed = true; NotifyPropertyChanged("max_heads"); } }
         }

        private List<XenRef<PGPU>> _supported_on_PGPUs;
        public virtual List<XenRef<PGPU>> supported_on_PGPUs {
             get { return _supported_on_PGPUs; }
             set { if (!Helper.AreEqual(value, _supported_on_PGPUs)) { _supported_on_PGPUs = value; Changed = true; NotifyPropertyChanged("supported_on_PGPUs"); } }
         }

        private List<XenRef<PGPU>> _enabled_on_PGPUs;
        public virtual List<XenRef<PGPU>> enabled_on_PGPUs {
             get { return _enabled_on_PGPUs; }
             set { if (!Helper.AreEqual(value, _enabled_on_PGPUs)) { _enabled_on_PGPUs = value; Changed = true; NotifyPropertyChanged("enabled_on_PGPUs"); } }
         }

        private List<XenRef<VGPU>> _VGPUs;
        public virtual List<XenRef<VGPU>> VGPUs {
             get { return _VGPUs; }
             set { if (!Helper.AreEqual(value, _VGPUs)) { _VGPUs = value; Changed = true; NotifyPropertyChanged("VGPUs"); } }
         }


    }
}
