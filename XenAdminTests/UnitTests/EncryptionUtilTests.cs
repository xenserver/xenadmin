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
using System.Text;
using System.Security.Cryptography;
using NUnit.Framework;
using XenCenterLib;


namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    internal class EncryptionUtilTests
    {
        private string[] _transformedStrings =
        {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "abcçdefgğhıijklmnoöprsştuüvyz",
            "短𠀁𪛕",
            "ÀàÂâÆæ,Çç,ÉéÈèÊêËë,ÎîÏï,ÔôŒœ,ÙùÛûÜü,Ÿÿ"
        };

        private string[] _keys =
        {
            "kd385Am£dm*C",
            "j33LZ$89cngA39%",
            "Pzm#d&cJ309",
            "短𠀁𪛕",
            "çdefgğhıijklmno",
            "ÀâæÇÉèêëîïÔŒÙÛüŸ"
        };

        [Test]
        public void TestProtectionCurrentUser()
        {
            foreach (var str in _transformedStrings)
            {
                var protectedStr = EncryptionUtils.Protect(str);
                var unprotectedStr = EncryptionUtils.Unprotect(protectedStr);
                Assert.AreEqual(str, unprotectedStr,
                    "String corruption after protection, then un-protection (current user)");
            }
        }

        [Test]
        public void TestProtectionLocalMachine()
        {
            foreach (var str in _transformedStrings)
            {
                var protectedStr = EncryptionUtils.ProtectForLocalMachine(str);
                var unprotectedStr = EncryptionUtils.UnprotectForLocalMachine(protectedStr);
                Assert.AreEqual(str, unprotectedStr,
                    "String corruption after protection, then un-protection (local machine)");
            }
        }

        [Test]
        public void TestProtectionCurrentUserLegacy()
        {
            foreach (var str in _transformedStrings)
            {
                var legacyProtectedStr = LegacyProtection(str, DataProtectionScope.CurrentUser);
                var unprotectedStr = EncryptionUtils.UnprotectForLocalMachine(legacyProtectedStr);
                Assert.AreEqual(str, unprotectedStr,
                    "String corruption after legacy protection, then un-protection (current user)");
            }
        }

        [Test]
        public void TestProtectionLocalMachineLegacy()
        {
            foreach (var str in _transformedStrings)
            {
                var legacyProtectedStr = LegacyProtection(str, DataProtectionScope.LocalMachine);
                var unprotectedStr = EncryptionUtils.UnprotectForLocalMachine(legacyProtectedStr);
                Assert.AreEqual(str, unprotectedStr,
                    "String corruption after legacy protection, then un-protection (local machine)");
            }
        }

        [Test]
        public void TestEncryption()
        {
            foreach (var str in _transformedStrings)
            foreach (var key in _keys)
            {
                var encryptedStr = EncryptionUtils.EncryptString(str, EncryptionUtils.ComputeHash(key));
                var decryptedStr = EncryptionUtils.DecryptString(encryptedStr, key);
                Assert.AreEqual(str, decryptedStr,
                    "String corruption after encryption, then decryption");
            }
        }

        [Test]
        public void TestEncryptionLegacy()
        {
            foreach (var str in _transformedStrings)
            foreach (var key in _keys)
            {
                var legacyEncryptedStr = LegacyEncryption(str, EncryptionUtils.ComputeHash(key));
                var decryptedStr = EncryptionUtils.DecryptString(legacyEncryptedStr, key);
                Assert.AreEqual(str, decryptedStr,
                    "String corruption after legacy encryption, then decryption");
            }
        }


        private static string LegacyProtection(string str, DataProtectionScope scope)
        {
            byte[] saltBytes = new UnicodeEncoding().GetBytes("XenRocks");
            byte[] dataBytes = Encoding.Unicode.GetBytes(str);
            byte[] protectedBytes = ProtectedData.Protect(dataBytes, saltBytes, scope);
            return $"{Convert.ToBase64String(protectedBytes)},{Convert.ToBase64String(saltBytes)}";
        }

        private static string LegacyEncryption(string clearString, byte[] keyBytes)
        {
            byte[] saltBytes = new UnicodeEncoding().GetBytes("XenRocks");

            using (var alg = new AesManaged())
            {
                alg.Key = keyBytes;
                alg.IV = saltBytes;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] clearBytes = Encoding.Unicode.GetBytes(clearString);
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.FlushFinalBlock(); //important for getting the padding right
                    return $"{Convert.ToBase64String(ms.ToArray())},{Convert.ToBase64String(saltBytes)}";
                }
            }
        }
    }
}