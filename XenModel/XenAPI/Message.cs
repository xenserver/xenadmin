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
    /// An message for the attention of the administrator
    /// First published in XenServer 5.0.
    /// </summary>
    public partial class Message : XenObject<Message>
    {
        public enum MessageType { POOL_CPU_FEATURES_UP, POOL_CPU_FEATURES_DOWN, HOST_CPU_FEATURES_UP, HOST_CPU_FEATURES_DOWN, BOND_STATUS_CHANGED, VMSS_SNAPSHOT_MISSED_EVENT, VMSS_XAPI_LOGON_FAILURE, VMSS_LICENSE_ERROR, VMSS_SNAPSHOT_FAILED, VMSS_SNAPSHOT_SUCCEEDED, VMSS_SNAPSHOT_LOCK_FAILED, VMPP_SNAPSHOT_ARCHIVE_ALREADY_EXISTS, VMPP_ARCHIVE_MISSED_EVENT, VMPP_SNAPSHOT_MISSED_EVENT, VMPP_XAPI_LOGON_FAILURE, VMPP_LICENSE_ERROR, VMPP_ARCHIVE_TARGET_UNMOUNT_FAILED, VMPP_ARCHIVE_TARGET_MOUNT_FAILED, VMPP_ARCHIVE_SUCCEEDED, VMPP_ARCHIVE_FAILED_0, VMPP_ARCHIVE_LOCK_FAILED, VMPP_SNAPSHOT_FAILED, VMPP_SNAPSHOT_SUCCEEDED, VMPP_SNAPSHOT_LOCK_FAILED, LICENSE_SERVER_VERSION_OBSOLETE, LICENSE_SERVER_UNREACHABLE, LICENSE_NOT_AVAILABLE, GRACE_LICENSE, LICENSE_EXPIRED, LICENSE_SERVER_UNAVAILABLE, LICENSE_SERVER_CONNECTED, MULTIPATH_PERIODIC_ALERT, EXTAUTH_IN_POOL_IS_NON_HOMOGENEOUS, EXTAUTH_INIT_IN_HOST_FAILED, WLB_OPTIMIZATION_ALERT, WLB_CONSULTATION_FAILED, ALARM, PBD_PLUG_FAILED_ON_SERVER_START, POOL_MASTER_TRANSITION, HOST_CLOCK_WENT_BACKWARDS, HOST_CLOCK_SKEW_DETECTED, HOST_SYNC_DATA_FAILED, VM_CLONED, VM_CRASHED, VM_RESUMED, VM_SUSPENDED, VM_REBOOTED, VM_SHUTDOWN, VM_STARTED, VCPU_QOS_FAILED, VBD_QOS_FAILED, VIF_QOS_FAILED, IP_CONFIGURED_PIF_CAN_UNPLUG, METADATA_LUN_BROKEN, METADATA_LUN_HEALTHY, HA_HOST_WAS_FENCED, HA_HOST_FAILED, HA_PROTECTED_VM_RESTART_FAILED, HA_POOL_DROP_IN_PLAN_EXISTS_FOR, HA_POOL_OVERCOMMITTED, HA_NETWORK_BONDING_ERROR, HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT, HA_STATEFILE_APPROACHING_TIMEOUT, HA_HEARTBEAT_APPROACHING_TIMEOUT, HA_STATEFILE_LOST, LICENSE_EXPIRES_SOON, LICENSE_DOES_NOT_SUPPORT_POOLING, unknown };

        public MessageType Type
        {
            get
            {
                switch (this.name)
                {
                    case "POOL_CPU_FEATURES_UP":
                        return MessageType.POOL_CPU_FEATURES_UP;
                    case "POOL_CPU_FEATURES_DOWN":
                        return MessageType.POOL_CPU_FEATURES_DOWN;
                    case "HOST_CPU_FEATURES_UP":
                        return MessageType.HOST_CPU_FEATURES_UP;
                    case "HOST_CPU_FEATURES_DOWN":
                        return MessageType.HOST_CPU_FEATURES_DOWN;
                    case "BOND_STATUS_CHANGED":
                        return MessageType.BOND_STATUS_CHANGED;
                    case "VMSS_SNAPSHOT_MISSED_EVENT":
                        return MessageType.VMSS_SNAPSHOT_MISSED_EVENT;
                    case "VMSS_XAPI_LOGON_FAILURE":
                        return MessageType.VMSS_XAPI_LOGON_FAILURE;
                    case "VMSS_LICENSE_ERROR":
                        return MessageType.VMSS_LICENSE_ERROR;
                    case "VMSS_SNAPSHOT_FAILED":
                        return MessageType.VMSS_SNAPSHOT_FAILED;
                    case "VMSS_SNAPSHOT_SUCCEEDED":
                        return MessageType.VMSS_SNAPSHOT_SUCCEEDED;
                    case "VMSS_SNAPSHOT_LOCK_FAILED":
                        return MessageType.VMSS_SNAPSHOT_LOCK_FAILED;
                    case "VMPP_SNAPSHOT_ARCHIVE_ALREADY_EXISTS":
                        return MessageType.VMPP_SNAPSHOT_ARCHIVE_ALREADY_EXISTS;
                    case "VMPP_ARCHIVE_MISSED_EVENT":
                        return MessageType.VMPP_ARCHIVE_MISSED_EVENT;
                    case "VMPP_SNAPSHOT_MISSED_EVENT":
                        return MessageType.VMPP_SNAPSHOT_MISSED_EVENT;
                    case "VMPP_XAPI_LOGON_FAILURE":
                        return MessageType.VMPP_XAPI_LOGON_FAILURE;
                    case "VMPP_LICENSE_ERROR":
                        return MessageType.VMPP_LICENSE_ERROR;
                    case "VMPP_ARCHIVE_TARGET_UNMOUNT_FAILED":
                        return MessageType.VMPP_ARCHIVE_TARGET_UNMOUNT_FAILED;
                    case "VMPP_ARCHIVE_TARGET_MOUNT_FAILED":
                        return MessageType.VMPP_ARCHIVE_TARGET_MOUNT_FAILED;
                    case "VMPP_ARCHIVE_SUCCEEDED":
                        return MessageType.VMPP_ARCHIVE_SUCCEEDED;
                    case "VMPP_ARCHIVE_FAILED_0":
                        return MessageType.VMPP_ARCHIVE_FAILED_0;
                    case "VMPP_ARCHIVE_LOCK_FAILED":
                        return MessageType.VMPP_ARCHIVE_LOCK_FAILED;
                    case "VMPP_SNAPSHOT_FAILED":
                        return MessageType.VMPP_SNAPSHOT_FAILED;
                    case "VMPP_SNAPSHOT_SUCCEEDED":
                        return MessageType.VMPP_SNAPSHOT_SUCCEEDED;
                    case "VMPP_SNAPSHOT_LOCK_FAILED":
                        return MessageType.VMPP_SNAPSHOT_LOCK_FAILED;
                    case "LICENSE_SERVER_VERSION_OBSOLETE":
                        return MessageType.LICENSE_SERVER_VERSION_OBSOLETE;
                    case "LICENSE_SERVER_UNREACHABLE":
                        return MessageType.LICENSE_SERVER_UNREACHABLE;
                    case "LICENSE_NOT_AVAILABLE":
                        return MessageType.LICENSE_NOT_AVAILABLE;
                    case "GRACE_LICENSE":
                        return MessageType.GRACE_LICENSE;
                    case "LICENSE_EXPIRED":
                        return MessageType.LICENSE_EXPIRED;
                    case "LICENSE_SERVER_UNAVAILABLE":
                        return MessageType.LICENSE_SERVER_UNAVAILABLE;
                    case "LICENSE_SERVER_CONNECTED":
                        return MessageType.LICENSE_SERVER_CONNECTED;
                    case "MULTIPATH_PERIODIC_ALERT":
                        return MessageType.MULTIPATH_PERIODIC_ALERT;
                    case "EXTAUTH_IN_POOL_IS_NON_HOMOGENEOUS":
                        return MessageType.EXTAUTH_IN_POOL_IS_NON_HOMOGENEOUS;
                    case "EXTAUTH_INIT_IN_HOST_FAILED":
                        return MessageType.EXTAUTH_INIT_IN_HOST_FAILED;
                    case "WLB_OPTIMIZATION_ALERT":
                        return MessageType.WLB_OPTIMIZATION_ALERT;
                    case "WLB_CONSULTATION_FAILED":
                        return MessageType.WLB_CONSULTATION_FAILED;
                    case "ALARM":
                        return MessageType.ALARM;
                    case "PBD_PLUG_FAILED_ON_SERVER_START":
                        return MessageType.PBD_PLUG_FAILED_ON_SERVER_START;
                    case "POOL_MASTER_TRANSITION":
                        return MessageType.POOL_MASTER_TRANSITION;
                    case "HOST_CLOCK_WENT_BACKWARDS":
                        return MessageType.HOST_CLOCK_WENT_BACKWARDS;
                    case "HOST_CLOCK_SKEW_DETECTED":
                        return MessageType.HOST_CLOCK_SKEW_DETECTED;
                    case "HOST_SYNC_DATA_FAILED":
                        return MessageType.HOST_SYNC_DATA_FAILED;
                    case "VM_CLONED":
                        return MessageType.VM_CLONED;
                    case "VM_CRASHED":
                        return MessageType.VM_CRASHED;
                    case "VM_RESUMED":
                        return MessageType.VM_RESUMED;
                    case "VM_SUSPENDED":
                        return MessageType.VM_SUSPENDED;
                    case "VM_REBOOTED":
                        return MessageType.VM_REBOOTED;
                    case "VM_SHUTDOWN":
                        return MessageType.VM_SHUTDOWN;
                    case "VM_STARTED":
                        return MessageType.VM_STARTED;
                    case "VCPU_QOS_FAILED":
                        return MessageType.VCPU_QOS_FAILED;
                    case "VBD_QOS_FAILED":
                        return MessageType.VBD_QOS_FAILED;
                    case "VIF_QOS_FAILED":
                        return MessageType.VIF_QOS_FAILED;
                    case "IP_CONFIGURED_PIF_CAN_UNPLUG":
                        return MessageType.IP_CONFIGURED_PIF_CAN_UNPLUG;
                    case "METADATA_LUN_BROKEN":
                        return MessageType.METADATA_LUN_BROKEN;
                    case "METADATA_LUN_HEALTHY":
                        return MessageType.METADATA_LUN_HEALTHY;
                    case "HA_HOST_WAS_FENCED":
                        return MessageType.HA_HOST_WAS_FENCED;
                    case "HA_HOST_FAILED":
                        return MessageType.HA_HOST_FAILED;
                    case "HA_PROTECTED_VM_RESTART_FAILED":
                        return MessageType.HA_PROTECTED_VM_RESTART_FAILED;
                    case "HA_POOL_DROP_IN_PLAN_EXISTS_FOR":
                        return MessageType.HA_POOL_DROP_IN_PLAN_EXISTS_FOR;
                    case "HA_POOL_OVERCOMMITTED":
                        return MessageType.HA_POOL_OVERCOMMITTED;
                    case "HA_NETWORK_BONDING_ERROR":
                        return MessageType.HA_NETWORK_BONDING_ERROR;
                    case "HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT":
                        return MessageType.HA_XAPI_HEALTHCHECK_APPROACHING_TIMEOUT;
                    case "HA_STATEFILE_APPROACHING_TIMEOUT":
                        return MessageType.HA_STATEFILE_APPROACHING_TIMEOUT;
                    case "HA_HEARTBEAT_APPROACHING_TIMEOUT":
                        return MessageType.HA_HEARTBEAT_APPROACHING_TIMEOUT;
                    case "HA_STATEFILE_LOST":
                        return MessageType.HA_STATEFILE_LOST;
                    case "LICENSE_EXPIRES_SOON":
                        return MessageType.LICENSE_EXPIRES_SOON;
                    case "LICENSE_DOES_NOT_SUPPORT_POOLING":
                        return MessageType.LICENSE_DOES_NOT_SUPPORT_POOLING;
                    default:
                        return MessageType.unknown;
                }
            }
        }

        public Message()
        {
        }

        public Message(string uuid,
            string name,
            long priority,
            cls cls,
            string obj_uuid,
            DateTime timestamp,
            string body)
        {
            this.uuid = uuid;
            this.name = name;
            this.priority = priority;
            this.cls = cls;
            this.obj_uuid = obj_uuid;
            this.timestamp = timestamp;
            this.body = body;
        }

        /// <summary>
        /// Creates a new Message from a Proxy_Message.
        /// </summary>
        /// <param name="proxy"></param>
        public Message(Proxy_Message proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Message update)
        {
            uuid = update.uuid;
            name = update.name;
            priority = update.priority;
            cls = update.cls;
            obj_uuid = update.obj_uuid;
            timestamp = update.timestamp;
            body = update.body;
        }

        internal void UpdateFromProxy(Proxy_Message proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name = proxy.name == null ? null : (string)proxy.name;
            priority = proxy.priority == null ? 0 : long.Parse((string)proxy.priority);
            cls = proxy.cls == null ? (cls) 0 : (cls)Helper.EnumParseDefault(typeof(cls), (string)proxy.cls);
            obj_uuid = proxy.obj_uuid == null ? null : (string)proxy.obj_uuid;
            timestamp = proxy.timestamp;
            body = proxy.body == null ? null : (string)proxy.body;
        }

        public Proxy_Message ToProxy()
        {
            Proxy_Message result_ = new Proxy_Message();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name = (name != null) ? name : "";
            result_.priority = priority.ToString();
            result_.cls = cls_helper.ToString(cls);
            result_.obj_uuid = (obj_uuid != null) ? obj_uuid : "";
            result_.timestamp = timestamp;
            result_.body = (body != null) ? body : "";
            return result_;
        }

        /// <summary>
        /// Creates a new Message from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Message(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name = Marshalling.ParseString(table, "name");
            priority = Marshalling.ParseLong(table, "priority");
            cls = (cls)Helper.EnumParseDefault(typeof(cls), Marshalling.ParseString(table, "cls"));
            obj_uuid = Marshalling.ParseString(table, "obj_uuid");
            timestamp = Marshalling.ParseDateTime(table, "timestamp");
            body = Marshalling.ParseString(table, "body");
        }

        public bool DeepEquals(Message other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name, other._name) &&
                Helper.AreEqual2(this._priority, other._priority) &&
                Helper.AreEqual2(this._cls, other._cls) &&
                Helper.AreEqual2(this._obj_uuid, other._obj_uuid) &&
                Helper.AreEqual2(this._timestamp, other._timestamp) &&
                Helper.AreEqual2(this._body, other._body);
        }

        public override string SaveChanges(Session session, string opaqueRef, Message server)
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
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The name of the message</param>
        /// <param name="_priority">The priority of the message</param>
        /// <param name="_cls">The class of object this message is associated with</param>
        /// <param name="_obj_uuid">The uuid of the object this message is associated with</param>
        /// <param name="_body">The body of the message</param>
        public static XenRef<Message> create(Session session, string _name, long _priority, cls _cls, string _obj_uuid, string _body)
        {
            return XenRef<Message>.Create(session.proxy.message_create(session.uuid, (_name != null) ? _name : "", _priority.ToString(), cls_helper.ToString(_cls), (_obj_uuid != null) ? _obj_uuid : "", (_body != null) ? _body : "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_message">The opaque_ref of the given message</param>
        public static void destroy(Session session, string _message)
        {
            session.proxy.message_destroy(session.uuid, (_message != null) ? _message : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cls">The class of object</param>
        /// <param name="_obj_uuid">The uuid of the object</param>
        /// <param name="_since">The cutoff time</param>
        public static Dictionary<XenRef<Message>, Message> get(Session session, cls _cls, string _obj_uuid, DateTime _since)
        {
            return XenRef<Message>.Create<Proxy_Message>(session.proxy.message_get(session.uuid, cls_helper.ToString(_cls), (_obj_uuid != null) ? _obj_uuid : "", _since).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Message>> get_all(Session session)
        {
            return XenRef<Message>.Create(session.proxy.message_get_all(session.uuid).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_since">The cutoff time</param>
        public static Dictionary<XenRef<Message>, Message> get_since(Session session, DateTime _since)
        {
            return XenRef<Message>.Create<Proxy_Message>(session.proxy.message_get_since(session.uuid, _since).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_message">The opaque_ref of the given message</param>
        public static Message get_record(Session session, string _message)
        {
            return new Message((Proxy_Message)session.proxy.message_get_record(session.uuid, (_message != null) ? _message : "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the message</param>
        public static XenRef<Message> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Message>.Create(session.proxy.message_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Message>, Message> get_all_records(Session session)
        {
            return XenRef<Message>.Create<Proxy_Message>(session.proxy.message_get_all_records(session.uuid).parse());
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
        /// The name of the message
        /// </summary>
        public virtual string name
        {
            get { return _name; }
            set
            {
                if (!Helper.AreEqual(value, _name))
                {
                    _name = value;
                    Changed = true;
                    NotifyPropertyChanged("name");
                }
            }
        }
        private string _name;

        /// <summary>
        /// The message priority, 0 being low priority
        /// </summary>
        public virtual long priority
        {
            get { return _priority; }
            set
            {
                if (!Helper.AreEqual(value, _priority))
                {
                    _priority = value;
                    Changed = true;
                    NotifyPropertyChanged("priority");
                }
            }
        }
        private long _priority;

        /// <summary>
        /// The class of the object this message is associated with
        /// </summary>
        public virtual cls cls
        {
            get { return _cls; }
            set
            {
                if (!Helper.AreEqual(value, _cls))
                {
                    _cls = value;
                    Changed = true;
                    NotifyPropertyChanged("cls");
                }
            }
        }
        private cls _cls;

        /// <summary>
        /// The uuid of the object this message is associated with
        /// </summary>
        public virtual string obj_uuid
        {
            get { return _obj_uuid; }
            set
            {
                if (!Helper.AreEqual(value, _obj_uuid))
                {
                    _obj_uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("obj_uuid");
                }
            }
        }
        private string _obj_uuid;

        /// <summary>
        /// The time at which the message was created
        /// </summary>
        public virtual DateTime timestamp
        {
            get { return _timestamp; }
            set
            {
                if (!Helper.AreEqual(value, _timestamp))
                {
                    _timestamp = value;
                    Changed = true;
                    NotifyPropertyChanged("timestamp");
                }
            }
        }
        private DateTime _timestamp;

        /// <summary>
        /// The body of the message
        /// </summary>
        public virtual string body
        {
            get { return _body; }
            set
            {
                if (!Helper.AreEqual(value, _body))
                {
                    _body = value;
                    Changed = true;
                    NotifyPropertyChanged("body");
                }
            }
        }
        private string _body;
    }
}
