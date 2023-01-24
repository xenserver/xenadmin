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
using System.Text;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Actions
{
    public interface IStatus
    {
        bool InProgress { get; }
        bool IsCompleted { get; }
        bool Succeeded { get; }
        bool IsCancelled { get; }
        bool IsError { get; }
        bool IsIncomplete { get; }
        bool IsQueued { get; }
    }

    public class ActionBase : IStatus
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static readonly log4net.ILog AuditLog = log4net.LogManager.GetLogger("Audit");

        public string Title;

        /// <summary>
        /// The client-side, local time that this action started.
        /// </summary>
        public DateTime Started = DateTime.Now;

        /// <summary>
        /// The client-side, local time that this action finished.
        /// </summary>
        public DateTime Finished;

        // Used for pairing incoming MessageActions with the action that triggered them

        private Pool _pool;
        private Host _host;
        private VM _vM;
        private SR _sR;
        private VM _template;

        private IXenConnection connection;
        public virtual IXenConnection Connection
        {
            get{ return connection; }
            protected set { connection = value; }
        }

        /// <summary>
        /// Indicates whether XenCenter can exit safely while a task is still in
        /// progress. If it's not safe, the MainWindow will try to cancel the
        /// running task on exit. If it is safe, the task will be left to complete
        /// after XenCenter has exited.
        /// Use this for composite tasks when an action has more than one xapi call.
        /// </summary>
        public bool SafeToExit { get; protected set; } = true;

        /// <summary>
        /// A list of opaque_refs, giving all the objects that this action applies to.
        /// Pool, Host, VM, SR, and Template will be added to this list automatically.
        /// </summary>
        public readonly List<string> AppliesTo = new List<string>();

        public Pool Pool
        {
            get
            {
                return _pool;
            }
            set
            {
                _pool = value;
                SetAppliesTo(Pool);
                if (Pool != null && Pool.Connection != null &&
                    Helpers.GetPool(Pool.Connection) == null &&
                    Pool.Connection.Cache.Hosts.Length == 1)
                {
                    SetAppliesTo(Pool.Connection.Cache.Hosts[0]);
                }
            }
        }

        public Host Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
                SetAppliesTo(Host);
            }
        }

        public VM VM
        {
            get
            {
                return _vM;
            }
            set
            {
                _vM = value;

                if (value == null)
                    return;

                if (_vM.is_a_snapshot)
                {
                    VM parentVM = _vM.Connection.Resolve<VM>(_vM.snapshot_of);
                    SetAppliesTo(parentVM);
                }
                else
                    SetAppliesTo(VM);
            }
        }

        public SR SR
        {
            get
            {
                return _sR;
            }
            set
            {
                _sR = value;
                SetAppliesTo(SR);

                if (Host == null)
                    Host = SR.Home();
            }
        }

        public VM Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
                SetAppliesTo(Template);
            }
        }

        protected void SetAppliesTo(IXenObject xo)
        {
            if (xo == null)
                return;

            if (xo is Pool)
            {
                Pool pool = (Pool)xo;
                AppliesTo.Add(pool.opaque_ref);
            }
            else if (xo is Host)
            {
                Host host = (Host)xo;
                SetAppliesTo(Helpers.GetPoolOfOne(host.Connection));
                AppliesTo.Add(host.opaque_ref);
            }
            else if (xo is VM)
            {
                VM vm = (VM)xo;
                SetAppliesTo(vm.Home());
                AppliesTo.Add(vm.opaque_ref);
            }
            else if (xo is SR)
            {
                SR sr = (SR)xo;
                SetAppliesTo(sr.Home());
                AppliesTo.Add(sr.opaque_ref);
            }
            else if (xo is VDI)
            {
                VDI vdi = (VDI)xo;
                SetAppliesTo(vdi.Connection.Resolve(vdi.SR));
            }
        }

        private string _description;
        private bool _isCompleted;
        private int _percentComplete;
        private Exception _exception;
        private bool _suppressHistory;

        #region Events
        public event Action<ActionBase> Changed;
        public event Action<ActionBase> Completed;
        public static event Action<ActionBase> NewAction;
        #endregion

        public bool LogDescriptionChanges = true;

        public bool ShowProgress { get; protected set; } = true;

        protected ActionBase(string title, string description, bool suppressHistory)
        {
            Title = title;
            _description = description;
            log.Debug(_description);

            SuppressHistory = suppressHistory;
        }

        protected bool SuppressHistory
        {
            get => _suppressHistory;
            set
            {
                _suppressHistory = value;

                if (!_suppressHistory)
                    NewAction?.Invoke(this);
            }
        }

        /// <remarks>
        /// If you want to set the PercentComplete and the Description at the
        /// same time,  use Tick() in order to avoid firing OnChanged() twice
        /// </remarks>
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    if (LogDescriptionChanges)
                        log.Debug(_description);
                    OnChanged();
                }
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnChanged();
                    OnCompleted();
                }
            }
        }

        /// <remarks>
        /// If you want to set the PercentComplete and the Description at the
        /// same time,  use Tick() in order to avoid firing OnChanged() twice
        /// </remarks>
        public virtual int PercentComplete
        {
            get => _percentComplete;
            set
            {
                if (_percentComplete != value)
                {
                    System.Diagnostics.Debug.Assert(0 <= value && value <= 100, $"Percent is {value}");

                    var percent = value;
                    if (percent < 0)
                        percent = 0;
                    else if (percent > 100)
                        percent = 100;

                    _percentComplete = percent;
                    OnChanged();
                }
            }
        }

        protected bool SuppressProgressReport { get; set; }

        protected void Tick(int percent, string description)
        {
            if (_percentComplete != percent || _description != description)
            {
                _description = description;

                System.Diagnostics.Debug.Assert(0 <= percent && percent <= 100, $"Percent is {percent}");

                if (percent < 0)
                    percent = 0;
                else if (percent > 100)
                    percent = 100;
                _percentComplete = percent;

                OnChanged();
            }
        }

        public bool Succeeded => IsCompleted && Exception == null;
        public bool IsCancelled => IsCompleted && !Succeeded && Exception is CancelledException;
        public bool IsError => IsCompleted && !Succeeded && !(Exception is CancelledException);
        public bool IsIncomplete => false;
        public bool IsQueued => false;
        public bool InProgress => !IsCompleted;

        public Exception Exception
        {
            get { return _exception; }
            protected set
            {
                _exception = value;
                OnChanged();
            }
        }

        public void SetObject(IXenObject model)
        {
            if (model is Pool)
                Pool = (Pool)model;
            else if (model is Host)
                Host = (Host)model;
            else if (model is VM)
                VM = (VM)model;
            else if (model is SR)
                SR = (SR)model;
            else if (model is VDI)
            {
                VDI vdi = (VDI)model;
                SetObject(vdi.Connection.Resolve(vdi.SR));
            }
            else if (model is VBD)
            {
                VBD vbd = (VBD)model;
                SetObject(vbd.Connection.Resolve(vbd.VM));
            }

        }

        public virtual bool CanCancel
        {
            get { return false; }
            protected set { throw new InvalidOperationException(); }
        }

        public virtual void Cancel()
        {
            throw new InvalidOperationException();
        }

        protected void OnChanged()
        {
            if (!SuppressProgressReport)
                Changed?.Invoke(this);
        }

        protected virtual void OnCompleted()
        {
            Completed?.Invoke(this);
        }

        protected void MarkCompleted(Exception e = null)
        {
            if (e != null)
            {
                log.Debug(e, e);
                Exception = e;
            }

            Finished = DateTime.Now;
            PercentComplete = 100;
            IsCompleted = true;
        }

        #region Audit logging

        protected virtual void AuditLogStarted()
        {
            AuditLog.InfoFormat("Operation started: {0}", AuditDescription());
        }

        protected virtual void AuditLogSuccess()
        {
            AuditLog.InfoFormat("Operation success: {0}", AuditDescription());
        }

        protected virtual void AuditLogFailure()
        {
            AuditLog.WarnFormat("Operation failure: {0}", AuditDescription());
        }

        protected virtual void AuditLogCancelled()
        {
            AuditLog.InfoFormat("Operation cancelled: {0}", AuditDescription());
        }

        protected virtual string AuditDescription()
        {
            return string.Format("{0}: {1}: {2}{3}", GetType().Name,
                DescribeConnection(), DescribeObject(), Description);
        }

        protected virtual string DescribeConnection()
        {
            if(Connection == null)
                return "Connection unknown";

            return Helpers.GetName(Connection);
        }

        protected virtual string DescribeObject()
        {
            StringBuilder sb = new StringBuilder();
            if (VM != null)
            {
                sb.Append(DescribeVM(VM));
            }
            if (Pool != null)
            {
                sb.Append(DescribePool(Pool));
            }
            if (Host != null)
            {
                sb.Append(DescribeHost(Host));
            }
            return sb.ToString();
        }

        protected virtual string DescribeVM(VM v)
        {
            return string.Format("VM {0} ({1}): ", v.uuid, v.Name());
        }

        protected virtual string DescribePool(Pool p)
        {
            return string.Format("Pool {0} ({1}): ", p.uuid, p.Name());
        }

        protected virtual string DescribeHost(Host h)
        {
            return string.Format("Host {0} ({1}): ", h.uuid, h.Name());
        }
        #endregion
    }
}
