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
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Globalization;

namespace XenAdmin.Model
{
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
