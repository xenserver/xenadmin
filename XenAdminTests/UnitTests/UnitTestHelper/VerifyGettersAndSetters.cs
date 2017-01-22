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

using System.Reflection;
using NUnit.Framework;

namespace XenAdminTests.UnitTests.UnitTestHelper
{
    public class VerifyGettersAndSetters : IUnitTestVerifier
    {
        private readonly object classToVerify = null;

        /// <param name="classToVerify">A class with getters and setters to verify</param>
        public VerifyGettersAndSetters(object classToVerify)
        {
            this.classToVerify = classToVerify;
        }

        /// <summary>
        /// Use this when you want to test that some data is set and the same data is retrieved afterwards
        /// </summary>
        /// <param name="expected">A struct of expected data content with members having the same name and return type as the getters and setters of the class</param>
        public void Verify(object expected)
        {
            VerifySettersAndGetters(expected, expected);
        }

        /// <summary>
        /// Use this when you want to test that some data is set and a set of different data is retrieved afterwards.
        /// This is useful when your class manipulates the underlying data before returning/storing it
        /// </summary>
        /// <param name="expected">A struct of expected data content with members having the same name and return type as the getters and setters of the class</param>
        /// <param name="toSet">A struct of data to set with members having the same name and return type as the getters and setters of the class</param>
        public void VerifySettersAndGetters(object expected, object toSet)
        {
            FieldInfo[] fields = expected.GetType().GetFields();
            foreach (FieldInfo fieldInfo in fields)
            {
                object valueToSet = fieldInfo.GetValue(toSet);
                string fieldName = fieldInfo.Name;

                PropertyInfo setter = classToVerify.GetType().GetProperty(fieldName);
                setter.SetValue(classToVerify, valueToSet, null);

                object expectedValue = fieldInfo.GetValue(expected);
                PropertyInfo getter = classToVerify.GetType().GetProperty(fieldName);
                Assert.AreEqual(expectedValue, getter.GetValue(classToVerify, null), "Property mismatched: " + fieldName);
            }
        }
    }
}
