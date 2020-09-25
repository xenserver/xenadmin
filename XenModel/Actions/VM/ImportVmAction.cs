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
			: base(connection, string.Format(Messages.IMPORTVM_TITLE, filename, Helpers.GetName(connection)), Messages.IMPORTVM_PREP)
		{
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
		}

    	private string GetVmRef(string result)
        {
            if (string.IsNullOrEmpty(result))
                return null;

            string head = "<value><array><data><value>";
            string tail = "</value></data></array></value>";

            if (!result.StartsWith(head) || !result.EndsWith(tail))
                return null;

            int start = head.Length;
            int length = result.IndexOf(tail) - start;

            return result.Substring(start, length);
        }

        protected override void Run()
        {
            SafeToExit = false;
        	bool isTemplate;

            string vmRef = GetVmRef(applyFile());
            if (string.IsNullOrEmpty(vmRef))
                return;

            // Now let's try and set the affinity and start the VM

            while (!Cancelling && (VM = Connection.Resolve(new XenRef<VM>(vmRef))) == null)
                Thread.Sleep(100);

            if (Cancelling)
                throw new CancelledException();

            isTemplate = VM.get_is_a_template(Session, vmRef);
            if (isTemplate && Helpers.FalconOrGreater(Connection) && VM.get_is_default_template(Session, vmRef))
            {
                var otherConfig = VM.get_other_config(Session, vmRef);
                if (!otherConfig.ContainsKey(IMPORT_TASK) || otherConfig[IMPORT_TASK] != RelatedTask.opaque_ref)
                {
                    throw new Exception(Messages.IMPORT_TEMPLATE_ALREADY_EXISTS);
                }
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

                // For ElyOrGreater hosts, we can move the VIFs to another network, 
                // but for older hosts we need to destroy all vifs and recreate them

                List<XenRef<VIF>> vifs = VM.get_VIFs(Session, vmRef);
                List<XenAPI.Network> networks = new List<XenAPI.Network>();

                bool canMoveVifs = Helpers.ElyOrGreater(Connection);

                foreach (XenRef<VIF> vif in vifs)
                {
                    // Save the network as we may have to delete it later
                    XenAPI.Network network = Connection.Resolve(VIF.get_network(Session, vif));
                    if (network != null)
                        networks.Add(network);

                    if (canMoveVifs)
                    {
                        var vifObj = Connection.Resolve(vif);
                        if (vifObj == null)
                            continue;
                        // try to find a matching VIF in the m_proxyVIFs list, based on the device field
                        var matchingProxyVif = m_VIFs.FirstOrDefault(proxyVIF => proxyVIF.device == vifObj.device);
                        if (matchingProxyVif != null)
                        {
                            // move the VIF to the desired network
                            VIF.move(Session, vif, matchingProxyVif.network);
                            // remove matchingProxyVif from the list, so we don't create the VIF again later
                            m_VIFs.Remove(matchingProxyVif);
                            continue;
                        }
                    }

                    // destroy the VIF, if we haven't managed to move it
                    VIF.destroy(Session, vif);
                }

                // recreate VIFs if needed (m_proxyVIFs can be empty, if we moved all the VIFs in the previous step)
                foreach (VIF vif in m_VIFs)
                {
                    vif.VM = new XenRef<VM>(vmRef);
                    VIF.create(Session, vif);
                }

                // now delete any Networks associated with this task if they have no VIFs

                foreach (XenAPI.Network network in networks)
                {
                    if (!network.other_config.ContainsKey(IMPORT_TASK))
                        continue;

                    if (network.other_config[IMPORT_TASK] != RelatedTask.opaque_ref)
                        continue;

                    try
                    {
                        if (XenAPI.Network.get_VIFs(Session, network.opaque_ref).Count > 0)
                            continue;

                        if (XenAPI.Network.get_PIFs(Session, network.opaque_ref).Count > 0)
                            continue;

                        XenAPI.Network.destroy(Session, network.opaque_ref);
                    }
                    catch (Exception e)
                    {
                        log.Error($"Exception while deleting network {network.Name()}. Squashing.", e);
                    }
                }
            }

            //get the record before marking the action as completed
            var vm = VM.get_record(Session, vmRef);
            vm.opaque_ref = vmRef;

            Description = isTemplate ? Messages.IMPORT_TEMPLATE_IMPORTCOMPLETE : Messages.IMPORTVM_IMPORTCOMPLETE;

            if (!vm.is_a_template && m_startAutomatically)
                new VMStartAction(vm, _warningDelegate, _failureDiagnosisDelegate).RunAsync();
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

        private string applyFile()
        {
            log.DebugFormat("Importing XVA from {0} to SR {1}", m_filename, SR.Name());

            Host host = SR.GetStorageHost();

            string hostURL;
            if (host == null)
            {
                Uri uri = new Uri(Session.Url);
                hostURL = uri.Host;
            }
            else
            {
                log.DebugFormat("SR is not shared -- redirecting to {0}", host.address);
                hostURL = host.address;
            }

            log.DebugFormat("Using {0} for import", hostURL);

            return HTTPHelper.Put(this, HTTP_PUT_TIMEOUT, m_filename, hostURL,
                                  (HTTP_actions.put_ssbbs)HTTP_actions.put_import,
                                  Session.opaque_ref, false, false, SR.opaque_ref);
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
