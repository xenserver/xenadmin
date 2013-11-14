using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XenAdmin.Controls.GPU;
using XenAPI;

namespace XenAdmin.TabPages
{
    public partial class GpuPage : BaseTabPage
    {
        private const int ROW_GAP = 10;

        public GpuPage()
        {
            InitializeComponent();
            Text = Messages.GPU;
        }

        private IXenObject xenObject;

        /// <summary>
        /// The object that the panel is displaying GPU info for. 
        /// </summary>
        public IXenObject XenObject
        {
            set
            {
                System.Diagnostics.Trace.Assert(value is Pool || value is Host);
                xenObject = value;

                gpuPlacementPolicyPanel1.XenObject = value;

                Rebuild();
            }
        }

        private bool _rebuild_needed;
        private bool _rebuilding = false;

        private void Rebuild()
        {
            Program.AssertOnEventThread();
            _rebuild_needed = false;
            if (!this.Visible)
                return;
            _rebuilding = true;
            pageContainerPanel.SuspendLayout();

            // Store a list of the current controls. We remove them at the end because it makes less flicker that way.
            List<Control> oldControls = new List<Control>(pageContainerPanel.Controls.Count);
            foreach (Control c in pageContainerPanel.Controls)
            {
                oldControls.Add(c);
            }

            int initScroll = pageContainerPanel.VerticalScroll.Value;
            int top = pageContainerPanel.Padding.Top - initScroll;

            foreach (Host host in xenObject.Connection.Cache.Hosts)
            {
                foreach (var pGpu in host.Connection.ResolveAll(host.PGPUs))
                {
                    AddRowToPanel(new GpuRow(host, pGpu), ref top);
                }
            }
            
            // Remove old controls
            foreach (Control c in oldControls)
            {
                pageContainerPanel.Controls.Remove(c);
                int scroll = initScroll;
                if (scroll > pageContainerPanel.VerticalScroll.Maximum)
                    scroll = pageContainerPanel.VerticalScroll.Maximum;
                pageContainerPanel.VerticalScroll.Value = scroll; 
                c.Dispose();
            }
            _rebuilding = false;
            pageContainerPanel.ResumeLayout();
            ReLayout();
        }

        private void ReLayout()
        {
            Program.AssertOnEventThread();
            if (_rebuilding)
                return;

            int initScroll = pageContainerPanel.VerticalScroll.Value;
            int top = pageContainerPanel.Padding.Top - initScroll;
            foreach (Control row in pageContainerPanel.Controls)
            {
                row.Top = top;
                top += row.Height + ROW_GAP;
            }
        }

        private void AddRowToPanel(UserControl row, ref int top)
        {
            row.Top = top;
            row.Left = pageContainerPanel.Padding.Left - pageContainerPanel.HorizontalScroll.Value;
            SetRowWidth(row);
            row.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            top += row.Height + ROW_GAP;
            row.Resize += row_Resize;
            pageContainerPanel.Controls.Add(row);
        }

        private void pageContainerPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control row in pageContainerPanel.Controls)
                SetRowWidth(row);
        }

        void row_Resize(object sender, EventArgs e)
        {
            ReLayout();
        }

        private void SetRowWidth(Control row)
        {
            row.Width = pageContainerPanel.Width - pageContainerPanel.Padding.Left - 25;  // It won't drop below row.MinimumSize.Width though
        }
    }
}
