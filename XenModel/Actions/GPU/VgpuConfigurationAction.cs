using System;
using System.Collections.Generic;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.GPU
{
    public class VgpuConfigurationAction : PureAsyncAction
    {
        private readonly Dictionary<PGPU, List<XenRef<VGPU_type>>> updatedEnabledVGpuListByPGpu;

        public VgpuConfigurationAction(Dictionary<PGPU, List<XenRef<VGPU_type>>> updatedEnabledVGpuListByPGpu, IXenConnection connection)
            : base(connection, Messages.ACTION_VGPU_CONFIGURATION_SAVING)
        {
            this.updatedEnabledVGpuListByPGpu = updatedEnabledVGpuListByPGpu;
            Description = Messages.ACTION_PREPARING;
            this.Pool = Core.Helpers.GetPool(connection);
        }

        protected override void Run()
        {
            Description = Messages.ACTION_VGPU_CONFIGURATION_SAVING;
            foreach(var kvp in updatedEnabledVGpuListByPGpu)
            {
                var pGpu = kvp.Key;
                PGPU.set_enabled_VGPU_types(Connection.Session,pGpu.opaque_ref, kvp.Value);
            }
            Description = Messages.ACTION_VGPU_CONFIGURATION_SAVED;
        }
    }
}
