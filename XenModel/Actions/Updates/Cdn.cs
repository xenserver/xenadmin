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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using XenAPI;


namespace XenAdmin.Core
{
    #region Serialiazable classes

    [Serializable]
    public class CdnPoolUpdateInfo
    {
        [JsonProperty("hosts")]
        public CdnHostUpdateInfo[] HostsWithUpdates { get; set; }

        [JsonProperty("updates")]
        public CdnUpdate[] Updates { get; set; }

        [JsonProperty("hash")]
        public string Checksum { get; set; }
    }

    [Serializable]
    public class CdnHostUpdateInfo
    {
        [JsonProperty("ref")]
        public string HostOpaqueRef { get; set; }

        [JsonProperty("recommended-guidance")]
        public CdnGuidance[] RecommendedGuidance { get; set; }

        [JsonProperty("absolute-guidance")]
        public CdnGuidance[] AbsoluteGuidance { get; set; }

        [JsonProperty("RPMS")]
        public string[] Rpms { get; set; }

        [JsonProperty("updates")]
        public string[] UpdateIDs { get; set; }

        [JsonProperty("livepatches")]
        public CdnLivePatch[] LivePatches { get; set; }
    }

    [Serializable]
    public class CdnUpdate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("issued")]
        [JsonConverter(typeof(XenDateTimeConverter))]
        public DateTime IssueDate { get; set; }

        [JsonProperty("severity")]
        public CdnUpdateSeverity Severity { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("special-info")]
        public string SpecialInfo { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("type")]
        public CdnUpdateType Type { get; set; }

        [JsonProperty("recommended-guidance")]
        public CdnGuidance RecommendedGuidance { get; set; }

        [JsonProperty("absolute-guidance")]
        public CdnGuidance AbsoluteGuidance { get; set; }

        [JsonProperty("livepatch-guidance")]
        public CdnLivePatchGuidance LivePatchGuidance { get; set; }

        [JsonProperty("livepatches")]
        public CdnLivePatch[] LivePatches { get; set; }

        public override bool Equals(object other)
        {
            return other is CdnUpdate update && Id == update.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    [Serializable]
    public class CdnLivePatch
    {
        [JsonProperty("component")]
        public string Component { get; set; }

        [JsonProperty("base-buildid")]
        public string BaseBuildId { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    #endregion

    #region Serialiazable enums

    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CdnUpdateType
    {
        SecurityFix,
        Bugfix,
        Improvement,
        NewFeature,
        PreviewFeature
    }

    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CdnUpdateSeverity
    {
        High,
        None
    }

    [Serializable]
    [JsonConverter(typeof(CdnGuidanceConverter))]
    public enum CdnGuidance
    {
        RebootHost,
        RestartDeviceModel,
        EvacuateHost,
        RestartToolstack,
        None
    }

    [Serializable]
    [JsonConverter(typeof(CdnLivePatchGuidanceConverter))]
    public enum CdnLivePatchGuidance
    {
        RestartDeviceModel,
        RestartToolstack,
        None
    }

    #endregion

    #region Json.NET extensions

    internal class CdnGuidanceConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var stringToParse = jToken.ToObject<string>();

            if (string.IsNullOrEmpty(stringToParse))
                return CdnGuidance.None;

            try
            {
                return Enum.Parse(objectType, stringToParse);
            }
            catch (ArgumentException)
            {
                return CdnGuidance.None;
            }
        }
    }

    internal class CdnLivePatchGuidanceConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var stringToParse = jToken.ToObject<string>();

            if (string.IsNullOrEmpty(stringToParse))
                return CdnLivePatchGuidance.None;

            try
            {
                return Enum.Parse(objectType, stringToParse);
            }
            catch (ArgumentException)
            {
                return CdnLivePatchGuidance.None;
            }
        }
    }

    #endregion
}
