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

// A ContextMenuStrip which doesn't reopen immediately after closing.
// This is to fix CA-25239.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace XenAdmin.Controls
{
    public class NonReopeningContextMenuStrip : ContextMenuStrip
    {
        private static TimeSpan Epsilon = TimeSpan.FromMilliseconds(200);  // the time within which we are not allowed to reopen

        private DateTime lastClosedTime = DateTime.MinValue;

        public NonReopeningContextMenuStrip()
            : base()
        {
            InitEvents();
        }

        public NonReopeningContextMenuStrip(IContainer container)
            : base(container)
        {
            InitEvents();
        }

        protected void InitEvents()
        {
            this.Opening += new CancelEventHandler(NonReopeningContextMenuStrip_Opening);
            this.Closed += new ToolStripDropDownClosedEventHandler(NonReopeningContextMenuStrip_Closed);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                Opening -= NonReopeningContextMenuStrip_Opening;
                Closed -= NonReopeningContextMenuStrip_Closed;
            }
            base.Dispose(disposing);
        }

        public bool CanOpen
        {
            get
            {
                TimeSpan timeSpan = DateTime.Now - lastClosedTime;
                return (timeSpan < TimeSpan.Zero || timeSpan > Epsilon);
            }
        }

        private void NonReopeningContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (!CanOpen)
                e.Cancel = true;
        }

        private void NonReopeningContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            lastClosedTime = DateTime.Now;
        }

    }
}
