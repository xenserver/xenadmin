﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin.Core;
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.Actions
{
    public class TransferProxySettingsAction : TransferDataToHealthCheckAction
    {
        private HTTPHelper.ProxyStyle proxyStyle;
        private string proxyAddress;
        private int proxyPort;
        private int timeOut;
        private bool bypassProxyForServers;
        private bool provideProxyCredentials;
        private string proxyUsername;
        private string proxyPassword;
        private HTTP.ProxyAuthenticationMethod proxyAuthenticationMethod;

        public TransferProxySettingsAction(HTTPHelper.ProxyStyle style, string address, int port, int timeout,
            bool suppressHistory, bool bypassForServer, bool provideCredentials, string username, string password, HTTP.ProxyAuthenticationMethod proxyAuthMethod)
            : base(null, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, Messages.ACTION_TRANSFER_HEALTHCHECK_SETTINGS, suppressHistory)
        {
            proxyStyle = style;
            proxyAddress = address;
            proxyPort = port;
            timeOut = timeout;
            bypassProxyForServers = bypassForServer;
            provideProxyCredentials = provideCredentials;
            proxyUsername = username;
            proxyPassword = password;
            proxyAuthenticationMethod = proxyAuthMethod;
        }

        protected override string GetMessageToBeSent()
        {
            string proxySettings;
            switch (proxyStyle)
            {
                case HTTPHelper.ProxyStyle.SpecifiedProxy:
                    proxySettings = String.Join(SEPARATOR.ToString(), new[] {
                            HealthCheckSettings.PROXY_SETTINGS,
                            ((Int32)HTTPHelper.ProxyStyle.SpecifiedProxy).ToString(),
                            proxyAddress,
                            proxyPort.ToString(),
                            timeOut.ToString(),
                            bypassProxyForServers.ToString(),
                            provideProxyCredentials.ToString(),
                            EncryptionUtils.ProtectForLocalMachine(proxyUsername),
                            EncryptionUtils.ProtectForLocalMachine(proxyPassword),
                            ((Int32)proxyAuthenticationMethod).ToString()});
                    return proxySettings;
                    
                case HTTPHelper.ProxyStyle.SystemProxy:
                    proxySettings = String.Join(SEPARATOR.ToString(), new[] {
                            HealthCheckSettings.PROXY_SETTINGS, ((Int32)HTTPHelper.ProxyStyle.SystemProxy).ToString()});
                    return proxySettings;

                default:
                    proxySettings = String.Join(SEPARATOR.ToString(), new[] {
                            HealthCheckSettings.PROXY_SETTINGS, ((Int32)HTTPHelper.ProxyStyle.DirectConnection).ToString()});
                    return proxySettings;
            }
        }
    }
}