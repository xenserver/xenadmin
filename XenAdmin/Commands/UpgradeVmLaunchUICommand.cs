using System.Linq;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
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
            IXenConnection connection = null;

            if (selection != null && selection.Count == 1)
            {
                if (selection[0].XenObject is Pool || selection[0].XenObject is Host) 
                    connection = selection[0].XenObject.Connection;
            }

            return connection != null && connection.IsConnected && Helpers.DundeeOrGreater(connection);
        }
    }
}
