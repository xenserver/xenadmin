/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Alerts;

namespace CFUValidator.Validators
{
    class CorePatchDetailsValidator : Validator
    {
        private readonly List<XenServerPatchAlert> alerts;

        public CorePatchDetailsValidator(List<XenServerPatchAlert> alerts)
        {
            this.alerts = alerts;
        }

        protected override string SummaryTitle => "Core details in patch checks:";

        protected override string Header => "Verifying core patch details...";

        protected override string Footer => "Verification of core patch details completed.";

        protected override void ValidateCore(Action<string> statusReporter)
        {
            foreach (XenServerPatchAlert alert in alerts)
            {
                if(string.IsNullOrEmpty(alert.Patch.Uuid))
                    Errors.Add("Missing patch uuid for patch: " + alert.Patch.Name);
                if(string.IsNullOrEmpty(alert.Patch.Name))
                    Errors.Add("Missing patch name for patch with UUID: " + alert.Patch.Uuid);
                if(string.IsNullOrEmpty(alert.Patch.PatchUrl))
                    Errors.Add("Missing patch patch-url for patch with UUID: " + alert.Patch.Uuid);
                if (string.IsNullOrEmpty(alert.Patch.Description))
                    Errors.Add("Missing patch description for patch with UUID: " + alert.Patch.Uuid);
                if (string.IsNullOrEmpty(alert.Patch.Url))
                    Errors.Add("Missing patch webpage url for patch with UUID: " + alert.Patch.Uuid);
                if (string.IsNullOrEmpty(alert.Patch.Guidance))
                    Errors.Add("Missing patch guidance for patch with UUID: " + alert.Patch.Uuid);
                if (string.IsNullOrEmpty(alert.Patch.TimeStamp.ToString()))
                    Errors.Add("Missing patch timestamp for patch with UUID: " + alert.Patch.Uuid);
            }
        }
    }
}
