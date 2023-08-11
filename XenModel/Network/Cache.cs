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
using System.ComponentModel;
using System.Reflection;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Model;
using System.Linq;
using System.Diagnostics;
using XenCenterLib;

namespace XenAdmin.Network
{
    public class Cache : ICache
    {
#pragma warning disable 0414

        // keep sorted please
        private readonly ChangeableDictionary<XenRef<Blob>, Blob> _blob = new ChangeableDictionary<XenRef<Blob>, Blob>();
        private readonly ChangeableDictionary<XenRef<Bond>, Bond> _bond = new ChangeableDictionary<XenRef<Bond>, Bond>();
        private readonly ChangeableDictionary<XenRef<Certificate>, Certificate> _certificates = new ChangeableDictionary<XenRef<Certificate>, Certificate>();
        private readonly ChangeableDictionary<XenRef<Cluster_host>, Cluster_host> _cluster_host = new ChangeableDictionary<XenRef<Cluster_host>, Cluster_host>();
        private readonly ChangeableDictionary<XenRef<Cluster>, Cluster> _cluster = new ChangeableDictionary<XenRef<Cluster>, Cluster>();
        private readonly ChangeableDictionary<XenRef<XenAPI.Console>, XenAPI.Console> _console = new ChangeableDictionary<XenRef<XenAPI.Console>, XenAPI.Console>();
        private readonly ChangeableDictionary<XenRef<Crashdump>, Crashdump> _crashdump = new ChangeableDictionary<XenRef<Crashdump>, Crashdump>();
        private readonly ChangeableDictionary<XenRef<DockerContainer>, DockerContainer> _dockerContainers = new ChangeableDictionary<XenRef<DockerContainer>, DockerContainer>();
        private readonly ChangeableDictionary<XenRef<Feature>, Feature> _feature = new ChangeableDictionary<XenRef<Feature>, Feature>();
        private readonly ChangeableDictionary<XenRef<Folder>, Folder> _folders = new ChangeableDictionary<XenRef<Folder>, Folder>();
        private readonly ChangeableDictionary<XenRef<GPU_group>, GPU_group> _gpu_groups = new ChangeableDictionary<XenRef<GPU_group>, GPU_group>();
        private readonly ChangeableDictionary<XenRef<Host_cpu>, Host_cpu> _host_cpu = new ChangeableDictionary<XenRef<Host_cpu>, Host_cpu>();
        private readonly ChangeableDictionary<XenRef<Host_crashdump>, Host_crashdump> _host_crashdump = new ChangeableDictionary<XenRef<Host_crashdump>, Host_crashdump>();
        private readonly ChangeableDictionary<XenRef<Host_metrics>, Host_metrics> _host_metrics = new ChangeableDictionary<XenRef<Host_metrics>, Host_metrics>();
        private readonly ChangeableDictionary<XenRef<Host_patch>, Host_patch> _host_patch = new ChangeableDictionary<XenRef<Host_patch>, Host_patch>();
        private readonly ChangeableDictionary<XenRef<Host>, Host> _host = new ChangeableDictionary<XenRef<Host>, Host>();
        private readonly ChangeableDictionary<XenRef<Message>, Message> _message = new ChangeableDictionary<XenRef<Message>, Message>();
        private readonly ChangeableDictionary<XenRef<Network_sriov>, Network_sriov> _network_sriov = new ChangeableDictionary<XenRef<Network_sriov>, Network_sriov>();
        private readonly ChangeableDictionary<XenRef<XenAPI.Network>, XenAPI.Network> _network = new ChangeableDictionary<XenRef<XenAPI.Network>, XenAPI.Network>();
        private readonly ChangeableDictionary<XenRef<PBD>, PBD> _pbd = new ChangeableDictionary<XenRef<PBD>, PBD>();
        private readonly ChangeableDictionary<XenRef<PCI>, PCI> _pcis = new ChangeableDictionary<XenRef<PCI>, PCI>();
        private readonly ChangeableDictionary<XenRef<PGPU>, PGPU> _pgpu = new ChangeableDictionary<XenRef<PGPU>, PGPU>();
        private readonly ChangeableDictionary<XenRef<PIF_metrics>, PIF_metrics> _pif_metrics = new ChangeableDictionary<XenRef<PIF_metrics>, PIF_metrics>();
        private readonly ChangeableDictionary<XenRef<PIF>, PIF> _pif = new ChangeableDictionary<XenRef<PIF>, PIF>();
        private readonly ChangeableDictionary<XenRef<Pool_patch>, Pool_patch> _pool_patch = new ChangeableDictionary<XenRef<Pool_patch>, Pool_patch>();
        private readonly ChangeableDictionary<XenRef<Pool_update>, Pool_update> _pool_update = new ChangeableDictionary<XenRef<Pool_update>, Pool_update>();
        private readonly ChangeableDictionary<XenRef<Pool>, Pool> _pool = new ChangeableDictionary<XenRef<Pool>, Pool>();
        private readonly ChangeableDictionary<XenRef<PUSB>, PUSB> _pusb = new ChangeableDictionary<XenRef<PUSB>, PUSB>();
        private readonly ChangeableDictionary<XenRef<PVS_cache_storage>, PVS_cache_storage> _pvs_cache_storage = new ChangeableDictionary<XenRef<PVS_cache_storage>, PVS_cache_storage>();
        private readonly ChangeableDictionary<XenRef<PVS_proxy>, PVS_proxy> _pvs_proxy = new ChangeableDictionary<XenRef<PVS_proxy>, PVS_proxy>();
        private readonly ChangeableDictionary<XenRef<PVS_server>, PVS_server> _pvs_server = new ChangeableDictionary<XenRef<PVS_server>, PVS_server>();
        private readonly ChangeableDictionary<XenRef<PVS_site>, PVS_site> _pvs_site = new ChangeableDictionary<XenRef<PVS_site>, PVS_site>();
        private readonly ChangeableDictionary<XenRef<Repository>, Repository> _repositories = new ChangeableDictionary<XenRef<Repository>, Repository>();
        private readonly ChangeableDictionary<XenRef<Role>, Role> _role = new ChangeableDictionary<XenRef<Role>, Role>();
        private readonly ChangeableDictionary<XenRef<SM>, SM> _sm = new ChangeableDictionary<XenRef<SM>, SM>();
        private readonly ChangeableDictionary<XenRef<SR>, SR> _sr = new ChangeableDictionary<XenRef<SR>, SR>();
        private readonly ChangeableDictionary<XenRef<Subject>, Subject> _subject = new ChangeableDictionary<XenRef<Subject>, Subject>();
        private readonly ChangeableDictionary<XenRef<Task>, Task> _task = new ChangeableDictionary<XenRef<Task>, Task>();
        private readonly ChangeableDictionary<XenRef<Tunnel>, Tunnel> _tunnel = new ChangeableDictionary<XenRef<Tunnel>, Tunnel>();
        private readonly ChangeableDictionary<XenRef<USB_group>, USB_group> _usb_group = new ChangeableDictionary<XenRef<USB_group>, USB_group>();
        private readonly ChangeableDictionary<XenRef<VBD_metrics>, VBD_metrics> _vbd_metrics = new ChangeableDictionary<XenRef<VBD_metrics>, VBD_metrics>();
        private readonly ChangeableDictionary<XenRef<VBD>, VBD> _vbd = new ChangeableDictionary<XenRef<VBD>, VBD>();
        private readonly ChangeableDictionary<XenRef<VDI>, VDI> _vdi = new ChangeableDictionary<XenRef<VDI>, VDI>();
        private readonly ChangeableDictionary<XenRef<VGPU_type>, VGPU_type> _vgpu_types = new ChangeableDictionary<XenRef<VGPU_type>, VGPU_type>();
        private readonly ChangeableDictionary<XenRef<VGPU>, VGPU> _vgpu = new ChangeableDictionary<XenRef<VGPU>, VGPU>();
        private readonly ChangeableDictionary<XenRef<VIF_metrics>, VIF_metrics> _vif_metrics = new ChangeableDictionary<XenRef<VIF_metrics>, VIF_metrics>();
        private readonly ChangeableDictionary<XenRef<VIF>, VIF> _vif = new ChangeableDictionary<XenRef<VIF>, VIF>();
        private readonly ChangeableDictionary<XenRef<VLAN>, VLAN> _vlan = new ChangeableDictionary<XenRef<VLAN>, VLAN>();
        private readonly ChangeableDictionary<XenRef<VM_appliance>, VM_appliance> _vm_appliance = new ChangeableDictionary<XenRef<VM_appliance>, VM_appliance>();
        private readonly ChangeableDictionary<XenRef<VM_guest_metrics>, VM_guest_metrics> _vm_guest_metrics = new ChangeableDictionary<XenRef<VM_guest_metrics>, VM_guest_metrics>();
        private readonly ChangeableDictionary<XenRef<VM_metrics>, VM_metrics> _vm_metrics = new ChangeableDictionary<XenRef<VM_metrics>, VM_metrics>();
        private readonly ChangeableDictionary<XenRef<VM>, VM> _vm = new ChangeableDictionary<XenRef<VM>, VM>();
        private readonly ChangeableDictionary<XenRef<VMSS>, VMSS> _vmss = new ChangeableDictionary<XenRef<VMSS>, VMSS>();
        private readonly ChangeableDictionary<XenRef<VTPM>, VTPM> _vtpms = new ChangeableDictionary<XenRef<VTPM>, VTPM>();
        private readonly ChangeableDictionary<XenRef<VUSB>, VUSB> _vusb = new ChangeableDictionary<XenRef<VUSB>, VUSB>();

#pragma warning restore 0414

