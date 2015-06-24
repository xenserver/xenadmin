using System.Collections.Generic;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SaveCallHomeSettingsAction : PureAsyncAction
    {
        private readonly Pool pool;
        CallHomeSettings callHomeSettings;
        private string authenticationToken;

        public SaveCallHomeSettingsAction(Pool pool, CallHomeSettings callHomeSettings, string authenticationToken, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_SAVE_CALLHOME_SETTINGS, string.Format(Messages.ACTION_SAVING_CALLHOME_SETTINGS, pool.Name), suppressHistory)
        {
            this.pool = pool;
            this.callHomeSettings = callHomeSettings;
            this.authenticationToken = authenticationToken;
        }
        
        protected override void Run()
        {
            Dictionary<string, string> newConfig = callHomeSettings.ToDictionary(pool.health_check_config);
            if (!string.IsNullOrEmpty(authenticationToken))
                CallHomeAuthenticationAction.SetUploadTokenSecret(Connection, newConfig, authenticationToken);
            Pool.set_health_check_config(Connection.Session, pool.opaque_ref, newConfig);
        }
    }
}

