using System;
using System.Linq;
using XenAPI;

namespace XenAdmin.Actions
{
    public class DeletePvsSiteAction : PureAsyncAction
    {
        private readonly PVS_site pvsSite;

        public DeletePvsSiteAction(PVS_site pvsSite)
            : base(pvsSite.Connection, string.Format(Messages.ACTION_DELETE_PVS_SITE_TITLE, pvsSite.Name.Ellipsise(50)),
                    Messages.ACTION_DELETE_PVS_SITE_DESCRIPTION, false)
        {
            System.Diagnostics.Trace.Assert(pvsSite != null);
            this.pvsSite = pvsSite;
        }
        
        protected override void Run()
        {
            // check if there are any running proxies
            var pvsProxies = Connection.Cache.PVS_proxies.Where(s => s.site.opaque_ref == pvsSite.opaque_ref).ToList();
            if (pvsProxies.Count > 0)
            {
                throw new Failure(Failure.PVS_SITE_CONTAINS_RUNNING_PROXIES);
            }

            // delete PVS_servers
            var pvsServers = Connection.Cache.PVS_servers.Where(s => s.site.opaque_ref == pvsSite.opaque_ref).ToList();
            int inc = pvsServers.Count > 0 ? 50 / pvsServers.Count : 50;
            foreach (var pvsServer in pvsServers)
            {
                RelatedTask = PVS_server.async_forget(Session, pvsServer.opaque_ref);
                PollToCompletion(PercentComplete, PercentComplete + inc);
            }

            RelatedTask = PVS_site.async_forget(Session, pvsSite.opaque_ref);
            PollToCompletion();
            Description = Messages.ACTION_DELETE_PVS_SITE_DONE;
            PercentComplete = 100;
        }
    }
}
