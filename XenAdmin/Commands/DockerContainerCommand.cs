/* Copyright (c) Cloud Software Group, Inc. 
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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.Commands
{
    internal abstract class DockerContainerCommand : Command
    {
        protected DockerContainerCommand()
        { }

        protected DockerContainerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<DockerContainer>(i => !Helpers.StockholmOrGreater(i.Connection)) &&
                   selection.AtLeastOneXenObjectCan<DockerContainer>(CanRunForContainer);
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var dockerContainers = new List<DockerContainer>();

            if (selection.AllItemsAre<DockerContainer>())
            {
                dockerContainers = (from IXenObject obj in selection.AsXenObjects()
                    let container = (DockerContainer)obj
                    where CanRunForContainer(container)
                    select container).ToList();
            }

            foreach (var container in dockerContainers)
                CreateAction(container).RunAsync();
        }

        protected abstract bool CanRunForContainer(DockerContainer dockerContainer);

        protected abstract AsyncAction CreateAction(DockerContainer dockerContainer);
    }


    internal class PauseDockerContainerCommand : DockerContainerCommand
    {
        public PauseDockerContainerCommand()
        { }

        public PauseDockerContainerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText => Messages.MAINWINDOW_PAUSE;

        public override Image MenuImage => Images.StaticImages._000_paused_h32bit_16;

        public override Image ToolBarImage => Images.StaticImages._000_Paused_h32bit_24;

        protected override bool CanRunForContainer(DockerContainer dockerContainer)
        {
            return dockerContainer.power_state == vm_power_state.Running && !dockerContainer.Parent.IsWindows();
        }

        protected override AsyncAction CreateAction(DockerContainer dockerContainer)
        {
            return new PauseDockerContainerAction(dockerContainer);
        }
    }


    internal class RestartDockerContainerCommand : DockerContainerCommand
    {
        public RestartDockerContainerCommand()
        { }

        public RestartDockerContainerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText => Messages.MAINWINDOW_RESTART;

        public override Image MenuImage => Images.StaticImages._001_Reboot_h32bit_16;

        public override Image ToolBarImage => Images.StaticImages._001_Reboot_h32bit_24;

        protected override bool CanRunForContainer(DockerContainer dockerContainer)
        {
            return dockerContainer.power_state == vm_power_state.Running;
        }

        protected override AsyncAction CreateAction(DockerContainer dockerContainer)
        {
            return new RestartDockerContainerAction(dockerContainer);
        }
    }


    internal class ResumeDockerContainerCommand : DockerContainerCommand
    {
        public ResumeDockerContainerCommand()
        { }

        public ResumeDockerContainerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText => Messages.MAINWINDOW_RESUME;

        public override Image MenuImage => Images.StaticImages._000_Resumed_h32bit_16;

        public override Image ToolBarImage => Images.StaticImages._000_Resumed_h32bit_24;

        protected override bool CanRunForContainer(DockerContainer dockerContainer)
        {
            return dockerContainer.power_state == vm_power_state.Paused;
        }

        protected override AsyncAction CreateAction(DockerContainer dockerContainer)
        {
            return new ResumeDockerContainerAction(dockerContainer);
        }
    }


    internal class StartDockerContainerCommand : DockerContainerCommand
    {
        public StartDockerContainerCommand()
        { }

        public StartDockerContainerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText => Messages.MAINWINDOW_START;

        public override Image MenuImage => Images.StaticImages._001_PowerOn_h32bit_16;

        public override Image ToolBarImage => Images.StaticImages._001_PowerOn_h32bit_24;

        protected override bool CanRunForContainer(DockerContainer dockerContainer)
        {
            return dockerContainer.power_state == vm_power_state.Halted;
        }

        protected override AsyncAction CreateAction(DockerContainer dockerContainer)
        {
            return new StartDockerContainerAction(dockerContainer);
        }
    }


    internal class StopDockerContainerCommand : DockerContainerCommand
    {
        public StopDockerContainerCommand()
        { }

        public StopDockerContainerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText => Messages.MAINWINDOW_STOP;

        public override Image MenuImage => Images.StaticImages._001_ShutDown_h32bit_16;

        public override Image ToolBarImage => Images.StaticImages._001_ShutDown_h32bit_24;

        protected override bool CanRunForContainer(DockerContainer dockerContainer)
        {
            return dockerContainer.power_state == vm_power_state.Running;
        }

        protected override AsyncAction CreateAction(DockerContainer dockerContainer)
        {
            return new StopDockerContainerAction(dockerContainer);
        }
    }
}
