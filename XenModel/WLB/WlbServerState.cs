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
using System.Text;
using XenAPI;
using XenAdmin.Core;



namespace XenAdmin.Wlb
{
    /// <summary>
    /// A Static class for describing and maintining the current state of the Wlb Server connection
    /// </summary>
    public static class WlbServerState
    {
        /// <summary>
        /// The key string used to store the connection status in Pool's OtherConfig dictionary
        /// </summary>
        private const string WLB_CONNECTION_STATUS = "WlbConnectionStatus";
        /// <summary>
        /// The key string used to store the connection error message in the pool's OtherConfig dictionary
        /// </summary>
        private const string WLB_CONNECTION_ERROR  = "WlbConnectionError";

        /// <summary>
        /// Public enumeration of the possible Wlb Conection states
        /// </summary>
        public enum ServerState
        {   
            Unknown = 0,
            NotConfigured,
            Enabled,
            Disabled,
            ConnectionError
        }

        /// <summary>
        /// object for locking SetState
        /// </summary>
        private static Object _lockObject = new Object();

        /// <summary>
        /// Public method for updating the Wlb Server state when there is no error (Failure).
        /// This method clears any existing failure message for the connection
        /// </summary>
        /// <param name="pool">The pool who's Wlb connection state we are updating</param>
        /// <param name="state">The current state of the pool's Wlb Server State</param>
        public static void SetState(Pool pool, ServerState state)
        {
            SetState(pool.Connection.Session, pool, state, null);
        }
        /// <summary>
        /// Public method for updating the Wlb Server state.  If the state is ConnectionFailure and
        /// a Failure is supplied, it's message is stored in the OtherConfig
        /// </summary>
        /// <param name="pool">The pool who's Wlb connection state we are updating</param>
        /// <param name="state">The current state of the pool's Wlb Server State</param>
        /// <param name="failure">The Failure (if any) describing the Connection Error</param>
        public static void SetState(Pool pool, ServerState state, Failure failure)
        {
            SetState(pool.Connection.Session, pool, state, failure);
        }
        /// <summary>
        /// Public method for updating the Wlb Server state when there is no error (Failure) and need specific seesion information.
        /// This method clears any existing failure message for the connection
        /// </summary>
        /// <param name="session">The User session use to do this operation</param>
        /// <param name="pool">The pool who's Wlb connection state we are updating</param>
        /// <param name="state">The current state of the pool's Wlb Server State</param>
        public static void SetState(Session session, Pool pool, ServerState state)
        {
            SetState(session, pool, state, null);
        }
        /// <summary>
        /// Public method for updating the Wlb Server state.  If the state is ConnectionFailure and
        /// a Failure is supplied, it's message is stored in the OtherConfig
        /// </summary>
        /// <param name="session">The User session use to do this operation</param>
        /// <param name="pool">The pool who's Wlb connection state we are updating</param>
        /// <param name="state">The current state of the pool's Wlb Server State</param>
        /// <param name="failure">The Failure (if any) describing the Connection Error</param>
        public static void SetState(Session session, Pool pool, ServerState state, Failure failure)
        {
            //only update the state if new value if different than current value
            //  this is to cut down on unneeded Pool_PropertiesChanged events
            if (GetState(pool) != state) 
            {
                // set a lock so we are setting state one at a time
                lock (_lockObject)
                {
                    Helpers.SetOtherConfig(session, pool, WLB_CONNECTION_STATUS, state.ToString());

                    if (null != failure && state == ServerState.ConnectionError)
                    {
                        string error = String.Empty;
                        if (failure.Message == FriendlyErrorNames.WLB_INTERNAL_ERROR)
                        {
                            error = Messages.ResourceManager.GetString("WLB_ERROR_" + failure.ErrorDescription[1]);
                        }
                        else
                        {
                            error = failure.Message;
                        }
                        Helpers.SetOtherConfig(session, pool, WLB_CONNECTION_ERROR, error);
                    }
                    else
                    {
                        Helpers.SetOtherConfig(session, pool, WLB_CONNECTION_ERROR, String.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Public method for retrieving the current state of a Pool's Wlb server connection
        /// </summary>
        /// <param name="pool">The pool who's Wlb connection state we are updating</param>
        /// <returns>ServerState enumeration representing the current Wlb server conection state</returns>
        public static ServerState GetState(Pool pool)
        {
            if (null == Helpers.GetOtherConfig(pool) && !CheckForKnownState(pool))
            {
                    return ServerState.NotConfigured;
            }
            else
            {
                if (!Helpers.GetOtherConfig(pool).ContainsKey(WLB_CONNECTION_STATUS))
                {
                    return ServerState.NotConfigured;
                }
                else
                {
                    return (ServerState)Enum.Parse(typeof(ServerState), Helpers.GetOtherConfig(pool)[WLB_CONNECTION_STATUS]);
                }
            }
        }

        /// <summary>
        /// Private method for determining (initializing) the state of the WlbConnectin when there is 
        /// nothing yet stored in OtherConfig
        /// </summary>
        /// <param name="pool">The pool who's Wlb connection state we are updating</param>
        /// <returns>Bool denoting whether we were able to ascertain the current server state</returns>
        private static bool CheckForKnownState(Pool pool)
        {
            bool stateFound = false;

            if (Helpers.WlbEnabled(pool.Connection))
            {
                SetState(pool, ServerState.Enabled);
                stateFound = true;
            }
            else if (String.IsNullOrEmpty(pool.wlb_url))
            {
                SetState(pool, ServerState.NotConfigured);
                stateFound = true;
            }
            return stateFound;
        }

        /// <summary>
        /// Public method for retrieving the Failure message string stored in OtherConfig for the pool
        /// </summary>
        /// <param name="pool">The pool who's Wlb connection state we are retrieving</param>
        /// <returns>A string containing the Failure message, or an empty string</returns>
        public static string GetFailureMessage(Pool pool)
        {
            string message = String.Empty;

            if (null != Helpers.GetOtherConfig(pool) && Helpers.GetOtherConfig(pool).ContainsKey(WLB_CONNECTION_ERROR))
            {
                message = Helpers.GetOtherConfig(pool)[WLB_CONNECTION_ERROR];
            }

            return message;
        }
    }
}
