﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Wizards;
using System.Drawing;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the New SR Wizard.
    /// </summary>
    internal class NewSRCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public NewSRCommand()
        {
        }

        public NewSRCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public NewSRCommand(IMainWindow mainWindow, IXenConnection connection)
            : base(mainWindow, Helpers.GetPoolOfOne(connection))
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            MainWindowCommandInterface.ShowPerConnectionWizard(selection[0].Connection, new NewSRWizard(selection[0].Connection));
        }

        private static bool CanExecuteOnHost(Host host)
        {
            return host != null && host.Connection.IsConnected && !HelpersGUI.HasActiveHostAction(host);
        }

        private static bool CanExecuteOnPool(Pool pool)
        {
            return pool != null && pool.Connection.IsConnected;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.HostAncestor != null)
            {
                return CanExecuteOnHost(selection.HostAncestor);
            }
            else if (selection.PoolAncestor != null)
            {
                return CanExecuteOnPool(selection.PoolAncestor);
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_NEW_STORAGE;
            }
        }

        public override string ButtonText
        {
            get
            {
                return Messages.MAINWINDOW_NEW_STORAGE;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_NewStorage_h32bit_16;
            }
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._000_NewStorage_h32bit_24;
            }
        }
    }
}
