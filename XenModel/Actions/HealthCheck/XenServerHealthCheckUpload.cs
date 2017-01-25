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
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using XenAdmin;
using XenAdmin.Network;
using XenAdmin.Core;

namespace XenServerHealthCheck
{
    public class XenServerHealthCheckUpload
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string UPLOAD_URL = "https://rttf.citrix.com/feeds/api/";
        public const int CHUNK_SIZE = 1 * 1024 * 1024;
        private JavaScriptSerializer serializer;
        private int verbosityLevel;

        private string uploadToken;
        private IWebProxy proxy;

        public XenServerHealthCheckUpload(string token, int verbosity, string uploadUrl, IXenConnection connection)
        {
            uploadToken = token;
            verbosityLevel = verbosity;
            serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            if (!string.IsNullOrEmpty(uploadUrl))
            {
                UPLOAD_URL = uploadUrl;
            }
            proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(connection);
        }

        // Request an upload and fetch the uploading id from CIS.
        public string InitiateUpload(string fileName, long size, string caseNumber)
        {
            // Request a new bundle upload to CIS server.
            string FULL_URL = UPLOAD_URL + "bundle/?size=" + size.ToString() + "&name=" + fileName.UrlEncode();
            if (!string.IsNullOrEmpty(caseNumber))
                FULL_URL += "&sr=" + caseNumber;
            log.InfoFormat("InitiateUpload, UPLOAD_URL: {0}", FULL_URL);
            Uri uri = new Uri(FULL_URL);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            if (uploadToken != null)
            {
                request.Headers.Add("Authorization", "BT " + uploadToken);
            }

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ServicePoint.Expect100Continue = false;
            request.Proxy = proxy;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                // Add the log verbosity level in json cotent.
                string verbosity = "{\"verbosity\":\""+ verbosityLevel + "\"}";
                streamWriter.Write(verbosity);
            }

            Dictionary<string, object> res = new Dictionary<string, object>();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string respString = reader.ReadToEnd();
                    res = (Dictionary<string, object>)serializer.DeserializeObject(respString);
                }
                response.Close();
            }
            catch (WebException e)
            {
                log.ErrorFormat("Exception while requesting a new CIS uploading: {0}", e);
                return "";
            }

            if (res.ContainsKey("id"))
            {
                // Get the uuid of uploading
                return (string)res["id"];
            }
            else
            {
                // Fail to initialize the upload request
                return "";
            }
        }

        // Upload a chunk.
        public bool UploadChunk(string address, string filePath, long offset, long thisChunkSize, string uploadToken)
        {
            log.InfoFormat("UploadChunk, UPLOAD_URL: {0}", address);
            Uri uri = new Uri(address);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = "POST";
            req.ContentType = "application/octet-stream";
            req.Headers.Add("Authorization", "BT " + uploadToken);
            req.Proxy = proxy;

            using (Stream destination = req.GetRequestStream())
            {
                using (FileStream source = File.Open(filePath, FileMode.Open))
                {
                    source.Position = offset;
                    int bytes = Convert.ToInt32(thisChunkSize);
                    byte[] buffer = new byte[CHUNK_SIZE];
                    int read;
                    while (bytes > 0 &&
                           (read = source.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
                    {
                        destination.Write(buffer, 0, read);
                        bytes -= read;
                    }
                }
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                HttpStatusCode statusCode = resp.StatusCode;

                // After sending every chunk upload request to server, response will contain a status indicating if it is complete.
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    string respString = reader.ReadToEnd();
                    Dictionary<string, object> res = (Dictionary<string, object>)serializer.DeserializeObject(respString);
                    log.InfoFormat("The status of chunk upload: {0}", res["status"]);
                }

                resp.Close();

                if (statusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
        }

        // Upload the zip file.
        public string UploadZip(string fileName, string caseNumber, System.Threading.CancellationToken cancel)
        {
            log.InfoFormat("Start to upload bundle {0}", fileName);
            FileInfo fileInfo = new FileInfo(fileName);
            long size = fileInfo.Length;

            // Fetch the upload UUID from CIS server.
            string uploadUuid = InitiateUpload(Path.GetFileName(fileName), size, caseNumber);
            if (string.IsNullOrEmpty(uploadUuid))
            {
                log.ErrorFormat("Cannot fetch the upload UUID from CIS server");
                return "";
            }

            // Start to upload zip file.
            log.InfoFormat("Upload server returns Upload ID: {0}", uploadUuid);

            long offset = 0;
            while (offset < size)
            {
                StringBuilder url = new StringBuilder(UPLOAD_URL + "upload_raw_chunk/?id=" + uploadUuid);
                url.Append(String.Format("&offset={0}", offset));
                long remainingSize = size - offset;
                long thisChunkSize = (remainingSize > CHUNK_SIZE) ? CHUNK_SIZE : remainingSize;

                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (cancel.IsCancellationRequested)
                        {
                            log.Info("Upload cancelled");
                            return "";
                        }

                        if (UploadChunk(url.ToString(), fileName, offset, thisChunkSize, uploadToken))
                        {
                            // This chunk is successfully uploaded
                            offset += thisChunkSize;
                            break;
                        }

                        // Fail to upload the chunk for 3 times so fail the bundle upload.
                        if (i == 2)
                        {
                            log.ErrorFormat("Fail to upload the chunk");
                            return "";
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, e);
                    return "";
                }

            }

            log.InfoFormat("Succeed to upload bundle {0}", fileName);
            return uploadUuid;
            
        }
    }


}
