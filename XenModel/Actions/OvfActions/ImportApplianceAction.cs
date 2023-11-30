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
using System.IO;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;


namespace XenAdmin.Actions.OvfActions
{
	public partial class ImportApplianceAction : ApplianceAction
	{
		#region Private fields

		private readonly Package m_package;
        protected readonly Dictionary<string, VmMapping> m_vmMappings;
		private readonly bool m_verifyManifest;
		private readonly bool m_verifySignature;
		private readonly string m_password;
        private Type m_encryptionClass;
        private string m_encryptionVersion;
        protected readonly bool m_runfixups;
		protected readonly SR m_selectedIsoSr;
        protected readonly bool _startAutomatically;

		#endregion

        public static RbacMethodList StaticRBACDependencies = new RbacMethodList(
            "VM.add_to_other_config",
            "VM.create",
            "VM.destroy",
            "VM.hard_shutdown",
            "VM.remove_from_other_config",
            "VM.set_HVM_boot_params",
            "VM.start",
            "VDI.create",
            "VDI.destroy",
            "VBD.create",
            "VBD.eject",
            "VIF.create",
            "Host.call_plugin");

        public ImportApplianceAction(IXenConnection connection, Package package, Dictionary<string, VmMapping> vmMappings,
            bool verifyManifest, bool verifySignature, string password, bool runfixups, SR selectedIsoSr, bool startAutomatically)
            : base(connection, string.Empty)
		{
			m_package = package; //this is null if importing a disk Image
			m_vmMappings = vmMappings;
			m_verifyManifest = verifyManifest;
			m_verifySignature = verifySignature;
			m_password = password;
			m_runfixups = runfixups;
			m_selectedIsoSr = selectedIsoSr;
            _startAutomatically = startAutomatically;

            if (package != null) 
                Title = string.Format(Messages.IMPORT_APPLIANCE, package.Name, Helpers.GetName(connection));
		}

        protected override void RunCore()
        {
            // The appliance has a signature and the user asked to verify it.
            if (m_verifySignature)
			{
				Description = Messages.VERIFYING_SIGNATURE;

				try
				{
					m_package.VerifySignature();
                    log.Info($"Verified signature for package {m_package.Name}");
				}
				catch (Exception e)
				{
					log.Error($"Signature verification failed for package {m_package.Name}", e);
					throw new Exception(String.Format(Messages.VERIFYING_SIGNATURE_ERROR, e.Message));
				}
			}
			
            // The appliance has
            // - a signature (in which case it also has a manifest that should be verified AFTER the signature); or
            // - a manifest without a signature
            // and the user asked to verify it

            if (m_verifySignature || m_verifyManifest)
			{
				Description = Messages.VERIFYING_MANIFEST;

				try
				{
					m_package.VerifyManifest();
                    log.Info($"Verified manifest for package {m_package.Name}");
				}
				catch (Exception e)
				{
                    log.Error($"Manifest verification failed for package {m_package.Name}", e);
					throw new Exception(String.Format(Messages.VERIFYING_MANIFEST_ERROR, e.Message));
				}
			}

			Tick(20, string.Format(Messages.IMPORT_APPLIANCE_PREPARING, m_package.Name));

			//create a copy of the OVF
			var envelopes = new List<EnvelopeType>();

			foreach (var vmMapping in m_vmMappings)
			{
                CheckForCancellation();

				string sysId = vmMapping.Key;
				var mapping = vmMapping.Value;
                EnvelopeType[] envs = OVF.Split(m_package.OvfEnvelope, "system", new object[] {new[] {sysId}});

				//storage
				foreach (var kvp in mapping.Storage)
					OVF.SetTargetSRInRASD(envs[0], sysId, kvp.Key, kvp.Value.uuid);

			    foreach (var kvp in mapping.StorageToAttach)
                    OVF.SetTargetVDIInRASD(envs[0], sysId, kvp.Key, kvp.Value.uuid);

				//network
				foreach (var kvp in mapping.Networks)
					OVF.SetTargetNetworkInRASD(envs[0], sysId, kvp.Key, kvp.Value.uuid);

				if (m_runfixups)
				{
					string cdId = OVF.SetRunOnceBootCDROMOSFixup(envs[0], sysId,
                        Path.GetDirectoryName(m_package.PackageSourceFile), BrandManager.ProductBrand);
					OVF.SetTargetISOSRInRASD(envs[0], sysId, cdId, m_selectedIsoSr.uuid);
				}

				envelopes.Add(envs[0]);
			}

            EnvelopeType env = OVF.Merge(envelopes, m_package.Name);

            object importedObject;
			try //import VMs
            {
                m_package.ExtractToWorkingDir(CheckForCancellation);
                CheckForCancellation();
                OVF.ParseEncryption(env, out m_encryptionClass, out m_encryptionVersion);
                importedObject = Process(env, m_package.WorkingDir, m_package.Name);
			}
			catch (OperationCanceledException)
			{
				throw new CancelledException();
			}
            finally
            {
                m_package.CleanUpWorkingDir();
            }

            Tick(100, Messages.COMPLETED);

            if (_startAutomatically && importedObject is XenRef<VM_appliance> applianceRef)
            {
                var appliance = Connection.Resolve(applianceRef);
                if (appliance != null)
                    new StartApplianceAction(appliance, false).RunAsync();
            }
		}
    }
}
