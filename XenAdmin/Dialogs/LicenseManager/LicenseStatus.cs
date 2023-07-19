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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public interface ILicenseStatus : IDisposable
    {
        LicenseStatus.HostState CurrentState { get; }
        Host.Edition LicenseEdition { get; }
        TimeSpan LicenseExpiresIn { get; }
        TimeSpan LicenseExpiresExactlyIn { get; }
        DateTime? ExpiryDate { get; }
        event Action ItemUpdated;
        bool Updated { get; }
        void BeginUpdate();
        Host LicensedHost { get; }
        string LicenseEntitlements { get; }
    }

    public class LicenseStatus : ILicenseStatus
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum HostState
        {
            Unknown,
            Expired,
            ExpiresSoon,
            RegularGrace,
            UpgradeGrace,
            Licensed,
            PartiallyLicensed,
            Free,
            Unavailable
        }

        private readonly EventHandlerList _events = new EventHandlerList();

        private const string StatusUpdatedEventKey = "LicenseStatusStatusUpdatedEventKey";

        public Host LicensedHost { get; private set; }

        public static bool IsInfinite(TimeSpan span)
        {
            return span.TotalDays >= 3653;
        }

        public static bool IsGraceLicence(TimeSpan span)
        {
            return span.TotalDays < 30;
        }

        private IXenObject XenObject { get; }

        public bool Updated { get; private set; }

        public LicenseStatus(IXenObject xo)
        {
            SetDefaultOptions();
            XenObject = xo;

            if (XenObject is Host host)
                LicensedHost = host;
            if (XenObject is Pool pool)
                SetMinimumLicenseValueHost(pool);

            if (XenObject != null)
            {
                XenObject.Connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;
                XenObject.Connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
            }
        }

        private void Connection_ConnectionStateChanged(IXenConnection conn)
        {
            if (LicensedHost != null)
            {
                TriggerStatusUpdatedEvent();
            }
        }

        private void SetMinimumLicenseValueHost(Pool pool)
        {
            LicensedHost = pool.Connection.Resolve(pool.master);

            if(LicensedHost == null)
                return;

            foreach (Host host in pool.Connection.Cache.Hosts)
            {
                if(host.LicenseExpiryUTC() < LicensedHost.LicenseExpiryUTC())
                    LicensedHost = host;
            }
        }

        private void SetDefaultOptions()
        {
            CurrentState = HostState.Unknown;
            Updated = false;
            LicenseExpiresExactlyIn = new TimeSpan();
        }

        public void BeginUpdate()
        {
            SetDefaultOptions();
            ThreadPool.QueueUserWorkItem(GetServerTime, LicensedHost);
        }

        private void GetServerTime(object state)
        {
            Host host = state as Host;
            if (host?.Connection?.Session == null)
            {
                log.Error("Will not fetch server time: host or connection could not be resolved");
                return;
            }

            try
            {
                //Note we're using the get_servertime call which returns the UTC time
                var serverTime = Host.get_servertime(host.Connection.Session, host.opaque_ref);

                if (LicensedHost != null)
                {
                    //ServerTime is UTC
                    DateTime currentRefTime = serverTime;
                    LicenseExpiresExactlyIn = LicensedHost.LicenseExpiryUTC().Subtract(currentRefTime);

                    CurrentState = CalculateCurrentState();
                    Updated = true;

                    TriggerStatusUpdatedEvent();
                }
            }
            catch (Exception e)
            {
                log.Error($"Failed to fetch server time for host {host.name_label}: ", e);
            }
        }

        private void TriggerStatusUpdatedEvent()
        {
            if (_events[StatusUpdatedEventKey] is Action handler)
                handler.Invoke();
        }

        private bool InRegularGrace
        {
            get
            {
                return LicensedHost.license_params != null && LicensedHost.license_params.ContainsKey("grace") && LicenseExpiresIn.Ticks > 0 && LicensedHost.license_params["grace"] == "regular grace";
            }
        }

        private bool InUpgradeGrace
        {
            get
            {
                return LicensedHost.license_params != null && LicensedHost.license_params.ContainsKey("grace") && LicenseExpiresIn.Ticks > 0 && LicensedHost.license_params["grace"] == "upgrade grace";
            }
        }

        internal static bool PoolIsMixedFreeAndExpiring(IXenObject xenObject)
        {
            if (xenObject is Pool)
            {
                if (xenObject.Connection.Cache.Hosts.Length == 1)
                    return false;

                int freeCount = xenObject.Connection.Cache.Hosts.Count(h => Host.GetEdition(h.edition) == Host.Edition.Free);
                if (freeCount == 0 || freeCount < xenObject.Connection.Cache.Hosts.Length)
                    return false;

                var expiryGroups = (from Host h in xenObject.Connection.Cache.Hosts
                                   let exp = h.LicenseExpiryUTC()
                                   group h by exp
                                   into g
                                   select new { ExpiryDate = g.Key, Hosts = g }).ToList();

                if (expiryGroups.Count > 1)
                {
                    expiryGroups = expiryGroups.OrderBy(g => g.ExpiryDate).ToList();
                    if ((expiryGroups.ElementAt(1).ExpiryDate - expiryGroups.ElementAt(0).ExpiryDate).TotalDays > 30)
                        return true;
                }
            }
            return false;
        }

        internal static bool PoolIsPartiallyLicensed(IXenObject xenObject)
        {
            if (xenObject is Pool)
            {
                if (xenObject.Connection.Cache.Hosts.Length == 1)
                    return false;

                int freeCount = xenObject.Connection.Cache.Hosts.Count(h => Host.GetEdition(h.edition) == Host.Edition.Free);
                return freeCount > 0 && freeCount < xenObject.Connection.Cache.Hosts.Length;
            }
            return false;
        }

        internal static bool PoolHasMixedLicenses(IXenObject xenObject)
        {
            if (xenObject is Pool pool)
            {
                if (xenObject.Connection.Cache.Hosts.Length == 1)
                    return false;

                if (xenObject.Connection.Cache.Hosts.Any(h => Host.GetEdition(h.edition) == Host.Edition.Free))
                    return false;

                var licenseGroups = from Host h in xenObject.Connection.Cache.Hosts
                                    let ed = Host.GetEdition(h.edition)
                                    group h by ed;

                return licenseGroups.Count() > 1;
            }
            return false;
        }

        private HostState CalculateCurrentState()
        {
            if (ExpiryDate.HasValue && ExpiryDate.Value.Day == 1 && ExpiryDate.Value.Month == 1 && ExpiryDate.Value.Year == 1970)
            {
                return HostState.Unavailable;
            }

            if (PoolIsPartiallyLicensed(XenObject))
                return HostState.PartiallyLicensed;

            if (LicenseEdition == Host.Edition.Free)
                return HostState.Free;

            if (!IsGraceLicence(LicenseExpiresIn))
                return HostState.Licensed;

            if (IsInfinite(LicenseExpiresIn))
            {
                return HostState.Licensed;
            }

            if (LicenseExpiresIn.Ticks <= 0)
            {
                return HostState.Expired;
            }

            if (IsGraceLicence(LicenseExpiresIn))
            {
                if (InRegularGrace)
                    return  HostState.RegularGrace;
                if (InUpgradeGrace)
                    return HostState.UpgradeGrace;

                return HostState.ExpiresSoon;
            }

            return LicenseEdition ==  Host.Edition.Free ? HostState.Free : HostState.Licensed;
        }

        #region ILicenseStatus Members
        public event Action ItemUpdated
        {
            add => _events.AddHandler(StatusUpdatedEventKey, value);
            remove => _events.RemoveHandler(StatusUpdatedEventKey, value);
        }

        public Host.Edition LicenseEdition => Host.GetEdition(LicensedHost.edition);

        public HostState CurrentState { get; private set; }

        public TimeSpan LicenseExpiresExactlyIn { get; private set; }

        /// <summary>
        /// License expiry, just days, hrs, mins
        /// </summary>
        public TimeSpan LicenseExpiresIn
        { 
            get
            {
                return new TimeSpan(LicenseExpiresExactlyIn.Days, LicenseExpiresExactlyIn.Hours, LicenseExpiresExactlyIn.Minutes, 0, 0);
            }
        }

        public DateTime? ExpiryDate
        {
            get
            {
                if (LicensedHost.license_params != null && LicensedHost.license_params.ContainsKey("expiry"))
                    return LicensedHost.LicenseExpiryUTC().ToLocalTime();
                return null;
            }
        }

        public string LicenseEntitlements
        {
            get
            {
                var hosts = XenObject.Connection.Cache.Hosts;
                var entitlements = new List<string>();
                string supportLevel = null;
                if (CurrentState == HostState.Licensed)
                {
                    if (hosts.All(h => h.EnterpriseFeaturesEnabled()))
                        supportLevel = Messages.LICENSE_ENTERPRISE_FEATURES_ENABLED;
                    else if (hosts.All(h => h.DesktopPlusFeaturesEnabled()))
                        supportLevel = string.Format(Messages.LICENSE_DESKTOP_PLUS_FEATURES_ENABLED, BrandManager.CompanyNameLegacy);
                    else if (hosts.All(h => h.DesktopFeaturesEnabled()))
                        supportLevel = string.Format(Messages.LICENSE_DESKTOP_FEATURES_ENABLED, BrandManager.CompanyNameLegacy);
                    else if (hosts.All(h => h.DesktopCloudFeaturesEnabled()))
                        supportLevel = string.Format(Messages.LICENSE_DESKTOP_CLOUD_FEATURES_ENABLED, BrandManager.CompanyNameLegacy);
                    else if (hosts.All(h => h.PremiumFeaturesEnabled()))
                        supportLevel = Messages.LICENSE_PREMIUM_FEATURES_ENABLED;
                    else if (hosts.All(h => h.StandardFeaturesEnabled()))
                        supportLevel = Messages.LICENSE_STANDARD_FEATURES_ENABLED;
                    else if (hosts.All(h => h.EligibleForSupport()))
                        supportLevel = Messages.LICENSE_STANDARD_FEATURES_ENABLED;

                    if (supportLevel != null)
                    {
                        entitlements.Add(hosts.Any(Helpers.Post82X) ? Messages.LICENSE_MANAGER_LICENSED : Messages.LICENSE_ELIGIBLE_FOR_SUPPORT);
                        entitlements.Add(supportLevel);
                    }
                } 
                
                if (CurrentState == HostState.Free || string.IsNullOrEmpty(supportLevel))
                {
                    if (hosts.Any(Helpers.Post82X))
                    {
                        // CP-43000: for hosts in preview we show "Licensed" even though they're not
                        entitlements.Add(hosts.Any(a => a.IsInPreviewRelease()) ? $"{Messages.LICENSE_MANAGER_LICENSED}{Environment.NewLine}{Messages.LICENSE_MANAGER_TRIAL_EDITION}" : Messages.LICENSE_MANAGER_TRIAL_LICENSE);
                    }
                    else
                    {
                        entitlements.Add(Messages.LICENSE_NOT_ELIGIBLE_FOR_SUPPORT);
                    }
                }

                if (hosts.Any(h => h.CanShowTrialEditionUpsell()))
                    entitlements.Add(Messages.TRIAL_EDITION_UPSELLING_MESSAGE);

                return entitlements.Count == 0 ? Messages.UNKNOWN : string.Join(Environment.NewLine, entitlements);
            }
        }

        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        public void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    if (XenObject != null && XenObject.Connection != null)
                        XenObject.Connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;

                    _events.Dispose();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
