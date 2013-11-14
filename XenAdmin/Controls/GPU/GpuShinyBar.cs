using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls.Ballooning;
using XenAPI;

namespace XenAdmin.Controls.GPU
{
    public partial class GpuShinyBar : ShinyBar
    {
        public GpuShinyBar()
        {
            InitializeComponent();
        }

        public PGPU PGPU { get; private set; }

        private List<VGPU> vGPUs;
        private Dictionary<VGPU, VM> vms;
        private long capacity;

        public void Initialize(PGPU pGPU)
        {
            this.PGPU = pGPU;

            vGPUs = pGPU.Connection.ResolveAll(pGPU.resident_VGPUs);

            vms = new Dictionary<VGPU, VM>();
            foreach (VGPU vgpu in vGPUs)
                vms[vgpu] = vgpu.Connection.Resolve(vgpu.VM);

            capacity = vGPUs.Count > 0 ? pGPU.supported_VGPU_max_capacities[vGPUs[0].type] : 8;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (PGPU == null || vGPUs == null)
                return;

            Graphics g = e.Graphics;
            Rectangle barArea = barRect;

            // Grid
            DrawGrid(g, barArea, barArea.Width);

            double left = barArea.Left;

            // A bar for each vGPU
            int i = 0;
            vGPUs.Sort();

            long segmentLength = barArea.Width / (capacity > 0 ? capacity : 8);
            foreach (VGPU vgpu in vGPUs)
            {
                VM vm = vms[vgpu];
                if (vm != null)
                {
                    var vGpuType = PGPU.Connection.Resolve(vgpu.type);

                    DrawSegment(g, segmentLength, vm.Name, vGpuType != null ? vGpuType.model_name : "",
                        GpuShinyBarColors.GpuShinyBar_VMs[i++ % GpuShinyBarColors.GpuShinyBar_VMs.Length],
                        ref left);
                }
            }

            // One final bar for free space
            Rectangle rectFree = new Rectangle((int)left, barArea.Top, barArea.Right - (int)left, barArea.Height);
            DrawToTarget(g, barArea, rectFree, GpuShinyBarColors.GpuShinyBar_Unused);
        }

        private void DrawGrid(Graphics g, Rectangle barArea, long max)
        {
            const int line_height = 12;

            int line_bottom = barArea.Top + barArea.Height / 2;
            int line_top = barArea.Top - line_height;

            long incr = max / (capacity > 0 ? capacity : 8);

            // Draw the grid
            using (Pen pen = new Pen(GpuShinyBarColors.Grid))
            {
                for (long x = 0; x <= max; x += incr)
                {
                    // Tick
                    int pos = barArea.Left + (int)((double)x);
                    g.DrawLine(pen, pos, line_top, pos, line_bottom);
                }
            }
        }

        private void DrawSegment(Graphics g, long width, string name, string description, Color color, ref double left)
        {
            if (width < 0)
                return;

            var rect = new Rectangle((int)left, barRect.Top,
                (int)(left + width) - (int)left,  // this is not necessarily the same as (int)width, which can leave a 1 pixel gap
                barRect.Height);
            var caption = name + "\n" + description;
            DrawToTarget(g, barRect, rect, color, caption, GpuShinyBarColors.GpuShinyBar_Text, HorizontalAlignment.Center, caption);
            left += width;
        }

        protected override int barHeight
        {
            get
            {
                return 40;
            }
        }
    }

    public static class GpuShinyBarColors
    {
        public static Color[] GpuShinyBar_VMs = { Color.FromArgb(83, 39, 139), Color.FromArgb(157, 115, 215) };
        public static Color GpuShinyBar_Unused = Color.Black;
        public static Color GpuShinyBar_Text = Color.White;

        public static Color Grid = Color.DarkGray;
    }
}
