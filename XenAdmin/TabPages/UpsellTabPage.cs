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

using XenAdmin.Core;


namespace XenAdmin.TabPages
{
    public partial class UpsellTabPage : BaseTabPage
    {
        protected UpsellTabPage(string title, string blurb)
        {
            InitializeComponent();
            base.Text = title;

            BlurbText = blurb;
            LearnMoreUrl = InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL;
        }

        public string BlurbText
        {
            set => upsellPage1.BlurbText = value;
        }

        public string LearnMoreUrl
        {
            set => upsellPage1.LearnMoreUrl = value;
        }
    }

    public class ADUpsellPage : UpsellTabPage
    {
        public ADUpsellPage()
            : base(Messages.ACTIVE_DIRECTORY_TAB_TITLE, string.Format(Messages.UPSELL_BLURB_AD, BrandManager.ProductBrand))
        {
        }

        public override string HelpID => "TabPageADUpsell";
    }

    public class HAUpsellPage : UpsellTabPage
    {
        public HAUpsellPage()
            : base(Messages.HIGH_AVAILABILITY, Messages.UPSELL_BLURB_HA)
        { }

        public override string HelpID => "TabPageHAUpsell";
    }

    public class WLBUpsellPage : UpsellTabPage
    {
        public WLBUpsellPage()
            : base(Messages.WORKLOAD_BALANCING, Messages.UPSELL_BLURB_WLB)
        { }

        public override string HelpID => "TabPageWLBUpsell";
    }
}
