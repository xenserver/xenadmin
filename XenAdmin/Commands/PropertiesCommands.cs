/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Properties;
using System.Drawing;
using System.Collections.ObjectModel;
using XenAdmin.Model;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the properties dialog for the selected XenObject.
    /// </summary>
    internal class PropertiesCommand : Command
    {
        private readonly string _tab = "GeneralEditPage";
        private readonly string _control = "txtName";

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public PropertiesCommand()
        {
        }

        public PropertiesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public PropertiesCommand(IMainWindow mainWindow, IXenObject xenObject)
            : base(mainWindow, new SelectedItem(xenObject))
        {
        }

        public PropertiesCommand(IMainWindow mainWindow, IXenObject xenObject, string tab, string control)
            : this(mainWindow, xenObject)
        {
            _tab = tab;
            _control = control;
        }

        protected void Execute(IXenObject xenObject)
        {
            PropertiesDialog dialog = new PropertiesDialog(xenObject);
            dialog.SelectTab(_tab);
            dialog.SelectControl(_control);
            dialog.ShowDialog(Parent);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(selection[0].XenObject);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.ContainsOneItemOfType<IXenObject>())
            {
                IXenObject xenObject = selection[0].XenObject;
                return !(xenObject is Folder) && xenObject.Connection != null && xenObject.Connection.IsConnected;
            }
            return false;
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.EDIT;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Resources.edit_16;
            }
        }
    }

    /// <summary>
    /// Shows the properties dialog for the selected VM.
    /// </summary>
    internal class VMPropertiesCommand : PropertiesCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public VMPropertiesCommand()
        {
        }

        public VMPropertiesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public VMPropertiesCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        public VMPropertiesCommand(IMainWindow mainWindow, VM vm, string tab, string control)
            : base(mainWindow, vm, tab, control)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                VM vm = selection[0].XenObject as VM;
                return vm != null && !vm.is_a_template && !vm.Locked;
            }
            return false;
        }
    }

    /// <summary>
    /// Shows the properties dialog for the selected SR.
    /// </summary>
    internal class SRPropertiesCommand : PropertiesCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public SRPropertiesCommand()
        {
        }

        public SRPropertiesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public SRPropertiesCommand(IMainWindow mainWindow, SR sr)
            : base(mainWindow, sr)
        {
        }

        public SRPropertiesCommand(IMainWindow mainWindow, SR sr, string tab, string control)
            : base(mainWindow, sr, tab, control)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                SR sr = selection[0].XenObject as SR;
                return sr != null && !sr.Locked;
            }
            return false;
        }
    }

    /// <summary>
    /// Shows the properties dialog for the selected Pool.
    /// </summary>
    internal class PoolPropertiesCommand : PropertiesCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public PoolPropertiesCommand()
        {
        }

        public PoolPropertiesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public PoolPropertiesCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        public PoolPropertiesCommand(IMainWindow mainWindow, Pool pool, string tab, string control)
            : base(mainWindow, pool, tab, control)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(selection[0].PoolAncestor);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                IXenConnection connection = selection[0].Connection;
                bool connected = connection != null && connection.IsConnected;
                bool inPool = selection[0].PoolAncestor != null && !selection[0].PoolAncestor.Locked;
                return connected && inPool;
            }
            return false;
        }
    }

    /// <summary>
    /// Shows the properties dialog for the selected Host.
    /// </summary>
    internal class HostPropertiesCommand : PropertiesCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public HostPropertiesCommand()
        {
        }

        public HostPropertiesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public HostPropertiesCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        public HostPropertiesCommand(IMainWindow mainWindow, Host host, string tab, string control)
            : base(mainWindow, host, tab, control)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(selection[0].HostAncestor);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                IXenConnection connection = selection[0].Connection;
                bool connected = connection != null && connection.IsConnected;
                bool inHost = selection[0].HostAncestor != null &&  !selection[0].HostAncestor.Locked;

                return connected && inHost;
            }
            return false;
        }
    }

    /// <summary>
    /// Shows the properties dialog for the selected Template.
    /// </summary>
    internal class TemplatePropertiesCommand : PropertiesCommand
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public TemplatePropertiesCommand()
        {
        }

        public TemplatePropertiesCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public TemplatePropertiesCommand(IMainWindow mainWindow, VM template)
            : base(mainWindow, template)
        {
        }

        public TemplatePropertiesCommand(IMainWindow mainWindow, VM template, string tab, string control)
            : base(mainWindow, template, tab, control)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                VM vm = selection[0].XenObject as VM;

                return vm != null && vm.is_a_template && !vm.is_a_snapshot && !vm.Locked;
            }
            return false;
        }
    }
}
