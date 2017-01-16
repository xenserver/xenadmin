/* Copyright (c) Citrix Systems, Inc. 
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

using System.Collections.Generic;
using XenAdmin.Alerts;

namespace CFUValidator.Validators
{
    class CorePatchDetailsValidator : AlertFeatureValidator
    {
        public CorePatchDetailsValidator(List<XenServerPatchAlert> alerts) : base(alerts){}

        public override void Validate()
        {
            foreach (XenServerPatchAlert alert in alerts)
            {
                VerifyPatchDetailsMissing(alert);
            }
        }

        private void VerifyPatchDetailsMissing(XenServerPatchAlert alert)
        {
            if(string.IsNullOrEmpty(alert.Patch.Uuid))
                Results.Add("Missing patch uuid for patch: " + alert.Patch.Name);
            if(string.IsNullOrEmpty(alert.Patch.Name))
                Results.Add("Missing patch name for patch with UUID: " + alert.Patch.Uuid);
            if(string.IsNullOrEmpty(alert.Patch.PatchUrl))
                Results.Add("Missing patch patch-url for patch with UUID: " + alert.Patch.Uuid);
            if (string.IsNullOrEmpty(alert.Patch.Description))
                Results.Add("Missing patch description for patch with UUID: " + alert.Patch.Uuid);
            if (string.IsNullOrEmpty(alert.Patch.Url))
                Results.Add("Missing patch webpage url for patch with UUID: " + alert.Patch.Uuid);
            if (string.IsNullOrEmpty(alert.Patch.Guidance))
                Results.Add("Missing patch guidance for patch with UUID: " + alert.Patch.Uuid);
            if (string.IsNullOrEmpty(alert.Patch.TimeStamp.ToString()))
                Results.Add("Missing patch timestamp for patch with UUID: " + alert.Patch.Uuid);
        }

        public override string Description
        {
            get { return "Verify core patch details"; }
        }
    }
}
