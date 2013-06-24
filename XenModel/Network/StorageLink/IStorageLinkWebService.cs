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
using XenAdmin.Network.StorageLink.Service;

namespace XenAdmin.Network.StorageLink.Service
{
    public interface IStorageLinkWebService : IDisposable
    {
        @event[] pullEvents(string serverTimeStamp, int startingEventIndex, int maxEvents, out int lastEventIndex, out string currentServerTimeStamp);
        jobInfo enumStorageRepositories(string hostGroupUuid, string filters, int flags, out storageRepositoryInfo[] list);
        jobInfo enumStoragePools(string storageSystemId, string filters, int flags, out storagePoolInfo[] spInfoList);
        jobInfo enumStorageSystems(string filters, int flags, out storageSystemInfo[] storageSystemInfoList);
        jobInfo enumStorageNodes(string storageSystemId, string storagePoolId, string filters, int flags, out storageVolumeInfo[] snInfoList);
        jobInfo addStorageManagementCredentials(managementCredentials cred, int flags, out managementCredentials info);
        storageAdapterInfo[] enumStorageAdapters();
        jobInfo cancelJob(string jobId);
        jobInfo getJobInfo(string jobId, string locale, int flags);
        void setLicenseServerInfo(string server, int port);
        licenseServerInfo getLicenseServerInfo();
        managementCredentials[] enumStorageManagementCredentials();
        jobInfo removeStorageManagementCredentials(string friendlyName, string uuid, int flags);
        jobInfo addHost(string hostName, string username, string password, int hypervisorType, int licenseType, int flags, out hostInfo info);
        void setServicePassword(string username, string password, int flags);
        storageRepositoryInfo addStorageToRepository(string uuid, string[] storageVolumeIdList, int flags);
        storageRepositoryInfo removeStorageFromRepository(string uuid, string[] storageVolumeIdList, int flags);
        string Host { get; }
        string Username { get; }
        string Password { get; }
    }
}
