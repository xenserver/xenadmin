/* Copyright (c) Citrix Systems Inc. 
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
    /// <summary>
    /// 
    /// </summary>
    public enum XenOvfTranportEventType
    {
        /// <summary>
        /// 
        /// </summary>
        FileStart,
        /// <summary>
        /// 
        /// </summary>
        FileProgress,
        /// <summary>
        /// 
        /// </summary>
        FileComplete,
        /// <summary>
        /// 
        /// </summary>
        FileCancelled,
        /// <summary>
        /// 
        /// </summary>
        ImportStart,
        /// <summary>
        /// 
        /// </summary>
        ImportProgress,
        /// <summary>
        /// 
        /// </summary>
        ImportThreadComplete,
        /// <summary>
        /// 
        /// </summary>
        ImportComplete,
        /// <summary>
        /// 
        /// </summary>
        ImportCancelled,
        /// <summary>
        /// 
        /// </summary>
        ExportStart,
        /// <summary>
        /// 
        /// </summary>
        ExportProgress,
        /// <summary>
        /// 
        /// </summary>
        ExportThreadComplete,
        /// <summary>
        /// 
        /// </summary>
        ExportComplete,
        /// <summary>
        /// 
        /// </summary>
        ExportCancelled,
        /// <summary>
        /// 
        /// </summary>
        Progress,
        /// <summary>
        /// 
        /// </summary>
        MarqueeOn,
        /// <summary>
        /// 
        /// </summary>
        MarqueeOff,
        /// <summary>
        /// 
        /// </summary>
        Failure,
        /// <summary>
        /// 
        /// </summary>
        Unknown
    }
    /// <summary>
    /// 
    /// </summary>
    public class XenOvfTranportEventArgs : EventArgs
    {
        private XenOvfTranportEventType _type = XenOvfTranportEventType.Unknown;
        private ulong _total;
        private ulong _transfered;
        private string _target;
        private string _message;
        private Exception _exception;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <param name="transfered"></param>
        /// <param name="total"></param>
        public XenOvfTranportEventArgs(XenOvfTranportEventType type, string target, string message, ulong transfered, ulong total)
        {
            _type = type;
            _target = target;
            _message = message;
            _total = total;
            _transfered = transfered;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Target
        {
            get { return _target; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ulong Total
        {
            get { return _total; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ulong Transfered
        {
            get { return _transfered; }
        }
        /// <summary>
        /// 
        /// </summary>
        public XenOvfTranportEventType Type
        {
            get { return _type; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Exception exception
        {
            get
            {
                return _exception;
            }
        }
    }
}
