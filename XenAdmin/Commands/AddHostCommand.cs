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

using System.Windows.Forms;
using XenAdmin.Properties;
using System.Drawing;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Pops up the dialog for adding a new Host to XenCenter.
    /// </summary>
    internal class AddHostCommand : Command
    {
        private AddServerDialog _dialog;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public AddHostCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHostCommand"/> class. 
        /// </summary>
        /// <param name="mainWindow">The main window interface. It can be found at MainWindow.CommandInterface.</param>
        public AddHostCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHostCommand"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window interface. It can be found at MainWindow.CommandInterface.</param>
        /// <param name="parent">The parent for the Add Server dialog.</param>
        public AddHostCommand(IMainWindow mainWindow, Control parent)
            : base(mainWindow)
        {
            SetParent(parent);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            _dialog = new AddServerDialog(null, false);
            _dialog.CachePopulated += dialog_CachePopulated;
            _dialog.Show(Parent);
        }

        private void dialog_CachePopulated(object sender, CachePopulatedEventArgs e)
        {
            _dialog.CachePopulated -= dialog_CachePopulated;

            // first select the disconnected host in the tree
            // before the tree is populated, the opaque_ref of the disconnected host is the hostname
            // so use this to select the object.
            MainWindowCommandInterface.Invoke(() => MainWindowCommandInterface.SelectObjectInTree(new Host { opaque_ref = e.Connection.Hostname }));
            MainWindowCommandInterface.TrySelectNewObjectInTree(e.Connection, true, true, true);
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_AddApplicationServer_h32bit_16;
            }
        }

        public override Image ToolBarImage
        {
            get
            {
                
                return Images.StaticImages._000_AddApplicationServer_h32bit_24;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_ADD_HOST;
            }
        }
    }
}
