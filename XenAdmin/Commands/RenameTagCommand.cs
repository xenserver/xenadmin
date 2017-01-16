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
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Model;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Renames the specified tag to the specified name.
    /// </summary>
    internal class RenameTagCommand : Command
    {
        private readonly string _oldName;
        private readonly string _newName;

        /// <summary>
        /// Occurs when the RenameTagGlobally action completes.
        /// </summary>
        public event EventHandler<RenameCompletedEventArgs> Completed;

        public RenameTagCommand(IMainWindow mainWindow, string oldName, string newName)
            : base(mainWindow)
        {
            Util.ThrowIfParameterNull(newName, "newName");
            Util.ThrowIfParameterNull(oldName, "oldName");

            if (oldName.Length == 0)
            {
                throw new ArgumentException("Invalid oldName", "oldName");
            }

            if (newName.Length == 0)
            {
                throw new ArgumentException("Invalid newName", "newName");
            }

            _oldName = oldName;
            _newName = newName;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return _oldName != _newName && _newName.Trim().Length > 0;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {

            DelegatedAsyncAction action = new DelegatedAsyncAction(null,
                String.Format(Messages.RENAME_TAG, _oldName),
                String.Format(Messages.RENAMING_TAG, _oldName),
                String.Format(Messages.RENAMED_TAG, _oldName),
                delegate(Session session)
                {
                    Tags.RenameTagGlobally(_oldName, _newName.Trim());
                });
            action.Completed += action_Completed;
            action.RunAsync();
        }

        private void action_Completed(ActionBase sender)
        {
            OnCompleted(new RenameCompletedEventArgs(sender.Succeeded));
        }

        protected virtual void OnCompleted(RenameCompletedEventArgs e)
        {
            EventHandler<RenameCompletedEventArgs> handler = Completed;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
