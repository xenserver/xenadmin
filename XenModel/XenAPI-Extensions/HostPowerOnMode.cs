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
using System.Linq;
using XenAdmin;
using XenAdmin.Core;


namespace XenAPI
{
    public partial class Host
    {
        private const string POWER_ON_IP = "power_on_ip";
        private const string POWER_ON_USER = "power_on_user";
        private const string POWER_ON_PASSWORD_SECRET = "power_on_password_secret";

        public abstract class PowerOnMode : IEquatable<PowerOnMode>
        {
            public static PowerOnMode Create(Host host)
            {
                if (string.IsNullOrEmpty(host.power_on_mode))
                    return new PowerOnModeDisabled();
                if (host.power_on_mode == "wake-on-lan")
                    return new PowerOnModeWakeOnLan();
                if (host.power_on_mode == "iLO")
                    if (Helpers.StockholmOrGreater(host.Connection))
                        return new PowerOnModeDisabled();
                    else
                        return new PowerOnModeiLO();
                if (host.power_on_mode == "DRAC")
                    return new PowerOnModeDRAC();

                return new PowerOnModeCustom {CustomMode = host.power_on_mode};
            }

            public bool Active { get; set; }

            public abstract Dictionary<string, string> Config { get; }

            public virtual string FriendlyName => ToString();

            public virtual void Load(Host host)
            {
            }

            protected string GetSecret(Host host)
            {
                try
                {
                    var opaqueRef = Secret.get_by_uuid(host.Connection.Session, host.power_on_config[POWER_ON_PASSWORD_SECRET]);
                    return Secret.get_value(host.Connection.Session, opaqueRef);
                }
                catch
                {
                    return string.Empty;
                }
            }

            public override bool Equals(object other)
            {
                return other is PowerOnMode o && ToString() == o.ToString();
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(PowerOnMode other)
            {
                return Equals(other as object);
            }
        }

        public class PowerOnModeDisabled : PowerOnMode
        {
            public override string ToString()
            {
                return string.Empty;
            }

            public override string FriendlyName => Messages.DISABLED;

            public override Dictionary<string, string> Config => new Dictionary<string, string>();
        }

        public class PowerOnModeWakeOnLan : PowerOnMode
        {
            public override string ToString()
            {
                return "wake-on-lan";
            }

            public override string FriendlyName => Messages.WAKE_ON_LAN;

            public override Dictionary<string, string> Config => new Dictionary<string, string>();
        }

        public class PowerOnModeDRAC : PowerOnMode
        {
            public string IpAddress { get; set; } = "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";

            public override string ToString()
            {
                return "DRAC";
            }

            public override void Load(Host host)
            {
                if (host.power_on_config.TryGetValue(POWER_ON_IP, out var powerOnIp))
                    IpAddress = powerOnIp;
                if (host.power_on_config.TryGetValue(POWER_ON_USER, out var powerOnUser))
                    Username = powerOnUser;
                Password = GetSecret(host);
            }

            public override Dictionary<string, string> Config =>
                new Dictionary<string, string>
                {
                    {POWER_ON_IP, IpAddress},
                    {POWER_ON_USER, Username},
                    {POWER_ON_PASSWORD_SECRET, Password}
                };
        }

        public class PowerOnModeiLO : PowerOnMode
        {
            public string IpAddress { get; set; } = "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";

            public override string ToString()
            {
                return "iLO";
            }

            public override void Load(Host host)
            {
                if (host.power_on_config.TryGetValue(POWER_ON_IP, out var powerOnIp))
                    IpAddress = powerOnIp;
                if (host.power_on_config.TryGetValue(POWER_ON_USER, out var powerOnUser))
                    Username = powerOnUser;
                Password = GetSecret(host);
            }

            public override Dictionary<string, string> Config =>
                new Dictionary<string, string>
                {
                    {POWER_ON_IP, IpAddress},
                    {POWER_ON_USER, Username},
                    {POWER_ON_PASSWORD_SECRET, Password}
                };
        }

        public class PowerOnModeCustom : PowerOnMode
        {
            public string CustomMode { get; set; } = string.Empty;
            public Dictionary<string, string> CustomConfig { get; private set; } = new Dictionary<string, string>();

            public PowerOnModeCustom()
            {
            }

            public override string ToString()
            {
                return CustomMode;
            }

            public override void Load(Host host)
            {
                CustomMode = host.power_on_mode;

                CustomConfig = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> pair in host.power_on_config)
                {
                    var val = pair.Key == POWER_ON_PASSWORD_SECRET ? GetSecret(host) : pair.Value;
                    CustomConfig.Add(pair.Key, val);
                }
            }

            public override Dictionary<string, string> Config => CustomConfig.ToDictionary(c => c.Key, c => c.Value);
        }
    }
}
