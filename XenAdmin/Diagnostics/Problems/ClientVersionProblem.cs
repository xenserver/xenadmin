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
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;

namespace XenAdmin.Diagnostics.Problems
{
    public class ClientVersionProblem : ProblemWithInformationUrl
    {
        private ClientVersion _requiredXenCenterVersion;

        public ClientVersionProblem(Check check, ClientVersion requiredXenCenterVersion)
            : base(check)
        {
            _requiredXenCenterVersion = requiredXenCenterVersion;
        }

        public override string Title => string.Format(Messages.PROBLEM_XENCENTER_VERSION_TITLE, BrandManager.BrandConsole);

        public override string Description => string.Format(Messages.UPDATES_WIZARD_NEWER_XENCENTER_REQUIRED, BrandManager.BrandConsole, _requiredXenCenterVersion.Version);

        public override string HelpMessage => LinkText;

        public override string LinkText => Messages.PATCHING_WIZARD_WEBPAGE_CELL;

        public override Uri UriToLaunch => new Uri(_requiredXenCenterVersion.Url);
    }

    public class ClientVersionWarning : WarningWithInformationUrl
    {
        private ClientVersion _requiredXenCenterVersion;

        public ClientVersionWarning(Check check, ClientVersion requiredXenCenterVersion)
            : base(check)
        {
            _requiredXenCenterVersion = requiredXenCenterVersion;
        }

        public override string Title => string.Format(Messages.PROBLEM_XENCENTER_VERSION_TITLE, BrandManager.BrandConsole);

        public override string Description => string.Format(Messages.UPDATES_WIZARD_NEWER_XENCENTER_WARNING,
            BrandManager.BrandConsole, _requiredXenCenterVersion.Version, BrandManager.ProductBrand);

        public override string HelpMessage => LinkText;

        public override string LinkText => Messages.PATCHING_WIZARD_WEBPAGE_CELL;

        public override Uri UriToLaunch => new Uri(_requiredXenCenterVersion.Url);
    }
}
