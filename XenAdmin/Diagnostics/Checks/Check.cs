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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    public abstract class Check
    {
        /// <summary>
        /// When set to true, the checks always return zero problems.
        /// You can use this property to prevent further checks from running.
        /// <br />
        /// For its recommended use with the <see cref="PatchingWizard_PrecheckPage"/>, check <see cref="PatchingWizard_PrecheckPage.GetPermanentCheck"/>.
        /// </summary>
        public bool Completed { get; set; } = false;

        protected abstract Problem RunCheck();

        /// <summary>
        /// By default, most Checks return zero or one Problems, but a 
        /// Check can override this to return multiple Problems
        /// </summary>
        public virtual List<Problem> RunAllChecks()
        {
            var list = new List<Problem>();

            if (Completed)
            {
                return list;
            }

            //normally checks will have not been added to the list if they can't run, but check again
            if (CanRun())
            {
                var problem = RunCheck();
                if (problem != null)
                    list.Add(problem);
            }

            return list;
        }

        public abstract string Description { get; }
        public abstract IList<IXenObject> XenObjects { get; }

        public virtual string SuccessfulCheckDescription =>
            string.IsNullOrEmpty(Description)
                ? string.Empty
                : string.Format(Messages.PATCHING_WIZARD_CHECK_OK, Description);

        public virtual bool CanRun()
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Check item))
            {
                return false;
            }


            if (XenObjects == null && item.XenObjects != null)
            {
                return true;
            }


            return (XenObjects ?? new List<IXenObject>()).SequenceEqual(item.XenObjects ?? new List<IXenObject>());
        }

        public override int GetHashCode()
        {
            return XenObjects.GetHashCode();
        }
    }


    public abstract class UpgradeCheck : Check
    {
        protected UpgradeCheck(List<Host> hosts)
        {
            Hosts = hosts;
        }

        protected List<Host> Hosts { get; }

        public sealed override IList<IXenObject> XenObjects => Hosts.Cast<IXenObject>().ToList();

        public override string SuccessfulCheckDescription =>
            string.IsNullOrEmpty(Description)
                ? string.Empty
                : string.Format(Messages.PATCHING_WIZARD_CHECK_OK, Description);
    }


    public abstract class PoolCheck : Check
    {
        protected PoolCheck(Pool pool)
        {
            Pool = pool;
        }

        protected Pool Pool { get; }

        public sealed override IList<IXenObject> XenObjects => new IXenObject[] { Pool };

        public override string SuccessfulCheckDescription =>
            string.IsNullOrEmpty(Description)
                ? string.Empty
                : string.Format(Messages.PATCHING_WIZARD_CHECK_ON_XENOBJECT_OK, Helpers.GetPoolOfOne(Pool.Connection), Description);
    }


    public abstract class HostCheck : Check
    {
        protected HostCheck(Host host)
        {
            Debug.Assert(host != null);
            Host = host;
        }

        protected Host Host { get; }

        public sealed override IList<IXenObject> XenObjects => new IXenObject[] { Host };

        public override string SuccessfulCheckDescription =>
            string.IsNullOrEmpty(Description)
                ? string.Empty
                : string.Format(Messages.PATCHING_WIZARD_CHECK_ON_XENOBJECT_OK, Host.Name(), Description);
    }
}
