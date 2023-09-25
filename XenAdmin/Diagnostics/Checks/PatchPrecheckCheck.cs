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
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Core;
using System.IO;
using XenAdmin.Diagnostics.Problems.HostProblem;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Problems.PoolProblem;

namespace XenAdmin.Diagnostics.Checks
{
    class PatchPrecheckCheck : HostCheck
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Pool_update Update;

        private static Regex PrecheckErrorRegex = new Regex("(<error).+(</error>)");

        private readonly Dictionary<string, livepatch_status> livePatchCodesByHost;
        private  SR srUploadedUpdates;

        public PatchPrecheckCheck(Host host, Pool_update update, Dictionary<string, livepatch_status> livePatchCodesByHost, SR srUploadedUpdates = null)
            : base(host)
        {
            Update = update;
            this.livePatchCodesByHost = livePatchCodesByHost;
            this.srUploadedUpdates = srUploadedUpdates;
        }

        protected override Problem RunCheck()
        {
            //
            // Check that the SR where the update was uploaded is still attached
            //
            if (srUploadedUpdates != null
                && ((srUploadedUpdates.shared && !srUploadedUpdates.CanBeSeenFrom(Host))
                 || (!srUploadedUpdates.shared && srUploadedUpdates.IsBroken())))
            {
                return new BrokenSRWarning(this, Host, srUploadedUpdates);
            }

            //
            // Check patch isn't already applied here
            //
            if (Update != null && Update.AppliedOn(Host))
            {
                return new PatchAlreadyApplied(this, Host);
            }
            
            if (!Host.IsLive())
                return new HostNotLiveWarning(this, Host);

            if (!Host.Connection.IsConnected)
                throw new EndOfStreamException(Helpers.GetName(Host.Connection));

            Session session = Host.Connection.DuplicateSession();

            try
            {
                if (Update!= null)
                {
                    var livepatchStatus = Pool_update.precheck(session, Update.opaque_ref, Host.opaque_ref);

                    log.DebugFormat("Pool_update.precheck returned livepatch_status: '{0}'", livepatchStatus);

                    if (livePatchCodesByHost != null)
                        livePatchCodesByHost[Host.uuid] = livepatchStatus;
                }
            }
            catch (Failure f)
            {
                log.Error(string.Join(",", f.ErrorDescription.ToArray()), f);

                // try and find problem from the xapi failure
                Problem problem = FindProblem(f);
                return problem ?? new PrecheckFailed(this, Host, f);
            }

            return null;
        }

        public override string Description => Messages.SERVER_SIDE_CHECK_DESCRIPTION;

        /// <summary>
        /// Find problem from xml result
        /// </summary>
        /// <param name="result">xml result, as returned by Pool_patch.precheck() call.
        ///     E.g.:
        ///     <error errorcode="PATCH_PRECHECK_FAILED_OUT_OF_SPACE">
        ///         <found>2049859584</found>
        ///         <required>10000000000</required>
        ///     </error> 
        /// </param>
        /// <returns>Problem or null, if no problem found</returns>
        private Problem FindProblem(string result)
        {
            if (!PrecheckErrorRegex.IsMatch(result.Replace("\n", "")))
                return null;

            Match m = PrecheckErrorRegex.Match(result.Replace("\n", ""));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(m.ToString());
            log.Error(m.ToString());
            XmlNode errorNode = doc.FirstChild;

            string errorcode = errorNode.Attributes != null && errorNode.Attributes["errorcode"] != null
                                   ? errorNode.Attributes["errorcode"].Value
                                   : string.Empty;

            if (errorcode == "")
                return null;

            var found = "";
            var required = "";

            foreach (XmlNode node in errorNode.ChildNodes)
            {
                if (node.Name == "found")
                    found = node.InnerXml;
                else if (node.Name == "required")
                    required = node.InnerXml;
            }
            var problem = FindProblem(errorcode, "", found, required);
            return problem ?? new PrecheckFailed(this, Host, new Failure(errorcode));
        }

