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
using System.Collections.Generic;
using System.Text;

namespace XenAdmin.Wlb
{
    public class WlbReportSubscription : WlbConfigurationBase
    {
        #region  Variables

        private static string SUB_ID = "id";
        private static string CREATED = "created";
        private static string SUB_NAME = "name";
        private static string SUB_DESCRIPTION = "description";
        private static string SUBSCRIBER_ID = "subscriberId";
        private static string SUBSCRIBER_NAME = "subscriberName";
        private static string SCHEDULE_ID = "scheduleId";
        private static string DAYOFWEEK = "daysOfWeek";
        private static string EXECUTE_TIMEOFDAY = "executeTimeOfDay";
        private static string TRIGGER_TYPE = "triggerType";
        private static string ENABLED = "enabled";
        private static string ENABLE_DATE = "enableDate";
        private static string DISABLE_DATE = "disableDate";
        private static string LAST_TOUCHED = "lastTouched";
        private static string LAST_TOUCHEDBY = "lastTouchedBy";
        private static string LAST_RUN = "lastRun";
        private static string LAST_RUNRESULT = "lastRunResult";
        private static string EMAIL_TO = "emailTo";
        private static string EMAIL_REPLYTO = "emailReplyTo";
        private static string REPORT_RENDERFORMAT = "rpRenderFormat";
        private static string EMAIL_SUBJECT = "emailSubject";
        private static string EMAIL_COMMENT = "emailComment";
        private static string EMAIL_CC = "emailCc";
        private static string EMAIL_BCC = "emailBcc";
        private static string REPORT_ID = "reportId";
        public static string REPORT_NAME = "reportName";

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
            base.Configuration = new Dictionary<string, string>();
            base.KeyBase = WlbConfigurationKeyBase.rpSub;
            base.ItemId = (String.IsNullOrEmpty(id) ? "0" : id);
            this._reportDisplayName = String.Empty;

            //Define the known keys
            base.WlbConfigurationKeys = 
                new List<string>(new string[] 
                                    { 
                                        SUB_ID,
                                        CREATED,
                                        SUB_NAME,
                                        SUB_DESCRIPTION,
                                        SUBSCRIBER_ID,
                                        SUBSCRIBER_NAME,
                                        SCHEDULE_ID,
                                        DAYOFWEEK,
                                        EXECUTE_TIMEOFDAY,
                                        TRIGGER_TYPE,
                                        ENABLED,
                                        ENABLE_DATE,
                                        DISABLE_DATE,
                                        LAST_TOUCHED,
                                        LAST_TOUCHEDBY,
                                        LAST_RUN,
                                        LAST_RUNRESULT,
                                        EMAIL_TO,
                                        EMAIL_REPLYTO,
                                        REPORT_RENDERFORMAT,
                                        EMAIL_SUBJECT,
                                        EMAIL_COMMENT,
                                        EMAIL_CC,
                                        EMAIL_BCC,
                                        REPORT_ID,
                                        REPORT_NAME
                                    });
        }

        #endregion


