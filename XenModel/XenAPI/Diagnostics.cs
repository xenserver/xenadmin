/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;


namespace XenAPI
{
    /// <summary>
    /// A set of functions for diagnostic purpose
    /// First published in Unreleased.
    /// </summary>
    public partial class Diagnostics : XenObject<Diagnostics>
    {
        #region Constructors

        public Diagnostics()
        {
        }

        /// <summary>
        /// Creates a new Diagnostics from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Diagnostics(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Creates a new Diagnostics from a Proxy_Diagnostics.
        /// </summary>
        /// <param name="proxy"></param>
        public Diagnostics(Proxy_Diagnostics proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Diagnostics.
        /// </summary>
        public override void UpdateFrom(Diagnostics update)
        {
        }

        internal void UpdateFrom(Proxy_Diagnostics proxy)
        {
        }

        public Proxy_Diagnostics ToProxy()
        {
            Proxy_Diagnostics result_ = new Proxy_Diagnostics();
            return result_;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Diagnostics
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
        }

        public bool DeepEquals(Diagnostics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return false;
        }

        internal static List<Diagnostics> ProxyArrayToObjectList(Proxy_Diagnostics[] input)
        {
            var result = new List<Diagnostics>();
            foreach (var item in input)
                result.Add(new Diagnostics(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Diagnostics server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Perform a full major collection and compact the heap on a host
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host to perform GC</param>
        public static void gc_compact(Session session, string _host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.diagnostics_gc_compact(session.opaque_ref, _host);
            else
                session.XmlRpcProxy.diagnostics_gc_compact(session.opaque_ref, _host ?? "").parse();
        }

        /// <summary>
        /// Perform a full major collection and compact the heap on a host
        /// First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host to perform GC</param>
        public static XenRef<Task> async_gc_compact(Session session, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_diagnostics_gc_compact(session.opaque_ref, _host);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_diagnostics_gc_compact(session.opaque_ref, _host ?? "").parse());
        }
    }
}
