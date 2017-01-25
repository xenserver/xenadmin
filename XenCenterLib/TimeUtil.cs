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
using System.Globalization;

namespace Citrix.XenCenter
{
    public class TimeUtil
    {
        public const long TicksBefore1970 = 621355968000000000;

        public const string ISO8601DateFormat = "yyyyMMddTHH:mm:ssZ";

        public static long TicksToSeconds(long ticks)
        {
            return ticks / 10000000;
        }

        public static long TicksToSecondsSince1970(long ticks)
        {
            return (long)Math.Floor(new TimeSpan(ticks - (TicksBefore1970)).TotalSeconds);
        }

        /// <summary>
        /// Parses an ISO 8601 date/time into a DateTime.
        /// </summary>
        /// <param name="toParse">Must be of the format yyyyMMddTHH:mm:ssZ. Must not be null.</param>
        /// <returns>The parsed DateTime with Kind DateTimeKind.Utc.</returns>
        public static DateTime ParseISO8601DateTime(string toParse)
        {
            return DateTime.ParseExact(toParse, ISO8601DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        public static string ToISO8601DateTime(DateTime t)
        {
            return t.ToUniversalTime().ToString(ISO8601DateFormat, CultureInfo.InvariantCulture);
        }
    }
}
