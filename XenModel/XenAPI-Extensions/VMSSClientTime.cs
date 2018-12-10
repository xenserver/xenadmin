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

namespace XenAPI
{
    public static class VMSSClient
    {
        public static Dictionary<string, string> FindScheduleWithGivenTimeOffset(
            TimeSpan timeDiff,
            vmss_frequency frequency,
            Dictionary<string, string> schedule)
        {
            string hour;
            string min;
            string days;
            var output = new Dictionary<string, string>();

            switch (frequency)
            {
                case vmss_frequency.hourly:
                    if (schedule.TryGetValue("hour", out hour))
                        output["hour"] = hour;

                    if (schedule.TryGetValue("min", out min))
                    {
                        var newMin = (int.Parse(min) + Convert.ToInt32(timeDiff.TotalMinutes)) % 60;
                        if (newMin < 0)
                            newMin = 60 - Math.Abs(newMin);
                        output["min"] = newMin.ToString();
                    }

                    if (schedule.TryGetValue("days", out days))
                        output["days"] = days;

                    return output;
                case vmss_frequency.daily:
                    if (schedule.TryGetValue("hour", out hour))
                    {
                        var newHour = (int.Parse(hour) + timeDiff.TotalHours) % 24;
                        if (newHour < 0)
                            newHour = 24 - Math.Abs(newHour);
                        output["hour"] = Convert.ToInt32(Math.Floor(newHour)).ToString();
                    }

                    if (schedule.TryGetValue("min", out min))
                    {
                        var newMin = (int.Parse(min) + Convert.ToInt32(timeDiff.TotalMinutes)) % 60;
                        if (newMin < 0)
                            newMin = 60 - Math.Abs(newMin);
                        output["min"] = newMin.ToString();
                    }

                    if (schedule.TryGetValue("days", out days))
                        output["days"] = days;

                    return output;
                case vmss_frequency.weekly:
                    throw new NotImplementedException("Unhandled vmss_frequency value.");
                case vmss_frequency.unknown:
                    return schedule;
                default:
                    throw new ArgumentException("Unhandled vmss_frequency value.");
            }
        }
    }
}
