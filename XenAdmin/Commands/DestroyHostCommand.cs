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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using System.Linq;

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

        public DestroyHostCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        public DestroyHostCommand(IMainWindow mainWindow, IEnumerable<Host> hosts)
            : base(mainWindow, ConvertToSelection<Host>(hosts))
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
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

        private static bool CanExecute(Host host)
        {
            if (host == null || host.Connection == null)
            {
                return false;
            }
            Pool pool = Helpers.GetPool(host.Connection);
            return pool != null && !Helpers.HostIsMaster(host) && !host.IsLive;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (!selection.AllItemsAre<Host>() || selection.Count > 1)
            {
                return false;
            }
            
            return CanExecute(selection.AsXenObjects<Host>().First());
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.DESTROY_HOST_CONTEXT_MENU_ITEM_TEXT;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                SelectedItemCollection selection = GetSelection();
                if (selection.Count > 0)
                {
                    Host host = (Host)selection[0].XenObject;
                    Pool pool = Helpers.GetPool(host.Connection);
                    
                    return string.Format(Messages.CONFIRM_DESTROY_HOST, host.Name);
                }
                return null;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                return Messages.CONFIRM_DESTROY_HOST_TITLE;
            }
        }

        protected override string ConfirmationDialogYesButtonLabel
        {
            get
            {
                return Messages.CONFIRM_DESTROY_HOST_YES_BUTTON_LABEL;
            }
        }

        protected override bool ConfirmationDialogNoButtonSelected
        {
            get
            {
                return true;
            }
        }
    }
}
