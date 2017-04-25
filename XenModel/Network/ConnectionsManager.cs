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

using System.Collections.Generic;
using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin
{
    public static class ConnectionsManager
    {
        public const int DEFAULT_XEN_PORT = HTTP.DEFAULT_HTTPS_PORT;

        /// <summary>
        /// Lock to ensure that checking for existing connections (checking for duplicates and multiple XenExpress connections) gets performed
        /// atomically.  Also protects when iterating over XenConnections.
        /// </summary>
        public static readonly object ConnectionsLock = new object();

        /// <summary>
        /// May only be accessed under the protection of ConnectionsLock.  If you want to perform a long iteration
        /// over this list, then take a copy, using XenConnectionsCopy.
        /// </summary>
        public static readonly ChangeableList<IXenConnection> XenConnections = new ChangeableList<IXenConnection>();

        /// <summary>
        /// Returns a copy of XenConnections, taking the ConnectionsLock for the duration.
        /// </summary>
        public static List<IXenConnection> XenConnectionsCopy
        {
            get
            {
                lock (ConnectionsLock)
                {
                    return new List<IXenConnection>(XenConnections);
                }
            }
        }

        /// <summary>
        /// Returns true if Program.XenConnections contains the given connection, taking the ConnectionsLock across
        /// the check.
        /// </summary>
        public static bool XenConnectionsContains(IXenConnection connection)
        {
            lock (ConnectionsLock)
            {
                return XenConnections.Contains(connection);
            }
        }

        /// <summary>
        /// Use this method to remove connections from the list. It clears the cache and waits for it to complete before removing the connection.
        /// This ensures that any event handlers that are relying on the remove events get a chance to receive them before being deregistered in
        /// MainWindow on the connection remove event handler.
        /// </summary>
        /// <param name="connection"></param>
        public static void ClearCacheAndRemoveConnection(IXenConnection connection)
        {
            Thread t = new Thread((ThreadStart)
                                  delegate
                                      {
                                          connection.Cache.Clear();
                                          lock (ConnectionsLock)
                                          {
                                              XenConnections.Remove(connection);
                                          }
                                      });
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// May only be accessed from the UI thread.
        /// </summary>
        public static readonly ChangeableList<Actions.ActionBase> History = new ChangeableList<Actions.ActionBase>();

        public static void CancelAllActions(IXenConnection connection)
        {
            foreach (ActionBase action in History)
            {
                IXenObject xo = action.Pool ?? action.Host ?? action.VM ?? action.SR as IXenObject;
                if (xo == null || xo.Connection != connection)
                    continue;

                AsyncAction a = action as AsyncAction;
                if (a != null && !a.IsCompleted)
                    action.Cancel();
            }
        }
    }
}
