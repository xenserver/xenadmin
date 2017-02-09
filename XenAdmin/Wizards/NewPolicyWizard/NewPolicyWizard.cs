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
using System.Globalization;
using System.Text;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Wizards.GenericPages;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    // This class acts as the base class for NewPolicyWizardSpecific. It's only here
    // because of a bug in Visual Studio: the Designer can't design classes of a
    // generic class. The workaround is to do the design in this non-generic class,
    // and then inherit the generic class from it. See
    // http://stackoverflow.com/questions/1627431/fix-embedded-resources-for-generic-usercontrol
    // http://bytes.com/topic/c-sharp/answers/537310-can-you-have-generic-type-windows-form
    // http://connect.microsoft.com/VisualStudio/feedback/details/115397/component-resource-manager-doesnt-work-with-generic-form-classes
    // (or search on Google for [ComponentResourceManager generic]).

    public partial class NewPolicyWizard : XenWizardBase
    {
        protected NewPolicyPolicyNamePage xenTabPagePolicy;
        protected NewPolicySnapshotFrequencyPage xenTabPageSnapshotFrequency;
        protected NewPolicyArchivePage xenTabPageArchive;
        protected NewPolicyEmailPage xenTabPageEmail;
        protected NewPolicyFinishPage xenTabPageFinish;
        protected RBACWarningPage xenTabPageRBAC;

        public readonly Pool Pool;
        public NewPolicyWizard(Pool pool)
            : base(pool.Connection)
        {
            InitializeComponent();
            Pool = pool;
        }
                        
    }
}
