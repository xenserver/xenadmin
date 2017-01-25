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
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.LicenseManagerSelectionVerifiers;

namespace XenAdminTests.LicensingTests
{
    public class BlanketResponseSelectionVerifierFactory : SelectionVerifierFactory
    {
        public BlanketResponseSelectionVerifierFactory(){}
        public BlanketResponseSelectionVerifierFactory(LicenseSelectionVerifier verifier)
        {
            VerifierToReturn = verifier;
        }
        public LicenseSelectionVerifier VerifierToReturn { set; private get; }
        public override LicenseSelectionVerifier Verifier(Option option, List<LicenseDataGridViewRow> rows)
        {
            VerifierToReturn.Verify();
            return VerifierToReturn;
        }
    }

    public class FailingOptionSelectionVerifierFactory : SelectionVerifierFactory
    {
        public FailingOptionSelectionVerifierFactory() { }
        public FailingOptionSelectionVerifierFactory(Option option)
        {
            FailingOption = option;
        }
        public Option FailingOption { set; private get; }
        public override LicenseSelectionVerifier Verifier(Option option, List<LicenseDataGridViewRow> rows)
        {
            LicenseSelectionVerifier verifier;
            if (option == FailingOption)
            {
                verifier = new AllNotOkVerifier();
            }
            else
            {
                verifier = new AllOkVerifier();
            }
                
            verifier.Verify();
            return verifier;
        }
    }

    public class AllOkVerifier : LicenseSelectionVerifier
    {
        public AllOkVerifier() : base(new List<LicenseDataGridViewRow>())
        {
        }

        public override void Verify()
        {
            Status = VerificationStatus.OK;
        }

        public override string VerificationDetails()
        {
            return "all ok";
        }
    }

    public class AllNotOkVerifier : LicenseSelectionVerifier
    {
        public AllNotOkVerifier()
            : base(new List<LicenseDataGridViewRow>())
        {
        }

        public override void Verify()
        {
            Status = VerificationStatus.Error;
        }

        public override string VerificationDetails()
        {
            return "all bad";
        }
    }
}
