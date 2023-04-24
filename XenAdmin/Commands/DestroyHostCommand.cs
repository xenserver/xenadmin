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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Destroys the selected hosts.
    /// </summary>
    class DestroyHostCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DestroyHostCommand()
        {
        }

        public DestroyHostCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();

            foreach (Host host in selection.AsXenObjects<Host>())
            {
                Pool pool = Helpers.GetPool(host.Connection);
                var action = new DestroyHostAction(pool, host);
                actions.Add(action);
            }

            RunMultipleActions(actions, Messages.DESTROYING_HOSTS_TITLE, Messages.DESTROYING_HOSTS_START_DESC,
                               Messages.DESTROYING_HOSTS_END_DESC, true);
        }

        private static bool CanRun(Host host)
        {
            if (host?.Connection == null)
                return false;

            Pool pool = Helpers.GetPool(host.Connection);
            return pool != null && !Helpers.HostIsCoordinator(host) && !host.IsLive();
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            foreach (var selectedItem in selection)
            {
                if (!(selectedItem.XenObject is Host host) || !CanRun(host))
                    return false;
            }

            return true;
        }

        public override string ContextMenuText => Messages.DESTROY_HOST_CONTEXT_MENU_ITEM_TEXT;

        protected override bool ConfirmationRequired => true;

        protected override string ConfirmationDialogText
        {
            get
            {
                SelectedItemCollection selection = GetSelection();

                if (selection.Count == 1)
                {
                    Host host = (Host)selection[0].XenObject;
                    return string.Format(Messages.CONFIRM_DESTROY_HOST, host.Name());
                }
                
                if (selection.Count > 1)
                    return Messages.CONFIRM_DESTROY_HOST_MANY;

                return null;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                SelectedItemCollection selection = GetSelection();
                
                if (selection.Count == 1)
                    return Messages.CONFIRM_DESTROY_HOST_TITLE;

                if (selection.Count > 1)
                    return Messages.CONFIRM_DESTROY_HOST_TITLE_MANY;

                return null;
            }
        }

        protected override string ConfirmationDialogYesButtonLabel => Messages.CONFIRM_DESTROY_HOST_YES_BUTTON_LABEL;

        protected override bool ConfirmationDialogNoButtonSelected => true;
    }
}
