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

using System.Threading;
using NUnit.Framework;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdminTests.XenModelTests.ActionTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class NetworkingTests : DatabaseTester_TestFixture
    {
        private AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
        private int networks;
        private Network newNetwork;
        private const string dbName = "TampaTwoHostPoolSelectioniSCSI.xml";

        public NetworkingTests() : base(dbName){}

        [SetUp]
        public void TestSetup()
        {
            networks = DatabaseManager.ConnectionFor(dbName).Cache.Networks.Length;
            newNetwork = GetNetwork();
        }


        private Network GetNetwork()
        {
            Network network = new Network();
            network.name_label = System.Guid.NewGuid().ToString();
            network.name_description = "test descript";
            return network;
        }

        [Test]
        public void CreateNetwork()
        {
            NetworkAction networkAction = new NetworkAction(DatabaseManager.ConnectionFor(dbName), newNetwork, true);
            networkAction.Completed += networkAction_Completed;
            networkAction.RunAsync();
            AutoResetEvent.WaitOne();
            DatabaseManager.ConnectionFor(dbName).LoadCache(DatabaseManager.ConnectionFor(dbName).Session);
            Assert.AreEqual(networks + 1, DatabaseManager.ConnectionFor(dbName).Cache.Networks.Length);
        }



        [Test]
        public void CreateExternalNetwork()
        {
            Assert.True(DatabaseManager.ConnectionFor(dbName).Cache.PIFs.Length > 0);
            PIF pif = DatabaseManager.ConnectionFor(dbName).Cache.PIFs[0];
            NetworkAction networkAction = new NetworkAction(DatabaseManager.ConnectionFor(dbName), newNetwork, pif, 2);
            networkAction.Completed += networkAction_Completed;
            networkAction.RunAsync();
            AutoResetEvent.WaitOne();
            DatabaseManager.RefreshCacheFor(dbName);
            Assert.AreEqual(networks + 1, DatabaseManager.ConnectionFor(dbName).Cache.Networks.Length);
        }

        [Test]
        public void DeleteNetwork()
        {
            CreateNetwork();
            Network network = FindCreatedNetwork();
            Assert.NotNull(network);
            NetworkAction networkAction = new NetworkAction(DatabaseManager.ConnectionFor(dbName), network, false);
            networkAction.Completed += networkAction_Completed;
            networkAction.RunAsync();
            AutoResetEvent.WaitOne();
            DatabaseManager.RefreshCacheFor(dbName);
            Assert.AreEqual(networks, DatabaseManager.ConnectionFor(dbName).Cache.Networks.Length);
        }

        private Network FindCreatedNetwork()
        {
            foreach (Network network in DatabaseManager.ConnectionFor(dbName).Cache.Networks)
            {
                if (network.name_label == newNetwork.name_label)
                {
                    return network;
                }
            }
            return null;
        }

        void networkAction_Completed(ActionBase sender)
        {
            AutoResetEvent.Set();
        }
    }



}
