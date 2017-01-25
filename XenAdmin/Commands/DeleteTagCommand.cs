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
    /// Deletes the selected tags.
    /// </summary>
    internal class DeleteTagCommand : CrossConnectionCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DeleteTagCommand()
        {
        }

        public DeleteTagCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        private static bool CanExecute(GroupingTag groupingTag)
        {
            return groupingTag.Grouping.GroupingName == Messages.TAGS;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<GroupingTag>(CanExecute);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (GroupingTag groupingTag in selection.AsGroupingTags())
            {
                string tag = groupingTag.Group.ToString();
                DelegatedAsyncAction action = new DelegatedAsyncAction(null,
                    String.Format(Messages.DELETE_ALL_TAG, tag),
                    String.Format(Messages.DELETING_ALL_TAG, tag),
                    String.Format(Messages.DELETED_ALL_TAG, tag),
                    delegate(Session session)
                    {
                        Tags.RemoveTagGlobally(tag);
                    });

                actions.Add(action);
            }

            RunMultipleActions(actions, Messages.DELETE_TAGS, Messages.DELETING_TAGS, Messages.DELETED_TAGS, true);
        }

        public override string MenuText
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.MAINWINDOW_DELETE_TAG;
                }
                return Messages.MAINWINDOW_DELETE_TAGS;
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
                    GroupingTag groupingTag = GetSelection()[0].GroupingTag;
                    return string.Format(Messages.CONFIRM_DELETE_TAG, groupingTag.Group);
                }
                return Messages.CONFIRM_DELETE_TAGS;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.CONFIRM_DELETE_TAG_TITLE;
                }
                return Messages.CONFIRM_DELETE_TAGS_TITLE;
            }
        }

        protected override List<IXenObject> GetAffectedObjects()
        {
            List<IXenObject> objs = new List<IXenObject>();
            foreach (GroupingTag groupingTag in GetSelection().AsGroupingTags())
            {
                string tag = groupingTag.Group.ToString();
                objs.AddRange(Tags.Users(tag));
            }
            return objs;
        }
    }
}
