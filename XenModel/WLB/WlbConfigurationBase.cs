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
using System.Globalization;

namespace XenAdmin.Wlb
{
    public class WlbConfigurationBase
    {
        protected const double KILOBYTE = 1024;

        private List<string> _wlbConfigurationKeys;
        private WlbConfigurationKeyBase _keyBase;
		private string _itemId;
        private Dictionary<string, string> _configuration;
        private NumberFormatInfo _nfi = new CultureInfo("en-US", false).NumberFormat;
        private DateTimeFormatInfo _dtfi = new CultureInfo("en-US", false).DateTimeFormat;

		public enum WlbConfigurationKeyBase
        {
            pool,
            host,
            schedTask,
            rpSub
        }

        #region Constructor
        public WlbConfigurationBase()
        {
            _configuration = new Dictionary<string, string>();
            _nfi = new CultureInfo("en-US", false).NumberFormat;
            _keyBase = WlbConfigurationKeyBase.pool;
        }
        #endregion

        #region Static Conversion Functions
        public static decimal ConvertToMB(double byteValue)
        {
            return Convert.ToDecimal(byteValue / 1024 / 1024);
        }

        public static double ConvertFromMB(decimal mbyteValue)
        {
            return (double)(mbyteValue * 1024 * 1024);
        }

        public static double ConvertToMbps(double rawValue)
        {
            return (int)(rawValue / KILOBYTE / KILOBYTE);
        }

        public static double ConvertFromMbps(double mbpsValue)
        {
            return (double)(mbpsValue * KILOBYTE * KILOBYTE);
        }
		#endregion


		#region Public Methods
        public Dictionary<string, string> ToDictionary()
        {
            return _configuration;
        }

        #endregion

        
        #region protected Methods

        protected string GetConfigValueString(string key)
        {
            string stringValue = String.Empty;
            if (_configuration.ContainsKey(key))
            {
                stringValue = _configuration[key];
            }
            return stringValue;
        }

        protected void SetConfigValueString(string key, string value)
        {
            SetConfigValueString(key, value, false);
        }

        protected void SetConfigValueString(string key, string value, bool createKey)
        {
            if (!_configuration.ContainsKey(key) && createKey)
            {
                _configuration.Add(key, value);
            }
            else if (_configuration.ContainsKey(key))
            {
                _configuration[key] = value;
            }
        }

        protected int GetConfigValuePercent(string key)
        {
            return GetConfigValuePercent(key, 0);
        }

        protected int GetConfigValuePercent(string key, int defaultValue)
        {
            double floatValue;
            if (!_configuration.ContainsKey(key) || !double.TryParse(_configuration[key], System.Globalization.NumberStyles.Any, _nfi, out floatValue))
            {
                floatValue = ((double)defaultValue) / 100;
            }
            return (int)(floatValue * 100);
        }

        protected void SetConfigValuePercent(string key, int value)
        {
            SetConfigValuePercent(key, value, false);
        }

        protected void SetConfigValuePercent(string key, int value, bool createKey)
        {
            double percentageValue = ((double)value) / 100;
            
            if (!_configuration.ContainsKey(key) && createKey)
            {
                _configuration.Add(key, percentageValue.ToString(_nfi));
            }
            else if (_configuration.ContainsKey(key))
            {
                _configuration[key] = percentageValue.ToString(_nfi);
            }
        }

        protected double GetConfigValueDouble(string key)
        {
            return GetConfigValueDouble(key, 0);
        }

        protected double GetConfigValueDouble(string key, double defaultValue)
        {
            double doubleValue;
            if (!_configuration.ContainsKey(key) || !double.TryParse(_configuration[key], System.Globalization.NumberStyles.Any, _nfi, out doubleValue))
            {
                doubleValue = defaultValue;
            }
            return doubleValue;
        }

        protected void SetConfigValueDouble(string key, double value)
        {
            SetConfigValueDouble(key, value, false);
        }

        protected void SetConfigValueDouble(string key, double value, bool createKey)
        {
            if (!_configuration.ContainsKey(key) && createKey)
            {
                _configuration.Add(key, Math.Round(value, 0).ToString(_nfi));
            }
            else if (_configuration.ContainsKey(key))
            {
                _configuration[key] = Math.Round(value, 0).ToString(_nfi);
            }
        }

        protected int GetConfigValueInt(string key)
        {
            return GetConfigValueInt(key, 0);
        }

        protected int GetConfigValueInt(string key, int defaultValue)
        {
            int result = defaultValue;
            if (_configuration.ContainsKey(key))
            {
                int.TryParse(_configuration[key], System.Globalization.NumberStyles.Any, _nfi, out result);
            }
            return result;
        }

        protected void SetConfigValueInt(string key, int value)
        {
            SetConfigValueInt(key, value, false);
        }

