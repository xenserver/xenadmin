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
    // A PureAsyncAction is one for which we can determine the set of API calls it will make by doing
    // an initial dummy run through the action using an RbacCollectorProxy.
    // In order to achieve this, it must have the following properties:
    // * No side effects (other than through the API).
    // * It doesn't destroy its own variables as it runs.
    // * No dialogs.
    // * No waits. This generally rules out object creation actions.
    // * No HTTP calls.
    // * No new threads.
    // * No API calls which are allowed to fail.
    public abstract class PureAsyncAction : AsyncAction
    {
        protected PureAsyncAction(IXenConnection connection, string title, string description, bool SuppressHistory)
            : base(connection, title, description, SuppressHistory)
        {
        }

        protected PureAsyncAction(IXenConnection connection, string title, string description)
            : base(connection, title, description)
        {
        }

        protected PureAsyncAction(IXenConnection connection, string title)
            : base(connection, title)
        {
        }

        protected PureAsyncAction(IXenConnection connection, string title, bool suppress_history)
            : base(connection, title, suppress_history)
        {
        }

        public override RbacMethodList GetApiMethodsToRoleCheck
        {
            get
            {
                System.Diagnostics.Trace.Assert(ApiMethodsToRoleCheck.Count == 0);  // shouldn't set ApiMethodsToRoleCheck for PureAsyncAction: it will be ignored
                RbacMethodList rbacMethods = new RbacMethodList();
                Session = new Session(RbacCollectorProxy.GetProxy(rbacMethods), Connection);
                base.SuppressProgressReport = true;
                Run();
                base.SuppressProgressReport = false;
                return rbacMethods;
            }
        }
    }
}
