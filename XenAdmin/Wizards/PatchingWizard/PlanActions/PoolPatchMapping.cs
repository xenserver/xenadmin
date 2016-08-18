using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class PoolPatchMapping
    {
        public XenServerPatch XenServerPatch { get; private set; }
        public Pool_patch Pool_patch { get; private set; }
        public Host MasterHost { get; private set; }

        public PoolPatchMapping(XenServerPatch xenServerPatch, Pool_patch pool_patch, Host masterHost)
        {
            if (xenServerPatch == null)
                throw new ArgumentNullException("xenServerPatch");

            if (pool_patch == null)
                throw new ArgumentNullException("pool_patch");

            if (masterHost == null)
                throw new ArgumentNullException("masterHost");

            this.XenServerPatch = xenServerPatch;
            this.Pool_patch = pool_patch;
            this.MasterHost = masterHost;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PoolPatchMapping))
                return false;

            var that = obj as PoolPatchMapping;

            return
                this.XenServerPatch == that.XenServerPatch
                && this.Pool_patch == that.Pool_patch
                && this.MasterHost == that.MasterHost;
        }

        public override int GetHashCode()
        {
            return XenServerPatch.GetHashCode() ^ Pool_patch.GetHashCode() ^ MasterHost.GetHashCode();
        }
    }
}
