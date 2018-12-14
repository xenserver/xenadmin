using XenAdmin.Controls;

namespace XenAdmin.Wizards.ImportWizard
{
    public partial class ImportBootOptionPage : XenTabPage
    {
        public ImportBootOptionPage()
        {
            InitializeComponent();
        }

        #region Base class (XenTabPage) overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.IMPORT_SELECT_BOOT_OPTIONS_PAGE_TITLE; } }

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text { get { return Messages.IMPORT_SELECT_BOOT_OPTIONS_PAGE_TEXT; } }

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID { get { return "VMConfig"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {

        }

        public override void PopulatePage()
        {
            bootModesControl1.CheckBIOSBootMode();
        }

        #endregion

        #region Accessors
		
        public Actions.VMActions.BootMode SelectedBootMode { get { return bootModesControl1.SelectedOption; } }
		
        #endregion
    }
}
