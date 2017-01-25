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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net;

namespace ApiVersionTool
{
    /// <summary>
    /// Reads all API calls which were introduced at a specified version e.g. orlando, george, miami and outputs them to an XML file.
    /// </summary>
    public partial class Form1 : Form
    {
        private const string _orlando = @"http://hg/carbon/orlando-update-4/xenadmin.hg/raw-file/tip/XenAdmin/XenAPI/Proxy.cs";
        private const string _george = @"http://hg/carbon/george/xenadmin.hg/raw-file/tip/XenAdmin/XenAPI/Proxy.cs";
        private const string _mnr = @"http://hg/carbon/mnr/xenadmin.hg/raw-file/tip/XenAdmin/XenAPI/Proxy.cs";

        public Form1()
        {
            InitializeComponent();
        }

        private List<string> Parse(string file)
        {
            List<string> output = new List<string>();
            string text = new WebClient().DownloadString(file);

            //[XmlRpcMethod("session.login_with_password")]
            //Response<string> session_login_with_password(string username, string password);
            foreach (Match m in Regex.Matches(text, @"Response\<[^\>]+\>\s+([^\(\s]+)", RegexOptions.Singleline))
            {
                output.Add(m.Groups[1].Value);
            }

            return output;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            List<string> orlando = null;
            List<string> george = null;
            List<string> mnr = null;

            try
            {
                orlando = Parse(_orlando);
                george = Parse(_george);
                mnr = Parse(_mnr);
            }
            catch (WebException ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }

            for (int i = mnr.Count - 1; i >= 0; i--)
            {
                if (george.Contains(mnr[i]))
                {
                    mnr.RemoveAt(i);
                }
            }

            for (int i = george.Count - 1; i >= 0; i--)
            {
                if (orlando.Contains(george[i]))
                {
                    george.RemoveAt(i);
                }
            }

            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            dic["george"] = george;
            dic["mnr"] = mnr;

            XmlDocument doc = new XmlDocument();

            XmlComment comment = doc.CreateComment(@"Created by devtools\ApiVersionTool");
            doc.AppendChild(comment);

            XmlNode root = doc.CreateNode(XmlNodeType.Element, "ApiVersionTool", "");
            doc.AppendChild(root);

            foreach (string ver in dic.Keys)
            {
                XmlNode verNode = doc.CreateNode(XmlNodeType.Element, "version", "");
                root.AppendChild(verNode);
                XmlAttribute a = doc.CreateAttribute("name");
                a.Value = ver;
                verNode.Attributes.Append(a);

                foreach (string call in dic[ver])
                {
                    XmlNode callNode = doc.CreateNode(XmlNodeType.Element, "call", "");
                    verNode.AppendChild(callNode);
                    a = doc.CreateAttribute("name");
                    a.Value = call;
                    callNode.Attributes.Append(a);
                }
            }

            string tmp = Path.GetTempFileName() + ".xml";
            doc.Save(tmp);
            webBrowser1.Url = new Uri("file://" + tmp);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (webBrowser1.Url != null)
            {
                Clipboard.SetText(File.ReadAllText(webBrowser1.Url.LocalPath));
            }
        }
    }
}
