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

        public override bool ShowNicColumn { get { return true; } }

        public override LvmOhbaSrDescriptor CreateSrDescriptor(FibreChannelDevice device)
        {
            return new FcoeSrDescriptor(device);
        }

        #endregion
    }
}
