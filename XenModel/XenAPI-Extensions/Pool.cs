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
using System.Linq;
using System.Net;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;
using System.Globalization;

namespace XenAPI
{
    public partial class Pool : IComparable<Pool>, IEquatable<Pool>
    {
        public override string ToString()
        {
            return Name;
        }

        public override string Description
        {
            get
            {
                return name_description;
            }
        }


        public override string Name
        {
            get
            {
                if (name_label != "")
                    return name_label;

                if (Connection == null)
                    return string.Empty;
                
                Host master = Connection.Resolve(this.master);
                return master == null ? "" : master.Name;
            }
        }

        internal override string LocationString
        {
            get
            {
                return string.Empty;
            }
        }

        public List<SR> GetHAHeartbeatSRs()
        {
            List<SR> result = new List<SR>();

            foreach (string opaqueRef in ha_statefiles)
            {
                XenRef<VDI> vdiRef = new XenRef<VDI>(opaqueRef);
                VDI vdi = Connection.Resolve(vdiRef);
                if (vdi == null)
                    continue;
                SR sr = vdi.Connection.Resolve(vdi.SR);
                if (sr != null)
                    result.Add(sr);
            }

            return result;
        }

        public bool IsMasterUpgraded
        {
            get
            {
                Host master = Helpers.GetMaster(this);
                foreach (var host in this.Connection.Cache.Hosts)
                {
                    if (host.LongProductVersion != master.LongProductVersion)
                        return true;
                }
                return false;
            }
        }

        public string LicenseString
        {
            get
            {
                if (Helpers.MidnightRideOrGreater(Connection))
                {
                    var hosts = new List<Host>(Connection.Cache.Hosts);
                    foreach (Host.Edition edition in Enum.GetValues(typeof(Host.Edition)))
                    {
                        Host.Edition edition1 = edition;
                        Host host = hosts.Find(h => h.edition == Host.GetEditionText(edition1));

                        if (host != null)
                        {
                            return Helpers.GetFriendlyLicenseName(host);
                        }
                    }
                    return PropertyManager.GetFriendlyName("Label-host.edition-free");
                }
                else
                {
                    Host lowestLicenseHost = null;
                    string lowestLicense = "";
                    foreach (Host host in this.Connection.Cache.Hosts)
                    {
                        string sku = host.license_params.ContainsKey("sku_type") ? host.license_params["sku_type"].Replace(" ", "_").ToLowerInvariant() : "xe_express";

                        //Express
                        if (sku.EndsWith("xe_express"))
                        {
                            lowestLicenseHost = host;
                            lowestLicense = sku;
                            break;
                        }

                        // Server
                        if (sku.EndsWith("xe_server") && lowestLicense.EndsWith("xe_enterprise"))
                        {
                            lowestLicenseHost = host;
                            lowestLicense = sku;
                        }

                        // Enterprise
                        if (lowestLicense == "")
                        {
                            lowestLicenseHost = host;
                            lowestLicense = sku;
                        }
                    }
                    if (lowestLicense.EndsWith("xe_express"))
                    {
                        return PropertyManager.GetFriendlyName("Label-host.sku_type-FG-xe_express");
                    }
                    else
                    {
                        return Helpers.GetFriendlyLicenseName(lowestLicenseHost);
                    }
                }
            }
        }
        /// <summary>
        /// Determine whether the given pool is a visible pool, i.e. not a pool-of-one.
        /// </summary>
        /// <param name="ThePool"></param>
        /// <returns></returns>
        public bool IsVisible
        {
            get
            {
                return Connection != null && (name_label != "" || Connection.Cache.HostCount > 1);
            }
        }

        private const String ROLLING_UPGRADE_IN_PROGRESS = "rolling_upgrade_in_progress";

        public bool RollingUpgrade
        {
            get
            {
                return other_config != null && other_config.ContainsKey(ROLLING_UPGRADE_IN_PROGRESS);
            }
        }

