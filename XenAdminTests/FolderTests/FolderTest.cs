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
using System.Text;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Actions;
using XenAdmin.Actions.GUIActions;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.XenSearch;
using XenAdminTests.SearchTests;
using XenAPI;

namespace XenAdminTests.FolderTests
{
    public abstract class FolderTest : MainWindowLauncher_TestFixture
    {
        public FolderTest() : base("state4.xml") { }
        
        [Test]
        public void RunTests()
        {
            VerifyResult(false);  // test the test framework and the database by verifying the "before" state

            DoAction();

            WaitForActions();

            VerifyResult(true);
        }

        private void VerifyResult(bool after)
        {
            Search search = EverythingInFolders();
            XmlNode expectedResults = GetExpectedResults(after);
            MW(() => ComparerAdapter.CompareResults(search, expectedResults));
        }

        protected void WaitForActions()
        {
            // Wait up to 30 seconds to finish all the actions
            for (int i = 0; i < 60; ++i)
            {
                Thread.Sleep(500);
                if (!HasOutstandingActions())
                    break;
            }
        }

        private Search EverythingInFolders()
        {
            QueryScope scope = new QueryScope(ObjectTypes.AllIncFolders);
            QueryFilter filter = new NullQuery<Folder>(PropertyNames.folder, false);
            Query q = new Query(scope, filter);
            Grouping grouping = new FolderGrouping((Grouping)null);
            return new Search(q, grouping, false, "", null, false);
        }

        private bool HasOutstandingActions()
        {
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (a is MeddlingAction || a.IsCompleted)
                    continue;

                return true;
            }
            return false;
        }

        protected XmlDocument doc;
        protected XmlElement xmlSearch, xmlFolderHR, xmlFolderIT, xmlIncubus, xmlHottub;

        private XmlNode GetExpectedResults(bool after)
        {
            // These are the folders for the unmodified database
            doc = new XmlDocument();
            xmlSearch = doc.CreateElement("search");
            doc.AppendChild(xmlSearch);

            xmlFolderHR = CreateNode("/HR");
            xmlSearch.AppendChild(xmlFolderHR);
            xmlIncubus = CreateNode("OpaqueRef:7166136b-f599-3e3c-4515-40dae8cbbead");
            xmlFolderHR.AppendChild(xmlIncubus);

            xmlFolderIT = CreateNode("/IT Dept");
            xmlSearch.AppendChild(xmlFolderIT);
            xmlHottub = CreateNode("OpaqueRef:ec64ce83-5185-193c-83a5-53fac244fab4");
            xmlFolderIT.AppendChild(xmlHottub);

            // The derived class then modifies them through this virtual function
            if (after)
                PrepareResults();

            return xmlSearch;
        }

        protected XmlElement CreateNode(string opaqueref)
        {
            XmlElement element = doc.CreateElement("IXenObject");
            element.SetAttribute("opaqueref", opaqueref);
            return element;

        }

        // Overridden by the derived class to do some sort of poking around in the folders
        protected abstract void DoAction();

        // Overridden by the derived class to alter the expected search results to correspond to the above poking
        protected abstract void PrepareResults();
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_DoNothing : FolderTest
    {
        protected override void DoAction()
        {
        }

        protected override void PrepareResults()
        {
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class FolderTest_Create : FolderTest
    {
        protected override void DoAction()
        {
            IXenConnection connection = GetAnyConnection();  // we only have one
            Folders.Create(connection, "/zzz", "/aaa/yyy", "/HR/newHR");
        }

        protected override void PrepareResults()
        {
            XmlElement zzz = CreateNode("/zzz");
            xmlSearch.AppendChild(zzz);

            XmlElement aaa = CreateNode("/aaa");
            xmlSearch.PrependChild(aaa);
            XmlElement yyy = CreateNode("/aaa/yyy");
            aaa.AppendChild(yyy);

            XmlElement newHR = CreateNode("/HR/newHR");
            xmlFolderHR.PrependChild(newHR);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_Delete : FolderTest
    {
        protected override void DoAction()
        {
            Folder folder = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "HR";
            });
            Folders.Unfolder(folder);
        }

        protected override void PrepareResults()
        {
            xmlSearch.RemoveChild(xmlFolderHR);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_DeleteSubFolder : FolderTest
    {
        protected override void DoAction()
        {
            IXenConnection connection = GetAnyConnection();
            Folders.Create(connection, "/HR/bbb/ccc/ddd");
            WaitForActions();

            Folder folder = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "ccc";
            });
            Folders.Unfolder(folder);
        }

        protected override void PrepareResults()
        {
            XmlElement bbb = CreateNode("/HR/bbb");
            xmlFolderHR.PrependChild(bbb);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_Rename : FolderTest
    {
        protected override void DoAction()
        {
            IXenConnection connection = GetAnyConnection();
            Folders.Create(connection, "/HR/bbb");
            WaitForActions();

            Folder folder = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "HR";
            });
            Folders.Rename(folder, "Human Resources");
        }

        protected override void PrepareResults()
        {
            xmlFolderHR.SetAttribute("opaqueref", "/Human Resources");
            XmlElement bbb = CreateNode("/Human Resources/bbb");
            xmlFolderHR.PrependChild(bbb);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_Unfolder : FolderTest
    {
        protected override void DoAction()
        {
            Folders.Unfolder(GetAnyPool());  // we only have one pool
        }

        protected override void PrepareResults()
        {
            xmlFolderIT.RemoveChild(xmlHottub);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_Insert : FolderTest
    {
        protected override void DoAction()
        {
            VM vm = GetAnyVM(delegate(VM v)
            {
                return v.name_label == "Iscsi Box";
            });

            Folder folder = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "IT Dept";
            });

            Folders.Move(vm, folder);
        }

        protected override void PrepareResults()
        {
            XmlElement vm = CreateNode("OpaqueRef:eef198a8-a2df-4141-2869-3234a029a4f5");
            xmlFolderIT.AppendChild(vm);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_MoveFolder : FolderTest
    {
        protected override void DoAction()
        {
            Folder folderHR = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "HR";
            });

            Folder folderIT = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "IT Dept";
            });

            Folders.Move(folderHR, folderIT);
        }

        protected override void PrepareResults()
        {
            xmlFolderHR.SetAttribute("opaqueref", "/IT Dept/HR");
            xmlFolderIT.PrependChild(xmlFolderHR);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderTest_CantMoveFolderIntoChild : FolderTest
    {
        protected override void DoAction()
        {
            IXenConnection connection = GetAnyConnection();
            Folders.Create(connection, "/HR/bbb/ccc/ddd");
            WaitForActions();

            Folder folderHR = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "HR";
            });

            Folder folderD = GetAnyFolder(delegate(Folder f)
            {
                return f.name_label == "ddd";
            });

            Folders.Move(folderHR, folderD);
        }

        protected override void PrepareResults()
        {
            XmlElement bbb = CreateNode("/HR/bbb");
            xmlFolderHR.PrependChild(bbb);
            XmlElement ccc = CreateNode("/HR/bbb/ccc");
            bbb.AppendChild(ccc);
            XmlElement ddd = CreateNode("/HR/bbb/ccc/ddd");
            ccc.AppendChild(ddd);
        }
    }
}
