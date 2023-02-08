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

using System;
using XenAPI;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace XenAdmin.Actions
{
    public enum SnapshotType { DISK, DISK_AND_MEMORY, QUIESCED_DISK };

    public class VMSnapshotCreateAction : AsyncAction
    {
        public const string VNC_SNAPSHOT = "XenCenter.VNCSnapshot";
        private readonly string _newName;
        private readonly string _newDescription;
        private readonly SnapshotType _type;
        /// <summary>
        /// 3 parameters: vm, username, password (username and password can be null)
        /// </summary>
        private readonly Func<VM, string, string, Image> _screenShotProvider; 

        public VMSnapshotCreateAction(VM vm, string newName, string newDescription,
            SnapshotType type, Func<VM, string, string, Image> screenShotProvider)
            : base(vm.Connection, string.Format(Messages.ACTION_VM_SNAPSHOT_TITLE, vm.Name()))
        {
            VM = vm;
            _newName = newName;
            _newDescription = newDescription;
            _type = type;
            _screenShotProvider = screenShotProvider;

            switch (type)
            {
                case SnapshotType.QUIESCED_DISK:
                    ApiMethodsToRoleCheck.Add("vm.async_snapshot_with_quiesce");
                    break;
                case SnapshotType.DISK_AND_MEMORY:
                    ApiMethodsToRoleCheck.Add("vm.async_checkpoint");
                    break;
                case SnapshotType.DISK:
                    ApiMethodsToRoleCheck.Add("vm.async_snapshot");
                    break;
            }

            ApiMethodsToRoleCheck.AddRange("vm.set_name_description", "vm.create_new_blob");
        }

        protected override void Run()
        {
            Description = Messages.SNAPSHOTTING;

            // Take screenshot before snapshot begins, to avoid possible console switching (CA-211369)
            Image screenshot = null;
            try
            {
                if (VM.power_state == vm_power_state.Running && _type == SnapshotType.DISK_AND_MEMORY)
                {
                    // use the sudo credentials for the screenshot (can be null) (CA-91132)
                    screenshot = _screenShotProvider(VM, sudoUsername, sudoPassword);
                }
            }
            catch
            {
                //Ignore; the screenshot is optional, we will do without it (CA-37095/CA-37103)
            }

            switch (_type)
            {
                case SnapshotType.QUIESCED_DISK:
                    RelatedTask = VM.async_snapshot_with_quiesce(Session, VM.opaque_ref, _newName);
                    break;
                case SnapshotType.DISK_AND_MEMORY:
                    RelatedTask = VM.async_checkpoint(Session, VM.opaque_ref, _newName);
                    break;
                default:
                    RelatedTask = VM.async_snapshot(Session, VM.opaque_ref, _newName);
                    break;
            }
            
            PollToCompletion();

            if (string.IsNullOrEmpty(Result))
                return;

            VM.set_name_description(Session, Result, _newDescription);
            SaveImageInBlob(Result, screenshot);

            Description = Messages.SNAPSHOTTED;
        }

        private void SaveImageInBlob(string newVmRef, Image image)
        {
            if (image == null)
                return;

            XenRef<Blob> blobRef = VM.create_new_blob(Session, newVmRef, VNC_SNAPSHOT, "image/jpeg", false);

            Blob blob = Connection.WaitForCache(blobRef);
            if (blob == null)
                return;

            using (MemoryStream saveStream = new MemoryStream())
            {
                image.Save(saveStream, ImageFormat.Jpeg);
                saveStream.Position = 0;
                blob.Save(saveStream, Session);
            }
        }
    }
}
