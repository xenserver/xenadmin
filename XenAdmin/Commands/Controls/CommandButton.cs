﻿/* Copyright (c) Citrix Systems, Inc. 
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
using System.Windows.Forms;
using System.ComponentModel;
using XenAdmin.Controls;

namespace XenAdmin.Commands
{
    /// <summary>
    /// A <see cref="Button"/> that can be bound to a Command.
    /// </summary>
    internal class CommandButton : Button, ICommandControl
    {
        private Command _command;
        private SelectionBroadcaster _selectionBroadcaster;

        /// <summary>
        /// Gets or sets the command which is being used by the Command control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Command Command
        {
            get
            {
                return _command;
            }
            set
            {
                if (value != _command)
                {
                    _command = value;
                    Update();
                }
            }
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (_command != null && _selectionBroadcaster != null)
            {
                ((ICommand)_command).SetSelection(_selectionBroadcaster.Selection);
                Update();
            }
        }

        private new void Update()
        {
            Enabled = _command != null && _command.CanExecute();
            base.Enabled = DesignMode || Enabled;

            if (_command != null)
            {
                if (_command.ButtonText != null)
                    Text = _command.ButtonText;

                if (Parent is ToolTipContainer ttContainer)
                    ttContainer.SetToolTip(Enabled ? _command.EnabledToolTipText : _command.DisabledToolTipText);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (Enabled)
            {
                _command.Execute();
            }
            base.OnClick(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SelectionBroadcaster = null;
            }

            base.Dispose(disposing);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Enabled { get; private set; }

        /// <summary>
        /// Sets the <see cref="SelectionBroadcaster"/> that should be listened to for selection changes.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectionBroadcaster SelectionBroadcaster
        {
            set
            {
                if (value != _selectionBroadcaster)
                {
                    if (_selectionBroadcaster != null)
                    {
                        _selectionBroadcaster.SelectionChanged -= SelectionChanged;
                    }

                    _selectionBroadcaster = value;

                    if (_selectionBroadcaster != null)
                    {
                        _selectionBroadcaster.SelectionChanged += SelectionChanged;
                    }

                    SelectionChanged(null, null);
                }
            }
        }
    }
}
