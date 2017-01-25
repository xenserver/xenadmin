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
using XenAdmin;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Actions;

/* Interface that will be implemented by VMSS and VMPP XenObjects*/
namespace XenAPI
{
    public enum policy_frequency
    {
        hourly, daily, weekly, unknown
    }

    public enum policy_backup_type
    {
        snapshot, checkpoint, snapshot_with_quiesce, unknown
    }

    public interface IVMPolicy
    {
        string Name { get; }
        string name_description {get; }
        string uuid { get; }
        List<XenRef<VM>> VMs { get; }
        IXenConnection Connection { get; }
        bool is_enabled { get; }
        bool is_running { get; }
        bool is_archiving { get; }
        string LastResult { get;}
        DateTime GetNextRunTime();
        DateTime GetNextArchiveRunTime();
        Type _Type { get; }
        List<PolicyAlert> PolicyAlerts { get; }
        bool hasArchive { get; }
        void set_vm_policy(Session session, string _vm, string _value);
        void do_destroy(Session session, string _policy);
        string run_now(Session session, string _policy);
        string opaque_ref { get; }
        void set_is_enabled(Session session, string _policy, bool _is_enabled);
        PureAsyncAction getAlertsAction(IVMPolicy policy, int hoursfromnow);
        policy_frequency policy_frequency { get; set; }
        Dictionary<string, string> policy_schedule { get; set; }
        long policy_retention { get; set; }
        string backup_schedule_min { get; }
        string backup_schedule_hour { get; }
        string backup_schedule_days { get; }
        policy_backup_type policy_type { get; set; }
        XenRef<Task> async_task_create(Session session);
        void set_policy(Session session, string _vm, string _value);

     }
}
