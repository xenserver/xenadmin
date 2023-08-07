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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public partial class ChooseSrTypePage : XenTabPage
    {
        private bool m_allowNext = true;
        private int _matchingFrontends;
        private Type m_preselectedWizardType = typeof(SrWizardType_Iscsi);
        private readonly RadioButton[] _radioButtons;

        public ChooseSrTypePage()
        {
            InitializeComponent();

            // Store the SrWizardType on the tag of their RadioButton
            radioButtonNfs.Tag = new SrWizardType_VhdoNfs();
            radioButtonIscsi.Tag = new SrWizardType_Iscsi();
            radioButtonFibreChannel.Tag = new SrWizardType_Hba();
            radioButtonNfsIso.Tag = new SrWizardType_NfsIso();
            radioButtonCifsIso.Tag = new SrWizardType_CifsIso();
            radioButtonCifs.Tag = new SrWizardType_Cifs();
            radioButtonFcoe.Tag = new SrWizardType_Fcoe();

            _radioButtons = new[]
            {
                radioButtonNfs, radioButtonIscsi, radioButtonFibreChannel,
                radioButtonCifs, radioButtonFcoe,
                radioButtonNfsIso, radioButtonCifsIso
            };
        }

        private void SetupDeprecationBanner(SrWizardType srWizardType)
        {
            if (srWizardType is SrWizardType_Fcoe)
            {
                deprecationBanner.AppliesToVersion = string.Format(Messages.STRING_SPACE_STRING,
                    BrandManager.ProductBrand, BrandManager.ProductVersionPost82);
                deprecationBanner.BannerType = DeprecationBanner.Type.Deprecation;
                deprecationBanner.FeatureName = Messages.SOFTWARE_FCOE_STORAGE_REPOSITORIES;
                deprecationBanner.LinkUri = new Uri(InvisibleMessages.DEPRECATION_URL);
                deprecationBanner.Visible = !HiddenFeatures.LinkLabelHidden;
            }
            else
            {
                deprecationBanner.Visible = false;
            }
        }

        #region XenTabPage overrides

        public override string Text => Messages.TYPE;

        public override string PageTitle => Messages.CHOOSE_SR_TYPE_PAGE_TITLE;

        public override string HelpID => "Type";

        public override void PopulatePage()
        {
            radioButtonCifs.Visible = !Helpers.FeatureForbidden(Connection, Host.RestrictCifs);

            radioButtonFcoe.Visible = true;

            foreach (var radioButton in _radioButtons)
            {
                var frontend = (SrWizardType)radioButton.Tag;
                frontend.ResetSrName(Connection);

                // these SR types have a blocked setter
                if (!(frontend is SrWizardType_CifsIso || frontend is SrWizardType_NfsIso || frontend is SrWizardType_Cifs))
                {
                    frontend.AllowToCreateNewSr = SrToReattach == null && !DisasterRecoveryTask;
                }

                frontend.DisasterRecoveryTask = DisasterRecoveryTask;
                frontend.SrToReattach = SrToReattach;
            }

            _matchingFrontends = 0;

            if (Connection == null)
            {
                Array.ForEach(_radioButtons, r => r.Enabled = r.Checked = false);
                _matchingFrontends = 1;
                return;
            }

            if (SrToReattach == null)
            {
                foreach (RadioButton radioButton in _radioButtons)
                {
                    SM sm = GetSmForRadioButton(radioButton);

                    // CA-21758: Use SM.other_config HideFromXenCenter flag to hide backends in the New SR wizard
                    // Only do this when we're doing a create.

                    if (sm == null || (!Properties.Settings.Default.ShowHiddenVMs && sm.IsHidden()))
                        radioButton.Visible = false;

                    if (radioButton.Visible && radioButton.Tag.GetType() == m_preselectedWizardType)
                        radioButton.Checked = true;
                }

                bool visibleRadioButtonsExist = _radioButtons.Any(r => r.Visible);
                bool checkedRadioButtonExists = _radioButtons.Any(r => r.Visible && r.Checked);

                tableLayoutPanel2.Visible = visibleRadioButtonsExist;

                if (visibleRadioButtonsExist && !checkedRadioButtonExists)
                    _radioButtons.First(r => r.Visible).Checked = true;
            }
            else
            {
                // If we're reconfiguring then try and select the correct page.  If we find more than one matching frontend
                // (ISO SRs - CA-19605) then just disable non-matching ones and leave it at the first page

                foreach (RadioButton radioButton in _radioButtons)
                {
                    SrWizardType wizardType = (SrWizardType)radioButton.Tag;
                    SM sm = GetSmForRadioButton(radioButton);

                    if (sm == null)
                        radioButton.Visible = false;
                    else if (wizardType.PossibleTypes.Contains(SrToReattach.GetSRType(true)))

                    {
                        if (_matchingFrontends == 0)
                            radioButton.Checked = true;
                        _matchingFrontends++;
                    }
                    else
                        radioButton.Enabled = false;
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

            labelVirtualDiskStorage.Visible = radioButtonNfs.Visible || radioButtonIscsi.Visible ||
                                             radioButtonFibreChannel.Visible ||
                                             radioButtonCifs.Visible || radioButtonFcoe.Visible;

            labelISOlibrary.Visible = radioButtonNfsIso.Visible || radioButtonCifsIso.Visible;
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
                foreach (RadioButton radioButton in _radioButtons)
                {
                    if (radioButton.Checked)
                        return (SrWizardType)radioButton.Tag;
                }
                return null;
            }
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
                foreach (RadioButton rb in _radioButtons)
                    rb.Checked = rb == radioButton;

                var frontend = (SrWizardType)radioButton.Tag;

                SetupDeprecationBanner(frontend);

                if (frontend.IsEnhancedSR && Helpers.FeatureForbidden(Connection, Host.RestrictStorageChoices))
                {
                    selectedStoreTypeLabel.Visible = false;
                    selectedStoreTypeLabel.Text = string.Empty;
                    SRBlurb.Visible = false;
                    upsellPage1.Visible = true;
                    upsellPage1.BlurbText = Messages.UPSELL_BLURB_ENHANCEDSR;
                    m_allowNext = false;
                }
                else
                {
                    upsellPage1.Visible = false;
                    selectedStoreTypeLabel.Visible = true;
                    selectedStoreTypeLabel.Text = frontend.FrontendTypeName;
                    SRBlurb.Visible = true;
                    SRBlurb.Text = frontend.FrontendBlurb;
                    m_allowNext = true;
                }
                OnPageUpdated();
            }
        }

        private SM GetSmForRadioButton(RadioButton radioButton)
        {
            SrWizardType wizardType = (SrWizardType)radioButton.Tag;
            return SM.GetByType(Connection, wizardType.Type.ToString());
        }
    }
}
