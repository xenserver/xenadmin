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
    /// A console
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Console : XenObject<Console>
    {
        public Console()
        {
        }

        public Console(string uuid,
            console_protocol protocol,
            string location,
            XenRef<VM> VM,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.protocol = protocol;
            this.location = location;
            this.VM = VM;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Console from a Proxy_Console.
        /// </summary>
        /// <param name="proxy"></param>
        public Console(Proxy_Console proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Console update)
        {
            uuid = update.uuid;
            protocol = update.protocol;
            location = update.location;
            VM = update.VM;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Console proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            protocol = proxy.protocol == null ? (console_protocol) 0 : (console_protocol)Helper.EnumParseDefault(typeof(console_protocol), (string)proxy.protocol);
            location = proxy.location == null ? null : (string)proxy.location;
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Console ToProxy()
        {
            Proxy_Console result_ = new Proxy_Console();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.protocol = console_protocol_helper.ToString(protocol);
            result_.location = (location != null) ? location : "";
            result_.VM = (VM != null) ? VM : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Console from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Console(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            protocol = (console_protocol)Helper.EnumParseDefault(typeof(console_protocol), Marshalling.ParseString(table, "protocol"));
            location = Marshalling.ParseString(table, "location");
            VM = Marshalling.ParseRef<VM>(table, "VM");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Console other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._protocol, other._protocol) &&
                Helper.AreEqual2(this._location, other._location) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Console server)
        {
            if (opaqueRef == null)
            {
                Proxy_Console p = this.ToProxy();
                return session.proxy.console_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Console.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static Console get_record(Session session, string _console)
        {
            return new Console((Proxy_Console)session.proxy.console_get_record(session.uuid, (_console != null) ? _console : "").parse());
        }

        /// <summary>
        /// Get a reference to the console instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Console> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Console>.Create(session.proxy.console_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Create a new console instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Console> create(Session session, Console _record)
        {
            return XenRef<Console>.Create(session.proxy.console_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new console instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, Console _record)
        {
            return XenRef<Task>.Create(session.proxy.async_console_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified console instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static void destroy(Session session, string _console)
        {
            session.proxy.console_destroy(session.uuid, (_console != null) ? _console : "").parse();
        }

        /// <summary>
        /// Destroy the specified console instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static XenRef<Task> async_destroy(Session session, string _console)
        {
            return XenRef<Task>.Create(session.proxy.async_console_destroy(session.uuid, (_console != null) ? _console : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static string get_uuid(Session session, string _console)
        {
            return (string)session.proxy.console_get_uuid(session.uuid, (_console != null) ? _console : "").parse();
        }

        /// <summary>
        /// Get the protocol field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static console_protocol get_protocol(Session session, string _console)
        {
            return (console_protocol)Helper.EnumParseDefault(typeof(console_protocol), (string)session.proxy.console_get_protocol(session.uuid, (_console != null) ? _console : "").parse());
        }

        /// <summary>
        /// Get the location field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static string get_location(Session session, string _console)
        {
            return (string)session.proxy.console_get_location(session.uuid, (_console != null) ? _console : "").parse();
        }

        /// <summary>
        /// Get the VM field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static XenRef<VM> get_VM(Session session, string _console)
        {
            return XenRef<VM>.Create(session.proxy.console_get_vm(session.uuid, (_console != null) ? _console : "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        public static Dictionary<string, string> get_other_config(Session session, string _console)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.console_get_other_config(session.uuid, (_console != null) ? _console : "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _console, Dictionary<string, string> _other_config)
        {
            session.proxy.console_set_other_config(session.uuid, (_console != null) ? _console : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _console, string _key, string _value)
        {
            session.proxy.console_add_to_other_config(session.uuid, (_console != null) ? _console : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given console.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _console, string _key)
        {
            session.proxy.console_remove_from_other_config(session.uuid, (_console != null) ? _console : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Return a list of all the consoles known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Console>> get_all(Session session)
        {
            return XenRef<Console>.Create(session.proxy.console_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the console Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Console>, Console> get_all_records(Session session)
        {
            return XenRef<Console>.Create<Proxy_Console>(session.proxy.console_get_all_records(session.uuid).parse());
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
        /// the protocol used by this console
        /// </summary>
        public virtual console_protocol protocol
        {
            get { return _protocol; }
            set
            {
                if (!Helper.AreEqual(value, _protocol))
                {
                    _protocol = value;
                    Changed = true;
                    NotifyPropertyChanged("protocol");
                }
            }
        }
        private console_protocol _protocol;

        /// <summary>
        /// URI for the console service
        /// </summary>
        public virtual string location
        {
            get { return _location; }
            set
            {
                if (!Helper.AreEqual(value, _location))
                {
                    _location = value;
                    Changed = true;
                    NotifyPropertyChanged("location");
                }
            }
        }
        private string _location;

        /// <summary>
        /// VM to which this console is attached
        /// </summary>
        public virtual XenRef<VM> VM
        {
            get { return _VM; }
            set
            {
                if (!Helper.AreEqual(value, _VM))
                {
                    _VM = value;
                    Changed = true;
                    NotifyPropertyChanged("VM");
                }
            }
        }
        private XenRef<VM> _VM;

        /// <summary>
        /// additional configuration
        /// </summary>
        public virtual Dictionary<string, string> other_config
        {
            get { return _other_config; }
            set
            {
                if (!Helper.AreEqual(value, _other_config))
                {
                    _other_config = value;
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config;
    }
}
