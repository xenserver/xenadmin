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

using System.Collections.Generic;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    internal class CrossPoolMigrateDestinationPage : SelectMultipleVMDestinationPage
    {
        private Host preSelectedHost;
        private List<VM> selectedVMs;
        private WizardMode wizardMode;
        
        public CrossPoolMigrateDestinationPage(): this(null, null, WizardMode.Migrate)
        {
        }

        public CrossPoolMigrateDestinationPage(Host preSelectedHost, List<VM> selectedVMs, WizardMode wizardMode)
        {
            this.preSelectedHost = preSelectedHost;
            SetDefaultTarget(preSelectedHost);
            this.selectedVMs = selectedVMs;
            this.wizardMode = wizardMode;
            InitializeText();
        }

        public override bool EnableNext()
        {
            return DestinationHasBeenSelected() && base.EnableNext();
        }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return "Destination"; } }

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.CPM_WIZARD_DESTINATION_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.CPM_WIZARD_DESTINATION_TAB_TITLE; } }

        protected override string InstructionText 
        {
            get
            {
                if (selectedVMs != null && selectedVMs.Count > 1)
                    return wizardMode == WizardMode.Copy ? Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_COPY : Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS;
                return wizardMode == WizardMode.Copy ? Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_COPY_SINGLE : Messages.CPM_WIZARD_DESTINATION_INSTRUCTIONS_SINGLE;
            }
        }

        protected override string HomeServerText { get { return Messages.CPM_WIZARD_DESTINATION_DESTINATION; } }

        protected override string HomeServerSelectionIntroText { get { return Messages.CPM_WIZARD_DESTINATION_TABLE_INTRO; } }


        public override DelayLoadingOptionComboBoxItem CreateDelayLoadingOptionComboBoxItem(IXenObject xenItem)
        {
           return new CrossPoolMigrateDelayLoadingComboBoxItem(xenItem, preSelectedHost, selectedVMs);
        }

        protected override List<ReasoningFilter> CreateHomeServerFilterList(IEnableableXenObjectComboBoxItem selectedItem)
        {
            List<ReasoningFilter> filters = new List<ReasoningFilter>{ new ResidentHostIsSameAsSelectionFilter(selectedVMs) };

            if(selectedItem != null)
            {
                filters.Add(new CrossPoolMigrateCanMigrateFilter(selectedItem.Item, selectedVMs));
                filters.Add(new WlbEnabledFilter(selectedItem.Item, selectedVMs));
            } 

            return filters;
        }
        
        public override void PageLeave(XenAdmin.Controls.PageLoadedDirection direction, ref bool cancel)
        {
            if (!cancel)
            {
                bool targetDisconnected = cancel;
                Program.Invoke(Program.MainWindow,
                               delegate
                                   {
                                       if (Connection == null || !Connection.IsConnected)
                                       {
                                           CrossPoolMigrateWizard.ShowWarningMessageBox(Messages.CPM_WIZARD_ERROR_TARGET_DISCONNECTED);
                                           targetDisconnected = true;
                                       }
                                   });
                cancel = targetDisconnected;
            }

            if (!cancel && !CrossPoolMigrateWizard.AllVMsAvailable(selectedVMs))
            {
                cancel = true;
                SetButtonNextEnabled(false);
            }

            base.PageLeave(direction, ref cancel);
        }
    }
}
