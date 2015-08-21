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
using System.Runtime.Serialization;
using XenAdmin.Network;
using XenAPI;
using System.Web.Script.Serialization;
using XenAdmin.Model;

namespace XenAdmin.Actions
{
    public class HealthCheckAuthenticationAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly Pool pool;
        private readonly string username;
        private readonly string password;

        private readonly bool saveTokenAsSecret;
        private readonly long tokenExpiration;

        private string uploadToken;

        private const string identityTokenUrl = "/auth/api/create_identity/";
        private const string uploadGrantTokenUrl = "/feeds/api/create_grant/";
        private const string uploadTokenUrl = "/feeds/api/create_upload/";

        private readonly string identityTokenDomainName = "https://cis.citrix.com";
        private readonly string uploadGrantTokenDomainName = "https://rttf.citrix.com";
        private readonly string uploadTokenDomainName = "https://rttf.citrix.com";

        private readonly string productKey = "1a2d94a4263cd016dd7a7d510bde87f058a0b75d";

        public HealthCheckAuthenticationAction(Pool pool, string username, string password, bool saveTokenAsSecret, long tokenExpiration, bool suppressHistory)
            : base(pool != null ? pool.Connection : null, Messages.ACTION_HEALTHCHECK_AUTHENTICATION, Messages.ACTION_HEALTHCHECK_AUTHENTICATION_PROGRESS, suppressHistory)
        {
            this.pool = pool;
            this.username = username;
            this.password = password;
            this.saveTokenAsSecret = saveTokenAsSecret;
            this.tokenExpiration = tokenExpiration;
            #region RBAC Dependencies
            if (saveTokenAsSecret)
                ApiMethodsToRoleCheck.Add("pool.set_health_check_config");
            #endregion
            
        }

        public HealthCheckAuthenticationAction(Pool pool, string username, string password,
            string identityTokenDomainName, string uploadGrantTokenDomainName, string uploadTokenDomainName, string productKey, bool saveTokenAsSecret, long tokenExpiration, bool suppressHistory)
            : this(pool, username, password, saveTokenAsSecret, tokenExpiration, suppressHistory)
        {
            if (!string.IsNullOrEmpty(identityTokenDomainName))
                this.identityTokenDomainName = identityTokenDomainName;
            if (!string.IsNullOrEmpty(identityTokenDomainName))
                this.uploadGrantTokenDomainName = uploadGrantTokenDomainName;
            if (!string.IsNullOrEmpty(identityTokenDomainName))
                this.uploadTokenDomainName = uploadTokenDomainName;
            if (!string.IsNullOrEmpty(productKey))
                this.productKey = productKey;
        }

        protected override void Run()
        {
            System.Diagnostics.Trace.Assert(pool != null || !saveTokenAsSecret, "Pool is null! Cannot save token as secret");
            try
            {
                string identityToken = GetIdentityToken();
                string uploadGrantToken = GetUploadGrantToken(identityToken);
                uploadToken = GetUploadToken(uploadGrantToken);

                if (saveTokenAsSecret && pool != null)
                {
                    log.Info("Saving upload token as xapi secret");
                    Dictionary<string, string> newConfig = pool.health_check_config;
                    SetSecretInfo(Connection, newConfig, HealthCheckSettings.UPLOAD_TOKEN_SECRET, uploadToken);
                    Pool.set_health_check_config(Connection.Session, pool.opaque_ref, newConfig);
                }
            }
            catch (HealthCheckAuthenticationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new HealthCheckAuthenticationException(e);
            }
        }

        public string UploadToken
        {
            get { return uploadToken; }
        }

