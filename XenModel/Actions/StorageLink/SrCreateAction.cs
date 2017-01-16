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
using System.Collections;
using System.Text;

using XenAdmin;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SrCreateAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _srName;
        private readonly string _srDescription;
        private readonly SR.SRTypes _srType;
        private readonly string _srContentType;
        private readonly bool _srIsShared;
        private readonly Dictionary<string, string> _dconf;
        private readonly Dictionary<string, string> _smconf;

        /// <summary>
        /// RBAC dependencies needed to create SR.
        /// </summary>
        public static RbacMethodList StaticRBACDependencies = new RbacMethodList("sr.create",
                                                                                 "sr.set_other_config", 
                                                                                 "sr.forget", 
                                                                                 "pbd.plug", 
                                                                                 "pbd.unplug");

        public SrCreateAction(IXenConnection connection, Host host, string srName,
            string srDescription, SR.SRTypes srType, string srContentType,
            Dictionary<string, string> dconf, Dictionary<string, string> smconf)
            : base(connection, string.Format(Messages.ACTION_SR_CREATING_TITLE,
            XenAPI.SR.getFriendlyTypeName(srType), srName, Helpers.GetName(connection)))
        {
            Host = host;
            Pool = Helpers.GetPool(connection);
            _srName = srName;
            _srDescription = srDescription;
            _srType = srType;
            _srContentType = srContentType;
            _srIsShared = true;  // used to depend on restrict_pool_attached_storage flag: now always true, but left in in case we want to create local SRs one day
            _dconf = dconf;
            _smconf = smconf;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.AddRange(StaticRBACDependencies);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);

            if (isFirstSharedNonISOSR())  // for SrAction(SrActionKind.SetAsDefault)
            {
                ApiMethodsToRoleCheck.Add("pool.set_name_label");
                ApiMethodsToRoleCheck.Add("pool.set_name_description");
                ApiMethodsToRoleCheck.Add("pool.set_default_SR");
                ApiMethodsToRoleCheck.Add("pool.set_suspend_image_SR");
                ApiMethodsToRoleCheck.Add("pool.set_crash_dump_SR");
                ApiMethodsToRoleCheck.Add("pool.set_other_config");
                ApiMethodsToRoleCheck.Add("pool.set_ha_allow_overcommit");
                ApiMethodsToRoleCheck.Add("pool.set_tags");
                ApiMethodsToRoleCheck.Add("pool.set_gui_config");
                ApiMethodsToRoleCheck.Add("pool.set_wlb_enabled");
                ApiMethodsToRoleCheck.Add("pool.set_wlb_verify_cert");
            }
            #endregion
        }

        protected override void Run()
        {
            log.Debug("Running SR.Create()");
            log.DebugFormat("host='{0}'", Host.Name);
            log.DebugFormat("name='{0}'", _srName);
            log.DebugFormat("description='{0}'", _srDescription);
            log.DebugFormat("type='{0}'", _srType);
            log.DebugFormat("content type='{0}'", _srContentType);
            log.DebugFormat("is shared='{0}'", _srIsShared);

            string secretuuid = null;
            string value;
            if (_dconf.TryGetValue("cifspassword", out value))
            {
                secretuuid = CreateSecret("cifspassword", value);
            }
            else if (_dconf.TryGetValue("password", out value))
            {
                secretuuid = CreateSecret("password", value);
            }
            else if (_dconf.TryGetValue("chappassword", out value))
            {
                secretuuid = CreateSecret("chappassword", value);
            }

            Description = Messages.ACTION_SR_CREATING;
            XenRef<SR> sr;
            try
            {
                sr = XenAPI.SR.create(Session, Host.opaque_ref, _dconf, 0,
                                                  _srName, _srDescription, _srType.ToString().ToLowerInvariant(),
                                                  _srContentType,
                                                  _srIsShared, 
                                                  _smconf ?? new Dictionary<string, string>());

                Result = sr;
            }
            catch
            {
                if (!string.IsNullOrEmpty(secretuuid))
                {
                    string opaqref = Secret.get_by_uuid(Session, secretuuid);
                    Secret.destroy(Session, opaqref);
                }
                throw;
            }
            finally 
            {
                // Destroy secret after the SR creation is complete. This is safe
                // since all PBDs will have duplicated the secret (CA-113396).
                //
                // We do this on a best-effort basis because some types of errors
                // mean the secret was never actually created, so the operation will
                // fail, masking any earlier error (CA-145254), or causing a successful
                // SR.create to be reported as an error. The worst that can happen is
                // that an unused secret will be left lying around without warning.
                try
                {
                    if (!string.IsNullOrEmpty(secretuuid) && Helpers.CreedenceOrGreater(Connection))
                    {
                        string opaqref = Secret.get_by_uuid(Session, secretuuid);
                        Secret.destroy(Session, opaqref);
                    }
                }
                catch { }
            }

            log.Debug("Checking that SR.create() actually succeeded");
            foreach (XenRef<PBD> pbdRef in XenAPI.SR.get_PBDs(Session, sr.opaque_ref))
            {
                if (!XenAPI.PBD.get_currently_attached(Session, pbdRef))
                {
                    // The automatic plug done by the SR.create has failed to plug this PDB:
                    // try the plug manually, and report the failure. Roll back the operation
                    // by forgetting the SR.
                    try
                    {
                        XenAPI.PBD.plug(this.Session, pbdRef);
                    }
                    catch (Exception exn)
                    {
                        if (exn is Failure)
                        {
                            Failure f = (Failure)exn;
                            if (f.ErrorDescription[0] == Failure.HOST_OFFLINE ||
                                f.ErrorDescription[0] == Failure.HOST_STILL_BOOTING)
                            {
                                log.Warn("Unable to check storage settings, due to host being down", f);
                            }
                        }
                        else
                        {
                            log.Debug("Plug failed on a PBD: performing SR.forget");
                            ForgetFailedSR(sr);
                            throw;
                        }
                    }
                }
            }

            Dictionary<string, string> other_config = new Dictionary<string, string>();
            other_config.Add("auto-scan", _srContentType == XenAPI.SR.Content_Type_ISO ? "true" : "false");
            XenAPI.SR.set_other_config(Session, Result, other_config);

            if (isFirstSharedNonISOSR())
            {
                SR new_sr = Connection.WaitForCache(new XenRef<SR>(Result), GetCancelling);
                if (Cancelling)
                    throw new CancelledException();
                if (new_sr == null)
                    throw new Failure(Failure.HANDLE_INVALID, "SR", Result);

                // Set this SR to be the default
                new SrAction(SrActionKind.SetAsDefault, new_sr).RunExternal(Session);
            }

            Description = Messages.ACTION_SR_CREATE_SUCCESSFUL;
        }

        private string CreateSecret(string key, string value)
        {
            _dconf.Remove(key);
            string uuid = Secret.CreateSecret(Session, value);
            _dconf[string.Format("{0}_secret", key)] = uuid;
            return uuid;
        }

        /// <summary>
        /// Try to forget an SR that has previously failed to completely plug.  Nothrow guarantee.
        /// </summary>
        private void ForgetFailedSR(XenRef<SR> sr)
        {
            try
            {
                foreach (XenRef<PBD> pbd in XenAPI.SR.get_PBDs(Session, sr.opaque_ref))
                {
                    if (PBD.get_currently_attached(Session, pbd))
                        PBD.unplug(Session, pbd);
                }
                XenAPI.SR.forget(Session, sr.opaque_ref);
            }
            catch
            {
                log.Debug("SR.forget() failed! Continuing anyway");
            }
        }

        private bool isFirstSharedNonISOSR()
        {
            if (_srType == XenAPI.SR.SRTypes.iso || !_srIsShared)
                return false;
            foreach (SR sr in Connection.Cache.SRs)
            {
                if (sr.opaque_ref != Result &&
                    sr.GetSRType(false) != XenAPI.SR.SRTypes.iso &&
                    sr.shared)
                    return false;
            }
            return true;
        }
    }
}
