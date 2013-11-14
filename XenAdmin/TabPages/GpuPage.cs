using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls.GPU;
using XenAPI;

namespace XenAdmin.TabPages
{
    public partial class GpuPage : BaseTabPage
    {
        private const int ROW_GAP = 5;

        public GpuPage()
        {
            InitializeComponent();
            Text = Messages.GPU;
            PGPU_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(PGPU_CollectionChanged);
        }


        private readonly CollectionChangeEventHandler PGPU_CollectionChangedWithInvoke;

        private IXenObject xenObject;
        List<PGPU> pGPUs = new List<PGPU>();

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

        private bool _rebuilding;

        private void Rebuild()
        {
            Program.AssertOnEventThread();
            if (!Visible)
                return;
            _rebuilding = true;
            pageContainerPanel.SuspendLayout();

            // Store a list of the current controls. We remove them at the end because it makes less flicker that way.
            List<Control> oldControls = new List<Control>(pageContainerPanel.Controls.Count);
            foreach (Control c in pageContainerPanel.Controls)
            {
                oldControls.Add(c);
            }


            // Group pGPUs with the same settings
            Dictionary<GpuSettings, List<PGPU>> settingsToPGPUs = new Dictionary<GpuSettings, List<PGPU>>();  // all PGPUs with a particular setting
            List<GpuSettings> listSettings = new List<GpuSettings>();  // also make a list of GpuSettings to preserve the order

            pGPUs.Clear();

            var allPgpus = xenObject.Connection.Cache.PGPUs;
            pGPUs.AddRange(from pGpu in allPgpus
                           let host = xenObject.Connection.Resolve(pGpu.host)
                           orderby host, pGpu.Name ascending
                           select pGpu
                );

            foreach (PGPU pGpu in pGPUs)
            {
                RegisterPgpuHandlers(pGpu);

                var enabledTypes = pGpu.Connection.ResolveAll(pGpu.enabled_VGPU_types);

                if (enabledTypes.Count > 1)
                {
                    enabledTypes.Sort((t1, t2) =>
                                          {
                                              int result = t1.Capacity.CompareTo(t2.Capacity);
                                              return result != 0 ? result : t1.Name.CompareTo(t2.Name);
                                          });
                }

                var newSettings = new GpuSettings(enabledTypes.ToArray());
                
                var existingSettings = settingsToPGPUs.Keys.FirstOrDefault(ss => ss.Equals(newSettings));

                if (existingSettings == null) // we've not seen these settings on another pGPU
                {
                    settingsToPGPUs.Add(newSettings, new List<PGPU>());
                    listSettings.Add(newSettings);
                    existingSettings = newSettings;
                }
                settingsToPGPUs[existingSettings].Add(pGpu);
            }

            int initScroll = pageContainerPanel.VerticalScroll.Value;
            int top = pageContainerPanel.Padding.Top - initScroll;

            foreach (GpuSettings settings in listSettings)
            {
                AddRowToPanel(new GpuRow(xenObject.Connection, settingsToPGPUs[settings]), ref top);
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

        private GpuRow FindRow(PGPU pgpu)
        {
            return pgpu != null ? pageContainerPanel.Controls.OfType<GpuRow>().FirstOrDefault(row => row.PGPUs.Contains(pgpu)) : null;
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

        private void PGPU_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                PGPU pgpu = e.Element as PGPU;
                UnregisterPgpuHandlers(pgpu);
            }
            XenObject = xenObject;
        }

        private void pgpu_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "resident_VGPUs")
            {
                var pgpu = sender as PGPU;
                var gpuRow = FindRow(pgpu);
                if (gpuRow != null) 
                    gpuRow.RefreshGpu(pgpu);
            }

            if (e.PropertyName == "enabled_VGPU_types")
            {
                Rebuild();
            }

        }
        private void RegisterPgpuHandlers(PGPU pgpu)
        {
            pgpu.PropertyChanged -= pgpu_PropertyChanged;
            pgpu.PropertyChanged += pgpu_PropertyChanged;
        }

        private void UnregisterPgpuHandlers(PGPU pgpu)
        {
            pgpu.PropertyChanged -= pgpu_PropertyChanged;
        }

        private void RegisterHandlers()
        {
            if (xenObject == null)
                return;

            xenObject.Connection.Cache.DeregisterCollectionChanged<PGPU>(PGPU_CollectionChangedWithInvoke);
            xenObject.Connection.Cache.RegisterCollectionChanged<PGPU>(PGPU_CollectionChangedWithInvoke);

            foreach (PGPU pgpu in xenObject.Connection.Cache.PGPUs)
            {
                UnregisterPgpuHandlers(pgpu);
                RegisterPgpuHandlers(pgpu);
            }
        }

        private void GpuPage_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                RegisterHandlers();
            else
                UnregisterHandlers();
        }

        private void UnregisterHandlers()
        {
            if (xenObject == null)
                return;

            xenObject.Connection.Cache.DeregisterCollectionChanged<PGPU>(PGPU_CollectionChangedWithInvoke);

            foreach (PGPU pgpu in xenObject.Connection.Cache.PGPUs)
            {
                UnregisterPgpuHandlers(pgpu);
            }
        }


        internal class GpuSettings : IEquatable<GpuSettings>
        {
            public readonly VGPU_type[] EnabledVgpuTypes;

            public GpuSettings(VGPU_type[] vgpuTypes)
            {
                EnabledVgpuTypes = vgpuTypes;
            }

            public bool Equals(GpuSettings other)
            {
                if ((EnabledVgpuTypes == null || EnabledVgpuTypes.Length == 0) &&
                    (other.EnabledVgpuTypes == null || other.EnabledVgpuTypes.Length == 0))
                    return true;

                if ((EnabledVgpuTypes == null || EnabledVgpuTypes.Length == 0) ||
                    (other.EnabledVgpuTypes == null || other.EnabledVgpuTypes.Length == 0))
                    return false;

                if (EnabledVgpuTypes.Length != other.EnabledVgpuTypes.Length)
                    return false;

                for (int i = 0; i < EnabledVgpuTypes.Length; i++)
                {
                    if (!EnabledVgpuTypes[i].Equals(other.EnabledVgpuTypes[i]))
                        return false;
                }

                return true;
            }

            public override string ToString()
            {
                return string.Join(",", EnabledVgpuTypes.Select(t => t.model_name).ToArray());
            }

        }
    }
}
