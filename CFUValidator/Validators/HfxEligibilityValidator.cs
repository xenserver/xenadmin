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
using XenAdmin.Core;

namespace CFUValidator.Validators
{
    class HfxEligibilityValidator : Validator
    {
        private List<XenServerVersion> xsversions;

        public HfxEligibilityValidator(List<XenServerVersion> xsversions)
        {
            this.xsversions = xsversions;
        }

        protected override string Header => "Running hotfix eligibility check...";

        protected override string Footer => "Hotfix Eligibility check completed.";

        protected override string SummaryTitle => "Hotfix eligibility check:";

        protected override void ValidateCore(Action<string> statusReporter)
        {
            foreach (XenServerVersion version in xsversions)
                DateSanityCheck(version);
        }

        private void DateSanityCheck(XenServerVersion version)
        {
            if (version.HotfixEligibility == hotfix_eligibility.none && version.EolDate == DateTime.MinValue)
                Errors.Add("Missing or wrong eol-date field on: " + version.Name);
            if (version.HotfixEligibility == hotfix_eligibility.premium && version.HotfixEligibilityPremiumDate == DateTime.MinValue)
                Errors.Add("Missing or wrong hotfix-eligibility-premium-date field on: " + version.Name);
            if (version.HotfixEligibility == hotfix_eligibility.cu && version.HotfixEligibilityNoneDate == DateTime.MinValue)
                Errors.Add("Missing or wrong hotfix-eligibility-none-date field on: " + version.Name);
            if (version.HotfixEligibility == hotfix_eligibility.cu && version.HotfixEligibilityPremiumDate == DateTime.MinValue)
                Errors.Add("Missing or wrong hotfix-eligibility-premium-date field on: " + version.Name);
        }
    }
}
