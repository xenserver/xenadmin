using System;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    public class WlbRetrieveVmRecommendationsAction: PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<VM> vms;
        private readonly Dictionary<VM, Dictionary<XenRef<Host>, string[]>> recommendations = new Dictionary<VM, Dictionary<XenRef<Host>, string[]>>();
        private bool isError;

        public WlbRetrieveVmRecommendationsAction(IXenConnection connection, List<VM> vms)
            : base(connection, Messages.WLB_RETRIEVING_VM_RECOMMENDATIONS, true)
        {
            this.vms = vms;
        }

        protected override void Run()
        {
            if (vms.Count == 0)
            {
                isError = true;
                return;
            }

            if (Helpers.WlbEnabled(vms[0].Connection))
            {
                try
                {
                    foreach (var vm in vms)
                    {
                        recommendations[vm] = VM.retrieve_wlb_recommendations(Session, vm.opaque_ref);
                    }
                }
                catch (Exception e)
                {
                    log.Error("Error getting WLB recommendations", e);
                    isError = true;
                }
            }
            else
            {
                isError = true;
            }
        }

        public Dictionary<VM, Dictionary<XenRef<Host>, string[]>> Recommendations
        {
            get { return isError ? null : recommendations; }
        }
    }
}
