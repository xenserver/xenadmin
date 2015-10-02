using System.Linq;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Commands
{
    internal class UpgradeVmLaunchUICommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public UpgradeVmLaunchUICommand()
        {
        }

        public UpgradeVmLaunchUICommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            
            new UpgradeVmDialog(selection, MainWindowCommandInterface).Show(Parent);

        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            var s = selection.AsXenObjects<Pool>();

            return
                s.Count > 0 && s[0] as Pool != null && s[0].Connection != null && s[0].Connection.IsConnected && Helpers.DundeeOrGreater(s[0].Connection);
        }
    }
}
