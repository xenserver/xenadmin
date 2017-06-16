using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Web.Script.Serialization;
using XenAdmin.Plugins;
using XenAPI;

namespace XenAdmin.Telemetry
{
    internal struct XenCenterMetadata
    {
        public SystemInfo System;
        public XenCenterSettings Settings;
        public XenCenterInfrastructure Infrastructure;
        public List<Plugin> Plugins;
        public string Now;
    }

    internal struct SystemInfo
    {
        public string Version;
        public string DotNetVersion;
        public string Culture;
        public string OsVersion;
        public string OsCulture;
        public string IpAddress;
    }

    internal struct CFU
    {
        public bool AllowXenCenterUpdates;
        public bool AllowPatchesUpdates;
        public bool AllowXenServerUpdates;
    }

    internal struct Proxy
    {
        public bool UseProxy;
        public bool UseIEProxy;
        public bool BypassProxyForServers;
        public bool ProxyAuthentication;
        public string ProxyAuthenticationMethod;
    }

    internal struct SaveAndRestore
    {
        public bool SaveSessionCredentials;
        public bool RequireMasterPassword;
    }

    internal struct XenCenterSettings
    {
        public CFU CFU;
        public Proxy Proxy;
        public SaveAndRestore SaveAndRestore;
        public string HelpLastUsed;
    }

    internal struct XenCenterInfrastructure
    {
        public int TotalConnections;
        public int Connected;
    }

    internal struct Plugin
    {
        public string Name;
        public string Organization;
        public bool Enabled;
    }

    public static class XenCenterTelemetry
    {
        public static string GenerateMetadata(PluginManager pluginManager)
        {
            var metadata = new XenCenterMetadata
            {
                System = new SystemInfo
                {
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    DotNetVersion = Environment.Version.ToString(4),
                    Culture = Thread.CurrentThread.CurrentUICulture.EnglishName,
                    OsVersion = Environment.OSVersion.ToString(),
                    OsCulture = CultureInfo.InstalledUICulture.EnglishName,
                    IpAddress = GetLocalIPAddress()
                },
                Settings = new XenCenterSettings
                {
                    CFU = new CFU
                    {
                        AllowXenCenterUpdates = Properties.Settings.Default.AllowXenCenterUpdates,
                        AllowPatchesUpdates = Properties.Settings.Default.AllowPatchesUpdates,
                        AllowXenServerUpdates = Properties.Settings.Default.AllowXenServerUpdates
                    },
                    Proxy = new Proxy
                    {
                        UseProxy = (HTTPHelper.ProxyStyle) Properties.Settings.Default.ProxySetting == HTTPHelper.ProxyStyle.SpecifiedProxy,
                        UseIEProxy = (HTTPHelper.ProxyStyle) Properties.Settings.Default.ProxySetting == HTTPHelper.ProxyStyle.SystemProxy,
                        BypassProxyForServers = Properties.Settings.Default.BypassProxyForServers,
                        ProxyAuthentication = Properties.Settings.Default.ProvideProxyAuthentication,
                        ProxyAuthenticationMethod = ((HTTP.ProxyAuthenticationMethod)Properties.Settings.Default.ProxyAuthenticationMethod).ToString()
                    },
                    SaveAndRestore = new SaveAndRestore
                    {
                        SaveSessionCredentials = Properties.Settings.Default.SaveSession,
                        RequireMasterPassword = Properties.Settings.Default.RequirePass
                    },
                    HelpLastUsed = Properties.Settings.Default.HelpLastUsed
                },
                Infrastructure = new XenCenterInfrastructure
                {
                    TotalConnections = ConnectionsManager.XenConnectionsCopy.Count,
                    Connected = ConnectionsManager.XenConnectionsCopy.Count(c => c.IsConnected)
                },
                Plugins = new List<Plugin>(),
                Now = DateTime.UtcNow.ToString("u")
            };

            if (pluginManager != null)
            {
                foreach (var plugin in pluginManager.Plugins)
                {
                    metadata.Plugins.Add(new Plugin
                    {
                        Name = plugin.Name,
                        Organization = plugin.Organization,
                        Enabled = plugin.Enabled
                    });
                }
            }

            var obj = new Dictionary<string, object> {{Messages.XENCENTER, metadata}};
            return new JavaScriptSerializer().Serialize(obj);
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress != null ? ipAddress.ToString() : null;
        }
    }
}
