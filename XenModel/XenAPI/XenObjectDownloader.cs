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
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using XenAdmin.Core;

namespace XenAPI
{
    [Serializable]
    public class EventNextBlockedException : Exception
    {
        public EventNextBlockedException() : base() { }

        public EventNextBlockedException(string message) : base(message) { }

        public EventNextBlockedException(string message, Exception exception) : base(message, exception) { }

        protected EventNextBlockedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public static class XenObjectDownloader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const double EVENT_FROM_TIMEOUT = 30; // 30 seconds

        /// <summary>
        /// Whether to use the legacy event system (event.next); the new system is event.from.
        /// </summary>
        /// <param name="session"></param>
        public static bool LegacyEventSystem(Session session)
        {
            return session.APIVersion <= API_Version.API_1_9;
        }

        /// <sumary>
        /// Gets all objects from the server. Used in order to fill the cache.
        /// This function implements the new event system, available from in API version 1.9.
        /// In the new event system, (GetAllRecords + GetEvents) sequence will replace (RegisterForEvents + DownloadObjects + GetNextEvents).
        /// </summary>
        /// <param name="session">The session over which to download the objects. Must not be null.</param>
        /// <param name="changes">The queue that the ObjectChanges will be put into. Must not be null.</param>
        /// <param name="cancelled">Used by GetEvents().</param>
        /// <param name="legacyEventSystem">True if legacy event system (event.next) should to be used.</param>
        /// <param name="token">Used by GetEvents().</param>
        public static void GetAllObjects(Session session, LockFreeQueue<ObjectChange> changes, HTTP.FuncBool cancelled, bool legacyEventSystem, ref string token)
        {
            if (legacyEventSystem)
            {
                DownloadObjects(session, changes);
                return;
            }
            
            // download objects that are not covered by event.from(), e.g. Roles
            List<ObjectChange> list = new List<ObjectChange>();
            Download_Role(session, list);
            foreach (ObjectChange o in list)
                changes.Enqueue(o);

            // get all objects with event.from()
            token = ""; 
            GetEvents(session, changes, cancelled, false, ref token);
        }

        /// <summary>
        /// Blocks until events are sent on the session, or timeout is reached, then processes any received events and adds them
        /// to eventQueue. This function implements the new event system, available in API version 1.9. 
        /// In the new event system, (GetAllRecords + GetEvents) sequence will replace (RegisterForEvents + DownloadObjects + GetNextEvents).
        /// </summary>
        /// <param name="session"></param>
        /// <param name="eventQueue"></param>
        /// <param name="cancelled"></param>
        /// <param name="legacyEventSystem">True if legacy event system (event.next) should be used.</param>
        /// <param name="token">A token used by event.from(). 
        /// It should be the empty string when event.from is first called, which is the replacement of get_all_records.
        /// </param>
        public static void GetEvents(Session session, LockFreeQueue<ObjectChange> eventQueue, HTTP.FuncBool cancelled, bool legacyEventSystem, ref string token)
        {
            if (legacyEventSystem)
            {
                GetNextEvents(session, eventQueue, cancelled);
                return;
            }

            Proxy_Event[] proxyEvents;
            try
            {
                var classes = new [] { "*" }; // classes that we are interested in receiving events from
                var eventResult = Event.from(session, classes, token, EVENT_FROM_TIMEOUT);
                token = eventResult.token;
                proxyEvents = eventResult.events;
            }
            catch (WebException e)
            {
                // Catch timeout, and turn it into an EventNextBlockedException so we can recognise it later (CA-33145)
                if (e.Status == WebExceptionStatus.Timeout)
                    throw new EventNextBlockedException();
                else
                    throw;
            }

            if (cancelled())
                return;

            //We want to do the marshalling on this bg thread so as not to block the gui thread
            foreach (Proxy_Event proxyEvent in proxyEvents)
            {
                ObjectChange objectChange = ProcessEvent(proxyEvent);

                if (objectChange != null)
                    eventQueue.Enqueue(objectChange);
            }
        }
        
        public static void RegisterForEvents(Session session)
        {
            Event.register(session, new string[] { "*" });
        }
       
