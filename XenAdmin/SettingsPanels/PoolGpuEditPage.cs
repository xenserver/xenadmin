using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class PoolGpuEditPage : UserControl, IEditPage
    {
        public Pool pool;
        private GPU_group[] gpu_groups;
        private allocation_algorithm currentAllocationAlgorithm;

        public PoolGpuEditPage()
        {
            InitializeComponent();
        }

        public allocation_algorithm AllocationAlgorithm
        {
            get
            {
                return radioButtonDepth.Checked ?
                    allocation_algorithm.depth_first :
                    radioButtonBreadth.Checked ? allocation_algorithm.breadth_first : allocation_algorithm.unknown;
            }
        }

        #region Implementation of VerticalTab

        public override string Text
        {
            get { return Messages.GPU; }
        }

        public string SubText
        {
            get 
            { 
                return radioButtonDepth.Checked ? Messages.GPU_PLACEMENT_POLICY_MAX_DENSITY : 
                    radioButtonBreadth.Checked ? Messages.GPU_PLACEMENT_POLICY_MAX_PERFORMANCE : Messages.GPU_PLACEMENT_POLICY_MIXED; }
        }

        public Image Image
        {
            get { return Resources._000_GetMemoryInfo_h32bit_16; }
        }

        #endregion

        #region Implementation of IEditPage

        public AsyncAction SaveSettings()
        {
            return new SetGpuPlacementPolicyAction(pool, AllocationAlgorithm);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is Pool || clone is Host);  // only Pools and Hosts should show this page

            pool = Helpers.GetPoolOfOne(clone.Connection);
            gpu_groups = pool.Connection.Cache.GPU_groups;
            PopulatePage();
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
            
        }

        public void Cleanup()
        {
            
        }

        public bool HasChanged
        {
            get
            {
                var allocationAlgorithm = radioButtonDepth.Checked ? 
                    allocation_algorithm.depth_first : 
                    radioButtonBreadth.Checked ? allocation_algorithm.breadth_first : allocation_algorithm.unknown;

                return !allocationAlgorithm.Equals(currentAllocationAlgorithm);
            }
        }

        #endregion

        private void PopulatePage()
        {
            currentAllocationAlgorithm = allocation_algorithm.unknown;

            foreach (GPU_group gpu_group in gpu_groups)
            {
                if (gpu_group.allocation_algorithm != currentAllocationAlgorithm && currentAllocationAlgorithm != allocation_algorithm.unknown)
                {
                    // a mixture of allocation algorithms
                    currentAllocationAlgorithm = allocation_algorithm.unknown;
                    break;
                }
                currentAllocationAlgorithm = gpu_group.allocation_algorithm;
            }

            radioButtonDepth.Checked = currentAllocationAlgorithm == allocation_algorithm.depth_first;
            radioButtonBreadth.Checked = currentAllocationAlgorithm == allocation_algorithm.breadth_first;

            radioButtonMixture.Checked = currentAllocationAlgorithm == allocation_algorithm.unknown;
            radioButtonMixture.Visible = currentAllocationAlgorithm == allocation_algorithm.unknown;
            radioButtonMixture.Enabled = currentAllocationAlgorithm == allocation_algorithm.unknown;
        }
    }
}
