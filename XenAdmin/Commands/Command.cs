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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// An interface used to hide the SetSelection and SetMainWindow methods on the <see cref="Command"/> class. 
    /// These methods are used only by the Command framework and are potentially confusing if seen by Command consumers.
    /// </summary>
    internal interface ICommand
    {
        void SetSelection(IEnumerable<SelectedItem> selection);
        void SetMainWindow(IMainWindow mainWindow);
    }

    /// <summary>
    /// A class which represents a user's task in XenCenter. Classes derived from <see cref="Command"/> can be easily
    /// assigned to menu items and toolbar buttons.
    /// </summary>
    [TypeConverter(typeof(CommandConverter))]
    internal abstract class Command : ICommand
    {
        private SelectedItemCollection _selection = new SelectedItemCollection();
        private IMainWindow _mainWindow;
        private Control _parent;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        protected Command()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        protected Command(IMainWindow mainWindow)
        {
            Util.ThrowIfParameterNull(mainWindow, "mainWindow");
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        /// <param name="selection">The selection context for the Command.</param>
        protected Command(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : this(mainWindow)
        {
            Util.ThrowIfParameterNull(selection, "selection");
            _selection = new SelectedItemCollection(selection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        /// <param name="selection">The selection context for the Command.</param>
        protected Command(IMainWindow mainWindow, SelectedItem selection)
            : this(mainWindow, new[] { selection })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="mainWindow">The application main window.</param>
        /// <param name="xenObject">The selection context for the Command.</param>
        protected Command(IMainWindow mainWindow, IXenObject xenObject)
            : this(mainWindow, new SelectedItem(xenObject))
        {
        }

        /// <summary>
        /// Gets the current selection context for the Command.
        /// </summary>
        public SelectedItemCollection GetSelection()
        {
            return _selection;
        }

        /// <summary>
        /// Determines whether this instance can run with the current selection context.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can run; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRun()
        {
            return MainWindowCommandInterface != null && CanRunCore(GetSelection());
        }

        /// <summary>
        /// Determines whether this instance can run with the current selection context.
        /// </summary>
        /// <param name="selection">The selection context.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can run with the specified selection; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRunCore(SelectedItemCollection selection)
        {
            return true;
        }

        /// <summary>
        /// Runs this Command on the current selection.
        /// </summary>
        public void Run()
        {
            if (Confirm())
            {
                var cantRunReasons = GetCantRunReasons();
                var errorDialog = cantRunReasons.Count > 0 ? GetErrorDialogCore(cantRunReasons) : null;

                RunCore(GetSelection());

                if (errorDialog != null)
                {
                    errorDialog.ShowDialog(Parent);
                }
            }
        }

        /// <summary>
        /// Runs this Command.
        /// </summary>
        /// <param name="selection">The selection the Command should operate on.</param>
        protected virtual void RunCore(SelectedItemCollection selection)
        {
        }

        /// <summary>
        /// Gets the text for a menu item which launches this Command.
        /// </summary>
        public virtual string MenuText => null;

        /// <summary>
        /// Gets the text for a context menu item which launches this Command.
        /// </summary>
        public virtual string ContextMenuText => null;

        /// <summary>
        /// Gets the image for a menu item which launches this Command.
        /// </summary>
        public virtual Image MenuImage => null;

        /// <summary>
        /// Gets the image for a context menu item which launches this Command.
        /// </summary>
        public virtual Image ContextMenuImage => null;

        /// <summary>
        /// Gets the text for the toolbar button which launches this Command.
        /// </summary>
        public virtual string ToolBarText => null;

        /// <summary>
        /// Gets the image for a toolbar button which launches this Command.
        /// </summary>
        public virtual Image ToolBarImage => null;

        /// <summary>
        /// Gets the text for a button which launches this Command.
        /// </summary>
        public virtual string ButtonText => null;

        /// <summary>
        /// Gets the tool tip text when running is not possible. This is the
        /// can't run reason for single selection, and null otherwise.
        /// </summary>
        public virtual string DisabledToolTipText
        {
            get
            {
                var selection = GetSelection();
                if (selection.Count == 1)
                {
                    var item = selection[0];
                    if (item?.XenObject == null)
                        return null;

                    string reason = GetCantRunReasonCore(item.XenObject);
                    return reason == Messages.UNKNOWN ? null : reason;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the tool tip text when the command is able to run. Null by default.
        /// </summary>
        public virtual string EnabledToolTipText => null;

        /// <summary>
        /// Gets the shortcut key display string. This is only used if this Command is used on the main menu.
        /// </summary>
        public virtual string ShortcutKeyDisplayString => null;

        /// <summary>
        /// Gets the shortcut keys. This is only used if this Command is used on the main menu.
        /// </summary>
        public virtual Keys ShortcutKeys => Keys.None;

        /// <summary>
        /// Gets a value indicating whether a confirmation dialog should be shown.
        /// </summary>
        protected virtual bool ConfirmationRequired => false;

        /// <summary>
        /// Gets the confirmation dialog title. The default for this is Messages.MESSAGEBOX_CONFIRM.
        /// </summary>
        protected virtual string ConfirmationDialogTitle => null;

        /// <summary>
        /// Gets the confirmation dialog text.
        /// </summary>
        protected virtual string ConfirmationDialogText => null;

        /// <summary>
        /// Gets the help id for the confirmation dialog.
        /// </summary>
        protected virtual string ConfirmationDialogHelpId => null;

        protected virtual string ConfirmationDialogYesButtonLabel => null;

        protected virtual string ConfirmationDialogNoButtonLabel => null;

        protected virtual bool ConfirmationDialogNoButtonSelected => false;

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        /// <returns>True if the user clicked Yes.</returns>
        private bool ShowConfirmationDialog()
        {
            if (Program.RunInAutomatedTestMode)
                return true;

            var buttonYes = new ThreeButtonDialog.TBDButton(
                string.IsNullOrEmpty(ConfirmationDialogYesButtonLabel) ? Messages.YES_BUTTON_CAPTION : ConfirmationDialogYesButtonLabel,
                DialogResult.Yes);

            var buttonNo = new ThreeButtonDialog.TBDButton(
                string.IsNullOrEmpty(ConfirmationDialogNoButtonLabel) ? Messages.NO_BUTTON_CAPTION : ConfirmationDialogNoButtonLabel,
                DialogResult.No,
                selected: ConfirmationDialogNoButtonSelected);

            using (var dialog = new WarningDialog(ConfirmationDialogText, buttonYes, buttonNo)
            { WindowTitle = ConfirmationDialogTitle })
            {
                if (!string.IsNullOrEmpty(ConfirmationDialogHelpId))
                    dialog.HelpNameSetter = ConfirmationDialogHelpId;

                return dialog.ShowDialog(Parent ?? Program.MainWindow) == DialogResult.Yes;
            }
        }

        /// <summary>
        /// Shows a confirmation dialog if required.
        /// </summary>
        /// <returns>True if the operation should proceed.</returns>
        protected virtual bool Confirm()
        {
            return !ConfirmationRequired || ShowConfirmationDialog();
        }

        /// <summary>
        /// Gets all of the reasons that items in the selection can't run.
        /// </summary>
        /// <returns>A dictionary of reasons keyed by the item name.</returns>
        public Dictionary<IXenObject, string> GetCantRunReasons()
        {
            var cantRunReasons = new Dictionary<IXenObject, string>();

            foreach (SelectedItem item in GetSelection())
            {
                if (item == null || item.XenObject == null)
                    continue;
                if (MainWindowCommandInterface != null && CanRunCore(new SelectedItemCollection(item)))
                    continue;

                string reason = GetCantRunReasonCore(item.XenObject);
                if (reason != null)
                    cantRunReasons.Add(item.XenObject, reason);
            }

            return cantRunReasons;
        }


        /// <summary>
        /// Gets the reason that the specified item from the selection can't run. This is displayed in the error dialog.
        /// The default is "Unknown".
        /// </summary>
        protected virtual string GetCantRunReasonCore(IXenObject item)
        {
            return Messages.UNKNOWN;
        }

        /// <summary>
        /// Gets the error dialog to be displayed if one or more items in the selection couldn't be run. Returns null by
        /// default i.e. An error dialog isn't displayed by default.
        /// </summary>
        /// <param name="cantRunReasons">The reasons for why the items couldn't run.</param>
        protected virtual CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantRunReasons)
        {
            return null;
        }

        /// <summary>
        /// Gets the main window to be used by the Command.
        /// </summary>
        public IMainWindow MainWindowCommandInterface => _mainWindow;


        /// <summary>
        /// Gets or sets the parent control for any dialogs launched during the
        /// running of the command. Defaults to the MainWindow Form.
        /// </summary>
        public Control Parent
        {
            get => _parent ?? _mainWindow?.Form;
            set => _parent = value;
        }

        /// <summary>
        /// Runs the specified <see cref="AsyncAction"/>s such that they are synchronous per connection but asynchronous across connections.
        /// </summary>
        /// <param name="endDescription"></param>
        /// <param name="runActionsInParallel">Whether the actions should be run simultaneously</param>
        /// <param name="actions"></param>
        /// <param name="title"></param>
        /// <param name="startDescription"></param>
        public void RunMultipleActions(IEnumerable<AsyncAction> actions, string title, string startDescription, string endDescription, bool runActionsInParallel)
        {
            MultipleActionLauncher launcher = new MultipleActionLauncher(actions, title, startDescription, endDescription, runActionsInParallel);
            launcher.Run();
        }

        #region ICommand Members

        /// <summary>
        /// Sets the selection context for the Command. This is hidden as it is only for use by the Command framework. 
        /// </summary>
        void ICommand.SetSelection(IEnumerable<SelectedItem> selection)
        {
            Util.ThrowIfParameterNull(selection, "selection");
            _selection = new SelectedItemCollection(selection);
        }

        /// <summary>
        /// Sets the main window for the Command. This is hidden as it is only for use by the Command framework.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        void ICommand.SetMainWindow(IMainWindow mainWindow)
        {
            Util.ThrowIfParameterNull(mainWindow, "mainWindow");
            _mainWindow = mainWindow;
        }

        #endregion
    }
}
