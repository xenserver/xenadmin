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
using System.Reflection;
using NUnit.Framework;

namespace XenAdminTests.UnitTests.UnitTestHelper
{
    public class VerifyStaticMethodReturn : IUnitTestVerifier
    {
        private readonly Type typeTovalidate;

        public VerifyStaticMethodReturn(Type typeTovalidate)
        {
            this.typeTovalidate = typeTovalidate;
        }

        /// <summary>
        /// Use this class to go through a class with the constructed type and 
        /// verify there are methods that return the expected type and the
        /// return value is not null
        /// </summary>
        /// <param name="returnTypeForMethod">Type that the static method will check for as a return type</param>
        public void Verify( object returnTypeForMethod )
        {
            Type returnType = returnTypeForMethod as Type;
            MethodInfo[] methods = typeTovalidate.GetMethods();
            
            Assert.AreNotEqual(0, methods.Length, "No methods found returning " + returnType );

            int staticMethodCount = 0;
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.ReturnType == returnType && methodInfo.IsStatic)
                {
                    Assert.IsNotNull(methodInfo.Invoke(null, null), "Null value returned for " + methodInfo.Name);
                    staticMethodCount++;
                }
            }

            Assert.AreNotEqual(0, staticMethodCount, "Static method call count should not be zero");
        }
    }
}
