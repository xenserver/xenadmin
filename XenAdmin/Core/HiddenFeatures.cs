using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenAdmin.Core
{
    public class HiddenFeatures
    {
        internal static bool CPSOptimizationHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(CPS_OPTIMIZATION_HIDDEN); }
        }

        internal static bool RDPPollingHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(RDP_POLLING_HIDDEN); }
        }

        internal static bool LearnMoreButtonHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(LEARN_MORE_HIDDEN); }
        }

        internal static bool LinkLabelHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(LINK_LABEL_HIDDEN); }
        }

        internal static bool ToolStripMenuItemHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(TOOL_STRIP_MENU_ITEM_HIDDEN); }
        }

        internal static bool CrossServerPrivateNetworkHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(CROSS_SERVER_PRIVATE_NETWORK_HIDDEN); }
        }

        internal static bool CopyrightHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(COPYRIGHT_HIDDEN); }
        }

        internal static bool HealthCheckHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(HEALTH_CHECK_HIDDEN); }
        }

        internal static bool UploadOptionHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(UPLOAD_OPTION_HIDDEN); }
        }

        internal static bool LicenseNagVisible
        {
            get
            { return Registry.AdditionalFeatures != null && Registry.AdditionalFeatures.Contains(LICENSE_NAG); }
        }

        internal static bool LicenseOperationsHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(LICENSE_OPERATIONS_HIDDEN); }
        }

        private const string CPS_OPTIMIZATION_HIDDEN = "cps_optimization";
        private const string RDP_POLLING_HIDDEN = "rdp_polling";
        private const string LEARN_MORE_HIDDEN = "learn_more";
        private const string LINK_LABEL_HIDDEN = "link_label";
        private const string TOOL_STRIP_MENU_ITEM_HIDDEN = "tool_strip_menu_item";
        private const string CROSS_SERVER_PRIVATE_NETWORK_HIDDEN = "cross_server_private_network";
        private const string COPYRIGHT_HIDDEN = "copyright";
        private const string HEALTH_CHECK_HIDDEN = "health_check";
        private const string UPLOAD_OPTION_HIDDEN = "upload_option";
        private const string LICENSE_NAG = "license_nag";
        private const string LICENSE_OPERATIONS_HIDDEN = "license_operations";
    }
}
