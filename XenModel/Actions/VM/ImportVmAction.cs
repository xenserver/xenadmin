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
using XenAdmin.Core;
using XenAPI;
using System.Threading;
using XenAdmin.Actions.VMActions;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class ImportVmAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int HTTP_PUT_TIMEOUT = 30*60*1000; //30 minutes

        private string m_filename;
        private readonly Host m_affinity;
        public const string IMPORT_TASK = "import_task";

		private bool m_startAutomatically;
    	private List<VIF> m_VIFs;

    	private object monitor = new object();
		private bool m_wizardDone;

        private readonly Action<VM, bool> _warningDelegate;
        private readonly Action<VMStartAbstractAction, Failure> _failureDiagnosisDelegate;

    	/// <summary>
        /// These calls are always required to import a VM, as opposed to the set which are only called if needed set in the constructor of this action.
        /// </summary>
        public static readonly RbacMethodList ConstantRBACRequirements = new RbacMethodList(            
            "vm.set_name_label",
            "network.destroy",
            "vif.create",
            "vif.destroy",
            "http/put_import"
        );

        public ImportVmAction(IXenConnection connection, Host affinity, string filename, SR sr,
            Action<VM, bool> warningDelegate, Action<VMStartAbstractAction, Failure> failureDiagnosisDelegate)
			: base(connection, "")
        {
            Description = Messages.IMPORTVM_PREP;
            Pool = Helpers.GetPoolOfOne(connection);
			m_affinity = affinity;
			Host = affinity ?? connection.Resolve(Pool.master);
			SR = sr;
			VM = null;
			m_filename = filename;
            _warningDelegate = warningDelegate;
            _failureDiagnosisDelegate = failureDiagnosisDelegate;

			#region RBAC Dependencies

			ApiMethodsToRoleCheck.AddRange(ConstantRBACRequirements);

			if (affinity != null)
				ApiMethodsToRoleCheck.Add("vm.set_affinity");

			ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
			ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);

			#endregion

            Title = string.Format(Messages.IMPORTVM_TITLE, filename, Host.NameWithLocation());
        }

        protected override void Run()
        {
            SafeToExit = false;

            string vmRef = UploadFile(out string importTaskRef);
            if (string.IsNullOrEmpty(vmRef))
                return;

            // Now let's try and set the affinity and start the VM

            while (!Cancelling && (VM = Connection.Resolve(new XenRef<VM>(vmRef))) == null)
                Thread.Sleep(100);

            if (Cancelling)
                throw new CancelledException();

            var vmRec = VM.get_record(Session, vmRef);
            var isTemplate = vmRec.is_a_template;

            if (isTemplate && vmRec.is_default_template && Helpers.FalconOrGreater(Connection))
            {
                if (!vmRec.other_config.ContainsKey(IMPORT_TASK) || vmRec.other_config[IMPORT_TASK] != importTaskRef)
                    throw new Exception(string.Format(Messages.IMPORT_TEMPLATE_ALREADY_EXISTS, BrandManager.ProductBrand));
            }

            Description = isTemplate ? Messages.IMPORT_TEMPLATE_UPDATING_TEMPLATE : Messages.IMPORTVM_UPDATING_VM;
            VM.set_name_label(Session, vmRef, DefaultVMName(VM.get_name_label(Session, vmRef)));

			if (!isTemplate && m_affinity != null)
				VM.set_affinity(Session, vmRef, m_affinity.opaque_ref);

            // Wait here for the wizard to finish
            Description = isTemplate ? Messages.IMPORT_TEMPLATE_WAITING_FOR_WIZARD : Messages.IMPORTVM_WAITING_FOR_WIZARD;
			lock (monitor)
			{
				while (!(m_wizardDone || Cancelling))
					Monitor.Wait(monitor);
			}

            if (Cancelling)
                throw new CancelledException();

            if (m_VIFs != null)
            {
                Description = isTemplate ? Messages.IMPORT_TEMPLATE_UPDATING_NETWORKS : Messages.IMPORTVM_UPDATING_NETWORKS;

                // For ElyOrGreater hosts, we can move the VIFs to another network

                List<XenRef<VIF>> vifs = VM.get_VIFs(Session, vmRef);
                List<XenAPI.Network> networks = new List<XenAPI.Network>();

                foreach (XenRef<VIF> vif in vifs)
                {
                    // Save the network as we may have to delete it later
                    XenAPI.Network network = Connection.Resolve(VIF.get_network(Session, vif));
                    if (network != null)
                        networks.Add(network);

                    var vifObj = Connection.Resolve(vif);
                    if (vifObj == null)
                        continue;

                    // try to find a VIF in the m_VIFs list which matches the device field,
                    // then move it to the desired network and remove it from the m_VIFs list
                    // so we don't create it again later

                    var matchingVif = m_VIFs.FirstOrDefault(v => v.device == vifObj.device);
                    if (matchingVif != null)
                    {
                        VIF.move(Session, vif, matchingVif.network);
                        m_VIFs.Remove(matchingVif);
                        continue;
                    }

                    // destroy the VIF, if we haven't managed to move it
                    VIF.destroy(Session, vif);
                }

                // recreate VIFs if needed (m_VIFs can be empty, if we moved all the VIFs in the previous step)
                foreach (VIF vif in m_VIFs)
                {
                    vif.VM = new XenRef<VM>(vmRef);
                    VIF.create(Session, vif);
                }

                // now delete any Networks associated with this task if they have no VIFs

                foreach (XenAPI.Network network in networks)
                {
                    if (!network.other_config.ContainsKey(IMPORT_TASK) || network.other_config[IMPORT_TASK] != importTaskRef)
                        continue;

                    try
                    {
                        var record = XenAPI.Network.get_record(Session, network.opaque_ref);
                        if (record.VIFs.Count > 0 || record.PIFs.Count > 0)
                            continue;

                        XenAPI.Network.destroy(Session, network.opaque_ref);
                    }
                    catch (Exception e)
                    {
                        log.Error($"Exception while deleting network {network.Name()}. Squashing.", e);
                    }
                }
            }

            var vm = Connection.WaitForCache(new XenRef<VM>(vmRef));
            Description = isTemplate ? Messages.IMPORT_TEMPLATE_IMPORTCOMPLETE : Messages.IMPORTVM_IMPORTCOMPLETE;

            if (vm != null && !vm.is_a_template && m_startAutomatically)
            {
                if (vm.power_state == vm_power_state.Suspended)
                    new VMResumeAction(vm, _warningDelegate, _failureDiagnosisDelegate).RunAsync();
                else
                    new VMStartAction(vm, _warningDelegate, _failureDiagnosisDelegate).RunAsync();
            }
        }

        private int VMsWithName(string name)
        {
            int i = 0;

            foreach (VM v in Connection.Cache.VMs)
                if (v.name_label == name)
                    i++;

            return i;
        }

        private string DefaultVMName(string vmName)
        {
            string name = vmName;
            int i = 0;

            if (VMsWithName(vmName) > 1)
                while (VMsWithName(name) > 0)
                {
                    i++;
                    name = string.Format(Messages.NEWVM_DEFAULTNAME, vmName, i);
                }

            return name;
        }

        private string UploadFile(out string importTaskRef)
        {
            Host host = SR.GetStorageHost();

            string hostUrl;
            if (host == null)
            {
                Uri uri = new Uri(Session.Url);
                hostUrl = uri.Host;
                log.InfoFormat("SR {0} is shared or disconnected or SR host could not be resolved. Uploading XVA {1} to {2}", SR.Name(), m_filename, hostUrl);
            }
            else
            {
                hostUrl = host.address;
                log.InfoFormat("Resolved host for SR {0}. Uploading XVA {1} to {2}", SR.Name(), m_filename, hostUrl);
            }

            try
            {
                RelatedTask = Task.create(Session, "put_import_task", hostUrl);
                //at the end of polling, the RelatedTask is destroyed; make a note of it for later use
                importTaskRef = RelatedTask.opaque_ref;

                log.DebugFormat("HTTP PUTTING file from {0} to {1}", m_filename, hostUrl);

                HTTP_actions.put_import(percent =>
                    {
                        Tick(percent, string.Format(Messages.IMPORTVM_UPLOADING, Path.GetFileName(m_filename), Pool.Name(), percent));
                    },
                    () => XenAdminConfigManager.Provider.ForcedExiting || GetCancelling(),
                    HTTP_PUT_TIMEOUT, hostUrl,
                    XenAdminConfigManager.Provider.GetProxyFromSettings(Connection),
                    m_filename, RelatedTask.opaque_ref, Session.opaque_ref, false, false, SR.opaque_ref);

                PollToCompletion();
                return Result;
            }
            catch (Exception e)
            {
                PollToCompletion();
                if (e is CancelledException || e is HTTP.CancelledException || e.InnerException is CancelledException)
                    throw new CancelledException();
                throw;
            }
        }

		public override void RecomputeCanCancel()
		{
		    CanCancel = m_wizardDone;
		}

        protected override void CancelRelatedTask()
        {
            log.InfoFormat("VM/Template import from '{0}' cancelled", m_filename);

			lock (monitor)
			{
			    Monitor.PulseAll(monitor);
			}

            base.CancelRelatedTask();
        }

		public void EndWizard(bool start, List<VIF> vifs)
		{
			m_startAutomatically = start;
			m_VIFs = vifs;

			lock (monitor)
			{
				m_wizardDone = true;
				Monitor.PulseAll(monitor);
			}
		}
    }
}
