/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XenAdmin.Alerts;

namespace CFUValidator.OutputDecorators
{
    class XenCenterUpdateDecorator: Decorator
    {
        private readonly List<XenCenterUpdateAlert> alerts;
        private const string header = "XenCenter updates required:";
        private const string updateNotFound = "XenCenter update could not be found";

        public XenCenterUpdateDecorator(OuputComponent ouputComponent, List<XenCenterUpdateAlert> alerts)
        {
            SetComponent(ouputComponent);
            this.alerts = alerts;
        }

        public override StringBuilder Generate()
        {
            StringBuilder sb = base.Generate();
            sb.AppendLine(header);
            if (alerts == null || alerts.Count == 0)
            {
                sb.AppendLine(updateNotFound);
            }
            else
            {
                var versions = from XenCenterUpdateAlert a in alerts
                               let v = a.NewVersion.VersionAndLang
                               where a.NewVersion != null
                               select v;
                sb.AppendLine(string.Join(",", versions));
            }
            
            return sb.AppendLine(String.Empty);
        }
    }
}
