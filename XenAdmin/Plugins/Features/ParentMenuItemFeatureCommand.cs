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
using System.Drawing;
using System.Collections.ObjectModel;
using XenAdmin.XenSearch;
using XenAdmin.Commands;

namespace XenAdmin.Plugins
{
    internal class ParentMenuItemFeatureCommand : Command
    {
        private readonly ParentMenuItemFeature _owner;
        private readonly Search _search;

        public ParentMenuItemFeatureCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, ParentMenuItemFeature owner, Search search)
            : base(mainWindow, selection)
        {
            Util.ThrowIfParameterNull(owner, "owner");
            _owner = owner;
            _search = search;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (!_owner.Enabled)
            {
                return false;
            }

            if (_search == null)
            {
                return true;
            }

            if (selection.ContainsOneItemOfType<IXenObject>())
            {
                return _search.Query.Match(selection[0].XenObject);
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                return _owner.ToString();
            }
        }

        public override Image MenuImage
        {
            get
            {
                return _owner.Icon;
            }
        }

        public override string ToolTipText
        {
            get
            {
                return _owner.Tooltip ?? string.Empty;
            }
        }
    }
}
