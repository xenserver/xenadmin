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

using System.Text.RegularExpressions;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Dialogs.Wlb;
using XenAdmin.Wlb;

namespace XenAdminTests.DialogTests.boston.WlbEditScheduledTaskTests
{
    public struct WlbEditScheduledTaskTestsData
    {
        public string ExpectedDay;
        public bool CheckBoxInitialState;
    }


    [TestFixture, Category(TestCategories.UICategoryA)]
    public class WlbEditScheduledTaskTestsTaskConstructor : WlbEditScheduledTaskTest
    {
        public WlbEditScheduledTaskTestsTaskConstructor()
        {
            WlbEditScheduledTaskTestsData data = new WlbEditScheduledTaskTestsData()
                                                     {
                                                         ExpectedDay = "Wednesday",
                                                         CheckBoxInitialState = false
                                                     };
            ExpectedData = data;
        }

        public override WlbEditScheduledTask ConstructBaseDialog()
        {
            WlbScheduledTask task = new WlbScheduledTask("12");
            task.AddTaskParameter("OptMode", "0");
            task.DaysOfWeek = WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday;
            return new WlbEditScheduledTask(task);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class WlbEditScheduledTaskTestsIdConstructor : WlbEditScheduledTaskTest
    {
        public WlbEditScheduledTaskTestsIdConstructor()
        {
            WlbEditScheduledTaskTestsData data = new WlbEditScheduledTaskTestsData()
            {
                ExpectedDay = "Every Day",
                CheckBoxInitialState = true
            };
            ExpectedData = data;
        }

        public override WlbEditScheduledTask ConstructBaseDialog()
        {
            return new WlbEditScheduledTask(1, WlbScheduledTask.WlbTaskActionType.SetOptimizationMode);
        }
    }



    public abstract class WlbEditScheduledTaskTest : DialogTest<WlbEditScheduledTask>
    {
        private WlbEditScheduledTaskTestsData data;

        public WlbEditScheduledTaskTestsData ExpectedData
        {
            set { data = value; }
        }

        protected override WlbEditScheduledTask NewDialog()
        {
            return ConstructBaseDialog();
        }

        public abstract WlbEditScheduledTask ConstructBaseDialog();

        protected override void RunAfter()
        {
            VerifyOptModeComboBox();
            VerifyDaysOfTheWeekComboBox();
            FindTheSelectedDay();
            VerifyTimeComboBox();
            VerifyEnableTaskCheckBox();
            VerifyOKCancelButtonState();
        }

        private void VerifyOKCancelButtonState()
        {
            Assert.IsTrue(TestUtils.GetButton(dialog, "buttonOK").Enabled);
            Assert.IsTrue(TestUtils.GetButton(dialog, "button1").Enabled);
        }

        private void VerifyEnableTaskCheckBox()
        {
            CheckBox cb = TestUtils.GetCheckBox(dialog, "checkBoxEnable");
            Assert.AreEqual(data.CheckBoxInitialState, cb.Checked, "check box initial state");
            MW(delegate { cb.Checked = !data.CheckBoxInitialState; });
            Assert.AreEqual(!data.CheckBoxInitialState, cb.Checked, "check box after set");
        }

        private void VerifyTimeComboBox()
        {
            ComboBox mode = TestUtils.GetComboBox(dialog, "comboBoxHour");
            Regex timestampRegex = new Regex(@"[0-9]{1,2}[\:][0]{2}[ ][A|P][M]");
            SelfTestRegex(timestampRegex);
            Assert.IsTrue(timestampRegex.IsMatch(MW(delegate { return mode.Text; })), "TimeStamp matches regex");
            Assert.AreEqual(24, mode.Items.Count);
        }

        private void SelfTestRegex(Regex timestampRegex)
        {
            Assert.IsTrue(timestampRegex.IsMatch("1:00 AM"), "TimeStamp self test 1");
            Assert.IsTrue(timestampRegex.IsMatch("12:00 PM"), "TimeStamp self test 2");
            Assert.IsFalse(timestampRegex.IsMatch("16:00"), "TimeStamp self test 3");
            Assert.IsFalse(timestampRegex.IsMatch("01:00 IM"), "TimeStamp self test 4");
        }

        private void VerifyOptModeComboBox()
        {
            ComboBox mode = TestUtils.GetComboBox(dialog, "comboOptMode");
            Assert.AreEqual("Maximize Performance", MW(delegate { return mode.Text; }));
            Assert.AreEqual(2, mode.Items.Count);
        }

        private void FindTheSelectedDay()
        {
            WlbScheduledTask.WlbTaskDaysOfWeek foundDay = dialog.FindSelectedDay(WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday);
            Assert.AreEqual(WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday, foundDay);
        }

        private void VerifyDaysOfTheWeekComboBox()
        {
            ComboBox dotw = TestUtils.GetComboBox(dialog, "comboDayOfWeek");
            Assert.AreEqual(10, dotw.Items.Count);
            Assert.AreEqual(data.ExpectedDay, MW(delegate { return dotw.Text; }));
        }
    }
}