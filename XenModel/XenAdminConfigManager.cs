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
using System.Collections.Generic;
using System.Net;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin
{
    public static class XenAdminConfigManager
    {
        public static IXenAdminConfigProvider Provider { get; set; }
    }

    public interface IXenAdminConfigProvider
    {
        Func<List<Role>, IXenConnection, string, AsyncAction.SudoElevationResult> ElevatedSessionDelegate { get; }
        int ConnectionTimeout { get; }
        Session CreateActionSession(Session session, IXenConnection connection);
        bool Exiting { get; }
        bool ForcedExiting { get; }
        string XenCenterUUID { get; }
        bool DontSudo { get; }
        IWebProxy GetProxyFromSettings(IXenConnection connection);
        IWebProxy GetProxyFromSettings(IXenConnection connection, bool isForXenServer);
        int GetProxyTimeout(bool timeout);
        void ShowObject(string opaqueRef);
        void HideObject(string opaqueRef);
        bool ObjectIsHidden(string opaqueRef);
        string GetLogFile();
        void UpdateServerHistory(string hostnameWithPort);
        void SaveSettingsIfRequired();
        bool ShowHiddenVMs { get; }
        string GetXenCenterMetadata(bool isForXenCenter);
        string GetCustomUpdatesXmlLocation();
        string GetInternalStageAuthToken();
        string GetInternalStageAuthTokenName();
    }
}
