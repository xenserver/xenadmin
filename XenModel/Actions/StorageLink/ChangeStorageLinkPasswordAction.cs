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
using System.Text;
using XenAdmin.Network.StorageLink;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class ChangeStorageLinkPasswordAction : AsyncAction
    {
        private readonly StorageLinkConnection _storageLinkConnection;
        private readonly string _username;
        private readonly string _oldPassword;
        private readonly string _newPassword;
        private readonly IEnumerable<IXenConnection> _xenConnections;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeStorageLinkPasswordAction"/> class.
        /// 
        /// You need to be a pool-operator or higher to even get access to the password for the storagelink service.
        /// 
        /// </summary>
        /// <param name="storageLinkConnection">The storage link connection.</param>
        /// <param name="username">The username.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        public ChangeStorageLinkPasswordAction(IEnumerable<IXenConnection> xenConnections,StorageLinkConnection storageLinkConnection, string username, string oldPassword, string newPassword)
            : base(null, Messages.CHANGE_SL_SERVER_PASSWORD_ACTION_TITLE, Messages.CHANGE_SL_SERVER_PASSWORD_ACTION_DES_START)
        {
            Util.ThrowIfParameterNull(storageLinkConnection, "storageLinkConnection");
            Util.ThrowIfStringParameterNullOrEmpty(username, "username");
            Util.ThrowIfParameterNull(oldPassword, "oldPassword");
            Util.ThrowIfParameterNull(newPassword, "newPassword");

            _storageLinkConnection = storageLinkConnection;
            _username = username;
            _oldPassword = oldPassword;
            _newPassword = newPassword;
            _xenConnections = xenConnections;

            AppliesTo.Add(storageLinkConnection.Cache.Server.opaque_ref);
            AppliesTo.AddRange(GetPoolsToCheck().ConvertAll(p => p.opaque_ref));
        }

        private List<Pool> GetPoolsToCheck()
        {
            List<Pool> output = new List<Pool>();
            foreach (IXenConnection c in _xenConnections)
            {
                Pool pool = Helpers.GetPoolOfOne(c);

                if (pool != null)
                {
                    StorageLinkCredentials creds = pool.GetStorageLinkCredentials();

                    if (creds != null && creds.Host == _storageLinkConnection.Host && creds.Username == _storageLinkConnection.Username)
                    {
                        output.Add(pool);
                    }
                }
            }
            return output;
        }

        protected override void Run()
        {
            // set the password on the storagelink service.
            _storageLinkConnection.SetPassword(_username, _oldPassword, _newPassword);

            foreach (Pool pool in GetPoolsToCheck())
            {
                // set the password on the pool other-config
                StorageLinkCredentials creds = pool.GetStorageLinkCredentials();
                creds.SetPassword(_newPassword);

                foreach (PBD pbd in pool.Connection.Cache.PBDs)
                {
                    creds = pbd.GetStorageLinkCredentials();

                    // set the password on the pbd device-config.
                    if (creds != null && 
                        creds.Host == _storageLinkConnection.Host && 
                        creds.Username == _storageLinkConnection.Username && 
                        creds.Password != null)
                    {
                        creds.SetPassword(_newPassword);
                    }
                }
            }
        }
    }
}
