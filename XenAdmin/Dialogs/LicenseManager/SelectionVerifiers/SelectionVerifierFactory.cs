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

using System;
using System.Collections.Generic;

namespace XenAdmin.Dialogs.LicenseManagerSelectionVerifiers
{
    public abstract class SelectionVerifierFactory
    {
        public enum Option
        {
            CanUseLicenseServer,
            HaOn,
            NotLive,
            OldServer
        }

        public abstract LicenseSelectionVerifier Verifier(Option option, List<LicenseDataGridViewRow> rows);
    }

    public class LicenseSelectionVerifierFactory : SelectionVerifierFactory
    {
        public override LicenseSelectionVerifier Verifier(Option option, List<LicenseDataGridViewRow> rows)
        {
            LicenseSelectionVerifier verifier = GetCorrectVerifier(option, rows);
            verifier.Verify();
            return verifier;
        }

        private LicenseSelectionVerifier GetCorrectVerifier(Option option, List<LicenseDataGridViewRow> rows)
        {
            if (option == Option.CanUseLicenseServer)
                return new CanUseLicenseServerVerifier(rows);
            if (option == Option.HaOn)
                return new HaOnVerifier(rows);
            if (option == Option.NotLive)
                return new NotLiveVerifier(rows);
            if (option == Option.OldServer)
                return new OlderServerVerifier(rows);

            throw new ArgumentException("No valid option was provided");
        }
    }
}
