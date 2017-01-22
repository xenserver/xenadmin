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
using XenAdmin;
using XenAdmin.Commands;

namespace XenAdminTests
{
    internal class MainToolBarWrapper : TestWrapper<MainWindow>
    {
        public MainToolBarWrapper(MainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public CommandToolStripButton AddHostToolStripButton
        {
            get
            {
                return GetField<CommandToolStripButton>("AddServerToolbarButton");
            }
        }

        public CommandToolStripButton StartVMToolStripButton
        {
            get
            {
                return GetField<CommandToolStripButton>("startVMToolStripButton");
            }
        }

        public CommandToolStripButton ShutDownToolStripButton
        {
            get
            {
                return GetField<CommandToolStripButton>("shutDownToolStripButton");
            }
        }

        public CommandToolStripButton NewVMToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("NewVmToolbarButton");
            }
        }
        
        public CommandToolStripButton ForceShutdownToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("ForceShutdownToolbarButton");
            }
        }
        
        public CommandToolStripButton ForceRebootToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("ForceRebootToolbarButton");
            }
        }

        public CommandToolStripButton BackButton
        {
            get
            {
                return GetField<CommandToolStripButton>("backButton");
            }
        }
        
        public CommandToolStripButton ForwardButton
        {
            get
            {
                return GetField<CommandToolStripButton>("forwardButton");
            }
        }
        
        public CommandToolStripButton NewPoolToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("AddPoolToolbarButton");
            }
        }
        
        public CommandToolStripButton NewStorageToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("newStorageToolbarButton");
            }
        }
        
        public CommandToolStripButton PowerOnHostToolStripButton
        {
            get
            {
                return GetField<CommandToolStripButton>("powerOnHostToolStripButton");
            }
        }
        
        public CommandToolStripButton RebootToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("RebootToolbarButton");
            }
        }
        
        public CommandToolStripButton ResumeToolStripButton
        {
            get
            {
                return GetField<CommandToolStripButton>("resumeToolStripButton");
            }
        }
        
        public CommandToolStripButton SuspendToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("SuspendToolbarButton");
            }
        }
        
        public CommandToolStripButton AlertsToolbarButton
        {
            get
            {
                return GetField<CommandToolStripButton>("AlertsToolbarButton");
            }
        }
    }
}
