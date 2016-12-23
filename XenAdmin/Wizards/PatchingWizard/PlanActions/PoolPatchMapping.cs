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
        public Pool_update Pool_update { get; private set; }
        public Host MasterHost { get; private set; }

        public PoolPatchMapping(XenServerPatch xenServerPatch, Pool_update pool_update, Host masterHost)
        {
            if (xenServerPatch == null)
                throw new ArgumentNullException("xenServerPatch");

            if (pool_update == null)
                throw new ArgumentNullException("pool_update");

            if (masterHost == null)
                throw new ArgumentNullException("masterHost");

            this.XenServerPatch = xenServerPatch;
            this.Pool_update = pool_update;
            this.MasterHost = masterHost;
        }

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
            var that = obj as PoolPatchMapping;

            if (that == null)
                return false;

            return
                this.XenServerPatch != null && this.XenServerPatch.Equals(that.XenServerPatch)
                && (this.Pool_patch != null && this.Pool_patch.Equals(that.Pool_patch) || this.Pool_update != null && this.Pool_update.Equals(that.Pool_update))
                && this.MasterHost != null && that.MasterHost != null & this.MasterHost.uuid == that.MasterHost.uuid;
        }

        public override int GetHashCode()
        {
            return XenServerPatch.GetHashCode() ^ Pool_patch.GetHashCode() ^ MasterHost.GetHashCode();
        }
    }
}
