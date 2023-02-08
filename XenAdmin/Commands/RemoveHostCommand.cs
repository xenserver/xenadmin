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
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Removes the selected hosts.
    /// </summary>
    internal class RemoveHostCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RemoveHostCommand()
        {
        }

        public RemoveHostCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public RemoveHostCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            MainWindowCommandInterface.SelectObjectInTree(null);

            foreach (Host host in selection.AsXenObjects<Host>())
            {
                string msg = string.Format(Messages.MAINWINDOW_LOG_REMOVECONNECTION, host.Connection.Hostname);
                log.Info($"Removed connection to {host.Connection.Hostname}");
                new DummyAction(msg, msg).Run();
                MainWindowCommandInterface.CloseActiveWizards(host.Connection);
                host.Connection.EndConnect();
                MainWindowCommandInterface.RemoveConnection(host.Connection);
            }

            MainWindowCommandInterface.SaveServerList();
        }

        private static bool CanRun(Host host)
        {
            bool disconnected = host.Connection != null && !host.Connection.IsConnected;
            return disconnected || host.IsCoordinator();
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<Host>(CanRun);
        }

        public override string MenuText => string.Format(Messages.MAINWINDOW_REMOVE_HOST, BrandManager.BrandConsole);
    }
}
