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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using XenCenterLib.Archive;
using XenCenterLib.Compression;


namespace XenOvf
{
    // Provides the properties of a digest for a file in the manifest of an OVF appliance.
    public class FileDigest
    {
        // Construct from a line in the manifest.
        public FileDigest(string line)
        {
            var fields = line.Split(new char[] { '(', ')', '=' });

            // Expect the first field to be the security algorithm used to compute the hash of the file.
            _AlgorithmName = fields[0].Trim();

            // Expect the second field to be the name of the file.
            Match nameMatch = new Regex(@"[\(](.*)[\)][=]").Match(line);
            _Name = nameMatch.Success ? nameMatch.Groups[1].Value.Trim() : fields[1].Trim();

            // Expect the last field to be the digest of the file.
            _DigestAsString = fields[fields.Length - 1].Trim();
        }


        // Name of the file with the digest.
        public string Name { get { return _Name; } }
        private string _Name;


        // Name of the algorithm to compute the digest.
        // It must be recognized by HashAlgorithm.Create().
        public string AlgorithmName { get { return _AlgorithmName; } }
        private string _AlgorithmName;


        // Algorithm to compute the digest.
        public HashAlgorithm Algorithm
        {
            get
            {
                var hashAlgorithm = HashAlgorithm.Create(_AlgorithmName);

                // Validate the algorithm.
                if (hashAlgorithm == null)
                    throw new ArgumentException(string.Format(Messages.SECURITY_NOT_SUPPORTED, _AlgorithmName));

                return hashAlgorithm;
            }
        }


        // Digest as a binary array.
        public byte[] Digest { get { return ToArray(_DigestAsString); } }
        private string _DigestAsString;
        protected string DigestAsString
        {
            get { return _DigestAsString; }
        }


        public bool WasVerified { get { return _WasVerified; } }
        private bool _WasVerified = false;

