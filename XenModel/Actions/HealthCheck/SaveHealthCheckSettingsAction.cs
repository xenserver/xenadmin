﻿/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SaveHealthCheckSettingsAction : PureAsyncAction
    {
        private readonly Pool pool;
        HealthCheckSettings healthCheckSettings;
        private string authenticationToken;
        private string diagnosticToken;
        private string username;
        private string password;

        public SaveHealthCheckSettingsAction(Pool pool, HealthCheckSettings healthCheckSettings, string authenticationToken, string diagnosticToken, string userName, string passWord, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_SAVE_HEALTHCHECK_SETTINGS, string.Format(Messages.ACTION_SAVING_HEALTHCHECK_SETTINGS, pool.Name), suppressHistory)
        {
            this.pool = pool;
            this.healthCheckSettings = healthCheckSettings;
            this.authenticationToken = authenticationToken;
            this.diagnosticToken = diagnosticToken;
            this.username = healthCheckSettings.Status == HealthCheckStatus.Enabled ? userName : null;
            this.password = healthCheckSettings.Status == HealthCheckStatus.Enabled ? passWord : null;
        }
        
        protected override void Run()
        {
            Dictionary<string, string> newConfig = healthCheckSettings.ToDictionary(pool.health_check_config);
            if (!string.IsNullOrEmpty(authenticationToken))
                HealthCheckAuthenticationAction.SetSecretInfo(Connection, newConfig, HealthCheckSettings.UPLOAD_TOKEN_SECRET, authenticationToken);
            if (!string.IsNullOrEmpty(diagnosticToken))
            HealthCheckAuthenticationAction.SetSecretInfo(Connection, newConfig, HealthCheckSettings.DIAGNOSTIC_TOKEN_SECRET, diagnosticToken);
            HealthCheckAuthenticationAction.SetSecretInfo(Connection, newConfig, HealthCheckSettings.UPLOAD_CREDENTIAL_USER_SECRET, username);
            HealthCheckAuthenticationAction.SetSecretInfo(Connection, newConfig, HealthCheckSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET, password);
            Pool.set_health_check_config(Connection.Session, pool.opaque_ref, newConfig);
        }
    }
}

