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
using XenAPI;
using XenAdmin.Model;


namespace XenAdmin.Actions
{
    // Save folders and tags from the GeneralEditPage
    class GeneralEditPageAction : AsyncAction
    {
        private readonly IXenObject xenObjectOrig;
        private readonly IXenObject xenObjectCopy;
        private readonly string newFolder;
        private readonly List<string> oldTags, newTags;

        public GeneralEditPageAction(IXenObject xenObjectOrig, IXenObject xenObjectCopy, string newFolder, List<string> newTags, bool suppressHistory)
            : base(xenObjectCopy.Connection, Messages.ACTION_SAVE_FOLDER_TAGS, string.Format(Messages.ACTION_SAVING_FOLDER_TAGS_FOR, xenObjectCopy), suppressHistory)
        {
            this.xenObjectOrig = xenObjectOrig;
            this.xenObjectCopy = xenObjectCopy;
            this.newFolder = newFolder;
            this.oldTags = new List<string>(Tags.GetTags(xenObjectCopy));
            this.newTags = newTags;
            oldTags.Sort();
            newTags.Sort();

            string type = xenObjectCopy.GetType().Name.ToLowerInvariant();

            if (newFolder != xenObjectCopy.Path)
            {
                ApiMethodsToRoleCheck.Add(type + ".remove_from_other_config", Folders.FOLDER);
                if (!String.IsNullOrEmpty(newFolder))
                    ApiMethodsToRoleCheck.Add(type + ".add_to_other_config", Folders.FOLDER);
                // TODO: Full RBAC for folders
            }
            foreach (string tag in oldTags)
            {
                if (newTags.BinarySearch(tag) < 0)
                {
                    ApiMethodsToRoleCheck.Add(type + ".remove_tags");
                    break;
                }
            }
            foreach (string tag in newTags)
            {
                if (oldTags.BinarySearch(tag) < 0)
                {
                    ApiMethodsToRoleCheck.Add(type + ".add_tags");
                    break;
                }
            }
        }

        protected override void Run()
        {
            if (newFolder != xenObjectCopy.Path)
            {
                if (!String.IsNullOrEmpty(newFolder))
                    Folders.Move(Session, xenObjectOrig, newFolder);
                else
                    Folders.Unfolder(Session, xenObjectOrig);
            }

            foreach (string tag in oldTags)
            {
                if (newTags.BinarySearch(tag) < 0)
                    Tags.RemoveTag(Session, xenObjectOrig, tag);
            }
            foreach (string tag in newTags)
            {
                if (oldTags.BinarySearch(tag) < 0)
                    Tags.AddTag(Session, xenObjectOrig, tag);
            }
        }
    }
}
