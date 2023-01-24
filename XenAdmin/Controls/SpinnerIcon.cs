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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Lightweight control to support easy display of the rotating spinning icon and a configurable success icon.
    /// </summary>
    public class SpinnerIcon : PictureBox
    {
        #region Fields

        private readonly Timer spinningTimer = new Timer();
        private const int SPEED = 150;
        private int currentPosition;
        private Image successImage = Images.StaticImages._000_Tick_h32bit_16;
        private Image failureImage = Images.StaticImages._000_error_h32bit_16;

        private readonly Image[] spinningImageFrames =
        {
            Images.StaticImages.SpinningFrame0,
            Images.StaticImages.SpinningFrame1,
            Images.StaticImages.SpinningFrame2,
            Images.StaticImages.SpinningFrame3,
            Images.StaticImages.SpinningFrame4,
            Images.StaticImages.SpinningFrame5,
            Images.StaticImages.SpinningFrame6,
            Images.StaticImages.SpinningFrame7
        };

        #endregion

        #region Properties

        /// <summary>
        /// Image to be displayed when DisplaySuccessImage() is called.
        /// Default value is _000_Tick_h32bit_16
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(typeof(int), "Image that is displayed after DisplaySuccessImage() is invoked.")]
        public Image SuccessImage
        {
            get => successImage;
            set
            {
                successImage = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Image to be displayed when DisplayFailureImage() is called.
        /// Default value is _000_error_h32bit_16
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(typeof(int), "Image that is displayed after DisplayFailureImage() is invoked.")]
        public Image FailureImage
        {
            get => failureImage;
            set
            {
                failureImage = value;
                Invalidate();
            }
        }

        #endregion

        public SpinnerIcon()
        {
            spinningTimer.Interval = SPEED;
            spinningTimer.Tick += spinningTimer_Tick;
        }

        private void spinningTimer_Tick(object sender, EventArgs e)
        {
            if (currentPosition == spinningImageFrames.Length)
                currentPosition = 0;

            Image = spinningImageFrames[currentPosition++];
        }

        /// <summary>
        /// Displays the spinning icon and sets Visible=true on the control.
        /// </summary>
        public void StartSpinning()
        {
            currentPosition = 0;
            Image = null;
            Visible = true;
            spinningTimer.Start();
        }

        /// <summary>
        /// Stops the spinning and sets Visible=false on the control.
        /// </summary>
        public void StopSpinning()
        {
            Visible = false;
            spinningTimer.Stop();
        }

        /// <summary>
        /// Shows the SuccessImage instead of the spinning icon and sets Visible=true on the control.
        /// </summary>
        /// <remarks>It is not necessary to call StopSpinning() before calling this method.</remarks>
        public void ShowSuccessImage()
        {
            StopSpinning();
            Image = SuccessImage;
            Visible = true;
        }

        /// <summary>
        /// Shows the FailureImage instead of the spinning icon and sets Visible=true on the control
        /// </summary>
        /// <remarks>It is not necessary to call StopSpinning() before calling this method.</remarks>
        public void ShowFailureImage()
        {
            StopSpinning();
            Image = FailureImage;
            Visible = true;
        }

        protected override void Dispose(bool disposing)
        {
            spinningTimer.Stop();
            spinningTimer.Tick -= spinningTimer_Tick;
            spinningTimer.Dispose();

            base.Dispose(disposing);
        }
    }
}
