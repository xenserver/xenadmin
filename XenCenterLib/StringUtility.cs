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
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;

namespace XenCenterLib
{
    public static class StringUtility
    {
        /// <summary>
        /// Parses server strings of the form "hostname:port" 
        /// </summary>
        public static void ParseHostnamePort(string server, out string hostname, out int port)
        {
            hostname = server;
            port = 0;

            if (string.IsNullOrWhiteSpace(server))
                return;

            var parts = server.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
                hostname = parts[0].Trim();

            if (parts.Length > 1)
                int.TryParse(parts[1].Trim(), out port);
        }

        public static int NaturalCompare(string s1, string s2)
        {
            if (string.Compare(s1, s2, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                // Strings are identical
                return 0;
            }

            if (s1 == null)
                return -1;
            if (s2 == null)
                return 1;

            char[] chars1 = s1.ToCharArray();
            char[] chars2 = s2.ToCharArray();

            // Compare strings char by char
            int min = Math.Min(chars1.Length, chars2.Length);
            for (int i = 0; i < min; i++)
            {
                char c1 = chars1[i];
                char c2 = chars2[i];

                bool c1IsDigit = char.IsDigit(c1);
                bool c2IsDigit = char.IsDigit(c2);

                if (!c1IsDigit && !c2IsDigit)
                {
                    // Two non-digits. Do a string (i.e. alphabetical) comparison.
                    int tmp = String.Compare(s1.Substring(i, 1), s2.Substring(i, 1), StringComparison.CurrentCultureIgnoreCase);
                    if (tmp == 0)
                        continue;  // Identical non-digits. Move onto next character.
                    else
                        return tmp;
                }
                else if (c1IsDigit && c2IsDigit)
                {
                    // See how many digits there are in a row in each string.
                    int j = 1;
                    while (i + j < chars1.Length && char.IsDigit(chars1[i + j]))
                        j++;
                    int k = 1;
                    while (i + k < chars2.Length && char.IsDigit(chars2[i + k]))
                        k++;

                    // A number that is shorter in decimal places must be smaller.
                    if (j < k)
                    {
                        return -1;
                    }
                    else if (k < j)
                    {
                        return 1;
                    }

                    // The two integers have the same number of digits. Compare them digit by digit.
                    for (int m = i; m < i + j; m++)
                    {
                        if (chars1[m] != chars2[m])
                            return chars1[m] - chars2[m];
                    }

                    // Skip the characters we've already compared, so we don't have to do them again. (CA-50738)
                    // (It's only j-1, not j, because we get one more in the loop increment).
                    i += j - 1;
                    continue;
                }
                else
                {
                    // We're comparing a digit to a non-digit.
                    return String.Compare(s1.Substring(i, 1), s2.Substring(i, 1), StringComparison.CurrentCultureIgnoreCase);
                }
            }
            // The shorter string comes first.
            return chars1.Length - chars2.Length;
        }

        private static readonly Regex IPRegex = new Regex(@"^([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})$");
        private static readonly Regex IPRegex0 = new Regex(@"^([0]{1,3})\.([0]{1,3})\.([0]{1,3})\.([0]{1,3})$");

        /// <summary>
        /// Validates IPv4 subnet mask string (strict)
        /// Eg. 255.255.240.0 is valid
        /// </summary>
        /// <remarks>
        /// Logically valid values with trailing zeros will not pass validation
        /// Eg.  255.255.240.00 will return NOT valid
        /// </remarks>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static bool IsValidNetmask(string netmask)
        {
            if (netmask == null)
                return false;
            
            var netmaskBytes = new List<byte>();
            var parts = netmask.Split('.').ToList();

            if (parts.Count != 4 || parts.Any(p => p.Length > 3))
                return false;

            foreach (var part in parts)
            {
                byte byteValue;

                //Converting value to byte if possible. The second check is to make sure that valid, but server side not supported values not get through (eg. 000, 00)
                if (!byte.TryParse(part, System.Globalization.NumberStyles.None, null, out byteValue) || byteValue.ToString() != part)
                    return false;

                netmaskBytes.Add(byteValue);
            }

            var bits = new BitArray(netmaskBytes.ToArray());
            if (bits.Count != 32)
                return false;

            bool wasZero = false;
            for (int octetNo = 0; octetNo < 4; octetNo ++)
                for (int relPos = 7; relPos >=0 ; relPos --) //less significant bit is on the left
                {
                    bool val = bits[octetNo * 8 +  relPos];
                    
                    //if there is 1 again and there has been any 0 before, netmask is invalid. All other cases (if we get here) are valid.
                    if (wasZero && val)
                        return false;

                    if (!val)
                        wasZero = true;
                }

            return true;
        }

        public static bool IsIPAddress(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            // check the general form is ok
            Match m = IPRegex.Match(s);
            if (!m.Success)
                return false;

            // check the individual numbers are in range, easier to do this as a parse than with the regex
            for (int i = 1; i < 3; i++)
            {
                int v;
                if (!int.TryParse(m.Groups[i].Value, out v))
                    return false;

                if (v > 255)
                    return false;
            }

            Match m2 = IPRegex0.Match(s);
            if (m2.Success)
                return false;

            return true;
        }

        /// <summary>
        /// To be used to format file and directory paths for File streams. <br />
        /// Prepends \\?\ to path if it is longer than (MAX_PATH - 12) in Windows. This is the legacy directory length limit.<br /><br />
        /// See <see href='https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilea'>CreateFileA</see> and <see href='https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry'>Maximum Path Length Limitation</see> for more info.
        /// </summary>
        /// <param name="inputPath">The input file path</param>
        /// <returns></returns>
        public static string ToLongWindowsPath(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath) || inputPath.Length < 248 || inputPath.StartsWith(@"\\?\"))
            {
                return inputPath;
            }
            return $@"\\?\{inputPath}";
        }
    }
}