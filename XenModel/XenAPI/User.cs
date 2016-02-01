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
    /// A user of the system
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class User : XenObject<User>
    {
        public User()
        {
        }

        public User(string uuid,
            string short_name,
            string fullname,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.short_name = short_name;
            this.fullname = fullname;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new User from a Proxy_User.
        /// </summary>
        /// <param name="proxy"></param>
        public User(Proxy_User proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(User update)
        {
            uuid = update.uuid;
            short_name = update.short_name;
            fullname = update.fullname;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_User proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            short_name = proxy.short_name == null ? null : (string)proxy.short_name;
            fullname = proxy.fullname == null ? null : (string)proxy.fullname;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_User ToProxy()
        {
            Proxy_User result_ = new Proxy_User();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.short_name = (short_name != null) ? short_name : "";
            result_.fullname = (fullname != null) ? fullname : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new User from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public User(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            short_name = Marshalling.ParseString(table, "short_name");
            fullname = Marshalling.ParseString(table, "fullname");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(User other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._short_name, other._short_name) &&
                Helper.AreEqual2(this._fullname, other._fullname) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, User server)
        {
            if (opaqueRef == null)
            {
                Proxy_User p = this.ToProxy();
                return session.proxy.user_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_fullname, server._fullname))
                {
                    User.set_fullname(session, opaqueRef, _fullname);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    User.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given user.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        [Deprecated("XenServer 5.5")]
        public static User get_record(Session session, string _user)
        {
            return new User((Proxy_User)session.proxy.user_get_record(session.uuid, (_user != null) ? _user : "").parse());
        }

        /// <summary>
        /// Get a reference to the user instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("XenServer 5.5")]
        public static XenRef<User> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<User>.Create(session.proxy.user_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Create a new user instance, and return its handle.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        [Deprecated("XenServer 5.5")]
        public static XenRef<User> create(Session session, User _record)
        {
            return XenRef<User>.Create(session.proxy.user_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new user instance, and return its handle.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        [Deprecated("XenServer 5.5")]
        public static XenRef<Task> async_create(Session session, User _record)
        {
            return XenRef<Task>.Create(session.proxy.async_user_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified user instance.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        [Deprecated("XenServer 5.5")]
        public static void destroy(Session session, string _user)
        {
            session.proxy.user_destroy(session.uuid, (_user != null) ? _user : "").parse();
        }

        /// <summary>
        /// Destroy the specified user instance.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        [Deprecated("XenServer 5.5")]
        public static XenRef<Task> async_destroy(Session session, string _user)
        {
            return XenRef<Task>.Create(session.proxy.async_user_destroy(session.uuid, (_user != null) ? _user : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        public static string get_uuid(Session session, string _user)
        {
            return (string)session.proxy.user_get_uuid(session.uuid, (_user != null) ? _user : "").parse();
        }

        /// <summary>
        /// Get the short_name field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        public static string get_short_name(Session session, string _user)
        {
            return (string)session.proxy.user_get_short_name(session.uuid, (_user != null) ? _user : "").parse();
        }

        /// <summary>
        /// Get the fullname field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        public static string get_fullname(Session session, string _user)
        {
            return (string)session.proxy.user_get_fullname(session.uuid, (_user != null) ? _user : "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given user.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        public static Dictionary<string, string> get_other_config(Session session, string _user)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.user_get_other_config(session.uuid, (_user != null) ? _user : "").parse());
        }

        /// <summary>
        /// Set the fullname field of the given user.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_fullname">New value to set</param>
        public static void set_fullname(Session session, string _user, string _fullname)
        {
            session.proxy.user_set_fullname(session.uuid, (_user != null) ? _user : "", (_fullname != null) ? _fullname : "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given user.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _user, Dictionary<string, string> _other_config)
        {
            session.proxy.user_set_other_config(session.uuid, (_user != null) ? _user : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given user.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _user, string _key, string _value)
        {
            session.proxy.user_add_to_other_config(session.uuid, (_user != null) ? _user : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given user.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_user">The opaque_ref of the given user</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _user, string _key)
        {
            session.proxy.user_remove_from_other_config(session.uuid, (_user != null) ? _user : "", (_key != null) ? _key : "").parse();
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
        /// short name (e.g. userid)
        /// </summary>
        public virtual string short_name
        {
            get { return _short_name; }
            set
            {
                if (!Helper.AreEqual(value, _short_name))
                {
                    _short_name = value;
                    Changed = true;
                    NotifyPropertyChanged("short_name");
                }
            }
        }
        private string _short_name;

        /// <summary>
        /// full name
        /// </summary>
        public virtual string fullname
        {
            get { return _fullname; }
            set
            {
                if (!Helper.AreEqual(value, _fullname))
                {
                    _fullname = value;
                    Changed = true;
                    NotifyPropertyChanged("fullname");
                }
            }
        }
        private string _fullname;

        /// <summary>
        /// additional configuration
        /// First published in XenServer 5.0.
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
