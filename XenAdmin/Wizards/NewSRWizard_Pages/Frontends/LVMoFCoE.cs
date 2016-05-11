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

        public override SR.SRTypes SrType { get { return srProvisioningMethod.Lvm ? SR.SRTypes.lvmofcoe : SR.SRTypes.gfs2; } }

        public override bool ShowNicColumn { get { return true; } }

        public override string HelpID { get { return "Location_FCOE"; } }

        public override LvmOhbaSrDescriptor CreateSrDescriptor(FibreChannelDevice device)
        {
            if (SrType == SR.SRTypes.gfs2)
                return new Gfs2HbaSrDescriptor(device);
            return new FcoeSrDescriptor(device);
        }

        public override LvmOhbaSrDescriptor CreateLvmSrDescriptor(FibreChannelDevice device)
        {
            return new FcoeSrDescriptor(device);
        }

        #endregion
    }
}
