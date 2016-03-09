using System;
using System.Collections.Generic;
using XenAPI;

namespace XenAdmin.Actions
{
    public class PoolPatchCleanAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pool_patch patch;

        public PoolPatchCleanAction(Pool pool, Pool_patch patch, bool suppressHistory)
            : base(pool.Connection, string.Format(Messages.UPDATES_WIZARD_REMOVING_UPDATE, patch.Name, pool.Name), suppressHistory)
        {
            this.patch = patch;
            if (patch == null)
                throw new ArgumentNullException("pool_patch");

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool_patch_pool_clean");
            #endregion
            
        }

        protected override void Run()
        {
            this.Description = string.Format(Messages.REMOVING_UPDATE, patch.Name);
            List<Pool_patch> poolPatches = new List<Pool_patch>(Connection.Cache.Pool_patches);
            var poolPatch = poolPatches.Find(delegate(Pool_patch otherPatch)
            {
                return string.Equals(otherPatch.uuid, patch.uuid, StringComparison.OrdinalIgnoreCase);
            });

            if (poolPatch != null)
            {
                Pool_patch.pool_clean(Session, poolPatch.opaque_ref);
            }
            Description = String.Format(Messages.REMOVED_UPDATE, patch.Name);
        }
    }
}
