/* Copyright (c) Cloud Software Group, Inc. 
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

using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Dialogs;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAPI;

namespace XenAdminTests.LicensingTests
{
    public class LicenseManagerTests : UnitTester_SingleConnectionTestFixture
    {
        private Mock<ILicenseManagerView> view;

        private class TestCase
        {
            public ObjectBuilderType MockType;
            public Host.Edition License = Host.Edition.Free;
        }

        [SetUp]
        public void SetupMocks()
        {
            view = new Mock<ILicenseManagerView>();
            ObjectManager.ClearXenObjects(id);
        }

        private List<CheckableDataGridViewRow> CreateList(params TestCase[] testCases)
        {
            List<CheckableDataGridViewRow> rows = new List<CheckableDataGridViewRow>();
            foreach (TestCase tc in testCases)
            {
                Mock<Host> mh = ObjectFactory.BuiltObject<Host>(tc.MockType, id);
                Mock<ILicenseStatus> status = new Mock<ILicenseStatus>();
                status.Setup(s => s.LicenseEdition).Returns(tc.License);

                if (tc.License != Host.Edition.Free)
                {
                    mh.Setup(h => h.license_server).Returns(new Dictionary<string, string>
                                                                {{"address", "address"}, {"port", "port"}});
                }
                rows.Add(new LicenseDataGridViewRow(mh.Object, status.Object));
            }

            return rows;
        }

        [Test, Category(TestCategories.SmokeTest)]
        public void ButtonEnablementForNoSelection()
        {
            LicenseManagerController licenseSummary = new LicenseManagerController(view.Object);
            view.Setup(v => v.GetCheckedRows).Returns(new List<CheckableDataGridViewRow>());
            licenseSummary.UpdateButtonEnablement();
            view.Verify(v => v.DrawAssignButtonAsDisabled(true));
            view.Verify(v => v.DrawReleaseButtonAsDisabled(true));
        }

        [Test, Category(TestCategories.SmokeTest)]
        public void ButtonEnablementForUnlicensedClearwaterSelection()
        {
            LicenseManagerController licenseSummary = new LicenseManagerController(view.Object)
                {
                    VerifierFactory = new BlanketResponseSelectionVerifierFactory(new AllOkVerifier()),
                };

            view.Setup(v => v.GetCheckedRows).Returns(CreateList(new TestCase { MockType = ObjectBuilderType.ClearwaterHost}));

            licenseSummary.UpdateButtonEnablement();
            view.Verify(v => v.DrawAssignButtonAsDisabled(false));
            view.Verify(v => v.DrawReleaseButtonAsDisabled(true));
        }

        [Test, Category(TestCategories.SmokeTest)]
        public void ButtonEnablementForLicensedClearwaterSelection()
        {
            LicenseManagerController licenseSummary = new LicenseManagerController(view.Object)
            {
                VerifierFactory = new BlanketResponseSelectionVerifierFactory(new AllOkVerifier()),
            };

            view.Setup(v => v.GetCheckedRows).Returns(CreateList(new TestCase { 
                                                                        MockType = ObjectBuilderType.ClearwaterHost, 
                                                                        License = Host.Edition.PerSocket}));

            licenseSummary.UpdateButtonEnablement();
            view.Verify(v => v.DrawAssignButtonAsDisabled(false));
            view.Verify(v => v.DrawReleaseButtonAsDisabled(false));          
        }

        [Test, Category(TestCategories.SmokeTest)]
        public void ButtonEnablementForTwoLicensedClearwaterSelection()
        {
            LicenseManagerController licenseSummary = new LicenseManagerController(view.Object)
            {
                VerifierFactory = new BlanketResponseSelectionVerifierFactory(new AllOkVerifier()),
            };

            view.Setup(v => v.GetCheckedRows).Returns(CreateList(
                new TestCase
                {
                    MockType = ObjectBuilderType.ClearwaterHost,
                    License = Host.Edition.PerSocket
                }, 
                new TestCase
                {
                    MockType = ObjectBuilderType.ClearwaterHost,
                    License = Host.Edition.PerSocket
                }));

            licenseSummary.UpdateButtonEnablement();
            view.Verify(v => v.DrawAssignButtonAsDisabled(false));
            view.Verify(v => v.DrawReleaseButtonAsDisabled(false));
        }

        [Test, Category(TestCategories.SmokeTest)]
        public void ButtonEnablementForTwoMixedLicenseClearwaterSelection()
        {
            LicenseManagerController licenseSummary = new LicenseManagerController(view.Object)
            {
                VerifierFactory = new BlanketResponseSelectionVerifierFactory(new AllOkVerifier()),
            };

            view.Setup(v => v.GetCheckedRows).Returns(CreateList(
                new TestCase
                {
                    MockType = ObjectBuilderType.ClearwaterHost,
                    License = Host.Edition.PerSocket
                },
                new TestCase
                {
                    MockType = ObjectBuilderType.ClearwaterHost,
                    License = Host.Edition.Free
                }));

            licenseSummary.UpdateButtonEnablement();
            view.Verify(v => v.DrawAssignButtonAsDisabled(false));
            view.Verify(v => v.DrawReleaseButtonAsDisabled(false));
        }

        [Test, Category(TestCategories.SmokeTest)]
        public void ButtonEnablementForFailingVerifier()
        {
            LicenseManagerController licenseSummary = new LicenseManagerController(view.Object)
            {
                VerifierFactory = new BlanketResponseSelectionVerifierFactory(new AllNotOkVerifier()),
            };

            view.Setup(v => v.GetCheckedRows).Returns(CreateList(new TestCase
            {
                MockType = ObjectBuilderType.BostonHost,
                License = Host.Edition.Free
            }));

            licenseSummary.UpdateButtonEnablement();
            view.Verify(v => v.DrawAssignButtonAsDisabled(true));
            view.Verify(v => v.DrawReleaseButtonAsDisabled(true));
        }
    }
}
