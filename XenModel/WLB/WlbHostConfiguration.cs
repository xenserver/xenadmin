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
using System.Collections;
using System.Text;

namespace XenAdmin.Wlb
{
    public class WlbHostConfiguration : WlbConfigurationBase
    {
        private const string KEY_BASE = "host";
        private const string KEY_PARTICIPATES_IN_POWER_MANAGEMENT = "ParticipatesInPowerManagement";
        private const string KEY_POWER_MANAGEMENT_TESTED = "LastPowerOnSucceeded";
        private const string KEY_HOST_EXCLUSION_PLACEMENT = "ExcludeFromPlacementRecommendations";
        private const string KEY_HOST_EXCLUSION_EVACUATION = "ExcludeFromEvacuationRecommendations";
        private const string KEY_HOST_EXCLUSION_OPTIMIZATION = "ExcludeFromPoolOptimizations";
        private const string KEY_HOST_EXCLUSION_OPTIMIZATION_ACCEPT_VMS = "ExcludeFromPoolOptimizationAcceptVMs";
        
        public WlbHostConfiguration(string Uuid)
        {
            base.Configuration = new Dictionary<string, string>();
            base.ItemId = Uuid;

            //Define the key base
            base.KeyBase = WlbConfigurationKeyBase.host;

            //Define the known keys
            base.WlbConfigurationKeys = new List<string>(new string[] 
                { 
                    KEY_PARTICIPATES_IN_POWER_MANAGEMENT,
                    KEY_POWER_MANAGEMENT_TESTED,
                    KEY_HOST_EXCLUSION_PLACEMENT,
                    KEY_HOST_EXCLUSION_EVACUATION,
                    KEY_HOST_EXCLUSION_OPTIMIZATION,
                    KEY_HOST_EXCLUSION_OPTIMIZATION_ACCEPT_VMS
                });
        }

        public bool ParticipatesInPowerManagement
        {
            get { return GetConfigValueBool(BuildComplexKey(KEY_PARTICIPATES_IN_POWER_MANAGEMENT)); }
            set { SetConfigValueBool(BuildComplexKey(KEY_PARTICIPATES_IN_POWER_MANAGEMENT), value, true); }
        }

        public bool LastPowerOnSucceeded
        {
            get { return GetConfigValueBool(BuildComplexKey(KEY_POWER_MANAGEMENT_TESTED), false); }
            set { SetConfigValueBool(BuildComplexKey(KEY_POWER_MANAGEMENT_TESTED), value, true); }
        }

        public bool Excluded
        {
            get
            {
                return GetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_PLACEMENT), false) ||
                       GetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_EVACUATION), false) ||
                       GetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_OPTIMIZATION), false) ||
                       GetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_OPTIMIZATION_ACCEPT_VMS), false);
            }
            set
            {
                SetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_PLACEMENT), value, true);
                SetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_EVACUATION), value, true);
                SetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_OPTIMIZATION), value, true);
                SetConfigValueBool(BuildComplexKey(KEY_HOST_EXCLUSION_OPTIMIZATION_ACCEPT_VMS), value, true);
            }
        }

        public Dictionary<string, string> Parameters
        {
            get { return GetOtherParameters(); }
            set { SetOtherParameters(value); }
        }

        public string Uuid
        {
            get { return ItemId; }
        }
    }

    public class WlbHostConfigurations : Dictionary<string, WlbHostConfiguration>
    {

        public WlbHostConfigurations(Dictionary<string, string> Configuration)
        {
            foreach (string key in Configuration.Keys)
            {
                if (key.StartsWith("host_"))
                {
                    string[] keyElements = key.Split('_');
                    string uuid = keyElements[1];

                    if (!this.ContainsKey(uuid))
                    {
                        this.Add(uuid, new WlbHostConfiguration(uuid));
                        this[uuid].AddParameter(key, Configuration[key]);
                    }
                    else
                    {
                        this[uuid].AddParameter(key, Configuration[key]);
                    }
                }
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> collectionDictionary = null;
             
            foreach (WlbHostConfiguration hostConfiguration in this.Values)
            {
                Dictionary<string, string> hostDictionary = hostConfiguration.ToDictionary();
                foreach (string key in hostDictionary.Keys)
                {
                    if (null == collectionDictionary)
                    {
                        collectionDictionary = new Dictionary<string, string>();
                    }
                    collectionDictionary.Add(key, hostDictionary[key]);
                }
            }
            return collectionDictionary;
        }
    }
}
