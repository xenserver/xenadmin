using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Win32;
using XenAdmin.Network;
using XenAPI;
using System.Web.Script.Serialization;

namespace XenAdmin.Actions
{
    public class CallHomeAuthenticationAction : AsyncAction
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

        private readonly string identityTokenDomainName = "http://cis-daily.citrite.net";
        private readonly string uploadGrantTokenDomainName = "http://cis-daily.citrite.com";
        private readonly string uploadTokenDomainName = "http://cis-daily.citrite.com";

        private const string productKey = "eb1b224c461038baf1f08dfba6b8d4b4413f96c7";

        private readonly string XenServerUsername;
        private readonly string XenServerPassword;

        public CallHomeAuthenticationAction(Pool pool, string username, string password, bool saveTokenAsSecret, long tokenExpiration, bool suppressHistory)
            : base(pool != null ? pool.Connection : null, Messages.ACTION_CALLHOME_AUTHENTICATION, Messages.ACTION_CALLHOME_AUTHENTICATION_PROGRESS, suppressHistory)
        {
            this.pool = pool;
            this.username = username;
            this.password = password;
            this.saveTokenAsSecret = saveTokenAsSecret;
            this.tokenExpiration = tokenExpiration;
            XenServerUsername = "root";
            XenServerPassword = "xenroot";
            #region RBAC Dependencies
            if (saveTokenAsSecret)
                ApiMethodsToRoleCheck.Add("pool.set_health_check_config");
            #endregion
            
        }

        public CallHomeAuthenticationAction(Pool pool, string username, string password,
            string identityTokenDomainName, string uploadGrantTokenDomainName, string uploadTokenDomainName, bool saveTokenAsSecret, long tokenExpiration, bool suppressHistory)
            : this(pool, username, password, saveTokenAsSecret, tokenExpiration, suppressHistory)
        {
            if (!string.IsNullOrEmpty(identityTokenDomainName))
                this.identityTokenDomainName = identityTokenDomainName;
            if (!string.IsNullOrEmpty(identityTokenDomainName))
                this.uploadGrantTokenDomainName = uploadGrantTokenDomainName;
            if (!string.IsNullOrEmpty(identityTokenDomainName))
                this.uploadTokenDomainName = uploadTokenDomainName;
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
                    SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_TOKEN_SECRET, uploadToken);
                    SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_CREDENTIAL_USER_SECRET, XenServerUsername);
                    SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET, XenServerPassword);
                    Pool.set_health_check_config(Connection.Session, pool.opaque_ref, newConfig);
                }
            }
            catch (Exception e)
            {
                log.Error("Exception trying to authenticate", e);
                Exception = e;
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
            return GetToken(urlString, json);
        }
        
        private string GetUploadGrantToken(string identityToken)
        {
            var json = new JavaScriptSerializer().Serialize(new
            {
                identity_token = identityToken,
                expiration = tokenExpiration
            });
            var urlString = string.Format("{0}{1}", uploadGrantTokenDomainName, uploadGrantTokenUrl);
            return GetToken(urlString, json);
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
            return GetToken(urlString, json);
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
    }
}

