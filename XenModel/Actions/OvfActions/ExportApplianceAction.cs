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
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib.Compression;
using XenOvf;
using XenOvf.Definitions;


namespace XenAdmin.Actions.OvfActions
{
	public partial class ExportApplianceAction : ApplianceAction
	{
		#region Private fields

		private readonly string m_applianceDirectory;
		private readonly string m_applianceFileName;
		private readonly List<VM> m_vmsToExport;
		private readonly IEnumerable<string> m_eulas;
		private readonly bool m_signAppliance;
		private readonly bool m_createManifest;
		private readonly X509Certificate2 m_certificate;
		private readonly bool m_encryptFiles;
		private readonly string m_encryptPassword;
		private readonly bool m_createOVA;
		private readonly bool m_compressOVFfiles;
        private readonly bool m_shouldVerify;

		#endregion

        public ExportApplianceAction(IXenConnection connection, string applianceDirectory, string applianceFileName, List<VM> vmsToExport,
            IEnumerable<string> eulas, bool signAppliance, bool createManifest, X509Certificate2 certificate,
            bool encryptFiles, string encryptPassword, bool createOVA, bool compressOVFfiles, bool shouldVerify)
            : base(connection, string.Format(createOVA ? Messages.EXPORT_OVA_PACKAGE : Messages.EXPORT_OVF_PACKAGE,
                applianceFileName, Helpers.GetName(connection)))
		{
			m_applianceDirectory = applianceDirectory;
			m_applianceFileName = applianceFileName;
			m_vmsToExport = vmsToExport;
			m_eulas = eulas;
			m_signAppliance = signAppliance;
			m_createManifest = createManifest;
			m_certificate = certificate;
			m_encryptFiles = encryptFiles;
			m_encryptPassword = encryptPassword;
			m_createOVA = createOVA;
			m_compressOVFfiles = compressOVFfiles;
            m_shouldVerify = shouldVerify;

			if (m_vmsToExport.Count == 1)
				VM = m_vmsToExport[0];
		}

        protected override void RunCore()
		{
			log.Info($"Exporting VMs to package {m_applianceFileName}");

			var appFolder = Path.Combine(m_applianceDirectory, m_applianceFileName);
			var appFile = string.Format("{0}.ovf", m_applianceFileName);

			if (!Directory.Exists(appFolder))
				Directory.CreateDirectory(appFolder);

			var envList = new List<EnvelopeType>();

            for (int i = 0; i < m_vmsToExport.Count; i++)
            {
                var vm = m_vmsToExport[i];
                CheckForCancellation();
                Description = string.Format(Messages.EXPORTING_VM_PREPARE, vm.Name());

                int curVm = i;
                void UpdatePercentage(float x)
                {
                    PercentComplete = (int)((curVm + x) * 80 / m_vmsToExport.Count);
                }

                try
                {
                    var envelope = Export(appFolder, vm, UpdatePercentage);
                    envList.Add(envelope);
                    PercentComplete = (i + 1) * 80 / m_vmsToExport.Count;
                }
                catch (OperationCanceledException)
                {
                    throw new CancelledException();
                }
            }

            EnvelopeType env = OVF.Merge(envList, m_applianceFileName);
            PercentComplete = 80;

			foreach (var eula in m_eulas)
			{
                CheckForCancellation();
				Description = Messages.ADDING_EULAS;
				OVF.AddEula(env, eula);
			}

            CheckForCancellation();
			var ovfPath = Path.Combine(appFolder, appFile);
			Description = String.Format(Messages.CREATING_FILE, appFile);
			OVF.SaveAs(env, ovfPath);
			PercentComplete = 85;

            CheckForCancellation();

			if (m_createOVA)
			{
                log.Info($"Archiving OVF package {m_applianceFileName} into OVA");
				Description = String.Format(Messages.CREATING_FILE, String.Format("{0}.ova", m_applianceFileName));

                ManifestAndSign(appFolder, appFile);
                PercentComplete = 90;

                try
                {
                    OVF.ConvertOVFtoOVA(env, ovfPath, () => Cancelling, m_compressOVFfiles);
                }
                catch (OperationCanceledException)
                {
                    throw new CancelledException();
                }
			}
			else if (m_compressOVFfiles)
			{
                log.Info($"Compressing package {m_applianceFileName}");
				Description = Messages.COMPRESSING_FILES;

				try
				{
                    OVF.CompressFiles(env, ovfPath, CompressionFactory.Type.Gz, () => Cancelling);
				}
				catch (OperationCanceledException)
				{
					throw new CancelledException();
				}

                PercentComplete = 95;
                ManifestAndSign(appFolder, appFile);
			}

			PercentComplete = 100;
			Description = Messages.COMPLETED;
		}

        private void ManifestAndSign(string appFolder, string appFile)
        {
            if (m_signAppliance)
            {	
                Description = Messages.SIGNING_APPLIANCE;
                OVF.Sign(m_certificate, appFolder, appFile);
            }
            else if (m_createManifest)
            {
                Description = Messages.CREATING_MANIFEST;
                OVF.Manifest(appFolder, appFile);
            }
        }

        protected override void CleanOnError()
        {
            if (!Directory.Exists(m_applianceDirectory))
                return;
            if (Directory.EnumerateFileSystemEntries(m_applianceDirectory).Any())
                return;

            try
            {
                Directory.Delete(m_applianceDirectory);
            }
            catch
            {
                //ignore
            }
        }
	}
}