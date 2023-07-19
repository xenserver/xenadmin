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
using System.IO;
using System.Xml;
using XenAdmin.Actions;

namespace CFUValidator.Updates
{
    class ReadFromFileUpdatesXmlSource : DownloadCfuAction, ICheckForUpdatesXMLSource
    {
        private readonly string _location;

        public ReadFromFileUpdatesXmlSource(string location)
            : base(true, true, "CFU", location, true)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
        }

        protected override XmlDocument FetchCheckForUpdatesXml()
        {
            if (!File.Exists(_location))
            {
                ErrorRaised = new CFUValidationException("File not found at: " + _location);
                throw ErrorRaised;
            }
                 
            try
            {
                XmlDocument xdoc = new XmlDocument();
                using (StreamReader sr = new StreamReader(_location))
                    xdoc.Load(sr);
                return xdoc;
            }
            catch(Exception)
            {
                ErrorRaised = new CFUValidationException("Could not read/parse file: " + _location);
                throw ErrorRaised;
            }
            
        }

        public Exception ErrorRaised { get; private set; }
    }
}
