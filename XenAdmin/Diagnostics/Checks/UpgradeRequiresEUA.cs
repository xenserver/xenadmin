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
using System.Linq;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.UtilityProblem;

namespace XenAdmin.Diagnostics.Checks
{
    internal class UpgradeRequiresEua : UpgradeCheck
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly Uri _targetUri;
        private readonly HashSet<string> _euas;
        private readonly HashSet<IXenObject> _hostsFailedToFetchEua;
        public UpgradeRequiresEua(List<Host> hosts, IReadOnlyDictionary<string, string> installMethodConfig)
            : base(hosts)
        {
            if (installMethodConfig == null || !installMethodConfig.TryGetValue("url", out var uriText))
                return;

            try
            {
                _targetUri = new Uri(uriText);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            _euas = new HashSet<string>();
            _hostsFailedToFetchEua = new HashSet<IXenObject>();
        }

        public override bool CanRun() => _targetUri != null && Hosts.Any(Helpers.Post82X);

        private void FetchHostEua(Host host)
        {
            string eua = null;
            if (Helpers.Post82X(host) && !Helpers.TryLoadHostEua(host, _targetUri, out eua))
            {
                lock (_hostsFailedToFetchEua)
                {
                    _hostsFailedToFetchEua.Add(host);
                }
                return;
            }

            lock (_euas)
            {
                _euas.Add(eua);
            }
        }
        protected override Problem RunCheck()
        {
            Hosts.AsParallel().ForAll(FetchHostEua);
            lock (_hostsFailedToFetchEua)
            {
                if (_hostsFailedToFetchEua.Count > 0)
                {
                    return new EuaNotFoundProblem(this, _hostsFailedToFetchEua.ToList());
                }
            }
            lock (_euas)
            {
                return new EuaNotAcceptedProblem( this, _euas.ToList());
            }
        }

        public override string Description => Messages.ACCEPT_EUA_CHECK_DESCRIPTION;

        public override bool Equals(object obj)
        {
            if (!(obj is UpgradeRequiresEua item))
            {
                return false;
            }

            // if the number of hosts that failed to fetch the EUA is not zero, we 
            // never consider the Checks equal. This is because this Check is used as a
            // permanent check
            lock (_hostsFailedToFetchEua)
            {
                if (_hostsFailedToFetchEua != null)
                {
                    if (_hostsFailedToFetchEua.Count > 0)
                    {
                        return false;
                    }

                    lock (item._hostsFailedToFetchEua)
                    {
                        if (item._hostsFailedToFetchEua != null && item._hostsFailedToFetchEua.Count > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return _targetUri.Equals(item._targetUri) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _targetUri.GetHashCode() ^ base.GetHashCode();
        }
    }
}
