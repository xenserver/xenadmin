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
using System.Text.RegularExpressions;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Core
{
    public static partial class Helpers
    {
        private static readonly Regex RpmVersionRegex = new Regex(@"^([0-9]+:)?([0-9a-zA-Z\.]+)(-[0-9a-zA-Z\.]+)?$");

        public class RpmVersion
        {
            public int Epoch { get; private set; }
            public List<int> Version { get; } = new List<int>();
            public List<int> Release { get; } = new List<int>();

            /// <summary>
            /// The xapi RPMs have versions like 1:22.13.0.1.g2226c2e3a-1.1.g4e82970.xs8.
            /// The parts g* referring to git commits and the last part xs8 referring to
            /// the distro are ignored by the parser.
            /// </summary>
            public static RpmVersion Parse(string versionString)
            {
                if (string.IsNullOrEmpty(versionString))
                    return null;

                var match = RpmVersionRegex.Match(versionString);
                if (!match.Success || match.Groups.Count < 4)
                    return null;

                var rpmVersion = new RpmVersion();

                var epoch = match.Groups[1].Value.TrimEnd(':');
                if (int.TryParse(epoch, out var epochRes))
                    rpmVersion.Epoch = epochRes;

                var version = match.Groups[2].Value.Split('.');
                foreach (var v in version)
                {
                    if (int.TryParse(v, out var result))
                        rpmVersion.Version.Add(result);
                    else
                        break;
                }

                var release = match.Groups[3].Value.TrimStart('-').Split('.');
                foreach (var v in release)
                {
                    if (int.TryParse(v, out var result))
                        rpmVersion.Release.Add(result);
                    else
                        break;
                }

                return rpmVersion;
            }
        }

        /// <remarks>Unlike the .NET Framework's Version.CompareTo() method, this method
        /// considers 1.2.0 and 1.2 as equal.</remarks>
        public static int ProductVersionCompare(string version1, string version2)
        {
            var v1 = RpmVersion.Parse(version1);
            var v2 = RpmVersion.Parse(version2);

            if (v1 == null && v2 == null)
                return 0;
            if (v1 == null)
                return -1;
            if (v2 == null)
                return 1;

            int result = v1.Epoch.CompareTo(v2.Epoch);
            if (result != 0)
                return result;

            int i = 0;
            var min = Math.Min(v1.Version.Count, v2.Version.Count);

            while (i < min)
            {
                result = v1.Version[i].CompareTo(v2.Version[i]);
                if (result != 0)
                    return result;
                i++;
            }

            while (i < v1.Version.Count)
            {
                result = v1.Version[i].CompareTo(0);
                if (result != 0)
                    return result;
                i++;
            }

            while (i < v2.Version.Count)
            {
                result = 0.CompareTo(v2.Version[i]);
                if (result != 0)
                    return result;
                i++;
            }

            i = 0;
            min = Math.Min(v1.Release.Count, v2.Release.Count);

            while (i < min)
            {
                result = v1.Release[i].CompareTo(v2.Release[i]);
                if (result != 0)
                    return result;
                i++;
            }

            while (i < v1.Release.Count)
            {
                result = v1.Release[i].CompareTo(0);
                if (result != 0)
                    return result;
                i++;
            }

            while (i < v2.Release.Count)
            {
                result = 0.CompareTo(v2.Release[i]);
                if (result != 0)
                    return result;
                i++;
            }

            return result;
        }


        #region Versions

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool DundeeOrGreater(IXenConnection conn)
        {
            return conn == null || DundeeOrGreater(Helpers.GetCoordinator(conn));
        }

        /// Dundee is ver. 2.0.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool DundeeOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platformVersion = HostPlatformVersion(host);
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.0.0") >= 0;
        }

        public static bool DundeePlusOrGreater(IXenConnection conn)
        {
            return conn == null || conn.Session == null || conn.Session.APIVersion >= API_Version.API_2_6;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool ElyOrGreater(IXenConnection conn)
        {
            return conn == null || ElyOrGreater(Helpers.GetCoordinator(conn));
        }

        /// Ely is ver. 2.1.1
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool ElyOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platformVersion = HostPlatformVersion(host);
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.1.1") >= 0;
        }

        public static bool HavanaOrGreater(IXenConnection conn)
        {
            return conn == null || HavanaOrGreater(Helpers.GetCoordinator(conn));
        }

        /// As Havana platform version is same with Ely and Honolulu, so use product version here
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool HavanaOrGreater(Host host)
        {
            if (host == null)
                return true;
            string productVersion = HostProductVersion(host);
            return
                productVersion != null &&
                ElyOrGreater(host) &&
                !FalconOrGreater(host) &&
                ProductVersionCompare(productVersion, BrandManager.ProductVersion712) >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool FalconOrGreater(IXenConnection conn)
        {
            return conn == null || FalconOrGreater(Helpers.GetCoordinator(conn));
        }

        /// Falcon is ver. 2.3.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool FalconOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platformVersion = HostPlatformVersion(host);
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.2.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool InvernessOrGreater(IXenConnection conn)
        {
            return conn == null || InvernessOrGreater(Helpers.GetCoordinator(conn));
        }

        /// Inverness is ver. 2.4.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool InvernessOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platformVersion = HostPlatformVersion(host);
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.3.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool JuraOrGreater(IXenConnection conn)
        {
            return conn == null || JuraOrGreater(Helpers.GetCoordinator(conn));
        }

        /// Jura is ver. 2.5.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool JuraOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platformVersion = HostPlatformVersion(host);
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.4.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool KolkataOrGreater(IXenConnection conn)
        {
            return conn == null || KolkataOrGreater(Helpers.GetCoordinator(conn));
        }

        /// Kolkata platform version is 2.6.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool KolkataOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platformVersion = HostPlatformVersion(host);
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.5.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool LimaOrGreater(IXenConnection conn)
        {
            return conn == null || LimaOrGreater(Helpers.GetCoordinator(conn));
        }

        /// Lima platform version is 2.7.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool LimaOrGreater(Host host)
        {
            if (host == null)
                return true;

            string platformVersion = HostPlatformVersion(host);
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.6.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool NaplesOrGreater(IXenConnection conn)
        {
            return conn == null || NaplesOrGreater(GetCoordinator(conn));
        }

        /// Naples is ver. 3.0.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool NaplesOrGreater(Host host)
        {
            return host == null || NaplesOrGreater(HostPlatformVersion(host));
        }

        public static bool NaplesOrGreater(string platformVersion)
        {
            return platformVersion != null && ProductVersionCompare(platformVersion, "2.9.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool QuebecOrGreater(IXenConnection conn)
        {
            return conn == null || QuebecOrGreater(GetCoordinator(conn));
        }

        /// Quebec platform version is 3.1.0
        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool QuebecOrGreater(Host host)
        {
            return host == null || QuebecOrGreater(HostPlatformVersion(host));
        }

        public static bool QuebecOrGreater(string platformVersion)
        {
            return platformVersion != null && ProductVersionCompare(platformVersion, "3.0.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool StockholmOrGreater(IXenConnection conn)
        {
            return conn == null || StockholmOrGreater(Helpers.GetCoordinator(conn));
        }

        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool StockholmOrGreater(Host host)
        {
            return host == null || StockholmOrGreater(HostPlatformVersion(host));
        }

        /// Stockholm is ver. 3.2.0
        public static bool StockholmOrGreater(string platformVersion)
        {
            return platformVersion != null && ProductVersionCompare(platformVersion, "3.1.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool Post82X(IXenConnection conn)
        {
            return conn == null || Post82X(GetCoordinator(conn));
        }

        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool Post82X(Host host)
        {
            return host == null || Post82X(HostPlatformVersion(host));
        }

        public static bool Post82X(string platformVersion)
        {
            return platformVersion != null && ProductVersionCompare(platformVersion, "3.2.50") >= 0;
        }

        /// <param name="conn">May be null, in which case true is returned.</param>
        public static bool YangtzeOrGreater(IXenConnection conn)
        {
            return conn == null || YangtzeOrGreater(Helpers.GetCoordinator(conn));
        }

        /// <param name="host">May be null, in which case true is returned.</param>
        public static bool YangtzeOrGreater(Host host)
        {
            return host == null || YangtzeOrGreater(HostPlatformVersion(host));
        }

        /// Yangtze is ver. 3.2.1
        public static bool YangtzeOrGreater(string platformVersion)
        {
            return platformVersion != null && ProductVersionCompare(platformVersion, "3.2.1") >= 0;
        }

        public static bool XapiEqualOrGreater_1_290_0(IXenConnection conn)
        {
            var coordinator = GetCoordinator(conn);
            return coordinator == null || ProductVersionCompare(coordinator.GetXapiVersion(), "1.290.0") >= 0;
        }

        public static bool XapiEqualOrGreater_1_290_0(Host host)
        {
            return host == null || ProductVersionCompare(host.GetXapiVersion(), "1.290.0") >= 0;
        }

        public static bool XapiEqualOrGreater_1_313_0(IXenConnection conn)
        {
            var coordinator = GetCoordinator(conn);
            return coordinator == null || ProductVersionCompare(coordinator.GetXapiVersion(), "1.313.0") >= 0;
        }

        public static bool XapiEqualOrGreater_1_313_0(Host host)
        {
            return host == null || ProductVersionCompare(host.GetXapiVersion(), "1.313.0") >= 0;
        }

        public static bool XapiEqualOrGreater_22_5_0(IXenConnection conn)
        {
            var coordinator = GetCoordinator(conn);
            return coordinator == null || ProductVersionCompare(coordinator.GetXapiVersion(), "22.5.0") >= 0;
        }

        public static bool XapiEqualOrGreater_22_19_0(IXenConnection conn)
        {
            var coordinator = GetCoordinator(conn);
            return coordinator == null || ProductVersionCompare(coordinator.GetXapiVersion(), "22.19.0") >= 0;
        }

        public static bool XapiEqualOrGreater_22_20_0(Host host)
        {
            return host == null || ProductVersionCompare(host.GetXapiVersion(), "22.20.0") >= 0;
        }

        public static bool XapiEqualOrGreater_vtpmtag(IXenConnection conn)
        {
            //TODO: replace with correct tag
            var coordinator = GetCoordinator(conn);
            return coordinator == null || ProductVersionCompare(coordinator.GetXapiVersion(), "22.23.0.66.g56cd7935e-next") >= 0;
        }

        #endregion
    }
}
