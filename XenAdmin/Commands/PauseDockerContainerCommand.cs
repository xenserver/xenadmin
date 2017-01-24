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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Model;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Commands
{
    internal class PauseDockerContainerCommand : Command
    {
        public PauseDockerContainerCommand()
        { }

        public PauseDockerContainerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public override string MenuText { get { return Messages.MAINWINDOW_PAUSE; } }
        
        public override Image MenuImage { get { return Images.StaticImages._000_paused_h32bit_16; } }

        public override Image ToolBarImage { get { return Images.StaticImages._000_Paused_h32bit_24; } }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.AllItemsAre<DockerContainer>())
                return selection.AtLeastOneXenObjectCan<DockerContainer>(CanExecute);
            return false;
        }

        private static bool CanExecute(DockerContainer dockerContainer)
        {
            return dockerContainer.power_state == vm_power_state.Running && !dockerContainer.Parent.IsWindows;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var dockerContainers = new List<DockerContainer>();

            if (selection.AllItemsAre<DockerContainer>())
            {
                dockerContainers = (from IXenObject obj in selection.AsXenObjects()
                                    let container = (DockerContainer)obj
                                    where CanExecute(container)
                                    select container).ToList();
            }

            foreach (var container in dockerContainers)
                (new PauseDockerContainerAction(container)).RunAsync();
        }
    }
}
