﻿/* Copyright (c) Cloud Software Group, Inc. 
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

using XenAPI;
using static XenAPI.Message;

namespace XenAdmin.Alerts
{
    public class LeafCoalesceAlert : MessageAlert
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly VM _vm;
        private readonly VDI _vDI;

        public LeafCoalesceAlert(Message msg)
            : base(msg)
        {
            var obj = msg.GetXenObject();

            if (obj is VDI vdi && vdi.Connection != null)
            {
                foreach (var vbdRef in vdi.VBDs)
                {
                    var vbd = vdi.Connection.Resolve(vbdRef);

                    if (vbd != null && vbd.currently_attached)
                    {
                        VM vm = vbd.Connection.Resolve(vbd.VM);

                        if (vm != null)
                        {
                            _vm = vm;
                            _vDI = vdi;
                            break;
                        }
                    }
                }
            }
        }

        public override string Description
        {
            get
            {
                switch (Message.Type)
                {
                    case MessageType.LEAF_COALESCE_START_MESSAGE:
                        return string.Format(Messages.LEAF_COALESCE_START_DESCRIPTION, _vDI.Name(), _vm.Name());
                    case MessageType.LEAF_COALESCE_COMPLETED:
                        return string.Format(Messages.LEAF_COALESCE_COMPLETED_DESCRIPTION, _vDI.Name(), _vm.Name());
                    case MessageType.LEAF_COALESCE_FAILED:
                        return string.Format(Messages.LEAF_COALESCE_FAILED_DESCRIPTION, _vDI.Name(), _vm.Name());
                    default:
                        return base.Description;
                }
            }
        }

        public override string Title
        {
            get
            {

                switch (Message.Type)
                {
                    case MessageType.LEAF_COALESCE_START_MESSAGE:
                        return string.Format(Messages.LEAF_COALESCE_START_TITLE, _vm.Name());
                    case MessageType.LEAF_COALESCE_COMPLETED:
                        return string.Format(Messages.LEAF_COALESCE_COMPLETED_TITLE, _vm.Name());
                    case MessageType.LEAF_COALESCE_FAILED:
                        return string.Format(Messages.LEAF_COALESCE_FAILED_TITLE, _vm.Name());
                    default:
                        return base.Title; ;
                }
            }
        }

        public override string HelpID
        {
            get
            {
                return "LeafCoalesceAlert";
            }
        }
    }
}
