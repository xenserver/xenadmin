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

/*

This file is derived from one in NMock:

Copyright (c) 2002, Joe Walnes, Chris Stevenson, Owen Rogers.  All rights reserved.

Redistribution and use in source and binary forms, with or without 
modification, are permitted provided that the following conditions 
are met:

* Redistributions of source code must retain the above copyright 
  notice, this list of conditions and the following disclaimer. 
  
* Redistributions in binary form must reproduce the above 
  copyright notice, this list of conditions and the following 
  disclaimer in the documentation and/or other materials provided
  with the distribution. 

* The names of the contributors may not be used to endorse or
  promote products derived from this software without specific 
  prior written permission. 
  
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS 
BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.
 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using XenAdmin.Core;

namespace XenAdmin.ServerDBs
{
    public class ClassGenerator
    {
        private const string INVOCATION_HANDLER_FIELD_NAME = "_invocationHandler";

        private const BindingFlags ALL_INSTANCE_METHODS
            = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly List<string> methodsToIgnore = new List<string>();

        static ClassGenerator()
        {
            methodsToIgnore.Add("Equals");
            methodsToIgnore.Add("ToString");
        }

        private readonly Type type;

        public ClassGenerator(Type type)
        {
            this.type = type;
        }

        public Type Generate()
        {
            TypeBuilder typeBuilder = CreateTypeBuilder();
            MethodImplementor methodImplementor =
                new MethodImplementor(typeBuilder, DefineInvocationHandlerField(typeBuilder));

            ImplementMethods(methodImplementor);
            return typeBuilder.CreateType();
        }

        public object CreateProxyInstance(Type proxyType, IInvocationHandler handler)
        {
            object result = Activator.CreateInstance(proxyType);

            FieldInfo handlerField = proxyType.GetField(INVOCATION_HANDLER_FIELD_NAME, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            handlerField.SetValue(result, handler);

            return result;
        }

        private TypeBuilder CreateTypeBuilder()
        {
            AppDomain appDomain = AppDomain.CurrentDomain;
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "DynamicProxyAssembly";
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MockModule");
            return moduleBuilder.DefineType(ProxyClassName, TypeAttributes.Public, ProxySuperClass, ProxyInterfaces);
        }

        private FieldBuilder DefineInvocationHandlerField(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineField(
                INVOCATION_HANDLER_FIELD_NAME,
                typeof(IInvocationHandler), FieldAttributes.Public);
        }

        private void ImplementMethods(MethodImplementor methodImplementor)
        {
            foreach (Type currentType in new InterfaceLister().List(type))
            {
                foreach (MethodInfo methodInfo in currentType.GetMethods(ALL_INSTANCE_METHODS))
                {
                    if (ShouldImplement(methodInfo))
                        methodImplementor.Implement(methodInfo);
                }
            }
        }

        private bool ShouldImplement(MethodInfo methodInfo)
        {
            return methodInfo.IsVirtual && !methodInfo.IsFinal && !methodInfo.IsAssembly &&
                !methodsToIgnore.Contains(methodInfo.Name);
        }

        private string ProxyClassName { get { return "Proxy" + type.Name; } }
        private Type ProxySuperClass { get { return type.IsInterface ? null : type; } }
        private Type[] ProxyInterfaces { get { return type.IsInterface ? new Type[] { type } : new Type[0]; } }
    }

    public class MethodImplementor
    {
        private static readonly Dictionary<Type, OpCode> boxingOpCodes = new Dictionary<Type, OpCode>();

        static MethodImplementor()
        {
            boxingOpCodes[typeof(sbyte)] = OpCodes.Ldind_I1;
            boxingOpCodes[typeof(short)] = OpCodes.Ldind_I2;
            boxingOpCodes[typeof(int)] = OpCodes.Ldind_I4;
            boxingOpCodes[typeof(long)] = OpCodes.Ldind_I8;
            boxingOpCodes[typeof(byte)] = OpCodes.Ldind_U1;
            boxingOpCodes[typeof(ushort)] = OpCodes.Ldind_U2;
            boxingOpCodes[typeof(uint)] = OpCodes.Ldind_U4;
            boxingOpCodes[typeof(ulong)] = OpCodes.Ldind_I8;
            boxingOpCodes[typeof(float)] = OpCodes.Ldind_R4;
            boxingOpCodes[typeof(double)] = OpCodes.Ldind_R8;
            boxingOpCodes[typeof(char)] = OpCodes.Ldind_U2;
            boxingOpCodes[typeof(bool)] = OpCodes.Ldind_I1;
        }

        private TypeBuilder typeBuilder;
        private FieldBuilder handlerFieldBuilder;

        public MethodImplementor(TypeBuilder typeBuilder, FieldBuilder handlerFieldBuilder)
        {
            this.typeBuilder = typeBuilder;
            this.handlerFieldBuilder = handlerFieldBuilder;
        }

        public virtual void Implement(MethodInfo methodInfo)
        {
            Type returnType = methodInfo.ReturnType;
            ParameterInfo[] parameterInfo = methodInfo.GetParameters();
            Type[] paramTypes = ExtractParameterTypes(parameterInfo);
            ILGenerator il = CreateILGenerator(methodInfo, returnType, paramTypes, parameterInfo);

            EmitMethodSignature(methodInfo, returnType, parameterInfo, il);
            EmitMethodCall(il);

            EmitOutParams(parameterInfo, il);

            EmitMethodReturn(returnType, il);
        }

        public virtual void EmitOutParams(ParameterInfo[] parameterInfos, ILGenerator il)
        {
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                //				il.Emit(OpCodes.Ldarg, i+1);
            }
        }

        private Type[] ExtractParameterTypes(ParameterInfo[] parameters)
        {
            Type[] paramTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                paramTypes[i] = parameters[i].ParameterType;
            }
            return paramTypes;
        }

        private ILGenerator CreateILGenerator(MethodInfo methodInfo, Type returnType, Type[] paramTypes, ParameterInfo[] paramInfos)
        {
            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, returnType, paramTypes);

            //			for (int i = 0; i < paramInfos.Length; i++) 
            //			{
            //				if (paramInfos[i].IsOut) 
            //				{
            //					methodBuilder.DefineParameter(i, ParameterAttributes.Out, paramInfos[i].Name);
            //				}
            //			}			

            return methodBuilder.GetILGenerator();
        }

        private void EmitMethodSignature(MethodInfo methodInfo, Type returnType, ParameterInfo[] paramTypes, ILGenerator il)
        {
            il.DeclareLocal(typeof(object));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, handlerFieldBuilder);
            il.Emit(OpCodes.Ldstr, StripGetSetPrefix(methodInfo));
            Type[] ts = returnType.GetGenericArguments();
            il.Emit(OpCodes.Ldstr, ts.Length == 1 ? ts[0].Name : "");  // normally Response<T> and we pick out the T. Couldn't work out how to pass this as a Type not a string.
            il.Emit(OpCodes.Ldc_I4_S, paramTypes.Length);
            il.Emit(OpCodes.Newarr, typeof(object));

            if (paramTypes.Length > 0)
            {
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);

                for (int i = 0; i < paramTypes.Length; i++)
                {

                    il.Emit(OpCodes.Ldc_I4_S, i);
                    il.Emit(OpCodes.Ldarg_S, i + 1);
                    if (paramTypes[i].ParameterType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, paramTypes[i].ParameterType);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                    il.Emit(OpCodes.Ldloc_0);
                }
            }
        }

        private void EmitMethodCall(ILGenerator il)
        {
            MethodInfo call = typeof(IInvocationHandler).GetMethod("Invoke");
            il.EmitCall(OpCodes.Callvirt, call, null);
        }

        private void EmitMethodReturn(Type returnType, ILGenerator il)
        {
            if (returnType == typeof(void))
            {
                il.Emit(OpCodes.Pop);
            }
            else
            {
                if (returnType.IsPrimitive || returnType.IsEnum)
                {
                    il.Emit(OpCodes.Unbox, returnType);
                    il.Emit(GetBoxingOpCode(returnType));
                }
                else if (returnType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox, returnType);
                    il.Emit(OpCodes.Ldobj, returnType);
                }

                il.DeclareLocal(returnType);
                il.Emit(OpCodes.Stloc_1);
                Label l = il.DefineLabel();
                il.Emit(OpCodes.Br_S, l);
                il.MarkLabel(l);
                il.Emit(OpCodes.Ldloc_1);
            }
            il.Emit(OpCodes.Ret);
        }

        private static OpCode GetBoxingOpCode(Type aType)
        {
            return boxingOpCodes.ContainsKey(aType) ? boxingOpCodes[aType] : OpCodes.Ldind_I1;
        }

        private string StripGetSetPrefix(MethodInfo methodInfo)
        {
            string methodName;
            methodName = methodInfo.Name;
            if (methodName.StartsWith("get_") || methodName.StartsWith("set_"))
            {
                methodName = methodName.Substring(4);
            }
            return methodName;
        }
    }
}
