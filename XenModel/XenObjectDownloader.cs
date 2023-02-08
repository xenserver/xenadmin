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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Core
{
    [Serializable]
    public class EventFromBlockedException : Exception
    {
        public EventFromBlockedException() { }

        public EventFromBlockedException(string message) : base(message) { }

        public EventFromBlockedException(string message, Exception exception) : base(message, exception) { }

        protected EventFromBlockedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public static class XenObjectDownloader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const double EVENT_FROM_TIMEOUT = 30; // 30 seconds

        /// <summary>
        /// Gets all objects from the server. Used in order to fill the cache.
        /// This function implements the new event system, available from in API version 1.9.
        /// In the new event system, (GetAllRecords + GetEvents) sequence will replace (RegisterForEvents + DownloadObjects + GetNextEvents).
        /// </summary>
        /// <param name="session">The session over which to download the objects. Must not be null.</param>
        /// <param name="changes">The queue that the ObjectChanges will be put into. Must not be null.</param>
        /// <param name="cancelled">Used by GetEvents().</param>
        /// <param name="token">Used by GetEvents().</param>
        public static void GetAllObjects(Session session, LockFreeQueue<ObjectChange> changes, HTTP.FuncBool cancelled, ref string token)
        {
            // download objects that are not covered by event.from(), e.g. Roles

            var roleRecords = Role.get_all_records(session);
            foreach (KeyValuePair<XenRef<Role>, Role> entry in roleRecords)
                changes.Enqueue(new ObjectChange(typeof(Role), entry.Key.opaque_ref, entry.Value));

            // get all objects with event.from()
            token = ""; 
            GetEvents(session, changes, cancelled, ref token);
        }

        /// <summary>
        /// Blocks until events are sent on the session, or timeout is reached, then processes any received events and adds them
        /// to eventQueue. This function implements the new event system, available in API version 1.9. 
        /// In the new event system, (GetAllRecords + GetEvents) sequence will replace (RegisterForEvents + DownloadObjects + GetNextEvents).
        /// </summary>
        /// <param name="session"></param>
        /// <param name="eventQueue"></param>
        /// <param name="cancelled"></param>
        /// <param name="token">A token used by event.from(). 
        /// It should be the empty string when event.from is first called, which is the replacement of get_all_records.
        /// </param>
        public static void GetEvents(Session session, LockFreeQueue<ObjectChange> eventQueue, HTTP.FuncBool cancelled, ref string token)
        {
            Proxy_Event[] proxyEvents = {};
            Event[] events = {};
            try
            {
                var classes = new [] { "*" }; // classes that we are interested in receiving events from
                var eventResult = Event.from(session, classes, token, EVENT_FROM_TIMEOUT);

                if (session.JsonRpcClient != null)
                {
                    var batch = (EventBatch)eventResult;
                    events = batch.events;
                    token = batch.token;
                }
                else
                {
                    var evts = (Events)eventResult;
                    proxyEvents = evts.events;
                    token = evts.token;
                }
            }
            catch (WebException e)
            {
                // Catch timeout, and turn it into an EventFromBlockedException so we can recognise it later (CA-33145)
                if (e.Status == WebExceptionStatus.Timeout)
                    throw new EventFromBlockedException();
                else
                    throw;
            }

            if (cancelled())
                return;

            //We want to do the marshalling on this background thread so as not to block the gui thread
            if (session.JsonRpcClient != null)
            {
                foreach (Event evt in events)
                {
                    var objectChange = ProcessEvent(evt.class_, evt.operation, evt.opaqueRef, evt.snapshot, false);

                    if (objectChange != null)
                        eventQueue.Enqueue(objectChange);
                }
            }
            else
            {
                foreach (Proxy_Event proxyEvent in proxyEvents)
                {
                    var objectChange = ProcessEvent(proxyEvent.class_, proxyEvent.operation, proxyEvent.opaqueRef, proxyEvent.snapshot, true);

                    if (objectChange != null)
                        eventQueue.Enqueue(objectChange);
                }
            }
        }
        
        /// <summary>
        /// Returns null if we get an event we're not interested in, or an unparseable event (e.g. for an object type we don't know about).
        /// </summary>
        private static ObjectChange ProcessEvent(string class_, string operation, string  opaqueRef, object snapshot, bool marshall)
        {
            switch (class_.ToLowerInvariant())
            {
                case "session":
                case "event":
                case "user":
                case "secret":
                    // We don't track events on these objects
                    return null;

                default:
                    Type typ = Marshalling.GetXenAPIType(class_);

                    if (typ == null)
                    {
                        log.DebugFormat("Unknown {0} event for class {1}.", operation, class_);
                        return null;
                    }

                    switch (operation)
                    {
                        case "add":
                        case "mod":
                            var marshalled = marshall ? Marshalling.convertStruct(typ, (Hashtable)snapshot) : snapshot;
                            return new ObjectChange(typ, opaqueRef, marshalled);
                        case "del":
                            return new ObjectChange(typ, opaqueRef, null);

                        default:
                            log.DebugFormat("Unknown event operation {0} for opaque ref {1}", operation, opaqueRef);
                            return null;
                    }
            }
        }
    }
}
