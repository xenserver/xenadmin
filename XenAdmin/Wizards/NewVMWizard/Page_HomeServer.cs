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

using System.Collections.Generic;
using XenAdmin.Actions.VMActions;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Core;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_HomeServer : XenTabPage
    {
        private VM Template;
        private Host CdAffinity;

        public Page_HomeServer()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_HOMESERVERPAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_HOMESERVERPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "HomeServer"; }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_HOMESERVERPAGE_HOMESERVER,
                                                         Affinity != null ? Affinity.Name : Messages.NEWVMWIZARD_HOMESERVER_NONE));
                return sum;
            }
        }

        public override bool EnableNext()
        {
            return affinityPicker1.ValidState();
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            // if not using CD, use selected template storage host
            // if using CD use CD or template storage host);
           
           if (SelectedInstallMethod != InstallMethod.CD || SelectedCD == null)
            {
                if (SelectedTemplate == Template && CdAffinity == null)
                    return;

                CdAffinity = null;
                Template = SelectedTemplate;
                affinityPicker1.SetAffinity(Connection, Affinity, Template.GetStorageHost(true));
            }
            else
            {
                SR sr = Connection.Resolve(SelectedCD.SR);
                if (sr == null)
                    return;

                Host cdAffinity = sr.GetStorageHost();

                if (SelectedTemplate == Template && cdAffinity == CdAffinity)
                    return;

                Template = SelectedTemplate;
                CdAffinity = cdAffinity;

                affinityPicker1.SetAffinity(Connection, Affinity,
                                           CdAffinity ?? Template.GetStorageHost(false));
            }
        }

        public override void SelectDefaultControl()
        {
            affinityPicker1.Select();
        }

        #endregion

        #region Accessors

        public Host SelectedHomeServer { get { return affinityPicker1.SelectedAffinity; } }

        public Host Affinity { private get; set; }
        public VM SelectedTemplate { private get; set; }
        public InstallMethod SelectedInstallMethod { private get; set; }
        public VDI SelectedCD { private get; set; }

        #endregion

        private void affinityPicker1_SelectedAffinityChanged(object sender, System.EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