        private bool foldersChanged;
        private bool dockerContainersChanged;
        private readonly Dictionary<Type, IDictionary> dictionaries = new Dictionary<Type, IDictionary>();

        public Cache()
        {
            foreach (FieldInfo f in GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (f.FieldType.Name.StartsWith("ChangeableDictionary"))
                {
                    dictionaries.Add(f.FieldType.GetGenericArguments()[0].GetGenericArguments()[0],
                                     (IDictionary)f.GetValue(this));
                }
            }
        }

        #region Properties

        public Bond[] Bonds => contents(_bond);

        public Certificate[] Certificates => contents(_certificates);

        public Cluster[] Clusters => contents(_cluster);

        public Cluster_host[] Cluster_hosts => contents(_cluster_host);

        public DockerContainer[] DockerContainers => contents(_dockerContainers);

        public Feature[] Features => contents(_feature);

        public Folder[] Folders => contents(_folders);

        public GPU_group[] GPU_groups => contents(_gpu_groups);

        public Host[] Hosts => contents(_host);

        public Host_cpu[] Host_cpus => contents(_host_cpu);

        public int HostCount => _host.Count;

        public Message[] Messages => contents(_message);

        public XenAPI.Network[] Networks => contents(_network);

        public Network_sriov[] Network_sriov => contents(_network_sriov);

