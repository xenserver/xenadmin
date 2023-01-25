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
using System.IO;
using XenCenterLib.Compression;
using XenOvf.Definitions;


namespace XenOvf
{
    public partial class OVF
    {
        public static void CompressFiles(EnvelopeType env, string ovfPath, CompressionFactory.Type method, Action cancellingDelegate)
        {
            ProcessCompression(env, ovfPath, true, cancellingDelegate, method);
            SaveAs(env, ovfPath);
        }

        public static void UncompressFiles(EnvelopeType env, string ovfPath, Action cancellingDelegate)
        {
            ProcessCompression(env, ovfPath, false, cancellingDelegate);
            SaveAs(env, ovfPath);
        }

        private static void ProcessCompression(EnvelopeType env, string ovfPath, bool compress,
            Action cancellingDelegate, CompressionFactory.Type method = CompressionFactory.Type.Gz)
        {
            if (env.References?.File == null)
                return;

            string path = Path.GetDirectoryName(ovfPath);

            foreach (File_Type file in env.References.File)
            {
                if (!compress)
                {
                    if (file.compression == null)
                    {
                        log.InfoFormat("File {0} was not marked as compressed, skipped.", file.href);
                        continue;
                    }

                    if (file.compression.ToLower() == "gzip")
                        method = CompressionFactory.Type.Gz;
                    else
                    {
                        log.ErrorFormat("File {0} uses unsupported method {1}. Must be Gzip. Skipping.",
                            file.href, file.compression);
                        continue;
                    }
                }

                int slash = file.href.LastIndexOf('/');
                string stem = slash >= 0 ? file.href.Substring(0, slash + 1) : "";
                string filePath = Path.Combine(path,  slash >= 0 ? file.href.Substring(slash + 1) : file.href);
                string tempfile = Path.Combine(path, Path.GetRandomFileName());

                try
                {
                    if (compress)
                        CompressionFactory.CompressFile(filePath, tempfile, method, cancellingDelegate);
                    else
                        CompressionFactory.UncompressFile(filePath, tempfile, method, cancellingDelegate);

                    File.Delete(filePath);
                    var ext = method.FileExtension();

                    if (compress)
                        filePath += ext;
                    else if (filePath.EndsWith(ext))
                        filePath = filePath.Substring(0, filePath.Length - ext.Length);

                    File.Move(tempfile, filePath);
                    file.href = stem + Path.GetFileName(filePath);
                    file.compression = compress ? method.StringOf() : null;
                    FileInfo fi = new FileInfo(filePath);
                    file.size = (ulong)fi.Length;
                }
                catch (EndOfStreamException eose)
                {
                    log.Error("End of Stream: ", eose);
                }
                finally
                {
                    try
                    {
                        File.Delete(tempfile);
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
        }
    }
}
