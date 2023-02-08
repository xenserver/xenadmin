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
using XenAdmin.Dialogs;
using XenAPI;


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

        protected override void RunCore(SelectedItemCollection selection)
        {
            if (selection[0].XenObject is Host host)
                MainWindowCommandInterface.ShowPerConnectionWizard(host.Connection, new ChangeServerPasswordDialog(host));
            else if (selection[0].XenObject is Pool pool)
                MainWindowCommandInterface.ShowPerConnectionWizard(pool.Connection, new ChangeServerPasswordDialog(pool));
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            // Only allow password change if the user is logged in as local root
            // (i.e. disallow password change if the user is logged in via AD)

            if (selection.Count == 1)
            {
                var session = selection[0].Connection?.Session;

                if (session != null && session.IsLocalSuperuser)
                    return selection[0].XenObject is Host host && host.IsLive() || selection[0].XenObject is Pool;
            }

            return false;
        }

        protected override string GetCantRunReasonCore(IXenObject item)
        {
            var session = item.Connection?.Session;

            if (session != null && !session.IsLocalSuperuser && (item is Host host && host.IsLive() || item is Pool))
                return Messages.AD_CANNOT_CHANGE_PASSWORD;
            
            return base.GetCantRunReasonCore(item);
        }
    }
}
