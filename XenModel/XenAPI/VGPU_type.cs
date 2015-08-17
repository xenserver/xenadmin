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
    /// <summary>
    /// A type of virtual GPU
    /// First published in XenServer 6.2 SP1 Tech-Preview.
    /// </summary>
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
            long max_resolution_x,
            long max_resolution_y,
            List<XenRef<PGPU>> supported_on_PGPUs,
            List<XenRef<PGPU>> enabled_on_PGPUs,
            List<XenRef<VGPU>> VGPUs,
            List<XenRef<GPU_group>> supported_on_GPU_groups,
            List<XenRef<GPU_group>> enabled_on_GPU_groups,
            vgpu_type_implementation implementation,
            string identifier,
            bool experimental)
        {
            this.uuid = uuid;
            this.vendor_name = vendor_name;
            this.model_name = model_name;
            this.framebuffer_size = framebuffer_size;
            this.max_heads = max_heads;
            this.max_resolution_x = max_resolution_x;
            this.max_resolution_y = max_resolution_y;
            this.supported_on_PGPUs = supported_on_PGPUs;
            this.enabled_on_PGPUs = enabled_on_PGPUs;
            this.VGPUs = VGPUs;
            this.supported_on_GPU_groups = supported_on_GPU_groups;
            this.enabled_on_GPU_groups = enabled_on_GPU_groups;
            this.implementation = implementation;
            this.identifier = identifier;
            this.experimental = experimental;
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
            max_resolution_x = update.max_resolution_x;
            max_resolution_y = update.max_resolution_y;
            supported_on_PGPUs = update.supported_on_PGPUs;
            enabled_on_PGPUs = update.enabled_on_PGPUs;
            VGPUs = update.VGPUs;
            supported_on_GPU_groups = update.supported_on_GPU_groups;
            enabled_on_GPU_groups = update.enabled_on_GPU_groups;
            implementation = update.implementation;
            identifier = update.identifier;
            experimental = update.experimental;
        }

        internal void UpdateFromProxy(Proxy_VGPU_type proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            vendor_name = proxy.vendor_name == null ? null : (string)proxy.vendor_name;
            model_name = proxy.model_name == null ? null : (string)proxy.model_name;
            framebuffer_size = proxy.framebuffer_size == null ? 0 : long.Parse((string)proxy.framebuffer_size);
            max_heads = proxy.max_heads == null ? 0 : long.Parse((string)proxy.max_heads);
            max_resolution_x = proxy.max_resolution_x == null ? 0 : long.Parse((string)proxy.max_resolution_x);
            max_resolution_y = proxy.max_resolution_y == null ? 0 : long.Parse((string)proxy.max_resolution_y);
            supported_on_PGPUs = proxy.supported_on_PGPUs == null ? null : XenRef<PGPU>.Create(proxy.supported_on_PGPUs);
            enabled_on_PGPUs = proxy.enabled_on_PGPUs == null ? null : XenRef<PGPU>.Create(proxy.enabled_on_PGPUs);
            VGPUs = proxy.VGPUs == null ? null : XenRef<VGPU>.Create(proxy.VGPUs);
            supported_on_GPU_groups = proxy.supported_on_GPU_groups == null ? null : XenRef<GPU_group>.Create(proxy.supported_on_GPU_groups);
            enabled_on_GPU_groups = proxy.enabled_on_GPU_groups == null ? null : XenRef<GPU_group>.Create(proxy.enabled_on_GPU_groups);
            implementation = proxy.implementation == null ? (vgpu_type_implementation) 0 : (vgpu_type_implementation)Helper.EnumParseDefault(typeof(vgpu_type_implementation), (string)proxy.implementation);
            identifier = proxy.identifier == null ? null : (string)proxy.identifier;
            experimental = (bool)proxy.experimental;
        }

        public Proxy_VGPU_type ToProxy()
        {
            Proxy_VGPU_type result_ = new Proxy_VGPU_type();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.vendor_name = (vendor_name != null) ? vendor_name : "";
            result_.model_name = (model_name != null) ? model_name : "";
            result_.framebuffer_size = framebuffer_size.ToString();
            result_.max_heads = max_heads.ToString();
            result_.max_resolution_x = max_resolution_x.ToString();
            result_.max_resolution_y = max_resolution_y.ToString();
            result_.supported_on_PGPUs = (supported_on_PGPUs != null) ? Helper.RefListToStringArray(supported_on_PGPUs) : new string[] {};
            result_.enabled_on_PGPUs = (enabled_on_PGPUs != null) ? Helper.RefListToStringArray(enabled_on_PGPUs) : new string[] {};
            result_.VGPUs = (VGPUs != null) ? Helper.RefListToStringArray(VGPUs) : new string[] {};
            result_.supported_on_GPU_groups = (supported_on_GPU_groups != null) ? Helper.RefListToStringArray(supported_on_GPU_groups) : new string[] {};
            result_.enabled_on_GPU_groups = (enabled_on_GPU_groups != null) ? Helper.RefListToStringArray(enabled_on_GPU_groups) : new string[] {};
            result_.implementation = vgpu_type_implementation_helper.ToString(implementation);
            result_.identifier = (identifier != null) ? identifier : "";
            result_.experimental = experimental;
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
            max_resolution_x = Marshalling.ParseLong(table, "max_resolution_x");
            max_resolution_y = Marshalling.ParseLong(table, "max_resolution_y");
            supported_on_PGPUs = Marshalling.ParseSetRef<PGPU>(table, "supported_on_PGPUs");
            enabled_on_PGPUs = Marshalling.ParseSetRef<PGPU>(table, "enabled_on_PGPUs");
            VGPUs = Marshalling.ParseSetRef<VGPU>(table, "VGPUs");
            supported_on_GPU_groups = Marshalling.ParseSetRef<GPU_group>(table, "supported_on_GPU_groups");
            enabled_on_GPU_groups = Marshalling.ParseSetRef<GPU_group>(table, "enabled_on_GPU_groups");
            implementation = (vgpu_type_implementation)Helper.EnumParseDefault(typeof(vgpu_type_implementation), Marshalling.ParseString(table, "implementation"));
            identifier = Marshalling.ParseString(table, "identifier");
            experimental = Marshalling.ParseBool(table, "experimental");
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
                Helper.AreEqual2(this._max_resolution_x, other._max_resolution_x) &&
                Helper.AreEqual2(this._max_resolution_y, other._max_resolution_y) &&
                Helper.AreEqual2(this._supported_on_PGPUs, other._supported_on_PGPUs) &&
                Helper.AreEqual2(this._enabled_on_PGPUs, other._enabled_on_PGPUs) &&
                Helper.AreEqual2(this._VGPUs, other._VGPUs) &&
                Helper.AreEqual2(this._supported_on_GPU_groups, other._supported_on_GPU_groups) &&
                Helper.AreEqual2(this._enabled_on_GPU_groups, other._enabled_on_GPU_groups) &&
                Helper.AreEqual2(this._implementation, other._implementation) &&
                Helper.AreEqual2(this._identifier, other._identifier) &&
                Helper.AreEqual2(this._experimental, other._experimental);
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
        /// <summary>
        /// Get a record containing the current state of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static VGPU_type get_record(Session session, string _vgpu_type)
        {
            return new VGPU_type((Proxy_VGPU_type)session.proxy.vgpu_type_get_record(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get a reference to the VGPU_type instance with the specified UUID.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VGPU_type> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VGPU_type>.Create(session.proxy.vgpu_type_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static string get_uuid(Session session, string _vgpu_type)
        {
            return (string)session.proxy.vgpu_type_get_uuid(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        /// <summary>
        /// Get the vendor_name field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static string get_vendor_name(Session session, string _vgpu_type)
        {
            return (string)session.proxy.vgpu_type_get_vendor_name(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        /// <summary>
        /// Get the model_name field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static string get_model_name(Session session, string _vgpu_type)
        {
            return (string)session.proxy.vgpu_type_get_model_name(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        /// <summary>
        /// Get the framebuffer_size field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static long get_framebuffer_size(Session session, string _vgpu_type)
        {
            return long.Parse((string)session.proxy.vgpu_type_get_framebuffer_size(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the max_heads field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static long get_max_heads(Session session, string _vgpu_type)
        {
            return long.Parse((string)session.proxy.vgpu_type_get_max_heads(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the max_resolution_x field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static long get_max_resolution_x(Session session, string _vgpu_type)
        {
            return long.Parse((string)session.proxy.vgpu_type_get_max_resolution_x(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the max_resolution_y field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static long get_max_resolution_y(Session session, string _vgpu_type)
        {
            return long.Parse((string)session.proxy.vgpu_type_get_max_resolution_y(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the supported_on_PGPUs field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static List<XenRef<PGPU>> get_supported_on_PGPUs(Session session, string _vgpu_type)
        {
            return XenRef<PGPU>.Create(session.proxy.vgpu_type_get_supported_on_pgpus(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the enabled_on_PGPUs field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static List<XenRef<PGPU>> get_enabled_on_PGPUs(Session session, string _vgpu_type)
        {
            return XenRef<PGPU>.Create(session.proxy.vgpu_type_get_enabled_on_pgpus(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the VGPUs field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static List<XenRef<VGPU>> get_VGPUs(Session session, string _vgpu_type)
        {
            return XenRef<VGPU>.Create(session.proxy.vgpu_type_get_vgpus(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the supported_on_GPU_groups field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static List<XenRef<GPU_group>> get_supported_on_GPU_groups(Session session, string _vgpu_type)
        {
            return XenRef<GPU_group>.Create(session.proxy.vgpu_type_get_supported_on_gpu_groups(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the enabled_on_GPU_groups field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static List<XenRef<GPU_group>> get_enabled_on_GPU_groups(Session session, string _vgpu_type)
        {
            return XenRef<GPU_group>.Create(session.proxy.vgpu_type_get_enabled_on_gpu_groups(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the implementation field of the given VGPU_type.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static vgpu_type_implementation get_implementation(Session session, string _vgpu_type)
        {
            return (vgpu_type_implementation)Helper.EnumParseDefault(typeof(vgpu_type_implementation), (string)session.proxy.vgpu_type_get_implementation(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse());
        }

        /// <summary>
        /// Get the identifier field of the given VGPU_type.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static string get_identifier(Session session, string _vgpu_type)
        {
            return (string)session.proxy.vgpu_type_get_identifier(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        /// <summary>
        /// Get the experimental field of the given VGPU_type.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        public static bool get_experimental(Session session, string _vgpu_type)
        {
            return (bool)session.proxy.vgpu_type_get_experimental(session.uuid, (_vgpu_type != null) ? _vgpu_type : "").parse();
        }

        /// <summary>
        /// Return a list of all the VGPU_types known to the system.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VGPU_type>> get_all(Session session)
        {
            return XenRef<VGPU_type>.Create(session.proxy.vgpu_type_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the VGPU_type Records at once, in a single XML RPC call
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VGPU_type>, VGPU_type> get_all_records(Session session)
        {
            return XenRef<VGPU_type>.Create<Proxy_VGPU_type>(session.proxy.vgpu_type_get_all_records(session.uuid).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid;

        /// <summary>
        /// Name of VGPU vendor
        /// </summary>
        public virtual string vendor_name
        {
            get { return _vendor_name; }
            set
            {
                if (!Helper.AreEqual(value, _vendor_name))
                {
                    _vendor_name = value;
                    Changed = true;
                    NotifyPropertyChanged("vendor_name");
                }
            }
        }
        private string _vendor_name;

        /// <summary>
        /// Model name associated with the VGPU type
        /// </summary>
        public virtual string model_name
        {
            get { return _model_name; }
            set
            {
                if (!Helper.AreEqual(value, _model_name))
                {
                    _model_name = value;
                    Changed = true;
                    NotifyPropertyChanged("model_name");
                }
            }
        }
        private string _model_name;

        /// <summary>
        /// Framebuffer size of the VGPU type, in bytes
        /// </summary>
        public virtual long framebuffer_size
        {
            get { return _framebuffer_size; }
            set
            {
                if (!Helper.AreEqual(value, _framebuffer_size))
                {
                    _framebuffer_size = value;
                    Changed = true;
                    NotifyPropertyChanged("framebuffer_size");
                }
            }
        }
        private long _framebuffer_size;

        /// <summary>
        /// Maximum number of displays supported by the VGPU type
        /// </summary>
        public virtual long max_heads
        {
            get { return _max_heads; }
            set
            {
                if (!Helper.AreEqual(value, _max_heads))
                {
                    _max_heads = value;
                    Changed = true;
                    NotifyPropertyChanged("max_heads");
                }
            }
        }
        private long _max_heads;

        /// <summary>
        /// Maximum resolution (width) supported by the VGPU type
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual long max_resolution_x
        {
            get { return _max_resolution_x; }
            set
            {
                if (!Helper.AreEqual(value, _max_resolution_x))
                {
                    _max_resolution_x = value;
                    Changed = true;
                    NotifyPropertyChanged("max_resolution_x");
                }
            }
        }
        private long _max_resolution_x;

        /// <summary>
        /// Maximum resolution (height) supported by the VGPU type
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual long max_resolution_y
        {
            get { return _max_resolution_y; }
            set
            {
                if (!Helper.AreEqual(value, _max_resolution_y))
                {
                    _max_resolution_y = value;
                    Changed = true;
                    NotifyPropertyChanged("max_resolution_y");
                }
            }
        }
        private long _max_resolution_y;

        /// <summary>
        /// List of PGPUs that support this VGPU type
        /// </summary>
        public virtual List<XenRef<PGPU>> supported_on_PGPUs
        {
            get { return _supported_on_PGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _supported_on_PGPUs))
                {
                    _supported_on_PGPUs = value;
                    Changed = true;
                    NotifyPropertyChanged("supported_on_PGPUs");
                }
            }
        }
        private List<XenRef<PGPU>> _supported_on_PGPUs;

        /// <summary>
        /// List of PGPUs that have this VGPU type enabled
        /// </summary>
        public virtual List<XenRef<PGPU>> enabled_on_PGPUs
        {
            get { return _enabled_on_PGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _enabled_on_PGPUs))
                {
                    _enabled_on_PGPUs = value;
                    Changed = true;
                    NotifyPropertyChanged("enabled_on_PGPUs");
                }
            }
        }
        private List<XenRef<PGPU>> _enabled_on_PGPUs;

        /// <summary>
        /// List of VGPUs of this type
        /// </summary>
        public virtual List<XenRef<VGPU>> VGPUs
        {
            get { return _VGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _VGPUs))
                {
                    _VGPUs = value;
                    Changed = true;
                    NotifyPropertyChanged("VGPUs");
                }
            }
        }
        private List<XenRef<VGPU>> _VGPUs;

        /// <summary>
        /// List of GPU groups in which at least one PGPU supports this VGPU type
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual List<XenRef<GPU_group>> supported_on_GPU_groups
        {
            get { return _supported_on_GPU_groups; }
            set
            {
                if (!Helper.AreEqual(value, _supported_on_GPU_groups))
                {
                    _supported_on_GPU_groups = value;
                    Changed = true;
                    NotifyPropertyChanged("supported_on_GPU_groups");
                }
            }
        }
        private List<XenRef<GPU_group>> _supported_on_GPU_groups;

        /// <summary>
        /// List of GPU groups in which at least one have this VGPU type enabled
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual List<XenRef<GPU_group>> enabled_on_GPU_groups
        {
            get { return _enabled_on_GPU_groups; }
            set
            {
                if (!Helper.AreEqual(value, _enabled_on_GPU_groups))
                {
                    _enabled_on_GPU_groups = value;
                    Changed = true;
                    NotifyPropertyChanged("enabled_on_GPU_groups");
                }
            }
        }
        private List<XenRef<GPU_group>> _enabled_on_GPU_groups;

        /// <summary>
        /// The internal implementation of this VGPU type
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual vgpu_type_implementation implementation
        {
            get { return _implementation; }
            set
            {
                if (!Helper.AreEqual(value, _implementation))
                {
                    _implementation = value;
                    Changed = true;
                    NotifyPropertyChanged("implementation");
                }
            }
        }
        private vgpu_type_implementation _implementation;

        /// <summary>
        /// Key used to identify VGPU types and avoid creating duplicates - this field is used internally and not intended for interpretation by API clients
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual string identifier
        {
            get { return _identifier; }
            set
            {
                if (!Helper.AreEqual(value, _identifier))
                {
                    _identifier = value;
                    Changed = true;
                    NotifyPropertyChanged("identifier");
                }
            }
        }
        private string _identifier;

        /// <summary>
        /// Indicates whether VGPUs of this type should be considered experimental
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual bool experimental
        {
            get { return _experimental; }
            set
            {
                if (!Helper.AreEqual(value, _experimental))
                {
                    _experimental = value;
                    Changed = true;
                    NotifyPropertyChanged("experimental");
                }
            }
        }
        private bool _experimental;
    }
}
