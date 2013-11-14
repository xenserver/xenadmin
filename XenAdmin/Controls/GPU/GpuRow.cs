using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Network;
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

        public GpuRow(IXenConnection connection, List<PGPU> pGpuList)
            : this()
        {
            this.connection = connection;
            pGpuLabel.Text = pGpuList[0].Name;
            RepopulateAllowedTypes(pGpuList[0]);
            Rebuild(pGpuList);
            SetupPage();
        }

        private readonly IXenConnection connection;

        private Dictionary<PGPU, CheckBox> pGpus = new Dictionary<PGPU, CheckBox>();

        private void Rebuild(List<PGPU> pGpuList)
        {
            Program.AssertOnEventThread();
            if (!Visible)
                return;

            shinyBarsContainerPanel.SuspendLayout();
            
            // Store a list of the current controls. We remove them at the end because it makes less flicker that way.
            var oldControls = new List<Control>(shinyBarsContainerPanel.Controls.Count);
            oldControls.AddRange(shinyBarsContainerPanel.Controls.Cast<Control>());

            int index = 1;
            XenRef<Host> hostRef = null;
            foreach (PGPU pgpu in pGpuList)
            {
                var host = connection.Resolve(pgpu.host);

                // add host label if needed
                if (hostRef == null || pgpu.host.opaque_ref != hostRef.opaque_ref)
                {
                    AddHostLabel(new Label { Text = String.Format(Messages.GPU_ON_HOST_LABEL, host.Name)}, ref index);
                    hostRef = pgpu.host;
                }

                // add pGPU shiny bar
                var gpuShinyBar = new GpuShinyBar();
                var checkBox = pGpuList.Count > 1 ? new CheckBox() : null; 
                AddShinyBar(gpuShinyBar, checkBox, ref index);
                gpuShinyBar.Initialize(pgpu);

                pGpus.Add(pgpu, checkBox);
            }    
            
            // Remove old controls
            foreach (Control c in oldControls)
            {
                shinyBarsContainerPanel.Controls.Remove(c);
                c.Dispose();
            }
            shinyBarsContainerPanel.ResumeLayout();
            ReLayout();
        }

        private void AddShinyBar(UserControl shinyBar, CheckBox checkBox, ref int index)
        {
            if (checkBox != null)
            {
                shinyBarsContainerPanel.Controls.Add(checkBox, 0, index);
                checkBox.Dock = DockStyle.Fill;
                checkBox.Margin = new Padding(3, 30, 0, 0);
                checkBox.CheckedChanged += CheckedChanged;
                checkBox.Checked = true;
            }

            shinyBarsContainerPanel.Controls.Add(shinyBar, 1, index);
            shinyBar.Dock = DockStyle.Fill;

            index++;
        }

        private void AddHostLabel(Label label, ref int index)
        {
            shinyBarsContainerPanel.Controls.Add(label, 0, index);
            shinyBarsContainerPanel.SetColumnSpan(label, 2); 
            label.Dock = DockStyle.Fill;
            index++;
        }

        private void ReLayout()
        {
            Program.AssertOnEventThread();
            Height = tableLayoutPanel1.Height + 2;
        }

        private void SetupPage()
        {
            multipleSelectionPanel.Visible = (pGpus.Count > 1);
            editButton.Visible = !multipleSelectionPanel.Visible;
        }

        private GpuShinyBar FindGpuShinyBar(PGPU pGpu)
        {
            return pGpu != null
                       ? shinyBarsContainerPanel.Controls.OfType<GpuShinyBar>().FirstOrDefault(
                           shinyBar => shinyBar.PGPU == pGpu)
                       : null;
        }

        public void RefreshGpu(PGPU pGpu)
        {
            Program.AssertOnEventThread();
            if (!Visible)
                return;

            GpuShinyBar gpuShinyBar = FindGpuShinyBar(pGpu);
            if (gpuShinyBar != null)
            {
                gpuShinyBar.Initialize(pGpu);
                gpuShinyBar.Refresh();
            }
        }

        public IEnumerable<PGPU> PGPUs
        {
            get
            {
                return pGpus.Keys;
            }
        }

        public List<PGPU> SelectedPGPUs
        {
            get
            {
                if (pGpus.Count == 0)
                    return null;

                return pGpus.Count > 1 
                    ? (from kvp in pGpus where kvp.Value != null && kvp.Value.Checked select kvp.Key).ToList() 
                    : new List<PGPU> {pGpus.Keys.ElementAt(0)};
            }
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            foreach (var checkBox in pGpus.Values.Where(checkBox => checkBox != null))
            {
                checkBox.Checked = true;
            }
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            foreach (var checkBox in pGpus.Values.Where(checkBox => checkBox != null))
            {
                checkBox.Checked = false;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            new GpuConfiguration(SelectedPGPUs).ShowDialog(Program.MainWindow);
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            editSelectedGpusButton.Enabled = clearAllButton.Enabled =
                (pGpus.Values.Where(checkBox => checkBox != null).Any(checkBox => checkBox.Checked));
            selectAllButton.Enabled =
                (pGpus.Values.Where(checkBox => checkBox != null).Any(checkBox => !checkBox.Checked));
        }

        #region Allowed vGpu types
        private void RepopulateAllowedTypes(PGPU pGpu)
        {
            allowedTypesGrid.SuspendLayout();
            allowedTypesGrid.Rows.Clear();
            allowedTypesGrid.Cursor = Cursors.WaitCursor;
            try
            {
                allowedTypesGrid.Rows.Clear();
                if (pGpu.supported_VGPU_types != null && pGpu.supported_VGPU_types.Count > 0)
                {
                    allowedTypesGrid.Rows.AddRange((from vGpuTypeRef in pGpu.supported_VGPU_types
                                                    let vGpuType = pGpu.Connection.Resolve(vGpuTypeRef)
                                                    let enabledType = pGpu.enabled_VGPU_types.Contains(vGpuTypeRef)
                                                    orderby vGpuType.Capacity ascending
                                                    select new VGpuTypeRow(vGpuType, enabledType)).ToArray());
                }
            }
            finally
            {
                allowedTypesGrid.ResumeLayout();
                allowedTypesGrid.Cursor = Cursors.Default;
            }
        }
        #endregion
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
