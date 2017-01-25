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
using System.ComponentModel;
using System.Diagnostics;


namespace XenAdmin
{
    public static class InvokeHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ISynchronizeInvoke Synchronizer;

        public static void Initialize(ISynchronizeInvoke synchronizer)
        {
            Synchronizer = synchronizer;
        }

        public static void Invoke(Action method)
        {
            Invoke(Synchronizer, method);
        }

        /// <summary>
        /// Invoke the given delegate on the given syncronizer.
        /// </summary>
        public static void Invoke(ISynchronizeInvoke syncronizer, Action method)
        {
            if (syncronizer != null && syncronizer.InvokeRequired)
            {
                Action exceptionLogger = () =>
                {
                    try
                    {
                        method.DynamicInvoke();
                    }
                    catch (Exception e)
                    {
                        log.Error("Exception in Invoke", e);
                        throw;
                    }
                };
                syncronizer.Invoke(exceptionLogger, new object[0]);
            }
            else
            {
                method();
            }
        }


        public static void BeginInvoke(Action method)
        {
            BeginInvoke(Synchronizer, method);
        }

        public static void BeginInvoke(ISynchronizeInvoke syncronizer, Action method)
        {
            syncronizer.BeginInvoke(method, new object[0]);
        }

        public static CollectionChangeEventHandler InvokeHandler(CollectionChangeEventHandler handler)
        {
            return delegate(object s, CollectionChangeEventArgs args)
            {
                if (Synchronizer != null)
                {
                    BeginInvoke(Synchronizer, delegate()
                    {
                        if (handler != null)
                            handler(s, args);
                    });
                }
            };
        }

        public static CollectionChangeEventHandler InvokeHandler(ISynchronizeInvoke synchronizer, CollectionChangeEventHandler handler)
        {
            return delegate(object s, CollectionChangeEventArgs args)
            {
                if (synchronizer != null)
                {
                    BeginInvoke(synchronizer, delegate()
                    {
                        if (handler != null)
                            handler(s, args);
                    });
                }
            };
        }

        public static void AssertOnEventThread()
        {
            Trace.Assert(Synchronizer == null || !Synchronizer.InvokeRequired);  // replacement for Program.AssertOnEventThread();
        }

        public static void AssertOffEventThread()
        {
            Trace.Assert(Synchronizer != null && Synchronizer.InvokeRequired);  // replacement for Program.AssertOffEventThread();
        }
    }
}
