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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;
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
            if (m_signAppliance && m_certificate == null)
                throw new ArgumentNullException(nameof(m_certificate));
			m_encryptFiles = encryptFiles;
			m_encryptPassword = encryptPassword;
			m_createOVA = createOVA;
			m_compressOVFfiles = compressOVFfiles;
            m_shouldVerify = shouldVerify;

			if (m_vmsToExport.Count == 1)
				VM = m_vmsToExport[0];
		}

        public static RbacMethodList StaticRBACDependencies
        {
            get
            {
                var list = new RbacMethodList("task.create", "http/get_export_raw_vdi");
                list.AddRange(Role.CommonTaskApiList);
                list.AddRange(Role.CommonSessionApiList);
                return list;
            }
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
			OVF.SaveAs(env, ovfPath);
			PercentComplete = 85;

            CheckForCancellation();

			if (m_createOVA)
			{
                ManifestAndSign(env, appFolder, appFile);
                PercentComplete = 90;
                log.Info($"Archiving OVF package {m_applianceFileName} into OVA");
                Description = string.Format(Messages.CREATING_OVA_FILE, string.Format("{0}.ova", m_applianceFileName));
                OVF.ConvertOVFtoOVA(env, ovfPath, CheckForCancellation, m_compressOVFfiles);
			}
			else if (m_compressOVFfiles)
			{
                log.Info($"Compressing package {m_applianceFileName}");
				Description = Messages.COMPRESSING_FILES;
                OVF.CompressFiles(env, ovfPath, CompressionFactory.Type.Gz, CheckForCancellation);
                PercentComplete = 95;
                ManifestAndSign(env, appFolder, appFile);
            }
            else
            {
                ManifestAndSign(env, appFolder, appFile);
			}

            Tick(100, Messages.COMPLETED);
		}

        private void ManifestAndSign(EnvelopeType ovfEnv, string appFolder, string appFile)
        {
            if (m_signAppliance || m_createManifest)
                Manifest(ovfEnv, appFolder, appFile);
            
            if (m_signAppliance)
                Sign(m_certificate, appFolder, appFile);
        }

        private void Manifest(EnvelopeType ovfEnv, string pathToOvf, string ovfFileName)
        {
            var manifestFiles = new List<string> {ovfFileName};
            
            var files = ovfEnv?.References?.File;
            if (files != null)
                manifestFiles.AddRange(files.Select(f => f.href).ToList());

            var fileDigests = new List<FileDigest>();
            foreach (var mf in manifestFiles)
            {
                CheckForCancellation();

                var mfPath = Path.Combine(pathToOvf, mf);
                if (!File.Exists(mfPath))
                    continue;

                Description = string.Format(Messages.CREATING_MANIFEST_CHECKSUM, mf);
                log.Info($"Calculating checksum for file {mf}");

                using (FileStream stream = new FileStream(mfPath, FileMode.Open, FileAccess.Read))
                {
                    var hash = StreamUtilities.ComputeHash(stream, out var hashAlgorithm);
                    fileDigests.Add(new FileDigest(Path.GetFileName(mf), hash, hashAlgorithm));
                }
            }

            CheckForCancellation();
            Description = Messages.CREATING_MANIFEST;
            string manifest = Path.Combine(pathToOvf, $"{Path.GetFileNameWithoutExtension(ovfFileName)}{Package.MANIFEST_EXT}");

            using (var stream = new FileStream(manifest, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(stream))
            {
                foreach (var fileDigest in fileDigests)
                    sw.WriteLine(fileDigest.ToManifestLine());
                sw.Flush();
            }

            log.Info($"Created manifest for package {ovfFileName}");
        }

        private void Sign(X509Certificate2 certificate, string pathToOvf, string ovfFileName)
        {
            Description = Messages.SIGNING_APPLIANCE;

            var packageName = Path.GetFileNameWithoutExtension(ovfFileName);
            string manifestFileName = packageName + Package.MANIFEST_EXT;
            string manifestPath = Path.Combine(pathToOvf, manifestFileName);

            CheckForCancellation();

            FileDigest fileDigest;
            using (FileStream stream = new FileStream(manifestPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var signedHash = StreamUtilities.ComputeSignedHash(stream, certificate, out var hashAlgorithm);
                fileDigest = new FileDigest(manifestFileName, signedHash, hashAlgorithm);
            }

            string signatureFileName = packageName + Package.CERTIFICATE_EXT;
            string signaturePath = Path.Combine(pathToOvf, signatureFileName);

            using (FileStream stream = new FileStream(signaturePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(fileDigest.ToManifestLine());

                // Export the certificate encoded in Base64 using DER
                string b64Cert = Convert.ToBase64String(certificate.Export(X509ContentType.SerializedCert));
                
                writer.WriteLine("-----BEGIN CERTIFICATE-----");
                writer.WriteLine(b64Cert);
                writer.WriteLine("-----END CERTIFICATE-----");
                writer.Flush();
            }

            log.Info($"Digitally signed package {ovfFileName}");
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
