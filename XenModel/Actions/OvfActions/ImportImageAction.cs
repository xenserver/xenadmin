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
using System.Diagnostics;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;


namespace XenAdmin.Actions.OvfActions
{
	public class ImportImageAction : ImportApplianceAction
	{
		#region Private fields

		private readonly EnvelopeType m_ovfEnvelope;
        private readonly string m_directory;

		#endregion

		public ImportImageAction(IXenConnection connection, EnvelopeType ovfEnv, string directory, Dictionary<string, VmMapping> vmMappings, bool runfixups, SR selectedIsoSr)
			: base(connection, null, vmMappings, false, false, null, runfixups, selectedIsoSr)
		{
			m_ovfEnvelope = ovfEnv;
			m_directory = directory;
            Title = string.Format(Messages.IMPORT_DISK_IMAGE, ovfEnv.Name, Helpers.GetName(connection));
        }

        protected override void RunCore()
        {
            Debug.Assert(m_vmMappings.Count == 1, "There is one VM mapping");

			string systemid = m_vmMappings.Keys.ElementAt(0);
			var mapping = m_vmMappings.Values.ElementAt(0);

			PercentComplete = 20;
			Description = Messages.IMPORTING_DISK_IMAGE;

			//create a copy of the ovf envelope
			EnvelopeType[] envs = OVF.Split(m_ovfEnvelope, "system", new object[] {new[] {systemid}});
			EnvelopeType curEnv = envs[0];

			//storage
			foreach (var kvp in mapping.Storage)
				OVF.SetTargetSRInRASD(curEnv, systemid, kvp.Key, kvp.Value.uuid);

			//network
			foreach (var kvp in mapping.Networks)
				OVF.SetTargetNetworkInRASD(curEnv, systemid, kvp.Key, kvp.Value.uuid);

			if (m_runfixups)
			{
				string cdId = OVF.SetRunOnceBootCDROMOSFixup(curEnv, systemid, m_directory);
				OVF.SetTargetISOSRInRASD(curEnv, systemid, cdId, m_selectedIsoSr.uuid);
			}

			try //importVM
			{
                Process(curEnv, m_directory);

				PercentComplete = 100;
				Description = Messages.COMPLETED;
			}
			catch (OperationCanceledException)
			{
				throw new CancelledException();
			}
		}
	}
}
