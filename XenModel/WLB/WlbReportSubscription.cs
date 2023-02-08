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
using System.Collections.Generic;

namespace XenAdmin.Wlb
{
    public class WlbReportSubscription : WlbConfigurationBase
    {
        #region  Variables

        private const string SUB_ID_KEY = "id";
        private const string CREATED_KEY = "created";
        private const string SUB_NAME_KEY = "name";
        private const string SUB_DESCRIPTION_KEY = "description";
        private const string SUBSCRIBER_ID_KEY = "subscriberId";
        private const string SUBSCRIBER_NAME_KEY = "subscriberName";
        private const string SCHEDULE_ID_KEY = "scheduleId";
        private const string DAYOFWEEK_KEY = "daysOfWeek";
        private const string EXECUTE_TIMEOFDAY_KEY = "executeTimeOfDay";
        private const string TRIGGER_TYPE_KEY = "triggerType";
        private const string ENABLED_KEY = "enabled";
        private const string ENABLE_DATE_KEY = "enableDate";
        private const string DISABLE_DATE_KEY = "disableDate";
        private const string LAST_TOUCHED_KEY = "lastTouched";
        private const string LAST_TOUCHEDBY_KEY = "lastTouchedBy";
        private const string LAST_RUN_KEY = "lastRun";
        private const string LAST_RUNRESULT_KEY = "lastRunResult";
        private const string EMAIL_TO_KEY = "emailTo";
        private const string EMAIL_REPLYTO_KEY = "emailReplyTo";
        private const string REPORT_RENDERFORMAT_KEY = "rpRenderFormat";
        private const string EMAIL_SUBJECT_KEY = "emailSubject";
        private const string EMAIL_COMMENT_KEY = "emailComment";
        private const string EMAIL_CC_KEY = "emailCc";
        private const string EMAIL_BCC_KEY = "emailBcc";
        private const string REPORT_ID_KEY = "reportId";
        public const string REPORT_NAME_KEY = "reportName";

        private string _reportDisplayName;

        #endregion


        #region Enum
        /// <summary>
        /// Report render format
        /// </summary>
        public enum WlbReportRenderFormat : int
        {
            /// <summary>
            /// Render report as PDF 
            /// </summary>
            PDF = 0,
            /// <summary>
            /// Render report as JPEG
            /// </summary>
            //JPEG = 1,
            /// <summary>
            /// Render report as Excel
            /// </summary>
            Excel = 1
        }

        #endregion


        #region Constructor
        /// <summary>
        /// Subscription Constructor, initialize report display name and 
        /// subscription key base string value: keyBase, itemId and key list
        /// </summary>
        /// <param name="id">Subscription id</param>
        public WlbReportSubscription(string id)
        {
            Configuration = new Dictionary<string, string>();
            KeyBase = WlbConfigurationKeyBase.rpSub;
            ItemId = string.IsNullOrEmpty(id) ? "0" : id;
            _reportDisplayName = string.Empty;

            //Define the known keys
            WlbConfigurationKeys = 
                new List<string>(new[] 
                                    { 
                                        SUB_ID_KEY,
                                        CREATED_KEY,
                                        SUB_NAME_KEY,
                                        SUB_DESCRIPTION_KEY,
                                        SUBSCRIBER_ID_KEY,
                                        SUBSCRIBER_NAME_KEY,
                                        SCHEDULE_ID_KEY,
                                        DAYOFWEEK_KEY,
                                        EXECUTE_TIMEOFDAY_KEY,
                                        TRIGGER_TYPE_KEY,
                                        ENABLED_KEY,
                                        ENABLE_DATE_KEY,
                                        DISABLE_DATE_KEY,
                                        LAST_TOUCHED_KEY,
                                        LAST_TOUCHEDBY_KEY,
                                        LAST_RUN_KEY,
                                        LAST_RUNRESULT_KEY,
                                        EMAIL_TO_KEY,
                                        EMAIL_REPLYTO_KEY,
                                        REPORT_RENDERFORMAT_KEY,
                                        EMAIL_SUBJECT_KEY,
                                        EMAIL_COMMENT_KEY,
                                        EMAIL_CC_KEY,
                                        EMAIL_BCC_KEY,
                                        REPORT_ID_KEY,
                                        REPORT_NAME_KEY
                                    });
        }

        #endregion


