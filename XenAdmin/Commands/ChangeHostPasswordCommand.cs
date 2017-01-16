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
using System.Text;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Dialogs;
using System.Collections.ObjectModel;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the change host password dialog for the specified host.
    /// </summary>
    internal class ChangeHostPasswordCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ChangeHostPasswordCommand()
        {
        }

        public ChangeHostPasswordCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ChangeHostPasswordCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        public ChangeHostPasswordCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Host host = selection[0].XenObject as Host;
            Pool pool = selection[0].XenObject as Pool;

            if (host != null)
                MainWindowCommandInterface.ShowPerConnectionWizard(selection[0].Connection, new ChangeServerPasswordDialog(host));
            if (pool != null)
                MainWindowCommandInterface.ShowPerConnectionWizard(selection[0].Connection, new ChangeServerPasswordDialog(pool));
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                IXenConnection connection = selection[0].Connection;
                Host host = selection[0].XenObject as Host;
                Pool pool = selection[0].XenObject as Pool;
                Session session = connection != null ? connection.Session : null;

                if (host != null)
                    return host.IsLive  && session != null && session.IsLocalSuperuser;
                if (pool != null)
                    return  session != null && session.IsLocalSuperuser;                
            }
            return false;
        }

        public override string ToolTipText
        {
            get
            {
                ReadOnlyCollection<SelectedItem> selection = GetSelection();

                if (selection.Count == 1)
                {
                    IXenConnection connection = selection[0].Connection;

                    // Only allow password change if the user is logged in as local root
                    // (i.e. disallow password change if the user is logged in via AD)

                    Host host = selection[0].XenObject as Host;
                    Pool pool = selection[0].XenObject as Pool;

                    if (host != null && host.IsLive && connection.Session != null && !connection.Session.IsLocalSuperuser)
                    {
                        return Messages.AD_CANNOT_CHANGE_PASSWORD;
                    }
                    
                    if (pool != null && connection.Session != null && !connection.Session.IsLocalSuperuser)
                    {
                        return Messages.AD_CANNOT_CHANGE_PASSWORD;
                    }                   
                }
                return string.Empty;
            }
        }
    }
}
