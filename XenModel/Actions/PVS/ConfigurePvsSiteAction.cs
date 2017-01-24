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

using System;
using System.Collections.Generic;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public class ConfigurePvsSiteAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private PVS_site pvsSite;
        private readonly List<PVS_cache_storage> pvsCacheStorages;
        private readonly string siteName;
        private const String URI = "uri";

        public ConfigurePvsSiteAction(IXenConnection connection, string siteName, PVS_site pvsSite, List<PVS_cache_storage> pvsCacheStorages)
            : base(connection, string.Format(Messages.ACTION_CONFUGURE_PVS_SITE_TITLE, siteName.Ellipsise(50)),
                    Messages.ACTION_CONFUGURE_PVS_SITE_DESCRIPTION, false)
        {
            this.pvsSite = pvsSite;
            this.pvsCacheStorages = pvsCacheStorages;
            this.siteName = siteName;
            SetRBACPermissions();
        }

        private void SetRBACPermissions()
        {
            ApiMethodsToRoleCheck.Add("pvs_site.introduce");
            ApiMethodsToRoleCheck.Add("pvs_site.introduce");
            ApiMethodsToRoleCheck.Add("sr.create");
            ApiMethodsToRoleCheck.Add("pvs_cache_storage.destroy");
            ApiMethodsToRoleCheck.Add("pvs_cache_storage.create");
        }

        protected override void Run()
        {
            if (pvsSite == null)
            {
                // create site
                RelatedTask = PVS_site.async_introduce(Session, siteName, string.Empty, string.Empty);
                PollToCompletion(0,10);
                pvsSite = Connection.WaitForCache(new XenRef<PVS_site>(Result));
            }
            else
            {
                // get the site again from cache, just in case it changed (or dissapeared) in the meantime
                pvsSite = Connection.Cache.Resolve(new XenRef<PVS_site>(pvsSite.opaque_ref));
                if (pvsSite == null)
                {
                    log.InfoFormat("PVS Site '{0}' cannot be configured, because it cannot be found.", siteName);
                    PercentComplete = 100;
                    Description = Messages.COMPLETED;
                    return;
                }

                if (pvsSite.name_label != siteName)
                {
                    // set name_label
                    PVS_site.set_name_label(Session, pvsSite.opaque_ref, siteName);
                }
            }
            PercentComplete = 10;

            int inc = pvsCacheStorages.Count > 0 ? 90 / pvsCacheStorages.Count / 3 : 90;
            foreach (var pvsCacheStorage in pvsCacheStorages)
            {
                // create Memory SR, if needed
                if (pvsCacheStorage.SR != null && Helper.IsNullOrEmptyOpaqueRef(pvsCacheStorage.SR.opaque_ref)) 
                {
                    RelatedTask = SR.async_create(Session, pvsCacheStorage.host, new Dictionary<string, string> { { URI, "" } }, 0,
                        Messages.PVS_CACHE_MEMORY_SR_NAME, "", SR.SRTypes.tmpfs.ToString(), "", false, new Dictionary<string, string>());
                    PollToCompletion(PercentComplete, PercentComplete + inc);
                    pvsCacheStorage.SR = new XenRef<SR>(Result);
                }
                else
                {
                    PercentComplete += inc;
                }

                // destroy existing PVS_cache_storage
                var existingConfiguration = pvsSite.PvsCacheStorage(Connection.Resolve(pvsCacheStorage.host));
                if (existingConfiguration != null)
                {
                    RelatedTask = PVS_cache_storage.async_destroy(Session, existingConfiguration.opaque_ref);
                    PollToCompletion(PercentComplete, PercentComplete + inc);
                }
                else
                {
                    PercentComplete += inc;
                }

                // create new PVS_cache_storage
                if (pvsCacheStorage.SR != null)
                {
                    pvsCacheStorage.site = new XenRef<PVS_site>(pvsSite); //asign the new site
                    RelatedTask = PVS_cache_storage.async_create(Session, pvsCacheStorage);
                    PollToCompletion(PercentComplete, PercentComplete + inc);
                }
                else
                {
                    PercentComplete += inc;
                }
            }
            PercentComplete = 100;
            Description = Messages.ACTION_CONFUGURE_PVS_SITE_DONE;
        }
    }
}
