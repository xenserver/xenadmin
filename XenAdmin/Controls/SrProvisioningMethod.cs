using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    public partial class SrProvisioningMethod : UserControl
    {
        public SrProvisioningMethod()
        {
            InitializeComponent();
        }

        internal bool Gfs2
        {
            get { return radioButtonGfs2.Checked; }
        }

        internal bool Lvm
        {
            get { return radioButtonLvm.Checked; }
        }
    }
}
