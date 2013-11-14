using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Controls
{
    public partial class GpuPlacementPolicyPanel : UserControl
    {
        public GpuPlacementPolicyPanel()
        {
            InitializeComponent();
            GPU_group_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(GPU_group_CollectionChanged);
        }

        IXenObject xenObject;

        private readonly CollectionChangeEventHandler GPU_group_CollectionChangedWithInvoke;

        public IXenObject XenObject
        {
            set
            {
                System.Diagnostics.Trace.Assert(value is Pool || value is Host);
                xenObject = value;

                RegisterHandlers();

                PopulatePage();

                Refresh();
            }
        }

        private void PopulatePage()
        {
            var currentAllocationAlgorithm = allocation_algorithm.unknown;

            foreach (GPU_group gpu_group in xenObject.Connection.Cache.GPU_groups)
            {
                if (gpu_group.allocation_algorithm != currentAllocationAlgorithm && currentAllocationAlgorithm != allocation_algorithm.unknown)
                {
                    // a mixture of allocation algorithms
                    currentAllocationAlgorithm = allocation_algorithm.unknown;
                    break;
                }
                currentAllocationAlgorithm = gpu_group.allocation_algorithm;
            }
            placementPolicyLabel.Text = string.Format("Placement policy: {0}",
                                                      PlacementPolicyWrapper.ToString(currentAllocationAlgorithm));
        }


        private void GPU_group_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    RegisterGpuGroupHandlers((GPU_group) e.Element);
                    break;
                case CollectionChangeAction.Remove:
                    UnregisterGpuGroupHandlers((GPU_group) e.Element);
                    break;
            }
            PopulatePage();
        }

        private void gpu_group_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "allocation_algorithm")
            {
                PopulatePage();
            }
        }

        private void RegisterGpuGroupHandlers(GPU_group gpu_group)
        {
            gpu_group.PropertyChanged -= gpu_group_PropertyChanged;
            gpu_group.PropertyChanged += gpu_group_PropertyChanged;
        }

        private void UnregisterGpuGroupHandlers(GPU_group gpu_group)
        {
            gpu_group.PropertyChanged -= gpu_group_PropertyChanged;
        }

        private void RegisterHandlers()
        {
            if (xenObject == null)
                return;
            
            xenObject.Connection.Cache.DeregisterCollectionChanged<GPU_group>(GPU_group_CollectionChangedWithInvoke);
            xenObject.Connection.Cache.RegisterCollectionChanged<GPU_group>(GPU_group_CollectionChangedWithInvoke);

            foreach (GPU_group gpu_group in xenObject.Connection.Cache.GPU_groups)
            {
                UnregisterGpuGroupHandlers(gpu_group); 
                RegisterGpuGroupHandlers(gpu_group);
            }
        }

        private void UnregisterHandlers()
        {
            if (xenObject == null)
                return;

            xenObject.Connection.Cache.DeregisterCollectionChanged<GPU_group>(GPU_group_CollectionChangedWithInvoke);
            
            foreach (GPU_group gpu_group in xenObject.Connection.Cache.GPU_groups)
                UnregisterGpuGroupHandlers(gpu_group);
        }

        private void editPlacementPolicyButton_Click(object sender, System.EventArgs e)
        {
            using (PropertiesDialog propertiesDialog = new PropertiesDialog(xenObject))
            {
                propertiesDialog.SelectPage(propertiesDialog.PoolGpuEditPage);
                propertiesDialog.ShowDialog(this);
            }
        }

        private void GpuPlacementPolicyPanel_VisibleChanged(object sender, System.EventArgs e)
        {
            if (Visible)
                RegisterHandlers();
            else
                UnregisterHandlers();
        }
    }

    public static class PlacementPolicyWrapper
    {
        public static string ToString(allocation_algorithm x)
        {
            switch (x)
            {
                case allocation_algorithm.breadth_first:
                    return "Maximum performance: put VMs on as many GPUs as possible";
                case allocation_algorithm.depth_first:
                    return "Maximum density: put as many VMs as possible on the same GPU";
                default:
                    return "Unknown or mixture";
            }
        }
    }
}
