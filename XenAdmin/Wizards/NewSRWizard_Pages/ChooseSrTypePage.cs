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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using Message = System.Windows.Forms.Message;

namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public partial class ChooseSrTypePage : XenTabPage
    {
        private bool m_allowNext = true;
        private int _matchingFrontends;
        private Type m_preselectedWizardType = typeof(SrWizardType_VhdoNfs);

        public ChooseSrTypePage()
        {
            InitializeComponent();

            // Store the SrWizardType on the tag of their RadioButton
            radioButtonNfs.Tag = new SrWizardType_VhdoNfs();
            radioButtonIscsi.Tag = new SrWizardType_LvmoIscsi();
            radioButtonFibreChannel.Tag = new SrWizardType_LvmoHba();
            radioButtonNfsIso.Tag = new SrWizardType_NfsIso();
            radioButtonCifsIso.Tag = new SrWizardType_CifsIso();
            radioButtonCslg.Tag = new SrWizardType_Cslg();
            radioButtonCifs.Tag = new SrWizardType_Cifs();
            radioButtonFcoe.Tag = new SrWizardType_Fcoe();
        }

        private void SetupDeprecationBanner(bool visible)
        {
            if(visible)
            {
                deprecationBanner.AppliesToVersion = Messages.XENSERVER_6_5;
                deprecationBanner.BannerType = DeprecationBanner.Type.Removal;
                deprecationBanner.FeatureName = Messages.ISL_SR;
                deprecationBanner.LinkUri = new Uri(InvisibleMessages.ISL_DEPRECATION_URL);
                deprecationBanner.Visible = true;
            }
            else
                deprecationBanner.Visible = false;
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.TYPE; } }

        public override string PageTitle { get { return Messages.CHOOSE_SR_TYPE_PAGE_TITLE; } }

        public override string HelpID { get { return "Type"; } }

        public override void PopulatePage()
        {
            radioButtonCslg.Visible = !Helpers.CreedenceOrGreater(Connection); //Hide iSL radio button for Creedence or higher (StorageLink is not supported)

            radioButtonCifs.Visible = !Helpers.FeatureForbidden(Connection, Host.RestrictCifs);

            radioButtonFcoe.Visible = Helpers.DundeeOrGreater(Connection);

            foreach (var radioButton in RadioButtons)
            {
                var frontend = (SrWizardType)radioButton.Tag;
                frontend.ResetSrName(Connection);
                frontend.AllowToCreateNewSr = SrToReattach == null && !DisasterRecoveryTask;
                frontend.DisasterRecoveryTask = DisasterRecoveryTask;
                frontend.SrToReattach = SrToReattach;
            }

            _matchingFrontends = 0;

            if (Connection == null)
            {
                // disable all except CSLG
                Array.ForEach(RadioButtons, r => r.Enabled = r.Checked = ((SrWizardType)r.Tag).Type == SR.SRTypes.cslg);
                _matchingFrontends = 1;
                return;
            }

            if (SrToReattach == null)
            {
                // CA-21758: Use SM.other_config HideFromXenCenter flag to hide backends in the New SR wizard
                // Only do this when we're doing a create.

                if (!Properties.Settings.Default.ShowHiddenVMs)
                {
                    foreach (RadioButton radioButton in RadioButtons)
                    {
                        SrWizardType wizardType = (SrWizardType)radioButton.Tag;

                        SM sm = SM.GetByType(Connection, wizardType.Type.ToString());

                        if (sm != null && sm.IsHidden)
                            radioButton.Visible = false;
                    }
                }

                foreach (RadioButton radioButton in RadioButtons)
                {
                    if (radioButton.Visible && radioButton.Tag.GetType() == m_preselectedWizardType)
                        radioButton.Checked = true;
                }
            }
            else
            {
                // If we're reconfiguring then try and select the correct page.  If we find more than one matching frontend
                // (ISO SRs - CA-19605) then just disable non-matching ones and leave it at the first page

                foreach (RadioButton radioButton in RadioButtons)
                {
                    SrWizardType wizardType = (SrWizardType)radioButton.Tag;

                    if (wizardType.Type.ToString() == SrToReattach.type)
                    {
                        if (_matchingFrontends == 0)
                            radioButton.Checked = true;
                        _matchingFrontends++;
                    }
                    else
                        radioButton.Enabled = false;
                }

                if (SrToReattach.type == SR.SRTypes.netapp.ToString() || SrToReattach.type == SR.SRTypes.equal.ToString())
                {
                    // the user is trying to reattach a netapp or Equallogic storage
                    // then move on to the correct page as there aren't radio buttons for them.
                    // we need to reenable the radioButtonCslg because it was disabled at the above iterations

                    _matchingFrontends++;
                    radioButtonCslg.Enabled = radioButtonCslg.Checked = true;

                    if (SrToReattach.type == SR.SRTypes.netapp.ToString())
                        radioButtonCslg.Tag = ((SrWizardType_Cslg)radioButtonCslg.Tag).ToNetApp();
                    else if (SrToReattach.type == SR.SRTypes.equal.ToString())
                        radioButtonCslg.Tag = ((SrWizardType_Cslg)radioButtonCslg.Tag).ToEqualLogic();
                }

                if (SrToReattach.type == "iso")
                {
                    string isoType;
                    if (SrToReattach.sm_config.TryGetValue("iso_type", out isoType))
                    {
                        if (isoType == "cifs")
                        {
                            radioButtonCifsIso.Checked = true;
                            _matchingFrontends--;
                        }
                        else if (isoType == "nfs_iso")
                        {
                            radioButtonNfsIso.Checked = true;
                            _matchingFrontends--;
                        }
                    }
                }
            }
        }

        public override bool EnableNext()
        {
            return m_allowNext;
        }

        #endregion

        public bool DisasterRecoveryTask { private get; set; }

        public SR SrToReattach { private get; set; }

        public int MatchingFrontends { get { return _matchingFrontends; } }

        public SrWizardType SrWizardType
        {
            get
            {
                foreach (RadioButton radioButton in RadioButtons)
                {
                    if (radioButton.Checked)
                        return (SrWizardType)radioButton.Tag;
                }
                return null;
            }
        }

        private RadioButton[] RadioButtons
        {
            get { return new[] { radioButtonNfs, radioButtonIscsi, radioButtonFibreChannel, radioButtonCslg, radioButtonNfsIso, radioButtonCifsIso, radioButtonCifs, radioButtonFcoe }; }
        }

        public void PreselectNewSrWizardType(Type type)
        {
            m_preselectedWizardType = type;
        }
        
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Checked)
            {
                foreach (RadioButton rb in RadioButtons)
                    rb.Checked = rb == radioButton;

                SrWizardType frontend = (SrWizardType)radioButton.Tag;

                SetupDeprecationBanner(frontend is SrWizardType_Cslg);

                if (frontend.IsEnhancedSR && Helpers.FeatureForbidden(Connection, Host.RestrictStorageChoices))
                {
                    selectedStoreTypeLabel.Visible = false;
                    selectedStoreTypeLabel.Text = string.Empty;
                    SRBlurb.Visible = false;
                    upsellPage1.Visible = true;
                    upsellPage1.SetAllTexts(Messages.UPSELL_BLURB_ENHANCEDSR, InvisibleMessages.UPSELL_LEARNMOREURL_ENHANCEDSR);
                    m_allowNext = false;
                }
                else
                {
                    upsellPage1.Visible = false;
                    selectedStoreTypeLabel.Visible = true;
                    selectedStoreTypeLabel.Text = radioButton.Text;
                    SRBlurb.Visible = true;
                    SRBlurb.Text = frontend.FrontendBlurb;
                    m_allowNext = true;
                }
                OnPageUpdated();
            }
        }
    }
}
