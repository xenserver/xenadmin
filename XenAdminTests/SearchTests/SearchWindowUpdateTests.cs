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

using NUnit.Framework;
using XenAdmin;
using XenAdmin.Network;
using XenAdmin.ServerDBs;
using XenAdmin.XenSearch;

namespace XenAdminTests.SearchTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class SearchWindowUpdateTests : MainWindowTester
    {
        [Test]
        public void TestSearchPanelUpdatesWhenTagsChange()
        {
            PerformSearch("Resources by Tag");

            MWWaitFor(() => !SearchWindow.GetVisibleResults().Contains("Tags: hello"));
            MWWaitFor(() => !SearchWindow.GetVisibleResults().Contains("Tags: there"));

            SetChesterTags(new[] { "hello", "there" });

            MWWaitFor(() => SearchWindow.GetVisibleResults().Contains("Tags: hello"));

            Assert.IsTrue(SearchWindow.GetVisibleResults().Contains("Tags: hello"), "The search window wasn't updated.");
            Assert.IsTrue(SearchWindow.GetVisibleResults().Contains("Tags: there"), "The search window wasn't updated.");
        }

        [Test]
        public void TestSearchPanelUpdatesWhenPowerStateOfVMsChange()
        {
            PerformSearch("VMs by Power State");

            // there are some running and some halted at the start
            MWWaitFor(() => SearchWindow.GetVisibleResults().Contains("Power state: Running"));
            MWWaitFor(() => SearchWindow.GetVisibleResults().Contains("Power state: Halted"));

            SetPowerStateOfAllVMs("Halted");

            MWWaitFor(() => !SearchWindow.GetVisibleResults().Contains("Power state: Running"), "The search window wasn't updated. There should be no running VMs shown.");
            MWWaitFor(() => SearchWindow.GetVisibleResults().Contains("Power state: Halted"), "The search window wasn't updated. All VMs should halted.");
        }


        private void SetChesterTags(string[] tags)
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnections)
            {
                if (connection.Hostname.IndexOf("state3") > 0)
                {
                    const string chesterHostRef = "OpaqueRef:d3a48ddc-8261-33df-64b2-1309e98b395d";
                    DbProxy dbProxy = DbProxy.proxys[connection];
                    dbProxy.db.Tables["host"].Rows[chesterHostRef].Props["tags"].XapiObjectValue = tags;
                    dbProxy.SendModObject("host", chesterHostRef);
                    return;
                }
            }
            Assert.Fail("Couldn't set tags in DB");
        }

        private void SetPowerStateOfAllVMs(string power_state)
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnections)
            {
                DbProxy dbProxy = DbProxy.proxys[connection];

                foreach (string opaqueRef in dbProxy.db.Tables["VM"].Rows.Keys)
                {
                    dbProxy.db.Tables["VM"].Rows[opaqueRef].Props["power_state"].XapiObjectValue = power_state;
                    dbProxy.SendModObject("VM", opaqueRef);
                }
            }
        }

        private void PerformSearch(string name)
        {
            foreach (Search search in Search.Searches)
            {
                if (search.ToString() == name)
                {
                    MW(() => Program.MainWindow.DoSearch(search));
                    return;
                }
            }
            Assert.Fail("Couldn't find " + name + " search.");
        }
    }
}
