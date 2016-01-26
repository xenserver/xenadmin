using System.Linq;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Commands
{
    internal class HealthCheckCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public HealthCheckCommand()
        {
        }

        public HealthCheckCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            if (Program.MainWindow.HealthCheckOverviewLauncher != null && !XenAdmin.Core.Registry.HealthCheckHidden)
                Program.MainWindow.HealthCheckOverviewLauncher.LaunchIfRequired(false, selection);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return ConnectionsManager.XenConnectionsCopy.Any(xenConnection => xenConnection.IsConnected 
                && !Helpers.FeatureForbidden(xenConnection, Host.RestrictHealthCheck));
        }
    }
}
