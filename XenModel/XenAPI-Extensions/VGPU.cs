namespace XenAPI
{
    partial class VGPU
    {
        public bool IsPassthrough
        {
            get
            {
                var vGPUType = Connection.Resolve(type);
                return vGPUType != null && vGPUType.max_heads == 0;
            }
        }
    }
}
