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
    /// Puts the treenode for the selected folder into edit mode.
    /// </summary>
    internal class PutFolderIntoRenameModeCommand : CrossConnectionCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public PutFolderIntoRenameModeCommand()
        {
        }

        public PutFolderIntoRenameModeCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public PutFolderIntoRenameModeCommand(IMainWindow mainWindow, Folder folder)
            : base(mainWindow, folder)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            MainWindowCommandInterface.PutSelectedNodeIntoEditMode();
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<Folder>(CanExecute);
        }

        private static bool CanExecute(Folder folder)
        {
            return !folder.IsRootFolder;
        }

        public override string MenuText
        {
            get
            {
                return Messages.RENAME_FOLDER;
            }
        }

        protected override List<IXenObject> GetAffectedObjects()
        {
            Folder folder = (Folder)GetSelection()[0].XenObject;
            List<IXenObject> objs = new List<IXenObject>(Folders.Descendants(folder.opaque_ref));
            objs.Add(folder);
            // Whether the folder and its descendant folders need to be operated on depends on
            // whether they're in the empty folders list on any connection. That's complicated
            // to work out, so let's just assume conservatively that they all do.
            return objs;
        }
    }
}
