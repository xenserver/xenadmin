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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.BugToolWizard
{
    public partial class BugToolWizard : XenWizardBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly GenericSelectHostsPage bugToolPageSelectHosts1;
        private readonly BugToolPageSelectCapabilities bugToolPageSelectCapabilities1;
        private readonly RBACWarningPage rbacWarningPage;
        private readonly BugToolPageRetrieveData bugToolPageRetrieveData;
        private readonly BugToolPageDestination bugToolPageDestination1;

        public BugToolWizard(params IXenObject[] selectedObjects)
            : this()
        {
            bugToolPageSelectHosts1.SelectHosts(new List<IXenObject>(selectedObjects));
        }

        public BugToolWizard()
        {
            InitializeComponent();

            bugToolPageSelectHosts1 = new GenericSelectHostsPage();
            bugToolPageSelectCapabilities1 = new BugToolPageSelectCapabilities();
            rbacWarningPage = new RBACWarningPage();

            bugToolPageDestination1 = new BugToolPageDestination();
            bugToolPageRetrieveData = new BugToolPageRetrieveData();

            AddPage(bugToolPageSelectHosts1);
            AddPage(bugToolPageSelectCapabilities1);

            AddPage(bugToolPageDestination1);
            AddPage(bugToolPageRetrieveData);
        }

        protected override void FinishWizard()
        {
            log.Debug("Cleaning up crash dump logs on server");
            var capabilities = bugToolPageSelectCapabilities1.CheckedCapabilities;
            foreach (Capability c in capabilities)
            {
                if (c.Key == "host-crashdump-dumps" && c.Checked)
                {
                    var hostList = bugToolPageSelectHosts1.SelectedHosts;
                    if (!hostList.Any(h => h.HasCrashDumps()))
                        break;

                    DialogResult result;
                    using (var dlg = new NoIconDialog(Messages.REMOVE_CRASHDUMP_QUESTION,
                                ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                    { WindowTitle = Messages.REMOVE_CRASHDUMP_FILES })
                    {
                        result = dlg.ShowDialog(this);
                    }
                    if (result == DialogResult.Yes)
                    {
                        foreach (Host host in hostList)
                        {
                            if (host != null && host.HasCrashDumps())
                            {
                                new Actions.DestroyHostCrashDumpAction(host).RunAsync();
                            }
                        }
                    }
                    break;
                }
            }

            base.FinishWizard();
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();
            if (prevPageType == typeof(GenericSelectHostsPage))
            {
                bugToolPageRetrieveData.SelectedHosts = bugToolPageSelectHosts1.SelectedHosts;

                var selectedHostsConnections = bugToolPageSelectHosts1.SelectedHosts.Select(host => host.Connection).ToList();

                if (selectedHostsConnections.Any(Helpers.ConnectionRequiresRbac))
                {
                    rbacWarningPage.SetPermissionChecks(selectedHostsConnections,
                        new WizardRbacCheck(Messages.RBAC_GET_SYSTEM_STATUS_BLOCKED, SingleHostStatusAction.StaticRBACDependencies) { Blocking = true });
                    AddAfterPage(bugToolPageSelectHosts1, rbacWarningPage);
                }
            }
            else if (prevPageType == typeof(BugToolPageSelectCapabilities))
            {
                bugToolPageRetrieveData.CapabilityList = bugToolPageSelectCapabilities1.CheckedCapabilities;
            }
            else if (prevPageType == typeof(BugToolPageDestination))
            {
                bugToolPageRetrieveData.OutputFile = bugToolPageDestination1.OutputFile;
            }
        }

        protected override bool RunNextPagePrecheck(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(GenericSelectHostsPage))
            {
                var hostList = bugToolPageSelectHosts1.SelectedHosts;
                return bugToolPageSelectCapabilities1.GetCommonCapabilities(hostList);
            }

            return true;
        }
    }
}
