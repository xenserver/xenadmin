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
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Drawing;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    internal class AddNewHostToPoolCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Pool _pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewHostToPoolCommand"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="pool">The pool that the new host is to be added to.</param>
        public AddNewHostToPoolCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow)
        {
            Util.ThrowIfParameterNull(pool, "pool");
            _pool = pool;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            AddServerDialog dialog = new AddServerDialog(null, false);
            dialog.CachePopulated += dialog_CachePopulated;
            dialog.Show(Parent);
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_AddApplicationServer_h32bit_16;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.ADD_NEW_SERVER_MENU_ITEM;
            }
        }

        /// <summary>
        /// Called in the 'connect new server and add to pool' action after the server has connected
        /// and its cache has been populated. Adds the new server to the pool.
        /// </summary>
        private void dialog_CachePopulated(object sender, CachePopulatedEventArgs e)
        {
            IXenConnection newConnection = e.Connection;

            // A new connection was successfully made: add the new server to its destination pool.
            Host hostToAdd = Helpers.GetMaster(newConnection);
            if (hostToAdd == null)
            {
                log.Debug("hostToAdd is null while joining host to pool in AddNewHostToPoolCommand: this should never happen!");
                return;
            }

            // Check newly-connected host is not in pool
            Pool hostPool = Helpers.GetPool(newConnection);

            MainWindowCommandInterface.Invoke(delegate
            {
                if (hostPool != null)
                {
                    string text = String.Format(Messages.HOST_ALREADY_IN_POOL, hostToAdd.Name, _pool.Name, hostPool.Name);
                    string caption = Messages.POOL_JOIN_IMPOSSIBLE;

                    using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Exclamation, text, caption)))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                }
                else
                {
                    new AddHostToPoolCommand(MainWindowCommandInterface, new Host[] { hostToAdd }, _pool, false).Execute();
                }
            });
        }
    }
}