        protected void SetConfigValueInt(string key, int value, bool createKey)
        {
            if (!_configuration.ContainsKey(key) && createKey)
            {
                _configuration.Add(key, value.ToString(_nfi));
            }
            else if(_configuration.ContainsKey(key))
            {
                _configuration[key] = value.ToString(_nfi);
            }
        }

        protected DateTime GetConfigValueUTCDateTime(string key)
        {
            return GetConfigValueUTCDateTime(key, DateTime.MinValue);
        }

        protected DateTime GetConfigValueUTCDateTime(string key, DateTime defaultValue)
        {
            DateTime result = defaultValue;
            if (_configuration.ContainsKey(key))
            {
                // Converts the specified string representation of a date and time to its System.DateTime
                // equivalent using the specified culture-specific format information and formatting style.
                DateTime.TryParse(_configuration[key], CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            return result;
        }

        protected void SetConfigValueUTCDateTime(string key, DateTime value)
        {
            SetConfigValueUTCDateTime(key, value, false);
        }

        /// <summary>
        /// Expect value is UTC time
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="createKey"></param>
        protected void SetConfigValueUTCDateTime(string key, DateTime value, bool createKey)
        {
            if (!_configuration.ContainsKey(key) && createKey)
            {
                _configuration.Add(key, value.ToString(_dtfi));
            }
            else if (_configuration.ContainsKey(key))
            {
                _configuration[key] = value.ToString(_dtfi);
            }
        }

        protected bool GetConfigValueBool(string key)
        {
            return GetConfigValueBool(key, false);
        }

        protected bool GetConfigValueBool(string key, bool defaultValue)
        {
            bool boolValue;
            if (!_configuration.ContainsKey(key) || !bool.TryParse(_configuration[key], out boolValue))
            {
                boolValue = defaultValue;
            }
            return boolValue;
        }

        protected void SetConfigValueBool(string key, bool value)
        {
            SetConfigValueBool(key, value, false);
        }

        protected void SetConfigValueBool(string key, bool value, bool createKey)
        {
            if (!_configuration.ContainsKey(key) && createKey)
            {
                _configuration.Add(key, value.ToString(_nfi));
            }
            else if (_configuration.ContainsKey(key))
            {
                _configuration[key] = value.ToString(_nfi).ToLower();
            }
        }

        /// <summary>
        /// Set the value of a non-defined key, considered a parameter
        /// </summary>
        /// <param name="Uuid">uuid of the host</param>
        /// <param name="Key">name of the parameter</param>
        /// <param name="Value">value of the parameter</param>
        protected void SetOtherParameter(string key, string value)
        {
            SetConfigValueString(BuildComplexKey(key), value, true);
        }

        /// <summary>
        /// Set an entire set of parameters (Dictionary) at once
        /// </summary>
        /// <param name="Parameters">Dictionary of parameters</param>
        protected void SetOtherParameters(Dictionary<string, string> Parameters)
        {
            foreach (string key in Parameters.Keys)
            {
                SetOtherParameter(key, Parameters[key]);
            }
        }


        /// <summary>
        /// Get all Other, unknown key's parameters
        /// </summary>
        /// <param name="keyBase">partial key used to form keys of the dicitionary </param>
        /// <returns>Dictionary of parameters (unknown keys and their values)</returns>
        protected Dictionary<string, string> GetOtherParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            string currentKeyBase = string.Format("{0}_{1}_", this._keyBase, this._itemId);
            foreach (string key in this._configuration.Keys)
            {
                if (key.StartsWith(currentKeyBase))
                {
                    string paramName = key.Remove(0, currentKeyBase.Length);
                    if (!this._wlbConfigurationKeys.Contains(paramName))
					{
                        parameters.Add(paramName, this._configuration[key]);
					}
                }
            }
            return parameters;
        }


        /// <summary>
        /// Assemble a dictionary key for a specific host and known key type
        /// </summary>
        /// <param name="Key">key type enumeration</param>
        /// <returns>assembled key string</returns>
        protected string BuildComplexKey(string Key)
        {
            return string.Format("{0}_{1}_{2}", this._keyBase, this._itemId, Key);
        }


        protected Dictionary<string, string> Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }

        /// <summary>
        /// Used by collections to directly insert entries into the Dictionary
        /// </summary>
        /// <param name="key">name of the key</param>
        /// <param name="value">value of the key</param>
        public void AddParameter(string key, string value)
        {
            _configuration.Add(key, value);
        }

        protected List<string> WlbConfigurationKeys
        {
            get { return _wlbConfigurationKeys; }
            set { _wlbConfigurationKeys = value; }
        }

        public WlbConfigurationKeyBase KeyBase
        {
            get { return _keyBase; }
            set { _keyBase = value; }
        }

        protected string ItemId
        {
            get { return _itemId; }
            set { _itemId = value; }
        }

        #endregion
    }
}
