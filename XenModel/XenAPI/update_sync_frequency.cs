/*
 * Copyright (c) Cloud Software Group, Inc.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using Newtonsoft.Json;


namespace XenAPI
{
    [JsonConverter(typeof(update_sync_frequencyConverter))]
    public enum update_sync_frequency
    {
        /// <summary>
        /// The update synchronizations happen every day
        /// </summary>
        daily,
        /// <summary>
        /// The update synchronizations happen every week on the chosen day
        /// </summary>
        weekly,
        unknown
    }

    public static class update_sync_frequency_helper
    {
        public static string ToString(update_sync_frequency x)
        {
            return x.StringOf();
        }
    }

    public static partial class EnumExt
    {
        public static string StringOf(this update_sync_frequency x)
        {
            switch (x)
            {
                case update_sync_frequency.daily:
                    return "daily";
                case update_sync_frequency.weekly:
                    return "weekly";
                default:
                    return "unknown";
            }
        }
    }

    internal class update_sync_frequencyConverter : XenEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((update_sync_frequency)value).StringOf());
        }
    }
}