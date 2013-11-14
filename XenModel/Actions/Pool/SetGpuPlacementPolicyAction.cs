using XenAPI;

namespace XenAdmin.Actions
{
    public class SetGpuPlacementPolicyAction : PureAsyncAction
    {
        private allocation_algorithm allocationAlgorithm;

        public SetGpuPlacementPolicyAction(Pool pool, allocation_algorithm allocationAlgorithm)
            : base(pool.Connection, Messages.SET_GPU_PLACEMENT_POLICY_ACTION_TITLE, 
            Messages.SET_GPU_PLACEMENT_POLICY_ACTION_DESCRIPTION, true)
        {
            this.allocationAlgorithm = allocationAlgorithm;
        }

        protected override void Run()
        {
            var gpuGroups = Connection.Cache.GPU_groups;

            foreach (var gpuGroup in gpuGroups)
            {
                GPU_group.set_allocation_algorithm(Session, gpuGroup.opaque_ref, allocationAlgorithm);
            }

            Description = Messages.SET_GPU_PLACEMENT_POLICY_ACTION_DONE;
        }
    }
}