        /// <summary>
        /// Find problem from xapi Failure
        /// </summary>
        /// <param name="failure">Xapi failure, thrown by Pool_patch.precheck() call.
        ///   E.g.: failure.ErrorDescription.Count = 4
        ///         ErrorDescription[0] = "PATCH_PRECHECK_FAILED_WRONG_SERVER_VERSION"
        ///         ErrorDescription[1] = "OpaqueRef:612b5eee-03dc-bbf5-3385-6905fdc9b079"
        ///         ErrorDescription[2] = "6.5.0"
        ///         ErrorDescription[3] = "^6\\.2\\.0$"
        ///
        ///   E.g.: failure.ErrorDescription.Count = 2
        ///         ErrorDescription[0] = "OUT_OF_SPACE"
        ///         ErrorDescription[1] = "/var/patch"
        ///
        ///   E.g.: failure.ErrorDescription.Count = 3 with the last parameter being an xml string
        ///         ErrorDescription[0] = "UPDATE_PRECHECK_FAILED_UNKNOWN_ERROR"
        ///         ErrorDescription[1] = "test-update"
        ///         ErrorDescription[2] = "<?x m l version="1.0" ?><error errorcode="LICENCE_RESTRICTION"></error>"
        ///
        ///   E.g.: failure.ErrorDescription.Count = 3 with the last parameter being a plain string
        ///         ErrorDescription[0] = "UPDATE_PRECHECK_FAILED_UNKNOWN_ERROR"
        ///         ErrorDescription[1] = "CH82"
        ///         ErrorDescription[2] = "VSWITCH_CONTROLLER_CONNECTED\nYou must [...] this update\n"
        /// </param>
        /// <returns>Problem or null, if no problem found</returns>
        private Problem FindProblem(Failure failure)
        {
            if (failure.ErrorDescription.Count == 0)
                return null;

            var errorcode = failure.ErrorDescription[0];
            var param1 = failure.ErrorDescription.Count > 1 ? failure.ErrorDescription[1] : "";
            var param2 = failure.ErrorDescription.Count > 2 ? failure.ErrorDescription[2] : "";
            var param3 = failure.ErrorDescription.Count > 3 ? failure.ErrorDescription[3] : "";

            return FindProblem(errorcode, param1, param2, param3);  
        }

        private Problem FindProblem(string errorcode, string param1, string param2, string param3)
        {
            switch (errorcode)
            {
                case "UPDATE_PRECHECK_FAILED_WRONG_SERVER_VERSION":
                    return new WrongServerVersion(this, Host);

                case "UPDATE_PRECHECK_FAILED_CONFLICT_PRESENT":
                    return new ConflictingUpdatePresent(this, param2, Host);

                case "UPDATE_PRECHECK_FAILED_PREREQUISITE_MISSING":
                    return new PrerequisiteUpdateMissing(this, param3, Host);

                case "UPDATE_PRECHECK_FAILED_OUT_OF_SPACE":
                    long.TryParse(param2, out var foundSpace);
                    long.TryParse(param3, out var requiredSpace);

                    var diskSpaceReq = new DiskSpaceRequirements(DiskSpaceRequirements.OperationTypes.install, Host, Update.Name(), requiredSpace, foundSpace, 0);

                    return new HostOutOfSpaceProblem(this, Host, Update, diskSpaceReq);

                case "OUT_OF_SPACE":
                    if (Update != null)
                    {
                        var action = new GetDiskSpaceRequirementsAction(Host, Update.Name(), Update.installation_size, true);
                        try
                        {
                            action.RunSync(action.Session);
                        }
                        catch
                        {
                            log.WarnFormat("Could not get disk space requirements");
                        }
                        if (action.Succeeded)
                            return new HostOutOfSpaceProblem(this, Host, Update, action.DiskSpaceRequirements);
                    }
                    break;

                case "LICENCE_RESTRICTION":
                    return new LicenseRestrictionProblem(this, Host);

                case "UPDATE_PRECHECK_FAILED_UNKNOWN_ERROR":
                    if (param1 == "CH82" && param2.StartsWith("VSWITCH_CONTROLLER_CONNECTED"))
                    {
                        var pool = Helpers.GetPoolOfOne(Host.Connection);
                        if (pool.vSwitchController())
                            return new VSwitchControllerProblem(this, pool);
                        return null;
                    }
                    else
                        return FindProblem(param2);
            }
            return null;
        }
    }
}