        public PBD[] PBDs => contents(_pbd);

        public PCI[] PCIs => contents(_pcis);

        public PGPU[] PGPUs => contents(_pgpu);

        public PIF[] PIFs => contents(_pif);

        public Pool[] Pools => contents(_pool);

        public Pool_patch[] Pool_patches => contents(_pool_patch);

        public Pool_update[] Pool_updates => contents(_pool_update);

        public PUSB[] PUSBs => contents(_pusb);

        public PVS_cache_storage[] PVS_cache_storages => contents(_pvs_cache_storage);

        public PVS_proxy[] PVS_proxies => contents(_pvs_proxy);

        public PVS_server[] PVS_servers => contents(_pvs_server);

        public PVS_site[] PVS_sites => contents(_pvs_site);

        public Repository[] Repositories => contents(_repositories);

        public Role[] Roles => contents(_role);

        public SM[] SMs => contents(_sm);

        public SR[] SRs => contents(_sr);

        public Subject[] Subjects => contents(_subject);

        public Tunnel[] Tunnels => contents(_tunnel);

        public USB_group[] USB_groups => contents(_usb_group);

        public VBD[] VBDs => contents(_vbd);

        public VDI[] VDIs => contents(_vdi);

        public VGPU[] VGPUs => contents(_vgpu);

