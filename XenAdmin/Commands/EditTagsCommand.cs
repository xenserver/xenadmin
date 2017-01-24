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
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Actions;
using System.Windows.Forms;
using System.Drawing;
using XenAdmin.Properties;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the edit-tags dialog for the selection.
    /// </summary>
    internal class EditTagsCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public EditTagsCommand()
        {
        }

        public EditTagsCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        private bool CanExecute(IXenObject xenObject)
        {
            return !(xenObject is Folder) && xenObject.Connection != null;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<IXenObject>(CanExecute);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<string> tags = new List<string>();
            List<string> indeterminateTags = new List<string>();

            Dictionary<IXenObject, List<string>> d = new Dictionary<IXenObject, List<string>>();

            foreach (SelectedItem item in selection)
            {
                d[item.XenObject] = new List<string>(Tags.GetTags(item.XenObject));
            }

            foreach (string tag in Tags.GetAllTags())
            {
                bool contained = false;
                bool notContained = false;
                foreach (IXenObject x in d.Keys)
                {
                    if (d[x].Contains(tag))
                    {
                        contained = true;
                    }
                    else
                    {
                        notContained = true;
                    }
                }

                if (contained && notContained)
                {
                    indeterminateTags.Add(tag);
                }
                else if (contained)
                {
                    tags.Add(tag);
                }
            }

            // show dialog modally
            NewTagDialog newTagDialog = new NewTagDialog(tags, indeterminateTags);
            
            if (DialogResult.OK == newTagDialog.ShowDialog(Parent))
            {
                List<AsyncAction> actions = new List<AsyncAction>();
                foreach (IXenObject xenObject in selection.AsXenObjects())
                {
                    // rebuild tabs lists as tags can be deleted in the dialog.
                    List<string> newTags = new List<string>(Tags.GetTags(xenObject));

                    for (int i = newTags.Count - 1; i >= 0; i--)
                    {
                        // remove any tags from this xenobject which aren't either checked or indeterminate
                        if (!newTagDialog.GetSelectedTags().Contains(newTags[i]) && !newTagDialog.GetIndeterminateTags().Contains(newTags[i]))
                        {
                            newTags.RemoveAt(i);
                        }
                    }

                    // now add any new tags
                    foreach (string t in newTagDialog.GetSelectedTags())
                    {
                        if (!newTags.Contains(t))
                        {
                            newTags.Add(t);
                        }
                    }

                    actions.Add(new GeneralEditPageAction(xenObject, xenObject.Clone(), xenObject.Path, newTags, true));

                }
                RunMultipleActions(actions, Messages.ACTION_SAVING_TAGS_TITLE, Messages.ACTION_SAVING_TAGS_DESCRIPTION, Messages.ACTION_SAVING_TAGS_DESCRIPTION, true);
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_EDIT_TAGS;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_Tag_h32bit_16;
            }
        }
    }
}
