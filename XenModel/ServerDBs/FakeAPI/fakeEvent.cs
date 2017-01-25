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
using System.Net;
using System.Threading;

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal class fakeEvent : fakeXenObject
    {
        private readonly HighLoadEventGenerator _highLoadEventGenerator;

        public fakeEvent(DbProxy proxy)
            : base("event", proxy)
        {
            _highLoadEventGenerator = new HighLoadEventGenerator(proxy);
        }

        public Response<String> register()
        {
            return new Response<String>("dummy");
        }

        public Response<Proxy_Event[]> next()
        {
            ExitOnDisconnect();

            for (int iter = 0; iter < 600; ++iter)
            {
                List<Proxy_Event> eventsAlias = null;
                lock (proxy.eventsListLock)
                {
                    proxy.eventsList.AddRange(_highLoadEventGenerator.GetHighLoadEvents());

                    if (proxy.eventsList.Count > 0)
                    {
                        eventsAlias = new List<Proxy_Event>(proxy.eventsList);
                        proxy.eventsList.Clear();
                    }
                }
                if (eventsAlias != null && eventsAlias.Count > 0)
                {
                    Proxy_Event[] eventarr = eventsAlias.ToArray();
                    return new Response<Proxy_Event[]>(eventarr);
                }
                Thread.Sleep(100);
                RunInEventLoop();
            }
            return new Response<Proxy_Event[]>(new Proxy_Event[] { DbProxy.dummyEvent });
        }

        private void ExitOnDisconnect()
        {
            if (proxy.MarkToDisconnect)
            {
                proxy.MarkToDisconnect = false;
                throw new WebException("Connection terminated", WebExceptionStatus.ConnectionClosed);
            }
        }

        private void RunInEventLoop()
        {
            if(proxy.RunInEventLoop != null)
            {
                
                proxy.RunInEventLoop();
            }
        }

        public Response<Events> from(string session, string[] _classes, string _token, double _timeout)
        {
            ExitOnDisconnect();

            int iterations = (int) (_timeout < 100 ? 1 : _timeout/100);

            for (int iter = 0; iter < iterations; ++iter)
            {
                List<Proxy_Event> eventsAlias = null;
                lock (proxy.eventsListLock)
                {
                    proxy.eventsList.AddRange(_token == ""
                                                  ? _highLoadEventGenerator.GetAddEvents(_classes)
                                                  : _highLoadEventGenerator.GetHighLoadEvents(_classes, "mod"));

                    if (proxy.eventsList.Count > 0)
                    {
                        eventsAlias = new List<Proxy_Event>(proxy.eventsList);
                        proxy.eventsList.Clear();
                    }
                }
                if (eventsAlias != null && eventsAlias.Count > 0)
                {
                    Proxy_Event[] eventarr = eventsAlias.ToArray();
                    var eventStruct = new Events { events = eventarr, valid_ref_counts = new object(), token = "1,1.0" };
                    return new Response<Events>(eventStruct);
                }

                Thread.Sleep(100);
                RunInEventLoop();
            }
            return
                new Response<Events>(new Events
                                         {
                                             events = new Proxy_Event[] {DbProxy.dummyEvent},
                                             valid_ref_counts = new object(),
                                             token = "1,1.0"
                                         });
        }

    }
}