        public VGPU_type[] VGPU_types => contents(_vgpu_types);

        public VIF[] VIFs => contents(_vif);

        public VM[] VMs => contents(_vm);

        public VM_appliance[] VM_appliances => contents(_vm_appliance);

        public VMSS[] VMSSs => contents(_vmss);

        public VTPM[] VTPMs => contents(_vtpms);

        public VUSB[] VUSBs => contents(_vusb);

        #endregion

        private static T[] contents<T>(ChangeableDictionary<XenRef<T>, T> d) where T : XenObject<T>
        {
            lock (d)
            {
                T[] result = new T[d.Values.Count];
                int i = 0;
                foreach (T o in d.Values)
                {
                    result[i] = o;
                    i++;
                }
                return result;
            }
        }

        /// <summary>
        /// Returns the collection for the given type, or null if no such dictionary is present.
        /// </summary>
        private IDictionary GetCollectionForType(Type t)
        {
            return dictionaries.ContainsKey(t) ? dictionaries[t] : null;
        }

        public void DeregisterCollectionChanged<T>(CollectionChangeEventHandler h) where T : XenObject<T>
        {
            ChangeableDictionary<XenRef<T>, T> d = GetCollectionForType(typeof(T)) as ChangeableDictionary<XenRef<T>, T>;
            if (d == null)
                return;

            d.CollectionChanged -= h;
        }

        public void RegisterCollectionChanged<T>(CollectionChangeEventHandler h) where T : XenObject<T>
        {
            ChangeableDictionary<XenRef<T>, T> d = GetCollectionForType(typeof(T)) as ChangeableDictionary<XenRef<T>, T>;
            if (d == null)
                return;

            d.CollectionChanged -= h;
            d.CollectionChanged += h;
        }

        public void DeregisterBatchCollectionChanged<T>(EventHandler h) where T : XenObject<T>
        {
            ChangeableDictionary<XenRef<T>, T> d = GetCollectionForType(typeof(T)) as ChangeableDictionary<XenRef<T>, T>;
            if (d == null)
                return;

            d.BatchCollectionChanged -= h;
        }

        public void RegisterBatchCollectionChanged<T>(EventHandler h) where T : XenObject<T>
        {
            ChangeableDictionary<XenRef<T>, T> d = GetCollectionForType(typeof(T)) as ChangeableDictionary<XenRef<T>, T>;
            if (d == null)
                return;

            d.BatchCollectionChanged -= h;
            d.BatchCollectionChanged += h;
        }

        public void AddAll<T>(List<T> l, Predicate<T> p) where T : XenObject<T>
        {
            IDictionary d = GetCollectionForType(typeof(T));
            if (d == null)
                return;

            lock (d)
            {
                if (p == null)
                    foreach (T value in d.Values)
                        l.Add(value);
                else
                    foreach (T value in d.Values)
                        if (p(value))
                            l.Add(value);
            }
        }

        private static MethodInfo ClearMethod = typeof(Cache).GetMethod("Clear_", BindingFlags.NonPublic | BindingFlags.Instance);

        public void Clear()
        {
            foreach (IDictionary d in dictionaries.Values)
            {
                lock (d)
                {
                    object[] args = { d };
                    ClearMethod.MakeGenericMethod(APIType(d)).Invoke(this, args);
                }

                INotifyCollectionChanged d1 = d as INotifyCollectionChanged;
                if (d1 != null)
                    d1.OnBatchCollectionChanged();
            }
        }

