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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using XenAdmin.Core;
using XenAdmin.Network;

using XenAPI;

namespace XenAdmin.Controls
{
    public class HaNtolControl : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool disposed;

        private volatile bool exitNtolUpdateThread;
        private volatile bool ntolUpdateInProgressOrFailed;
        private readonly AutoResetEvent waitingNtolUpdate = new AutoResetEvent(false);
        private Thread ntolUpdateThread;
        protected long ntolMax = -1;
        protected long ntol = -1;
        private IXenConnection connection;
        private Dictionary<VM, VM.HA_Restart_Priority> settings;

        private readonly CollectionChangeEventHandler VM_CollectionChangedWithInvoke;

        /// <summary>
        /// Fired whenever UpdateInProgressChanged changes.
        /// </summary>
        internal event EventHandler UpdateInProgressChanged;

        protected HaNtolControl()
        {
            VM_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(VM_CollectionChanged);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DeregisterEvents();
                disposed = true;
            }
            base.Dispose(disposing);
        }

        #region Accessors

        [Browsable(false)]
        public IXenConnection Connection
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                DeregisterEvents();
                connection = value;
                RegisterEvents();
            }
        }

        /// <summary>
        /// Whether an update to the max ntol (i.e. a call to Pool.ha_compute_max_host_failures_to_tolerate) is in progress.
        /// </summary>
        [Browsable(false)]
        public bool UpdateInProgress
        {
            get { return ntolUpdateInProgressOrFailed; }
        }

        /// <summary>
        /// The VM restart priorities that the control will use when interrogating the server for
        /// ha_compute_hypothetical_max_host_failures_to_tolerate. May not be null.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public Dictionary<VM, VM.HA_Restart_Priority> Settings
        {
            get { return settings; }
            set
            {
                System.Diagnostics.Trace.Assert(value != null);
                settings = new Dictionary<VM, VM.HA_Restart_Priority>(value);
                // Trigger ntol update
                waitingNtolUpdate.Set();
            }
        }

        [Browsable(false)]
        public long Ntol
        {
            get { return ntol; }
        }

        public bool Overcommitted
        {
            get { return ntol > ntolMax; }
        }

        [Browsable(false)]
        public long NtolMax
        {
            get { return ntolMax; }
        }

        #endregion

        private void RegisterEvents()
        {
            if (connection == null)
                return;

            // Add listeners
            connection.Cache.RegisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);

            foreach (VM vm in connection.Cache.VMs)
            {
                vm.PropertyChanged -= vm_PropertyChanged;
                vm.PropertyChanged += vm_PropertyChanged;
            }
        }

        private void DeregisterEvents()
        {
            if (connection == null)
                return;

            // Remove listeners
            connection.Cache.DeregisterCollectionChanged<VM>(VM_CollectionChangedWithInvoke);

            foreach (VM vm in connection.Cache.VMs)
                vm.PropertyChanged -= vm_PropertyChanged;
        }      

        private void updateNtol()
        {
            Program.AssertOffEventThread();
            log.Debug("Thread starting");

            while (!exitNtolUpdateThread)
            {
                Program.Invoke(this, () =>
                    {
                        // Don't do GUI stuff if we've been told to exit
                        if (!exitNtolUpdateThread)
                        {
                            LoadCalculatingMode();
                            ntolUpdateInProgressOrFailed = true;
                            OnNtolKnownChanged();
                        }
                    });

                try
                {
                    // Turn the settings dictionary into an api-level one we can pass to compute_hypothetical_max.
                    var config = Helpers.GetVmHaRestartPrioritiesForApi(settings);

                    Session dupSess = connection.DuplicateSession(60 * 1000);

                    // Use a 1 minute timeout here (rather than the default 1 day)
                    ntolMax = Pool.GetMaximumTolerableHostFailures(dupSess, config);

                    if (exitNtolUpdateThread)
                        continue;

                    log.DebugFormat("Received ntolMax of {0}", ntolMax);
                    Pool p = Helpers.GetPool(connection);
                    if (p == null)
                        throw new Exception("Pool was equal to null, sleeping");

                    //This is the value we will use to set the ntol indicator if
                    //it is a first population, or if resetting because of pool changes
                    decimal value = p.ha_enabled
                                        ? (Pool.get_ha_host_failures_to_tolerate(dupSess, p.opaque_ref))
                                        : ntolMax;

                    Program.Invoke(this, () =>
                        {
                            // Don't do GUI stuff if we've been told to exit
                            if (!exitNtolUpdateThread)
                            {
                                LoadCalculationSucceededMode(value);
                                ntolUpdateInProgressOrFailed = false;
                                OnNtolKnownChanged();
                            }
                        });
                }
                catch (Exception e)
                {
                    log.Warn(e, e);
                    ntol = -1;
                    Program.Invoke(this, () =>
                        {
                            // Don't do GUI stuff if we've been told to exit
                            if (!exitNtolUpdateThread)
                                LoadCalculationFailedMode();
                        });
                }

                waitingNtolUpdate.WaitOne();
                log.Debug("Thread woken");
            }

            log.Debug("Thread exiting");
        }

        // These functions should be abstract, but the Designer can't display
        // a control that is derived from an abstract base class.
        protected virtual void LoadCalculatingMode() { }
        protected virtual void LoadCalculationSucceededMode(decimal value) { }
        protected virtual void LoadCalculationFailedMode() { }

        internal void StartNtolUpdate()
        {
            StopNtolUpdate();
            ntolUpdateThread = new Thread(updateNtol);
            ntolUpdateThread.IsBackground = true;
            ntolUpdateThread.Name = "Ntol updating thread for pool " + Helpers.GetName(connection);
            exitNtolUpdateThread = false;
            ntolUpdateThread.Start();
        }

        internal void StopNtolUpdate()
        {
            if (ntolUpdateThread != null)
            {
                exitNtolUpdateThread = true;
                waitingNtolUpdate.Set();
                // Prevent double dispose
                ntolUpdateThread = null;
            }
        }

        protected void OnNtolKnownChanged()
        {
            if (UpdateInProgressChanged != null)
                UpdateInProgressChanged(this, null);
        }

        #region Event handlers

        private void VM_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            VM vm = (VM)e.Element;
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    vm.PropertyChanged -= vm_PropertyChanged;
                    vm.PropertyChanged += vm_PropertyChanged;
                    break;
                case CollectionChangeAction.Remove:
                    vm.PropertyChanged -= vm_PropertyChanged;
                    break;
            }
        }

        private void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ha_always_run"
                || e.PropertyName == "ha_restart_priority"
                || e.PropertyName == "power_state"
                || e.PropertyName == "resident_on"
                || e.PropertyName.StartsWith("memory"))
            {
                // Trigger ntol update
                Program.Invoke(this, () => waitingNtolUpdate.Set());
            }
        }

        #endregion
    }
}
