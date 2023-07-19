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
using System.Reflection;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Creates a template from the selected snapshot. Shows a confirmation dialog.
    /// </summary>
    internal class NewTemplateFromSnapshotCommand : Command
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public NewTemplateFromSnapshotCommand()
        {
        }

        public NewTemplateFromSnapshotCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public NewTemplateFromSnapshotCommand(IMainWindow mainWindow, VM snapshot)
            : base(mainWindow, snapshot)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var snapshot = (VM)selection[0].XenObject;

            // Generate list of all taken VM/snapshot/template names
            var takenNames = new List<VM>(snapshot.Connection.Cache.VMs).ConvertAll(v => v.Name());

            // Generate a unique suggested name for the new template
            var defaultName = Helpers.MakeUniqueName(string.Format(Messages.TEMPLATE_FROM_SNAPSHOT_DEFAULT_NAME, snapshot.Name()), takenNames);

            using (var dialog = new InputPromptDialog
            {
                Text = Messages.SAVE_AS_TEMPLATE,
                PromptText = Messages.NEW_TEMPLATE_PROMPT,
                InputText = defaultName,
                HelpID = "VMSnapshotPage"
            })
            {
                if (dialog.ShowDialog(Parent) == DialogResult.OK)
                {
                    var action = new VMCloneAction(snapshot, dialog.InputText, string.Format(Messages.TEMPLATE_FROM_SNAPSHOT_DEFAULT_DESCRIPTION, BrandManager.BrandConsole, snapshot.Name()));
                    action.Completed += action_Completed;
                    action.RunAsync();
                }
            }
        }

        private static void action_Completed(ActionBase sender)
        {
            if (!sender.Succeeded || !(sender is AsyncAction action))
                return;

            var vm = action.Connection.Resolve(new XenRef<VM>(action.Result));
            if (vm != null)
                new SetVMOtherConfigAction(vm.Connection, vm, "instant", "true").RunAsync();
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<VM>() && selection.AtLeastOneXenObjectCan<VM>(v => v.is_a_snapshot);
        }

        public override string MenuText => Messages.CREATE_TEMPLATE_FROM_SNAPSHOT_MENU_ITEM;
    }
}
