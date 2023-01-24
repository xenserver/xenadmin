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
using XenAdmin.Actions.GUIActions;
using XenAdmin.Network;
using XenAdmin.Actions;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Core;
using System.Threading;
using XenAdmin.Dialogs.WarningDialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Disconnects the specified connection.
    /// </summary>
    internal class DisconnectCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _prompt;
        private readonly IXenConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectCommand"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="connection">The connection to be disconnected</param>
        /// <param name="prompt">if set to <c>true</c> a confirmation prompt is shown.</param>
        public DisconnectCommand(IMainWindow mainWindow, IXenConnection connection, bool prompt)
            : base(mainWindow)
        {
            _prompt = prompt;
            _connection = connection;
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return _connection != null && (_connection.IsConnected || _connection.InProgress);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>false if the user cancels the disconnect.</returns>
        public new bool Run()
        {
            return CanRun() && Run(_connection, _prompt);
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            Run(_connection, _prompt);
        }

        private bool Run(IXenConnection connection, bool prompt)
        {
            if (!ConfirmCancelRunningActions(MainWindowCommandInterface, Parent, connection, prompt))
                return false;

            DoDisconnect(connection);
            return true;
        }

        public static bool ConfirmCancelRunningActions(IMainWindow mainWindow, IWin32Window parent, IXenConnection connection, bool prompt)
        {
            if (prompt)
            {
                if (!AllActionsFinished(connection, true))
                {
                    if (mainWindow.RunInAutomatedTestMode ||
                        new CloseXenCenterWarningDialog(false, connection).ShowDialog(parent) == DialogResult.OK)
                    {
                        ConnectionsManager.CancelAllActions(connection);

                        var waitForCancelAction = new DelegatedAsyncAction(connection,
                            Messages.CANCELING_TASKS, Messages.CANCELING, Messages.COMPLETED,
                            delegate
                            {
                                DateTime startTime = DateTime.Now;
                                while ((DateTime.Now - startTime).TotalSeconds < 6.0)
                                {
                                    if (AllActionsFinished(connection, false))
                                        break;

                                    Thread.Sleep(2000);
                                }
                            });

                        using (var pd = new ActionProgressDialog(waitForCancelAction, ProgressBarStyle.Marquee))
                            pd.ShowDialog(parent);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                // no prompt. All tasks are cancelled and the server disconnected
                ConnectionsManager.CancelAllActions(connection);
            }

            return true;
        }

        private void DoDisconnect(IXenConnection connection)
        {
            string msg = string.Format(Messages.CONNECTION_CLOSED_NOTICE_TEXT, connection.Hostname);
            new DummyAction(msg, msg)
            {
                Pool = Helpers.GetPoolOfOne(connection),
                Host = Helpers.GetCoordinator(connection)
            }.Run();
            log.Warn($"Connection to {connection.Hostname} closed.");

            MainWindowCommandInterface.CloseActiveWizards(connection);
            XenDialogBase.CloseAll(connection);
            connection.EndConnect();
            MainWindowCommandInterface.SaveServerList();
        }

        private static bool AllActionsFinished(IXenConnection connection, bool treatCancelingAsFinished)
        {
            foreach (ActionBase action in ConnectionsManager.History)
            {
                if (action.IsCompleted || action is MeddlingAction)
                    continue;

                if (treatCancelingAsFinished)
                {
                    if (action is AsyncAction a && a.Cancelling)
                        continue;
                }

                IXenObject xo = action.Pool ?? action.Host ?? action.VM ?? action.SR as IXenObject;
                if (xo == null || xo.Connection != connection)
                    continue;

                return false;
            }
            return true;
        }
    }
}
