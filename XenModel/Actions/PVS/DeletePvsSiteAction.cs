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
            RelatedTask = PVS_site.async_forget(Session, pvsSite.opaque_ref);
            PollToCompletion();
            Description = Messages.ACTION_DELETE_PVS_SITE_DONE;
            PercentComplete = 100;
        }
    }
}
