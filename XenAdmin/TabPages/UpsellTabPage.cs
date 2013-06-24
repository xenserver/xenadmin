/* Copyright (c) Citrix Systems Inc. 
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace XenAdmin.TabPages
{
    public partial class UpsellTabPage : BaseTabPage
    {
        public UpsellTabPage(string title, string blurb, string learnMoreUrl)
        {
            InitializeComponent();
            base.Text = title;

            upsellPage1.SetAllTexts(blurb, learnMoreUrl);
        }
    }

    public class BallooningUpsellPage : UpsellTabPage
    {
        public BallooningUpsellPage()
            : base(Messages.DYNAMIC_MEMORY_CONTROL, Messages.UPSELL_BLURB_DMC, InvisibleMessages.UPSELL_LEARNMOREURL_DMC)
        { }
    }

    public class HAUpsellPage : UpsellTabPage
    {
        public HAUpsellPage()
            : base(Messages.HIGH_AVAILABILITY, Messages.UPSELL_BLURB_HA, InvisibleMessages.UPSELL_LEARNMOREURL_HA)
        { }
    }

    public class WLBUpsellPage : UpsellTabPage
    {
        public WLBUpsellPage()
            : base(Messages.WORKLOAD_BALANCING, Messages.UPSELL_BLURB_WLB, InvisibleMessages.UPSELL_LEARNMOREURL_WLB)
        { }
    }
}
