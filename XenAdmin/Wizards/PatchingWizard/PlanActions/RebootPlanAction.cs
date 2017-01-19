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
using System.Globalization;
using System.Threading;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    // TODO COWLEY: This code is used more widely and the common parts should move into HostAction.

    public abstract class RebootPlanAction : PlanActionWithSession
    {
        protected readonly XenRef<Host> Host;
        private bool _cancelled = false;

        protected RebootPlanAction(IXenConnection connection, XenRef<Host> host, String title)
            : base(connection, title)
        {
            this.Host = host;
        }

        protected delegate double MetricDelegate(Session session);
        protected delegate void DelegateWithSession(Session session);

        protected void WaitForReboot(ref Session session, DelegateWithSession methodInvoker)
        {
            Host host = TryResolveWithTimeout(this.Host);

            _WaitForReboot(host.IsMaster(), ref session, GetHostBootTime, methodInvoker);
        }

        protected void WaitForAgent(ref Session session, DelegateWithSession methodInvoker)
        {
            Host host = TryResolveWithTimeout(this.Host);

            _WaitForReboot(host.IsMaster(), ref session, GetAgentStartTime, methodInvoker);
        }

        private double GetAgentStartTime(Session session)
        {
            Dictionary<String, String> other_config = XenAPI.Host.get_other_config(session, Host);

            if (other_config == null)
                return 0.0;

            if (!other_config.ContainsKey(XenAPI.Host.AGENT_START_TIME))
                return 0.0;

            double agentStartTime;

            if (!double.TryParse(other_config[XenAPI.Host.AGENT_START_TIME], NumberStyles.Number,
                                 CultureInfo.InvariantCulture, out agentStartTime))
                return 0.0;

            return agentStartTime;
        }

        private double GetHostBootTime(Session session)
        {
            Dictionary<String, String> other_config = XenAPI.Host.get_other_config(session, Host);

            if (other_config == null)
                return 0.0;

            if (!other_config.ContainsKey(XenAPI.Host.BOOT_TIME))
                return 0.0;

            double agentStartTime;

            if (!double.TryParse(other_config[XenAPI.Host.BOOT_TIME], NumberStyles.Number,
                                 CultureInfo.InvariantCulture, out agentStartTime))
                return 0.0;

            return agentStartTime;
        }

        public override void Cancel()
        {
            _cancelled = true;

            lock (this)
            {
                Monitor.PulseAll(this);
            }
        }

        private void RegisterConnectionLostEvent()
        {
            Connection.ConnectionLost -= connection_ConnectionLost;
            Connection.ConnectionLost += connection_ConnectionLost;
        }

        private void DeregisterConnectionLostEvent()
        {
            Connection.ConnectionLost -= connection_ConnectionLost;
        }

        private void _WaitForReboot(bool master, ref Session session, MetricDelegate metricDelegate, DelegateWithSession methodInvoker)
        {
            _cancelled = false;
            double metric = metricDelegate(session);

            log.DebugFormat("{0}._WaitForReboot(master='{1}', metric='{2}')", GetType().Name, master, metric);

            PercentComplete = 10;

            RegisterConnectionLostEvent();
            try
            {
                methodInvoker(session);

                PercentComplete = 20;

                log.DebugFormat("{0}._WaitForReboot executed delegate...", GetType().Name);

                session = WaitForHostToStart(master, session, metricDelegate, metric);
            }
            finally
            {
                DeregisterConnectionLostEvent();
            }
        }


        private Session WaitForHostToStart(bool master, Session session, MetricDelegate metricDelegate, double metric)
        {
            Connection.ExpectDisruption = true;

            try
            {
                if (master)
                {
                    Connection.SupressErrors = true;

                    //
                    // Wait for a dissconnection
                    //

                    WaitForDisconnection();

                    //
                    // Now, we need to wait for a reconnection
                    //
                    session = WaitReconnectToMaster(session);
                }

                PercentComplete = 60;

                // 
                // Now wait for boot time to be greater than it was before
                //
                WaitForBootTimeToBeGreaterThanBefore(master, session, metricDelegate, metric);

                log.DebugFormat("{0}._WaitForReboot done!", GetType().Name);
            }
            finally
            {
                Connection.SupressErrors = false;
            }
            return session;
        }

        private void WaitForDisconnection()
        {
            log.DebugFormat("{0}._WaitForReboot waiting for connection to go away...", GetType().Name);

            lock (this)
            {
                while (!lostConnection && !_cancelled)
                {
                    Monitor.Wait(this);
                }
            }

            log.DebugFormat("{0}._WaitForReboot connection went away...", GetType().Name);

            if (_cancelled)
                throw new CancelledException();

            PercentComplete = 40;

            Thread.Sleep(1000);
        }

        private void WaitForBootTimeToBeGreaterThanBefore(bool master, Session session, MetricDelegate metricDelegate, double metric)
        {

            DateTime waitForMetric = DateTime.Now;
            int lastMetricDebug = 0;

            log.DebugFormat("{0}._WaitForReboot waiting for metric to update... (metric='{1}')", GetType().Name, metric);

            double currentMetric;
            do
            {
                currentMetric = metricDelegate(session);

                if (_cancelled)
                    throw new CancelledException();

                Thread.Sleep(1000);

                // Debug message once a minute

                int currMin = (int)((DateTime.Now - waitForMetric).TotalSeconds) / 60;
                if (currMin <= lastMetricDebug)
                    continue;

                lastMetricDebug = currMin;

                log.DebugFormat("{0}._WaitForReboot still waiting for metric after {1}... (old metric='{2}', current metric='{3}')",
                                GetType().Name, DateTime.Now - waitForMetric, metric, currentMetric);
            }
            while (metric == currentMetric);

            Thread.Sleep(1000);

            log.DebugFormat("{0}._WaitForReboot metric now up to date... (old metric='{1}', current metric='{2}')",
                            GetType().Name, metric, currentMetric);

            if (master)
            {
                //
                // Force a reconnect to prime the cache for the next actions
                //

                log.DebugFormat("{0}._WaitForReboot connecting up...", GetType().Name);

                Program.Invoke(Program.MainWindow, () => XenConnectionUI.BeginConnect(Connection, false, null, false));
            }
        }

        private Session WaitReconnectToMaster(Session session)
        {

            log.DebugFormat("{0}._WaitForReboot waiting for reconnect...", GetType().Name);

            DateTime waitForReconnect = DateTime.Now;
            int lastMinDebug = 0;

            do
            {
                if (_cancelled)
                    throw new CancelledException();

                try
                {
                    session = Connection.DuplicateSession();
                }
                catch (Exception e)
                {
                    session = null;
                    Thread.Sleep(5000);

                    // Debug message once a minute

                    int currMin = (int)((DateTime.Now - waitForReconnect).TotalSeconds) / 60;

                    if (currMin <= lastMinDebug)
                        continue;

                    lastMinDebug = currMin;

                    log.Debug(string.Format("{0}._WaitForReboot still waiting for connection after {1}...",
                        GetType().Name, DateTime.Now - waitForReconnect),e);
                }
            }
            while (session == null);

            log.DebugFormat("{0}._WaitForReboot reconnected after {1}...", GetType().Name, DateTime.Now - waitForReconnect);
            return session;
        }

        private bool lostConnection = false;

        private void connection_ConnectionLost(object sender, EventArgs e)
        {
            lock (this)
            {
                lostConnection = true;
                Monitor.PulseAll(this);
            }
        }
    }
}