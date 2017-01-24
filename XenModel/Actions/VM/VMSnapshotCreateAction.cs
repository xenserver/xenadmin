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
using XenAdmin.Core;
using XenAPI;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;


namespace XenAdmin.Actions
{

    public enum SnapshotType { DISK, DISK_AND_MEMORY, QUIESCED_DISK };
    public class VMSnapshotCreateAction : AsyncAction
    {
        public const String VNC_SNAPSHOT = "XenCenter.VNCSnapshot";
        private readonly String NewName;
        private readonly string m_NewDescription;
        private readonly SnapshotType m_Type;
        private String taskResult = null;
        private Func<VM,String,String,Image> _screenShotProvider; // 3 parameters: vm, username, password (username and password can be null)
        public VMSnapshotCreateAction(VM vm, string NewName, string newDescription, SnapshotType type,Func<VM,String,String,Image> screenShotProvider )
            : base(vm.Connection, String.Format(Messages.ACTION_VM_SNAPSHOT_TITLE, vm.Name))
        {
            this.VM = vm;
            this.NewName = NewName;
            this.m_Type = type;
            _screenShotProvider = screenShotProvider;
            this.m_NewDescription = newDescription;
            switch (type)
            {
                case SnapshotType.QUIESCED_DISK:
                    ApiMethodsToRoleCheck.Add("vm.snapshot_with_quiesce");
                    break;
                case SnapshotType.DISK_AND_MEMORY:
                    ApiMethodsToRoleCheck.Add("vm.checkpoint");
                    break;
                case SnapshotType.DISK:
                    ApiMethodsToRoleCheck.Add("vm.snapshot");
                    break;
            }
            ApiMethodsToRoleCheck.Add("vm.set_name_description");
        }

        protected override void Run()
        {
            Description = Messages.SNAPSHOTTING;

            // Take screenshot before snapshot begins, to avoid possible console switching (CA-211369)
            // The screenshot is optional. If it throws an exception, we will do without it. CA-37095/CA-37103.
            Image snapshot = null;
            try
            {
                if (VM.power_state == vm_power_state.Running && m_Type == SnapshotType.DISK_AND_MEMORY)
                {
                    // use the sudo credentials for the snapshot (can be null) (CA-91132)
                    snapshot = _screenShotProvider(VM, sudoUsername, sudoPassword);
                }
            }
            catch (Exception)
            {
            }

            if (m_Type == SnapshotType.QUIESCED_DISK)
            {
                RelatedTask = XenAPI.VM.async_snapshot_with_quiesce(Session, VM.opaque_ref, NewName);
            }
            else if (m_Type == SnapshotType.DISK_AND_MEMORY)
            {
                RelatedTask = XenAPI.VM.async_checkpoint(Session, VM.opaque_ref, NewName);
            }
            else
            {
                RelatedTask = XenAPI.VM.async_snapshot(Session, VM.opaque_ref, NewName);
            }
            PollToCompletion();
            if (String.IsNullOrEmpty(taskResult) || !taskResult.StartsWith("<value>") || !taskResult.EndsWith("</value>"))
                return;

            string newVmRef = taskResult.Substring(7, taskResult.Length - 15);
            XenAPI.VM.set_name_description(Session, newVmRef, m_NewDescription);

            SaveImageInBlob(newVmRef, snapshot);

            Description = Messages.SNAPSHOTTED;
        }

        private void SaveImageInBlob(string newVmRef, Image image)
        {
            if (image == null)
                return;  // Discarded

            XenRef<Blob> blobRef = Helpers.TampaOrGreater(Session.Connection)
                                       ? XenAPI.VM.create_new_blob(Session, newVmRef, VNC_SNAPSHOT, "image/jpeg", false)
                                       : XenAPI.VM.create_new_blob(Session, newVmRef, VNC_SNAPSHOT, "image/jpeg");

            Blob blob = null;
            while ((blob = Connection.Resolve(blobRef)) == null) Thread.Sleep(1000);
            using (MemoryStream saveStream = new MemoryStream())
            {
                image.Save(saveStream, ImageFormat.Jpeg);
                saveStream.Position = 0;
                blob.Save(saveStream, Session);
            }
        }

        public override void DestroyTask()
        {
            taskResult = XenAPI.Task.get_result(Session, RelatedTask.opaque_ref);

            base.DestroyTask();
        }
    }
}
