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
using System.Linq;
using Moq;
using XenAdmin;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAPI;

namespace XenAdminTests
{
    public interface IObjectManager : IDisposable
    {
        List<IXenConnection> AllConnections { get; }
        void CreateNewConnection(params string[] ids);
    }

    /// <summary>
    /// Provide the infrastructure for the mock connection to the server
    /// connection will provide mock versions of a cache, proxy and session, pre-setup
    /// 
    /// This class provides access to the Mock objects it auto generates an connects up
    /// The mocked connection is pre-setup to provide access to the underlying objects
    /// 
    /// Note: if you cannot mock a method, you will need to make it virtual
    /// </summary>
    public class MockObjectManager : IObjectManager
    {
        private readonly Dictionary<string, Mock<IXenConnection>> connections = new Dictionary<string, Mock<IXenConnection>>();
        private readonly Dictionary<string, Mock<ICache>> caches = new Dictionary<string, Mock<ICache>>();
        private readonly Dictionary<string, List<Mock>> xenObjects = new Dictionary<string, List<Mock>>();
        private readonly Dictionary<string, Mock<Proxy>> proxies = new Dictionary<string, Mock<Proxy>>();
        private readonly Dictionary<string, Mock<Session>> sessions = new Dictionary<string, Mock<Session>>();
        private readonly Mock<IXenAdminConfigProvider> config = new Mock<IXenAdminConfigProvider>();

        private const string defaultUserName = "Default User Name";
        private const string defaultPassword = "Default Password";

        public Mock<IXenAdminConfigProvider> MockConfigProvider{ get { return config; } }

        public List<IXenConnection> AllConnections
        {
            get { return connections.Values.ToList().ConvertAll(c => c.Object); }
        }

        public IXenConnection ConnectionFor(string connectionId)
        {
            return connections[connectionId].Object;
        }

        /// <summary>
        /// Get the mock form of a generated connection
        /// You can set your expectations for connection methods
        /// The basic ones are already provided
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Mock<IXenConnection> MockConnectionFor(string connectionId)
        {
            return connections[connectionId];
        }

        /// <summary>
        /// Get the mock for of the cache for a specified connection id
        /// The XenObject types are pre-populated if you use the NewXenObject<T> method
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Mock<ICache> MockCacheFor(string connectionId)
        {
            return caches[connectionId];
        }

        /// <summary>
        /// Provide access to the underlying proxy for the connection - this allows you to intercept static API calls
        /// So, if your class calls Pool.get_uuid, you need to mock setup the method pool_get_uuid
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Mock<Proxy> MockProxyFor(string connectionId)
        {
            return proxies[connectionId];
        }

        /// <summary>
        /// Provide access to a mock session for the connection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Mock<Session> MockSessionFor(string connectionId)
        {
            return sessions[connectionId];
        }

        public void ClearXenObjects(string connectionId)
        {
            xenObjects[connectionId].Clear();
        }

        public void CreateNewConnection(params string[] connectionId)
        {
            CreateNewConnection(defaultUserName, defaultPassword, connectionId);
        }

        public void CreateNewConnection(string username, string password, params string[] connectionId)
        {
            foreach (string db in connectionId)
            {
                CreateNewConnection(db, username, password);
            }
        }

        protected void CreateNewConnection(string connectionId, string username, string password)
        {
            XenAdminConfigManager.Provider = config.Object;
            connections.Add(connectionId, new Mock<IXenConnection>());
            xenObjects.Add(connectionId, new List<Mock>());
            caches.Add(connectionId, new Mock<ICache>());
            proxies.Add(connectionId, new Mock<Proxy>());
            sessions.Add(connectionId, new Mock<Session>(proxies[connectionId].Object, connections[connectionId].Object));

            connections[connectionId].Setup(c => c.Session).Returns(sessions[connectionId].Object);
            connections[connectionId].Setup(c => c.DuplicateSession()).Returns(sessions[connectionId].Object);
            connections[connectionId].Setup(c => c.Cache).Returns(caches[connectionId].Object);
            connections[connectionId].Setup(c => c.Name).Returns(connectionId);
            connections[connectionId].Setup(c => c.Username).Returns(username);
            connections[connectionId].Setup(c => c.Password).Returns(password);

            RefreshCache(connectionId);
        }

