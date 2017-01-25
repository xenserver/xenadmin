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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Pops up the dialog for managing vApps or VMPPs in XenCenter.
    /// </summary>
    internal class VMGroupCommand<T> : Command where T : XenObject<T>
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public VMGroupCommand()
        {
        }

        public VMGroupCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public VMGroupCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public VMGroupCommand(IMainWindow mainWindow, IXenObject selection)
            : base(mainWindow, selection)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            
            var pool = Helpers.GetPoolOfOne(selection.FirstAsXenObject.Connection);
            if (pool != null)
            {
                if (Helpers.FeatureForbidden(pool.Connection, VMGroup<T>.FeatureRestricted)) 
                    ShowUpsellDialog(Parent);
                else
                    this.MainWindowCommandInterface.ShowPerConnectionWizard(pool.Connection, VMGroup<T>.ManageGroupsDialog(pool));
            }
        }

        public static void ShowUpsellDialog(IWin32Window parent)
        {
            using (var dlg = new UpsellDialog(VMGroup<T>.UpsellBlurb, VMGroup<T>.UpsellLearnMoreUrl))
                dlg.ShowDialog(parent);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (typeof(T) == typeof(VMPP) && selection.Any(s => Helpers.ClearwaterOrGreater(s.Connection)))
            {
                return false;
            }
            else if (typeof(T) == typeof(VMSS) && selection.Any(s => !Helpers.DundeeOrGreater(s.Connection)))
            {
                return false;
            }

            return selection.FirstAsXenObject != null && selection.FirstAsXenObject.Connection != null && selection.FirstAsXenObject.Connection.IsConnected
                && VMGroup<T>.FeaturePossible(selection.FirstAsXenObject.Connection)
				&& (selection.PoolAncestor != null || selection.HostAncestor != null); //CA-61207: this check ensures there's no cross-pool selection
        }

        public override string ContextMenuText
        {
            get
            {
                return VMGroup<T>.ManageContextMenuString; 
            }
        }

        public override string MenuText
        {
            get
            {
                return VMGroup<T>.ManageMainMenuString;
            }
        }
    }

    /// <summary>
    /// Class used for the benefit of visual studio's form designer which has trouble with generic controls
    /// </summary>
    internal sealed class VMGroupCommandVMPP : VMGroupCommand<VMPP>
    { }

    /// <summary>
    /// Class used for the benefit of visual studio's form designer which has trouble with generic controls
    /// </summary>
    internal sealed class VMGroupCommandVMSS : VMGroupCommand<VMSS>
    { }

    /// <summary>
    /// Class used for the benefit of visual studio's form designer which has trouble with generic controls
    /// </summary>
    internal sealed class VMGroupCommandVM_appliance : VMGroupCommand<VM_appliance>
    { }
}
