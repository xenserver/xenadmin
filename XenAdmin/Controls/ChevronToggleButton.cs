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
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    public class ChevronToggleButton : ChevronButton
    {
        public enum TogglePosition
        {
            Collapsed,
            Expanded
        }

        public string ToggleUpText { private get; set; }
        public string ToggleDownText { private get; set; }
        private Image ToggleUpImage { get; set; }
        private Image ToggleDownImage { get; set; }

        private TogglePosition currentTogglePosition;
        public TogglePosition CurrentTogglePosition
        {
            get { return currentTogglePosition; }
            set
            {
                currentTogglePosition = value;
                Image = value == TogglePosition.Collapsed ? ToggleUpImage : ToggleDownImage;
                Text = value == TogglePosition.Collapsed ? ToggleUpText : ToggleDownText;
            }
        }

        public ChevronToggleButton() : this(String.Empty, String.Empty){}

        public ChevronToggleButton(string toogleUpText, string toogleDownText)
        {
            ToggleUpImage = Properties.Resources.PDChevronRight;
            ToggleDownImage = Properties.Resources.PDChevronDown;
            ToggleUpText = toogleUpText;
            ToggleDownText = toogleDownText;
            CurrentTogglePosition = TogglePosition.Collapsed;
            ButtonClick += ChevronToggleButton_ButtonClick;
            KeyDown += ChevronToggleButton_KeyDown;
        }

        void ChevronToggleButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                ChevronToggleButton_ButtonClick(sender, e);
        }

        void ChevronToggleButton_ButtonClick(object sender, EventArgs e)
        {
            CurrentTogglePosition = CurrentTogglePosition == TogglePosition.Collapsed ? TogglePosition.Expanded : TogglePosition.Collapsed;
        }
    }
}
