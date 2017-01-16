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
using System.Windows.Forms;
using System.Collections.ObjectModel;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Deletes the selected folder. Shows a confirmation dialog.
    /// </summary>
    internal class DeleteFolderCommand : CrossConnectionCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DeleteFolderCommand()
        {
        }

        public DeleteFolderCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public DeleteFolderCommand(IMainWindow mainWindow, Folder folder)
            : base(mainWindow, folder)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<IXenObject> folders = new List<IXenObject>(selection.AsXenObjects<Folder>(CanExecute));

            folders.RemoveAll((Predicate<IXenObject>)delegate(IXenObject folder)
            {
                // if the list contains any folders that are children to others in the list then
                // they will automatically get deleted, so remove them here.

                foreach (var f in folders)
                {
                    if (folder.opaque_ref.StartsWith(f.opaque_ref + "/"))
                    {
                        return true;
                    }
                }
                return false;
            });

            new DeleteFolderAction(folders).RunAsync();
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<Folder>(CanExecute);
        }

        private static bool CanExecute(Folder folder)
        {
            return folder != null && !folder.IsRootFolder;
        }

        public override string MenuText
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.DELETE_FOLDER_MENU;
                }
                return Messages.MAINWINDOW_DELETE_OBJECTS;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    Folder folder = (Folder)GetSelection()[0].XenObject;
                    bool hasSubfolders = Folders.HasSubfolders(folder.opaque_ref);
                    bool hasContents = Folders.ContainsResources(folder.opaque_ref);

                    string msg = Messages.DELETE_FOLDER_CONFIRM_EMPTY;

                    if (hasContents && hasSubfolders)
                    {
                        msg = Messages.DELETE_FOLDER_CONFIRM_CONTENTS_AND_SUBFOLDERS;
                    }
                    else if (hasContents)
                    {
                        msg = Messages.DELETE_FOLDER_CONFIRM_CONTENTS;
                    }
                    else if (hasSubfolders)
                    {
                        msg = Messages.DELETE_FOLDER_CONFIRM_SUBFOLDERS;
                    }

                    return string.Format(msg, folder.Name);
                }
                else
                {
                    bool hasSubfolders = false;
                    bool hasContents = false;

                    foreach (Folder folder in GetSelection().AsXenObjects<Folder>(CanExecute))
                    {
                        hasSubfolders |= Folders.HasSubfolders(folder.opaque_ref);
                        hasContents |= Folders.ContainsResources(folder.opaque_ref);

                        if (hasContents && hasSubfolders)
                        {
                            break;
                        }
                    }

                    if (hasContents && hasSubfolders)
                    {
                        return Messages.DELETE_FOLDERS_CONFIRM_CONTENTS_AND_SUBFOLDERS;
                    }
                    else if (hasContents)
                    {
                        return Messages.DELETE_FOLDERS_CONFIRM_CONTENTS;
                    }
                    else if (hasSubfolders)
                    {
                        return Messages.DELETE_FOLDERS_CONFIRM_SUBFOLDERS;
                    }
                    return Messages.DELETE_FOLDERS_CONFIRM_EMPTY;
                }
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.DELETE_FOLDER_DIALOG_TITLE;
                }
                return Messages.DELETE_FOLDERS_DIALOG_TITLE;
            }
        }

        protected override List<IXenObject> GetAffectedObjects()
        {
            List<IXenObject> objs = new List<IXenObject>();
            foreach (Folder folder in GetSelection().AsXenObjects<Folder>(CanExecute))
            {
                objs.AddRange(Folders.Descendants(folder.opaque_ref));
                objs.Add(folder);
                // Whether the folder and its descendant folders need to be operated on depends on
                // whether they're in the empty folders list on any connection. That's complicated
                // to work out, so let's just assume conservatively that they all do.
            }
            return objs;
        }
    }
}
