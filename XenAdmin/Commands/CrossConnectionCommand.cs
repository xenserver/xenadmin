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
using System.Windows.Forms;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using System.Drawing;


namespace XenAdmin.Commands
{
    // The only difference between a CrossConnectionCommand and a Command is that
    // it first checks that the user can do the necessary operation on all the
    // connections, and if not, it throws a dialog and gives up.
    //
    // At the moment, this does a very basic check, which is good enough for folders and tags:
    // it just checks that the user has better than Read Only access on all affected objects.
    // This breaks the abstraction in the Role code, but to calculate exactly which operations
    // are required on which objects is quite complicated (although it will probably become
    // necessary when we have more granular RBAC).
    internal abstract class CrossConnectionCommand : Command
    {
        // All the constructors just pass their arguments through to the base constructor
        protected CrossConnectionCommand() : base() {}
        protected CrossConnectionCommand(IMainWindow mainWindow) : base(mainWindow) {}
        protected CrossConnectionCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection) : base(mainWindow, selection) {}
        protected CrossConnectionCommand(IMainWindow mainWindow, IXenObject selection) : base(mainWindow, selection) {}

        protected override bool Confirm()
        {
            List<IXenConnection> failedConnections = FailedConnections();
            if (failedConnections.Count > 0)
            {
                if (!Program.RunInAutomatedTestMode)
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            SystemIcons.Error,
                            MessageBoxMessage(failedConnections),
                            Messages.PERMISSION_DENIED)))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }

                return false;  // returning false stops the command from proceeding
            }
            else
                return base.Confirm();
        }

        /// <returns>The list of connections that we didn't have permission on</returns>
        private List<IXenConnection> FailedConnections()
        {
            Dictionary<IXenConnection, bool> allConnections = new Dictionary<IXenConnection, bool>();
            foreach (IXenObject o in GetAffectedObjects())
                allConnections[o.Connection] = true;

            List<IXenConnection> failedConnections = new List<IXenConnection>();
            foreach (IXenConnection connection in allConnections.Keys)
            {
                if (IsReadOnly(connection))
                    failedConnections.Add(connection);
            }
            return failedConnections;
        }

        /// <summary>
        /// Find the list of connections to operate on.
        /// </summary>
        protected abstract List<IXenObject> GetAffectedObjects();

        /// <summary>
        /// Is the user Read Only on this connection? This should not normally be used:
        /// use the code in Role.cs instead, which is more abstract.
        /// </summary>
        public static bool IsReadOnly(IXenConnection connection)
        {
            return connection.Session.Roles.Find(
                    delegate(Role r) { return r.name_label.ToLowerInvariant() == Role.MR_ROLE_READ_ONLY; })
                != null;
        }

        private string MessageBoxMessage(List<IXenConnection> connections)
        {
            if (connections.Count == 1)
                return string.Format(Messages.READ_ONLY_ON_SINGULAR, Helpers.GetName(connections[0]));
            else
                return string.Format(Messages.READ_ONLY_ON_PLURAL,
                    Helpers.StringifyList(connections.ConvertAll<string>(delegate(IXenConnection c) { return Helpers.GetName(c); })));
        }
    }
}