        // Convert a hex string to a binary array.
        public static byte[] ToArray(string HexString)
        {
            if ((HexString.Length % 2) > 0)
            {
                // The length is odd.
                // Pad the hex string on the left with a space to interpret as a leading zero.
                HexString = HexString.PadLeft(HexString.Length + 1);
            }

            var array = new byte[HexString.Length / 2];

            try
            {
                for (int i = 0; i < HexString.Length; i += 2)
                {
                    array[i / 2] = byte.Parse(HexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch
            {
                throw new Exception(HexString + " contains an invalid hexadecimal number.");
            }

            return array;
        }


        // Verify the digest of the file content accessible from the given stream by comparing it to the stored digest.
        public void Verify(Stream stream)
        {
            var computedDigest = Algorithm.ComputeHash(stream);

            if (!System.Linq.Enumerable.SequenceEqual(Digest, computedDigest))
            {
                throw new Exception(string.Format(Messages.SECURITY_SIGNATURE_FAILED, Name));
            }

            _WasVerified = true;
        }


        // Verify the digest, when used as a signature, of the file content accessible from the given stream.
        public void Verify(Stream stream, RSACryptoServiceProvider key)
        {
            var computedDigest = Algorithm.ComputeHash(stream);

            if (!key.VerifyHash(computedDigest, CryptoConfig.MapNameToOID(_AlgorithmName), Digest))
            {
                throw new Exception(string.Format(Messages.SECURITY_SIGNATURE_FAILED, Name));
            }
        }
    }


    // OVF Appliance in the form of a folder.
    internal class FolderPackage : Package
    {
        public FolderPackage(string path)
        {
        	PackageSourceFile = path;
            _Folder = path;

            var extension = Path.GetExtension(path);

            if (String.Compare(extension, Properties.Settings.Default.ovfFileExtension, true) == 0)
                _Folder = Path.GetDirectoryName(path);

            if (_Folder == null)
            {
                // Handle a package in the root or current folder.
                _Folder = "";
            }
        }

        private string _Folder;


        public override string DescriptorFileName 
        { 
            get 
            { 
                return Path.GetFileName(PackageSourceFile); 
            } 
        }


        public override bool HasFile(string fileName)
        {
            try
            {
                using (var stream = File.OpenRead(Path.Combine(_Folder, fileName)))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        protected override byte[] ReadAllBytes(string fileName)
        {
            return File.ReadAllBytes(Path.Combine(_Folder, fileName));
        }


        protected override string ReadAllText(string fileName)
        {
            return File.ReadAllText(Path.Combine(_Folder, fileName));
        }


        public override void VerifyManifest()
        {
            // Verify the presence of a manifest.
            var manifest = Manifest;

            // For a folder package, it is efficient to iterate by the order of files in the manifest.
            foreach (FileDigest fileDigest in manifest)
            {
                using (var stream = File.OpenRead(Path.Combine(_Folder, fileDigest.Name)))
                {
                    // Verify each manifest item.
                    fileDigest.Verify(stream);
                }
            }
        }
    }


    // Exposes a stream of entries within a tar for TarPackage.
    internal class TarFileStream : IDisposable /*: Stream */
    {
        // Open a file within the tar.
        public TarFileStream(string tarPath)
        {
            _tarStream = File.OpenRead(tarPath);

            var extension = Path.GetExtension(tarPath);

            // Check for a compressed archive.
            if (String.Compare(extension, ".gz", true) == 0)
            {
                _compressionStream = CompressionFactory.Reader(CompressionFactory.Type.Gz, _tarStream);
                _tarInputStream = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, _compressionStream);
            }
            else if (String.Compare(extension, ".bz2", true) == 0)
            {
                _compressionStream = CompressionFactory.Reader(CompressionFactory.Type.Bz2, _tarStream);
                _tarInputStream = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, _compressionStream);
            }
            else
            {
                _tarInputStream = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, _tarStream);
            }
        }


        // An improvement would be to construct an enumerator for use with foreach.
        public bool FindNextFile(string filePath)
        {
            // Emulate the simple pattern matching of Directory.GetFiles(string path, string pattern)
            var regexPattern = new System.Text.StringBuilder(filePath);

            // Escape any directory separators first.
            regexPattern = regexPattern.Replace("\\", "\\\\");

            // Escape any extension separator second.
            regexPattern = regexPattern.Replace(".", "\\.");

            // Translate any wildcards to their RegEx equivalents.
            regexPattern = regexPattern.Replace("*", ".*");
            regexPattern = regexPattern.Replace("?", ".?");

            var regexPatternAsString = regexPattern.ToString();

            while (MoveNext())
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(Current, regexPatternAsString, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        public byte[] ReadAllBytes()
        {
            MemoryStream ms = new MemoryStream();
            _tarInputStream.ExtractCurrentFile(ms);
            
            if( ms.Length < 1 )
                return null;

            return ms.ToArray();
        }


        // Name of current file in the archive.
        public string Current
        {
            get
            {
                return _tarInputStream.CurrentFileName();
            }
        }


        public bool MoveNext()
        {
            return _tarInputStream.HasNext();
        }


        protected void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                // TarInputStream does not implement Dispose(boolean).
                _tarInputStream.Dispose();
                if (_compressionStream != null)
                    _compressionStream.Dispose();
                _tarStream.Dispose();
            }
        }


        private Stream _tarStream;

        private Stream _compressionStream;

        private ArchiveIterator _tarInputStream;

        public void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Close();
        }

    }


    // OVF Appliance in the form of a tape archive (tar).
    internal class TarPackage : Package
    {
        public TarPackage(string path)
        {
			PackageSourceFile = path;

            _DirectoryCache = new Dictionary<string, byte[]>();
        }
        private Dictionary<string, byte[]> _DirectoryCache;


        public override string DescriptorFileName
        { 
            get 
            {
                if (_DescriptorFileName != null)
                    return _DescriptorFileName;

                Load();

                if (_DescriptorFileName != null)
                {
                    return _DescriptorFileName;
                }

                // Last resort is to search the whole archive.
                var ovfFilePattern = "*" + Properties.Settings.Default.ovfFileExtension;

                using (var stream = OpenRead(ovfFilePattern))
                {
                    _DescriptorFileName = stream.Current;
                }

                return _DescriptorFileName;
            } 
        }
        private string _DescriptorFileName;


