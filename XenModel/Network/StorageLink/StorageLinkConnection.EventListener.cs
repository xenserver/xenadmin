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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using XenAdmin.Network.StorageLink.Service;
using XenAPI;

namespace XenAdmin.Network.StorageLink
{
    partial class StorageLinkConnection
    {
        private class EventListener
        {
            private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            private readonly StorageLinkConnection _connection;
            private int _pullEventsIndex = 0;
            private string _pullEventsTimestamp = "0";

            public EventListener(StorageLinkConnection connection)
            {
                _connection = connection;
            }

            private @event[] PullEvents()
            {
                try
                {
                    return _connection._service.pullEvents(_pullEventsTimestamp, _pullEventsIndex, 10000, out _pullEventsIndex, out _pullEventsTimestamp);
                }
                catch (SoapException e)
                {
                    if (e.Message.Contains("Error 47"))
                    {
                        log.Warn("Received error from SL pull events", e);
                        _pullEventsIndex = 0;
                        _pullEventsTimestamp = "0";
                    }
                    else
                    {
                        throw;
                    }
                }
                return new @event[0];
            }

            public void ProcessEvents()
            {
                var systemsToRefresh = new List<string>();
                var hostGroupsToRefresh = new List<string>();

                foreach (@event ev in PullEvents())
                {
                    string[] evParams = ParseEventId(ev.eventId);

                    if (evParams.Length > 5 && evParams[2] == "storage-repository" && !hostGroupsToRefresh.Contains(evParams[5]))
                    {
                        hostGroupsToRefresh.Add(evParams[5]);
                        RefreshStorageRepositories(evParams[5]);
                    }
                    else if (evParams.Length > 5 && evParams[2] == "storage-pool" && !systemsToRefresh.Contains(evParams[5]))
                    {
                        systemsToRefresh.Add(evParams[5]);
                        RefreshStorageLinkSystem(evParams[5]);
                    }
                    else if (evParams.Length > 5 && evParams[2] == "storage-node" && !systemsToRefresh.Contains(evParams[5]))
                    {
                        systemsToRefresh.Add(evParams[5]);
                        RefreshStorageLinkSystem(evParams[5]);
                    }
                    else if (evParams.Length > 5 && evParams[2] == "storage-system")
                    {
                        _connection.Update();
                        break;
                    }
                }
            }

            private string[] ParseEventId(string eventId)
            {
                string[] ss = Regex.Split(eventId, @"\.");
                for (int i = 0; i < ss.Length; i++)
                {
                    if (ss[i].StartsWith("{") && ss[i].EndsWith("}"))
                    {
                        ss[i] = ss[i].Substring(1, ss[i].Length - 2);
                    }
                }
                return ss;
            }

            private void RefreshStorageRepositories(string hostGroupUuid)
            {
                StorageLinkServer server = _connection.Cache.Server;

                if (server != null)
                {
                    storageRepositoryInfo[] srs;
                    _connection._service.enumStorageRepositories(hostGroupUuid, string.Empty, (int)StorageLinkEnums.FlagsType.NONE, out srs);

                    var repositories = new List<StorageLinkRepository>();
                    var systems = new List<StorageLinkSystem>(_connection.Cache.StorageSystems);
                    var pools = new List<StorageLinkPool>(_connection.Cache.StoragePools);

                    foreach (storageRepositoryInfo sr in srs)
                    {
                        if (systems.Find(s => s.opaque_ref == sr.storageSystemId) != null && pools.Find(p => p.opaque_ref == sr.storagePoolId) != null)
                        {
                            repositories.Add(new StorageLinkRepository(_connection,
                                sr.objectId,
                                sr.friendlyName,
                                server,
                                sr.storageSystemId,
                                sr.storagePoolId,
                                (StorageLinkEnums.RaidType)sr.raidType,
                                (StorageLinkEnums.ProvisioningType)sr.provisioningType,
                                (StorageLinkEnums.ProvisioningOptions)sr.useDeduplication,
                                hostGroupUuid));
                        }
                    }

                    _connection.Invoke(() => _connection.Cache.Update(hostGroupUuid, repositories));
                }
            }

            private void RefreshStorageLinkSystem(string storageSystemId)
            {
                StorageLinkSystem system = new List<StorageLinkSystem>(_connection.Cache.StorageSystems).Find(s => s.opaque_ref == storageSystemId);

                if (system != null)
                {
                    storagePoolInfo[] storagePools = new storagePoolInfo[0];
                    storageVolumeInfo[] storageVolumes = new storageVolumeInfo[0];

                    try
                    {
                        _connection._service.enumStoragePools(storageSystemId, string.Empty, (int)StorageLinkEnums.FlagsType.NONE, out storagePools);
                        _connection._service.enumStorageNodes(storageSystemId, string.Empty, string.Empty, (int)StorageLinkEnums.FlagsType.NONE, out storageVolumes);
                    }
                    catch (SoapException e)
                    {
                        // if the creds have just been removed then don't throw the exception on.
                        if (e.Detail == null || e.Detail.InnerText.IndexOf("SL_MSG_ERR_NO_STORAGE_MANAGEMENT_CREDENTIALS_COULD_BE_FOUND_FOR_STORAGE_SYSTEM") < 0)
                        {
                            throw;
                        }
                    }

                    List<StorageLinkPool> pools = new List<StorageLinkPool>();
                    List<StorageLinkVolume> volumes = new List<StorageLinkVolume>();

                    foreach (storagePoolInfo poolInfo in storagePools)
                    {
                        pools.Add(new StorageLinkPool(_connection, poolInfo.objectId, poolInfo.friendlyName, poolInfo.parentPool, poolInfo.storageSystemId, poolInfo.sizeInMB, poolInfo.sizeInMB - poolInfo.freeSpaceInMB, (StorageLinkEnums.RaidType)poolInfo.supportedRaidTypes, (StorageLinkEnums.ProvisioningType)poolInfo.supportedProvisioningTypes));
                    }

                    foreach (storageVolumeInfo volumeInfo in storageVolumes)
                    {
                        volumes.Add(new StorageLinkVolume(_connection,
                            volumeInfo.objectId,
                            system.StorageLinkServer,
                            volumeInfo.friendlyName,
                            volumeInfo.storageSystemId,
                            volumeInfo.storagePoolId,
                            volumeInfo.sizeInMB,
                            volumeInfo.usedSpaceInMB));
                    }

                    _connection.Invoke(() => _connection.Cache.Update(storageSystemId, pools, volumes));
                }
            }
        }
    }
}
