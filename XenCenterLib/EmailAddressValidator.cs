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
using System.Net.Mail;

namespace XenCenterLib
{
    /// <summary>
    /// Validates an email address. 
    /// </summary>
    public class EmailAddressValidator
    {
        /// <summary>
        /// Validates an email address. Null, empty, or strings containing forbidden chars
        /// will return false. Otherwise validity is checked using a MS class.
        /// 
        /// Note: This uses FormatException to control flow.
        /// </summary>
        /// <returns>email address is valid</returns>
        public bool IsValid(string emailAddress)
        {
            if (String.IsNullOrEmpty(emailAddress))
                return false;

            if (ContainsForbiddenCharacters(emailAddress))
                return false;

            MailMessage mm = new MailMessage();
            try
            {
                mm.To.Add(emailAddress);
                return true;
            }
            catch (FormatException) //Thrown if an ill-formed email address is given
            {
                return false;
            }
        }

        private bool ContainsForbiddenCharacters(string emailAddress)
        {
            char[] forbiddenCharacters = new []
                                             {
                                                 '(',
                                                 ')',
                                                 ' '
                                             };

            if (emailAddress.IndexOfAny(forbiddenCharacters) == -1)
                return false;
            return true;
        }
    }
}
