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
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using XenAdmin.Core;

namespace XenAdmin.Actions
{
    public class HealthCheckAuthenticationAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly string username;
        private readonly string password;

        private readonly long tokenExpiration;

        private string uploadToken;
        private string diagnosticToken;

        private readonly string _identityDomain = "https://cis.citrix.com";
        private readonly string _uploadGrantDomain = "https://rttf.citrix.com";
        private readonly string _uploadDomain = "https://rttf.citrix.com";
        private readonly string _diagnosticDomain = " https://cis.citrix.com";

        private readonly string productKey = "1a2d94a4263cd016dd7a7d510bde87f058a0b75d";

        private readonly bool diagnosticTokenRequired;

        public HealthCheckAuthenticationAction(string username, string password, long tokenExpiration, bool suppressHistory)
            : base(null, "", "", suppressHistory)
        {
            this.username = username;
            this.password = password;
            this.tokenExpiration = tokenExpiration;

            Title = string.Format(Messages.ACTION_HEALTHCHECK_AUTHENTICATION, BrandManager.Cis);
        }

        public HealthCheckAuthenticationAction(string username, string password,
            string identityDomain, string uploadGrantDomain, string uploadDomain, string diagnosticDomain, 
            string productKey, long tokenExpiration, bool diagnosticTokenRequired, bool suppressHistory)
            : this(username, password, tokenExpiration, suppressHistory)
        {
            if (!string.IsNullOrEmpty(identityDomain))
                _identityDomain = identityDomain;

            if (!string.IsNullOrEmpty(uploadGrantDomain))
                _uploadGrantDomain = uploadGrantDomain;

            if (!string.IsNullOrEmpty(uploadDomain))
                _uploadDomain = uploadDomain;

            if (!string.IsNullOrEmpty(diagnosticDomain))
                _diagnosticDomain = diagnosticDomain;

            if (!string.IsNullOrEmpty(productKey))
                this.productKey = productKey;

            this.diagnosticTokenRequired = diagnosticTokenRequired;
        }

        protected override void Run()
        {
            Description = string.Format(Messages.ACTION_HEALTHCHECK_AUTHENTICATION_PROGRESS, BrandManager.Cis);

            try
            {
                string identityToken = GetIdentityToken();
                string uploadGrantToken = GetUploadGrantToken(identityToken);
                uploadToken = GetUploadToken(uploadGrantToken);
                if (diagnosticTokenRequired) 
                    diagnosticToken = GetDiagnosticToken(identityToken);
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

        public string DiagnosticToken
        {
            get { return diagnosticToken; }
        }

        private string GetIdentityToken()
        {
            var json = new JavaScriptSerializer().Serialize(new
            {
                username,
                password
            });
            var urlString = $"{_identityDomain}/auth/api/create_identity/";
            try
            {
                return GetToken(urlString, json, null);
            }
            catch (WebException e)
            {
                log.Info($"WebException while getting identity token from {_identityDomain}.", e);
                if (e.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse) e.Response).StatusCode == HttpStatusCode.Forbidden)
                    throw new HealthCheckAuthenticationException(Messages.HEALTH_CHECK_AUTHENTICATION_INVALID_CREDENTIALS, e);
                throw;
            }
            catch (Exception e) 
            {
                log.Info($"Exception while getting identity token from {_identityDomain}.", e);
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
            var urlString = $"{_uploadGrantDomain}/feeds/api/create_grant/";

            try
            {
                return GetToken(urlString, json, null);
            }
            catch (Exception e)
            {
                log.Info($"Exception while getting upload grant token from {_uploadGrantDomain}.", e);
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
            var urlString = $"{_uploadDomain}/feeds/api/create_upload/";
            try
            {
                return GetToken(urlString, json, null);
            }
            catch (Exception e)
            {
                log.Info($"Exception while getting upload token from {_uploadDomain}.", e);
                throw;
            }
        }

        private string GetDiagnosticToken(string identityToken)
        {
            var json = new JavaScriptSerializer().Serialize(new
            {
                agent = "XenServer",
                max_age = tokenExpiration
            });
            var urlString = $"{_diagnosticDomain}/diag_sdk/token_grant/";

            try
            {
                return GetToken(urlString, json, "BT " + identityToken);
            }
            catch (Exception e)
            {
                log.Info($"Exception while getting diagnostic token from {_diagnosticDomain}.", e);
                throw;
            }
        }

        private string GetToken(string urlString, string jsonParameters, string authorizationHeader)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(urlString);
            if (authorizationHeader != null)
            {
                httpWebRequest.Headers.Add("Authorization", authorizationHeader);
            }
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

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

            public HealthCheckAuthenticationException(Exception exception)
                : base(string.Format(Messages.HEALTH_CHECK_AUTHENTICATION_FAILED, BrandManager.Cis), exception) { }

            public HealthCheckAuthenticationException(SerializationInfo serialinfo, StreamingContext context) : base(serialinfo, context) { }
        }
    }
}

