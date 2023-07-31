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
using System.Drawing;
using System.Linq;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public class LicenseDataGridViewRow : CheckableDataGridViewRow
    {
        public enum Status
        {
            Error,
            Warning,
            Ok,
            Updating,
            // CP-43000: to be used for post Nile hosts using trial edition
            Passable
        }

        private readonly ILicenseStatus licenseStatus;

        public LicenseDataGridViewRow(IXenObject xenObject = null)
            : this(xenObject, new LicenseStatus(xenObject))
        {
        }

        public LicenseDataGridViewRow(IXenObject xenObject, ILicenseStatus status)
            : base(xenObject)
        {
            licenseStatus = status;
            licenseStatus.ItemUpdated += licenseStatus_ItemUpdated;
        }

        public override void BeginCellUpdate()
        {
            licenseStatus.BeginUpdate();
        }

        private void licenseStatus_ItemUpdated()
        {
            Program.Invoke(DataGridView?.Parent, TriggerCellTextUpdatedEvent);
        }

        public override Queue<object> CellText
        {
            get
            {
                bool connectionUp = XenObject != null && XenObject.Connection.IsConnected;
                if(!connectionUp)
                    Disabled = true;

                Queue<object> cellDetails = new Queue<object>();
                cellDetails.Enqueue(XenObject?.Name());
                cellDetails.Enqueue(LicenseName);
                cellDetails.Enqueue(new Bitmap(1,1));
                cellDetails.Enqueue(OverallStatus);
                return cellDetails;
            }
        }

        public bool IsUsingLicenseServer
        {
            get { return !String.IsNullOrEmpty(LicenseServer); }
        }

        private Host XenObjectHost
        {
            get
            {
                var host = XenObject as Host;
                if (host != null)
                    return host;

                var pool = XenObject as Pool;
                if (pool != null)
                    return pool.Connection.Resolve(pool.master);

                return null;
            }
        }

        public override bool CellDataLoaded
        {
            get
            {
                return licenseStatus.Updated;
            }
        }

        public virtual string LicenseName
        {
            get
            {
                if (XenObject == null || XenObject.Connection == null || !XenObject.Connection.IsConnected)
                    return Messages.UNKNOWN;

                // for a pool, get the lowest license
                Pool pool = Helpers.GetPool(XenObject.Connection);
                if (pool != null)
                    return Helpers.GetFriendlyLicenseName(pool);

                if (XenObjectHost != null)
                    return Helpers.GetFriendlyLicenseName(XenObjectHost);

                return Messages.UNKNOWN;
            }
        }

        public override bool LicenseWarningRequired => ShouldShowLicenseWarningText(out _, out _);

        public override string LicenseWarningText
        {
            get
            {
                var _ = ShouldShowLicenseWarningText(out var text, out var _);
                return text;
            }
        }

        public override bool SupportWarningRequired => ShouldShowSupportWarningText(out _, out _);

        public override string SupportWarningText
        {
            get
            {
                var _ = ShouldShowSupportWarningText(out var text, out var _);
                return text;
            }
        }

        private bool ShouldShowSupportWarningText(out string text, out Status status)
        {
            text = null;
            status = Status.Ok;
            if (XenObjectHost == null || XenObjectHost.IsInPreviewRelease() || !Helpers.NileOrGreater(XenObjectHost))
            {
                return false;
            }

            if (XenObjectHost.CssLicenseHasExpired())
            {
                status = Status.Error;
                text = $"{Messages.LICENSE_MANAGER_EXPIRED_CSS_LONG}{Environment.NewLine}{Messages.EXPIRED_CSS_UPSELLING_MESSAGE_POOL}";
            }
            else
            {
                status = Status.Ok;
                text = Messages.LICENSE_MANAGER_ACTIVE_CSS_LONG;
            }

            return true;
        }

        private bool ShouldShowLicenseWarningText(out string text, out Status status)
        {
            text = null;
            status = Status.Ok;
            if (XenObjectHost == null)
            {
                return false;
            }

            switch (CurrentLicenseState)
            {
                case Dialogs.LicenseStatus.HostState.Free:
                {
                    var pool = Helpers.GetPool(XenObjectHost.Connection);
                    text = Dialogs.LicenseStatus.PoolIsMixedFreeAndExpiring(pool) ? Messages.POOL_IS_PARTIALLY_LICENSED : licenseStatus.LicenseEntitlements;
                    status = Helpers.CloudOrGreater(XenObjectHost) ? Status.Passable :Status.Error;
                }
                    break;
                case Dialogs.LicenseStatus.HostState.PartiallyLicensed:
                    text = Messages.POOL_IS_PARTIALLY_LICENSED;
                    status = Status.Warning;
                    break;
                case Dialogs.LicenseStatus.HostState.Licensed:
                {
                    var pool = Helpers.GetPool(XenObjectHost.Connection);
                    if (Dialogs.LicenseStatus.PoolHasMixedLicenses(pool))
                    {
                        text = Messages.POOL_HAS_MIXED_LICENSES;
                    }
                    else if (Dialogs.LicenseStatus.PoolIsPartiallyLicensed(pool))
                    {
                        text = Messages.POOL_IS_PARTIALLY_LICENSED;
                    }
                    else
                    {
                        text = licenseStatus.LicenseEntitlements;
                    }

                    status = Status.Ok;
                }
                    break;
                case Dialogs.LicenseStatus.HostState.Unavailable:
                    text = Messages.LICENSE_EXPIRED_NO_LICENSES_AVAILABLE;
                    status = Status.Error;
                    break;
                case Dialogs.LicenseStatus.HostState.Expired:
                    text = Messages.LICENSE_YOUR_LICENCE_HAS_EXPIRED;
                    status = Status.Error;
                    break;
                case Dialogs.LicenseStatus.HostState.RegularGrace:
                case Dialogs.LicenseStatus.HostState.UpgradeGrace:
                case Dialogs.LicenseStatus.HostState.ExpiresSoon:
                    text = string.Format(Messages.LICENSE_YOUR_LICENCE_EXPIRES_IN, licenseStatus.LicenseExpiresIn.FuzzyTime());
                    status = Status.Warning;
                    break;
                case Dialogs.LicenseStatus.HostState.Unknown:
                default:
                    status = licenseStatus.Updated ? Status.Warning : Status.Updating;
                    text = Messages.UNKNOWN;
                    return !licenseStatus.Updated;
            }

            return true;
        }

        public bool LicenseHelperUrlRequired => ShouldShowLicenseWarningText(out _, out var status) &&
                                                (status == Status.Error || status == Status.Warning || status == Status.Passable);

        public bool SupportHelperUrlRequired => ShouldShowSupportWarningText(out _, out  var status) &&
                                                (status == Status.Error || status == Status.Warning) &&
                                                !LicenseHelperUrlRequired;

        public Status RowStatus
        {
            get
            {
                ShouldShowLicenseWarningText(out _, out var licenseWarningStatus);
                ShouldShowSupportWarningText(out _, out var supportWarningStatus);

                if (!XenObjectHost.IsInPreviewRelease() && 
                    (licenseWarningStatus != supportWarningStatus || licenseWarningStatus == Status.Passable && supportWarningStatus == Status.Error)
                    )
                {
                    // will show a warning icon
                    return Status.Warning;
                }

                if (licenseWarningStatus != Status.Ok)
                {
                    return licenseWarningStatus;
                }

                if (supportWarningStatus != Status.Ok)
                {
                    return supportWarningStatus;
                }

                return Status.Ok;
            }
        }

        public Status RowLicenseStatus
        {
            get
            {
                var _ = ShouldShowLicenseWarningText(out var _, out var status);
                return status;
            }
        }

        public Status RowSupportStatus
        {
            get
            {
                var _ = ShouldShowSupportWarningText(out var _, out var status);
                return status;
            }
        }

        public string LicenseServer
        {
            get
            {
                if (licenseStatus.LicenseEdition != Host.Edition.Free)
                {
                    string address = LicenseServerAddress;
                    string port = LicenseServerPort;

                    if(!String.IsNullOrEmpty(port))
                    {
                        return String.Format(Messages.LICENSE_SERVER_PORT_FORMAT, address, port);
                    }

                    return address;
                }
                return string.Empty;
            }
        }

        public string LicenseServerAddress
        {
            get { return LicenseServerDetailKeyedOn("address"); }
        }

        public string LicenseServerPort
        {
            get { return LicenseServerDetailKeyedOn("port"); }
        }

        private string LicenseServerDetailKeyedOn(string key)
        {
            if (licenseStatus.LicenseEdition != Host.Edition.Free)
            {
                if (XenObjectHost.license_server.ContainsKey(key) && !string.IsNullOrEmpty(XenObjectHost.license_server[key]))
                {
                    return XenObjectHost.license_server[key];
                }
            }
            return string.Empty;
        }

        public bool CanUseLicenseServer
        {
            get { return !string.IsNullOrEmpty(XenObjectHost.edition); }
        }

        public bool HasLicenseServer
        {
            get { return !String.IsNullOrEmpty(LicenseServer); }
        }

        public virtual string LicenseProductVersion
        {
            get
            {
                if (XenObject != null && !XenObject.Connection.IsConnected)
                    return String.Empty;

                return XenObjectHost.ProductVersionTextShort();
            }
        }

        public DateTime? LicenseExpires
        {
            get { return licenseStatus.ExpiryDate; }
        }

        public TimeSpan LicenseExpiresIn
        {
            get { return licenseStatus.LicenseExpiresIn; }
        }


        public int NumberOfSockets
        {
            get
            {
                var h = XenObject as Host;
                if (h != null)
                    return h.CpuSockets();

                var p = XenObject as Pool;
                if (p != null)
                    return p.CpuSockets();
                return 0;
            }
        }

        public LicenseStatus.HostState CurrentLicenseState
        {
            get
            {
                if (XenObject != null && !XenObject.Connection.IsConnected)
                    return Dialogs.LicenseStatus.HostState.Unknown;
                
                return licenseStatus.CurrentState;
            }
        }

        public Host.Edition LicenseEdition
        {
            get { return licenseStatus.LicenseEdition; }
        }

        private string OverallStatus
        {
            get
            {
                var statuses = new[]
                {
                    LicenseStatus,
                    SupportStatus
                };

                return string.Join("; ", statuses.Where((s) => !string.IsNullOrEmpty(s)));
            }
        }

        private string LicenseStatus
        {
            get
            {
                switch (CurrentLicenseState)
                {
                    case Dialogs.LicenseStatus.HostState.PartiallyLicensed:
                        return Messages.PARTIALLY_LICENSED;
                    case Dialogs.LicenseStatus.HostState.Unavailable:
                    case Dialogs.LicenseStatus.HostState.Expired:
                        return Messages.LICENSE_UNLICENSED;
                    case Dialogs.LicenseStatus.HostState.Free:
                        return XenObjectHost.IsInPreviewRelease() ? Messages.LICENSE_TRIAL : Messages.LICENSE_UNLICENSED;
                    case Dialogs.LicenseStatus.HostState.Licensed:
                        return Messages.LICENSE_LICENSED;
                    case Dialogs.LicenseStatus.HostState.RegularGrace:
                    case Dialogs.LicenseStatus.HostState.UpgradeGrace:
                    case Dialogs.LicenseStatus.HostState.ExpiresSoon:
                        return licenseStatus.LicenseExpiresIn.FuzzyTime();
                    case Dialogs.LicenseStatus.HostState.Unknown:
                        if(!licenseStatus.Updated)
                            return Messages.LICENSE_UPDATING;
                        return Messages.UNKNOWN;
                    default:
                        return Messages.UNKNOWN;
                }
            }
        }

        private string SupportStatus
        {
            get
            {
                if (!ShouldShowSupportWarningText(out _, out var supportWarningStatus))
                {
                    return null;
                }

                return supportWarningStatus == Status.Ok ? Messages.LICENSE_MANAGER_ACTIVE_CSS : Messages.LICENSE_MANAGER_EXPIRED_CSS;
            }
        }

        public List<Host> RepresentedHosts
        {
            get
            {
                List<Host> hosts = new List<Host>();
                if (XenObject is Pool)
                    hosts.AddRange(XenObject.Connection.Cache.Hosts);
                if (XenObject is Host)
                    hosts.Add(XenObject as Host);
                return hosts;
            }
        }

        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    if(licenseStatus != null)
                        licenseStatus.Dispose();
                }

                disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
