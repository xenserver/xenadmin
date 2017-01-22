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
using System.Collections.Generic;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdminTests.XenModelTests;
using XenAPI;

namespace XenAdminTests
{
    public abstract class TestObjectProvider
    {

        public abstract List<IXenConnection> ConnectionManager { get; }
        public abstract List<IXenConnection> ConnectionManagerCopy { get; }

        protected IXenConnection GetAnyConnection(Predicate<IXenConnection> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (cond == null || cond(connection))
                    return connection;
            }
            Assert.Fail("Failed to find connection");
            return null;
        }

        protected IXenConnection GetAnyConnection()
        {
            return GetAnyConnection(null);
        }

        // Only returns true pools, not pools-of-one
        protected Pool GetAnyPool(Predicate<Pool> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                Pool pool = Helpers.GetPool(connection);
                if (pool != null && (cond == null || cond(pool)))
                    return pool;
            }
            Assert.Fail("Failed to find pool");
            return null;
        }

        protected Pool GetAnyPool()
        {
            return GetAnyPool(null);
        }

        // Returns any pool or pool-of-one
        protected Pool GetAnyPoolOfOne(Predicate<Pool> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool != null && (cond == null || cond(pool)))
                    return pool;
            }
            Assert.Fail("Failed to find pool");
            return null;
        }

        protected Pool GetAnyPoolOfOne()
        {
            return GetAnyPoolOfOne(null);
        }

        protected Host GetAnyHost(Predicate<Host> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (Host host in connection.Cache.Hosts)
                {
                    if (cond == null || cond(host))
                        return host;
                }
            }
            Assert.Fail("Failed to find host");
            return null;
        }

        protected Host GetAnyHost()
        {
            return GetAnyHost(null);
        }

        protected VM GetAnyVM(Predicate<VM> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VM vm in connection.Cache.VMs)
                {
                    if (vm.is_a_real_vm && (cond == null || cond(vm)))
                        return vm;
                }
            }
            Assert.Fail("Failed to find VM");
            return null;
        }

        protected VM GetAnyVM()
        {
            return GetAnyVM(null);
        }

        protected VM GetAnyDefaultTemplate(Predicate<VM> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VM vm in connection.Cache.VMs)
                {
                    if (vm.is_a_template && !vm.is_a_snapshot && 
                        vm.Show(XenAdminConfigManager.Provider.ShowHiddenVMs) &&
                        vm.DefaultTemplate && (cond == null || cond(vm)))
                        return vm;
                }
            }
            Assert.Fail("Failed to find default template");
            return null;
        }

        protected VM GetAnyDefaultTemplate()
        {
            return GetAnyDefaultTemplate(null);
        }

        protected VM GetAnyUserTemplate(Predicate<VM> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VM vm in connection.Cache.VMs)
                {
                    if (vm.is_a_template && !vm.is_a_snapshot &&
                        !vm.DefaultTemplate && (cond == null || cond(vm)))
                        return vm;
                }
            }
            Assert.Fail("Failed to find user template");
            return null;
        }

        protected VM GetAnyTemplate(Predicate<VM> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VM vm in connection.Cache.VMs)
                {
                    if (vm.is_a_template && (cond == null || cond(vm)))
                        return vm;
                }
            }
            Assert.Fail("Failed to find user template");
            return null;
        }

        protected VM GetAnyUserTemplate()
        {
            return GetAnyUserTemplate(null);
        }

        protected VM GetAnySnapshot(Predicate<VM> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VM vm in connection.Cache.VMs)
                {
                    if (vm.is_a_snapshot && (cond == null || cond(vm)))
                        return vm;
                }
            }
            Assert.Fail("Failed to find snapshot");
            return null;
        }

        protected VM GetAnySnapshot()
        {
            return GetAnySnapshot(null);
        }

        protected SR GetAnySR(Predicate<SR> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (SR sr in connection.Cache.SRs)
                {
                    if (cond == null || cond(sr))
                        return sr;
                }
            }
            Assert.Fail("Failed to find storage repository");
            return null;
        }

        protected SR GetAnySR()
        {
            return GetAnySR(null);
        }

        protected Network GetAnyNetwork(Predicate<Network> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (Network network in connection.Cache.Networks)
                {
                    if (cond == null || cond(network))
                        return network;
                }
            }
            Assert.Fail("Failed to find network");
            return null;
        }

        protected Network GetAnyNetwork()
        {
            return GetAnyNetwork(null);
        }

        protected VBD GetAnyVBD(Predicate<VBD> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VBD vbd in connection.Cache.VBDs)
                {
                    if (cond == null || cond(vbd))
                        return vbd;
                }
            }
            Assert.Fail("Failed to find VBD");
            return null;
        }

        protected VBD GetAnyVBD()
        {
            return GetAnyVBD(null);
        }

        protected VIF GetAnyVIF(Predicate<VIF> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VIF vif in connection.Cache.VIFs)
                {
                    if (cond == null || cond(vif))
                        return vif;
                }
            }
            Assert.Fail("Failed to find VIF");
            return null;
        }

        protected VIF GetAnyVIF()
        {
            return GetAnyVIF(null);
        }

        protected VDI GetAnyVDI(Predicate<VDI> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (VDI vdi in connection.Cache.VDIs)
                {
                    if (cond == null || cond(vdi))
                        return vdi;
                }
            }
            Assert.Fail("Failed to find VDI");
            return null;
        }

        protected VDI GetAnyVDI()
        {
            return GetAnyVDI(null);
        }

        protected Folder GetAnyFolder(Predicate<Folder> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (!connection.IsConnected)
                    continue;

                foreach (Folder folder in connection.Cache.Folders)
                {
                    if (cond == null || cond(folder))
                        return folder;
                }
            }
            Assert.Fail("Failed to find folder");
            return null;
        }

        protected Folder GetAnyFolder()
        {
            return GetAnyFolder(null);
        }

        protected IXenObject GetAnyXenObject(Predicate<IXenObject> cond)
        {
            foreach (IXenConnection connection in ConnectionManager)
            {
                if (connection.IsConnected)
                {
                    foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                    {
                        if (cond(o))
                        {
                            return o;
                        }
                    }
                }
            }

            Assert.Fail("Failed to find IXenObject");
            return null;
        }

        protected List<T> GetAllXenObjects<T>() where T : IXenObject
        {
            return GetAllXenObjects<T>(t => true);
        }

        protected List<T> GetAllXenObjects<T>(Predicate<T> cond) where T : IXenObject
        {
            List<T> output = new List<T>();

            foreach (IXenConnection connection in ConnectionManager)
            {
                if (connection.IsConnected)
                {
                    foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                    {
                        if (o is T && cond((T)o))
                        {
                            output.Add((T)o);
                        }
                    }
                }
            }
            return output;
        }
    }
}
