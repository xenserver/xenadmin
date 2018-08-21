using System.Collections.Generic;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SetSecondaryManagementPurposeAction : AsyncAction
    {
        private Pool pool;
        private List<PIF> pifs;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SetSecondaryManagementPurposeAction(IXenConnection connection, Pool pool, List<PIF> pifs)
            : base(connection, Messages.ACTION_SET_SECONDARY_MANAGEMENT_PURPOSE_TITLE)
        {
            this.pool = pool;
            this.pifs = pifs;
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pif.set_other_config");
            #endregion
        }

        protected override void Run()
        {
            foreach (PIF pif in pifs)
            {
                XenAPI.Network network = Connection.Resolve(pif.network);
                if (network == null)
                {
                    log.Warn("Network has gone away");
                    return;
                }

                List<PIF> allPifs = Connection.ResolveAll(network.PIFs);
                List<PIF> toReconfigure = pool != null ? allPifs : allPifs.FindAll(
                    p => p.host.opaque_ref == pif.host.opaque_ref);

                if (toReconfigure.Count == 0)
                    return;

                foreach (PIF p in toReconfigure)
                {
                    p.Locked = true;
                    try
                    {
                        pif.SaveChanges(Session, p.opaque_ref, p);
                    }
                    finally
                    {
                        p.Locked = false;
                    }
                }
            }
            Description = Messages.ACTION_SET_SECONDARY_MANAGEMENT_PURPOSE_DONE;
        }
    }
}
