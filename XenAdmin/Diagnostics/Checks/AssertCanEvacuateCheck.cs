﻿/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems.VMProblem;
using XenAdmin.Diagnostics.Problems.HostProblem;

using System.Linq;

namespace XenAdmin.Diagnostics.Checks
{
    public class AssertCanEvacuateCheck : Check
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AssertCanEvacuateCheck(Host host)
            : base(host)
        {
        }

        protected List<Problem> CheckHost()
        {
            var problems = new List<Problem>();

            var VMsWithProblems = new List<string>();
            var residentVMs = Host.Connection.ResolveAll(Host.resident_VMs);
            foreach (var residentVM in residentVMs)
            {
                if (residentVM.AutoPowerOn)
                {
                    problems.Add(new AutoStartEnabled(this, residentVM));
                    VMsWithProblems.Add(residentVM.opaque_ref);
                    continue;
                }

                SR sr = residentVM.FindVMCDROMSR();
                if (sr != null && sr.IsToolsSR)
                {
                    problems.Add(new ToolsCD(this, residentVM));
                    VMsWithProblems.Add(residentVM.opaque_ref);
                }
                else if (sr != null && sr.content_type == SR.Content_Type_ISO)
                {
                    problems.Add(new LocalCD(this, residentVM));
                    VMsWithProblems.Add(residentVM.opaque_ref);
                }
            }

            Session session = Host.Connection.DuplicateSession();
            Dictionary<XenRef<VM>, String[]> vms =
                Host.get_vms_which_prevent_evacuation(session, Host.opaque_ref);

            foreach (KeyValuePair<XenRef<VM>, String[]> kvp in vms)
            {
                String[] exception = kvp.Value;
                XenRef<VM> vmRef = kvp.Key;

                if (VMsWithProblems.Contains(vmRef))
                    continue;

                try
                {
                    Problem p = GetProblem(Host.Connection, vmRef, exception);
                    if (p != null)
                        problems.Add(p);
                }
                catch (Exception e)
                {
                    log.Debug("Didn't recognise reason", e);
                    log.Debug(exception);
                    log.Debug(e, e);

                    VM vm = Host.Connection.Resolve(kvp.Key);

                    if (vm != null)
                        problems.Add(new CannotMigrateVM(this, vm));
                }
            }

            return problems;
        }

        private Problem GetProblem(IXenConnection connection, XenRef<VM> vmRef, string[] exception)
        {
            try
            {
                System.Diagnostics.Trace.Assert(exception.Length > 0);

                VM vm;

                switch (exception[0])
                {
                    case Failure.VM_REQUIRES_SR:
                        vm = connection.Resolve<VM>(vmRef);

                        if (vm == null)
                            throw new NullReferenceException(Failure.VM_REQUIRES_SR);

                        XenRef<SR> srRef = new XenRef<SR>(exception[2]);
                        SR sr = connection.Resolve<SR>(srRef);

                        if (sr == null)
                            throw new NullReferenceException(Failure.VM_REQUIRES_SR);

                        if (sr.content_type == SR.Content_Type_ISO)
                        {
                            return new LocalCD(this, vm);
                        }
                        else if (!sr.shared)
                        {
                            // Only show the problem if it is really local storage
                            // As the pbd-plug checks will pick up broken storage.
                            return new LocalStorage(this, vm);
                        }

                        return null;

                    case Failure.VM_MISSING_PV_DRIVERS:
                        vm = connection.Resolve<VM>(vmRef);

                        if (vm == null)
                            throw new NullReferenceException(Failure.VM_MISSING_PV_DRIVERS);

                        return new NoPVDrivers(this, vm);

                    case "VM_OLD_PV_DRIVERS":
                        vm = connection.Resolve<VM>(vmRef);

                        if (vm == null)
                            throw new NullReferenceException("VM_OLD_PV_DRIVERS");

                        return new PVDriversOutOfDate(this, vm);

                    case Failure.NO_HOSTS_AVAILABLE:
                        //CA-63531: Boston server will come here in case of single host pool or standalone host
                        vm = connection.Resolve<VM>(vmRef);

                        if (vm == null)
                            throw new NullReferenceException(Failure.NO_HOSTS_AVAILABLE);

                        return new NoHosts(this, vm);

                    case Failure.HOST_NOT_ENOUGH_FREE_MEMORY:
                        vm = connection.Resolve<VM>(vmRef);

                        if (vm == null)
                            throw new NullReferenceException(Failure.HOST_NOT_ENOUGH_FREE_MEMORY);

                        Pool pool = Helpers.GetPool(vm.Connection);

                        if (pool == null || pool.Connection.Cache.HostCount == 1)
                        {
                            //CA-63531: Cowley server will come here in case of single host pool or standalone host
                            return new NoHosts(this, vm);
                        }

                        Host host = vm.Connection.Resolve(vm.resident_on);
                        return new NotEnoughMem(this, host);

                    case Failure.VM_REQUIRES_NETWORK:
                        vm = connection.Resolve(vmRef);

                        if (vm == null)
                            throw new NullReferenceException(Failure.VM_REQUIRES_NETWORK);

                        XenRef<XenAPI.Network> netRef = new XenRef<XenAPI.Network>(exception[2]);
                        XenAPI.Network network = connection.Resolve(netRef);

                        if (network == null)
                            throw new NullReferenceException(Failure.VM_REQUIRES_NETWORK);

                        return new VMCannotSeeNetwork(this, vm, network);

                    case Failure.VM_HAS_VGPU:
                        vm = connection.Resolve(vmRef);

                        if (vm == null)
                            throw new NullReferenceException(Failure.VM_HAS_VGPU);

                        return new VmHasVgpu(this, vm);

                    default:
                        throw new NullReferenceException(exception[0]);
                }
            }
            catch (Exception e)
            {
                log.Debug("Exception parsing exception", e);
                log.Debug(e, e);

                throw new Failure(new List<String>(exception));
            }
        }

        // This function only tests certain host-wide conditions.
        // Further per-VM conditions are in CheckHost().
        // See RunAllChecks() for how we combine them.
        protected override Problem RunCheck()
        {
            if (!Host.IsLive)
                return new HostNotLiveWarning(this, Host);

            Pool pool = Helpers.GetPool(Host.Connection);
            if (pool != null)
            {
                if (pool.ha_enabled)
                    return new HAEnabledWarning(this, pool, Host);

                if (Helpers.WlbEnabled(pool.Connection))
                    return new WLBEnabledWarning(this, pool, Host);
            }

            return null;
        }

        public override List<Problem> RunAllChecks()
        {
            var list = base.RunAllChecks();
            if (list.Count > 0)
                return list;
            else
                return CheckHost();
        }

        public override string Description
        {
            get { return Messages.ASSERT_CAN_EVACUATE_CHECK_DESCRIPTION; }
        }
    }
}