        #region Properties
        /// <summary>
        /// Subscription id
        /// </summary>
        public string Id
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUB_ID_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(SUB_ID_KEY), value, true); }
        }

        /// <summary>
        /// The date of the subscription is created
        /// </summary>
        public DateTime Created
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(CREATED_KEY)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(CREATED_KEY), value, true); }
        }

        /// <summary>
        /// Subscription name
        /// </summary>
        public string Name
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUB_NAME_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(SUB_NAME_KEY), value, true); }
        }

        /// <summary>
        /// Subscription description
        /// </summary>
        public string Description
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUB_DESCRIPTION_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(SUB_DESCRIPTION_KEY), value, true); }
        }
        
        /// <summary>
        /// Subscriber id
        /// </summary>
        public string SubscriberId
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUBSCRIBER_ID_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(SUBSCRIBER_ID_KEY), value, true); }
        }

        /// <summary>
        /// Subscriber name
        /// </summary>
        public string SubscriberName
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUBSCRIBER_NAME_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(SUBSCRIBER_NAME_KEY), value, true); }
        }

        /// <summary>
        /// Schedule id
        /// </summary>
        public string ScheduleId
        {
            get { return GetConfigValueString(base.BuildComplexKey(SCHEDULE_ID_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(SCHEDULE_ID_KEY), value, true); }
        }

        /// <summary>
        /// Days of the week sends subscriptions, equivalent to WlbScheduledTask.WlbTaskDaysOfWeek
        /// </summary>
        public WlbScheduledTask.WlbTaskDaysOfWeek DaysOfWeek
        {
            get { return (WlbScheduledTask.WlbTaskDaysOfWeek)GetConfigValueInt(base.BuildComplexKey(DAYOFWEEK_KEY)); }
            set { SetConfigValueInt(base.BuildComplexKey(DAYOFWEEK_KEY), (int)value, true); }
        }

        /// <summary>
        /// Time of the day sends subscriptions
        /// </summary>
        public DateTime RunTimeOfDay
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(EXECUTE_TIMEOFDAY_KEY)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(EXECUTE_TIMEOFDAY_KEY), value, true); }
        }

        /// <summary>
        /// How often sends subscription, equivlent to WlbScheduledTask.WlbTaskTriggerType
        /// </summary>
        public int TriggerType
        {
            get { return GetConfigValueInt(base.BuildComplexKey(TRIGGER_TYPE_KEY)); }
            set { SetConfigValueInt(base.BuildComplexKey(TRIGGER_TYPE_KEY), value, true); }
        }

        /// <summary>
        /// Enable subscription if it's true, otherwise false
        /// </summary>
        public bool Enabled
        {
            get { return GetConfigValueBool(base.BuildComplexKey(ENABLED_KEY)); }
            set { SetConfigValueBool(base.BuildComplexKey(ENABLED_KEY), value, true); }
        }

        /// <summary>
        /// The date of the subscription is enabled
        /// </summary>
        public DateTime EnableDate
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(ENABLE_DATE_KEY)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(ENABLE_DATE_KEY), value, true); }
        }

        /// <summary>
        /// The date of the subscription will be disabled
        /// </summary>
        public DateTime DisableDate
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(DISABLE_DATE_KEY)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(DISABLE_DATE_KEY), value, true); }
        }

        /// <summary>
        /// The date of the subscription last modified
        /// </summary>
        public DateTime LastTouched
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(LAST_TOUCHED_KEY)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(LAST_TOUCHED_KEY), value, true); }
        }

        /// <summary>
        /// The person who modified subscription last
        /// </summary>
        public string LastTouchedBy
        {
            get { return GetConfigValueString(base.BuildComplexKey(LAST_TOUCHEDBY_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(LAST_TOUCHEDBY_KEY), value, true); }
        }

        /// <summary>
        /// The date of the subscription last sent
        /// </summary>
        public DateTime LastRun
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(LAST_RUN_KEY)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(LAST_RUN_KEY), value, true); }
        }

        /// <summary>
        /// The result of the subscription last sent
        /// </summary>
        public string LastRunResult
        {
            get { return GetConfigValueString(base.BuildComplexKey(LAST_RUNRESULT_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(LAST_RUNRESULT_KEY), value, true); }
        }

        /// <summary>
        /// The email addresses that the subscription is sent to
        /// </summary>
        public string EmailTo
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_TO_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_TO_KEY), value, true); }
        }

        /// <summary>
        /// Email reply addresses 
        /// </summary>
        public string EmailReplyTo
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_REPLYTO_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_REPLYTO_KEY), value, true); }
        }

        /// <summary>
        /// Report render format
        /// </summary>
        public int ReportRenderFormat
        {
            get { return GetConfigValueInt(base.BuildComplexKey(REPORT_RENDERFORMAT_KEY)); }
            set { SetConfigValueInt(base.BuildComplexKey(REPORT_RENDERFORMAT_KEY), (int)value, true); }
        }

        /// <summary>
        /// Email subject
        /// </summary>
        public string EmailSubject
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_SUBJECT_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_SUBJECT_KEY), value, true); }
        }

        /// <summary>
        /// Email comment
        /// </summary>
        public string EmailComment
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_COMMENT_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_COMMENT_KEY), value, true); }
        }

        /// <summary>
        /// The email addresses that the subscription is CCed on
        /// </summary>
        public string EmailCc
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_CC_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_CC_KEY), value, true); }
        }

        /// <summary>
        /// The email addresses that the subscription is BCCed on
        /// </summary>
        public string EmailBcc
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_BCC_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_BCC_KEY), value, true); }
        }

        /// <summary>
        /// Report id
        /// </summary>
        public int ReportId
        {
            get { return GetConfigValueInt(base.BuildComplexKey(REPORT_ID_KEY)); }
            set { SetConfigValueInt(base.BuildComplexKey(REPORT_ID_KEY), value, true); }
        }

        /// <summary>
        /// Report file name
        /// </summary>
        public string ReportName
        {
            get { return GetConfigValueString(base.BuildComplexKey(REPORT_NAME_KEY)); }
            set { SetConfigValueString(base.BuildComplexKey(REPORT_NAME_KEY), value, true); }
        }

        /// <summary>
        /// Report's parameters
        /// </summary>
        public Dictionary<string, string> ReportParameters
        {
            get { return GetOtherParameters(); }
            set { SetOtherParameters(value); }
        }

        /// <summary>
        /// Report display name
        /// </summary>
        public string ReportDisplayName
        {
            get { return _reportDisplayName; }
            set { _reportDisplayName = value; }
        }
        #endregion

        public static int GetUTCToLocalOffsetMinutes()
        {
            DateTime localTime = DateTime.Now;
            DateTime UTCTime = DateTime.Now.ToUniversalTime();
            TimeSpan difference = UTCTime.Subtract(localTime);
            int offsetMinutes = difference.Hours * 60 + difference.Minutes;
            return (offsetMinutes);
        }
    }


    /// <summary>
    /// Collection of WlbReportSubscription instance
    /// </summary>
    public class WlbReportSubscriptionCollection : Dictionary<string, WlbReportSubscription>
    {
        #region Constructors

        /// <summary>
        /// WlbReportSubscriptionCollection constructor: loads all subscriptions
        /// </summary>
        /// <param name="configurationParams">Configuration dictionary to be loaded</param>
        public WlbReportSubscriptionCollection(Dictionary<string, string> configurationParams)
        {
            LoadReportSubscriptions(configurationParams, String.Empty);
        }

        /// <summary>
        /// WlbReportSubscriptionCollection constructor: loads a specific subscription
        /// </summary>
        /// <param name="configurationParams">Configuration dictionary to be loaded</param>
        /// <param name="subscriptionId">Subscription id</param>
        public WlbReportSubscriptionCollection(Dictionary<string, string> configurationParams, string subscriptionId)
        {
            LoadReportSubscriptions(configurationParams, subscriptionId);
        }
        #endregion


        /// <summary>
        /// Convert WlbReportSubscriptionCollection to a dictionary
        /// </summary>
        /// <returns>A dictionary that contains WlbReportSubscriptionCollection data</returns>
        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> collectionDictionary = null;

            foreach (WlbReportSubscription rpSubscription in this.Values)
            {
                Dictionary<string, string> rpSubDictionary = rpSubscription.ToDictionary();
                foreach (string key in rpSubDictionary.Keys)
                {
                    if (null == collectionDictionary)
                    {
                        collectionDictionary = new Dictionary<string, string>();
                    }
                    collectionDictionary.Add(key, rpSubDictionary[key]);
                }
            }
            return collectionDictionary;
        }


        /// <summary>
        /// Get a dictionary of WlbReportSubscription instances for a specific report
        /// </summary>
        /// <param name="reportName">Report file name</param>
        /// <returns>A dictionary of WlbReportSubscription instances for the given report file name</returns>
        public Dictionary<string, WlbReportSubscription> GetReportSubscriptionByReportName(string reportName)
        {
            Dictionary<string, WlbReportSubscription> subscriptions = new Dictionary<string, WlbReportSubscription>();
            foreach (string key in this.Keys)
            {
                if(String.Compare(this[key].ReportName, reportName.Split('.')[0], true) == 0)
                    subscriptions.Add(key, this[key]);
            }

            return subscriptions;
        }


        /// <summary>
        /// Load one or all WlbReportSubscription instances to WlbReportSubscriptionCollection.
        /// </summary>
        /// <param name="configurationParams">A dictionary that contains subscription data</param>
        /// <param name="subscriptionId">A subscription id, can be empty</param>
        private void LoadReportSubscriptions(Dictionary<string, string> configurationParams, string subscriptionId)
        {
            foreach (string key in configurationParams.Keys)
            {
                if (key.StartsWith(String.Concat(WlbConfigurationBase.WlbConfigurationKeyBase.rpSub.ToString(),"_"), true, null) )
                {
                    string[] keyElements = key.Split('_');
                    string subId = keyElements[1];

                    if (String.IsNullOrEmpty(subscriptionId) || (String.Compare(subId, subscriptionId, true) == 0))
                    {
                        if (!this.ContainsKey(subId))
                        {
                            this.Add(subId, new WlbReportSubscription(subId));
                            this[subId].AddParameter(key, configurationParams[key]);
                        }
                        else
                        {
                            this[subId].AddParameter(key, configurationParams[key]);
                        }
                    }
                }
            }
        }
    }
}
