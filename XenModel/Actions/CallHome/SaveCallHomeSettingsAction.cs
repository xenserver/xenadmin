using System.Collections.Generic;
using XenAPI;

namespace XenAdmin.Actions
{
    public class SaveCallHomeSettingsAction : PureAsyncAction
    {
        private readonly Pool pool;
        CallHomeSettings callHomeSettings;

        public SaveCallHomeSettingsAction(Pool pool, CallHomeSettings callHomeSettings, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_SAVE_CALLHOME_SETTINGS, string.Format(Messages.ACTION_SAVING_CALLHOME_SETTINGS, pool.Name), suppressHistory)
        {
            this.pool = pool;
            this.callHomeSettings = callHomeSettings;
        }
        
        protected override void Run()
        {
            Dictionary<string, string> newConfig = callHomeSettings.ToDictionary(pool.gui_config);
            Pool.set_gui_config(Connection.Session, pool.opaque_ref, newConfig);
        }
    }
}

