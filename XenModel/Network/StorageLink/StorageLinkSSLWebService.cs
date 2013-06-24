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
using System.Net;
using System.Text;
using System.Threading;

namespace XenAdmin.Network.StorageLink.Service
{
    public class StorageLinkSSLWebService : StorageLinkWebService, IStorageLinkWebService
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Host { get; private set; }

        private const int DefaultPortNumber = 21605;

        /// <summary>
        /// An SSL connection to a storage link gateway
        /// </summary>
        /// <throws>UriFormatException if the host string contains invalid data such as commas etc.</throws>
        /// <param name="username">The user name</param>
        /// <param name="password">Required password</param>
        /// <param name="host">Path to the host</param>
        public StorageLinkSSLWebService(string username, string password, string host)
        {
            Util.ThrowIfStringParameterNullOrEmpty(username, "username");
            Util.ThrowIfParameterNull(password, "password");
            Util.ThrowIfStringParameterNullOrEmpty(host, "host");

            Username = username;
            Password = password;
            Timeout = System.Threading.Timeout.Infinite;

            Uri uri = ParseAndFormatHostUri(host);
            Url = uri.ToString();
            Host = uri.Host;
        }

        /// <summary>
        /// Parse the string containing the host and modify where necessary
        /// </summary>
        /// <param name="host">A string containing the host name. If a port has been supplied
        /// then it will be used, otherwise port 21605 is set</param>
        /// <returns>Uri containg the correctly formatted host</returns>
        /// <throws>UriFormatException if the host string contains invalid data such as commas etc.</throws>
        private Uri ParseAndFormatHostUri(string host)
        {
            Uri uri = null;
            if (!Uri.TryCreate(String.Format("https://{0}", host), UriKind.Absolute, out uri))
                throw new UriFormatException( String.Format("The URI: {0} was badly formatted", host ) );

            UriBuilder uriBuilder = new UriBuilder(uri);

            //Check if there has been a port provided and set the default if not.
            if ( host.IndexOf(':') < 0 )
                uriBuilder.Port = DefaultPortNumber;

            return uriBuilder.Uri;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest wr = base.GetWebRequest(uri);

            if (!string.IsNullOrEmpty(Username))
            {
                wr.Headers["AUTHORIZATION"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Username + ":" + Password));
                wr.Headers["Accept-Language"] = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            }

            return wr;
        }
    }
}
