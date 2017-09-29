﻿/* Copyright (c) Citrix Systems, Inc. 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAPI;

namespace XenAdmin.Actions
{
    public class USBPassthrough : PureAsyncAction
    {
        private PUSB _pusb;
        private VM _vm;
        private bool _passthroughEnabled;

        public USBPassthrough (PUSB pusb, bool passthroughEnabled) :
            base(pusb.Connection, String.Format(passthroughEnabled ? Messages.ACTION_USB_PASSTHROUGH_ENABLING : Messages.ACTION_USB_PASSTHROUGH_DISABLING, pusb.Name()))
        {
            _pusb = pusb;
            _passthroughEnabled = passthroughEnabled;
        }

        protected override void Run()
        {
            try
            {
                PUSB.set_passthrough_enabled(_pusb.Connection.Session, _pusb.opaque_ref, _passthroughEnabled);
            }
            catch
            {
                Description = _passthroughEnabled ? Messages.ACTION_USB_PASSTHROUGH_ENABLE_FAILED : Messages.ACTION_USB_PASSTHROUGH_DISABLE_FAILED;
                throw;
            }
            Description = _passthroughEnabled ? Messages.ACTION_USB_PASSTHROUGH_ENABLED : Messages.ACTION_USB_PASSTHROUGH_DISABLED;
        }
    }
}
