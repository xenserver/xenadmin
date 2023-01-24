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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XenAdmin.Actions.VMActions;
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
        private readonly Action<VM, bool> _warningDelegate;
        private readonly Action<VMStartAbstractAction, Failure> _failureDiagnosisDelegate;

		#endregion

        public ImportImageAction(IXenConnection connection, EnvelopeType ovfEnv, string directory,
            Dictionary<string, VmMapping> vmMappings, bool runfixups, SR selectedIsoSr, bool startAutomatically,
            Action<VM, bool> warningDelegate, Action<VMStartAbstractAction, Failure> failureDiagnosisDelegate)
            : base(connection, null, vmMappings, false, false, null, runfixups, selectedIsoSr, startAutomatically)
        {
            m_ovfEnvelope = ovfEnv;
            m_directory = directory;
            Title = string.Format(Messages.IMPORT_DISK_IMAGE, ovfEnv.Name, Helpers.GetName(connection));
            _warningDelegate = warningDelegate;
            _failureDiagnosisDelegate = failureDiagnosisDelegate;
        }

        protected override void RunCore()
        {
            Debug.Assert(m_vmMappings.Count == 1, "There must be only one VM mapping");

			string sysId = m_vmMappings.Keys.ElementAt(0);
			var mapping = m_vmMappings.Values.ElementAt(0);

			Tick(20, Messages.IMPORTING_DISK_IMAGE);

			//create a copy of the ovf envelope
			EnvelopeType[] envs = OVF.Split(m_ovfEnvelope, "system", new object[] {new[] {sysId}});
			EnvelopeType curEnv = envs[0];

			//storage
			foreach (var kvp in mapping.Storage)
				OVF.SetTargetSRInRASD(curEnv, sysId, kvp.Key, kvp.Value.uuid);

			//network
			foreach (var kvp in mapping.Networks)
				OVF.SetTargetNetworkInRASD(curEnv, sysId, kvp.Key, kvp.Value.uuid);

			if (m_runfixups)
			{
				string cdId = OVF.SetRunOnceBootCDROMOSFixup(curEnv, sysId, m_directory, BrandManager.ProductBrand);
				OVF.SetTargetISOSRInRASD(curEnv, sysId, cdId, m_selectedIsoSr.uuid);
			}

            object importedObject;
			try //importVM
			{
                importedObject = Process(curEnv, m_directory);

				Tick(100, Messages.COMPLETED);
			}
			catch (OperationCanceledException)
			{
				throw new CancelledException();
			}

            if (_startAutomatically && importedObject is XenRef<VM> vmRef)
            {
                var vm = Connection.Resolve(vmRef);
                if (vm != null)
                    new VMStartAction(vm, _warningDelegate, _failureDiagnosisDelegate).RunAsync();
            }
		}
	}
}
