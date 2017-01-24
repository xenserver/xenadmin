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
using System.IO;
using CookComputing.XmlRpc;
using log4net.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class ActivationRequestAction : AsyncAction
    {
        private const int ACTIVATION_REQUEST_TIMEOUT = 30 * 1000;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _requestString;

        public ActivationRequestAction(string requestString)
            : base(null, Messages.ACTIVATION_REQUEST_TITLE)
        {
            _requestString = requestString;
        }

        protected override void Run()
        {
            Description = Messages.ACTIVATION_REQUEST_TITLE;

            ActivationProxy proxy = XmlRpcProxyGen.Create<ActivationProxy>();
            proxy.Url = string.Format(InvisibleMessages.ACTIVATION_URL, InvisibleMessages.ACTIVATION_SERVER);
            proxy.Timeout = ACTIVATION_REQUEST_TIMEOUT;
            proxy.UseIndentation = false;
            proxy.UserAgent = Session.UserAgent;
            proxy.KeepAlive = true;
            proxy.RequestEvent += LogRequest;
            proxy.ResponseEvent += LogResponse;
            proxy.Proxy = Session.Proxy;

            // response is the transaction id of this call to the activation service

            Result = proxy.process_reactivation_request(_requestString);
            Description = Messages.COMPLETED;
        }


        private void LogRequest(object o, XmlRpcRequestEventArgs args)
        {
            LogSomething("Invoking XML-RPC method:", args.RequestStream);
        }

        private void LogResponse(object o, XmlRpcResponseEventArgs args)
        {
            LogSomething("XML-RPC response:", args.ResponseStream);
        }

        private void LogSomething(string msg, Stream s)
        {
            if (log.Logger.IsEnabledFor(Level.Debug))
            {
                LogMsg(msg);
                DumpStream(s);
            }
        }

        private void DumpStream(Stream s)
        {
            TextReader r = new StreamReader(s);
            String l;
            while ((l = r.ReadLine()) != null)
            {
                LogMsg(l);
            }
        }

        private void LogMsg(String msg)
        {
            log.Debug(msg);

        }
    }
}

