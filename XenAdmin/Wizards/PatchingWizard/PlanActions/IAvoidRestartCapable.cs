using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    interface IAvoidRestartHostsAware
    {
        List<string> AvoidRestartHosts { set; }
    }
}