        /// <summary>
        /// Blocks until events are sent on the session, then processes any received events and adds them
        /// to eventQueue. Will always add at least one event to eventQueue.
        /// This function should be used with XenServer up to version 6.0. For XenServer higher than 6.0, GetEvents() should be used instead.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="eventQueue"></param>
        /// <param name="cancelled"></param>
        public static void GetNextEvents(Session session, LockFreeQueue<ObjectChange> eventQueue, HTTP.FuncBool cancelled)
        {
            Proxy_Event[] proxyEvents;

            try
            {
                proxyEvents = Event.next(session);
            }
            catch (WebException e)
            {
                // Catch timeout, and turn it into an EventNextBlockedException so we can recognise it later (CA-33145)
                if (e.Status == WebExceptionStatus.Timeout)
                    throw new EventNextBlockedException();
                else
                    throw;
            }

            if (proxyEvents.Length == 0)
                throw new IOException("Event.next() returned no events; the server is misbehaving.");

            if (cancelled())
                return;

            //We want to do the marshalling on this bg thread so as not to block the gui thread
            foreach (Proxy_Event proxyEvent in proxyEvents)
            {
                ObjectChange objectChange = ProcessEvent(proxyEvent);

                if (objectChange != null)
                    eventQueue.Enqueue(objectChange);
            }
        }

        /// <summary>
        /// Returns null if we get an event we're not interested in, or an unparseable event (e.g. for an object type we don't know about).
        /// </summary>
        /// <param name="proxyEvent"></param>
        /// <returns></returns>
        private static ObjectChange ProcessEvent(Proxy_Event proxyEvent)
        {
            switch (proxyEvent.class_.ToLowerInvariant())
            {
                case "session":
                case "event":
                case "vtpm":
                case "user":
                case "secret":
                    // We don't track events on these objects
                    return null;

                default:
                    Type typ = Marshalling.GetXenAPIType(proxyEvent.class_);

                    if (typ == null)
                    {
                        log.DebugFormat("Unknown {0} event for class {1}.", proxyEvent.operation, proxyEvent.class_);
                        return null;
                    }

                    switch (proxyEvent.operation)
                    {
                        case "add":
                        case "mod":
                            return new ObjectChange(typ, proxyEvent.opaqueRef, Marshalling.convertStruct(typ, (Hashtable)proxyEvent.snapshot));
                        case "del":
                            return new ObjectChange(typ, proxyEvent.opaqueRef, null);

                        default:
                            log.DebugFormat("Unknown event operation {0} for opaque ref {1}", proxyEvent.operation, proxyEvent.opaqueRef);
                            return null;
                    }
            }
        }

        /// <summary>
        /// Downloads all objects from the server. Used in order to fill the cache.
        /// This function should be used with XenServer up to version 6.0. For XenServer higher than 6.0, GetAllObjects() should be used instead.
        /// </summary>
        /// <param name="session">The session over which to download the objects. Must not be null.</param>
        /// <param name="changes">The queue that the ObjectChanges will be put into. Must not be null.</param>
        public static void DownloadObjects(Session session, LockFreeQueue<ObjectChange> changes)
        {
            List<ObjectChange> list = new List<ObjectChange>();

            Download_Task(session, list);
            Download_Pool(session, list);
            Download_VM(session, list);
            Download_VM_metrics(session, list);
            Download_VM_guest_metrics(session, list);
            Download_Host(session, list);
            Download_Host_crashdump(session, list);
            Download_Host_patch(session, list);
            Download_Host_metrics(session, list);
            Download_Host_cpu(session, list);
            Download_Network(session, list);
            Download_VIF(session, list);
            Download_VIF_metrics(session, list);
            Download_PIF(session, list);
            Download_PIF_metrics(session, list);
            Download_SM(session, list);
            Download_SR(session, list);
            Download_VDI(session, list);
            Download_VBD(session, list);
            Download_VBD_metrics(session, list);
            Download_PBD(session, list);
            Download_Crashdump(session, list);
            Download_Console(session, list);

            if (session.APIVersion >= API_Version.API_1_2)
            {
                // Download Miami-only objects
                Download_Pool_patch(session, list);
                Download_Bond(session, list);
                Download_VLAN(session, list);
            }

            if (session.APIVersion >= API_Version.API_1_3)
            {
                // Download Orlando-only objects
                Download_Blob(session, list);
                Download_Message(session, list);
            }

            if (session.APIVersion >= API_Version.API_1_6)
            {
                // Download George-only objects
                Download_Subject(session, list);
            }

            if (session.APIVersion >= API_Version.API_1_7)
            {
                // Download Midnight Ride-only objects
                Download_Role(session, list);
            }

            if (session.APIVersion >= API_Version.API_1_8)
            {
                // Download Cowley-only objects
                Download_VMPP(session, list);
                Download_Tunnel(session, list);
            }

            if (session.APIVersion >= API_Version.API_1_9)
            {
                // Download Boston-only objects
                Download_VM_appliance(session, list);
                Download_DR_task(session, list);
                Download_PCI(session, list);
                Download_PGPU(session, list);
                Download_GPU_group(session, list);
                Download_VGPU(session, list);
            }

            foreach (ObjectChange o in list)
            {
                changes.Enqueue(o);
            }
        }

