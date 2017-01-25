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
using System.Drawing;
using System.Linq;
using XenAdmin.Controls;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public class LicenseDataGridViewRow : CheckableDataGridViewRow
    {
        public enum Status
        {
            Warning,
            Information,
            Ok,
            Updating
        }

        private readonly ILicenseStatus licenseStatus;

        public LicenseDataGridViewRow() : this(null)
        {
            
        }

        public LicenseDataGridViewRow(IXenObject xenObject) : this(xenObject, new LicenseStatus(xenObject))
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

        private bool refreshing = false;
        private void licenseStatus_ItemUpdated(object sender, EventArgs e)
        {
            if (refreshing)
                return;

            // check if we need to do a full refresh (i.e. pool members need to be displayed as individual items in the list)
            if (RowShouldBeExpanded(XenObject) && DataGridView is LicenseCheckableDataGridView)
            {
                refreshing = true;
                Program.Invoke(Program.MainWindow, TriggerRefreshAllEvent);  
            }
            else
                Program.Invoke(Program.MainWindow, TriggerCellTextUpdatedEvent);
        }

        public static bool RowShouldBeExpanded(IXenObject xenObject)
        {
            return xenObject is Pool && xenObject.Connection.Cache.Hosts.Length > 1 
                && LicenseActivationRequest.CanActivate(xenObject as Pool);
        }

        public override Queue<object> CellText
        {
            get
            {
                bool connectionUp = XenObject != null && XenObject.Connection.IsConnected;
                if(!connectionUp)
                    Disabled = true;

                Queue<object> cellDetails = new Queue<object>();
                cellDetails.Enqueue(XenObject.Name);
                cellDetails.Enqueue(LicenseName);
                cellDetails.Enqueue(new Bitmap(1,1));
                cellDetails.Enqueue(LicenseStatus);
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
                if(XenObject is Host)
                    return XenObject as Host;
                if(XenObject is Pool)
                {
                   Pool pool = XenObject as Pool;
                   return pool.Connection.Resolve(pool.master);
                }
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
                if (XenObject != null && !XenObject.Connection.IsConnected)
                    return Messages.UNKNOWN;

                // for a pool, get the lowest license, i.e. pool.LicenseString
                Pool pool = Helpers.GetPool(XenObjectHost.Connection);

                if (pool == null || XenObject is Host)
                    return Helpers.GetFriendlyLicenseName(XenObjectHost);

                return pool.LicenseString;
            } 
        }

        public override bool WarningRequired
        {
            get
            {
                if (!licenseStatus.Updated)
                    return false;

                if (CurrentLicenseState == Dialogs.LicenseStatus.HostState.Unknown)
                    return false;

                if (licenseStatus.PoolLicensingModel == Dialogs.LicenseStatus.LicensingModel.Creedence)
                    return true;

                Pool pool = Helpers.GetPool(XenObjectHost.Connection);

                if (CurrentLicenseState == Dialogs.LicenseStatus.HostState.Free)
                    return Dialogs.LicenseStatus.PoolIsMixedFreeAndExpiring(pool)
                           || licenseStatus.PoolLicensingModel == Dialogs.LicenseStatus.LicensingModel.Clearwater;
                
                if (CurrentLicenseState == Dialogs.LicenseStatus.HostState.Licensed)
                    return Dialogs.LicenseStatus.PoolIsPartiallyLicensed(pool)
                           || Dialogs.LicenseStatus.PoolHasMixedLicenses(pool);

                return true;
            }
        }

        public override string WarningText
        {
            get
            {
                switch (CurrentLicenseState)
                {
                    case Dialogs.LicenseStatus.HostState.Free:
                        {
                            Pool pool = Helpers.GetPool(XenObjectHost.Connection);
                            if (Dialogs.LicenseStatus.PoolIsMixedFreeAndExpiring(pool))
                                return Messages.POOL_IS_PARTIALLY_LICENSED;
                            return licenseStatus.LicenseEntitlements;
                        }
                    case Dialogs.LicenseStatus.HostState.PartiallyLicensed:
                        return Messages.POOL_IS_PARTIALLY_LICENSED;
                    case Dialogs.LicenseStatus.HostState.Licensed:
                        {
                            Pool pool = Helpers.GetPool(XenObjectHost.Connection);
                            if (Dialogs.LicenseStatus.PoolHasMixedLicenses(pool))
                                return Messages.POOL_HAS_MIXED_LICENSES;

                            if (Dialogs.LicenseStatus.PoolIsPartiallyLicensed(pool))
                                return Messages.POOL_IS_PARTIALLY_LICENSED;

                            return licenseStatus.LicenseEntitlements;
                        }
                    case Dialogs.LicenseStatus.HostState.Unavailable:
                        return Messages.LICENSE_EXPIRED_NO_LICENSES_AVAILABLE;
                    case Dialogs.LicenseStatus.HostState.Expired:
                        return licenseStatus.PoolLicensingModel == Dialogs.LicenseStatus.LicensingModel.Clearwater ? Messages.LICENSE_SA_EXPIRED : Messages.LICENSE_YOUR_LICENCE_HAS_EXPIRED;
                    case Dialogs.LicenseStatus.HostState.RegularGrace:
                    case Dialogs.LicenseStatus.HostState.UpgradeGrace:
                    case Dialogs.LicenseStatus.HostState.ExpiresSoon:
                        if (licenseStatus.PoolLicensingModel == Dialogs.LicenseStatus.LicensingModel.Clearwater)
                            return string.Format(Messages.LICENSE_SA_EXPIRES_IN, licenseStatus.LicenseExpiresIn.FuzzyTime());
                        return string.Format(Messages.LICENSE_YOUR_LICENCE_EXPIRES_IN, licenseStatus.LicenseExpiresIn.FuzzyTime());
                    default:
                        return Messages.UNKNOWN;
                }
            }
        }

        public bool HelperUrlRequired
        {
            get { return XenObject == null ? false : Helpers.ClearwaterOrGreater(XenObject.Connection); }  // CA-115256
        }

        public Status RowStatus
        {
            get
            {
                switch (CurrentLicenseState)
                {
                    case Dialogs.LicenseStatus.HostState.Unavailable:
                    case Dialogs.LicenseStatus.HostState.Expired:
                        return Status.Warning;
                    case Dialogs.LicenseStatus.HostState.Free:
                        return licenseStatus.PoolLicensingModel == Dialogs.LicenseStatus.LicensingModel.PreClearwater ? Status.Ok : Status.Warning;
                    case Dialogs.LicenseStatus.HostState.Licensed:
                        return Status.Ok;
                    case Dialogs.LicenseStatus.HostState.PartiallyLicensed:
                    case Dialogs.LicenseStatus.HostState.RegularGrace:
                    case Dialogs.LicenseStatus.HostState.UpgradeGrace:
                    case Dialogs.LicenseStatus.HostState.ExpiresSoon:
                        return Status.Information;
                    case Dialogs.LicenseStatus.HostState.Unknown:
                        if (!licenseStatus.Updated)
                            return Status.Updating;
                        return Status.Information;
                    default:
                        return Status.Information;
                }
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

                return XenObjectHost.ProductVersionTextShort;
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
                if (XenObject is Host)
                    return (XenObject as Host).CpuSockets;
                if (XenObject is Pool)
                    return (XenObject as Pool).CpuSockets;
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
                        if (licenseStatus.PoolLicensingModel == Dialogs.LicenseStatus.LicensingModel.Clearwater)
                            return Messages.LICENSE_UNSUPPORTED;
                        return Messages.LICENSE_EXPIRED;
                    case Dialogs.LicenseStatus.HostState.Free:
                        switch (licenseStatus.PoolLicensingModel)
                        {
                            case Dialogs.LicenseStatus.LicensingModel.Clearwater:
                                return Messages.LICENSE_UNSUPPORTED;
                            case Dialogs.LicenseStatus.LicensingModel.Creedence:
                                return Messages.LICENSE_EXPIRED;
                            default:
                                return Messages.LICENSE_FREE;
                        }
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

        protected void TriggerRefreshAllEvent()
        {
            var view = DataGridView as LicenseCheckableDataGridView;
            if (view != null)
                view.TriggerRefreshAllEvent();
        }
    }
}
