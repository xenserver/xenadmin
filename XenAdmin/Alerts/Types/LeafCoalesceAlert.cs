using XenAPI;
using static XenAPI.Message;

namespace XenAdmin.Alerts
{
    public class LeafCoalesceAlert : MessageAlert
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MessageType LeafType;
        private string vmID;

        public LeafCoalesceAlert(Message msg)
            : base(msg)
        {
            LeafType = msg.Type;
            MapVdiToVm(msg);
        }

        private void MapVdiToVm(Message msg)
        {            
            var obj = msg.GetXenObject();
            
            if (obj is VDI vdi && vdi.Connection != null)
            {
                foreach (VBD vbd in vdi.Connection.ResolveAll(vdi.VBDs))
                {                   
                    VM vm = vbd.Connection.Resolve(vbd.VM);
                    // look for if VBD is attached, only 1 can be attached at a time
                    if (vbd.currently_attached && vm != null) 
                    {
                        vmID = vm.uuid;
                    }
                }
            }
        }

        public override string Description
        {
            get
            {
                switch (LeafType)
                {
                    case MessageType.LEAF_COALESCE_START_MESSAGE:
                        return string.Format(Messages.LEAF_COALESCE_START_DESCRIPTION, vmID);
                    case MessageType.LEAF_COALESCE_COMPLETED:
                        return string.Format(Messages.LEAF_COALESCE_COMPLETED_DESCRIPTION, vmID);
                    case MessageType.LEAF_COALESCE_FAILED:
                        return string.Format(Messages.LEAF_COALESCE_FAILED_DESCRIPTION, vmID);
                    default:
                        return base.Description;
                }
            }
        }

        public override string Title
        {
            get
            {
                switch (LeafType)
                {
                    case MessageType.LEAF_COALESCE_START_MESSAGE:
                        return Messages.LEAF_COALESCE_START_TITLE;
                    case MessageType.LEAF_COALESCE_COMPLETED:
                        return Messages.LEAF_COALESCE_COMPLETED_TITLE;
                    case MessageType.LEAF_COALESCE_FAILED:
                        return Messages.LEAF_COALESCE_FAILED_TITLE;
                    default:
                        return base.Title;
                }
            }
        }

        public override string HelpID
        {
            get
            {
                return string.Format("{0}UsageMessageAlert", LeafType);
            }
        }
    }
}
