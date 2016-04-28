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
        public XenServerPatch XenServerPatch { get; set; }
        public Pool_patch Pool_patch { get; set; }
        public Host Host { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is PoolPatchMapping))
                return false;

            var that = obj as PoolPatchMapping;

            return
                this.XenServerPatch == that.XenServerPatch
                && this.Pool_patch == that.Pool_patch
                && this.Host == that.Host;
        }

        public override int GetHashCode()
        {
            return XenServerPatch.GetHashCode() ^ Pool_patch.GetHashCode() ^ Host.GetHashCode();
        }
    }
}
