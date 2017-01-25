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
using XenAdmin.Model;
using XenAPI;
using XenAdmin.Actions;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Renames the specified folder to the specified name.
    /// </summary>
    internal class RenameFolderCommand : Command
    {
        private readonly string _newName;
        private readonly Folder _folder;

        /// <summary>
        /// Occurs when the RenameFolder action completes.
        /// </summary>
        public event EventHandler<RenameCompletedEventArgs> Completed;

        public RenameFolderCommand(IMainWindow mainWindow, Folder folder, string newName)
            : base(mainWindow, folder)
        {
            Util.ThrowIfParameterNull(newName, "newName");
            Util.ThrowIfParameterNull(folder, "folder");

            if (newName.Length == 0)
            {
                throw new ArgumentException("Invalid name", "newName");
            }

            _newName = newName;
            _folder = folder;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            string newName = _newName;
            Folders.FixupRelativePath(ref newName);

            FolderAction action = new RenameFolderAction(_folder, newName);
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

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (!_folder.IsRootFolder)
            {
                string newName = _newName;
                Folders.FixupRelativePath(ref newName);
                return newName != _folder.Name && !newName.Contains(";") && !newName.Contains("/"); // CA-29480
            }
            return false;
        }
    }

    internal class RenameCompletedEventArgs : EventArgs
    {
        public bool Success { get; private set; }
        
        public RenameCompletedEventArgs(bool success)
        {
            Success = success;
        }
    }
}
