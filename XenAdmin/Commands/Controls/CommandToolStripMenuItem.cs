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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace XenAdmin.Commands
{
    /// <summary>
    /// A <see cref="ToolStripMenuItem"/> that can be bound to a Command.
    /// </summary>
    internal class CommandToolStripMenuItem : ToolStripMenuItem, ICommandControl
    {
        private Command _command;
        private readonly bool _inContextMenu;
        private SelectionBroadcaster _selectionBroadcaster;

        private CommandToolStripMenuItem(Command command, bool inContextMenu, string text, Image image)
            : base(text, image)
        {
            //Registering these event handlers from the constructor used by the program initialiser eg.
            //when command == null consumes unecessary memory
            if(command != null)
            {
                base.DropDownOpening += CommandToolStripMenuItem_DropDownOpening;
                DropDown.Opening += DropDown_Opening;
            }

            Font = Program.DefaultFont;
            _inContextMenu = inContextMenu;
            Command = command;
            Update();
        }

		private void DropDown_Opening(object sender, CancelEventArgs e)
		{
			//CA-47312: if the parent item is disabled the drop down shouldn't open;
			//this handles a control bug where the drop down of a disabled item can be
			//opened if previously the drop down of an enabled item has been opened

			ToolStripDropDown dropDown = sender as ToolStripDropDown;

			if (dropDown != null && dropDown.OwnerItem != null)
			{
				if (!dropDown.OwnerItem.Enabled)
					e.Cancel = true;
			}
		}

        private void CommandToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // CA-42123 check command can execute again before opening dropdown.
            if (Command.CanExecute())
            {
                OnDropDownOpening(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandToolStripMenuItem"/> class.
        /// </summary>
        public CommandToolStripMenuItem()
            : this(null, false, string.Empty, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="command">The command which should be bound to.</param>
        public CommandToolStripMenuItem(Command command)
            : this(command, false, string.Empty, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandToolStripMenuItem"/> class.
        /// </summary>
        /// <param name="command">The command which should be bound to.</param>
        /// <param name="inContextMenu">if set to <c>true</c> then this menu item is for a context menu.</param>
        public CommandToolStripMenuItem(Command command, bool inContextMenu)
            : this(command, inContextMenu, string.Empty, null)
        {
        }

        public CommandToolStripMenuItem(Command command, string text)
            : this(command, false, text, null)
        {
        }

        public CommandToolStripMenuItem(Command command, string text, Image image)
            : this(command, false, text, image)
        {
        }

        /// <summary>
        /// Gets or sets the command which is being used by the Command control.
        /// </summary>
        /// <value></value>
        [DefaultValue(null)]
        [Editor(typeof(CommandEditor<Command>), typeof(UITypeEditor))]
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

        protected virtual void Update()
        {
            base.Enabled = DesignMode || Enabled;

            if (_command != null)
            {
                if (_inContextMenu && _command.ContextMenuText != null)
                {
                    Text = _command.ContextMenuText;
                }
                else if (_command.MenuText != null)
                {
                    Text = _command.MenuText;
                }

                if (_inContextMenu && _command.ContextMenuImage != null)
                {
                    Image = _command.ContextMenuImage;
                }
                else if (_command.MenuImage != null)
                {
                    Image = _command.MenuImage;
                }

                ToolTipText = _command.ToolTipText; // null ToolTip is allowed (CA-47310)

                if (_command.ShortcutKeyDisplayString != null)
                {
                    if (Command.MainWindowCommandInterface != null && Command.MainWindowCommandInterface.MenuShortcutsEnabled && !_inContextMenu)
                    {
                        ShortcutKeyDisplayString = _command.ShortcutKeyDisplayString;
                    }
                    else
                    {
                        ShortcutKeyDisplayString = string.Empty;
                    }
                }
                if (_command.ShortcutKeys != Keys.None)
                {
                    if (Command.MainWindowCommandInterface != null && Command.MainWindowCommandInterface.MenuShortcutsEnabled && !_inContextMenu)
                    {
                        ShortcutKeys = _command.ShortcutKeys;
                    }
                    else
                    {
                        ShortcutKeys = Keys.None;
                    }
                }
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
                base.DropDownOpening -= CommandToolStripMenuItem_DropDownOpening;
                DropDown.Opening -= DropDown_Opening;
                SelectionBroadcaster = null;
                DropDownOpening = null;
            }

            base.Dispose(disposing);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Enabled
        {
            get
            {
                return _command != null && _command.CanExecute();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SelectionBroadcaster"/> that should be listened to for selection changes.
        /// </summary>
        /// <value></value>
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
            private get { return _selectionBroadcaster; }
        }

        /// <summary>
        /// Don't allow designer to set font. We do that in the constructor of this class.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DropDownOpening"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDropDownOpening(EventArgs e)
        {
            var handler = DropDownOpening;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs as the <see cref="T:System.Windows.Forms.ToolStripDropDown"/> is opening.
        /// </summary>
        public new event EventHandler DropDownOpening;
    }
}
