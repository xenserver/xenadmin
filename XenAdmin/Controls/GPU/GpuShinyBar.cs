using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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

        private PGPU pGPU;

        private List<VGPU> vGPUs;
        Dictionary<VGPU, VM> vms;
        private long capacity;

        private Host host;
        private Host_metrics host_metrics;

        public void Initialize(Host host, PGPU pGPU)
        {
            this.pGPU = pGPU;
            this.host = host;
            this.host_metrics = host.Connection.Resolve(host.metrics);

            vGPUs = pGPU.Connection.ResolveAll(pGPU.resident_VGPUs);

            var capacities = pGPU.supported_VGPU_max_capacities;
            capacity = capacities.Sum(pair => pair.Value);

            vms = new Dictionary<VGPU, VM>();
            foreach (VGPU vgpu in vGPUs)
                vms[vgpu] = vgpu.Connection.Resolve(vgpu.VM);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (host == null || pGPU == null || vGPUs == null/* || capacity == 0*/)
                return;

            Graphics g = e.Graphics;
            Rectangle barArea = barRect;
            double bytesPerPixel = (double)host_metrics.memory_total / (double)barArea.Width;

            // Grid
            //DrawGrid(g, barArea, bytesPerPixel, host_metrics.memory_total);

            double left = (double)barArea.Left;

            // A bar for each vGPU
            int i = 0;
            vGPUs.Sort();
            long length = host_metrics.memory_total / 4;
            foreach (VGPU vgpu in vGPUs)
            {
                VM vm = vms[vgpu];
                if (vm != null)
                {
                    DrawSegment(g, length, bytesPerPixel, vm.Name, vm,
                        BallooningColors.HostShinyBar_VMs[i++ % BallooningColors.HostShinyBar_VMs.Length],
                        ref left);
                }
            }

            // One final bar for free space
            Rectangle rectFree = new Rectangle((int)left, barArea.Top, barArea.Right - (int)left, barArea.Height);
            DrawToTarget(g, barArea, rectFree, BallooningColors.HostShinyBar_Unused);
        }

        private void DrawSegment(Graphics g, long mem, double bytesPerPixel, string name, VM vm, Color color, ref double left)
        {
            // This should never happen, but actually does happen in older server dbs because
            // we didn't used to have the amount of free memory accurately (see CA-31223).
            if (mem < 0)
                return;

            Rectangle barArea = barRect;
            double width = mem / bytesPerPixel;
            Rectangle rect = new Rectangle((int)left, barArea.Top,
                (int)(left + width) - (int)left,  // this is not necessarily the same as (int)width, which can leave a 1 pixel gap
                barArea.Height);
            string bytesString = Util.SuperiorSizeString(mem, 0);
            string caption = name + "\n" + bytesString;
            string toolTip = name + "\n" + string.Format(Messages.CURRENT_MEMORY_USAGE, bytesString);
            if (vm != null && vm.has_ballooning)
            {
                if (vm.memory_dynamic_max == vm.memory_static_max)
                    toolTip += string.Format("\n{0}: {1}\n{2}: {3}",
                                             Messages.DYNAMIC_MIN, Util.SuperiorSizeString(vm.memory_dynamic_min, 0),
                                             Messages.DYNAMIC_MAX, Util.SuperiorSizeString(vm.memory_dynamic_max, 0));
                else
                    toolTip += string.Format("\n{0}: {1}\n{2}: {3}\n{4}: {5}",
                                             Messages.DYNAMIC_MIN, Util.SuperiorSizeString(vm.memory_dynamic_min, 0),
                                             Messages.DYNAMIC_MAX, Util.SuperiorSizeString(vm.memory_dynamic_max, 0),
                                             Messages.STATIC_MAX, Util.SuperiorSizeString(vm.memory_static_max, 0));
            }
            DrawToTarget(g, barArea, rect, color, caption, BallooningColors.HostShinyBar_Text, HorizontalAlignment.Center, toolTip);
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
}
