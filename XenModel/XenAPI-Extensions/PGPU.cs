namespace XenAPI
{
    partial class PGPU
    {
        public override string Name
        {
            get
            {
                Host host = Connection.Resolve(this.host);

                PCI pci = Connection.Resolve(PCI);
                var name = pci != null ? pci.device_name : uuid;

                return string.Format("{0}: {1}", host != null ? host.Name : "", name);
            }
        }
    }
}
