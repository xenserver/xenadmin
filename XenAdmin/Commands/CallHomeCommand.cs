using System.Linq;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.CallHome;

namespace XenAdmin.Commands
{
    internal class CallHomeCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public CallHomeCommand()
        {
        }

        public CallHomeCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            if (Program.MainWindow.HealthCheckOverviewLauncher != null)
                Program.MainWindow.HealthCheckOverviewLauncher.LaunchIfRequired(false, selection);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return ConnectionsManager.XenConnectionsCopy.Any(xenConnection => xenConnection.IsConnected);
        }
    }
}
