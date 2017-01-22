/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
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
using System.Reflection;
using System.Xml;

using XenAPI;
using XenAdmin.Network;
using XenAdmin.ServerDBs.FakeAPI;
using XenAdmin.Core;

namespace XenAdmin.ServerDBs
{
    public class DbProxy : IInvocationHandler
    {
        private static readonly ClassGenerator generator = new ClassGenerator(typeof(Proxy));
        private static readonly Type proxyType = generator.Generate();
        public static readonly Proxy_Event dummyEvent = new Proxy_Event();
        public static readonly Dictionary<IXenConnection, DbProxy> proxys = new Dictionary<IXenConnection, DbProxy>();
        private static readonly object dicLock = new object();

        public readonly Db db;
        public readonly List<Proxy_Event> eventsList = new List<Proxy_Event>();
        public readonly object eventsListLock = new object();
        private readonly string url;
        public readonly IXenConnection connection;
        public volatile bool MarkToDisconnect;

        /// Set this delegate to run inside the event loop.
        /// Throwing an specific exception type or Failure will simulate the termination
        /// of the event loop mid-loop
        public Action RunInEventLoop { get; private set; }

        private static readonly Dictionary<string, List<string>> callVersions = new Dictionary<string, List<string>>();
        public bool IsSuperUser { get; private set; }

        public void SetIsSuperUser(bool isSuperUser)
        {
            IsSuperUser = isSuperUser;
        }

        public static void RemoveAll()
        {
            lock (dicLock)
            {
                foreach (KeyValuePair<IXenConnection, DbProxy> kvp in proxys)
                {
                    kvp.Value.db.Dispose();
                    kvp.Value.RunInEventLoop = delegate()
                    {
                        throw new Failure(Failure.SESSION_INVALID);
                    };
                }
                proxys.Clear();
            }
        }

        /// <summary>
        /// Initializes the <see cref="DbProxy"/> class.
        /// </summary>
        static DbProxy()
        {
            dummyEvent.class_ = "vtpm"; // This will cause XenCenter to ignore the event;

            // load the api-version.xml file to detect whether a call is made for the wrong version of the server being simulated.
            
            /*XmlDocument doc = new XmlDocument();
            doc.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("XenAdmin.TestResources.api-version.xml"));

            foreach (XmlNode node in doc.SelectNodes("ApiVersionTool/version"))
            {
                string ver = node.Attributes["name"].Value;
                callVersions.Add(ver, new List<string>());

                foreach (XmlNode n in node.SelectNodes("call"))
                {
                    callVersions[ver].Add(n.Attributes["name"].Value);
                }
            }
            */
        }

        /// <summary>
        /// Generates a Proxy instance which implements <see cref="Proxy"/> for the specified <see cref="IXenConnection"/> and XML document.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static Proxy GetProxy(IXenConnection connection, string fileName)
        {
            return (Proxy)generator.CreateProxyInstance(proxyType, ProxyInstance(connection, fileName));
        }

