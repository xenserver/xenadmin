namespace XenAPI
{
    partial class PGPU
    {
        public override string Name
        {
            get
            {
                PCI pci = Connection.Resolve(PCI);
                return pci != null ? pci.device_name : uuid;
            }
        }
    }
}
