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
using System.Text;
using CFUValidator.OutputDecorators;


namespace CFUValidator.Validators
{
    public abstract class Validator : ISummaryGenerator
    {
        protected List<string> Errors { get; } = new List<string>();

        public void Validate(Action<string> statusReporter)
        {
            statusReporter(Header);
            ValidateCore(statusReporter);
            statusReporter(Footer);
            statusReporter(string.Empty);
        }

        public string GenerateSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine(SummaryTitle);

            if (Errors.Count > 0)
                Errors.ForEach(v => sb.AppendLine(v));
            else
                sb.AppendLine("All OK");

            return sb.ToString();
        }

        protected abstract void ValidateCore(Action<string> statusReporter);

        protected abstract string Header { get; }

        protected abstract string Footer { get; }

        protected abstract string SummaryTitle { get; }
    }
}
