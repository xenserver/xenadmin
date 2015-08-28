﻿/* Copyright (c) Citrix Systems Inc. 
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
using System.Web.Script.Serialization;
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.Actions
{
    public class GetHealthCheckAnalysisResultAction: AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string DIAG_RESULT_URL = "/diag_sdk/diag_results/";
        private readonly string diagnosticDomainName = "https://cis.citrix.com";
       
        public GetHealthCheckAnalysisResultAction(Pool pool, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_GET_HEALTH_CHECK_RESULT, Messages.ACTION_GET_HEALTH_CHECK_RESULT_PROGRESS, suppressHistory)
        {
            
        }

        public GetHealthCheckAnalysisResultAction(Pool pool, string diagnosticDomainName, bool suppressHistory)
            : this(pool, suppressHistory)
        {
            if (!string.IsNullOrEmpty(diagnosticDomainName))
                this.diagnosticDomainName = diagnosticDomainName;
        }

        protected override void Run()
        {
            try
            {
                var diagnosticToken = Pool.HealthCheckSettings.GetSecretyInfo(Pool.Connection, HealthCheckSettings.DIAGNOSTIC_TOKEN_SECRET);
                if (string.IsNullOrEmpty(diagnosticToken))
                {
                    log.InfoFormat("Cannot get the diagnostic result for {0}, because couldn't retrieve the diagnostic token", Pool.Name);
                    Description = Messages.ACTION_GET_HEALTH_CHECK_RESULT_FAILED;
                    return;
                }
                if (!Pool.HealthCheckSettings.HasUpload)
                {
                    log.InfoFormat("Cannot get the diagnostic result for {0}, because the there is no upload completed yet", Pool.Name);
                    Description = Messages.ACTION_GET_HEALTH_CHECK_RESULT_FAILED;
                    return;
                }
                var analysisResult = GetAnalysisResult(diagnosticToken, Pool.HealthCheckSettings.UploadUuid);
                log.Info("Saving analysis result");
                Dictionary<string, string> newConfig = Pool.health_check_config;
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_SEVERITY] = HealthCheckSettings.DiagnosticAlertSeverityToString(GetMaxSeverity(analysisResult));
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_ISSUES_DETECTED] = analysisResult.Count.ToString();
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_UPLOAD_UUID] = Pool.HealthCheckSettings.UploadUuid;
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_UPLOAD_TIME] = Pool.HealthCheckSettings.LastSuccessfulUpload;
                Pool.set_health_check_config(Session, Pool.opaque_ref, newConfig);
            }
            catch (Exception e)
            {
                log.InfoFormat("Exception while getting diagnostic result from {0}. Exception Message: {1} ", diagnosticDomainName, e.Message);
                Description = Messages.ACTION_GET_HEALTH_CHECK_RESULT_FAILED;
                throw;
            }
            
            Description = Messages.COMPLETED;
        }

        private DiagnosticAlertSeverity GetMaxSeverity(List<AnalysisResult> diagnosticAlerts)
        {
            var maxSeverity = DiagnosticAlertSeverity.Info;
            foreach (var diagnosticAlert in diagnosticAlerts)
            {
                var severity = HealthCheckSettings.StringToDiagnosticAlertSeverity(diagnosticAlert.severity);
                if (severity > maxSeverity)
                    maxSeverity = severity;
            }
            return maxSeverity;
        }

        private List<AnalysisResult> GetAnalysisResult(string diagnosticToken, string uploadUuid)
        {
            var urlString = string.Format("{0}{1}?upload_uuid={2}", diagnosticDomainName, DIAG_RESULT_URL, uploadUuid);
            var authorizationHeader = "BT " + diagnosticToken;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
            httpWebRequest.Headers.Add("Authorization", authorizationHeader);
            httpWebRequest.Method = "GET";

            string result;
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            TemplateResponse response = new JavaScriptSerializer().Deserialize<TemplateResponse>(result);
            return response.results;
        }

        private class TemplateResponse
        {
            public List<AnalysisResult> results { get; set; }
        }

        private class AnalysisResult
        {
            public string severity { get; set; }
            /*public string extra_information { get; set; }
            public string owner { get; set; }
            public string crm_id { get; set; }
            public string id { get; set; }
            public List<string> category { get; set; }
            public string analysis_id { get; set; }
            public string priority { get; set; }
            public string product { get; set; }
            public string description { get; set; }
            public DateTime analysis_completion_time { get; set; }
            public string component { get; set; }
            public string name { get; set; }
            public string upload_description { get; set; }
            public string upload_id { get; set; }
            public string recommendations { get; set; }
            public string env_identifier { get; set; }*/
        }
    }
}
