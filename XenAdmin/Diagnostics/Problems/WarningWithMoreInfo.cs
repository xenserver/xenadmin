using System.Drawing;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;

namespace XenAdmin.Diagnostics.Problems
{
    public abstract class WarningWithMoreInfo : Warning
    {
        protected WarningWithMoreInfo(Check check) : base(check)
        {
        }
        public override bool IsFixable => false;

        public override string HelpMessage => Messages.PATCHINGWIZARD_MORE_INFO;

        protected override Actions.AsyncAction CreateAction(out bool cancelled)
        {
            Program.Invoke(Program.MainWindow, delegate ()
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Message)))
                {
                    dlg.ShowDialog();
                }
            });

            cancelled = true;
            return null;
        }

        public abstract string Message { get; }
    }
}
