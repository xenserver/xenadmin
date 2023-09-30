﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class GetDiskSpaceRequirementsAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string updateName;
        private readonly long updateSize;
        private readonly Pool_patch currentPatch;
        private readonly DiskSpaceRequirements.OperationTypes operation = DiskSpaceRequirements.OperationTypes.upload;

        public DiskSpaceRequirements DiskSpaceRequirements { get; private set; }

        /// <summary>
        /// This constructor is used to calculate the disk space requirements for installing or uploading a single patch
        /// </summary>
        public GetDiskSpaceRequirementsAction(Host host, Pool_patch patch, bool suppressHistory)
            : this(host, patch.Name(), patch.size, suppressHistory)
        {
            currentPatch = patch;
        }

        /// <summary>
        /// This constructor is used to calculate the disk space when the required space is known only
        /// </summary>
        public GetDiskSpaceRequirementsAction(Host host, long size, bool suppressHistory, DiskSpaceRequirements.OperationTypes operation)
            : this(host, null, size, suppressHistory)
        {
            this.operation = operation;
        }

        /// <summary>
        /// This constructor is used to check disk space for installing or uploading an update of given size
        /// </summary>
        public GetDiskSpaceRequirementsAction(Host host, string updateName, long size, bool suppressHistory)
            : base(host.Connection, Messages.ACTION_GET_DISK_SPACE_REQUIREMENTS_TITLE, suppressHistory)
        {
            Host = host;
            this.updateName = updateName;
            updateSize = size; 
            ApiMethodsToRoleCheck.Add("host.call_plugin");
        }

        protected override void Run()
        {
            Description = string.Format(Messages.ACTION_GET_DISK_SPACE_REQUIREMENTS_DESCRIPTION, Host.Name());

            long requiredDiskSpace = updateSize;
            long availableDiskSpace = 0;

            try
            {
                var result = Host.call_plugin(Session, Host.opaque_ref, "disk-space", "get_avail_host_disk_space", new Dictionary<string, string>());
                availableDiskSpace = Convert.ToInt64(result);
            }
            catch (Failure failure)
            {
                log.WarnFormat("Plugin call disk-space.get_avail_host_disk_space on {0} failed with {1}", Host.Name(), failure.Message);
            }

            // get reclaimable disk space (excluding current patch)
            long reclaimableDiskSpace = 0;

            DiskSpaceRequirements = new DiskSpaceRequirements(operation, Host, updateName, requiredDiskSpace, availableDiskSpace, reclaimableDiskSpace);

            if (availableDiskSpace < requiredDiskSpace)
                log.WarnFormat("Cleanup message: \r\n{0}", DiskSpaceRequirements.GetSpaceRequirementsMessage());
        }
    }


    public class DiskSpaceRequirements
    {
        public readonly OperationTypes Operation;
        public readonly Host Host;
        public readonly string UpdateName;
        public readonly long RequiredDiskSpace;
        public readonly long AvailableDiskSpace;
        public readonly long ReclaimableDiskSpace;

        public enum OperationTypes { install, upload, automatedUpdates, automatedUpdatesUploadOne, automatedUpdatesUploadAll }
      
        public DiskSpaceRequirements(OperationTypes operation, Host host, string updateName, long requiredDiskSpace, long availableDiskSpace, long reclaimableDiskSpace)
        {
            Operation = operation;
            Host = host;
            UpdateName = updateName;
            RequiredDiskSpace = requiredDiskSpace;
            AvailableDiskSpace = availableDiskSpace;
            ReclaimableDiskSpace = reclaimableDiskSpace;
        }

        public bool CanCleanup
        {
            get { return ReclaimableDiskSpace + AvailableDiskSpace > RequiredDiskSpace && RequiredDiskSpace > 0 && ReclaimableDiskSpace > 0; }
        }

        public string GetSpaceRequirementsMessage()
        {
            StringBuilder sbMessage = new StringBuilder();
            var pool = Helpers.GetPool(Host.Connection);

            switch (Operation)
            {
                case OperationTypes.install :
                    sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_INSTALL, Host.Name(), UpdateName);
                    break;
                case OperationTypes.upload :
                    sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_UPLOAD, Host.Name(), UpdateName);
                    break;
                case OperationTypes.automatedUpdates :
                    sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_AUTO_UPDATE, Host.Name());
                    break;
                case OperationTypes.automatedUpdatesUploadOne:
                    sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_AUTO_UPDATE_UPLOAD_ONE, pool != null ? pool.Name() : Host.Name());
                    break;
                case OperationTypes.automatedUpdatesUploadAll:
                    sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_AUTO_UPDATE_UPLOAD_ALL, pool != null ? pool.Name() : Host.Name());
                    break;
            }

            sbMessage.AppendLine();
            sbMessage.AppendLine();
            if (RequiredDiskSpace > 0)
            {
                sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_REQUIRED_SPACE, Util.DiskSizeString(RequiredDiskSpace));
                sbMessage.AppendLine();
            }
            sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_AVAILABLE_SPACE, Util.DiskSizeString(AvailableDiskSpace));
            sbMessage.AppendLine();
            sbMessage.AppendLine();
            if (CanCleanup)
                sbMessage.AppendFormat(Messages.NOT_ENOUGH_SPACE_MESSAGE_CLEANUP, BrandManager.BrandConsole,
                    Util.DiskSizeString(ReclaimableDiskSpace));
            else
                sbMessage.AppendLine(Messages.NOT_ENOUGH_SPACE_MESSAGE_NOCLEANUP);
            return sbMessage.ToString();
        }

        public string GetMessageForActionLink()
        {
            return CanCleanup ? Messages.PATCHINGWIZARD_CLEANUP : Messages.MORE_INFO;
        }
    }
}
