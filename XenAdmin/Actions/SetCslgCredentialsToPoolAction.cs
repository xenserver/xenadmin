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
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network.StorageLink;
using System.Threading;
using System.Diagnostics;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class SetCslgCredentialsToPoolAction : AsyncAction
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;

        public SetCslgCredentialsToPoolAction(IXenConnection connection, string host, string username, string password)
            : base(connection,
            string.Format(Messages.SET_STORAGELINK_CREDS_TO_POOL_ACTION_TITLE, Helpers.GetPoolOfOne(connection)),
            string.Format(Messages.SET_STORAGELINK_CREDS_TO_POOL_ACTION_DESCRIPTION, Helpers.GetPoolOfOne(connection)))
        {
            Util.ThrowIfParameterNull(connection, "connection");

            _host = host;
            _username = username;
            _password = password;

            if (Helpers.FeatureForbidden(connection, Host.RestrictStorageChoices))
            {
                throw new ArgumentException("Pool not licensed.", "host");
            }

            XenAPI.Pool pool = Helpers.GetPool(Connection);

            if (pool != null)
            {
                AppliesTo.Add(pool.opaque_ref);
            }
            else
            {
                XenAPI.Host master = Helpers.GetMaster(Connection);

                if (master != null)
                {
                    AppliesTo.Add(master.opaque_ref);
                }
            }
        }

        protected override void Run()
        {
            if (Connection.IsConnected)
            {
#if DEBUG
                Program.Invoke(Program.MainWindow, () =>
                    {
                        // check that local SL creds have been moved to pool.other_config by StorageLinkConnectionManager.
                        Settings.CslgCredentials localCrds = Settings.GetCslgCredentials(Connection);
                        Debug.Assert(localCrds == null || string.IsNullOrEmpty(localCrds.Host));
                    });
#endif

                Pool pool = Helpers.GetPoolOfOne(Connection);
                pool.SetStorageLinkCredentials(_host, _username, _password);

                // other-config gets set on event thread.
                // so wait until other-config has been updated.

                WaitForUpdate(pool);

                // force an other-config change event. In case only the value of the secret has changed.
                Program.BeginInvoke(Program.MainWindow, () => pool.NotifyPropertyChanged("other_config"));

                foreach (PBD pbd in Connection.Cache.PBDs)
                {
                    StorageLinkCredentials creds = pbd.GetStorageLinkCredentials();

                    if (creds != null && creds.Host == _host && creds.Username == _username)
                    {
                        creds.SetPassword(_password);
                    }
                }
            }
        }

        private void WaitForUpdate(Pool pool)
        {
            StorageLinkCredentials creds = null;

            for (int i = 0; i < 10; i++)
            {
                Program.Invoke(Program.MainWindow, () => creds = pool.GetStorageLinkCredentials());

                if (creds != null && creds.Host == _host && creds.Username == _username)
                {
                    return;
                }
                Thread.Sleep(500);
            }
        }
    }
}
