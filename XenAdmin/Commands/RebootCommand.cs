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

using System.Drawing;
using System.Collections.ObjectModel;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Reboots the selected Host or VM.
    /// </summary>
    internal class RebootCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RebootCommand()
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            Command cmd = new RebootHostCommand(MainWindowCommandInterface, selection);

            if (cmd.CanRun())
            {
                cmd.Run();
            }
            else
            {
                cmd = new RebootVMCommand(MainWindowCommandInterface, selection);

                if (cmd.CanRun())
                {
                    cmd.Run();
                }
            }
        }

        public override string EnabledToolTipText
        {
            get
            {
                ReadOnlyCollection<SelectedItem> selection = GetSelection();

                if (selection.Count == 1 && selection[0].XenObject is VM)
                {
                    return Messages.MAINWINDOW_TOOLBAR_REBOOT;
                }
                return Messages.MAINWINDOW_TOOLBAR_REBOOTSERVER;
            }
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return new RebootVMCommand(MainWindowCommandInterface, selection).CanRun() || new RebootHostCommand(MainWindowCommandInterface, selection).CanRun();
        }

        public override Image ToolBarImage => Images.StaticImages._001_Reboot_h32bit_24;
    }
}
