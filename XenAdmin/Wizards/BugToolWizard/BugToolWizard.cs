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

        private readonly GenericSelectHostsPage _bugToolPageSelectHosts;
        private readonly BugToolPageSelectCapabilities _bugToolPageSelectCapabilities;
        private readonly RBACWarningPage _rbacWarningPage;
        private readonly BugToolPageRetrieveData _bugToolPageRetrieveData;
        private readonly BugToolPageDestination _bugToolPageDestination;

        public BugToolWizard(params IXenObject[] selectedObjects)
        {
            InitializeComponent();

            _bugToolPageSelectHosts = new GenericSelectHostsPage();
            _bugToolPageSelectCapabilities = new BugToolPageSelectCapabilities();
            _rbacWarningPage = new RBACWarningPage();
            _bugToolPageDestination = new BugToolPageDestination();
            _bugToolPageRetrieveData = new BugToolPageRetrieveData();

            AddPages(_bugToolPageSelectHosts,
                _bugToolPageSelectCapabilities,
                _bugToolPageDestination,
                _bugToolPageRetrieveData);

            _bugToolPageSelectHosts.SelectHosts(new List<IXenObject>(selectedObjects));
        }

        protected override void FinishWizard()
        {
            log.Debug("Cleaning up crash dump logs on server");
            var capabilities = _bugToolPageSelectCapabilities.CheckedCapabilities;

            foreach (Capability c in capabilities)
            {
                if (c.Key == "host-crashdump-dumps" && c.Checked)
                {
                    var hostList = _bugToolPageSelectHosts.SelectedHosts;
                    
                    var crashedHosts = hostList.Where(h => h.HasCrashDumps()).ToList();
                    if (crashedHosts.Count == 0)
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
                        foreach (Host host in crashedHosts)
                            new DestroyHostCrashDumpAction(host).RunAsync();
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
                _bugToolPageRetrieveData.SelectedHosts = _bugToolPageSelectHosts.SelectedHosts;

                var selectedHostsConnections = _bugToolPageSelectHosts.SelectedHosts.Select(host => host.Connection).ToList();

                if (selectedHostsConnections.Any(Helpers.ConnectionRequiresRbac))
                {
                    _rbacWarningPage.SetPermissionChecks(selectedHostsConnections,
                        new WizardRbacCheck(Messages.RBAC_GET_SYSTEM_STATUS_BLOCKED, SingleHostStatusAction.StaticRBACDependencies) { Blocking = true });
                    AddAfterPage(_bugToolPageSelectHosts, _rbacWarningPage);
                }
            }
            else if (prevPageType == typeof(BugToolPageSelectCapabilities))
            {
                _bugToolPageRetrieveData.CapabilityList = _bugToolPageSelectCapabilities.CheckedCapabilities;
            }
            else if (prevPageType == typeof(BugToolPageDestination))
            {
                _bugToolPageRetrieveData.OutputFile = _bugToolPageDestination.OutputFile;
            }
        }

        protected override bool RunNextPagePrecheck(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(GenericSelectHostsPage))
            {
                var hostList = _bugToolPageSelectHosts.SelectedHosts;
                return _bugToolPageSelectCapabilities.GetCommonCapabilities(hostList);
            }

            return true;
        }
    }
}
