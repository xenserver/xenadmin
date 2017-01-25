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

using System.Linq;

using NUnit.Framework;

using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using XenAdmin;
using XenAdmin.Controls;
using System.Collections.Generic;
using XenAdmin.Model;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class ToolbarGeorge : TabsAndMenus
    {
        public ToolbarGeorge()
            : base("state1.xml")
        {
        }

        protected ToolbarGeorge(string db)
            : base(db)
        {
        }

        [Test]
        public void ToolBar_XenCenterNode()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", false  ),
	            new ExpectedTextMenuItem("New VM", false  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", false  ),
	            new ExpectedTextMenuItem("Reboot", false  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            VerifyToolbar(null, buttons);
        }

        [Test]
        public void ToolBar_Pool()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", false  ),
	            new ExpectedTextMenuItem("Reboot", false  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };


            VerifyToolbar(GetAnyPool(), buttons);
        }

        [Test]
        public void ToolBar_Host()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", true  ),
	            new ExpectedTextMenuItem("Reboot", true  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            foreach (Host host in GetAllXenObjects<Host>())
            {
                VerifyToolbar(host, buttons);
            }
        }

        [Test]
        public void ToolBar_DefaultTemplate()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem);

            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", false  ),
	            new ExpectedTextMenuItem("Reboot", false  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            VerifyToolbar(GetAnyDefaultTemplate(), buttons);
        }

        [Test]
        public void ToolBar_UserTemplate()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem);

            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", false  ),
	            new ExpectedTextMenuItem("Reboot", false  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_template && !v.DefaultTemplate && !v.is_a_snapshot))
            {
                VerifyToolbar(vm, buttons);
            }
        }

        [Test]
        public void ToolBar_SR()
        {
            EnsureChecked(MainWindowWrapper.ViewMenuItems.LocalStorageToolStripMenuItem);

            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", false  ),
	            new ExpectedTextMenuItem("Reboot", false  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            foreach (SR sr in GetAllXenObjects<SR>(s => !s.IsToolsSR))
            {
                VerifyToolbar(sr, buttons);
            }
        }

        [Test]
        public void ToolBar_Snapshot()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Add New Server", true  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("New Pool", true  ),
	                new ExpectedTextMenuItem("New Storage", false  ),
	                new ExpectedTextMenuItem("New VM", false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Shut Down", false  ),
	                new ExpectedTextMenuItem("Reboot", false  ),
	                new ExpectedTextMenuItem("Suspend", false  ),
                };

                foreach (VM snapshot in GetAllXenObjects<VM>(v => v.is_a_snapshot))
                {
                    VerifyToolbar(snapshot, buttons);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ToolBar_VDI()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Add New Server", true  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("New Pool", true  ),
	                new ExpectedTextMenuItem("New Storage", false  ),
	                new ExpectedTextMenuItem("New VM", false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Shut Down", false  ),
	                new ExpectedTextMenuItem("Reboot", false  ),
	                new ExpectedTextMenuItem("Suspend", false  ),
                };

                VerifyToolbar(GetAnyVDI(v => v.name_label != "base copy"), buttons);
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ToolBar_Network()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Add New Server", true  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("New Pool", true  ),
	                new ExpectedTextMenuItem("New Storage", false  ),
	                new ExpectedTextMenuItem("New VM", false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Shut Down", false  ),
	                new ExpectedTextMenuItem("Reboot", false  ),
	                new ExpectedTextMenuItem("Suspend", false  ),
                };

                foreach (XenAPI.Network network in GetAllXenObjects<XenAPI.Network>(n => n.name_label != "Guest installer network"))
                {
                    VerifyToolbar(network, buttons);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ToolBar_GroupingTag()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Objects);
            try
            {
                ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Add New Server", true  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("New Pool", true  ),
	                new ExpectedTextMenuItem("New Storage", false  ),
	                new ExpectedTextMenuItem("New VM", false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Shut Down", false  ),
	                new ExpectedTextMenuItem("Reboot", false  ),
	                new ExpectedTextMenuItem("Suspend", false  ),
                };

                foreach (GroupingTag gt in GetAllTreeNodes().FindAll(v => v.Tag is GroupingTag).ConvertAll(v => (GroupingTag)v.Tag))
                {
                    VerifyToolbar(gt, buttons);
                }
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ToolBar_Folder()
        {
            PutInNavigationMode(NavigationPane.NavigationMode.Folders);
            try
            {
                ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Add New Server", true  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("New Pool", true  ),
	                new ExpectedTextMenuItem("New Storage", false  ),
	                new ExpectedTextMenuItem("New VM", false  ),
	                new ExpectedSeparator(),
	                new ExpectedTextMenuItem("Shut Down", false  ),
	                new ExpectedTextMenuItem("Reboot", false  ),
	                new ExpectedTextMenuItem("Suspend", false  ),
                };

                var folders = GetAllXenObjects<Folder>().Where(f => !(string.IsNullOrEmpty(f.ToString())));
                foreach (Folder folder in folders)
                    VerifyToolbar(folder, buttons);
            }
            finally
            {
                PutInNavigationMode(NavigationPane.NavigationMode.Infrastructure);
            }
        }

        [Test]
        public void ToolBar_Master()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", true  ),
	            new ExpectedTextMenuItem("Reboot", true  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            VerifyToolbar(GetAnyHost(IsMaster), buttons);
        }

        [Test]
        public void ToolBar_Slave()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", true  ),
	            new ExpectedTextMenuItem("Reboot", true  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            VerifyToolbar(GetAnyHost(IsSlave), buttons);
        }

        [Test]
        public void ToolBar_VMWithTools()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", true  ),
	            new ExpectedTextMenuItem("Reboot", true  ),
	            new ExpectedTextMenuItem("Suspend", true  ),
            };

            foreach (VM vm in GetAllXenObjects<VM>(HasTools))
            {
                VerifyToolbar(vm, buttons);
            }
        }

        [Test]
        public void ToolBar_VMWithoutTools()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Shut Down", false  ),
	            new ExpectedTextMenuItem("Reboot", false  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
                new ExpectedTextMenuItem("Force Shut Down", true  ),
                new ExpectedTextMenuItem("Force Reboot", true  ),
            };

            foreach (VM vm in GetAllXenObjects<VM>(NoTools))
            {
                VerifyToolbar(vm, buttons);
            }
        }

        [Test]
        public void ToolBar_VMShutdown()
        {
            ExpectedMenuItem[] buttons = new ExpectedMenuItem[]{
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Add New Server", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("New Pool", true  ),
	            new ExpectedTextMenuItem("New Storage", true  ),
	            new ExpectedTextMenuItem("New VM", true  ),
	            new ExpectedSeparator(),
	            new ExpectedTextMenuItem("Start", true  ),
	            new ExpectedTextMenuItem("Reboot", false  ),
	            new ExpectedTextMenuItem("Suspend", false  ),
            };

            foreach (VM vm in GetAllXenObjects<VM>(IsShutdown))
            {
                VerifyToolbar(vm, buttons);
            }
        }
    }
}
