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
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XenOvf.Definitions;
using XenOvf.Definitions.XENC;
using XenOvf.Utilities;

namespace XenOvf
{
    public partial class OVF
    {
        //TODO: do these need to be configurabe by XenAdmin?
        private const int ENCRYPT_KEY_LENGTH = 192;
        private const string ENCRYPTION_ALGORITHM = "http://www.w3.org/2001/04/xmlenc#aes192-cbc";
        private const string SECURITY_VERSION = "1.3.1";

        // LATIN: No fortification is such that it cannot be subdued with money.
        private const string KnownEncrypt = "Nihil tam munitum quod non expugnari pecunia possit.                                              ";

        private static bool _cancelEncrypt = false;
        private static ulong _position = 0;
        private static ulong _length = 0;

        private const int _KeySize = 0;
        /// <summary>
        /// Set to TRUE to cancel current Encrypt Decrypt operation.
        /// </summary>
        public static bool CancelEncryption
        {
            get
            {
                return _cancelEncrypt;
            }
            set
            {
                Tools.CancelStreamCopy = true;
                _cancelEncrypt = value;
            }
        }
        /// <summary>
        /// Where [bytes] the encryption/decryption operation is.
        /// </summary>
        public static ulong Position
        {
            get
            {
                return _position;
            }
        }
        /// <summary>
        /// How many [bytes] to encrypt/decrypt
        /// </summary>
        public static ulong Length
        {
            get
            {
                return _length;
            }
        }


        #region ENCRYPTION

        /// <summary>
        /// Encrypt the files associated with the provided OVF package.
        /// </summary>
        /// <param name="env">EnvelopeType ovf object</param>
        /// <param name="ovfFileName">fullpath/filename</param>
        /// <param name="password">password to use during encryption.</param>
        public static void Encrypt(EnvelopeType env, string ovfFileName, string password)
        {
            _cancelEncrypt = false;
            CryptoFileWrapper(env, ovfFileName, password, true);
            if (_cancelEncrypt)
            {
                log.Info("Encrypt: CANCELLED successfully.");
            }
            else
            {
                SaveAs(env, ovfFileName);
            }
        }

