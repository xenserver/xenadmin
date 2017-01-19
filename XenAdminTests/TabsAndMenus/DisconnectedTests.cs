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
using NUnit.Framework;
using XenAdmin.Commands;
using XenAdmin.Network;
using XenAdmin;
using System.Threading;

namespace XenAdminTests.TabsAndMenus
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class DisconnectedTests : TabsAndMenus
    {
        public DisconnectedTests()
            : base("state4.xml")
        {
        }

        [Test]
        public void ToolBar_AfterPoolDisconnection()
        {
            // disconnect from the pool and check that the toolbar buttons have be disabled.

            // select pool
            SelectInTree(GetAnyPool());

            IXenConnection connection = GetAnyPool().Connection;

            // now disconnect
            MW<bool>(new DisconnectCommand(Program.MainWindow, connection, false).Execute);

            // wait until the New Storage button is disabled.
            MWWaitFor(() => !MainWindowWrapper.MainToolStrip.Items[6].Enabled);

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

            // check toolbar buttons (while disconnected)

            VerifyToolbar(buttons);
        }
    }
}
