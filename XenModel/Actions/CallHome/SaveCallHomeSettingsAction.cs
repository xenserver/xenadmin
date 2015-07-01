using System.Collections.Generic;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SaveCallHomeSettingsAction : PureAsyncAction
    {
        private readonly Pool pool;
        CallHomeSettings callHomeSettings;
        private string authenticationToken;
        private string username;
        private string password;

        public SaveCallHomeSettingsAction(Pool pool, CallHomeSettings callHomeSettings, string authenticationToken, string userName, string passWord, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_SAVE_CALLHOME_SETTINGS, string.Format(Messages.ACTION_SAVING_CALLHOME_SETTINGS, pool.Name), suppressHistory)
        {
            this.pool = pool;
            this.callHomeSettings = callHomeSettings;
            this.authenticationToken = authenticationToken;
            this.username = userName;
            this.password = passWord;
        }
        
        protected override void Run()
        {
            Dictionary<string, string> newConfig = callHomeSettings.ToDictionary(pool.health_check_config);
            if (!string.IsNullOrEmpty(authenticationToken))
                CallHomeAuthenticationAction.SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_TOKEN_SECRET, authenticationToken);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                CallHomeAuthenticationAction.SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_CREDENTIAL_USER_SECRET, username);
                CallHomeAuthenticationAction.SetSecretInfo(Connection, newConfig, CallHomeSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET, password);
            }
            Pool.set_health_check_config(Connection.Session, pool.opaque_ref, newConfig);
        }
    }
}

