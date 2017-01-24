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
using NUnit.Framework;

using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using XenAdmin;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Model;


namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class EditTagsCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public EditTagsCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        private EditTagsCommandTest tester = new EditTagsCommandTest();

        [Test]
        public void TestMultipleSelectAddTag()
        {
            tester.TestMultipleSelectAddTag();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class EditTagsCommandRbacTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public EditTagsCommandRbacTestMidnightRide()
            : base(true, CommandTestsDatabase.MidnightRide)
        { }

        private EditTagsCommandTest tester = new EditTagsCommandTest();

        [Test]
        public void TestMultipleSelectRbacMidnightRide()
        {
            tester.TestMultipleSelectRbacMidnightRide();
        }
    }

    public class EditTagsCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new EditTagsCommand();
        }

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Tags; }
        }

        public void TestMultipleSelectAddTag()
        {
            foreach (SelectedItemCollection selection in RunTest(GetMultipleSelections()))
            {
                string newTag = Guid.NewGuid().ToString();

                HandleModalDialog<NewTagDialogWrapper>("Edit Tags", Command.Execute, d =>
                {
                    d.TextBox.Text = newTag;
                    d.AddButton.PerformClick();
                    d.OkButton.PerformClick();
                });

                MWWaitFor(() => selection.AsXenObjects<VM>().TrueForAll(vm => Tags.GetTagList(vm).Contains(newTag)), "tag not found.");
            }
        }

        public void TestMultipleSelectRbacMidnightRide()
        {
            foreach (SelectedItemCollection selection in RunTest(GetMultipleSelections()))
            {
                HandleModalDialog<NewTagDialogWrapper>("Edit Tags", Command.Execute, d =>
                {
                    // switch checkboxes for all tags
                    Util.PopulateList<ListViewItem>(d.TagsListView.Items).ForEach(i => i.Checked = !i.Checked);
                    d.OkButton.PerformClick();
                });

                // sudo dialog should now appear.
                HandleModalDialog<RoleElevationDialogWrapper>(Messages.XENCENTER, () => { }, d => d.ButtonCancel.PerformClick());

                // check there are no extra sudo dialogs (this was a bug once.)
                MWWaitFor(() => null == Win32Window.GetWindowWithText(Messages.XENCENTER, w => Control.FromHandle(w.Handle) is RoleElevationDialog), "Extra sudo dialog(s) found.");
            }
        }

        private IEnumerable<SelectedItemCollection> GetMultipleSelections()
        {
            yield return new SelectedItemCollection(GetAllXenObjects<VM>(v => v.is_a_real_vm).ConvertAll(v => new SelectedItem(v)));
        }
    }
}