        // Open a file within the tar.
        protected TarFileStream OpenRead(string fileName)
        {
            var stream = new TarFileStream(PackageSourceFile);

            if (!stream.FindNextFile(fileName))
            {
                stream.Close();
                throw new FileNotFoundException(fileName);
            }

            return stream;
        }


        public override bool HasFile(string fileName)
        {
            try
            {
                if (_DirectoryCache.ContainsKey(fileName))
                    return true;

                using (var stream = OpenRead(fileName))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        // Wrap calls to TarFileStream.MoveNext() to cache file names for HasFile().
        private bool MoveNext(TarFileStream stream)
        {
            var haveFile = stream.MoveNext();

            if (haveFile)
                _DirectoryCache[stream.Current] = null;

            return haveFile;
        }


        protected override byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer;

            if ((_DirectoryCache.TryGetValue(fileName, out buffer)) && (buffer != null))
            {
                return buffer;
            }

            using (var stream = OpenRead(fileName))
            {
                return (_DirectoryCache[stream.Current] = stream.ReadAllBytes());
            }
        }


        protected override string ReadAllText(string fileName)
        {
            var buffer = ReadAllBytes(fileName);

            using (var stream = new MemoryStream(buffer))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        private void Load()
        {
            using (var stream = new TarFileStream(PackageSourceFile))
            {
                while (MoveNext(stream))
                {
                    var extension = Path.GetExtension(stream.Current);

                    if ((_DescriptorFileName == null) && (String.Compare(extension, Properties.Settings.Default.ovfFileExtension, true) == 0))
                    {
                        // Use the first file name with an ovf extension.
                        _DescriptorFileName = stream.Current;
                    }
                    else if (String.Compare(extension, Properties.Settings.Default.manifestFileExtension, true) == 0)
                    {
                        // Skip a manifest file that does not have the base name as the descriptor.
                        if ((_DescriptorFileName != null) && (String.Compare(stream.Current, ManifestFileName, true) != 0))
                            continue;
                    }
                    else if (String.Compare(extension, Properties.Settings.Default.certificateFileExtension, true) == 0)
                    {
                        // Skip a certificate file that does not have the base name as the descriptor.
                        if ((_DescriptorFileName != null) && (String.Compare(stream.Current, CertificateFileName, true) != 0))
                            continue;
                    }
                    else
                    {
                        // Stop loading because descriptor, manifest, and certificate files must precede all other files.
                        break;
                    }

                    // Load this file.
                    _DirectoryCache[stream.Current] = stream.ReadAllBytes();
                }
            }
        }


        public override void VerifyManifest()
        {
            // Verify the presence of a manifest.
            var manifest = Manifest;

            int verifiedCount = 0;

            using (var stream = new TarFileStream(PackageSourceFile))
            {
                // For a tar package, it is more efficient to iterate the files in the order within the archive.
                while (MoveNext(stream))
                {
                    // Find the manifest entry for the current tar entry.
                    FileDigest fileDigest = manifest.Find(
                        delegate(FileDigest aFileDigest)
                        {
                            return (String.Compare(aFileDigest.Name, stream.Current, true) == 0);
                        });

                    if (fileDigest == null)
                        continue;

                    // The manifest entry was found.
                    // Verify its digest.
                    fileDigest.Verify(new MemoryStream(stream.ReadAllBytes()));

                    verifiedCount++;
                }

                foreach (var fileDigest in manifest)
                {
                    if (!fileDigest.WasVerified)
                    {
                        // A manifest entry was missing.
                        log.ErrorFormat(string.Format(Messages.FILE_MISSING, fileDigest.Name));
                    }
                }

                if (verifiedCount != manifest.Count)
                {
                    // A manifest entry was missing.
                    throw new Exception(Messages.SECURITY_FILE_MISSING);
                }
            }
        }
    }

    
    // Abstract class to access any time of package.
    public abstract class Package
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Package factory.
        public static Package Create(string path)
        {
            var extension = Path.GetExtension(path);

            if (String.Compare(extension, Properties.Settings.Default.ovfFileExtension, true) == 0)
            {
                return new FolderPackage(path);
            }

            // Assume it is an archive.
            return new TarPackage(path);
        }


        // Hide the constructor because only the factory can create a package.
        protected Package() { }


        #region Properties


        // DescriptorFileName is a special property. 
        // Access to the rest of the package depends on it.
        // Abstract because the most efficient method to get it depends on the package type.
        public abstract string DescriptorFileName { get; }

        // According to the OVF specification, name of the *package* is base name of the descriptor file.
        public string Name 
        { 
            get 
            { 
                return Path.GetFileNameWithoutExtension(DescriptorFileName); 
            } 
        }

        // According to the OVF specification, base name of the manifest file must be the same as the descriptor file.
        public string ManifestFileName 
        { 
            get 
            { 
                return (Name + Properties.Settings.Default.manifestFileExtension); 
            } 
        }

        // According to the OVF specification, base name of the certificate file must be the same as the descriptor file.
        public string CertificateFileName { get { return (Name + Properties.Settings.Default.certificateFileExtension); } }


        // Contents of the OVF file.
        public virtual string Descriptor
        {
            get
            {
                if (_Descriptor != null)
                    return _Descriptor;

                return (_Descriptor = ReadAllText(DescriptorFileName));
            }
        }
        // Cache this property because it is expensive to get.
        private string _Descriptor;


        public List<FileDigest> Manifest
        {
            get
            {
                var manifest = new List<FileDigest>();

                using (var stream = new MemoryStream(RawManifest))
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        manifest.Add(new FileDigest(reader.ReadLine()));
                    }
                }

                return manifest;
            }
        }


