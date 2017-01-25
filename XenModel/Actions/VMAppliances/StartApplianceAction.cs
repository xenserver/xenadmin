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

using System.Collections.Generic;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
	/// <summary>
	/// Command used to start the VMs in the appliance
	/// </summary>
	public class StartApplianceAction : AsyncAction
	{
		private readonly VM_appliance m_appliance;
		private readonly bool m_suspend;

		public StartApplianceAction(VM_appliance appliance, bool suspend)
			: base(appliance.Connection, suspend ? Messages.VM_APPLIANCE_START_PAUSED : Messages.VM_APPLIANCE_START)
		{
			Pool = Helpers.GetPool(appliance.Connection);
			if (Pool == null)
				Host = Helpers.GetMaster(appliance.Connection);

			ApiMethodsToRoleCheck.Add("VM_appliance.start");

			m_suspend = suspend;
			m_appliance = appliance;
		}

		protected override void Run()
		{
			Description = string.Format(m_suspend ? Messages.VM_APPLIANCE_STARTING_PAUSED : Messages.VM_APPLIANCE_STARTING, m_appliance.Name);
            //if (m_appliance.allowed_operations.Contains(vm_appliance_operation.start))
            if (m_appliance.VMs.Count > 0)
                VM_appliance.start(Session, m_appliance.opaque_ref, m_suspend);
			Description = string.Format(m_suspend ? Messages.VM_APPLIANCE_STARTING_PAUSED_COMPLETED : Messages.VM_APPLIANCE_STARTING_COMPLETED, m_appliance.Name);
		}
	}
}
