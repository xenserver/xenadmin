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
using XenCenterLib;
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.Actions
{
    public class TransferProxySettingsAction : TransferDataToHealthCheckAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HTTPHelper.ProxyStyle proxyStyle;
        private string proxyAddress;
        private int proxyPort;
        private int timeOut;
        private bool bypassProxyForServers;
        private bool provideProxyCredentials;
        private string protectedProxyUsername;
        private string protectedProxyPassword;
        private HTTP.ProxyAuthenticationMethod proxyAuthenticationMethod;

        public TransferProxySettingsAction(HTTPHelper.ProxyStyle style, string address, int port, int timeout,
            bool suppressHistory, bool bypassForServer, bool provideCredentials, string protectedProxyUsername,
            string protectedProxyPassword, HTTP.ProxyAuthenticationMethod proxyAuthMethod)
            : base(null, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, suppressHistory)
        {
            proxyStyle = style;
            proxyAddress = address;
            proxyPort = port;
            timeOut = timeout;
            bypassProxyForServers = bypassForServer;
            provideProxyCredentials = provideCredentials;
            this.protectedProxyUsername = protectedProxyUsername;
            this.protectedProxyPassword = protectedProxyPassword;
            proxyAuthenticationMethod = proxyAuthMethod;
        }

        protected override string GetMessageToBeSent()
        {
            switch (proxyStyle)
            {
                case HTTPHelper.ProxyStyle.SpecifiedProxy:
                    var proxyUsername = "";
                    try
                    {
                        if (!string.IsNullOrEmpty(protectedProxyUsername))
                            proxyUsername = EncryptionUtils.Unprotect(protectedProxyUsername);
                    }
                    catch (Exception e)
                    {
                        log.Error("Could not unprotect internet proxy username.", e);
                        return null;
                    }

                    var proxyPassword = "";
                    try
                    {
                        if (!string.IsNullOrEmpty(protectedProxyPassword))
                            proxyPassword = EncryptionUtils.Unprotect(protectedProxyPassword);
                    }
                    catch (Exception e)
                    {
                        log.Error("Could not unprotect internet proxy password.", e);
                        return null;
                    }

                    return string.Join(SEPARATOR.ToString(),
                        HealthCheckSettings.PROXY_SETTINGS,
                        ((int)HTTPHelper.ProxyStyle.SpecifiedProxy).ToString(),
                        proxyAddress, proxyPort.ToString(),
                        timeOut.ToString(),
                        bypassProxyForServers.ToString(),
                        provideProxyCredentials.ToString(),
                        EncryptionUtils.ProtectForLocalMachine(proxyUsername),
                        EncryptionUtils.ProtectForLocalMachine(proxyPassword),
                        ((int)proxyAuthenticationMethod).ToString());

                case HTTPHelper.ProxyStyle.SystemProxy:
                    return string.Join(SEPARATOR.ToString(),
                        HealthCheckSettings.PROXY_SETTINGS,
                        ((int)HTTPHelper.ProxyStyle.SystemProxy).ToString());

                default:
                    return string.Join(SEPARATOR.ToString(),
                        HealthCheckSettings.PROXY_SETTINGS,
                        ((int)HTTPHelper.ProxyStyle.DirectConnection).ToString());
            }
        }
    }
}