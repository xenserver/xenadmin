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
using System.Threading;
using XenAPI;

namespace XenAdmin.Utils
{
    public class AsyncServerTimeEventArgs: EventArgs
    {
        public Host QueriedHost { get; private set; }
        public bool Success { get; private set; }
        public Exception Failure { get; private set; }
        public AsyncServerTimeEventArgs(Host host, bool success, Exception failure)
        {
            QueriedHost = host;
            Success = success;
            Failure = failure;
        }    
    }

    /// <summary>
    /// Get the UTC server time in an async fashion
    /// 
    /// - Updated server time is not guaranteed
    /// - No retry
    /// - Default return time is the time at which the class is constructed
    /// </summary>
    public class AsyncServerTime
    {
        public delegate void ServerTimeEventHandler(object sender, AsyncServerTimeEventArgs args);
        
        /// <summary>
        /// Event triggered when the server time is obtained
        /// </summary>
        public event ServerTimeEventHandler ServerTimeObtained;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Server time (UTC) - updated by thread and user is informed via the ServerTimeEventHandler
        /// Default = DateTime.UtcNow
        /// </summary>
        public DateTime ServerTime { get; private set; }

        /// <summary>
        /// Constructs the class but doesn't trigger the fetch of the server time
        /// </summary>
        public AsyncServerTime()
        {
            ServerTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Get the server time for given host on a background thread
        /// 
        /// Connect to the ServerTimeEventHandler for updates otherwise you'll get the current UTC time
        /// and it won't be updated
        /// </summary>
        /// <param name="host"></param>
        public void Fetch(Host host)
        {
            ThreadPool.QueueUserWorkItem(GetServerTime, host);
        }

        private void TriggerEvent(Host host, bool success, Exception failureReason)
        {
            if (ServerTimeObtained != null)
                ServerTimeObtained(this, new AsyncServerTimeEventArgs(host, success, failureReason));
        }

        private void GetServerTime(object state)
        {
            Host host = state as Host;

            if (host == null)
            {
                TriggerEvent(host, false, new ArgumentNullException(typeof(object).ToString()));
                log.ErrorFormat("Failed to fetch server time for host as host could not be resolved");
                return;
            }

            try
            {
                //Note we're using the get_servertime call which returns the UTC time
                ServerTime = Host.get_servertime(host.Connection.Session, host.opaque_ref);
            }

            catch(Exception e)
            {
                TriggerEvent(host, false, e);
                log.ErrorFormat("Failed to fetch server time for host {0} because {1}", host.name_label, e.Message);
                return;
            }

            TriggerEvent(host, true, null);
        }
    }
}
