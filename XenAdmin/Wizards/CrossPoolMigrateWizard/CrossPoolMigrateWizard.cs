/* Copyright (c) Citrix Systems Inc. 
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
using System.Drawing;
using System.Linq;
using XenAdmin.Actions.VMActions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    internal partial class CrossPoolMigrateWizard : XenWizardBase
	{
        private CrossPoolMigrateDestinationPage m_pageDestination;
        private CrossPoolMigrateStoragePage m_pageStorage;
        private CrossPoolMigrateNetworkingPage m_pageNetwork;
        private CrossPoolMigrateFinishPage m_pageFinish;
        private CrossPoolMigrateTransferNetworkPage m_pageTransferNetwork;
        private RBACWarningPage m_pageTargetRbac;

        private IXenConnection TargetConnection { get; set; }

		private Dictionary<string, VmMapping> m_vmMappings = new Dictionary<string, VmMapping>();

        private Host hostPreSelection = null;

        public CrossPoolMigrateWizard(IXenConnection con, IEnumerable<SelectedItem> selection, Host targetHostPreSelection)
            : base(con)
        {
            InitializeComponent();
            hostPreSelection = targetHostPreSelection;
            InitialiseWizard(selection);
        }


        public CrossPoolMigrateWizard(IXenConnection con, IEnumerable<SelectedItem> selection)
			: base(con)
        {
            InitializeComponent();
            InitialiseWizard(selection);
        }

        private bool IsIntraPoolMigration()
        {
            return m_vmMappings.All(IsIntraPoolMigration);
        }

        private bool IsIntraPoolMigration(KeyValuePair<string, VmMapping> mapping)
        {
            VM vm = xenConnection.Resolve(new XenRef<VM>(mapping.Key));
            if (vm.resident_on == mapping.Value.XenRef)
                return true;

            Pool pool = Helpers.GetPool(vm.Connection);
            if (mapping.Value.XenRef is XenRef<Pool>)
            {
                Pool targetPool = TargetConnection.Resolve(mapping.Value.XenRef as XenRef<Pool>);
                if ( pool == targetPool)
                    return true;
            }

            Host host = xenConnection.Resolve(vm.resident_on);
            if (mapping.Value.XenRef is XenRef<Host>)
            {
                Host targetHost = TargetConnection.Resolve(mapping.Value.XenRef as XenRef<Host>);
                Pool targetPool = Helpers.GetPool(targetHost.Connection);

                // 2 stand alone hosts
                if(pool == null && targetPool == null)
                {
                    if(targetHost == host)
                        return true;

                    return false;
                }
                    
                if ( pool == targetPool)
                    return true;
            }

            return false;
        }

        private void InitialiseWizard(IEnumerable<SelectedItem> selection)
        {
            Text = Messages.CPM_WIZARD_TITLE;
            CreateMappingsFromSelection(selection);
            m_pageDestination = new CrossPoolMigrateDestinationPage(hostPreSelection, VmsFromSelection(selection) )
                                    {
                                        VmMappings = m_vmMappings,
                                    };

            m_pageStorage = new CrossPoolMigrateStoragePage();
            m_pageNetwork = new CrossPoolMigrateNetworkingPage();
            m_pageTransferNetwork = new CrossPoolMigrateTransferNetworkPage();
            m_pageFinish = new CrossPoolMigrateFinishPage {SummaryRetreiver = GetVMMappingSummary};
            m_pageTargetRbac = new RBACWarningPage();

            AddPages(m_pageDestination, m_pageStorage, m_pageTransferNetwork, m_pageFinish);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        protected override void FinishWizard()
        {
            foreach (KeyValuePair<string, VmMapping> pair in m_vmMappings)
            {
                VM vm = xenConnection.Resolve(new XenRef<VM>(pair.Key));
                
                Host target = TargetConnection.Resolve(m_vmMappings[pair.Key].XenRef as XenRef<Host>);

                //if a pool has been selected but no specific homeserver Key is the pool opaque ref
                if(target == null)
                {
                    Pool targetPool = TargetConnection.Resolve(m_vmMappings[pair.Key].XenRef as XenRef<Pool>);

                    if (targetPool == null)
                    {
                        ShowErrorMessageBox(Messages.CPM_WIZARD_ERROR_TARGET_DISCONNECTED);
                        base.FinishWizard();
                        return;
                    }
                        
                    target = TargetConnection.Resolve(targetPool.master);
                }

                if(target == null)
                    throw new ApplicationException("Cannot resolve the target host");

                new VMCrossPoolMigrateAction(vm, target, SelectedTransferNetwork, pair.Value).RunAsync();
            }
            
            base.FinishWizard();
        }

        private void ShowErrorMessageBox(string message)
        {
            new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, message)).ShowDialog(Program.MainWindow);
        }

        private void CreateMappingsFromSelection(IEnumerable<SelectedItem> selection)
        {
            foreach (SelectedItem item in selection)
            {
                VmMapping mapping = new VmMapping
                                        {
                                            VmNameLabel = item.XenObject.Name
                                        };
                
                m_vmMappings.Add(item.XenObject.opaque_ref, mapping);
            }
        }

        private List<VM> VmsFromSelection(IEnumerable<SelectedItem> selection)
        {
            return selection.Select(item => item.XenObject).OfType<VM>().ToList();
        }

        private void AddHostNameToWindowTitle()
        {
            if(m_vmMappings != null &&  m_vmMappings.Count > 0 ) 
                Text = String.Format(Messages.CPM_WIZARD_TITLE_AND_LOCATION, m_vmMappings.First().Value.TargetName);
            else
                Text = Messages.CPM_WIZARD_TITLE;
        }

        protected override void UpdateWizardContent(XenTabPage page)
        {
  
            Type type = page.GetType();
 
            if (type == typeof(CrossPoolMigrateDestinationPage))
            {
                RemovePage(m_pageNetwork);
                RemovePage(m_pageTargetRbac);
                m_vmMappings = m_pageDestination.VmMappings;
                TargetConnection = m_pageDestination.Connection;
                m_pageStorage.TargetConnection = TargetConnection;
                m_pageNetwork.TargetConnection = TargetConnection;
                m_pageTransferNetwork.Connection = TargetConnection;
                ConfigureRbacPage();
                AddHostNameToWindowTitle();

                if(!IsIntraPoolMigration())
                {
                    AddAfterPage(m_pageStorage, m_pageNetwork);
                }
                //If no network mappings xapi should map the networks
                //with the same names together - ideal for intra-pool
                ClearAllNetworkMappings();
                m_pageStorage.VmMappings = m_vmMappings;
                m_pageDestination.SetDefaultTarget(m_pageDestination.ChosenItem);
            }
            else if (type == typeof(CrossPoolMigrateStoragePage))
            {
                AddHostNameToWindowTitle();
                m_vmMappings = m_pageStorage.VmMappings;
                m_pageNetwork.VmMappings = m_vmMappings;
            }
            else if (type == typeof(CrossPoolMigrateNetworkingPage))
            {
                AddHostNameToWindowTitle();
                m_vmMappings = m_pageNetwork.VmMappings;
            }
            else if (type == typeof(CrossPoolMigrateTransferNetworkPage))
            {
                AddHostNameToWindowTitle();
                string netRef = m_pageTransferNetwork.NetworkUuid.Key;
                SelectedTransferNetwork = TargetConnection.Cache.Networks.FirstOrDefault(n => n.uuid == netRef);
            }

            if (type != typeof(CrossPoolMigrateFinishPage))
                NotifyNextPagesOfChange(m_pageDestination, m_pageStorage, m_pageNetwork, m_pageTransferNetwork, m_pageFinish);
        }

        private XenAPI.Network SelectedTransferNetwork { get; set; }

        private void ClearAllNetworkMappings()
        {
            foreach (KeyValuePair<string, VmMapping> pair in m_vmMappings)
                pair.Value.Networks = new Dictionary<string, XenAPI.Network>();
        }

        protected override string WizardPaneHelpID()
        {
            return FormatHelpId(CurrentStepTabPage.HelpID);
        }

        private bool ConnectionDoesNotRequireRBAC(IXenConnection connection)
        {
            if( connection == null )
                throw new NullReferenceException("RBAC check was given a null connection");

            if (connection.Session.IsLocalSuperuser)
                return true;

            if (Helpers.GetMaster(connection).external_auth_type == Auth.AUTH_TYPE_NONE)
                return true;

            return false;
        }


        private void ConfigureRbacPage()
        {
            if (ConnectionDoesNotRequireRBAC(xenConnection) && ConnectionDoesNotRequireRBAC(TargetConnection))
                return;

            m_pageTargetRbac.ClearPermissionChecks();
            RBACWarningPage.WizardPermissionCheck migrateCheck =
                new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_CROSS_POOL_MIGRATE_VM_BLOCKED) { Blocking = true };
            migrateCheck.AddApiCheckRange(VMCrossPoolMigrateAction.StaticRBACDependencies);

            m_pageTargetRbac.AddPermissionChecks(xenConnection, migrateCheck);
            if (!xenConnection.Equals(TargetConnection))
                m_pageTargetRbac.AddPermissionChecks(TargetConnection, migrateCheck);

            AddAfterPage(m_pageDestination, m_pageTargetRbac);
        }

        private IEnumerable<SummaryDetails> GetVMMappingSummary()
        {
            //Use decorators to build a summary
            MappingSummary summary = new VMMappingSummary();
            foreach (KeyValuePair<string, VmMapping> pair in m_vmMappings)
            {
                summary = new TitleSummary(summary, pair.Value);
                summary = new DestinationPoolSummary(summary, pair.Value, TargetConnection);
                summary = new HomeServerSummary(summary, pair.Value, TargetConnection);
                summary = new TransferNetworkSummary(summary, m_pageTransferNetwork.NetworkUuid.Value);
                summary = new StorageSummary(summary, pair.Value, xenConnection);
                summary = new NetworkSummary(summary, pair.Value, xenConnection); 
                summary = new SummarySplitter(summary);
            }
            return summary.Details;
        }
	}
}
