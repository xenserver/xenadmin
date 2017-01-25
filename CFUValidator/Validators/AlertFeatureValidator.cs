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
using XenAdmin.Alerts;

namespace CFUValidator.Validators
{
    abstract class AlertFeatureValidator
    {
        protected List<XenServerPatchAlert> alerts;

        protected AlertFeatureValidator(List<XenServerPatchAlert> alerts)
        {
            this.alerts = alerts;
            Results = new List<string>();
        }

        public abstract void Validate();
        public abstract string Description { get; }
        public List<string> Results { get; protected set; }
        public bool ErrorsFound { get { return Results.Count > 0; } }

        public delegate void StatusChangedHandler(object sender, EventArgs e);

        public event StatusChangedHandler StatusChanged;

        protected virtual void OnStatusChanged()
        {
            if (StatusChanged != null)
                StatusChanged(Status, EventArgs.Empty);
        }

        private string status;
        protected string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnStatusChanged();
            }
        }
    }
}
