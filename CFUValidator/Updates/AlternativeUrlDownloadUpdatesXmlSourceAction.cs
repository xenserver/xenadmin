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
using System.Net;
using System.Xml;
using XenAdmin.Actions;


namespace CFUValidator.Updates
{
    class AlternativeUrlDownloadUpdatesXmlSourceAction : DownloadUpdatesXmlAction, ICheckForUpdatesXMLSource
    {
        private readonly string _url;

        public AlternativeUrlDownloadUpdatesXmlSourceAction(string url)
            : base(true, true, true, "CFU", true)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        protected override XmlDocument FetchCheckForUpdatesXml()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                    IgnoreProcessingInstructions = true
                };

                WebRequest wr = WebRequest.Create(_url);
                using (Stream xmlStream = wr.GetResponse().GetResponseStream())
                {
                    if (xmlStream != null)
                        using (var reader = XmlReader.Create(xmlStream, settings))
                            doc.Load(reader);
                }

                return doc;
            }
            catch (Exception)
            {
                ErrorRaised = new CFUValidationException("Failed to wget the URL: " + _url);
                throw ErrorRaised;
            }
        }

        public Exception ErrorRaised { get; private set; }
    }
}
