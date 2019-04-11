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
        private List<VGPU> existingVGpus;
        public GpuTuple selectedTuple { private set; get; }

        public AddVGPUDialog(VM vm, List<VGPU> VGpus)
        {
            _vm = vm;
            existingVGpus = VGpus;
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

                    List<string> commonTypes = new List<string>();
                    foreach (VGPU_type vgpuType in allTypes)
                        commonTypes.Add(vgpuType.model_name);
                    
                        
                    foreach (VGPU eVgpu in existingVGpus)
                    {
                        VGPU_type etype = _vm.Connection.Resolve(eVgpu.type);
                        List<string> tmpCommonTypes = new List<string>();
                        foreach (string compatible_type in etype.compatible_types_in_vm)
                            if (commonTypes.Contains(compatible_type))
                                tmpCommonTypes.Add(compatible_type);
                        commonTypes = tmpCommonTypes;
                    }
                    foreach (var vgpuType in allTypes)
                        if (commonTypes.Contains(vgpuType.model_name))
                            comboBoxTypes.Items.Add(new GpuTuple(gpu_group, vgpuType, disabledTypes.ToArray())); // GPU_type item

                }
            }
            comboBoxTypes.SelectedItem = noneItem;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            GpuTuple tuple = comboBoxTypes.SelectedItem as GpuTuple;
            if (tuple == null || tuple.VgpuTypes == null || tuple.VgpuTypes.Length == 0)
                selectedTuple = null;
            else
                selectedTuple = tuple;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            
        }
    }
}
