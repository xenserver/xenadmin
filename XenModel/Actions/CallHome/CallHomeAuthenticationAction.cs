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
    public class CallHomeAuthenticationAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly Pool pool;
        private readonly string username;
        private string password;
        private readonly string identityTokenUrl = "/auth/api/create_identity/";
        private readonly string uploadGrantTokenUrl = "/feeds/api/create_grant/";
        private readonly string uploadTokenUrl = "/feeds/api/create_upload/";
        private readonly string identityTokenDomainName = "http://cis-daily.citrite.net";
        private readonly string uploadGrantTokenDomainName = "https://rttf-staging.citrix.com";
        private readonly string uploadTokenDomainName = "https://rttf-staging.citrix.com";
        private const string productKey = "eb1b224c461038baf1f08dfba6b8d4b4413f96c7";

        public CallHomeAuthenticationAction(Pool pool, string username, string password, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_CALLHOME_AUTHENTICATION, Messages.ACTION_CALLHOME_AUTHENTICATION_PROGRESS, suppressHistory)
        {
            this.pool = pool;
            this.username = username;
            this.password = password;
        }

        public CallHomeAuthenticationAction(Pool pool, string username, string password, 
            string identityTokenDomainName, string uploadGrantTokenDomainName, string uploadTokenDomainName, bool suppressHistory)
            : this(pool, username, password, suppressHistory)
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
            Dictionary<string, string> newConfig = pool.gui_config;
            try
            {
                string identityToken = GetIdentityToken();
                string uploadGrantToken = GetUploadGrantToken(identityToken);
                string uploadToken = GetUploadToken(uploadGrantToken);

                SetUploadTokenSecret(Connection, newConfig, uploadToken);
                Pool.set_gui_config(Connection.Session, pool.opaque_ref, newConfig);
            }
            catch (Exception e)
            {
                log.Error("Exception trying to authenticate", e);
                Exception = e;
            }
        }

        public static void SetUploadTokenSecret(IXenConnection connection, Dictionary<string, string> config, string uploadToken)
        {
            if (uploadToken == null)
            {
                config.Remove(CallHomeSettings.UPLOAD_TOKEN_SECRET);
            }
            else if (config.ContainsKey(CallHomeSettings.UPLOAD_TOKEN_SECRET))
            {
                try
                {
                    string secretRef = Secret.get_by_uuid(connection.Session, config[CallHomeSettings.UPLOAD_TOKEN_SECRET]);
                    Secret.set_value(connection.Session, secretRef, uploadToken);
                }
                catch (Failure)
                {
                    config[CallHomeSettings.UPLOAD_TOKEN_SECRET] = Secret.CreateSecret(connection.Session, uploadToken);
                }
                catch (WebException)
                {
                    config[CallHomeSettings.UPLOAD_TOKEN_SECRET] = Secret.CreateSecret(connection.Session, uploadToken);
                }
            }
            else
            {
                config[CallHomeSettings.UPLOAD_TOKEN_SECRET] = Secret.CreateSecret(connection.Session, uploadToken);
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
                identity_token = identityToken
            });
            var urlString = string.Format("{0}{1}", uploadGrantTokenDomainName, uploadGrantTokenUrl);
            return GetToken(urlString, json);
        }

        private string GetUploadToken(string grantToken)
        {
            var json = new JavaScriptSerializer().Serialize(new
            {
                grant_token = grantToken,
                product_key = productKey
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

