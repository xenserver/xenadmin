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
using System.Collections.Generic;


namespace XenAPI
{
    public partial class Event : XenObject<Event>
    {
        public Event()
        {
        }

        internal Event(Proxy_Event proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        internal void UpdateFromProxy(Proxy_Event proxy)
        {
            id = long.Parse((string)proxy.id);
        }

        public override void UpdateFrom(Event update)
        {
            id = update.id;
        }

        internal Proxy_Event ToProxy()
        {
            Proxy_Event result = new Proxy_Event();
            result.id = id.ToString();
            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Event serverObject)
        {
            Event server = (Event)serverObject;
            if (opaqueRef == null)
            {
                Proxy_Event p = this.ToProxy();
                throw new InvalidOperationException("There is no constructor available for this type; you cannot directly create one on the server.");
            }
            else
            {
                if (!_id.Equals(server._id))
                {
                    Event.set_id(session, opaqueRef, _id);
                }

                return null;
            }
        }

        public static Event get_record(Session session, string _event)
        {
            return new Event((Proxy_Event)session.proxy.event_get_record(session.uuid, (_event != null) ? _event : "").parse());
        }

        public static string get_by_uuid(Session session, string _uuid)
        {
            return (string)session.proxy.event_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse();
        }

        public static long get_id(Session session, string _event)
        {
            return long.Parse((string)session.proxy.event_get_id(session.uuid, (_event != null) ? _event : "").parse());
        }

        public static void set_id(Session session, string _event, long _id)
        {
            session.proxy.event_set_id(session.uuid, (_event != null) ? _event : "", _id.ToString()).parse();
        }

        public static void register(Session session, string[] _classes)
        {
            session.proxy.event_register(session.uuid, _classes).parse();
        }

        public static void unregister(Session session, string[] _classes)
        {
            session.proxy.event_unregister(session.uuid, _classes).parse();
        }

        public static Proxy_Event[] next(Session session)
        {
            return session.proxy.event_next(session.uuid).parse();
        }

        public static Events from(Session session, string[] _classes, string _token, double _timeout)
        {
            return session.proxy.event_from(session.uuid, _classes, _token, _timeout).parse();
        }

        private long _id;
        public long id
        {
             get { return _id; }
             set { if (value != _id) { _id = value; Changed = true; NotifyPropertyChanged("id"); } }
        }
    }
}
