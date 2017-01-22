/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls.Ballooning;
using XenAdmin.Core;
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
        private long maxCapacity;

        public void Initialize(PGPU pGPU)
        {
            this.PGPU = pGPU;

            vGPUs = pGPU.Connection.ResolveAll(pGPU.resident_VGPUs);

            vms = new Dictionary<VGPU, VM>();
            foreach (VGPU vgpu in vGPUs)
                vms[vgpu] = vgpu.Connection.Resolve(vgpu.VM);

            maxCapacity = 1;
            if (!Helpers.FeatureForbidden(pGPU, Host.RestrictVgpu) && pGPU.HasVGpu && pGPU.supported_VGPU_max_capacities != null)
            {
                foreach (var n in pGPU.supported_VGPU_max_capacities.Values)
                {
                    if (n > maxCapacity)
                        maxCapacity = n;
                }
            }

            capacity = vGPUs.Count > 0 && pGPU.supported_VGPU_max_capacities.ContainsKey(vGPUs[0].type)
                ? pGPU.supported_VGPU_max_capacities[vGPUs[0].type] : maxCapacity;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (PGPU == null || vGPUs == null)
                return;

            Graphics g = e.Graphics;
            Rectangle barArea = barRect;

            // Grid
            if (maxCapacity > 1)
                DrawGrid(g, barArea, barArea.Width);

            double left = barArea.Left;

            // A bar for each vGPU
            int i = 0;
            vGPUs.Sort();

            long segmentLength = barArea.Width / (capacity > 0 ? capacity : maxCapacity);
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

            long incr = max / (capacity > 0 ? capacity : maxCapacity);

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

        protected override Rectangle barRect
        {
            get
            {
                return new Rectangle(10, 20, this.Width - 25, barHeight);
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
