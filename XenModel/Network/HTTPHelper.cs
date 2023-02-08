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
using XenAdmin;
using XenAdmin.Network;


namespace XenAPI
{
    public class HTTPHelper
    {
        public enum ProxyStyle
        {
            // Note that these numbers make it into user settings files, so need to be preserved.
            DirectConnection = 0,
            SystemProxy = 1,
            SpecifiedProxy = 2
        }

        public static Stream CONNECT(Uri uri, IXenConnection connection, string session, bool timeout)
        {
            return HTTP.HttpConnectStream(uri, XenAdminConfigManager.Provider.GetProxyFromSettings(connection), session, XenAdminConfigManager.Provider.GetProxyTimeout(timeout));
        }

        public static Stream PUT(Uri uri, long ContentLength, bool timeout)
        {
            return HTTP.HttpPutStream(uri, XenAdminConfigManager.Provider.GetProxyFromSettings(null), ContentLength, XenAdminConfigManager.Provider.GetProxyTimeout(timeout));
        }

        public static Stream GET(Uri uri, IXenConnection connection, bool timeout)
        {
            return HTTP.HttpGetStream(uri, XenAdminConfigManager.Provider.GetProxyFromSettings(connection, true), XenAdminConfigManager.Provider.GetProxyTimeout(timeout));
        }
    }
}
