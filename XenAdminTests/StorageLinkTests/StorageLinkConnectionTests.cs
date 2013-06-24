/* Copyright (c) Citrix Systems Inc. 
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
using System.ComponentModel;
using System.Net;
using System.Threading;
using NUnit.Framework;
using XenAdmin.Network.StorageLink;
using XenAdmin.Network.StorageLink.Service;

namespace XenAdminTests.StorageLinkTests
{
    [TestFixture, NUnit.Framework.Category(TestCategories.UICategoryB)]
    public class StorageLinkConnectionTests : MainWindowTester
    {
        private MockStorageLinkWebService _ws;
        private StorageLinkConnection _con;

        [SetUp]
        public void SetUp()
        {
            _ws = new MockStorageLinkWebService("Mock", "bla", "bla");
            _con = new StorageLinkConnection(null, _ws);
        }

        private void Connect()
        {
            Assert.AreEqual(_con.ConnectionState, StorageLinkConnectionState.Disconnected);
            Assert.IsFalse(_con.IsConnectionEnding);

            _con.BeginConnect();

            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Connected, "Connection didn't connect.");
            Assert.IsFalse(_con.IsConnectionEnding);
        }

        private void EndConnect()
        {
            Assert.AreEqual(_con.ConnectionState, StorageLinkConnectionState.Connected);
            Assert.IsFalse(_con.IsConnectionEnding);

            _con.EndConnect();
            Assert.IsTrue(_con.IsConnectionEnding);
            WaitFor(() => _ws.IsDisposed, "WebService not disposed.");
            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Disconnected, "Connection not disconnected");
        }

        [TearDown]
        public void TearDown()
        {
            if (_con.ConnectionState != StorageLinkConnectionState.Disconnected)
            {
                EndConnect();
            }
        }

        [Test]
        public void TestStorageSystemExistsOnConnect()
        {
            storageSystemInfo ss = _ws.GetSampleStorageSystem();
            storagePoolInfo sp = _ws.GetSampleStoragePool();
            storageVolumeInfo sv = _ws.GetSampleStorageVolume();

            _ws.StorageSystemsAndPools.Add(ss, new[] { sp });
            _ws.StoragePoolsAndVolumes.Add(sp, new[] { sv });

            Connect();

            WaitFor(() => _con.Cache.StorageSystems.Count == 1, "Storage System didn't get added.");

            Assert.AreEqual(_con.Cache.StorageSystems[0].opaque_ref, ss.objectId);
            Assert.AreEqual(_con.Cache.StorageSystems[0].SerialNumber, ss.serialNum);
            Assert.AreEqual(_con.Cache.StorageSystems[0].Model, ss.model);
            Assert.AreEqual(_con.Cache.StorageSystems[0].FullName, ss.displayName);
            Assert.AreEqual((uint)_con.Cache.StorageSystems[0].Capabilities, ss.capabilities);
            Assert.AreEqual(_con.Cache.StorageSystems[0].FriendlyName, ss.friendlyName);

            Assert.AreEqual(_con.Cache.StoragePools[0].opaque_ref, sp.objectId);

            Assert.AreEqual(_con.Cache.StorageVolumes[0].opaque_ref, sv.objectId);
        }

        [Test]
        public void TestExceptionOnStorageSystemEnumeration()
        {
            const string message = "test error";
            _ws.StorageSystemsEnumerating += (s, e) => { throw new WebException(message); };

            _con.BeginConnect();

            WaitFor(() => _con.Error == message, "Error message wasn't shown");
            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Disconnected, "Connection not shown as disconnected on exception");

            _con.EndConnect();

            Assert.IsTrue(_con.IsConnectionEnding);
            WaitFor(() => _ws.IsDisposed, "WebService not disposed.");
            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Disconnected, "Connection not disconnected");
        }

        [Test]
        public void TestTemporaryExceptionOnStorageSystemEnumeration()
        {
            const string message = "test error";
            EventHandler exceptionThrower = (s, e) => { throw new WebException(message); };
            _ws.StorageSystemsEnumerating += exceptionThrower;

            _con.BeginConnect();

            WaitFor(() => _con.Error == message, "Error message wasn't shown");
            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Disconnected, "Connection not shown as disconnected on exception");

            _ws.StorageSystemsEnumerating -= exceptionThrower;

            // now check error has gone away and we connect
            WaitFor(() => _con.Error == "", "Error message didn't go away.");
            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Connected, "Didn't connect after exception throwing stopped.");

            EndConnect();
        }

        [Test]
        public void TestCacheWasRefreshedAfterTemporaryExceptionOnStorageSystemEnumeration()
        {
            const string message = "test error";
            EventHandler exceptionThrower = (s, e) => { throw new WebException(message); };
            _ws.StorageSystemsEnumerating += exceptionThrower;

            _con.BeginConnect();

            WaitFor(() => _con.Error == message, "Error message wasn't shown");
            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Disconnected, "Connection not shown as disconnected on exception");

            // add new storage-system
            storageSystemInfo ss = _ws.GetSampleStorageSystem();
            _ws.StorageSystemsAndPools.Add(ss, new storagePoolInfo[0]);

            // stop throwing the exception.
            _ws.StorageSystemsEnumerating -= exceptionThrower;

            // now check error has gone away and we connect
            WaitFor(() => _con.Error == "", "Error message didn't go away.");
            WaitFor(() => _con.ConnectionState == StorageLinkConnectionState.Connected, "Didn't connect after exception throwing stopped.");

            // now check cache refreshed
            WaitFor(() => _con.Cache.StorageSystems.Count == 1, "Storage System didn't get added.");

            Assert.AreEqual(_con.Cache.StorageSystems[0].opaque_ref, ss.objectId);
            Assert.AreEqual(_con.Cache.StorageSystems[0].SerialNumber, ss.serialNum);
            Assert.AreEqual(_con.Cache.StorageSystems[0].Model, ss.model);
            Assert.AreEqual(_con.Cache.StorageSystems[0].FullName, ss.displayName);
            Assert.AreEqual((uint)_con.Cache.StorageSystems[0].Capabilities, ss.capabilities);
            Assert.AreEqual(_con.Cache.StorageSystems[0].FriendlyName, ss.friendlyName);

            EndConnect();
        }

        [Test]
        public void TestStorageSystemEvent()
        {
            Connect();

            storageSystemInfo ss = _ws.GetSampleStorageSystem();
            _ws.StorageSystemsAndPools.Add(ss, new storagePoolInfo[0]);

            Thread.Sleep(2000);

            // check cache isn't updated. It requires an event to be updated.
            Assert.AreEqual(0, _con.Cache.StorageSystems.Count);

            // add event to trigger storage-system refresh
            _ws.Events.Add(new @event() { eventId = "event.object.storage-system.add.{bla}.{bla}" });

            WaitFor(() => _con.Cache.StorageSystems.Count == 1);

            Assert.AreEqual(_con.Cache.StorageSystems[0].opaque_ref, ss.objectId);
            Assert.AreEqual(_con.Cache.StorageSystems[0].SerialNumber, ss.serialNum);
            Assert.AreEqual(_con.Cache.StorageSystems[0].Model, ss.model);
            Assert.AreEqual(_con.Cache.StorageSystems[0].FullName, ss.displayName);
            Assert.AreEqual((uint)_con.Cache.StorageSystems[0].Capabilities, ss.capabilities);
            Assert.AreEqual(_con.Cache.StorageSystems[0].FriendlyName, ss.friendlyName);

            EndConnect();
        }
    }
}