        protected byte[] RawManifest
        {
            get
            {
                if (_RawManifest != null)
                    return _RawManifest;

                return (_RawManifest = ReadAllBytes(ManifestFileName));
            }
        }
        private byte[] _RawManifest;


        public X509Certificate2 Certificate
        {
            get
            {
                return (new X509Certificate2(RawCertificate));
            }
        }


        protected byte[] RawCertificate
        {
            get
            {
                if (_RawCertificate != null)
                    return _RawCertificate;

                return (_RawCertificate = ReadAllBytes(CertificateFileName));
            }
        }
        private byte[] _RawCertificate;


        public string PackageSourceFile { get; protected set; }


        #endregion


        public virtual bool HasManifest()
        {
            return ((_RawManifest != null) || (HasFile(ManifestFileName)));
        }


        public virtual bool HasSignature()
        {
            return ((_RawCertificate != null) || (HasFile(CertificateFileName)));
        }


        public virtual void VerifySignature()
        {
            // Verify the certificate used to sign the package.
            var certificate = Certificate;

            if (!certificate.Verify())
            {
                throw new Exception(Messages.CERTIFICATE_IS_INVALID);
            }

            // Get the package signature from the certificate file.
            FileDigest fileDigest = null;

            using (Stream stream = new MemoryStream(RawCertificate))
            using (StreamReader reader = new StreamReader(stream))
            {
                // The package signature is the digest of the file identified on the first line in the certificate file.
                fileDigest = new FileDigest(reader.ReadLine());
            }

            // Compare the stored signature to the computed signature.
            // Do this independently to minimize the number of files opened concurrently.
            using (Stream stream = new MemoryStream(RawManifest))
            {
                // Verify the signature using the public key.
                fileDigest.Verify(stream, (RSACryptoServiceProvider)certificate.PublicKey.Key);
            }
        }

        #region Package methods to specialize

        public abstract bool HasFile(string name);

        protected abstract byte[] ReadAllBytes(string fileName);

        protected abstract string ReadAllText(string fileName);

        // Abstract because the file enumerator method varies by package type.
        public abstract void VerifyManifest();

        #endregion
    }
}
