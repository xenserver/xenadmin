/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Core;

namespace XenAPI
{
    public partial class Message
    {
        private string _messageTypeString;

        public string MessageTypeString()
        {
            if (string.IsNullOrEmpty(_messageTypeString))
                _messageTypeString = Type.ToString();
            return _messageTypeString;
        }

        public DateTime TimestampLocal()
        {
            DateTime t = timestamp + Connection.ServerTimeOffset;
            return t.ToLocalTime();
        }

        public static string FriendlyName(string type)
        {
            return FriendlyNameManager.GetFriendlyName(string.Format("Message.name-{0}", type.ToLowerInvariant()));
        }

        public static string FriendlyBody(string type)
        {
            return FriendlyNameManager.GetFriendlyName(string.Format("Message.body-{0}", type.ToLowerInvariant()));
        }

        public static string FriendlyHelp(string type)
        {
            return FriendlyNameManager.GetFriendlyName(string.Format("Message.help-{0}", type.ToLowerInvariant()));
        }

        public static string FriendlyAction(string type)
        {
            return FriendlyNameManager.GetFriendlyName(string.Format("Message.action-{0}", type.ToLowerInvariant()));
        }

        public bool ShowOnGraphs()
        {
            var typ = this.Type;
            return typ == MessageType.VM_CLONED
                   || typ == MessageType.VM_CRASHED
                   || typ == MessageType.VM_REBOOTED
                   || typ == MessageType.VM_RESUMED
                   || typ == MessageType.VM_SHUTDOWN
                   || typ == MessageType.VM_STARTED
                   || typ == MessageType.VM_SUSPENDED;
        }

        public bool IsSquelched()
        {
            // We don't show HA_POOL_OVERCOMMITTED because we get HA_POOL_DROP_IN_PLAN_EXISTS_FOR at the same time.
            return Type == MessageType.HA_POOL_OVERCOMMITTED;
        }

        /// <summary>
        /// Retrieves the IXenObject the message was issued for.
        /// May return null if the object is of unknown type or not found.
        /// </summary>
        public IXenObject GetXenObject()
        {
            switch (cls)
            {
                case cls.Pool:
                    return Connection.Cache.Find_By_Uuid<Pool>(obj_uuid);
                case cls.Host:
                    return Connection.Cache.Find_By_Uuid<Host>(obj_uuid);
                case cls.VM:
                    return Connection.Cache.Find_By_Uuid<VM>(obj_uuid);
                case cls.SR:
                    return Connection.Cache.Find_By_Uuid<SR>(obj_uuid);
                case cls.VMSS:
                    return Connection.Cache.Find_By_Uuid<VMSS>(obj_uuid);
                case cls.PVS_proxy:
                    return Connection.Cache.Find_By_Uuid<PVS_proxy>(obj_uuid);
                case cls.Certificate:
                    return Connection.Cache.Find_By_Uuid<Certificate>(obj_uuid);
                case cls.VDI:
                    return Connection.Cache.Find_By_Uuid<VDI>(obj_uuid);
                default:
                    return null;
            }
        }
    }
}
