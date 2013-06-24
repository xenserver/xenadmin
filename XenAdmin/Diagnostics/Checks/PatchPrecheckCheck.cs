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

using System;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Core;
using System.IO;
using XenAdmin.Diagnostics.Problems.HostProblem;
using System.Text.RegularExpressions;
using System.Xml;


namespace XenAdmin.Diagnostics.Checks
{
    class PatchPrecheckCheck : Check
    {
        private readonly Pool_patch _patch;

        private static Regex PrecheckErrorRegex = new Regex("(<error).+(</error>)");



        public PatchPrecheckCheck(Host host, Pool_patch patch)
            : base(host)
        {
            _patch = patch;
        }


        public override Problem RunCheck()
        {
            if (!Host.IsLive)
                return new HostNotLive(this, Host);

            if (!Host.Connection.IsConnected)
                throw new EndOfStreamException(Helpers.GetName(Host.Connection));

            Session session = Host.Connection.DuplicateSession();

            //
            // Check patch isn't already applied here
            //
            if (Patch.AppliedOn(Host) != DateTime.MaxValue)
            {
                return new PatchAlreadyApplied(this, Host);

            }

            try
            {
                return FindProblem(Pool_patch.precheck(session, Patch.opaque_ref, Host.opaque_ref), Host);

            }
            catch (Failure f)
            {
                log.Error(f.ToString());
                if(f.ErrorDescription.Count>0)
                    log.Error(f.ErrorDescription[0]);
                if (f.ErrorDescription.Count > 1)
                    log.Error(f.ErrorDescription[1]);
                if (f.ErrorDescription.Count > 2)
                    log.Error(f.ErrorDescription[2]);
                return new PrecheckFailed(this,Host, f);
            }
        }

        public override string Description
        {
            get { return Messages.SERVER_SIDE_CHECK_DESCRIPTION; }
        }

        public Pool_patch Patch
        {
            get { return _patch; }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        private Problem FindProblem(string result, Host host)
        {
            if (!PrecheckErrorRegex.IsMatch(result.Replace("\n", "")))
                return null;

            Match m = PrecheckErrorRegex.Match(result.Replace("\n", ""));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(m.ToString());
            log.Error(m.ToString());
            XmlNode errorNode = doc.FirstChild;

            string errorcode = errorNode.Attributes["errorcode"] != null
                                   ? errorNode.Attributes["errorcode"].Value
                                   : string.Empty;

            switch (errorcode)
            {
                case "PATCH_PRECHECK_FAILED_WRONG_SERVER_VERSION":
                    foreach (XmlNode node in errorNode.ChildNodes)
                    {
                        if (node.Name == "required")
                        {
                            return new WrongServerVersion(this, node.InnerXml,host);
                        }
                    }
                    break;
                case "":
                    return null;
                default:
                    return new PrecheckFailed(this, host,new Failure(errorcode));
            }
            return null;
        }
    }
}
