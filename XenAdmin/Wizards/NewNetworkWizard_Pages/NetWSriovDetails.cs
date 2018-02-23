using System;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Network;

namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWSriovDetails : XenTabPage
    {
        internal Host Host;

        public NetWSriovDetails()
        {
            InitializeComponent();
        }
        public override string Text { get { return Messages.NETW_DETAILS_TEXT; } }

        public override string PageTitle
        {
            get
            {
                return Messages.NETW_INTERNAL_DETAILS_TITLE;
            }
        }
        public override void PopulatePage()
        {
            PopulateHostNicList(Host, Connection);
        }
        private void PopulateHostNicList(Host host, IXenConnection conn)
        {
            comboBoxNicList.Items.Clear();
            foreach (PIF thePIF in conn.Cache.PIFs)
            {
                if (thePIF.host.opaque_ref == host.opaque_ref && thePIF.IsPhysical() && !thePIF.IsBondNIC() && thePIF.SriovCapable())
                {
                    comboBoxNicList.Items.Add(thePIF);
                }
            }
            if (comboBoxNicList.Items.Count > 0)
                comboBoxNicList.SelectedIndex = 0;
        }

        public XenAPI.PIF SelectedHostNic
        {
            get { return (XenAPI.PIF)comboBoxNicList.SelectedItem; }
        }

        public bool isAutomaticAddNicToVM
        {
            get { return cbxAutomatic.Checked; }
        }
    }
}
