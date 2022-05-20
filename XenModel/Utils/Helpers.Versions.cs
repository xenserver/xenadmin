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

using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Core
{
    public static partial class Helpers
    {
        /// <summary>
        /// Only log the unrecognised version message once (CA-11201).
        /// </summary>
        private static bool _unrecognisedVersionWarned = false;
        /// <summary>
        /// Numbers should have three parts, i.e. be in the form a.b.c, otherwise they won't be parsed.
        /// </summary>
        /// <param name="version1">May be null.</param>
        /// <param name="version2">May be null.</param>
        /// <returns></returns>
        public static int ProductVersionCompare(string version1, string version2)
        {
            // Assume version numbers are of form 'a.b.c'
            int a1 = 99, b1 = 99, c1 = 99, a2 = 99, b2 = 99, c2 = 99;


            string[] tokens = null;
            if (version1 != null)
                tokens = version1.Split('.');
            if (tokens != null && tokens.Length == 3)
            {
                a1 = int.Parse(tokens[0]);
                b1 = int.Parse(tokens[1]);
                c1 = int.Parse(tokens[2]);
            }
            else
            {
                if (!_unrecognisedVersionWarned)
                {
                    log.DebugFormat("Unrecognised version format {0} - treating as developer version", version1);
                    _unrecognisedVersionWarned = true;
                }
            }
            tokens = null;
            if (version2 != null)
                tokens = version2.Split('.');
            if (tokens != null && tokens.Length == 3)
            {
                a2 = int.Parse(tokens[0]);
                b2 = int.Parse(tokens[1]);
                c2 = int.Parse(tokens[2]);
            }
            else
            {
                if (!_unrecognisedVersionWarned)
                {
                    log.DebugFormat("Unrecognised version format {0} - treating as developer version", version2);
                    _unrecognisedVersionWarned = true;
                }
            }

            if (a2 > a1)
            {
                return -1;
            }
            else if (a2 == a1)
            {
                if (b2 > b1)
                {
                    return -1;
                }
                else if (b2 == b1)
                {
                    if (c2 > c1)
                    {
                        return -1;
                    }
                    else if (c1 == c2)
                    {
                        return 0;
                    }
                }
            }
            return 1;
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

        #endregion
    }
}