        private static void Download_Subject(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Subject>, Subject> records = Subject.get_all_records(session);
            foreach (KeyValuePair<XenRef<Subject>, Subject> entry in records)
                changes.Add(new ObjectChange(typeof(Subject), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Role(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Role>, Role> records = Role.get_all_records(session);
            foreach (KeyValuePair<XenRef<Role>, Role> entry in records)
                changes.Add(new ObjectChange(typeof(Role), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Task(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Task>, Task> records = Task.get_all_records(session);
            foreach (KeyValuePair<XenRef<Task>, Task> entry in records)
                changes.Add(new ObjectChange(typeof(Task), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Pool(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Pool>, Pool> records = Pool.get_all_records(session);
            foreach (KeyValuePair<XenRef<Pool>, Pool> entry in records)
                changes.Add(new ObjectChange(typeof(Pool), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Pool_patch(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Pool_patch>, Pool_patch> records = Pool_patch.get_all_records(session);
            foreach (KeyValuePair<XenRef<Pool_patch>, Pool_patch> entry in records)
                changes.Add(new ObjectChange(typeof(Pool_patch), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VM(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VM>, VM> records = VM.get_all_records(session);
            foreach (KeyValuePair<XenRef<VM>, VM> entry in records)
                changes.Add(new ObjectChange(typeof(VM), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VM_metrics(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VM_metrics>, VM_metrics> records = VM_metrics.get_all_records(session);
            foreach (KeyValuePair<XenRef<VM_metrics>, VM_metrics> entry in records)
                changes.Add(new ObjectChange(typeof(VM_metrics), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VM_guest_metrics(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VM_guest_metrics>, VM_guest_metrics> records = VM_guest_metrics.get_all_records(session);
            foreach (KeyValuePair<XenRef<VM_guest_metrics>, VM_guest_metrics> entry in records)
                changes.Add(new ObjectChange(typeof(VM_guest_metrics), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VMPP(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VMPP>, VMPP> records = VMPP.get_all_records(session);
            foreach (KeyValuePair<XenRef<VMPP>, VMPP> entry in records)
                changes.Add(new ObjectChange(typeof(VMPP), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VM_appliance(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VM_appliance>, VM_appliance> records = VM_appliance.get_all_records(session);
            foreach (KeyValuePair<XenRef<VM_appliance>, VM_appliance> entry in records)
                changes.Add(new ObjectChange(typeof(VM_appliance), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_DR_task(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<DR_task>, DR_task> records = DR_task.get_all_records(session);
            foreach (KeyValuePair<XenRef<DR_task>, DR_task> entry in records)
                changes.Add(new ObjectChange(typeof(DR_task), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Host(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Host>, Host> records = Host.get_all_records(session);
            foreach (KeyValuePair<XenRef<Host>, Host> entry in records)
                changes.Add(new ObjectChange(typeof(Host), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Host_crashdump(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Host_crashdump>, Host_crashdump> records = Host_crashdump.get_all_records(session);
            foreach (KeyValuePair<XenRef<Host_crashdump>, Host_crashdump> entry in records)
                changes.Add(new ObjectChange(typeof(Host_crashdump), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Host_patch(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Host_patch>, Host_patch> records = Host_patch.get_all_records(session);
            foreach (KeyValuePair<XenRef<Host_patch>, Host_patch> entry in records)
                changes.Add(new ObjectChange(typeof(Host_patch), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Host_metrics(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Host_metrics>, Host_metrics> records = Host_metrics.get_all_records(session);
            foreach (KeyValuePair<XenRef<Host_metrics>, Host_metrics> entry in records)
                changes.Add(new ObjectChange(typeof(Host_metrics), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Host_cpu(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Host_cpu>, Host_cpu> records = Host_cpu.get_all_records(session);
            foreach (KeyValuePair<XenRef<Host_cpu>, Host_cpu> entry in records)
                changes.Add(new ObjectChange(typeof(Host_cpu), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Network(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Network>, Network> records = Network.get_all_records(session);
            foreach (KeyValuePair<XenRef<Network>, Network> entry in records)
                changes.Add(new ObjectChange(typeof(Network), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VIF(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VIF>, VIF> records = VIF.get_all_records(session);
            foreach (KeyValuePair<XenRef<VIF>, VIF> entry in records)
                changes.Add(new ObjectChange(typeof(VIF), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VIF_metrics(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VIF_metrics>, VIF_metrics> records = VIF_metrics.get_all_records(session);
            foreach (KeyValuePair<XenRef<VIF_metrics>, VIF_metrics> entry in records)
                changes.Add(new ObjectChange(typeof(VIF_metrics), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_PIF(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<PIF>, PIF> records = PIF.get_all_records(session);
            foreach (KeyValuePair<XenRef<PIF>, PIF> entry in records)
                changes.Add(new ObjectChange(typeof(PIF), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_PIF_metrics(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<PIF_metrics>, PIF_metrics> records = PIF_metrics.get_all_records(session);
            foreach (KeyValuePair<XenRef<PIF_metrics>, PIF_metrics> entry in records)
                changes.Add(new ObjectChange(typeof(PIF_metrics), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Bond(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Bond>, Bond> records = Bond.get_all_records(session);
            foreach (KeyValuePair<XenRef<Bond>, Bond> entry in records)
                changes.Add(new ObjectChange(typeof(Bond), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VLAN(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VLAN>, VLAN> records = VLAN.get_all_records(session);
            foreach (KeyValuePair<XenRef<VLAN>, VLAN> entry in records)
                changes.Add(new ObjectChange(typeof(VLAN), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_SM(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<SM>, SM> records = SM.get_all_records(session);
            foreach (KeyValuePair<XenRef<SM>, SM> entry in records)
                changes.Add(new ObjectChange(typeof(SM), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_SR(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<SR>, SR> records = SR.get_all_records(session);
            foreach (KeyValuePair<XenRef<SR>, SR> entry in records)
                changes.Add(new ObjectChange(typeof(SR), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VDI(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VDI>, VDI> records = VDI.get_all_records(session);
            foreach (KeyValuePair<XenRef<VDI>, VDI> entry in records)
                changes.Add(new ObjectChange(typeof(VDI), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VBD(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VBD>, VBD> records = VBD.get_all_records(session);
            foreach (KeyValuePair<XenRef<VBD>, VBD> entry in records)
                changes.Add(new ObjectChange(typeof(VBD), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VBD_metrics(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VBD_metrics>, VBD_metrics> records = VBD_metrics.get_all_records(session);
            foreach (KeyValuePair<XenRef<VBD_metrics>, VBD_metrics> entry in records)
                changes.Add(new ObjectChange(typeof(VBD_metrics), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_PBD(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<PBD>, PBD> records = PBD.get_all_records(session);
            foreach (KeyValuePair<XenRef<PBD>, PBD> entry in records)
                changes.Add(new ObjectChange(typeof(PBD), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Crashdump(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Crashdump>, Crashdump> records = Crashdump.get_all_records(session);
            foreach (KeyValuePair<XenRef<Crashdump>, Crashdump> entry in records)
                changes.Add(new ObjectChange(typeof(Crashdump), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Console(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Console>, Console> records = Console.get_all_records(session);
            foreach (KeyValuePair<XenRef<Console>, Console> entry in records)
                changes.Add(new ObjectChange(typeof(Console), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Blob(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Blob>, Blob> records = Blob.get_all_records(session);
            foreach (KeyValuePair<XenRef<Blob>, Blob> entry in records)
                changes.Add(new ObjectChange(typeof(Blob), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Message(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Message>, Message> records = Message.get_all_records(session);
            foreach (KeyValuePair<XenRef<Message>, Message> entry in records)
                changes.Add(new ObjectChange(typeof(Message), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_Tunnel(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<Tunnel>, Tunnel> records = Tunnel.get_all_records(session);
            foreach (KeyValuePair<XenRef<Tunnel>, Tunnel> entry in records)
                changes.Add(new ObjectChange(typeof(Tunnel), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_PCI(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<PCI>, PCI> records = PCI.get_all_records(session);
            foreach (KeyValuePair<XenRef<PCI>, PCI> entry in records)
                changes.Add(new ObjectChange(typeof(PCI), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_PGPU(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<PGPU>, PGPU> records = PGPU.get_all_records(session);
            foreach (KeyValuePair<XenRef<PGPU>, PGPU> entry in records)
                changes.Add(new ObjectChange(typeof(PGPU), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_GPU_group(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<GPU_group>, GPU_group> records = GPU_group.get_all_records(session);
            foreach (KeyValuePair<XenRef<GPU_group>, GPU_group> entry in records)
                changes.Add(new ObjectChange(typeof(GPU_group), entry.Key.opaque_ref, entry.Value));
        }

        private static void Download_VGPU(Session session, List<ObjectChange> changes)
        {
            Dictionary<XenRef<VGPU>, VGPU> records = VGPU.get_all_records(session);
            foreach (KeyValuePair<XenRef<VGPU>, VGPU> entry in records)
                changes.Add(new ObjectChange(typeof(VGPU), entry.Key.opaque_ref, entry.Value));
        }
    }
}
