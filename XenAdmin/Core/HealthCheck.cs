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
using System.Collections.Generic;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Model;


namespace XenAdmin.Core
{
    public class HealthCheck
    {
        public static event Action<bool> CheckForAnalysisResultsCompleted;

        /// <summary>
        /// Checks the analysis results for all connections
        /// </summary>
        public static void CheckForAnalysisResults()
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                Pool pool = Helpers.GetPoolOfOne(xenConnection);
                if (pool == null || Helpers.FeatureForbidden(xenConnection, Host.RestrictHealthCheck))
                    continue;

                var healthCheckSettings = pool.HealthCheckSettings;
                
                if (healthCheckSettings.Status == HealthCheckStatus.Enabled && !healthCheckSettings.HasAnalysisResult)
                    actions.Add(new GetHealthCheckAnalysisResultAction(pool, Registry.HealthCheckDiagnosticDomainName, true));
            }

            if (actions.Count == 1)
            {
                actions[0].Completed += actionCompleted;
                actions[0].RunAsync();
            }
            else if (actions.Count > 1)
            {
                var parallelAction = new ParallelAction(null, "", "", "", actions);
                parallelAction.Completed += actionCompleted;
                parallelAction.RunAsync();
            }
        }

        /// <summary>
        /// Checks the analysis results for the specified connections
        /// </summary>
        public static void CheckForAnalysisResults(object connection)
        {
            Pool pool = Helpers.GetPoolOfOne((IXenConnection)connection);
            if (pool == null || Helpers.FeatureForbidden((IXenConnection)connection, Host.RestrictHealthCheck))
                return;

            var healthCheckSettings = pool.HealthCheckSettings;

            if (healthCheckSettings.Status == HealthCheckStatus.Enabled && !healthCheckSettings.HasAnalysisResult)
            {
                var action = new GetHealthCheckAnalysisResultAction(pool, Registry.HealthCheckDiagnosticDomainName, true);
                action.Completed += actionCompleted;
                action.RunAsync();
            }
        }

        private static void actionCompleted(ActionBase sender)
        {
            Program.AssertOffEventThread();

            GetHealthCheckAnalysisResultAction action = sender as GetHealthCheckAnalysisResultAction;
            if (action == null)
                return;

            if (CheckForAnalysisResultsCompleted != null)
                CheckForAnalysisResultsCompleted(action.Succeeded);
        }
    }
}