        private static MethodInfo UpdateFromMethod = typeof(Cache).GetMethod("UpdateFrom_", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <returns>true if some changes have been made.  This is used to indicate that XenObjectsUpdated should be fired by IXenConnection.</returns>
        public bool UpdateFrom(IXenConnection connection, IList<ObjectChange> changes)
        {
            Dictionary<IDictionary, object> tofire = new Dictionary<IDictionary, object>();
            foreach (ObjectChange o in changes)
            {
                if (IgnoreObjectChange(o))
                    continue;

                IDictionary d = GetCollectionForType(o.type);
                if (d == null)
                    continue;
                object[] args = { connection, d, o };

                UpdateFromMethod.MakeGenericMethod(o.type).Invoke(this, args);

                tofire[d] = null;
            }

            bool result = false;
            foreach (IDictionary d in tofire.Keys)
            {
                INotifyCollectionChanged n = d as INotifyCollectionChanged;
                if (n != null)
                    n.OnBatchCollectionChanged();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// For performance reasons, we ignore some events.  These are:
        /// 1. The heartbeat event from xapi, which is a "change" on the pool object, but where the object doesn't actually change.
        /// 2. Tasks that are frequent and which we don't care about, like SR.scan.
        /// 3. Changes on SR that only toggle current_operations.  These happen at the same time as the
        /// periodic SR.scan.
        /// </summary>
        private bool IgnoreObjectChange(ObjectChange obj)
        {
            if (obj.value == null)
            {
                // Object is being deleted.  Never ignore these!
                return false;
            }
            else if (obj.type == typeof(Task))
            {
                Task task = (Task)obj.value;
                return task.IgnoreInCacheUpdate();
            }
            else if (obj.type == typeof(Pool))
            {
                Pool newPool = (Pool)obj.value;
                Pool oldPool = Resolve(new XenRef<Pool>(obj.xenref));
                return oldPool != null && newPool.DeepEquals(oldPool, false); //We do want to see changes in the Pool.current_operations.
            }
            else if (obj.type == typeof(SR))
            {
                SR newSR = (SR)obj.value;
                SR oldSR = Resolve(new XenRef<SR>(obj.xenref));
                return oldSR != null && newSR.DeepEquals(oldSR, true);
            }
            else
            {
                return false;
            }
        }

        public T Find_By_Uuid<T>(string uuid) where T : XenObject<T>
        {
            Type t = typeof(T);
            PropertyInfo p = t.GetProperty("uuid", BindingFlags.Public | BindingFlags.Instance);
            if (p == null)
                return null;
            ChangeableDictionary<XenRef<T>, T> d = GetCollectionForType(t) as ChangeableDictionary<XenRef<T>, T>;
            if (d == null)
                return null;
            lock (d)
            {
                foreach (T m in d.Values)
                {
                    if (string.Equals((string)p.GetValue(m, null), uuid, StringComparison.OrdinalIgnoreCase))
                        return m;
                }
            }
            return null;
        }

        /// <summary>
        /// Find a XenRef corresponding to the given XenObject, using its UUID.
        /// Returns null if no such object is found.
        /// </summary>
        public XenRef<T> FindRef<T>(T needle) where T : XenObject<T>
        {
            Type t = typeof(T);
            PropertyInfo p = t.GetProperty("uuid", BindingFlags.Public | BindingFlags.Instance);
            if (p == null)
                return null;
            string uuid = (string)p.GetValue(needle, null);

            ChangeableDictionary<XenRef<T>, T> d = GetCollectionForType(t) as ChangeableDictionary<XenRef<T>, T>;
            if (d == null)
                return null;
            lock (d)
            {
                foreach (KeyValuePair<XenRef<T>, T> kvp in d)
                {
                    if (string.Equals((string)p.GetValue(kvp.Value, null), uuid, StringComparison.OrdinalIgnoreCase))
                        return kvp.Key;
                }
            }
            return null;
        }

        public bool TryResolve<T>(XenRef<T> xenRef, out T result) where T : XenObject<T>
        {
            result = Resolve(xenRef);
            return result != null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xenRef">May be null, in which case null is returned. May not be a null ref.</param>
        /// <returns></returns>
        public T Resolve<T>(XenRef<T> xenRef) where T : XenObject<T>
        {
            if (xenRef == null)
                return null;

            ChangeableDictionary<XenRef<T>, T> d = GetCollectionForType(typeof(T)) as ChangeableDictionary<XenRef<T>, T>;
            if (d == null)
                return null;
            T result;
            return d.TryGetValue(xenRef, out result) ? result : null;
        }

        private void Clear_<T>(ChangeableDictionary<XenRef<T>, T> o) where T : XenObject<T>
        {
            // explicitly remove each element so change events are fired
            XenRef<T>[] keys = new XenRef<T>[o.Keys.Count];
            o.Keys.CopyTo(keys, 0);
            foreach (XenRef<T> key in keys)
            {
                lock (o)
                {
                    o.Remove(key);
                }
            }
        }

        private void UpdateFrom_<T>(Network.IXenConnection connection, ChangeableDictionary<XenRef<T>, T> target, ObjectChange source) where T : XenObject<T>, new()
        {
            XenRef<T> xenref = source.xenref as XenRef<T>;
            if (xenref == null)
            {
                xenref = new XenRef<T>((string)source.xenref);
            }

            if (source.value != null)
            {
                T to_update = null;
                lock (target)
                {
                    if (!target.TryGetValue(xenref, out to_update))
                    {
                        // add
                        T obj = new T();
                        obj.Connection = connection;
                        obj.UpdateFrom((T)source.value);
                        obj.opaque_ref = xenref.opaque_ref;
                        target.Add(xenref, obj);
                    }
                }

                // Update the object that we found above.  Note that this needs to be done out of the
                // scope of the lock(target), as UpdateFrom is going to fire events.
                if (to_update != null)
                    to_update.UpdateFrom((T)source.value);
            }
            else
            {
                // delete the source object from our model
                lock (target)
                {
                    target.Remove(xenref);
                }
            }
        }

        private Type APIType(IDictionary d)
        {
            return d.GetType().GetGenericArguments()[1];
        }

        public void AddFolder(XenRef<Folder> path, Folder folder)
        {
            _folders[path] = folder;
            foldersChanged = true;
        }

        public void RemoveFolder(XenRef<Folder> path)
        {
            lock (_folders)
            {
                _folders.Remove(path);
            }
            foldersChanged = true;
        }

        public void CheckFoldersBatchChange()
        {
            if (foldersChanged)
            {
                foldersChanged = false;
                _folders.OnBatchCollectionChanged();
            }
        }

        public void CheckDockerContainersBatchChange()
        {
            if (dockerContainersChanged)
            {
                dockerContainersChanged = false;
                _dockerContainers.OnBatchCollectionChanged();
            }
        }

        public void UpdateDockerContainersForVM(IList<DockerContainer> containers, VM vm)
        {
            Trace.Assert(vm != null);

            //updating existing, adding new containers
            dockerContainersChanged = dockerContainersChanged || containers.Count > 0;
            foreach (var c in containers)
            {
                _dockerContainers[new XenRef<DockerContainer>(c)] = c;
            }

            List<KeyValuePair<XenRef<DockerContainer>, DockerContainer>> containersGoneFromThisVM = null;
            //removing the ones that are not there anymore on this VM
            lock (_dockerContainers)
            {
                containersGoneFromThisVM = _dockerContainers.Where(c => c.Value != null && c.Value.Parent.uuid == vm.uuid && !containers.Any(cont => cont.uuid == c.Value.uuid)).ToList();
                dockerContainersChanged = dockerContainersChanged || containersGoneFromThisVM.Count > 0;
                foreach (var c in containersGoneFromThisVM)
                {
                    _dockerContainers.Remove(new XenRef<DockerContainer>(c.Value));
                }
            }
        }

        public IEnumerable<IXenObject> XenSearchableObjects
        {
            get
            {
                foreach (IXenObject o in VMs)
                    yield return o;

				foreach (IXenObject o in VM_appliances)
					yield return o;

                foreach (IXenObject o in Hosts)
                    yield return o;

                foreach (IXenObject o in SRs)
                    yield return o;

                foreach (IXenObject o in Networks)
                    yield return o;

                foreach (IXenObject o in VDIs)
                    yield return o;

                foreach (IXenObject o in Folders)
                    yield return o;

                foreach (IXenObject o in DockerContainers)
                    yield return o;

                foreach (Pool pool in Pools)
                {
                    if (pool != null && pool.IsVisible())
                    {
                        yield return pool;
                        break;
                    }
                }
            }
        }
    }
}
