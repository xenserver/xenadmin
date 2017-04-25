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
using XenAdmin.Commands;
using XenAdmin.Core;
using System.IO;
using System.Text.RegularExpressions;
using XenAPI;
using XenAdmin;
using NUnit.Framework;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BackupHostCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public BackupHostCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            BackupHostCommandTest tester = new BackupHostCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BackupHostCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public BackupHostCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            BackupHostCommandTest tester = new BackupHostCommandTest();
            tester.Test();
        }
    }

    public class BackupHostCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new BackupHostCommand();
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest())
            {
                Assert.AreEqual(1, selection.Count, "This command only supports single select.");
                Host host = (Host)selection[0].XenObject;

                string tempFile = Path.GetTempFileName();
                try
                {
                    MW(new BackupHostCommand(Program.MainWindow, host, tempFile).Execute);

                    Func<bool> finished = delegate
                    {
                        if (File.Exists(tempFile))
                        {
                            string data = File.ReadAllText(tempFile);
                            return data != null && Regex.IsMatch(data, @"GET /host_backup\?task_id=task\d+&session_id=dummy");
                        }
                        return false;
                    };

                    // wait until command finished.
                    MWWaitFor(finished, "BackupHostCommandTest.Test() didn't finish.");
                }
                finally
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
            }
        }
    }
}
