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
using System.Net;
using System.Text;

namespace XenAdmin.Core
{
    public class PartialIP
    {
        public short A, B, C, D;

        private PartialIP(short A, short B, short C, short D)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = D;
        }

        public override String ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}",
                A > -1 ? A.ToString() : "*",
                B > -1 ? B.ToString() : "*",
                C > -1 ? C.ToString() : "*",
                D > -1 ? D.ToString() : "*");
        }


        public bool Equals(IPAddress address)
        {
            byte[] addressBytes = address.GetAddressBytes();

            if (A > -1 && A != addressBytes[0])
                return false;
            else if (B > -1 && B != addressBytes[1])
                return false;
            else if (C > -1 && C != addressBytes[2])
                return false;
            else if (D > -1 && D != addressBytes[3])
                return false;

            return true;
        }

        public static bool TryParse(String candidate, out PartialIP ip)
        {
            ip = null;

            if (!CouldBeValidPartialIp(candidate))
                return false;

            try
            {
                ip = Parse(candidate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static char[] SEPARATORS = new char[] { '.' };
        private static String[] WILDCARDS = new String[] { "", "*", "_", "x", "n" };

        public static PartialIP Parse(String candidate)
        {
            if (!CouldBeValidPartialIp(candidate))
                throw new InvalidOperationException(String.Format("'{0}' is not a valid partial IP", candidate));

            String[] segments = candidate.Split(SEPARATORS);

            return new PartialIP(GetSegment(segments, 0), GetSegment(segments, 1),
                GetSegment(segments, 2), GetSegment(segments, 3));
        }

        private static bool CouldBeValidPartialIp(String candidate)
        {
            return candidate != null && candidate.Contains(".");
        }

        private static short GetSegment(String[] segments, int index)
        {
            if (segments.Length <= index)
                return -1;

            short segment;
            if (short.TryParse(segments[index], out segment) && segment >= 0)
                return segment;

            foreach (String wildcard in WILDCARDS)
                if (segments[index] == wildcard)
                    return -1;

            throw new InvalidOperationException(
                String.Format("'{0}' is not a valid partial IP", String.Join(".", segments)));
        }
    }
}