        /// <summary>
        /// Refresh the cache
        /// If a new XenObject type has been added to the API it'll need to be added here too
        /// </summary>
        /// <param name="connectionId"></param>
        public void RefreshCache(string connectionId)
        {
            caches[connectionId].Setup(c => c.Bonds).Returns(GeneratedXenObjects<Bond>(connectionId));
            caches[connectionId].Setup(c => c.Folders).Returns(GeneratedXenObjects<Folder>(connectionId));
            caches[connectionId].Setup(c => c.GPU_groups).Returns(GeneratedXenObjects<GPU_group>(connectionId));
            caches[connectionId].Setup(c => c.Host_cpus).Returns(GeneratedXenObjects<Host_cpu>(connectionId));
            caches[connectionId].Setup(c => c.Hosts).Returns(GeneratedXenObjects<Host>(connectionId));
            caches[connectionId].Setup(c => c.Messages).Returns(GeneratedXenObjects<Message>(connectionId));
            caches[connectionId].Setup(c => c.Networks).Returns(GeneratedXenObjects<Network>(connectionId));
            caches[connectionId].Setup(c => c.PBDs).Returns(GeneratedXenObjects<PBD>(connectionId));
            caches[connectionId].Setup(c => c.PGPUs).Returns(GeneratedXenObjects<PGPU>(connectionId));
            caches[connectionId].Setup(c => c.PIFs).Returns(GeneratedXenObjects<PIF>(connectionId));
            caches[connectionId].Setup(c => c.Pool_patches).Returns(GeneratedXenObjects<Pool_patch>(connectionId));
            caches[connectionId].Setup(c => c.Pools).Returns(GeneratedXenObjects<Pool>(connectionId));
            caches[connectionId].Setup(c => c.Roles).Returns(GeneratedXenObjects<Role>(connectionId));
            caches[connectionId].Setup(c => c.SMs).Returns(GeneratedXenObjects<SM>(connectionId));
            caches[connectionId].Setup(c => c.SRs).Returns(GeneratedXenObjects<SR>(connectionId));
            caches[connectionId].Setup(c => c.Subjects).Returns(GeneratedXenObjects<Subject>(connectionId));
            caches[connectionId].Setup(c => c.Tunnels).Returns(GeneratedXenObjects<Tunnel>(connectionId));
            caches[connectionId].Setup(c => c.VBDs).Returns(GeneratedXenObjects<VBD>(connectionId));
            caches[connectionId].Setup(c => c.VDIs).Returns(GeneratedXenObjects<VDI>(connectionId));
            caches[connectionId].Setup(c => c.VGPUs).Returns(GeneratedXenObjects<VGPU>(connectionId));
            caches[connectionId].Setup(c => c.VIFs).Returns(GeneratedXenObjects<VIF>(connectionId));
            caches[connectionId].Setup(c => c.VMPPs).Returns(GeneratedXenObjects<VMPP>(connectionId));
            caches[connectionId].Setup(c => c.VM_appliances).Returns(GeneratedXenObjects<VM_appliance>(connectionId));
            caches[connectionId].Setup(c => c.VMs).Returns(GeneratedXenObjects<VM>(connectionId));
        }

        private T[] GeneratedXenObjects<T>(string connectionId) where T : XenObject<T>
        {
            return GeneratedXenObjectsMocks<T>(connectionId).ConvertAll(m => m.Object).ToArray();
        }

        public List<Mock<T>> GeneratedXenObjectsMocks<T>(string connectionId) where T : XenObject<T>
        {
            List<Mock<T>> mockObjects = new List<Mock<T>>();
            foreach (Mock mockedObject in xenObjects[connectionId])
            {
                if (typeof(T) == mockedObject.Object.GetType().BaseType)
                    mockObjects.Add(mockedObject as Mock<T>);
            }
            return mockObjects;
        }

        /// <summary>
        /// Generate a mock object for the connection with specified Id
        /// The mock's underlying object will become accessible via. a call to the cache as well as the mock.Object 
        /// The cache is refreshed automatically
        /// 
        /// The method *must* be virtual to be mocked. You may need to add the virtual keyword to your method
        /// </summary>
        /// <example><![CDATA[
        /// 
        /// Mock<SR> sr = ObjectManager.NewXenObject<SR>("id");
        /// sr.Setup(s => s.HBALunPerVDI).Returns(true);
        /// SR[] srs = ObjectManager.ConnectionFor("id").Cache.SRs;
        /// Assert.IsTrue(srs[0].HBALunPerVDI);
        /// 
        /// ]]>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionId"></param>
        /// <param name="refreshCache">automatically refresh the cache</param>
        /// <returns></returns>
        /// 
        public Mock<T> NewXenObject<T>(string connectionId, bool refreshCache) where T : XenObject<T>
        {
            Mock<T> mock = new Mock<T>();

            if(!connections.ContainsKey(connectionId))
                throw new KeyNotFoundException("Connection id missing: " + connectionId);

            mock.Object.Connection = connections[connectionId].Object;
            mock.Object.opaque_ref = string.Format("OpaqueRef:{0}", DateTime.UtcNow.Ticks);
            xenObjects[connectionId].Add(mock);
            if(refreshCache)
                RefreshCache(connectionId);
            return mock;
        }

        /// <summary>
        /// Create XenObject and auto-refresh the cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Mock<T> NewXenObject<T>(string connectionId) where T : XenObject<T>
        {
            return NewXenObject<T>(connectionId, true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void VerifyAllMocks(string connectionId)
        {
            connections[connectionId].VerifyAll();
            caches[connectionId].VerifyAll();
            proxies[connectionId].VerifyAll();
            sessions[connectionId].VerifyAll();
            xenObjects[connectionId].ForEach(m=>m.VerifyAll());
            config.VerifyAll();
        }

        private bool disposed;
        protected void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(!disposing)
                {
                    connections.Clear();
                    caches.Clear();
                    xenObjects.Clear();
                    proxies.Clear();
                    sessions.Clear();
                    XenAdminConfigManager.Provider = new WinformsXenAdminConfigProvider(); //Bit hacky but stops subsequent UI tests from failing
                }
                disposed = true;
            }
        }
    }
}