        /// <summary>
        /// Decrypt the files associated with the provided OVF package.
        /// </summary>
        /// <param name="env">EnvelopeType ovf object</param>
        /// <param name="ovfFileName">fullpath/filename</param>
        /// <param name="password">password to use during decryption.</param>
        public void Decrypt(EnvelopeType env, string ovfFileName, string password)
        {
            _cancelEncrypt = false;
            CryptoFileWrapper(env, ovfFileName, password, false);
            if (_cancelEncrypt)
            {
                log.Info("Encrypt: CANCELLED successfully.");
            }
            else
            {
                SaveAs(env, ovfFileName);
            }
        }
        /// <summary>
        /// Create a file decryption stream to read out an encrypted file.
        /// </summary>
        /// <param name="filename">File to decrypt</param>
        /// <param name="password">password of to file.</param>
        /// <returns>Decrypted Stream</returns>
        public static Stream DecryptFile(string filename, string version, string password)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return CryptoStreamWrapper(fs, password, false, version);
        }
        /// <summary>
        /// Create a file encryption stream to write out an encrypted file.
        /// </summary>
        /// <param name="filename">File to encrypt to.</param>
        /// <param name="password">password of to file.</param>
        /// <returns>Decrypted Stream</returns>
        public Stream EncryptFile(string filename, string version, string password)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            return CryptoStreamWrapper(fs, password, true, version);
        }
        /// <summary>
        /// Decrypt a file to a temporary file.
        /// Action can be cancel via: CancelEncryption = true
        /// </summary>
        /// <param name="classname">encryption class to use must implement: ICryptoTransform ie: System.Security.Cryptography.RijndaelManaged</param>
        /// <param name="filename">Encrypted file name</param>
        /// <param name="password">Password to perform decryption</param>
        /// <param name="tempfile">file to write to.</param>
        public static void DecryptToTempFile(Type cryptoclassType, string filename, string version, string password, string tempfile)
        {
            if (version != null && CheckSecurityVersion(version, SECURITY_VERSION) >= 0)
            {
                using (CryptoStream decryptStream = (CryptoStream)DecryptFile(filename, version, password))
                {
                    using (FileStream fileStream = new FileStream(tempfile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        Tools.StreamCopy(decryptStream, fileStream);
                    }
                }
            }
            else
            {
                // Encryption with issues... original code base.
                ICryptoTransform transform = CryptoSetup(cryptoclassType, password, false, version);
                DeprecatedCryptoFile(transform, filename, tempfile, false);
                if (_cancelEncrypt) File.Delete(tempfile);
            }
        }

        /// <summary>
        /// Checks to see if an OVF is encrypted by checking whether a security section is defined
        /// </summary>
        public static bool HasEncryption(EnvelopeType ovfObj, out SecuritySection_Type[] security)
        {
            security = null;

            if (ovfObj == null)
                return false;
            
            security = FindSections<SecuritySection_Type>(ovfObj.Sections);
            return security != null && security.Length > 0;
        }
        
        public static void ParseEncryption(EnvelopeType ovfObj, out Type cryptoclassType, out string encryptionVersion)
        {
            cryptoclassType = null;
            encryptionVersion = null;

            if (!HasEncryption(ovfObj, out SecuritySection_Type[] securitysection))
                return;

            string fileUuids = "";
                
            foreach (Security_Type securitytype in securitysection[0].Security)
            {
                if (securitytype.ReferenceList.Items != null)
                {
                    foreach (ReferenceType refType in securitytype.ReferenceList.Items)
                    {
                        if (refType is DataReference dataRef)
                            fileUuids += ":" + dataRef.ValueType;
                    }
                }

                cryptoclassType = GetAlgorithmClass(securitytype.EncryptionMethod?.Algorithm);

                if (!string.IsNullOrEmpty(securitytype.version))
                    encryptionVersion = securitytype.version;
            }
        }
        
        /// <summary>
        /// An ovf can contain both encrypted and non-encrypted file mixed together.
        /// find if file name is encrypted.
        /// 1. check the References for the security ID
        /// 2. check the Security id section exists.
        /// </summary>
        /// <param name="ovfObj">OVF Envelope</param>
        /// <param name="filename">filename to check</param>
        /// <returns>true = encrypted; false = not encrypted</returns>
		public static bool IsThisEncrypted(EnvelopeType ovfObj, RASD_Type rasd)
        {
            bool _isEncrypted = false;
            // 15,16,17,19,20 are attached files.
            // rest is RASD specific

            switch (rasd.ResourceType.Value)
            {
                case 15:
                case 16:
                case 17:
                case 19:
                case 20:
                    {
                        File_Type file = FindFileReferenceByRASD(ovfObj, rasd);
                        if (file != null)
                        {
                            if (!string.IsNullOrEmpty(file.Id))
                            {
                                _isEncrypted = IsThisIdEncrypted(ovfObj, file.Id);
                            }
                        }
                        break;
                    }
                default:
                    {
                        // currently encrypted RASD or Elements, isn't being done, but this can check it.
                        if (rasd.AnyAttr != null && rasd.AnyAttr.Length > 0)
                        {
                            foreach (XmlAttribute xa in rasd.AnyAttr)
                            {
                                if (xa.Name.ToLower().Equals("xenc:id"))
                                {
                                    _isEncrypted = IsThisIdEncrypted(ovfObj, xa.Value);
                                    break;
                                }
                            }
                        }
                        break;
                    }
            }
            return _isEncrypted;
        }

        public static bool IsThisIdEncrypted(EnvelopeType ovfObj, string id)
        {
            SecuritySection_Type[] security = FindSections<SecuritySection_Type>(ovfObj.Sections);

            if (security != null && security.Length > 0) // if no security section don't bother going any further.
            {
                foreach (SecuritySection_Type sst in security)
                {
                    foreach (Security_Type st in sst.Security)
                    {
                        foreach (XenOvf.Definitions.XENC.DataReference dataref in st.ReferenceList.Items)
                        {
                            if (!string.IsNullOrEmpty(dataref.ValueType) && dataref.ValueType.Contains(id))
                            {
                                return true;  // no need to go anyfurther, nicer just to leave now.
                            }
                        }
                    }
                }
            }       

            return false;  // get here... its not encrypted.
        }

        /// <summary>
        /// Validate password prior to decrypting, depends on sample encrypted section in The SecuritySection.
        /// </summary>
        /// <param name="ovfObj">EnvelopeType OVF Object</param>
        /// <param name="password">password to check</param>
        /// <returns>true = valid password, false = password failed</returns>
        public bool CheckPassword(EnvelopeType ovfObj, string password)
        {
        	bool isValid = false;          

            SecuritySection_Type[] security = FindSections<SecuritySection_Type>(ovfObj.Sections);

            if (security != null && security.Length == 1)
            {
                foreach (Security_Type sec in security[0].Security)
                {
                    EncryptedDataType edt = null;

                    if (sec.EncryptedData != null && sec.EncryptedData.CipherData != null && sec.EncryptedData.CipherData.Item != null)
                    {
                        edt = sec.EncryptedData;
                    }
                    if (edt == null && sec.Any != null)
                    {
                        foreach (XmlElement xe in sec.Any)
                        {
                            if (xe.Name.Contains(":EncryptedData") || xe.Name.Contains(":EncrypteData"))
                            {
                                CipherDataType cdt = Tools.Deserialize<CipherDataType>(xe.InnerXml);
                                edt = new EncryptedDataType();
                                edt.CipherData = cdt;
                            }
                        }
                    }

                    if (edt != null)
                    {
                        if (sec.version != null && CheckSecurityVersion(sec.version, SECURITY_VERSION) >= 0)
                        {
                            isValid = InternalCheckPassword((byte[])edt.CipherData.Item, password, sec.version);
                        }
                        else
                        {
                            isValid = DeprecatedCheckPassword((byte[])edt.CipherData.Item, password, sec.version);
                        }
                    }
                    else
                    {
                        throw new Exception(Messages.SECURITY_SECTION_INVALID);
                    }
                }

            }
            log.Debug(isValid ? "Password is valid." : "Password is not valid.");
            return isValid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static int CalculateStrength(string password)
        {
            int charSet = 0;
            int passStrength = -1;

            Regex pattern = new Regex(@"[\d]");
            if (pattern.IsMatch(password)) { charSet += 10; }
            pattern = new Regex("[a-z]");
            if (pattern.IsMatch(password)) { charSet += 26; }
            pattern = new Regex("[A-Z]");
            if (pattern.IsMatch(password)) { charSet += 26; }
            pattern = new Regex(@"[\W|_]");
            if (pattern.IsMatch(password)) { charSet += 31; }

            double result = Math.Log(Math.Pow(charSet, password.Length)) / Math.Log(2);

            if (result <= 32) { passStrength = 0; }         //= "Low;"; }
            else if (result <= 64) { passStrength = 1; }    //= "Fair;"; }
            else if (result <= 128) { passStrength = 2; }   //= "Good;"; }
            else if (result > 128) { passStrength = 3; }    //= "Strong;"; }

            return passStrength;
        }       
        #endregion

        #region PRIVATE

        private static Type GetAlgorithmClass(string key)
        {
            string algorithm = "";

            if (!string.IsNullOrEmpty(key))
            {
                string[] parts = key.Split('#');

                if (parts.Length > 1)
                    algorithm = parts[1].ToLower().Replace('-', '_');
            }

            switch (algorithm)
            {
                case "base64":
                    return typeof(FromBase64Transform);
                case "rsa_1_5":
                case "rsa_oaep_mgf1p":
                    return typeof(RSACryptoServiceProvider);
                case "tripledes_cbc":
                case "kw_tripledes":
                    return typeof(TripleDESCryptoServiceProvider);
                case "sha1":
                    return typeof(SHA1CryptoServiceProvider);
                case "sha256":
                    return typeof(SHA256CryptoServiceProvider);
                case "sha384":
                    return typeof(SHA384CryptoServiceProvider);
                case "sha512":
                    return typeof(SHA512CryptoServiceProvider);
                case "des":
                    return typeof(DESCryptoServiceProvider);
                case "rc2":
                    return typeof(RC2CryptoServiceProvider);
                case "kw_aes128":
                case "kw_aes256":
                case "kw_aes192":
                case "aes128_cbc":
                case "aes256_cbc":
                case "aes192_cbc":
                default:
                    return typeof(RijndaelManaged);
            }
        }

        private static void CryptoFileWrapper(EnvelopeType env, string ovffilename, string password, bool encrypt)
        {
            bool process = true;

            if ((env.References == null) ||
                (env.References.File == null) ||
                (env.References.File.Length == 0))
            {
                log.Info("OVF.Security: No files to encrypt/decrypt.");
                return;
            }
            try
            {
                List<DataReference> dataReference = new List<DataReference>();
                Type cryptoclassType = GetAlgorithmClass(ENCRYPTION_ALGORITHM);
                int keysize = ENCRYPT_KEY_LENGTH;
                string fileuuids = null;
                string version = null;
                //
                // Initial version really only works when there is ONLY ONE SecuritySection::Security
                //
                #region GET DECRYPT INFO
                if (!encrypt)
                {
                    SecuritySection_Type securitysection = null;
                    foreach (Section_Type section in env.Sections)
                    {
                        if (section is SecuritySection_Type)
                        {
                            securitysection = (SecuritySection_Type)section;
                            break;
                        }
                    }

                    if (securitysection != null)
                    {
                        foreach (Security_Type securitytype in securitysection.Security)
                        {
                            foreach (XenOvf.Definitions.XENC.ReferenceType dataref in securitytype.ReferenceList.Items)
                            {
                                if (dataref is DataReference)
                                {
                                    fileuuids += ":" + ((DataReference)dataref).ValueType;
                                }
                            }
                            if (securitytype.EncryptionMethod != null &&
                                securitytype.EncryptionMethod.Algorithm != null)
                            {
                                cryptoclassType = GetAlgorithmClass(securitytype.EncryptionMethod.Algorithm);
                                keysize = Convert.ToInt32(securitytype.EncryptionMethod.KeySize);
                            }
                            if (!string.IsNullOrEmpty(securitytype.version))
                            {
                                version = securitytype.version;
                            }
                        }
                    }
                }
                #endregion

                #region PROCESS FILES
                foreach (File_Type file in env.References.File)
                {
                    if (encrypt)
                    {
                        version = SECURITY_VERSION;
                        if (file.Id == null)
                        {
                            file.Id = "xen_" + Guid.NewGuid().ToString();
                            DataReference newDataReference = new DataReference();
                            newDataReference.ValueType = file.Id;
                            dataReference.Add(newDataReference);
                            process = true;
                        }
                        else
                        {
                            log.InfoFormat("File already encrypted, skipping. [{0}]", file.href);
                            process = false;
                        }
                    }
                    else
                    {
                        if (file.Id != null &&
                            fileuuids != null &&
                            fileuuids.ToLower().Contains(file.Id.ToLower()))
                        {
                            process = true;
                            file.Id = null;
                        }
                        else
                        {
                            process = false;
                            log.InfoFormat("File is not encrypted, skipping. [{0}]", file.href);
                        }
                    }

                    if (process)
                    {
                        string fullname = string.Format(@"{0}\{1}", Path.GetDirectoryName(ovffilename), file.href);
                        log.DebugFormat(encrypt ? "Encrypt: {0}" : "Decrypt: {0}", fullname);
                        ICryptoTransform trans = CryptoSetup(cryptoclassType, password, encrypt, version);                        
                        CryptoFile(trans, fullname, fullname + ".tmp", encrypt);
                        if (_cancelEncrypt)
                        {
                            File.Delete(fullname + ".tmp");
                        }
                        else
                        {
                            File.Delete(fullname);
                            File.Move(fullname + ".tmp", fullname);
                        }
                    }
                }
                #endregion

                #region BUILD SECURITY SECTION
                if (encrypt && process && !_cancelEncrypt)
                {
                    List<Section_Type> sections = new List<Section_Type>();
                    SecuritySection_Type securitySection = null;

                    foreach (Section_Type section in env.Sections)
                    {
                        if (section is SecuritySection_Type)
                        {
                            securitySection = (SecuritySection_Type)section;
                        }
                        else
                        {
                            sections.Add(section);
                        }
                    }

                    if (securitySection == null)
                    {
                        securitySection = new SecuritySection_Type();
                        securitySection.Info = new Msg_Type();
                        securitySection.Info.Value = "Encrypted Content Definition";
                    }

                    List<Security_Type> secType = new List<Security_Type>();
                    if (securitySection.Security != null && securitySection.Security.Length > 0)
                    {
                        secType.AddRange(securitySection.Security);
                    }

                    Security_Type securityType = new Security_Type();
                    securityType.version = SECURITY_VERSION;
                    securityType.Id = "xen_" + Guid.NewGuid().ToString();
                    ReferenceList referenceList = new ReferenceList();
                    referenceList.Items = dataReference.ToArray();
                    List<ItemsChoiceType3> ictList = new List<ItemsChoiceType3>();
                    for (int i = 0; i < dataReference.Count; i++)
                    {
                        ictList.Add(ItemsChoiceType3.DataReference);
                    }
                    referenceList.ItemsElementName = ictList.ToArray();
                    EncryptionMethodType encryptMethod = new EncryptionMethodType();
                    encryptMethod.KeySize = Convert.ToString(_KeySize);
                    encryptMethod.Algorithm = ENCRYPTION_ALGORITHM;

                    EncryptedDataType EncryptedData = new EncryptedDataType();
                    EncryptedData.CipherData = new CipherDataType();

                    CryptoElement(EncryptedData, KnownEncrypt, cryptoclassType, version, password);

                    securityType.ReferenceList = referenceList;
                    securityType.EncryptionMethod = encryptMethod;
                    securityType.EncryptedData = EncryptedData;


                    secType.Add(securityType);
                    securitySection.Security = secType.ToArray();
                    sections.Add(securitySection);
                    env.Sections = sections.ToArray();
                }
                #endregion
            }
            catch (Exception ex)
            {
                log.ErrorFormat("OVF.Security: Cryptography error: {0}", ex.Message);
                throw;
            }
        }
        private static CryptoStream CryptoStreamWrapper(Stream inputStream, string password, bool encrypt, string version)
        {
            try
            {
                Type cryptoclassType = GetAlgorithmClass(ENCRYPTION_ALGORITHM);
                ICryptoTransform trans = CryptoSetup(cryptoclassType, password, encrypt, version);
                return CryptoStream1(trans, inputStream, encrypt);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static ICryptoTransform CryptoSetup(Type cryptoclassType, string password, bool encrypt, string version)
        {

            log.DebugFormat("CryptoSetup: using {0}", cryptoclassType);
            SymmetricAlgorithm cryptObject = null;
            try
            {
                cryptObject = (SymmetricAlgorithm)Activator.CreateInstance(cryptoclassType);
                if (!string.IsNullOrEmpty(version) && CheckSecurityVersion(version, SECURITY_VERSION) >= 0)
                {
                    cryptObject.Padding = PaddingMode.PKCS7;
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Encryption class error: {0}", ex.Message);
                throw;
            }

            if (cryptObject == null)
            {
                log.Error("Encryption class could not be created");
                throw new ArgumentNullException();
            }

            // OLD Initializers.
            byte[] Key = new byte[24];
            byte[] IV = new byte[16];

            if (!string.IsNullOrEmpty(version) && (CheckSecurityVersion(version, "1.3") >= 0))
            {
                Key = new byte[cryptObject.Key.Length];
                IV = new byte[cryptObject.IV.Length];
            }

            GenerateKey(password, ref Key, ref IV);

            password = string.Empty;

            cryptObject.Key = Key;
            cryptObject.IV = IV;

            ICryptoTransform transform = null;
            if (encrypt)
            {
                transform = cryptObject.CreateEncryptor(cryptObject.Key, cryptObject.IV);
            }
            else
            {
                transform = cryptObject.CreateDecryptor(cryptObject.Key, cryptObject.IV);
            }
            return transform;
        }
        private static void CryptoFile(ICryptoTransform transform, string fullPathToFileName, string targetfile, bool encrypt)
        {
            FileStream inputFile = new FileStream(fullPathToFileName, FileMode.Open, FileAccess.Read, FileShare.None);
            FileStream outputFile = new FileStream(targetfile, FileMode.Create, FileAccess.Write, FileShare.None);
            CryptoStream cryptostream = null;
            OnChanged(new OvfEventArgs(OvfEventType.Start, "Crypto: Start: ", fullPathToFileName, (ulong)0, (ulong)inputFile.Length));
            _length = (ulong)inputFile.Length;

            if (encrypt)
            {
                cryptostream = CryptoStream1(transform, outputFile, encrypt);
                Tools.StreamCopy(inputFile, cryptostream);
                cryptostream.FlushFinalBlock();
                cryptostream.Flush();
            }
            else
            {
                cryptostream = CryptoStream1(transform, inputFile, encrypt);
                Tools.StreamCopy(cryptostream, outputFile);
                outputFile.Flush();
            }

            OnChanged(new OvfEventArgs(OvfEventType.End, "Crypto: Completed", fullPathToFileName, (ulong)0, (ulong)inputFile.Length));
            inputFile.Dispose();
            outputFile.Dispose();
        }
        private static void DeprecatedCryptoFile(ICryptoTransform transform, string fullPathToFileName, string targetfile, bool encrypt)
        {
            FileStream inputFile = new FileStream(fullPathToFileName, FileMode.Open, FileAccess.Read, FileShare.None);
            FileStream outputFile = new FileStream(targetfile, FileMode.Create, FileAccess.Write, FileShare.None);
            CryptoStream cryptostream = new CryptoStream(outputFile, transform, CryptoStreamMode.Write);
            byte[] inputarray = new byte[MB * 2];
            int currentRead = 0;
            int totalRead = 0;

            OnChanged(new OvfEventArgs(OvfEventType.Start, "Crypto: Start: ", fullPathToFileName, (ulong)0, (ulong)inputFile.Length));
            _length = (ulong)inputFile.Length;
            while (true)
            {
                currentRead = inputFile.Read(inputarray, 0, inputarray.Length);
                if (currentRead == 0) break;
                cryptostream.Write(inputarray, 0, currentRead);
                totalRead += currentRead;
                OnChanged(new OvfEventArgs(OvfEventType.Progress, "Crypto: Continue", fullPathToFileName, (ulong)totalRead, (ulong)inputFile.Length));
                _position = (ulong)totalRead;
                if (totalRead >= inputFile.Length) break;
                if (_cancelEncrypt) break;
            }
            cryptostream.Flush();
            OnChanged(new OvfEventArgs(OvfEventType.End, "Crypto: Completed", fullPathToFileName, (ulong)totalRead, (ulong)inputFile.Length));
            // Not sure 'why' but it appears that the transform might not flush the last
            // 16 bytes.
            if (!encrypt && !_cancelEncrypt)
            {
                if (inputFile.Length > outputFile.Length && !_cancelEncrypt)
                {
                    byte[] missing = new byte[inputFile.Length - outputFile.Length];
                    log.WarnFormat("PADDING Unencrypted VHD, with {0} zeros", (inputFile.Length - outputFile.Length));
                    outputFile.Write(missing, 0, missing.Length);
                }
            }
            inputFile.Dispose();
            outputFile.Flush();
            outputFile.Dispose();
        }
        private bool InternalCheckPassword(byte[] bytearray, string password, string version)
        {
            bool isValid = false;
            MemoryStream ms = new MemoryStream(bytearray);
            CryptoStream checkStream = CryptoStreamWrapper(ms, password, false, version);
            byte[] buff = new byte[bytearray.Length];
            try
            {
                checkStream.Read(buff, 0, (int)bytearray.Length);
                Encoding uni = new UnicodeEncoding();
                string original = uni.GetString(buff, 0, buff.Length);
                original = original.Trim(new char[] { ' ', '\0' });
                if (original == KnownEncrypt.Trim()) { isValid = true; }
                checkStream.Dispose();
            }
            catch (CryptographicException ce)
            {
                // If we get here the password is considered invalid
                log.DebugFormat("InternalCheckPassword: Invalid password. {0}", ce.Message);                    
            }
            return isValid;
        }
        private bool DeprecatedCheckPassword(byte[] bytearray, string password, string version)
        {
            bool isValid = false;
            MemoryStream ms = new MemoryStream(bytearray);
            MemoryStream os = new MemoryStream();
            Stream checkStream = CryptoStreamWrapper(ms, password, false, version);
            while (true)
            {
                int r = -1;
                try
                {
                    r = checkStream.ReadByte();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("CRYPTO Error: {0}", ex.Message);
                    break;
                }
                if (r == -1)
                    break;
                os.WriteByte((byte)r);
            }
            os.Position = 0;
            StreamReader sr = new StreamReader(os);
            string original = sr.ReadToEnd(); ;
            if (original.Trim() == KnownEncrypt.Trim()){ isValid = true; }
            return isValid;
        }
        private static int CheckSecurityVersion(string version, string against)
        {
            int rtnvalue = 0;
            string[] tstversion = version.Split(new char[] { '.' });
            string[] curversion = against.Split(new char[] { '.' });

            int tstRelease = 0;
            int tstMajor = 0;
            int tstMinor = 0;
            int curRelease = 0;
            int curMajor = 0;
            int curMinor = 0;

            if (tstversion.Length >= 1)
                tstRelease = Convert.ToInt32(tstversion[0]);
            if (tstversion.Length >= 2)
                tstMajor = Convert.ToInt32(tstversion[1]);
            if (tstversion.Length >= 3)
                tstMinor = Convert.ToInt32(tstversion[1]);

            if (curversion.Length >= 1)
                curRelease = Convert.ToInt32(tstversion[0]);
            if (curversion.Length >= 2)
                curMajor = Convert.ToInt32(tstversion[1]);
            if (curversion.Length >= 3)
                curMinor = Convert.ToInt32(tstversion[1]);

            if (tstRelease < curRelease) { rtnvalue = -1; }
            else if (tstRelease > curRelease) { rtnvalue = 1; }
            else if (tstMajor < curMajor) { rtnvalue = -1; }
            else if (tstMajor > curMajor) { rtnvalue = 1; }
            else if (tstMinor < curMinor) { rtnvalue = -1; }
            else if (tstMinor > curMinor) { rtnvalue = 1; }

            return rtnvalue;
        }

        private static void CryptoElement(EncryptedDataType element, string original, Type cryptoclassType, string version, string password)
        {
            Encoding encoding = new UnicodeEncoding();
            MemoryStream ms = new MemoryStream();
            CryptoStream crypted = CryptoStream1(CryptoSetup(cryptoclassType, password, true, version), ms, true);
            byte[] bytes = encoding.GetBytes(original);
            crypted.Write(bytes, 0, bytes.Length);
            crypted.FlushFinalBlock();

            ms.Position = 0;
            byte[] CipherValue = new byte[ms.Length];
            ms.Read(CipherValue, 0, CipherValue.Length);
            element.CipherData.Item = CipherValue;

            crypted.Dispose();
        }
        private CryptoStream CryptoStream1(ICryptoTransform transform, string fullPathToFileName, bool encrypt)
        {
            FileStream inputFile = new FileStream(fullPathToFileName, FileMode.Open, FileAccess.Read, FileShare.None);
            return CryptoStream1(transform, inputFile, encrypt);
        }
        private static CryptoStream CryptoStream1(ICryptoTransform transform, Stream inputStream, bool encrypt)
        {
            return new CryptoStream(inputStream, transform, encrypt ? CryptoStreamMode.Write : CryptoStreamMode.Read);
        }
        private static void GenerateKey(string SecretPhrase, ref byte[] key, ref byte[] iv)
        {
            // Initialize internal values

            // Perform a hash operation using the phrase.  This will 
            // generate a unique 32 character value to be used as the key.
            byte[] bytePhrase = Encoding.ASCII.GetBytes(SecretPhrase);
            SHA384Managed sha384 = new SHA384Managed();
            sha384.ComputeHash(bytePhrase);
            byte[] result = sha384.Hash;

            for (int loop = 0; loop < key.Length; loop++)
                key[loop] = result[loop];
            for (int loop = key.Length; loop < (key.Length + iv.Length); loop++)
                iv[loop - key.Length] = result[loop];
        }
        #endregion
    }
}
