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
using XenAdmin.Network;

namespace XenAPI
{
    public partial interface IXenObject : IEquatable<IXenObject>, IComparable
    {
        Object Get(String property);
        void Set(String property, object val);
        void Do(String method, params Object[] methodParams);

        String Path { get; set; }

        string Name { get; }

        IXenConnection Connection { get; set; }

        bool Locked { get; set; }

        /// <summary>
        /// Whether the HideFromXenCenter other_config key is set.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Whether the object should be shown in general (for example, VDIs with managed=false are never shown in the UI).
        /// This value obeys the View > Hidden Objects menu item, so if that item is checked, then more things will return true here.
        /// </summary>
        bool Show(bool showHiddenVMs);

        IXenObject Clone();

        string SaveChanges(Session session);
        string SaveChanges(Session session, IXenObject beforeObject);

        string Description { get; }
        string NameWithLocation { get; }
    }
}
