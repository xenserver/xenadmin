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

using Newtonsoft.Json;
using System;
using System.IO;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class CheckForCdnUpdatesAction : AsyncAction
    {
        public CheckForCdnUpdatesAction(IXenConnection connection)
            : base(connection, string.Empty)
        {
            Title = Description = Messages.YUM_REPO_ACTION_CHECK_FOR_UPDATES;
            ApiMethodsToRoleCheck.Add("http/updates");
        }

        public CdnPoolUpdateInfo Updates { get; set; }

        protected override void Run()
        {
            var coordinator = Connection.Resolve(Helpers.GetPoolOfOne(Connection)?.master);
            if (coordinator == null)
                return;

            if (Session == null)
                return;

            UriBuilder builder = new UriBuilder
            {
                Scheme = coordinator.Connection.UriScheme,
                Host = coordinator.address,
                Port = coordinator.Connection.Port,
                Path = "updates",
                Query = $"session_id={Uri.EscapeDataString(Session.opaque_ref)}"
            };

            string json;

            using (Stream httpStream = HTTPHelper.GET(builder.Uri, Connection, true))
            {
                using (var streamReader = new StreamReader(httpStream))
                    json = streamReader.ReadToEnd();
            }

            Updates = JsonConvert.DeserializeObject<CdnPoolUpdateInfo>(json);
        }
    }
}
