using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Controls.GPU
{
    public partial class GpuRow : UserControl
    {
        public GpuRow()
        {
            InitializeComponent();
        }

        public GpuRow(Host host, PGPU pGpu)
            : this()
        {
            this.host = host;
            this.pGpu = pGpu;
        }

        private Host host;
        private PGPU pGpu
        {
            set
            {
                pGPULabel.Text = value.Name;
                RepopulateAllowedTypes(value);
                // Initialize the shiny bar
                gpuShinyBar1.Initialize(host, value);
            }
        }

        private void RepopulateAllowedTypes(PGPU pGpu)
        {
            dataGridViewEx1.SuspendLayout();
            dataGridViewEx1.Rows.Clear();
            dataGridViewEx1.Cursor = Cursors.WaitCursor;
            try
            {
                dataGridViewEx1.Rows.Clear();
                if (pGpu.supported_VGPU_types != null && pGpu.supported_VGPU_types.Count > 0)
                {
                    dataGridViewEx1.Rows.AddRange((from vGpuTypeRef in pGpu.supported_VGPU_types
                                                   let vGpuType = pGpu.Connection.Resolve(vGpuTypeRef)
                                                   let enabledType = pGpu.enabled_VGPU_types.Contains(vGpuTypeRef)
                                                   select new VGpuTypeRow(vGpuType, enabledType)).ToArray());
                }
            }
            finally
            {
                dataGridViewEx1.ResumeLayout();
                dataGridViewEx1.Cursor = Cursors.Default;
            }
        }
    }

    class VGpuTypeRow : DataGridViewExRow
    {
        public VGPU_type VGpuType;
        public bool EnabledType;

        private readonly DataGridViewExImageCell ImageCell = new DataGridViewExImageCell();
        private readonly DataGridViewTextBoxCell NameCell = new DataGridViewTextBoxCell();

        public VGpuTypeRow(VGPU_type vGpuType, bool enabledType)
        {
            VGpuType = vGpuType;
            EnabledType = enabledType;

            Cells.AddRange(ImageCell,
                           NameCell);

            UpdateDetails();
        }

        public void UpdateDetails()
        {
            ImageCell.Value = EnabledType ? Resources._000_Tick_h32bit_16 : Resources._000_Abort_h32bit_16;
            NameCell.Value = VGpuType.Name;
        }
    }

}
