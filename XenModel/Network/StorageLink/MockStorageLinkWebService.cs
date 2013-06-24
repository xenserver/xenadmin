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
using XenAPI;

namespace XenAdmin.Network.StorageLink
{
    public class MockStorageLinkWebService : IStorageLinkWebService
    {
        public readonly Dictionary<storageSystemInfo, storagePoolInfo[]> StorageSystemsAndPools = new Dictionary<storageSystemInfo, storagePoolInfo[]>();
        public readonly Dictionary<storagePoolInfo, storageVolumeInfo[]> StoragePoolsAndVolumes = new Dictionary<storagePoolInfo, storageVolumeInfo[]>();
        public readonly Dictionary<hostInfo, storageRepositoryInfo[]> HostsAndSRs = new Dictionary<hostInfo, storageRepositoryInfo[]>();
        public readonly List<storageAdapterInfo> StorageAdapters = new List<storageAdapterInfo>();
        public readonly List<@event> Events = new List<@event>();
        public bool IsDisposed { get; private set; }
        public event EventHandler StorageSystemsEnumerating;
        public event EventHandler PullEvents;
        public event EventHandler SetPassword;

        public MockStorageLinkWebService(string host, string username, string password)
        {
            Host = host;
            Username = username;
            Password = password;
        }

        public storageSystemInfo GetSampleStorageSystem()
        {
            storageSystemInfo ss = new storageSystemInfo();
            ss.objectId = "storageSystemId";
            ss.serialNum = "serialNum";
            ss.model = "model";
            ss.displayName = "displayname";
            ss.capabilities = 231;
            ss.friendlyName = "friendlyName";
            ss.storageSystemId = "storageSystemId";
            ss.storageAdapterId = "storageAdapterId";

            return ss;
        }

        public storagePoolInfo GetSampleStoragePool()
        {
            storagePoolInfo sp = new storagePoolInfo();
            sp.objectId = "storagePoolInfo";
            sp.friendlyName = "samplePool";
            sp.storageSystemId = "storageSystemId";
            sp.parentPool = "";
            return sp;
        }

        public storageVolumeInfo GetSampleStorageVolume()
        {
            var sv = new storageVolumeInfo();
            sv.displayName = "";
            sv.friendlyName = "";
            sv.serialNum = "serialNum";
            sv.sizeInMB = 5000;
            sv.storagePoolId = "storagePoolInfo";
            sv.storageSystemId = "storageSystemId";
            sv.storageVolumeId = "storageVolumeId";
            sv.usedSpaceInMB = 1000;
            sv.objectId = "storageVolumeId";
            return sv;
        }

        #region IStorageLinkWebService Members

        public @event[] pullEvents(string serverTimeStamp, int startingEventIndex, int maxEvents, out int lastEventIndex, out string currentServerTimeStamp)
        {
            if (PullEvents != null)
            {
                PullEvents(this, EventArgs.Empty);
            }

            lastEventIndex = 0;
            currentServerTimeStamp = "0";
            return Events.ToArray();
        }

        public jobInfo enumStorageRepositories(string hostGroupUuid, string filters, int flags, out storageRepositoryInfo[] list)
        {
            list = new storageRepositoryInfo[0];
            foreach (hostInfo hi in HostsAndSRs.Keys)
            {
                if (hi.hostGroupUuid == hostGroupUuid)
                {
                    list = HostsAndSRs[hi];
                    break;
                }
            }

            return null;
        }

        public jobInfo enumStoragePools(string storageSystemId, string filters, int flags, out storagePoolInfo[] spInfoList)
        {
            spInfoList = new storagePoolInfo[0];
            foreach (storageSystemInfo ss in StorageSystemsAndPools.Keys)
            {
                if (ss.objectId == storageSystemId)
                {
                    spInfoList = StorageSystemsAndPools[ss];
                }
            }
            return null;
        }

        public jobInfo enumStorageSystems(string filters, int flags, out storageSystemInfo[] storageSystemInfoList)
        {
            if (StorageSystemsEnumerating != null)
            {
                StorageSystemsEnumerating(this, EventArgs.Empty);
            }

            storageSystemInfoList = new List<storageSystemInfo>(StorageSystemsAndPools.Keys).ToArray();
            return null;
        }

        public jobInfo enumStorageNodes(string storageSystemId, string storagePoolId, string filters, int flags, out storageVolumeInfo[] snInfoList)
        {
            var output = new List<storageVolumeInfo>();

            foreach (storageSystemInfo ss in StorageSystemsAndPools.Keys)
            {
                if (ss.storageSystemId == storageSystemId)
                {
                    foreach (storagePoolInfo sp in StoragePoolsAndVolumes.Keys)
                    {
                        if (storagePoolId.Length == 0)
                        {
                            output.AddRange(StoragePoolsAndVolumes[sp]);
                        }
                        else if (sp.storagePoolId == storagePoolId)
                        {
                            snInfoList = new List<storageVolumeInfo>(StoragePoolsAndVolumes[sp]).ToArray();
                            return null;
                        }
                    }
                }
            }
            snInfoList = output.ToArray();
            return null;
        }

        private jobInfo GetCompletedJobInfo()
        {
            return new jobInfo()
            {
                jobId = "bla",
                progress = 100,
                name = new i18nString() { defaultMessage = "bla" },
                description = new i18nString() { defaultMessage = "bla" },
                completedDateStamp = DateTime.Now,
                jobStatus = (long)StorageLinkEnums.JobState.Successful
            };
        }

        public jobInfo addStorageManagementCredentials(managementCredentials cred, int flags, out managementCredentials info)
        {
            info = cred;
            StorageSystemsAndPools.Add(GetSampleStorageSystem(), new[] { GetSampleStoragePool() });
            Events.Add(new @event() { eventId = "event.object.storage-system.add.{bla}.{bla}" });

            return GetCompletedJobInfo();
        }

        public storageAdapterInfo[] enumStorageAdapters()
        {
            return StorageAdapters.ToArray();
        }

        public jobInfo cancelJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public jobInfo getJobInfo(string jobId, string locale, int flags)
        {
            return GetCompletedJobInfo();
        }

        public void setLicenseServerInfo(string server, int port)
        {
            throw new NotImplementedException();
        }

        public licenseServerInfo getLicenseServerInfo()
        {
            throw new NotImplementedException();
        }

        public managementCredentials[] enumStorageManagementCredentials()
        {
            throw new NotImplementedException();
        }

        public jobInfo removeStorageManagementCredentials(string friendlyName, string uuid, int flags)
        {
            throw new NotImplementedException();
        }

        public jobInfo addHost(string hostName, string username, string password, int hypervisorType, int licenseType, int flags, out hostInfo info)
        {
            throw new NotImplementedException();
        }

        public void setServicePassword(string username, string password, int flags)
        {
            if (SetPassword != null)
            {
                SetPassword(this, EventArgs.Empty);
            }
        }

        public storageRepositoryInfo addStorageToRepository(string uuid, string[] storageVolumeIdList, int flags)
        {
            throw new NotImplementedException();
        }

        public storageRepositoryInfo removeStorageFromRepository(string uuid, string[] storageVolumeIdList, int flags)
        {
            throw new NotImplementedException();
        }

        public string Host { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            IsDisposed = true;
        }

        #endregion
    }
}