        #region Properties
        /// <summary>
        /// Subscription id
        /// </summary>
        public string Id
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUB_ID)); }
            set { SetConfigValueString(base.BuildComplexKey(SUB_ID), value, true); }
        }

        /// <summary>
        /// The date of the subscription is created
        /// </summary>
        public DateTime Created
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(CREATED)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(CREATED), value, true); }
        }

        /// <summary>
        /// Subscription name
        /// </summary>
        public string Name
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUB_NAME)); }
            set { SetConfigValueString(base.BuildComplexKey(SUB_NAME), value, true); }
        }

        /// <summary>
        /// Subscription description
        /// </summary>
        public string Description
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUB_DESCRIPTION)); }
            set { SetConfigValueString(base.BuildComplexKey(SUB_DESCRIPTION), value, true); }
        }
        
        /// <summary>
        /// Subscriber id
        /// </summary>
        public string SubscriberId
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUBSCRIBER_ID)); }
            set { SetConfigValueString(base.BuildComplexKey(SUBSCRIBER_ID), value, true); }
        }

        /// <summary>
        /// Subscriber name
        /// </summary>
        public string SubscriberName
        {
            get { return GetConfigValueString(base.BuildComplexKey(SUBSCRIBER_NAME)); }
            set { SetConfigValueString(base.BuildComplexKey(SUBSCRIBER_NAME), value, true); }
        }

        /// <summary>
        /// Schedule id
        /// </summary>
        public string ScheduleId
        {
            get { return GetConfigValueString(base.BuildComplexKey(SCHEDULE_ID)); }
            set { SetConfigValueString(base.BuildComplexKey(SCHEDULE_ID), value, true); }
        }

        /// <summary>
        /// Days of the week sends subscriptions, equivalent to WlbScheduledTask.WlbTaskDaysOfWeek
        /// </summary>
        public WlbScheduledTask.WlbTaskDaysOfWeek DaysOfWeek
        {
            get { return (WlbScheduledTask.WlbTaskDaysOfWeek)GetConfigValueInt(base.BuildComplexKey(DAYOFWEEK)); }
            set { SetConfigValueInt(base.BuildComplexKey(DAYOFWEEK), (int)value, true); }
        }

        /// <summary>
        /// Time of the day sends subscriptions
        /// </summary>
        public DateTime ExecuteTimeOfDay
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(EXECUTE_TIMEOFDAY)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(EXECUTE_TIMEOFDAY), value, true); }
        }

        /// <summary>
        /// How often sends subscription, equivlent to WlbScheduledTask.WlbTaskTriggerType
        /// </summary>
        public int TriggerType
        {
            get { return GetConfigValueInt(base.BuildComplexKey(TRIGGER_TYPE)); }
            set { SetConfigValueInt(base.BuildComplexKey(TRIGGER_TYPE), value, true); }
        }

        /// <summary>
        /// Enable subscription if it's true, otherwise false
        /// </summary>
        public bool Enabled
        {
            get { return GetConfigValueBool(base.BuildComplexKey(ENABLED)); }
            set { SetConfigValueBool(base.BuildComplexKey(ENABLED), value, true); }
        }

        /// <summary>
        /// The date of the subscription is enabled
        /// </summary>
        public DateTime EnableDate
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(ENABLE_DATE)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(ENABLE_DATE), value, true); }
        }

        /// <summary>
        /// The date of the subscription will be disabled
        /// </summary>
        public DateTime DisableDate
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(DISABLE_DATE)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(DISABLE_DATE), value, true); }
        }

        /// <summary>
        /// The date of the subscription last modified
        /// </summary>
        public DateTime LastTouched
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(LAST_TOUCHED)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(LAST_TOUCHED), value, true); }
        }

        /// <summary>
        /// The person who modified subscription last
        /// </summary>
        public string LastTouchedBy
        {
            get { return GetConfigValueString(base.BuildComplexKey(LAST_TOUCHEDBY)); }
            set { SetConfigValueString(base.BuildComplexKey(LAST_TOUCHEDBY), value, true); }
        }

        /// <summary>
        /// The date of the subscription last sent
        /// </summary>
        public DateTime LastRun
        {
            get { return GetConfigValueUTCDateTime(base.BuildComplexKey(LAST_RUN)); }
            set { SetConfigValueUTCDateTime(base.BuildComplexKey(LAST_RUN), value, true); }
        }

        /// <summary>
        /// The result of the subscription last sent
        /// </summary>
        public string LastRunResult
        {
            get { return GetConfigValueString(base.BuildComplexKey(LAST_RUNRESULT)); }
            set { SetConfigValueString(base.BuildComplexKey(LAST_RUNRESULT), value, true); }
        }

        /// <summary>
        /// The email addresses that the subscription is sent to
        /// </summary>
        public string EmailTo
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_TO)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_TO), value, true); }
        }

        /// <summary>
        /// Email reply addresses 
        /// </summary>
        public string EmailReplyTo
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_REPLYTO)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_REPLYTO), value, true); }
        }

        /// <summary>
        /// Report render format
        /// </summary>
        public int ReportRenderFormat
        {
            get { return GetConfigValueInt(base.BuildComplexKey(REPORT_RENDERFORMAT)); }
            set { SetConfigValueInt(base.BuildComplexKey(REPORT_RENDERFORMAT), (int)value, true); }
        }

        /// <summary>
        /// Email subject
        /// </summary>
        public string EmailSubject
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_SUBJECT)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_SUBJECT), value, true); }
        }

        /// <summary>
        /// Email comment
        /// </summary>
        public string EmailComment
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_COMMENT)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_COMMENT), value, true); }
        }

        /// <summary>
        /// The email addresses that the subscription is CCed on
        /// </summary>
        public string EmailCc
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_CC)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_CC), value, true); }
        }

        /// <summary>
        /// The email addresses that the subscription is BCCed on
        /// </summary>
        public string EmailBcc
        {
            get { return GetConfigValueString(base.BuildComplexKey(EMAIL_BCC)); }
            set { SetConfigValueString(base.BuildComplexKey(EMAIL_BCC), value, true); }
        }

        /// <summary>
        /// Report id
        /// </summary>
        public int ReportId
        {
            get { return GetConfigValueInt(base.BuildComplexKey(REPORT_ID)); }
            set { SetConfigValueInt(base.BuildComplexKey(REPORT_ID), value, true); }
        }

        /// <summary>
        /// Report file name
        /// </summary>
        public string ReportName
        {
            get { return GetConfigValueString(base.BuildComplexKey(REPORT_NAME)); }
            set { SetConfigValueString(base.BuildComplexKey(REPORT_NAME), value, true); }
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
