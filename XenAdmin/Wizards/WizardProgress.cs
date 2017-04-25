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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using XenAdmin.Controls;

namespace XenAdmin.Wizards
{
    public partial class WizardProgress : UserControl
    {
        private static readonly Color HighlightColorMiddle = Color.FromArgb(10, 80, 200);
        private static readonly Color HighlightColorEdge = Color.FromArgb(9, 70, 162);
        private static readonly Color bgBrushColor = Color.FromArgb(255, 255, 255);

        private int _currentStep = 0;

        public WizardProgress()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public readonly List<XenTabPage> Steps = new List<XenTabPage>();

        /// <summary>
        /// Transitions to the previous non-disabled step, or remains at the present step
        /// if already at the first non-disabled step.
        /// </summary>
        public void PreviousStep()
        {
            System.Diagnostics.Trace.Assert(!IsFirstStep);

            if (!OnLeavingStep(false))
            {
                // If anyone cancels the transition, remain on this step
                return;
            }

            int destination = _currentStep - 1;
            // Find the previous non-disabled step
            while (destination > 0 && Steps[destination].DisableStep)
            {
                destination--;
            }
            if (!Steps[destination].DisableStep)
            {
                _currentStep = destination;
                Invalidate();
                OnEnteringStep(false);
            }
        }

        /// <summary>
        /// Transitions to the next non-disabled step, or remains at the present step
        /// if already at the last non-disabled step.
        /// </summary>
        public void NextStep()
        {
            if (!OnLeavingStep(true))
            {
                // If anyone cancels the transition, remain on this step
                return;
            }

            System.Diagnostics.Trace.Assert(!IsLastStep);

            int destination = _currentStep + 1;
            // Find the next non-disabled step
            while (destination < Steps.Count - 1 && Steps[destination].DisableStep)
            {
                destination++;
            }
            if (!Steps[destination].DisableStep)
            {
                _currentStep = destination;
                Invalidate();
                OnEnteringStep(true);
            }
        }

        public event EventHandler<WizardProgressEventArgs> LeavingStep;
        public event EventHandler<WizardProgressEventArgs> EnteringStep;

        public XenTabPage CurrentStepTabPage
        {
            get { return Steps[_currentStep]; }
        }

        /// <summary>
        /// IsFirstStep doesn't include any disabled pages at the beginning,
        /// because we can never turn these on (within this instance of the wizard).
        /// </summary>
        public bool IsFirstStep
        {
            get
            {
                int firstRealStep = 0;
                while (firstRealStep < Steps.Count && Steps[firstRealStep].DisableStep)
                    ++firstRealStep;
                return _currentStep == firstRealStep;
            }
        }

        public bool IsLastStep
        {
            get { return _currentStep == Steps.Count - 1; }
        }

        /// <returns>
        /// True if the event has gone through, false if cancelled.
        /// </returns>
        private bool OnLeavingStep(bool isForwardsTransition)
        {
            WizardProgressEventArgs e = new WizardProgressEventArgs(isForwardsTransition);
            if (LeavingStep != null)
                LeavingStep(this, e);
            return !e.Cancelled;
        }

        private void OnEnteringStep(bool isForwardsTransition)
        {
            WizardProgressEventArgs e = new WizardProgressEventArgs(isForwardsTransition);
            if (EnteringStep != null)
                EnteringStep(this, e);
        }

        /// <summary>
        /// Draws the list of wizard steps, with the current step highlighted and any disabled steps grayed out.
        /// </summary>

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bg = Properties.Resources.wizard_background;
            int bg_h = (int)(bg.Height * (Width / (float)bg.Width)); //The assumption made is that Width/bg.Width ratio always matches the system's dpi setting. Normally (at 100% dpi setting) this equals to 1.
            int bg_top = Height - bg_h;
            if (bg_top > 0)
            {
                using (SolidBrush bgBrush = new SolidBrush(bgBrushColor))
                    e.Graphics.FillRectangle(bgBrush, new Rectangle(0, 0, Width, bg_top));
            }

            int bg_w = Width;

            //This makes sure we compensate for rounding if it happened while bg_h was calculated. (to avoid gap on the right) 
            //Switch to 125% dpi to see why this is needed...
            if (Width / (float)bg.Width != 1f)
                bg_w++;

            e.Graphics.DrawImage(bg, new Rectangle(0, bg_top, bg_w, bg_h ));

            using (LinearGradientBrush highlight = new LinearGradientBrush(Point.Empty, new Point(Width, 0), HighlightColorEdge, HighlightColorMiddle))
            {
                highlight.SetBlendTriangularShape(0.5f);
                float y = 15F;
                for (int step = 0; step < Steps.Count; step++)
                {
                    if (step == _currentStep)
                    {
                        e.Graphics.FillRectangle(highlight, 0, y, Width, 20F);
                        TextRenderer.DrawText(e.Graphics, Steps[step].Text, Program.DefaultFontBold, new Rectangle(10, (int)y, Width, 20), SystemColors.HighlightText,
                                              TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix);
                    }
                    else
                    {
                        TextRenderer.DrawText(e.Graphics, Steps[step].Text, Program.DefaultFont, new Rectangle(10, (int)y, Width, 20), Steps[step].DisableStep ? SystemColors.GrayText : Color.Black, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix);
                    }
                    y += 24F;
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }
}
