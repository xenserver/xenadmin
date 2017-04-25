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

namespace XenOvf
{
    /// <summary>
    /// Enumerations for OVF Events { Start, Progress, End, Unknown }
    /// </summary>
    public enum OvfEventType
    {
        /// <summary>
        /// Start Event
        /// </summary>
        Start,
        /// <summary>
        /// Progress Event
        /// </summary>
        Progress,
        /// <summary>
        /// End Event
        /// </summary>
        End,
        /// <summary>
        /// Unknown Event
        /// </summary>
        Unknown
    }
    
    /// <summary>
    /// Ovf Event Arguments that give detail status.
    /// </summary>
    public class OvfEventArgs : EventArgs
    {
        private OvfEventType _type = OvfEventType.Unknown;
        private ulong _total;
        private ulong _transfered;
        private string _target;
        private string _message;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">target identifier</param>
        /// <param name="message">message</param>
        public OvfEventArgs(string target, string message)
        {
            _target = target;
            _message = message;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">type of event</param>
        /// <param name="target">target identifier</param>
        /// <param name="message">message</param>
        /// <param name="transfered">bytes transfered</param>
        /// <param name="total">total bytes to transfer</param>
        public OvfEventArgs(OvfEventType type, string target, string message, ulong transfered, ulong total)
        {
            _type = type;
            _target = target;
            _message = message;
            _total = total;
            _transfered = transfered;
        }
        /// <summary>
        /// String identifing target
        /// </summary>
        public string Target
        {
            get { return _target; }
        }
        /// <summary>
        /// Free form message
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
        /// <summary>
        /// Total number of bytes to transfer
        /// </summary>
        public ulong Total
        {
            get { return _total; }
        }
        /// <summary>
        /// Number of bytes transfered.
        /// </summary>
        public ulong Transfered
        {
            get { return _transfered; }
        }
        /// <summary>
        /// event type
        /// </summary>
        public OvfEventType Type
        {
            get { return _type; }
        }
    }
}
