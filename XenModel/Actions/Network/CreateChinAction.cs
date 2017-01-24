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

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class CreateChinAction : PureAsyncAction
    {
        XenAPI.Network newNetwork;
        XenAPI.Network theInterface;

        public CreateChinAction(IXenConnection connection, XenAPI.Network newNetwork, XenAPI.Network theInterface)
            : base(connection,
                string.Format(Messages.NETWORK_ACTION_CREATING_NETWORK_TITLE, newNetwork.Name, Helpers.GetName(connection)))
        {
            this.newNetwork = newNetwork;
            this.theInterface = theInterface;
        }

        protected override void Run()
        {
            // Create the new network
            XenRef<XenAPI.Network> networkRef = XenAPI.Network.create(Session, newNetwork);

            // Create a tunnel on each transport PIF
            foreach (PIF pif in Connection.ResolveAll(theInterface.PIFs))
            {
                if (pif.IsManagementInterface(true))
                    Tunnel.create(Session, pif.opaque_ref, networkRef);
            }
        }
    }
}
