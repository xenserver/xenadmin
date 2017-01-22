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
using System.Text;


namespace XenAdmin.Core
{
    // Wrap a TimeSpan object with prettier printing. Also allows us to specify null spans.
    // (Can't use inheritance, because TimeSpan is a struct not a class).
    public class PrettyTimeSpan: IComparable, IComparable<PrettyTimeSpan>, IEquatable<PrettyTimeSpan>
    {
        private readonly TimeSpan ts;  // the object we are wrapping

        public PrettyTimeSpan(TimeSpan ts)
        {
            this.ts = ts;
        }

        public PrettyTimeSpan(long ticks)
        {
            this.ts = new TimeSpan(ticks);
        }

        public PrettyTimeSpan(int hours, int minutes, int seconds)
        {
            this.ts = new TimeSpan(hours, minutes, seconds);
        }

        public PrettyTimeSpan(int days, int hours, int minutes, int seconds)
        {
            this.ts = new TimeSpan(days, hours, minutes, seconds);
        }

        public PrettyTimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            this.ts = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }

        public override string ToString()
        {
            string format = ts.Minutes != 1 ? Messages.TIME_MINUTES : Messages.TIME_MINUTE;
            string ans = String.Format(format, ts.Minutes);
            if (ts.Ticks >= TimeSpan.TicksPerHour)
            {
                format = ts.Hours != 1 ? Messages.TIME_HOURS : Messages.TIME_HOUR;
                ans = String.Format("{0} {1}", String.Format(format, ts.Hours), ans);
            }
            if (ts.Ticks >= TimeSpan.TicksPerDay)
            {
                format = ts.Days != 1 ? Messages.TIME_DAYS : Messages.TIME_DAY;
                ans = String.Format("{0} {1}", String.Format(format, ts.Days), ans);
            }
            return ans;
        }

        #region Implement interfaces

        public int CompareTo(object obj)
        {
            // other is a PrettyTimeSpan: use that comparison instead
            PrettyTimeSpan otherPTS = obj as PrettyTimeSpan;
            if (otherPTS != null)
                return CompareTo(otherPTS);

            // otherwise use comparison given by underlying timespan
            return ts.CompareTo(obj);
        }

        public int CompareTo(PrettyTimeSpan other)
        {
            // use comparison on underlying timespans
            return ts.CompareTo(other.ts);
        }

        public bool Equals(PrettyTimeSpan other)
        {
            // use comparison on underlying timespans
            return ts.Equals(other.ts);
        }

        #endregion
    }
}
