using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class AddVGPUDialog : XenDialogBase
    {
        private VM _vm;
        private GPU_group[] gpu_groups;

        public AddVGPUDialog(VM vm)
        {
            _vm = vm;
            InitializeComponent();
            BuildList();
        }

        private void BuildList()
        {
            gpu_groups = _vm.Connection.Cache.GPU_groups.Where(g => g.PGPUs.Count > 0 && g.supported_VGPU_types.Count != 0).ToArray();
            PopulateComboBox();
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is VM);  // only VMs should show this page
            Trace.Assert(!Helpers.FeatureForbidden(clone, Host.RestrictGpu));  // If license insufficient, we show upsell page instead

            _vm = (VM)clone;

            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            var noneItem = new GpuTuple(null, null, null); // "None" item
            comboBoxTypes.Items.Add(noneItem);

            Array.Sort(gpu_groups);
            foreach (GPU_group gpu_group in gpu_groups)
            {
                if (Helpers.FeatureForbidden(_vm.Connection, Host.RestrictVgpu) || !_vm.CanHaveVGpu())
                {
                    if (gpu_group.HasPassthrough())
                        comboBoxTypes.Items.Add(new GpuTuple(gpu_group, null, null));  //GPU pass-through item
                }
                else
                {
                    var enabledTypes = _vm.Connection.ResolveAll(gpu_group.enabled_VGPU_types);
                    var allTypes = _vm.Connection.ResolveAll(gpu_group.supported_VGPU_types);

                    var disabledTypes = allTypes.FindAll(t => !enabledTypes.Exists(e => e.opaque_ref == t.opaque_ref));

                    if (gpu_group.HasVGpu())
                    {
                        allTypes.Sort();
                        allTypes.Reverse();
                        comboBoxTypes.Items.Add(new GpuTuple(gpu_group, allTypes.ToArray())); // Group item
                    }

                    foreach (var vgpuType in allTypes)
                        comboBoxTypes.Items.Add(new GpuTuple(gpu_group, vgpuType, disabledTypes.ToArray())); // GPU_type item

                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
