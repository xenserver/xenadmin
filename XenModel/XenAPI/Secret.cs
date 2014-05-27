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
    /// A secret
    /// First published in XenServer 5.6.
    /// </summary>
    public partial class Secret : XenObject<Secret>
    {
        public Secret()
        {
        }

        public Secret(string uuid,
            string value,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.value = value;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Secret from a Proxy_Secret.
        /// </summary>
        /// <param name="proxy"></param>
        public Secret(Proxy_Secret proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Secret update)
        {
            uuid = update.uuid;
            value = update.value;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Secret proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            value = proxy.value == null ? null : (string)proxy.value;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Secret ToProxy()
        {
            Proxy_Secret result_ = new Proxy_Secret();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.value = (value != null) ? value : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Secret from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Secret(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            value = Marshalling.ParseString(table, "value");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Secret other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._value, other._value) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Secret server)
        {
            if (opaqueRef == null)
            {
                Proxy_Secret p = this.ToProxy();
                return session.proxy.secret_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_value, server._value))
                {
                    Secret.set_value(session, opaqueRef, _value);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Secret.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        public static Secret get_record(Session session, string _secret)
        {
            return new Secret((Proxy_Secret)session.proxy.secret_get_record(session.uuid, (_secret != null) ? _secret : "").parse());
        }

        /// <summary>
        /// Get a reference to the secret instance with the specified UUID.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Secret> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Secret>.Create(session.proxy.secret_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Create a new secret instance, and return its handle.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Secret> create(Session session, Secret _record)
        {
            return XenRef<Secret>.Create(session.proxy.secret_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new secret instance, and return its handle.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, Secret _record)
        {
            return XenRef<Task>.Create(session.proxy.async_secret_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified secret instance.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        public static void destroy(Session session, string _secret)
        {
            session.proxy.secret_destroy(session.uuid, (_secret != null) ? _secret : "").parse();
        }

        /// <summary>
        /// Destroy the specified secret instance.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        public static XenRef<Task> async_destroy(Session session, string _secret)
        {
            return XenRef<Task>.Create(session.proxy.async_secret_destroy(session.uuid, (_secret != null) ? _secret : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        public static string get_uuid(Session session, string _secret)
        {
            return (string)session.proxy.secret_get_uuid(session.uuid, (_secret != null) ? _secret : "").parse();
        }

        /// <summary>
        /// Get the value field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        public static string get_value(Session session, string _secret)
        {
            return (string)session.proxy.secret_get_value(session.uuid, (_secret != null) ? _secret : "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        public static Dictionary<string, string> get_other_config(Session session, string _secret)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.secret_get_other_config(session.uuid, (_secret != null) ? _secret : "").parse());
        }

        /// <summary>
        /// Set the value field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_value">New value to set</param>
        public static void set_value(Session session, string _secret, string _value)
        {
            session.proxy.secret_set_value(session.uuid, (_secret != null) ? _secret : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _secret, Dictionary<string, string> _other_config)
        {
            session.proxy.secret_set_other_config(session.uuid, (_secret != null) ? _secret : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given secret.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _secret, string _key, string _value)
        {
            session.proxy.secret_add_to_other_config(session.uuid, (_secret != null) ? _secret : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given secret.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_secret">The opaque_ref of the given secret</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _secret, string _key)
        {
            session.proxy.secret_remove_from_other_config(session.uuid, (_secret != null) ? _secret : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Return a list of all the secrets known to the system.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Secret>> get_all(Session session)
        {
            return XenRef<Secret>.Create(session.proxy.secret_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the secret Records at once, in a single XML RPC call
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Secret>, Secret> get_all_records(Session session)
        {
            return XenRef<Secret>.Create<Proxy_Secret>(session.proxy.secret_get_all_records(session.uuid).parse());
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
        /// the secret
        /// </summary>
        public virtual string value
        {
            get { return _value; }
            set
            {
                if (!Helper.AreEqual(value, _value))
                {
                    _value = value;
                    Changed = true;
                    NotifyPropertyChanged("value");
                }
            }
        }
        private string _value;

        /// <summary>
        /// other_config
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
