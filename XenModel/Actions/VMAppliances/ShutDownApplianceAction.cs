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

using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
	public class ShutDownApplianceAction : AsyncAction
	{
		private readonly VM_appliance m_appliance;

		public ShutDownApplianceAction(VM_appliance appliance)
			: base(appliance.Connection, Messages.VM_APPLIANCE_SHUT_DOWN)
		{
			Pool = Helpers.GetPool(appliance.Connection);
			if (Pool == null)
				Host = Helpers.GetMaster(appliance.Connection);

			ApiMethodsToRoleCheck.Add("VM_appliance.shutdown");
			
			m_appliance = appliance;
		}

		protected override void Run()
		{
            Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN, m_appliance.Name);
            if (m_appliance.allowed_operations.Contains(vm_appliance_operation.shutdown))
                VM_appliance.shutdown(Session, m_appliance.opaque_ref);
			Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN_COMPLETED, m_appliance.Name);
		}
	}

    public class HardShutDownApplianceAction : AsyncAction
    {
        private readonly VM_appliance m_appliance;

        public HardShutDownApplianceAction(VM_appliance appliance)
            : base(appliance.Connection, Messages.VM_APPLIANCE_SHUT_DOWN)
        {
            Pool = Helpers.GetPool(appliance.Connection);
            if (Pool == null)
                Host = Helpers.GetMaster(appliance.Connection);

            ApiMethodsToRoleCheck.Add("VM_appliance.hard_shutdown");

            m_appliance = appliance;
        }

        protected override void Run()
        {
            Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN, m_appliance.Name);
            if (m_appliance.allowed_operations.Contains(vm_appliance_operation.hard_shutdown))
                VM_appliance.hard_shutdown(Session, m_appliance.opaque_ref);
            Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN_COMPLETED, m_appliance.Name);
        }
    }

    public class CleanShutDownApplianceAction : AsyncAction
    {
        private readonly VM_appliance m_appliance;

        public CleanShutDownApplianceAction(VM_appliance appliance)
            : base(appliance.Connection, Messages.VM_APPLIANCE_SHUT_DOWN)
        {
            Pool = Helpers.GetPool(appliance.Connection);
            if (Pool == null)
                Host = Helpers.GetMaster(appliance.Connection);

            ApiMethodsToRoleCheck.Add("VM_appliance.clean_shutdown");

            m_appliance = appliance;
        }

        protected override void Run()
        {
            Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN, m_appliance.Name);
            if (m_appliance.allowed_operations.Contains(vm_appliance_operation.clean_shutdown))
                VM_appliance.clean_shutdown(Session, m_appliance.opaque_ref);
            Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN_COMPLETED, m_appliance.Name);
        }
    }
}
