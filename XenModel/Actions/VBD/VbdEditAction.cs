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
using XenAPI;


namespace XenAdmin.Actions
{
    public class VbdEditAction : PureAsyncAction
    {
        private readonly VBD _vbd;
        private readonly vbd_mode _vbdMode;
        private readonly bool _changeDevicePosition;
        private readonly VBD _other;
        private readonly string _devicePosition;
        private readonly int _priority;

        public VbdEditAction(VBD vbd, vbd_mode vbdMode, int priority, bool changeDevicePosition, VBD other, string devicePosition, bool suppressHistory)
            : base(vbd.Connection, string.Format(Messages.ACTION_SAVE_SETTINGS, vbd.Connection.Resolve(vbd.VDI)), string.Format(Messages.ACTION_SAVE_SETTINGS, vbd.Connection.Resolve(vbd.VDI)), suppressHistory)
        {
            this._vbd = vbd;
            _priority = priority;
            VM = vbd.Connection.Resolve(vbd.VM);
            this._vbdMode = vbdMode;
            this._changeDevicePosition = changeDevicePosition;
            this._other = other;
            this._devicePosition = devicePosition;
        }

        protected override void Run()
        {
            if (_vbdMode != _vbd.mode)
                VBD.set_mode(Session, _vbd.opaque_ref, _vbdMode);

            if(_priority != _vbd.IONice)
            {
                _vbd.IONice = _priority;
                VBD.set_qos_algorithm_params(Session, _vbd.opaque_ref, _vbd.qos_algorithm_params );
            }
                
            if (!_changeDevicePosition)
                return;

            if (_other != null)
            {
                // We're going to have to do a swap

                SetUserDevice(Session, VM, _other, _vbd.userdevice, false);
                SetUserDevice(Session, VM, _vbd, _devicePosition, true);

                if (VM.power_state == vm_power_state.Running && _other.allowed_operations.Contains(vbd_operations.plug))
                    VBD.plug(Session, _other.opaque_ref);
            }
            else
            {
                SetUserDevice(Session, VM, _vbd, _devicePosition, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="vm"></param>
        /// <param name="vbd"></param>
        /// <param name="userdevice"></param>
        /// <param name="plug"></param>
        /// <returns>True if it warned the user, so you don't warn twice</returns>
        private static void SetUserDevice(Session session, VM vm, VBD vbd, String userdevice, bool plug)
        {
            //Program.AssertOffEventThread();


            if (vm.power_state == vm_power_state.Running &&
                vbd.currently_attached &&
                vbd.allowed_operations.Contains(vbd_operations.unplug))
            {
                VBD.unplug(session, vbd.opaque_ref);
            }

            VBD.set_userdevice(session, vbd.opaque_ref, userdevice);

            if (plug && vbd.allowed_operations.Contains(vbd_operations.plug) && vm.power_state == vm_power_state.Running)
            {
                VBD.plug(session, vbd.opaque_ref);
            }

        }
    }
}
