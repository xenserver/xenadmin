using System.Collections.Generic;
using XenAPI;

namespace XenAdmin.Actions
{
    public class CallHomeAuthenticationAction : PureAsyncAction
    {
        private readonly Pool pool;

        public CallHomeAuthenticationAction(Pool pool, string username, string password, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_CALLHOME_AUTHENTICATION, Messages.ACTION_CALLHOME_AUTHENTICATION_PROGRESS, suppressHistory)
        {
            this.pool = pool;
        }
        
        protected override void Run()
        {
            Dictionary<string, string> newConfig = pool.gui_config;
            var authenticationToken = "testToken";
            System.Threading.Thread.Sleep(2000);
            newConfig[CallHomeSettings.AUTHENTICATION_TOKEN] = authenticationToken;
            Pool.set_gui_config(Connection.Session, pool.opaque_ref, newConfig);
        }
    }
}

