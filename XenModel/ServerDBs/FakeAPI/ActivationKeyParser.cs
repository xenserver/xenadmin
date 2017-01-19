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
using System.Reflection;
using System.Text.RegularExpressions;

namespace XenAdmin.ServerDBs.FakeAPI
{
    /// <summary>
    /// Parses activation keys so that activation can be simulated using DbProxy.
    /// </summary>
    internal class ActivationKeyParser
    {
        public readonly string sku_type = null;
        public readonly string version = null;
        public readonly string productcode = null;
        public readonly string serialnumber = null;
        public readonly string sockets = null;
        public readonly string expiry = null;
        public readonly string human_readable_expiry = null;
        public readonly string name = null;
        public readonly string address1 = null;
        public readonly string address2 = null;
        public readonly string city = null;
        public readonly string state = null;
        public readonly string postalcode = null;
        public readonly string country = null;
        public readonly string company = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationKeyParser"/> class.
        /// </summary>
        /// <param name="text">The text of the activation key base 64 encoded. </param>
        public ActivationKeyParser(string base64Key)
        {
            Util.ThrowIfStringParameterNullOrEmpty(base64Key, "base64Key");

            byte[] bytes = Convert.FromBase64String(base64Key);
            string text = Encoding.ASCII.GetString(bytes);

            //-----BEGIN PGP SIGNED MESSAGE-----
            //Hash: SHA1

            //<xe_license sku_type="XE Enterprise" version="5.0.0" productcode="f3a6-a3c0-ed16-c709-b055-8789" serialnumber="cbc0b0d4-d961-cf8f-0558-5fedde2caf9e" sockets="2" expiry="1264896000.000000" human_readable_expiry="2010-01-30" name="Citrix XenServer Sample License" address1="" address2="" city="" state="" postalcode="" country="" company=""/>
            //-----BEGIN PGP SIGNATURE-----
            //Version: GnuPG v1.4.6 (GNU/Linux)

            //iD8DBQFJLrvDor8EvHqMeKcRAuO8AJ94i1aUlIOCTJcbzG8wiwkxj5enxQCZAcDE
            //2lSAnkkPMob14pZJbh+F6XI=
            //=oxXD
            //-----END PGP SIGNATURE-----


            // parse each attribute from this part of the key:
            // <xe_license sku_type="XE Enterprise" version="5.0.0" productcode="f3a6-a3c0-ed16-c709-b055-8789" serialnumber="cbc0b0d4-d961-cf8f-0558-5fedde2caf9e" sockets="2" expiry="1264896000.000000" human_readable_expiry="2010-01-30" name="Citrix XenServer Sample License" address1="" address2="" city="" state="" postalcode="" country="" company=""/>
            foreach (FieldInfo fi in GetType().GetFields())
            {
                Match m = Regex.Match(text, fi.Name + @"=""([^""]*)""");

                if (m.Success)
                {
                    fi.SetValue(this, m.Groups[1].Value);
                }
            }
        }
    }
}