        private static IInvocationHandler ProxyInstance(IXenConnection connection, string fileName)
        {
            lock (dicLock)
            {
                if (!proxys.ContainsKey(connection))
                {
                    proxys[connection] = new DbProxy(connection, fileName);
                }

                return proxys[connection];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbProxy"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="url">The http url of filename to the XenServer status report XML document to be used.</param>
        public DbProxy(IXenConnection connection, string url)
        {
            Util.ThrowIfParameterNull(connection, "connection");
            Util.ThrowIfStringParameterNullOrEmpty(url, "url");

            this.url = url;
            this.connection = connection;
            db = new Db(connection, url);
        }

        #region IInvocationHandler Members

       
        

        public object ExecuteMethod(ProxyMethodInfo pmi, object[] args)
        {
            object obj;
            
            if (pmi.FakeType == typeof(fakeUnknown))
            {
                obj = new fakeUnknown(pmi.TypeName, this);
            }
            else
            {
                obj = Activator.CreateInstance(pmi.FakeType, this);
            }

            object[] methodArgs = new object[pmi.FakeMethod.GetParameters().Length];
            for (int i = 0; i < methodArgs.Length; i++)
            {
                methodArgs[i] = args[i];
            }

            switch (pmi.FakeMethodType)
            {
                case MethodType.Sync:
                    return pmi.FakeMethod.Invoke(obj, methodArgs);

                case MethodType.Async:
                    Response<string> response = (Response<string>)pmi.FakeMethod.Invoke(obj, methodArgs);
                    if (response.Status == ResponseTypes.SUCCESS)
                    {
                        return new Response<string>(CreateTask(DbProxy.TaskStatus.success, response.Value));
                    }
                    else
                    {
                        return new Response<string>(CreateTask(DbProxy.TaskStatus.failure, response.ErrorDescription));
                    }

            }

            return new Response<String>(true, new String[] { Failure.INTERNAL_ERROR });
        }

        public event EventHandler<DbProxyInvokingEventArgs> Invoking;

        protected virtual void OnInvoking(DbProxyInvokingEventArgs e)
        {
            var handler = Invoking;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public object Invoke(string proxyMethodName, string returnType, params object[] args)
        {
            VerifyVersion(proxyMethodName);
            
            ProxyMethodInfo pmi = ProxyMethodNameParser.Parse(proxyMethodName);

            OnInvoking(new DbProxyInvokingEventArgs(pmi));

            if (pmi.FakeType != null)
            {
                return ExecuteMethod(pmi, args);
            }

            if (pmi.MethodName.StartsWith("set_"))
            {
                string fieldname = pmi.MethodName.Substring(4);

                // Special case: set_memory_limits is not just editing a "memory_limits" field
                if (fieldname == "memory_limits")
                {
                    EditObject(pmi.TypeName, (string)args[1], "memory_static_min", args[2]);
                    EditObject(pmi.TypeName, (string)args[1], "memory_static_max", args[3]);
                    EditObject(pmi.TypeName, (string)args[1], "memory_dynamic_min", args[4]);
                    EditObject(pmi.TypeName, (string)args[1], "memory_dynamic_max", args[5]);
                }
                else
                {
                    EditObject(pmi.TypeName, (string)args[1], fieldname, args[2]);
                }

                return new Response<string>("");
            }

            switch (pmi.MethodName)
            {
                case "Url":
                    return url;// "http://XenCenter.Simulator/";

                case "get_record":
                    {
                        string uuid = (string)args[1];
                        if (uuid.StartsWith("task"))  // tasks are kept track of separately
                        {
                            return new Response<Proxy_Task>(task_get_record(uuid));
                        }
                        return get_record(pmi.TypeName, uuid, true);
                    }

                case "destroy":
                    {
                        string uuid = (string)args[1];
                        if (uuid.StartsWith("task"))
                        {
                            task_destroy(uuid);
                            return new Response<string>("");
                        }
                        break;
                    }

                case "add_to_other_config":
                    {
                        string uuid = (string)args[1];
                        if (!uuid.StartsWith("task"))  // ignore other_config for tasks (used to keep track of meddling actions)
                        {
                            AddToDictionary(pmi.TypeName, (string)args[1], "other_config", args[2], args[3]);
                        }
                        return new Response<string>("");
                    }

                case "remove_from_other_config":
                    {
                        string uuid = (string)args[1];
                        if (!uuid.StartsWith("task"))  // ignore other_config for tasks (used to keep track of meddling actions)
                        {
                            RemoveFromDictionary(pmi.TypeName, (string)args[1], "other_config", args[2]);
                        }
                        return new Response<string>("");
                    }

                case "add_to_gui_config":
                    AddToDictionary(pmi.TypeName, (string)args[1], "gui_config", args[2], args[3]);
                    return new Response<string>("");

                case "remove_from_gui_config":
                    RemoveFromDictionary(pmi.TypeName, (string)args[1], "gui_config", args[2]);
                    return new Response<string>("");

                case "add_tags":
                    AddToArray(pmi.TypeName, (string)args[1], "tags", args[2]);
                    return new Response<string>("");

                case "remove_tags":
                    RemoveFromArray(pmi.TypeName, (string)args[1], "tags", args[2]);
                    return new Response<string>("");

                case "get_subject_information_from_identifier":
                    Hashtable subjectInfo = new Hashtable();
                    if ((string)args[1] == "SID1")
                    {
                        subjectInfo["subject-name"] = @"citrix\tu_one";
                        subjectInfo["subject-displayname"] = "Test User 1";
                        subjectInfo["subject-is-group"] = "false";
                    }
                    else if ((string)args[1] == "SID2")
                    {
                        subjectInfo["subject-name"] = @"citrix\tu_two";
                        subjectInfo["subject-displayname"] = "Test User 2";
                        subjectInfo["subject-is-group"] = "false";
                    }
                    else if ((string)args[1] == "SID3")
                    {
                        subjectInfo["subject-name"] = @"citrix\tg";
                        subjectInfo["subject-displayname"] = "Test Group";
                        subjectInfo["subject-is-group"] = "true";
                    }
                    return new Response<Object>(subjectInfo);

                // For VBDEditPage.CalculateDevicePositions()
                case "get_allowed_vbd_devices":
                    return new Response<String[]>(new String[] { "0", "1", "2", "3", "4", "5", "6", "7" });

                case "retrieve_wlb_recommendations":
                case "retrieve_wlb_evacuate_recommendations":
                case "retrieve_wlb_configuration":
                    Hashtable tbl = new Hashtable();
                    return new Response<object>(tbl);

                case "get_uncooperative_resident_vms":
                    return new Response<string[]>(new string[0]);

                // For HAWizard_Pages.AssignPriorities
                case "ha_compute_hypothetical_max_host_failures_to_tolerate":
                    return new Response<string>("1");

                case "get_ha_host_failures_to_tolerate":
                    return new Response<string>("1");

                // For EvacuateHostDialog and RollingUpgradeWizard
                case "get_vms_which_prevent_evacuation":
                    return new Response<object>(new Hashtable());

                // For RollingUpgradeWizard
                case "get_live":
                    return new Response<bool>(true);

                case "get_allowed_operations":
                    return new Response<string[]>(new string[0]);

                case "get_health_check_config":
                    string uuid1 = (string)args[1];
                    return new Response<object>(get_health_check_config(pmi.TypeName, uuid1, true));

            }

            return new Response<String>(true, new String[] { Failure.INTERNAL_ERROR });

        }

        #endregion

        // Code to create fake asynchronous tasks that just return a known value

        private static Dictionary<string, Proxy_Task> tasks = new Dictionary<string, Proxy_Task>();
        private readonly Random rand = new Random();

        public enum TaskStatus { success, failure, pending }

        // If success, use the next argument as the result;
        // if !success, use all the subsequent arguments as the error_info.
        public string CreateTask(TaskStatus status, params string[] result)
        {
            Proxy_Task task = new Proxy_Task();
            task.uuid = String.Format("task{0}", rand.Next());
            task.name_label = task.uuid;
            task.name_description = "";
            task.created = DateTime.Now;
            task.status = status.ToString();

            if (status != TaskStatus.pending)
            {
                task.finished = DateTime.Now;
                task.progress = 100.0;

                if (status == TaskStatus.success)
                {
                    task.result = result[0];
                }
                else
                {
                    task.error_info = result;
                }
            }

            tasks.Add(task.uuid, task);
            return task.uuid;
        }

        public Proxy_Task GetTask(string opaque_ref)
        {
            return tasks[opaque_ref];
        }

        private static Proxy_Task task_get_record(string uuid)
        {
            Proxy_Task task;
            if (tasks.TryGetValue(uuid, out task))
            {
                return task;
            }
            return null;
        }

        private static bool task_destroy(string uuid)
        {
            return tasks.Remove(uuid);
        }

        public enum EditTypes { Replace, AddToDict, RemoveFromDict, AddToArray, RemoveFromArray }

        public void EditObject_(EditTypes editType, string typeName, string opaqueRef, string field, params object[] args)
        {
            // Edit the XML to have the new value
            edit_record(editType, typeName, opaqueRef, field, args);

            // Make a Proxy object from the new XML
            Type proxyT = TypeCache.GetProxyType(typeName);
            object proxy = get_record(typeName, opaqueRef, false);

            // Make a Proxy_Event representing this edit, and add it to the events queue
            lock (eventsListLock)
            {
                eventsList.Add(MakeProxyEvent(typeName, opaqueRef, "mod", proxyT, proxy));
            }
        } 

        private void EditObject(string typeName, string opaqueRef, string field, object new_value)
        {
            EditObject_(EditTypes.Replace, typeName, opaqueRef, field, new_value);
        }

        private void AddToDictionary(string typeName, string opaqueRef, string field, object key, object new_value)
        {
            EditObject_(EditTypes.AddToDict, typeName, opaqueRef, field, key, new_value);
        }

        private void RemoveFromDictionary(string typeName, string opaqueRef, string field, object key)
        {
            EditObject_(EditTypes.RemoveFromDict, typeName, opaqueRef, field, key);
        }

        private void AddToArray(string typeName, string opaqueRef, string field, object value)
        {
            EditObject_(EditTypes.AddToArray, typeName, opaqueRef, field, value);
        }

        private void RemoveFromArray(string typeName, string opaqueRef, string field, object value)
        {
            EditObject_(EditTypes.RemoveFromArray, typeName, opaqueRef, field, value);
        }

        public static Proxy_Event MakeProxyEvent(string typeName, string opaqueRef, string operation, Type ProxyT, object proxy)
        {
            Proxy_Event ev = new Proxy_Event();
            ev.class_ = typeName;
            ev.opaqueRef = opaqueRef;
            ev.operation = operation;
            ev.snapshot = ProxyToHashtable(ProxyT, proxy);
            return ev;
        }

        public static Hashtable ProxyToHashtable(Type proxyT, object proxy)
        {
            Hashtable table = new Hashtable();
            FieldInfo[] fields = proxyT.GetFields();
            foreach (FieldInfo field in fields)
            {
                table[field.Name] = field.GetValue(proxy);
            }
            return table;
        }

        public Object get_record(String clazz, String opaqueRef, bool makeResponse)
        {
            Db.Table table = db.Tables[clazz];

            Type type = table.XapiType;

            if (!table.Rows.ContainsKey(opaqueRef))
            {
                if (makeResponse)
                {
                    return typeof(Response<>).MakeGenericType(type).GetConstructor(new Type[] { typeof(bool), typeof(String[]) })
                        .Invoke(new Object[] { true, new String[] { Failure.OBJECT_NO_LONGER_EXISTS, opaqueRef } });
                }

                throw new Exception(Failure.OBJECT_NO_LONGER_EXISTS);
            }

            Db.Row row = table.Rows[opaqueRef];
            Object result = Activator.CreateInstance(type);

            foreach (string propName in row.Props.Keys)
            {
                FieldInfo info = type.GetField(propName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                info.SetValue(result, row.Props[propName].XapiObjectValue);
            }

            if (makeResponse)
            {
                return typeof(Response<>).MakeGenericType(type).GetConstructor(new Type[] { type }).Invoke(new Object[] { result });
            }

            return result;
        }

        public Object get_health_check_config(String clazz, String opaqueRef, bool makeResponse)
        {
            Db.Table table = db.Tables[clazz];

            if (!table.Rows.ContainsKey(opaqueRef))
            {
                if (makeResponse)
                    return new object();

                throw new Exception(Failure.OBJECT_NO_LONGER_EXISTS);
            }
            return table.Rows[opaqueRef].Props["health_check_config"].XapiObjectValue;
        }

        private void edit_record(EditTypes editType, string clazz, string opaqueRef, string field, params object[] args)
        {
            Db.Row row = db.Tables[clazz].Rows[opaqueRef];
            Db.Prop prop;
            if (row.Props.TryGetValue(field, out prop))
            {
                prop.XapiObjectValue = NewValue(editType, prop.XapiObjectValue, args);
            }
            else
            {
                prop = new Db.Prop(row, field, NewValue(editType, null, args));
                row.Props[field] = prop;
            }

        }

        private object NewValue(EditTypes editType, object fieldBefore, object[] args)
        {
            switch (editType)
            {
                case EditTypes.Replace:
                    return args[0];

                case EditTypes.AddToDict:
                    Hashtable ht = fieldBefore == null ? new Hashtable() : (Hashtable)fieldBefore;
                    ht[args[0]] = args[1];
                    return ht;

                case EditTypes.RemoveFromDict:
                    if (fieldBefore == null)
                    {
                        return new Hashtable();
                    }
                    else
                    {
                        ht = (Hashtable)fieldBefore;
                        ht.Remove(args[0]);
                        return ht;
                    }

                case EditTypes.AddToArray:
                    // We only support string[] at the moment
                    string[] arr = fieldBefore == null ? new string[0] : (string[])fieldBefore;
                    List<string> list = new List<string>(arr);
                    if (!list.Contains((string)args[0]))
                    {
                        list.Add((string)args[0]);
                    }
                    return list.ToArray();

                case EditTypes.RemoveFromArray:
                    if (fieldBefore == null)
                    {
                        return new string[0];
                    }
                    else
                    {
                        arr = (string[])fieldBefore;
                        list = new List<string>(arr);
                        while (list.Contains((string)args[0]))
                        {
                            list.Remove((string)args[0]);
                        }
                        return list.ToArray();
                    }

                default:
                    System.Diagnostics.Trace.Assert(false);
                    return null;
            }
        }

        public string GetRandomRef(string clazz)
        {
            string[] refs = new string[db.Tables[clazz].Rows.Keys.Count];
            db.Tables[clazz].Rows.Keys.CopyTo(refs, 0);

            return refs[rand.Next(0, refs.Length)];
        }

        public string CopyObject(string clazz, string opaque_ref)
        {
            if (!db.Tables[clazz].Rows.ContainsKey(opaque_ref))
                return Helper.NullOpaqueRef;
            Db.Row r = db.Tables[clazz].Rows[opaque_ref].CopyOf();
            string new_opaque_ref = CreateOpaqueRef();
            r.Props["uuid"].XapiObjectValue = Guid.NewGuid().ToString(); // this object needs a new uuid & ref
            db.Tables[clazz].Rows.Add(new_opaque_ref, r);
            return new_opaque_ref;
        }

        public void SendCreateObject(string typeName, string opaqueRef)
        {
            // Make a Proxy object from the new XML
            Type proxyT = TypeCache.GetProxyType(typeName);
            object proxy = get_record(typeName, opaqueRef, false);

            // Make a Proxy_Event representing this edit, and add it to the events queue
            lock (eventsListLock)
            {
                eventsList.Add(MakeProxyEvent(typeName, opaqueRef, "add", proxyT, proxy));
            }
        }

        public void SendDestroyObject(string typeName, string opaqueRef)
        {
            // Make a Proxy object from the new XML
            Type proxyT = TypeCache.GetProxyType(typeName);

            // Make a Proxy_Event representing this edit, and add it to the events queue
            lock (eventsListLock)
            {
                eventsList.Add(MakeProxyEvent(typeName, opaqueRef, "del", proxyT, Activator.CreateInstance(proxyT)));
            }
        }

        public void SendModObject(string typeName, string opaqueRef)
        {
            // Make a Proxy object from the new XML
            Type proxyT = TypeCache.GetProxyType(typeName);
            object proxy = get_record(typeName, opaqueRef, false);

            // Make a Proxy_Event representing this edit, and add it to the events queue
            lock (eventsListLock)
            {
                eventsList.Add(MakeProxyEvent(typeName, opaqueRef, "mod", proxyT, proxy));
            }
        }

        /// <summary>
        /// Verifies that the specified call can be made with this version of server.
        /// </summary>
        private void VerifyVersion(string call)
        {
            string version = null;

            foreach (string key in callVersions.Keys)
            {
                if (callVersions[key].Contains(call))
                {
                    version = key;
                    break;
                }
            }
        }

        public string CreateOpaqueRef()
        {
            return string.Format("OpaqueRef:{0}", Guid.NewGuid());
        }
    }
}
