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

using System.Collections.Generic;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SaveCallHomeSettingsAction : PureAsyncAction
    {
        private readonly Pool pool;
        CallHomeSettings callHomeSettings;
        private string authenticationToken;
        private string username;
        private string password;

        public SaveCallHomeSettingsAction(Pool pool, CallHomeSettings callHomeSettings, string authenticationToken, string userName, string passWord, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_SAVE_CALLHOME_SETTINGS, string.Format(Messages.ACTION_SAVING_CALLHOME_SETTINGS, pool.Name), suppressHistory)
        {
            this.pool = pool;
            this.callHomeSettings = callHomeSettings;
            this.authenticationToken = authenticationToken;
            this.username = callHomeSettings.Status == CallHomeStatus.Enabled ? userName : null;
            this.password = callHomeSettings.Status == CallHomeStatus.Enabled ? passWord : null;
        }
        
        protected override void Run()
        {
            Dictionary<string, string> newConfig = callHomeSettings.ToDictionary(pool.health_check_config);
            if (!string.IsNullOrEmpty(authenticationToken))
                CallHomeAuthenticationAction.SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_TOKEN_SECRET, authenticationToken);
            CallHomeAuthenticationAction.SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_CREDENTIAL_USER_SECRET, username);
            CallHomeAuthenticationAction.SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET, password);
            Pool.set_health_check_config(Connection.Session, pool.opaque_ref, newConfig);
        }
    }
}

