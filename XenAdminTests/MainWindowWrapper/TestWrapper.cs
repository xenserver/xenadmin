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
using System.Reflection;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;

namespace XenAdminTests
{
    /// <summary>
    /// A class for exposing private fields of classes. This avoids having to make fields public
    /// in the production code, therefore maintaining encapsulation while allowing testing.
    /// 
    /// Look in LicensingTests.LicenseSummaryWrapper for example usage.
    /// </summary>
    /// <typeparam name="TClass">The type of the wrapped class.</typeparam>
    internal abstract class TestWrapper<TClass> where TClass : class
    {
        private readonly TClass _item;

        protected TestWrapper(TClass item)
        {
            Util.ThrowIfParameterNull(item, "item");
            _item = item;
        }

        protected TestWrapper(IWin32Window window)
            : this(GetControlFromWindow(window))
        {
        }

        private static TClass GetControlFromWindow(IWin32Window window)
        {
            var control = Control.FromHandle(window.Handle) as TClass;

            if (control == null)
                throw new ArgumentException($"window is not a {typeof(TClass).Name} window");

            return control;
        }

        /// <summary>
        /// Gets the value of the private field from the wrapped class of the specified name.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="name">The name of the private field.</param>
        /// <returns>The value of the private field from the wrapped class of the specified name.</returns>
        protected TField GetField<TField>(string name)
        {
            Util.ThrowIfStringParameterNullOrEmpty(name, "name");

            try
            {
                return (TField)_item.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_item);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Field {0} of {1} throws {2}.", name, typeof(TClass).Name, e.GetType().Name));
                throw;
            }
        }

        /// <summary>
        /// Gets the value of a given private field from the wrapped class's *base* class
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="name">The name of the field.</param>
        /// <returns>The value of the private field from the base class of the wrapped class of the specified name.</returns>
        protected TField GetBaseClassField<TField>(string name)
        {
            Util.ThrowIfStringParameterNullOrEmpty(name, "name");

            try
            {
                Type baseType = _item.GetType().BaseType;

                Assert.NotNull(baseType, $"{name} is a field of a class that has no base class");

                return (TField)baseType.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_item);
            }
            catch (Exception e)
            {
                Assert.Fail($"Field {name} of {typeof(TClass).Name} throws {e.GetType().Name}.");
                throw;
            }
        }

        /// <summary>
        /// Gets the wrapped object.
        /// </summary>
        public TClass Item
        {
            get { return _item; }
        }

        /// <summary>
        /// Executes the private method with the specified name from the wrapped class.
        /// </summary>
        /// <param name="name">The name of the private method.</param>
        /// <param name="parameters">The parameters of the private method.</param>
        protected object ExecuteMethod(string name, object[] parameters)
        {
            return _item.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_item, parameters);
        }

        /// <summary>
        /// Executes the private method with the specified name from the wrapped class.
        /// </summary>
        /// <param name="name">The name of the private method.</param>
        /// <param name="types">The types of the parameters of the private method.</param>
        /// <param name="parameters">The parameters of the private method.</param>
        protected object ExecuteMethod(string name, Type[] types, object[] parameters)
        {
            return _item.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic, null, types, null).Invoke(_item, parameters);
        }
    }
}