        public static void SetSecretInfo(IXenConnection connection, Dictionary<string, string> config, string infoKey, string infoValue)
        {
            if (string.IsNullOrEmpty(infoKey))
                return;

            if (infoValue == null)
            {
                if (config.ContainsKey(infoKey))
                {
                    TryToDestroySecret(connection, config[infoKey]);
                    config.Remove(infoKey);
                }
            }
            else if (config.ContainsKey(infoKey))
            {
                try
                {
                    string secretRef = Secret.get_by_uuid(connection.Session, config[infoKey]);
                    Secret.set_value(connection.Session, secretRef, infoValue);
                }
                catch (Failure)
                {
                    config[infoKey] = Secret.CreateSecret(connection.Session, infoValue);
                }
                catch (WebException)
                {
                    config[infoKey] = Secret.CreateSecret(connection.Session, infoValue);
                }
            }
            else
            {
                config[infoKey] = Secret.CreateSecret(connection.Session, infoValue);
            }
        }
        
        private static void TryToDestroySecret(IXenConnection connection, string secret_uuid)
        {
            try
            {
                var secret = Secret.get_by_uuid(connection.Session, secret_uuid);
                Secret.destroy(connection.Session, secret.opaque_ref);
                log.DebugFormat("Successfully destroyed secret {0}", secret_uuid);
            }
            catch (Exception exn)
            {
                log.Error(string.Format("Failed to destroy secret {0}", secret_uuid), exn);
            }
        }

        private string GetIdentityToken()
        {
            var json = new JavaScriptSerializer().Serialize(new
            {
                username,
                password
            });
            var urlString = string.Format("{0}{1}", identityTokenDomainName, identityTokenUrl);
            try
            {
                return GetToken(urlString, json);
            }
            catch (WebException e)
            {
                log.InfoFormat("WebException while getting identity token. Exception Message: " + e.Message);
                if (e.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse) e.Response).StatusCode == HttpStatusCode.Forbidden)
                    throw new HealthCheckAuthenticationException(Messages.HEALTH_CHECK_AUTHENTICATION_INVALID_CREDENTIALS, e);
                throw;
            }
            catch (Exception e) 
            {
                log.InfoFormat("Exception while getting identity token. Exception Message: " + e.Message);
                throw;
            }
        }
        
        private string GetUploadGrantToken(string identityToken)
        {
            var json = new JavaScriptSerializer().Serialize(new
            {
                identity_token = identityToken
            });
            if (tokenExpiration != 0)
            {
                json = new JavaScriptSerializer().Serialize(new
                {
                    identity_token = identityToken,
                    expiration = tokenExpiration
                });
            }
            var urlString = string.Format("{0}{1}", uploadGrantTokenDomainName, uploadGrantTokenUrl);

            try
            {
                return GetToken(urlString, json);
            }
            catch (Exception e)
            {
                log.InfoFormat("Exception while getting upload grant token. Exception Message: " + e.Message);
                throw;
            }
        }

        private string GetUploadToken(string grantToken)
        {
            var json = new JavaScriptSerializer().Serialize(new
            {
                grant_token = grantToken,
                product_key = productKey,
                expiration = tokenExpiration
            });
            var urlString = string.Format("{0}{1}", uploadTokenDomainName, uploadTokenUrl);
            try
            {
                return GetToken(urlString, json);
            }
            catch (Exception e)
            {
                log.InfoFormat("Exception while getting upload token. Exception Message: " + e.Message);
                throw;
            }
        }

        private string GetToken(string urlString, string jsonParameters)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(urlString);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonParameters);
            }
            string result;
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            TemplateResponse response = new JavaScriptSerializer().Deserialize<TemplateResponse>(result);
            return response.Token;
        }

        class TemplateResponse
        {
            public string Token { get; set; }
        }

        [Serializable]
        public class HealthCheckAuthenticationException : Exception
        {
            public HealthCheckAuthenticationException() : base() { }

            public HealthCheckAuthenticationException(string message) : base(message) { }

            public HealthCheckAuthenticationException(string message, Exception exception) : base(message, exception) { }

            public HealthCheckAuthenticationException(Exception exception) : base(Messages.HEALTH_CHECK_AUTHENTICATION_FAILED, exception) { }

            public HealthCheckAuthenticationException(SerializationInfo serialinfo, StreamingContext context) : base(serialinfo, context) { }
        }
    }
}