        public static Dictionary<string, string> retrieve_wlb_default_configuration(Session session)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_retrieve_wlb_configuration("default").parse());
        }

        public string GetXCPluginSecret(string plugin_name, IXenObject obj)
        {
            return Get(gui_config, XCPluginSecretName(plugin_name, obj));
        }

        public void RemoveXCPluginSecret(Session session, string plugin_name, IXenObject obj)
        {
            Pool.remove_from_gui_config(session, opaque_ref, XCPluginSecretName(plugin_name, obj));
        }

        public void SetXCPluginSecret(Session session, string plugin_name, IXenObject obj, string secret_uuid)
        {
            string n = XCPluginSecretName(plugin_name, obj);
            Pool.remove_from_gui_config(session, opaque_ref, n);
            Pool.add_to_gui_config(session, opaque_ref, n, secret_uuid);
        }

        private string XCPluginSecretName(string plugin_name, IXenObject obj)
        {
            return string.Format("XC_PLUGIN_SECRET_{0}_{1}_{2}", obj.Connection.Username, plugin_name, Helpers.GetUuid(obj));
        }

        public void ClearStorageLinkCredentials()
        {
            var otherConfig = new Dictionary<string, string>(other_config);

            otherConfig.Remove("storagelink_host");
            otherConfig.Remove("storagelink_user");
            otherConfig.Remove("storagelink_password_secret");
            Pool.set_other_config(Connection.Session, opaque_ref, otherConfig);
        }

        public void SetStorageLinkCredentials(string host, string username, string password)
        {
            var otherConfig = new Dictionary<string, string>(other_config);

            if (host == null)
            {
                otherConfig.Remove("storagelink_host");
            }
            else
            {
                otherConfig["storagelink_host"] = host;
            }

            if (username == null)
            {
                otherConfig.Remove("storagelink_user");
            }
            else
            {
                otherConfig["storagelink_user"] = username;
            }

            if (password == null)
            {
                otherConfig.Remove("storagelink_password_secret");
            }
            else if (otherConfig.ContainsKey("storagelink_password_secret"))
            {
                try
                {
                    string secretRef = Secret.get_by_uuid(Connection.Session, otherConfig["storagelink_password_secret"]);
                    Secret.set_value(Connection.Session, secretRef, password);
                }
                catch (Failure)
                {
                    otherConfig["storagelink_password_secret"] = Secret.CreateSecret(Connection.Session, password);
                }
                catch (WebException)
                {
                    otherConfig["storagelink_password_secret"] = Secret.CreateSecret(Connection.Session, password);
                }
            }
            else
            {
                otherConfig["storagelink_password_secret"] = Secret.CreateSecret(Connection.Session, password);
            }

            Pool.set_other_config(Connection.Session, opaque_ref, otherConfig);
        }

        public string StorageLinkHost
        {
            get
            {
                string host;
                other_config.TryGetValue("storagelink_host", out host);
                return host;
            }
        }

        public string StorageLinkUsername
        {
            get
            {
                string user;
                other_config.TryGetValue("storagelink_user", out user);
                return user;
            }
        }

        public StorageLinkCredentials GetStorageLinkCredentials()
        {
            if (other_config != null && Helpers.MidnightRideOrGreater(Connection) && !Helpers.BostonOrGreater(Connection))
            {
                var otherConfig = new Dictionary<string, string>(other_config);
                string host, user, passwordSecret;
                otherConfig.TryGetValue("storagelink_host", out host);
                otherConfig.TryGetValue("storagelink_user", out user);
                otherConfig.TryGetValue("storagelink_password_secret", out passwordSecret);

                return new StorageLinkCredentials(Connection, host, user, null, passwordSecret);
            }
            return null;
        }

        // Whether the vSwitch Controller appears to be configured.
        // (Note that we can't tell whether it's actually working properly through the API).
        public bool vSwitchController
        {
            get
            {
                if (string.IsNullOrEmpty(vswitch_controller))
                    return false;

                foreach (Host h in Connection.Cache.Hosts)
                {
                    if (!Helpers.CowleyOrGreater(h) ||
                        Host.RestrictVSwitchController(h) ||
                        !h.software_version.ContainsKey("network_backend") ||
                        h.software_version["network_backend"] != "openvswitch")
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public List<XenAPI.Host> HostsToUpgrade
        {
            get
            {
                //First one to upgrade has to be the master
                var master = Helpers.GetMaster(Connection);

                List<XenAPI.Host> result = IsMasterUpgraded
                               ? Connection.Cache.Hosts.Where(host => host.LongProductVersion != master.LongProductVersion).ToList()
                               : Connection.Cache.Hosts.ToList();
                result.Sort();
                
                return result;
            }
        }

        public bool IsPoolFullyUpgraded
        {
            get
            {
                Host master = Helpers.GetMaster(this);

                foreach (var host in this.Connection.Cache.Hosts)
                {
                    if (host.LongProductVersion != master.LongProductVersion)
                        return false;
                }
                return true;
            }
        }

        public Host SmallerVersionHost
        {
            get
            {
                Host hostWithSmallerVersion = null;
                foreach (var host in this.Connection.Cache.Hosts)
                {
                    if (hostWithSmallerVersion == null)
                        hostWithSmallerVersion = host;
                    else if (Helpers.productVersionCompare(hostWithSmallerVersion.ProductVersion, host.ProductVersion) > 0)
                        hostWithSmallerVersion = host;
                }
                return hostWithSmallerVersion;
            }
        }

        public virtual int CpuSockets
        {
            get { return Connection.Cache.Hosts.Sum(h => h.CpuSockets); }
        }

        public bool HasGpu
        {
            get { return Connection.Cache.PGPUs.Length > 0; }
        }

        public bool HasVGpu
        {
            get { return HasGpu && Connection.Cache.PGPUs.Any(pGpu => pGpu.HasVGpu); }
        }

        #region Health Check settings
        public HealthCheckSettings HealthCheckSettings
        {
            get 
            {
               return new HealthCheckSettings(health_check_config);
            }
        }
        #endregion

        #region IEquatable<Pool> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object. This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(Pool other)
        {
            return base.Equals(other);
        }

        #endregion
    }


    public enum HealthCheckStatus
    {
        Disabled, Enabled, Undefined
    }

    public class HealthCheckSettings
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HealthCheckStatus Status;
        public int IntervalInDays, TimeOfDay, RetryInterval;
        public DayOfWeek DayOfWeek;
        public string UploadTokenSecretUuid;
        public string NewUploadRequest;
        public string UserNameSecretUuid;
        public string PasswordSecretUuid;
        public string LastSuccessfulUpload;
        
        public const int DefaultRetryInterval = 7; // in days
        public const int UploadRequestValidityInterval = 30; // in minutes

        public const string STATUS = "Enrollment";
        public const string INTERVAL_IN_DAYS = "Schedule.IntervalInDays";
        public const int intervalInDaysDefault = 14;
        public const string DAY_OF_WEEK = "Schedule.DayOfWeek";
        public const string TIME_OF_DAY = "Schedule.TimeOfDay";
        public const string RETRY_INTERVAL = "Schedule.RetryInterval";
        public const int RetryIntervalDefault = 7;
        public const string UPLOAD_TOKEN_SECRET = "UploadToken.Secret";
        public const string UPLOAD_CREDENTIAL_USER_SECRET = "User.Secret";
        public const string UPLOAD_CREDENTIAL_PASSWORD_SECRET = "Password.Secret";
        public const string UPLOAD_LOCK = "UploadLock";
        public const string LAST_SUCCESSFUL_UPLOAD = "LastSuccessfulUpload";
        public const string LAST_FAILED_UPLOAD = "LastFailedUpload";
        public const string NEW_UPLOAD_REQUEST = "NewUploadRequest";
        public const string HEALTH_CHECK_PIPE = "HealthCheckServicePipe";
        public const string HEALTH_CHECK_PIPE_END_MESSAGE = "HealthCheckServicePipe";
        public const string UPLOAD_UUID = "UploadUuid";

        public HealthCheckSettings(HealthCheckStatus status, int intervalInDays, DayOfWeek dayOfWeek, int timeOfDay, int retryInterval)
        {
            Status = status;
            IntervalInDays = intervalInDays;
            DayOfWeek = dayOfWeek;
            TimeOfDay = timeOfDay;
            RetryInterval = retryInterval;
        }

        public HealthCheckSettings(Dictionary<string, string> config)
        {
            Status = config == null || !config.ContainsKey(STATUS)
                           ? HealthCheckStatus.Undefined
                           : (BoolKey(config, STATUS) ? HealthCheckStatus.Enabled : HealthCheckStatus.Disabled);
            IntervalInDays = IntKey(config, INTERVAL_IN_DAYS, intervalInDaysDefault);
            if (!Enum.TryParse(Get(config, DAY_OF_WEEK), out DayOfWeek))
                DayOfWeek = (DayOfWeek) GetDefaultDay();
            TimeOfDay = IntKey(config, TIME_OF_DAY, GetDefaultTime());
            RetryInterval = IntKey(config, RETRY_INTERVAL, RetryIntervalDefault);
            UploadTokenSecretUuid = Get(config, UPLOAD_TOKEN_SECRET);
            NewUploadRequest = Get(config, NEW_UPLOAD_REQUEST);
            UserNameSecretUuid = Get(config, UPLOAD_CREDENTIAL_USER_SECRET);
            PasswordSecretUuid = Get(config, UPLOAD_CREDENTIAL_PASSWORD_SECRET);
            LastSuccessfulUpload = Get(config, LAST_SUCCESSFUL_UPLOAD);
        }

        public Dictionary<string, string> ToDictionary(Dictionary<string, string> baseDictionary)
        {
            var newConfig = baseDictionary == null ? new Dictionary<string, string>() : new Dictionary<string, string>(baseDictionary);
            newConfig[STATUS] = Status == HealthCheckStatus.Enabled ? "true" : "false";
            newConfig[INTERVAL_IN_DAYS] = IntervalInDays.ToString();
            var day = (int) DayOfWeek;
            newConfig[DAY_OF_WEEK] = day.ToString();
            newConfig[TIME_OF_DAY] = TimeOfDay.ToString();
            newConfig[RETRY_INTERVAL] = RetryInterval.ToString();
            newConfig[UPLOAD_TOKEN_SECRET] = UploadTokenSecretUuid;
            newConfig[NEW_UPLOAD_REQUEST] = NewUploadRequest;
            return newConfig;
        }

        public int IntervalInWeeks { get { return IntervalInDays / 7; } }

        public string StatusDescription
        {
            get
            {
                return Status == HealthCheckStatus.Enabled
                           ? Messages.HEALTHCHECK_STATUS_NOT_AVAILABLE_YET
                           : Messages.HEALTHCHECK_STATUS_NOT_ENROLLED;
            }
        }

        public bool CanRequestNewUpload
        {
            get
            {
                if (Status != HealthCheckStatus.Enabled)
                    return false;
                var uploadRequestExpiryTime = NewUploadRequestTime.AddMinutes(UploadRequestValidityInterval);
                return DateTime.Compare(uploadRequestExpiryTime, DateTime.UtcNow) < 0;
            }
        }

        public DateTime NewUploadRequestTime
        {
            get
            {
                if (!string.IsNullOrEmpty(NewUploadRequest))
                {
                    try
                    {
                        return StringToDateTime(NewUploadRequest);
                    }
                    catch (Exception exn)
                    {
                        log.Error("Exception while parsing NewUploadRequest", exn);
                    }
                }
                return DateTime.MinValue;
            }
        }

        #region Helper functions
        private static T Get<T>(Dictionary<string, T> d, string k) where T : class
        {
            return d != null && d.ContainsKey(k) ? d[k] : null;
        }

        private static bool BoolKey(Dictionary<string, string> d, string k)
        {
            string v = Get(d, k);
            return v == null ? false : v == "true";
        }

        private static int IntKey(Dictionary<string, string> d, string k, int def)
        {
            int result;
            string s = Get(d, k);
            return s != null && int.TryParse(s, out result) ? result : def;
        }

        private static int GetDefaultDay()
        {
            return new Random().Next(1, 7);
        }

        public static int GetDefaultTime()
        {
            return new Random().Next(1, 5);
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            // Round-trip format time
            DateTime rtime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return rtime.ToString("o");
        }

        public static DateTime StringToDateTime(string dateTimeString)
        {
            // Round-trip format time
            DateTime dateTime = DateTime.ParseExact(dateTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return dateTime;
        }

        public static bool TryParseStringToDateTime(string dateTimeString, out DateTime dateTime)
        {
            // Round-trip format time
            return DateTime.TryParseExact(dateTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime);
        }

        #endregion

        public string GetSecretyInfo(IXenConnection connection, string secretType)
        {
            string UUID = string.Empty;
            switch (secretType)
            {
                case HealthCheckSettings.UPLOAD_CREDENTIAL_USER_SECRET:
                    UUID = UserNameSecretUuid;
                    break;
                case HealthCheckSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET:
                    UUID = PasswordSecretUuid;
                    break;
                case HealthCheckSettings.UPLOAD_TOKEN_SECRET:
                    UUID = UploadTokenSecretUuid;
                    break;
                default:
                    log.ErrorFormat("Error getting the {0} from the xapi secret", secretType);
                    break;
            }

            if (connection == null || string.IsNullOrEmpty(UUID))
                return null;
            try
            {
                string opaqueref = Secret.get_by_uuid(connection.Session, UUID);
                return Secret.get_value(connection.Session, opaqueref);
            }
            catch (Exception e)
            {
                log.Error("Exception getting the upload token from the xapi secret", e);
                return null;
            }
        }

        public string GetExistingSecretyInfo(IXenConnection connection, string secretType)
        {
            if (connection == null)
                return null;

            string token = GetSecretyInfo(connection, secretType);

            if (string.IsNullOrEmpty(token))
                token = GetSecretyInfoFromOtherConnections(connection, secretType);

            return token;
        }

        private static string GetSecretyInfoFromOtherConnections(IXenConnection currentConnection, string secretType)
        {
            foreach (var connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (connection == currentConnection || !connection.IsConnected)
                    continue;
                var poolOfOne = Helpers.GetPoolOfOne(connection);
                if (poolOfOne != null)
                {
                    var token = poolOfOne.HealthCheckSettings.GetSecretyInfo(connection, secretType);
                    if (!string.IsNullOrEmpty(token))
                        return token;
                }
            }
            return null;
        }
    }
}
