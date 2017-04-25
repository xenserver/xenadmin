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
using System.Threading;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class PlanActionWithSession : PlanAction
    {
        protected readonly IXenConnection Connection;

        protected PlanActionWithSession(IXenConnection connection, String description, int offset)
            : base(description)
        {
            if(connection==null)
                throw new ArgumentException(this.GetType().Name+" connection null");
            this.Connection = connection;
        }

        protected PlanActionWithSession(IXenConnection connection, String description)
            : this(connection, description, 1)
        {
        }

        protected T TryResolveWithTimeout<T>(XenRef<T> t) where T : XenObject<T>
        {
            log.DebugFormat("Resolving {0} {1}", t, t.opaque_ref);
            int timeout = 120; // two minutes;

            while (timeout > 0)
            {
                T obj = Connection.Resolve(t);
                if (obj != null)
                    return obj;

                Thread.Sleep(1000);
                timeout = timeout - 1;
            }

            if (typeof(T) == typeof(Host))
                throw new Failure(Failure.HOST_OFFLINE);
            throw new Failure(Failure.HANDLE_INVALID, typeof(T).Name, t.opaque_ref);
        }

        protected abstract void RunWithSession(ref Session session);

        protected sealed override void _Run()
        {
            Session session = Connection.DuplicateSession();

            RunWithSession(ref session);
        }

        public sealed override void Run()
        {
            base.Run();
        }
    }
}
