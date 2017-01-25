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
using System.Threading;

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using System.Reflection;


namespace XenAdmin.Actions
{
    /// <summary>
    /// Saves the changes on the given IXenModelObjects
    /// </summary>
    public class SaveChangesAction : PureAsyncAction
    {
        private readonly IXenObject _xenObject = null;
        private readonly IXenObject _serverXenObject = null;
        private readonly IXenObject _beforeObject = null;

        // In 5.5, we used to lock the object being edited in the RunAsync() action. We now lock it in the
        // constructor to avoid a race, but we still throw the exception for trying to lock a locked object
        // in Run() (it no longer goes through RunAsync()) to avoid reworking all the calling code.
        private bool lockViolation = false;

        protected SaveChangesAction(IXenObject obj)
            :this(obj, false)
        {}

        protected SaveChangesAction(IXenObject obj, bool suppressHistory)
            : base(obj.Connection, Messages.ACTION_SAVE_CHANGES_TITLE, Messages.ACTION_SAVE_CHANGES_IN_PROGRESS, suppressHistory)
        {
            // This is lovely. We need to lock the server object (not the copy we have taken) before calling save changes.
            // We don't know the type so we use the MethodInfo object and MakeGenericMethod to do a resolve against the type
            // we have extracted from GetType(). The Resolve() call itself needs a XenRef which we make by calling the XenRef constructor that takes the
            // opaque ref as an argument and using the MakeGenericType call to give it the same type as the object...
            // ... _then_ we lock it.
            SetObject(obj);
            _xenObject = obj;
            if (obj.opaque_ref != null)  // creating a new object comes through here, but with obj.opaque_ref == null
            {
                System.Reflection.MethodInfo mi = typeof(IXenConnection).GetMethod("Resolve", BindingFlags.Public | BindingFlags.Instance);
                Type type = obj.GetType();
                object[] xenRefParam = new object[] { 
                typeof(XenRef<>).MakeGenericType(type).GetConstructor(new Type[] {typeof(string)}).Invoke(new Object[] {obj.opaque_ref}) 
                };
                _serverXenObject = (IXenObject)mi.MakeGenericMethod(type).Invoke(obj.Connection, xenRefParam);

                if (_serverXenObject != null)
                {
                    // CA-35210: Removed this exception pending locking overhaul post MR in CA-38966
                    //if (_serverXenObject.Locked)
                    //    lockViolation = true;
                    _serverXenObject.Locked = true;
                }
            }
        }

        public SaveChangesAction(IXenObject obj, IXenObject beforeObject, bool suppressHistory)
            : this(obj, suppressHistory)
        {
            _beforeObject = beforeObject;
        }

        protected override void Run()
        {
            if (lockViolation)
                throw new InvalidOperationException(Messages.SAVECHANGES_LOCKED);
            _xenObject.SaveChanges(Session, _beforeObject);
            Description = Messages.ACTION_SAVE_CHANGES_SUCCESSFUL;
        }

        protected override void Clean()
        {
            if (_serverXenObject != null)
                _serverXenObject.Locked = false;
        }
       
    }
}
