/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAPI;

namespace XenAdmin.Actions
{
    public class DeleteVUSBAction : PureAsyncAction
    {
        private VUSB _vusb;

        public DeleteVUSBAction(VUSB vusb, VM vm) : 
            base(vusb.Connection, String.Format(Messages.ACTION_VUSB_DELETING, vusb.Name(), vm.Name()))
        {
            _vusb = vusb;
        }

        protected override void Run()
        {
            try
            {
                if ((_vusb.currently_attached) &&
                    XenAPI.VUSB.get_allowed_operations(Session, _vusb.opaque_ref).Contains(XenAPI.vusb_operations.unplug))
                {
                    RelatedTask = VUSB.async_unplug(Session, _vusb.opaque_ref);
                    PollToCompletion(0, 50);
                }
            }
            finally
            {
                PercentComplete = 50;
                RelatedTask = VUSB.async_destroy(Session, _vusb.opaque_ref);
                PollToCompletion(51, 100);
            }
            Description = Messages.ACTION_VUSB_DELETED;
        }

    }

}
