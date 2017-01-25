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

namespace XenOvfTransport
{
    public enum XenOvfTranportEventType
    {
        FileStart,
        FileProgress,
        FileComplete,
        FileCancelled,
        ImportStart,
        ImportProgress,
        ImportThreadComplete,
        ImportComplete,
        ImportCancelled,
        ExportStart,
        ExportProgress,
        ExportThreadComplete,
        ExportComplete,
        ExportCancelled,
        Progress,
        MarqueeOn,
        MarqueeOff,
        Failure,
        Unknown
    }

    public class XenOvfTranportEventArgs : EventArgs
    {
        private XenOvfTranportEventType _type = XenOvfTranportEventType.Unknown;
        private ulong _total;
        private ulong _transfered;
        private string _target;
        private string _message;
        private Exception _exception;

        public XenOvfTranportEventArgs(XenOvfTranportEventType type, string target, string message)
        {
            _type = type;
            _target = target;
            _message = message;
        }
        public XenOvfTranportEventArgs(XenOvfTranportEventType type, string target, string message, Exception exception)
        {
            _type = type;
            _target = target;
            _message = message;
            _exception = exception;
        }

        public XenOvfTranportEventArgs(XenOvfTranportEventType type, string target, string message, ulong transfered, ulong total)
        {
            _type = type;
            _target = target;
            _message = message;
            _total = total;
            _transfered = transfered;
        }

        public string Target
        {
            get { return _target; }
        }

        public string Message
        {
            get { return _message; }
        }

        public ulong Total
        {
            get { return _total; }
        }

        public ulong Transfered
        {
            get { return _transfered; }
        }

        public XenOvfTranportEventType Type
        {
            get { return _type; }
        }

        public Exception exception
        {
            get
            {
                return _exception;
            }
        }
    }
}
