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

using XenAdmin;
using XenAdmin.Network;
using System.Net;

namespace XenAPI
{
    public class StorageLinkCredentials
    {
        public string Host { get; private set; }
        public string Username { get; private set; }
        private string _password;
        public string PasswordSecret { get; private set; }
        private readonly IXenConnection _connection;

        public StorageLinkCredentials(IXenConnection connection, string host, string username, string password, string passwordSecret)
        {
            Util.ThrowIfParameterNull(connection, "connection");

            Host = host;
            Username = username;
            _password = password;
            PasswordSecret = passwordSecret;
            _connection = connection;
        }

        public string Password
        {
            get
            {
                if (_password != null)
                {
                    return _password;
                }
                
                var session = _connection.Session;

                if (session != null && !string.IsNullOrEmpty(PasswordSecret))
                {
                    try
                    {
                        string secretRef = Secret.get_by_uuid(session, PasswordSecret);
                        _password = Secret.get_value(session, secretRef);
                        return _password;
                    }
                    catch (Failure)
                    {
                    }
                    catch (WebException)
                    {
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Sets the password. This is done by setting the value of the password-secret stored in the credentials.
        /// </summary>
        /// <param name="password">The new password.</param>
        public void SetPassword(string password)
        {
            Util.ThrowIfParameterNull(password, "password");

            var session = _connection.Session;
            
            if (session != null)
            {
                string secretRef = Secret.get_by_uuid(session, PasswordSecret);
                Secret.set_value(_connection.Session, secretRef, password);
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(PasswordSecret);
            }
        }
    }
}
