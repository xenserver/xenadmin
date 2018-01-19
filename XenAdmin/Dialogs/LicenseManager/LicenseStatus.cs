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
using System.ComponentModel;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Utils;
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
        event LicenseStatus.StatusUpdatedEvent ItemUpdated;
        bool Updated { get; }
        void BeginUpdate();
        Host LicencedHost { get; }
        LicenseStatus.LicensingModel PoolLicensingModel { get; }
        string LicenseEntitlements { get; }
    }

    public class LicenseStatus : ILicenseStatus
    {
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

        private readonly EventHandlerList events = new EventHandlerList();
        protected EventHandlerList Events { get { return events; } }
        private const string StatusUpdatedEventKey = "LicenseStatusStatusUpdatedEventKey";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Host LicencedHost { get; private set; }
        private readonly AsyncServerTime serverTime = new AsyncServerTime();
        public delegate void StatusUpdatedEvent(object sender, EventArgs e);

        public static bool IsInfinite(TimeSpan span)
        {
            return span.TotalDays >= 3653;
        }

        public static bool IsGraceLicence(TimeSpan span)
        {
            return span.TotalDays < 30;
        }

        private IXenObject XenObject { get; set; }

        public bool Updated { get; set; }

        public LicenseStatus(IXenObject xo)
        {
            SetDefaultOptions();
            XenObject = xo;

            if (XenObject is Host)
                LicencedHost = XenObject as Host;
            if (XenObject is Pool)
            {
                Pool pool = XenObject as Pool;
                SetMinimumLicenseValueHost(pool);
            }

            serverTime.ServerTimeObtained -= ServerTimeUpdatedEventHandler;
            serverTime.ServerTimeObtained += ServerTimeUpdatedEventHandler;
            
            if (XenObject != null)
            {
                XenObject.Connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;
                XenObject.Connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
            }
        }

        void Connection_ConnectionStateChanged(object sender, EventArgs e)
        {
            if (LicencedHost != null)
            {
                TriggerStatusUpdatedEvent();
            }
        }

        private void SetMinimumLicenseValueHost(Pool pool)
        {
            LicencedHost = pool.Connection.Resolve(pool.master);

            if(LicencedHost == null)
                return;

            foreach (Host host in pool.Connection.Cache.Hosts)
            {
                if(host.LicenseExpiryUTC() < LicencedHost.LicenseExpiryUTC())
                    LicencedHost = host;
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
            serverTime.Fetch(LicencedHost);
        }

        private void ServerTimeUpdatedEventHandler(object sender, AsyncServerTimeEventArgs e)
        {
            if (!e.Success)
            {
                if(e.QueriedHost == null)
                {
                    log.ErrorFormat("Couldn't get the server time because: {0}", e.Failure.Message);
                    return;
                }

                log.ErrorFormat("Couldn't get the server time for {0} because: {1}", e.QueriedHost.name_label, e.Failure.Message);
                return;
            }

            if (LicencedHost != null)
            {
                CalculateLicenseState();
                TriggerStatusUpdatedEvent();
            }
        }

        protected void CalculateLicenseState()
        {
            PoolLicensingModel = GetLicensingModel(XenObject.Connection);
            LicenseExpiresExactlyIn = CalculateLicenceExpiresIn();
            CurrentState = CalculateCurrentState();
            Updated = true;
        }

        private void TriggerStatusUpdatedEvent()
        {
            StatusUpdatedEvent handler = Events[StatusUpdatedEventKey] as StatusUpdatedEvent;
            if (handler != null)
                handler.Invoke(this, EventArgs.Empty);
        }

        private bool InRegularGrace
        {
            get
            {
                return LicencedHost.license_params != null && LicencedHost.license_params.ContainsKey("grace") && LicenseExpiresIn.Ticks > 0 && LicencedHost.license_params["grace"] == "regular grace";
            }
        }

        private bool InUpgradeGrace
        {
            get
            {
                return LicencedHost.license_params != null && LicencedHost.license_params.ContainsKey("grace") && LicenseExpiresIn.Ticks > 0 && LicencedHost.license_params["grace"] == "upgrade grace";
            }
        }

        protected virtual TimeSpan CalculateLicenceExpiresIn()
        {
            //ServerTime is UTC
            DateTime currentRefTime = serverTime.ServerTime;
            return LicencedHost.LicenseExpiryUTC().Subtract(currentRefTime);
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

                var expiryGroups = from Host h in xenObject.Connection.Cache.Hosts
                                   let exp = h.LicenseExpiryUTC()
                                   group h by exp
                                   into g
                                   select new { ExpiryDate = g.Key, Hosts = g };

                if(expiryGroups.Count() > 1)
                {
                    expiryGroups.OrderBy(g => g.ExpiryDate);
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
            var pool = xenObject as Pool;
            if (pool != null)
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
        public event StatusUpdatedEvent ItemUpdated
        {
            add { Events.AddHandler(StatusUpdatedEventKey, value); }
            remove { Events.RemoveHandler(StatusUpdatedEventKey, value); }
        }

        public Host.Edition LicenseEdition { get { return Host.GetEdition(LicencedHost.edition); } }

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
                if (LicencedHost.license_params != null && LicencedHost.license_params.ContainsKey("expiry"))
                    return LicencedHost.LicenseExpiryUTC().ToLocalTime();
                return null;
            }
        }

        public LicensingModel PoolLicensingModel { get; private set; }

        public string LicenseEntitlements
        {
            get
            {
                if (PoolLicensingModel == LicensingModel.Creedence && CurrentState == HostState.Licensed)
                {
                    if (XenObject.Connection.Cache.Hosts.All(h => h.EnterpriseFeaturesEnabled()))
                        return Messages.LICENSE_SUPPORT_AND_ENTERPRISE_FEATURES_ENABLED;
                    if (XenObject.Connection.Cache.Hosts.All(h => h.DesktopPlusFeaturesEnabled()))
                        return Messages.LICENSE_SUPPORT_AND_DESKTOP_PLUS_FEATURES_ENABLED;
                    if (XenObject.Connection.Cache.Hosts.All(h => h.DesktopFeaturesEnabled()))
                        return Messages.LICENSE_SUPPORT_AND_DESKTOP_FEATURES_ENABLED;
                    if (XenObject.Connection.Cache.Hosts.All(h => h.DesktopCloudFeaturesEnabled()))
                        return Messages.LICENSE_SUPPORT_AND_DESKTOP_CLOUD_FEATURES_ENABLED;
                    if (XenObject.Connection.Cache.Hosts.All(h => h.PremiumFeaturesEnabled()))
                        return Messages.LICENSE_SUPPORT_AND_PREMIUM_FEATURES_ENABLED;
                    if (XenObject.Connection.Cache.Hosts.All(h => h.StandardFeaturesEnabled()))
                        return Messages.LICENSE_SUPPORT_AND_STANDARD_FEATURES_ENABLED;
                    if (XenObject.Connection.Cache.Hosts.All(h => h.EligibleForSupport()))
                        return Messages.LICENSE_SUPPORT_AND_STANDARD_FEATURES_ENABLED;
                    return Messages.LICENSE_NOT_ELIGIBLE_FOR_SUPPORT;
                }

                if (CurrentState == HostState.Free)
                {
                    return Messages.LICENSE_NOT_ELIGIBLE_FOR_SUPPORT;
                }

                return Messages.UNKNOWN;
            }
        }

        #endregion

        #region LicensingModel
        public enum LicensingModel
        {
            Clearwater,
            Creedence
        }

        public static LicensingModel GetLicensingModel(IXenConnection connection)
        {
            if (Helpers.CreedenceOrGreater(connection))
                return LicensingModel.Creedence;
            return LicensingModel.Clearwater;
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
                    if (serverTime != null)
                        serverTime.ServerTimeObtained -= ServerTimeUpdatedEventHandler;

                    if (XenObject != null && XenObject.Connection != null)
                        XenObject.Connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;

                    Events.Dispose();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
