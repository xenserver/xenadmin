using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenAdmin.Alerts
{
    public class NewVersionPriorityAlertComparer : IComparer<Alert>
    {
        public int Compare(Alert alert1, Alert alert2)
        {
            if (alert1 == null || alert2 == null)
                return 0;

            int sortResult = 0;

            if (IsVersionOrVersionUpdateAlert(alert1) && !IsVersionOrVersionUpdateAlert(alert2))
                sortResult = 1;

            if (!IsVersionOrVersionUpdateAlert(alert1) && IsVersionOrVersionUpdateAlert(alert2))
                sortResult = -1;

            if (sortResult == 0)
                sortResult = Alert.CompareOnDate(alert1, alert2);

            return -sortResult;
        }

        private bool IsVersionOrVersionUpdateAlert(Alert alert)
        {
            return alert is XenServerPatchAlert && (alert as XenServerPatchAlert).NewServerVersion != null
                || alert is XenServerVersionAlert
                || alert is XenCenterUpdateAlert;
        }
    }
}
