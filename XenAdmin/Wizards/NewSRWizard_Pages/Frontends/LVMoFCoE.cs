using XenAPI;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class LVMoFCoE : LVMoHBA
    {
        public LVMoFCoE()
        {
            InitializeComponent();
        }

        #region LVMoHBA overrides

        public override SR.SRTypes SrType { get { return SR.SRTypes.lvmofcoe; } }
        
        #endregion
    }
}
