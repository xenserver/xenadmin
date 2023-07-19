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
using System.Text;
using System.Web.Script.Serialization;

namespace XenAdmin.Actions.Updates
{
    public static class TokenManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int ExpiryInMinutes = 1440; //24 hours
        private static object _lock = new object();
        private static FileServiceToken _token;


        public static void GetToken(IConfigProvider provider)
        {
            lock (_lock)
            {
                if (!IsTokenValid())
                    CreateToken(provider);
            }
        }

        public static string GetDownloadCredential(IConfigProvider configProvider)
        {
            lock (_lock)
            {
                if (!IsTokenValid())
                    CreateToken(configProvider);

                return Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{configProvider.FileServiceUsername}_{_token.session_id}:{_token.token}"));
            }
        }

        public static void InvalidateToken(IConfigProvider configProvider)
        {
            lock (_lock)
            {
                if (_token != null)
                    DeleteToken(configProvider);
            }
        }


        private static void CreateToken(IConfigProvider configProvider)
        {
            var url = configProvider.GetCustomTokenUrl() ?? InvisibleMessages.TOKEN_API_URL;
            var username = configProvider.FileServiceUsername;
            var clientId = configProvider.FileServiceClientId;
            var credential = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{clientId}"));

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Headers.Add("Authorization", $"Basic {credential}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Proxy = configProvider.GetProxyFromSettings(null, false);

            using (var requestStream = httpWebRequest.GetRequestStream())
            using (var streamWriter = new StreamWriter(requestStream))
                streamWriter.Write($"{{\"expiration\": {ExpiryInMinutes}}}");

            try
            {
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (httpResponse.StatusCode != HttpStatusCode.Created)
                        throw new WebException();

                    using (var responseStream = httpResponse.GetResponseStream())
                    {
                        if (responseStream == null)
                            throw new WebException();

                        log.Info("Account authenticated. Created token.");

                        using (var streamReader = new StreamReader(responseStream))
                        {
                            var json = streamReader.ReadToEnd();
                            _token = new JavaScriptSerializer().Deserialize(json, typeof(FileServiceToken)) as FileServiceToken;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (!(ex.Response is HttpWebResponse response))
                    throw new WebException(Messages.FILESERVICE_AUTHENTICATE_ERROR);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        log.Error("Could not authenticate account (400 Bad request). A field provided was invalid");
                        throw new Exception(string.Format(Messages.STRING_SPACE_STRING,
                            Messages.FILESERVICE_AUTHENTICATE_ERROR, Messages.FILESERVICE_ERROR_400));

                    case HttpStatusCode.Unauthorized:
                        log.Error("Could not authenticate account (401 Unauthorized). Client ID may be invalid or revoked.");
                        throw new Exception(string.Format(Messages.STRING_SPACE_STRING,
                            Messages.FILESERVICE_AUTHENTICATE_ERROR, Messages.FILESERVICE_ERROR_401));

                    case HttpStatusCode.InternalServerError:
                        log.Error("Could not authenticate account (500 Internal server error). Could not get session token or configuration object was null.");
                        throw new Exception(string.Format(Messages.STRING_SPACE_STRING,
                            Messages.FILESERVICE_AUTHENTICATE_ERROR, Messages.FILESERVICE_ERROR_500));

                    default:
                        log.Error($"Could not authenticate account (HTTP status code {response.StatusCode})");
                        throw new WebException(Messages.FILESERVICE_AUTHENTICATE_ERROR);
                }
            }
        }

        private static bool IsTokenValid()
        {
            if (_token == null)
                return false;

            if (DateTime.TryParse(_token.expires_at, out var expiryDate) &&
                expiryDate - DateTime.Now > TimeSpan.Zero)
                return true;

            return false;
        }

        private static void DeleteToken(IConfigProvider configProvider)
        {
            var username = configProvider.FileServiceUsername;
            var clientId = configProvider.FileServiceClientId;

            var url = configProvider.GetCustomTokenUrl() ?? InvisibleMessages.TOKEN_API_URL;
            var credential = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{clientId}"));

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Headers.Add("Authorization", $"Basic {credential}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";
            httpWebRequest.Proxy = configProvider.GetProxyFromSettings(null, false);

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    streamWriter.Write($"{{\"sessionId\": \"{_token.session_id}\"}}");

                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        log.Warn($"Could not invalidated session token ({httpResponse.StatusCode}).");
                        return;
                    }

                    using (var responseStream = httpResponse.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            log.Warn("Could not invalidated session token.");
                            return;
                        }

                        using (var streamReader = new StreamReader(responseStream))
                        {
                            var json = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                if (wex.Response is HttpWebResponse response)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            log.Warn("Could not invalidated session token (400 Bad request).");
                            return;
                        case HttpStatusCode.Unauthorized:
                            log.Warn("Could not invalidated session token (401 Unauthorized). Client ID may be invalid or revoked.");
                            return;
                        case HttpStatusCode.InternalServerError:
                            log.Warn("Could not invalidated session token (500 Internal server error).");
                            return;
                    }
                }

                log.Warn("Could not invalidated session token.", wex);
            }
            catch (Exception e)
            {
                log.Warn("Could not invalidated session token.", e);
            }
            finally
            {
                _token = null;
            }
        }
    }

    [Serializable]
    public class FileServiceClientId
    {
        public string username { get; set; }
        public string apikey { get; set; }
        public string createdDate { get; set; }
    }

    [Serializable]
    public class FileServiceToken
    {
        public string token { get; set; }
        public string token_type { get; set; }
        public string session_id { get; set; }
        public string issued_at { get; set; }
        public string expires_at { get; set; }
    }
}
