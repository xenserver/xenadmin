﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using XenAdmin.Actions.GUIActions;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Wizards.ImportWizard;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.TabPages;
using XenAdmin.XenSearch;
using XenAdmin.Plugins;
using XenCenterLib;
using System.Linq;
using XenAdmin.Controls.GradientPanel;
using XenAdmin.Help;

namespace XenAdmin
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public partial class MainWindow : Form, ISynchronizeInvoke, IMainWindow, IFormWithHelp
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A mapping between objects in the tree and the associated selected tab.
        /// </summary>
        private Dictionary<object, TabPage> selectedTabs = new Dictionary<object, TabPage>();

        /// <summary>
        /// The selected tab for the overview node.
        /// </summary>
        private TabPage selectedOverviewTab;

        internal readonly PerformancePage PerformancePage = new PerformancePage();
        internal readonly GeneralTabPage GeneralPage = new GeneralTabPage();
        internal readonly BallooningPage BallooningPage = new BallooningPage();
        internal readonly ConsolePanel ConsolePanel = new ConsolePanel();
        internal readonly CvmConsolePanel CvmConsolePanel = new CvmConsolePanel();
        internal readonly HAPage HAPage = new HAPage();
        internal readonly HAUpsellPage HAUpsellPage = new HAUpsellPage();
        internal readonly HomePage HomePage = new HomePage();
        internal readonly SearchPage SearchPage = new SearchPage();
        internal readonly NetworkPage NetworkPage = new NetworkPage();
        internal readonly NICPage NICPage = new NICPage();
        internal readonly WlbPage WlbPage = new WlbPage();
        internal readonly WLBUpsellPage WLBUpsellPage = new WLBUpsellPage();
        internal readonly SrStoragePage SrStoragePage = new SrStoragePage();
        internal readonly PhysicalStoragePage PhysicalStoragePage = new PhysicalStoragePage();
        internal readonly VMStoragePage VMStoragePage = new VMStoragePage();
        internal readonly AdPage AdPage = new AdPage();
        internal readonly ADUpsellPage AdUpsellPage = new ADUpsellPage();
        internal readonly GpuPage GpuPage = new GpuPage();
        internal readonly PvsPage PvsPage = new PvsPage();
        internal readonly DockerProcessPage DockerProcessPage = new DockerProcessPage();
        internal readonly DockerDetailsPage DockerDetailsPage = new DockerDetailsPage();
        internal readonly UsbPage UsbPage = new UsbPage();

        private ActionBase statusBarAction;

        private bool IgnoreTabChanges;

        /// <summary>
        /// Helper boolean to only trigger Resize_End when window is really resized by dragging edges
        /// Without this Resize_End is triggered even when window is moved around and not resized
        /// </summary>
        private bool mainWindowResized;

        private readonly Dictionary<IXenConnection, IList<Form>> activePoolWizards = new Dictionary<IXenConnection, IList<Form>>();

        private string[] _commandLineArgs;

        private static readonly System.Windows.Forms.Timer CheckForUpdatesTimer = new System.Windows.Forms.Timer();

        public readonly PluginManager PluginManager;
        private readonly ContextMenuBuilder contextMenuBuilder;

        private readonly LicenseManagerLauncher licenseManagerLauncher;
        private readonly LicenseTimer licenseTimer;

        private Dictionary<ToolStripMenuItem, int> pluginMenuItemStartIndexes = new Dictionary<ToolStripMenuItem, int>();

        private bool expandTreeNodesOnStartup;
        private int connectionsInProgressOnStartup;

        private ClientUpdateAlert updateAlert;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern uint RegisterApplicationRestart(string pszCommandline, uint dwFlags);

        public event Action CloseSplashRequested;

        public MainWindow(string[] args)
        {
            _commandLineArgs = args;
            licenseManagerLauncher = new LicenseManagerLauncher(Program.MainWindow);
            InvokeHelper.Initialize(this);

            ConnectionsManager.XenConnections.Clear();
            ConnectionsManager.History.Clear();
            Search.InitSearch();

            InitializeComponent();
            SetMenuItemStartIndexes();
            Icon = Properties.Resources.AppIcon;

            //CA-270999: Add registration to RestartManager
            RegisterApplicationRestart(null, 0);

            #region Add Tab pages

            components.Add(NICPage);
            components.Add(VMStoragePage);
            components.Add(SrStoragePage);
            components.Add(PerformancePage);
            components.Add(GeneralPage);
            components.Add(BallooningPage);
            components.Add(ConsolePanel);
            components.Add(CvmConsolePanel);
            components.Add(NetworkPage);
            components.Add(HAPage);
            components.Add(HomePage);
            components.Add(WlbPage);
            components.Add(AdPage);
            components.Add(GpuPage);
            components.Add(PvsPage);
            components.Add(SearchPage);
            components.Add(DockerProcessPage);
            components.Add(DockerDetailsPage);
            components.Add(UsbPage);

            AddTabContents(VMStoragePage, TabPageStorage);
            AddTabContents(SrStoragePage, TabPageSR);
            AddTabContents(NICPage, TabPageNICs);
            AddTabContents(PerformancePage, TabPagePeformance);
            AddTabContents(GeneralPage, TabPageGeneral);
            AddTabContents(BallooningPage, TabPageBallooning);
            AddTabContents(ConsolePanel, TabPageConsole);
            AddTabContents(CvmConsolePanel, TabPageCvmConsole);
            AddTabContents(NetworkPage, TabPageNetwork);
            AddTabContents(HAPage, TabPageHA);
            AddTabContents(HAUpsellPage, TabPageHAUpsell);
            AddTabContents(HomePage, TabPageHome);
            AddTabContents(WlbPage, TabPageWLB);
            AddTabContents(WLBUpsellPage, TabPageWLBUpsell);
            AddTabContents(PhysicalStoragePage, TabPagePhysicalStorage);
            AddTabContents(AdPage, TabPageAD);
            AddTabContents(AdUpsellPage, TabPageADUpsell);
            AddTabContents(GpuPage, TabPageGPU);
            AddTabContents(PvsPage, TabPagePvs);
            AddTabContents(SearchPage, TabPageSearch);
            AddTabContents(DockerProcessPage, TabPageDockerProcess);
            AddTabContents(DockerDetailsPage, TabPageDockerDetails);
            AddTabContents(UsbPage, TabPageUSB);

            #endregion

            PoolCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Pool>);
            MessageCollectionChangedWithInvoke = Program.ProgramInvokeHandler(MessageCollectionChanged);
            HostCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Host>);
            VMCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<VM>);
            SRCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<SR>);
            FolderCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Folder>);
            TaskCollectionChangedWithInvoke = Program.ProgramInvokeHandler(MeddlingActionManager.TaskCollectionChanged);

            RegisterEvents();

            PluginManager = new PluginManager();
            PluginManager.PluginsChanged += pluginManager_PluginsChanged;
            PluginManager.LoadPlugins();
            contextMenuBuilder = new ContextMenuBuilder(PluginManager, this);
            ((WinformsXenAdminConfigProvider) XenAdminConfigManager.Provider).PluginManager = PluginManager;

            FormFontFixer.Fix(this);

            Folders.InitFolders();
            DockerContainers.InitDockerContainers();

            // Fix colour of text on gradient panels
            TitleLabel.ForeColor = VerticalGradientPanel.TextColor;
            loggedInLabel1.SetTextColor(VerticalGradientPanel.TextColor);

            statusProgressBar.Visible = false;

            SelectionManager.BindTo(MainMenuBar.Items, this);
            SelectionManager.BindTo(ToolStrip.Items, this);

            licenseTimer = new LicenseTimer(licenseManagerLauncher);
            GeneralPage.LicenseLauncher = licenseManagerLauncher;

            updateClientToolStripMenuItem.Visible = false;

            xenSourceOnTheWebToolStripMenuItem.Text = string.Format(xenSourceOnTheWebToolStripMenuItem.Text,
                BrandManager.ProductBrand);
            viewApplicationLogToolStripMenuItem.Text = string.Format(viewApplicationLogToolStripMenuItem.Text, BrandManager.BrandConsole);
            xenCenterPluginsOnlineToolStripMenuItem.Text = string.Format(xenCenterPluginsOnlineToolStripMenuItem.Text, BrandManager.BrandConsole);
            aboutXenSourceAdminToolStripMenuItem.Text = string.Format(aboutXenSourceAdminToolStripMenuItem.Text, BrandManager.BrandConsole);
            templatesToolStripMenuItem1.Text = string.Format(templatesToolStripMenuItem1.Text, BrandManager.ProductBrand);
            updateClientToolStripMenuItem.Text = string.Format(updateClientToolStripMenuItem.Text, BrandManager.BrandConsole);
            toolStripMenuItemCfu.Text = string.Format(toolStripMenuItemCfu.Text, BrandManager.BrandConsole);

            toolStripSeparator7.Visible = xenSourceOnTheWebToolStripMenuItem.Visible = xenCenterPluginsOnlineToolStripMenuItem.Visible = !HiddenFeatures.ToolStripMenuItemHidden;

            statusLabelAlerts.Visible = statusLabelUpdates.Visible = statusLabelErrors.Visible = false;
        }

        private void RegisterEvents()
        {
            //ClipboardViewer is registered in OnHandleCreated
            OtherConfigAndTagsWatcher.RegisterEventHandlers();
            Alert.RegisterAlertCollectionChanged(XenCenterAlerts_CollectionChanged);
            Updates.UpdateAlertCollectionChanged += Updates_CollectionChanged;
            Updates.CheckForClientUpdatesStarted += ClientUpdatesCheck_Started;
            Updates.CheckForClientUpdatesCompleted += ClientUpdatesCheck_Completed;
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;
            //ConnectionsManager.XenConnections.CollectionChanged is registered in OnShown
            Properties.Settings.Default.SettingChanging += Default_SettingChanging;
            eventsPage.GoToXenObjectRequested += eventsPage_GoToXenObjectRequested;
            SearchPage.SearchChanged += SearchPanel_SearchChanged;
        }

        private void UnRegisterEvents()
        {
            Clip.UnregisterClipboardViewer();
            OtherConfigAndTagsWatcher.DeregisterEventHandlers();
            Alert.DeregisterAlertCollectionChanged(XenCenterAlerts_CollectionChanged);
            Updates.UpdateAlertCollectionChanged -= Updates_CollectionChanged;
            Updates.CheckForClientUpdatesStarted -= ClientUpdatesCheck_Started;
            Updates.CheckForClientUpdatesCompleted -= ClientUpdatesCheck_Completed;
            ConnectionsManager.History.CollectionChanged -= History_CollectionChanged;
            ConnectionsManager.XenConnections.CollectionChanged -= XenConnection_CollectionChanged;
            Properties.Settings.Default.SettingChanging -= Default_SettingChanging;
            eventsPage.GoToXenObjectRequested -= eventsPage_GoToXenObjectRequested;
            SearchPage.SearchChanged -= SearchPanel_SearchChanged;
        }

        private void Default_SettingChanging(object sender, SettingChangingEventArgs e)
        {
			if (e == null)
				return;

            if (e.SettingName == "AutoSwitchToRDP" || e.SettingName == "EnableRDPPolling")
            {
                ConsolePanel.ResetAllViews();

				if (SelectionManager.Selection.FirstIsRealVM)
					ConsolePanel.SetCurrentSource((VM)SelectionManager.Selection.First);
                else if (SelectionManager.Selection.FirstIs<Host>())
                    ConsolePanel.SetCurrentSource((Host)SelectionManager.Selection.First);

                ConsolePanel.UnpauseActiveView(sender == TheTabControl);
            }
        }

        private void SetMenuItemStartIndexes()
        {
            foreach (ToolStripMenuItem menu in MainMenuBar.Items)
            {
                foreach (ToolStripItem item in menu.DropDownItems)
                {
                    if (item != null && item.Text == "PluginItemsPlaceHolder")
                    {
                        pluginMenuItemStartIndexes.Add(menu, menu.DropDownItems.IndexOf(item));
                        menu.DropDownItems.Remove(item);
                        break;
                    }
                }
            }
        }

        internal SelectionBroadcaster SelectionManager
        {
            get
            {
                return navigationPane.SelectionManager;
            }
        }

        internal ContextMenuBuilder ContextMenuBuilder
        {
            get
            {
                return contextMenuBuilder;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Program.AssertOnEventThread();

            History.EnableHistoryButtons();
            History.NewHistoryItem(new XenModelObjectHistoryItem(null, TabPageHome));
            Text = BrandManager.BrandConsole;

            /*
             * Resume window size and location
             */
            try
            {
                // Bring in previous version user setting for the first time.  
                if (Properties.Settings.Default.DoUpgrade)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.DoUpgrade = false;
                    XenAdmin.Settings.TrySaveSettings();
                }

                Point savedLocation = Properties.Settings.Default.WindowLocation;
                Size savedSize = Properties.Settings.Default.WindowSize;

                if (HelpersGUI.WindowIsOnScreen(savedLocation, savedSize))
                {
                    this.Location = savedLocation;
                    this.Size = savedSize;
                }
            }
            catch
            {
                // ignored
            }

            // Using the Load event ensures that the handle has been 
            // created:
            base.OnLoad(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            TheTabControl.Visible = true;
            alertPage.Visible = updatesPage.Visible = eventsPage.Visible = false;
            SetFiltersLabel();
            alertPage.FiltersChanged += NotificationsPage_FiltersChanged;
            updatesPage.FiltersChanged += NotificationsPage_FiltersChanged;
            eventsPage.FiltersChanged += NotificationsPage_FiltersChanged;
            navigationPane.FocusTreeView();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Clip.RegisterClipboardViewer();
        }

        protected override void WndProc(ref System.Windows.Forms.Message e)
        {
            //System.Console.WriteLine(Win32.GetWindowsMessageName(e.Msg));
            switch (e.Msg)
            {
                case Win32.WM_CHANGECBCHAIN: // Clipboard chain has changed.
                    Clip.ProcessWMChangeCBChain(e);
                    break;

                case Win32.WM_DRAWCLIPBOARD: // Content of clipboard has changed.
                    Clip.ProcessWMDrawClipboard(e);
                    break;

                case Win32.WM_DESTROY:
                    Clip.UnregisterClipboardViewer();
                    base.WndProc(ref e);
                    break;

                default:
                    base.WndProc(ref e);
                    break;
            }
        }

        private void AddTabContents(Control contents, TabPage TabPage)
        {
            contents.Location = new Point(0, 0);
            contents.Size = TabPage.Size;
            contents.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Right;
            TabPage.Controls.Add(contents);
        }

        private void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (Program.Exiting)
                return;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    if (e.Element is ActionBase action)
                    {
                        if (!(action is MeddlingAction))
                        {
                            Program.Invoke(this, () =>
                            {
                                SetStatusBar(null, null);
                                statusBarAction = action;
                            });
                        }

                        action.Changed += actionChanged;
                        action.Completed += actionCompleted;
                        actionChanged(action);
                    }
                    break;

                case CollectionChangeAction.Remove:
                    if (e.Element is ActionBase actionB)
                    {
                        actionB.Changed -= actionChanged;
                        actionB.Completed -= actionCompleted;
                    }
                    else if (e.Element is List<ActionBase> range)
                    {
                        foreach (var a in range)
                        {
                            a.Changed -= actionChanged;
                            a.Completed -= actionCompleted;
                        }
                    }
                    else
                        return;

                    UpdateErrorStatusLabel();
                    break;
            }
        }

        private void actionCompleted(ActionBase action)
        {
            action.Changed -= actionChanged;
            action.Completed -= actionCompleted;

            actionChanged(action);

            if (action is ISrAction)
                Program.Invoke(this, UpdateToolbars);
        }

        private void actionChanged(ActionBase action)
        {
            if (Program.Exiting)
                return;

            Program.Invoke(this, () =>
            {
                UpdateStatusProgressBar(action);
                UpdateErrorStatusLabel();
            });
        }

        private void UpdateStatusProgressBar(ActionBase action)
        {
            if (statusBarAction != action)
                return;

            statusProgressBar.Visible = action.ShowProgress && !action.IsCompleted;

            var percentage = action.PercentComplete;
            Debug.Assert(0 <= percentage && percentage <= 100,
                "PercentComplete is out of range, the reporting action needs to be fixed."); //CA-8517

            if (percentage < 0)
                percentage = 0;
            else if (percentage > 100)
                percentage = 100;
            statusProgressBar.Value = percentage;

            // Don't show cancelled exception
            if (action.Exception != null && !(action.Exception is CancelledException))
            {
                SetStatusBar(Images.StaticImages._000_error_h32bit_16, action.Exception.Message);
            }
            else
            {
                SetStatusBar(null, action.IsCompleted
                    ? null
                    : !string.IsNullOrEmpty(action.Description)
                        ? action.Description
                        : !string.IsNullOrEmpty(action.Title)
                            ? action.Title
                            : null);
            }
        }

        private void UpdateErrorStatusLabel()
        {
            int errorCount = ConnectionsManager.History.Count(a =>
                a.IsCompleted && !a.Succeeded && !(a is CancellingAction ca && ca.Cancelled));

            navigationPane.UpdateNotificationsButton(NotificationsSubMode.Events, errorCount);

            var errorText = errorCount == 1
                ? Messages.NOTIFICATIONS_SUBMODE_EVENTS_STATUS_ERROR
                : string.Format(Messages.NOTIFICATIONS_SUBMODE_EVENTS_STATUS_ERRORS, errorCount);

            int progressCount = ConnectionsManager.History.Count(a => !a.IsCompleted);
            var progressText = string.Format(Messages.NOTIFICATIONS_SUBMODE_EVENTS_STATUS_IN_PROGRESS, progressCount);

            if (errorCount > 0 && progressCount > 0)
                statusLabelErrors.Text = string.Format(Messages.NOTIFICATIONS_SUBMODE_EVENTS_STATUS,
                    string.Format(Messages.STRING_COMMA_SPACE_STRING, errorText, progressText));
            else if (errorCount > 0)
                statusLabelErrors.Text = string.Format(Messages.NOTIFICATIONS_SUBMODE_EVENTS_STATUS, errorText);
            else if (progressCount > 0)
                statusLabelErrors.Text = string.Format(Messages.NOTIFICATIONS_SUBMODE_EVENTS_STATUS, progressText);

            statusLabelErrors.Visible = errorCount > 0 || progressCount > 0;

            if (eventsPage.Visible)
            {
                TitleLabel.Text = NotificationsSubModeItem.GetText(NotificationsSubMode.Events, errorCount);
                TitleIcon.Image = NotificationsSubModeItem.GetImage(NotificationsSubMode.Events, errorCount);
            }
        }

        private void SetStatusBar(Image image, string message)
        {
            statusLabel.Image = image;
            statusLabel.Text = Helpers.FirstLine(message);
        }

        public void CloseSplashScreen()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                // Sleep a short time before closing the splash
                Thread.Sleep(500);
                Program.Invoke(Program.MainWindow, () => CloseSplashRequested?.Invoke());
            });
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            MainMenuBar.Location = new Point(0, 0);

            var rendProf = ToolStrip.Renderer as ToolStripProfessionalRenderer;
            if (rendProf != null)
                rendProf.RoundedEdges = false;

            RequestRefreshTreeView();

            ConnectionsManager.XenConnections.CollectionChanged += XenConnection_CollectionChanged;
            
            //no need to catch ConfigurationErrorsException as the settings have already been loaded
            Settings.RestoreSession();

            // if there are fewer than 30 connections, then expand the tree nodes.
            expandTreeNodesOnStartup = ConnectionsManager.XenConnectionsCopy.Count < 30;

            connectionsInProgressOnStartup = 0;
            // kick-off connections for all the loaded server list
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.SaveDisconnected)
                {
                    connectionsInProgressOnStartup++;
                    connection.ConnectionStateChanged += Connection_ConnectionStateChangedOnStartup;
                    connection.CachePopulated += connection_CachePopulatedOnStartup;
                    XenConnectionUI.BeginConnect(connection, true, this, true);
                }
            }

            CloseSplashScreen();

            if (!Program.RunInAutomatedTestMode && !Helpers.CommonCriteriaCertificationRelease)
            {
                if (!Properties.Settings.Default.SeenAllowUpdatesDialog)
                    using (var dlg = new NoIconDialog(string.Format(Messages.ALLOWED_UPDATES_DIALOG_MESSAGE, BrandManager.BrandConsole, BrandManager.ProductBrand),
                        ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                    {
                        HelpButton = true,
                        HelpNameSetter = "AllowUpdatesDialog",
                        ShowCheckbox = true,
                        CheckboxCaption = Messages.ALLOWED_UPDATES_DIALOG_CHECKBOX
                    })
                    {
                        var result = dlg.ShowDialog(this) == DialogResult.Yes;

                        Properties.Settings.Default.AllowXenCenterUpdates = result;
                        Properties.Settings.Default.AllowPatchesUpdates = result;
                        Properties.Settings.Default.AllowXenServerUpdates = result;
                        Properties.Settings.Default.SeenAllowUpdatesDialog = true;

                        if (result && dlg.IsCheckBoxChecked)
                        {
                            using (var dialog = new OptionsDialog(PluginManager))
                            {
                                dialog.SelectConnectionOptionsPage();
                                dialog.ShowDialog(this);
                            }
                        }

                        Settings.TrySaveSettings();
                    }

                // start checkforupdates thread
                CheckForUpdatesTimer.Interval = 1000 * 60 * 60 * 24; // 24 hours
                CheckForUpdatesTimer.Tick += CheckForUpdatesTimer_Tick;
                CheckForUpdatesTimer.Start();
                Updates.CheckForClientUpdates();
                Updates.CheckForServerUpdates();
            }

            ProcessCommand(_commandLineArgs);
        }

        private void CheckForUpdatesTimer_Tick(object sender, EventArgs e)
        {
            Updates.CheckForClientUpdates();
            Updates.CheckForServerUpdates();
        }

        private void LoadTasksAsMeddlingActions(IXenConnection connection)
        {
            if (!connection.IsConnected || connection.Session == null)
                return;

            Dictionary<XenRef<Task>, Task> tasks = Task.get_all_records(connection.Session);
            foreach (KeyValuePair<XenRef<Task>, Task> pair in tasks)
            {
                pair.Value.Connection = connection;
                pair.Value.opaque_ref = pair.Key;
                MeddlingActionManager.ForceAddTask(pair.Value);
            }
        }

        private void connection_CachePopulatedOnStartup(IXenConnection c)
        {
            c.CachePopulated -= connection_CachePopulatedOnStartup;
            if (expandTreeNodesOnStartup)
                TrySelectNewNode(c, false, true, false);

            Program.Invoke(this, ShowAboutDialogOnStartup);
        }

        private void Connection_ConnectionStateChangedOnStartup(IXenConnection c)
        {
            c.ConnectionStateChanged -= Connection_ConnectionStateChangedOnStartup;

            Program.Invoke(Program.MainWindow, delegate
            {
                connectionsInProgressOnStartup--;
                // show the About dialog if this was the last connection in progress and the connection failed
                if (!c.IsConnected)
                    ShowAboutDialogOnStartup();
            }); 
        }

        /// <summary>
        /// Show the About dialog after all conncections kicked-off on startup have finished the connection phase (cache populated)
        /// Must be called on the event thread.
        /// </summary>
        private void ShowAboutDialogOnStartup()
        {
            Program.AssertOnEventThread();
            if (connectionsInProgressOnStartup > 0)
                return;
            if (Properties.Settings.Default.ShowAboutDialog && HiddenFeatures.LicenseNagVisible)
                ShowForm(typeof(AboutDialog));
        }

        #region Commnad line args processing

        internal void ProcessCommand(params string[] args)
        {
            if (args != null && args.Length > 1)
            {
                switch (args[0])
                {
                    case "import":
                        log.DebugFormat("CLI: Importing VM export from {0}", args[1]);
                        OpenGlobalImportWizard(args[1]);
                        break;
                    case "license":
                        log.DebugFormat("CLI: Installing license from {0}", args[1]);
                        LaunchLicensePicker(args[1]);
                        break;
                    case "restore":
                        log.DebugFormat("CLI: Restoring host backup from {0}", args[1]);
                        new RestoreHostFromBackupCommand(this, null, args[1]).Run();
                        break;
                    case "search":
                        log.DebugFormat("CLI: Importing saved XenSearch from '{0}'", args[1]);
                        new ImportSearchCommand(this, args[1]).Run();
                        break;
                    case "connect":
                        log.DebugFormat("CLI: Connecting to server '{0}'", args[1]);

                        var connection = new XenConnection
                        {
                            Hostname = args[1],
                            Port = ConnectionsManager.DEFAULT_XEN_PORT,
                            Username = args.Length > 2 ? args[2] : "",
                            Password = args.Length > 3 ? args[3] : ""
                        };

                        if (File.Exists(args[1]))
                            XenConnectionUI.ConnectToXapiDatabase(connection, this);
                        else
                            XenConnectionUI.BeginConnect(connection, true, this, false);
                        break;
                    default:
                        log.Warn("CLI: Wrong syntax or unknown command line options.");
                        break;
                }
            }

            HelpersGUI.BringFormToFront(this);
        }

        #endregion


        /// <summary>
        /// Manages UI and network updates whenever hosts are added and removed
        /// </summary>
        private void XenConnection_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (Program.Exiting)
                return;

            //Program.AssertOnEventThread();
            Program.BeginInvoke(Program.MainWindow, () => XenConnectionCollectionChanged(e));
        }
        
        private readonly CollectionChangeEventHandler PoolCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler MessageCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler HostCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler VMCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler SRCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler FolderCollectionChangedWithInvoke = null;
        private readonly CollectionChangeEventHandler TaskCollectionChangedWithInvoke = null;

        private void XenConnectionCollectionChanged(CollectionChangeEventArgs e)
        {
            try
            {
                IXenConnection connection = e.Element as IXenConnection;

                navigationPane.XenConnectionCollectionChanged(e);

                if (e.Action == CollectionChangeAction.Add)
                {
                    if (connection == null)
                        return;

                    connection.ClearingCache += connection_ClearingCache;
                    connection.ConnectionResult += Connection_ConnectionResult;
                    connection.ConnectionLost += Connection_ConnectionLost;
                    connection.ConnectionClosed += Connection_ConnectionClosed;
                    connection.ConnectionReconnecting += connection_ConnectionReconnecting;
                    connection.XenObjectsUpdated += Connection_XenObjectsUpdated;
                    connection.Cache.RegisterCollectionChanged<XenAPI.Message>(MessageCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                    connection.Cache.RegisterCollectionChanged<Folder>(FolderCollectionChangedWithInvoke);

                    connection.Cache.RegisterCollectionChanged<Task>(TaskCollectionChangedWithInvoke);

                    connection.CachePopulated += connection_CachePopulated;
                }
                else if (e.Action == CollectionChangeAction.Remove)
                {
                    var range = new List<IXenConnection>();
                    if (connection != null)
                    {
                        range.Add(connection);
                    }
                    else
                    {
                        var r = e.Element as List<IXenConnection>;
                        if (r != null)
                            range = r;
                        else
                            return;
                    }

                    foreach (var con in range)
                    {
                        con.ClearingCache -= connection_ClearingCache;
                        con.ConnectionResult -= Connection_ConnectionResult;
                        con.ConnectionLost -= Connection_ConnectionLost;
                        con.ConnectionClosed -= Connection_ConnectionClosed;
                        con.ConnectionReconnecting -= connection_ConnectionReconnecting;
                        con.XenObjectsUpdated -= Connection_XenObjectsUpdated;
                        con.Cache.DeregisterCollectionChanged<XenAPI.Message>(MessageCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                        con.Cache.DeregisterCollectionChanged<Folder>(FolderCollectionChangedWithInvoke);

                        con.Cache.DeregisterCollectionChanged<Task>(TaskCollectionChangedWithInvoke);

                        con.CachePopulated -= connection_CachePopulated;

                        foreach (VM vm in con.Cache.VMs)
                        {
                            ConsolePanel.CloseVncForSource(vm);
                        }

                        foreach (Host host in con.Cache.Hosts)
                        {
                            ConsolePanel.CloseVncForSource(host.ControlDomainZero());

                            foreach (VM vm in host.OtherControlDomains())
                                CvmConsolePanel.CloseVncForSource(vm);
                        }

                        con.EndConnect();
                    }

                    //CA-41228 refresh submenu items when there are no connections
                    SelectionManager.RefreshSelection();
                }
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can't do any more about this.
            }
        }

        /// <summary>
        /// Closes any wizards for this connection. Must be done before we clear the cache so that per-VM wizards are closed.
        /// In many cases this is already covered (e.g. if the user explicitly disconnects). This method ensures we also
        /// do it when we unexpectedly lose the connection.
        /// </summary>
        private void connection_ClearingCache(IXenConnection connection)
        {
            CloseActiveWizards(connection);
            Alert.RemoveAlert(alert => alert.Connection != null && alert.Connection.Equals(connection));
            Updates.RefreshUpdateAlerts(Updates.UpdateType.ServerPatches | Updates.UpdateType.ServerVersion);

            RequestRefreshTreeView();
        }

        private void connection_CachePopulated(IXenConnection connection)
        {
            Host coordinator = Helpers.GetCoordinator(connection);
            if (coordinator == null)
                return;

            log.InfoFormat("Connected to {0} (version {1}, build {2}.{3}) with {4} {5}",
                Helpers.GetName(coordinator), Helpers.HostProductVersionText(coordinator), Helpers.HostProductVersion(coordinator),
                coordinator.BuildNumberRaw(), BrandManager.BrandConsole, Program.Version);

            // Check the PRODUCT_BRAND
            if (!Program.RunInAutomatedTestMode && !SameProductBrand(coordinator))
            {
                connection.EndConnect();

                Program.Invoke(Program.MainWindow, delegate
                {
                    var title = string.Format(Messages.CONNECTION_REFUSED_TITLE, Helpers.GetName(coordinator).Ellipsise(80));
                    new DummyAction(title, "", string.Format(Messages.INCOMPATIBLE_PRODUCTS, BrandManager.BrandConsole)).Run();

                    using (var dlog = new ErrorDialog(string.Format(Messages.INCOMPATIBLE_PRODUCTS, BrandManager.BrandConsole))
                        {WindowTitle = title})
                        dlog.ShowDialog(this);
                });
                return;
            }

            //check the pool has no supporters earlier than the lowest supported version 
            //(could happen if trying to connect to a partially upgraded pool where
            //the newest hosts have been upgraded using a earlier XenCenter)

            var supporters = connection.Cache.Hosts.Where(h => h.opaque_ref != coordinator.opaque_ref);
            foreach (var supporter in supporters)
            {
                if (Helpers.NaplesOrGreater(supporter))
                    continue;

                connection.EndConnect();

                Program.Invoke(Program.MainWindow, () =>
                {
                    var title = string.Format(Messages.CONNECTION_REFUSED_TITLE, Helpers.GetName(coordinator).Ellipsise(80));
                    var msg = string.Format(Messages.SUPPORTER_TOO_OLD, BrandManager.ProductVersion712, BrandManager.BrandConsole);

                    new DummyAction(title, "", msg).Run();

                    using (var dlg = new ErrorDialog(msg, ThreeButtonDialog.ButtonOK)
                        {WindowTitle = Messages.CONNECT_TO_SERVER})
                    {
                        dlg.ShowDialog(this);
                    }
                });
                return;
            }

            // When releasing a new version of the server, we should set xencenter_min and xencenter_max on the server
            // as follows:
            //
            // xencenter_min should be the lowest version of XenCenter we want the new server to work with. In the
            // (common) case that we want to force the user to upgrade XenCenter when they upgrade the server,
            // xencenter_min should equal the current version of XenCenter.  // if (server_min > current_version)
            //
            // xencenter_max should always equal the current version of XenCenter. This ensures that even if they are
            // not required to upgrade, we at least warn them.  // else if (server_max > current_version)

            int serverMin = coordinator.XenCenterMin();
            int serverMax = coordinator.XenCenterMax();

            if (serverMin > 0 && serverMax > 0)
            {
                int currentVersion = (int)API_Version.LATEST;

                if (serverMin > currentVersion)
                {
                    connection.EndConnect();

                    Program.Invoke(Program.MainWindow, delegate
                    {
                        var msg = string.Format(Messages.GUI_OUT_OF_DATE, BrandManager.BrandConsole, Helpers.GetName(coordinator));
                        var url = InvisibleMessages.OUT_OF_DATE_WEBSITE;
                        var title = string.Format(Messages.CONNECTION_REFUSED_TITLE, Helpers.GetName(coordinator).Ellipsise(80));
                        var error = $"{msg}\n{url}";

                        new DummyAction(title, "", error).Run();

                        using (var dlog = new ErrorDialog(msg)
                        {
                            WindowTitle = title,
                            ShowLinkLabel = !HiddenFeatures.LinkLabelHidden,
                            LinkText = url,
                            LinkData = url
                        })
                            dlog.ShowDialog(this);
                    });
                    return;
                }

                // Allow connection only to Naples or greater versions

                if (!Helpers.NaplesOrGreater(coordinator))
                {
                    connection.EndConnect();

                    Program.Invoke(Program.MainWindow, delegate
                    {
                        var msg = string.Format(Messages.GUI_NOT_COMPATIBLE, BrandManager.BrandConsole, BrandManager.ProductVersion712,
                            BrandManager.ProductVersion80, Helpers.GetName(coordinator));
                        var url = InvisibleMessages.OUT_OF_DATE_WEBSITE;
                        var title = string.Format(Messages.CONNECTION_REFUSED_TITLE, Helpers.GetName(coordinator).Ellipsise(80));
                        var error = $"{msg}\n{url}";

                        new DummyAction(title, "", error).Run();

                        using (var dlog = new ErrorDialog(msg)
                        {
                            WindowTitle = title,
                            ShowLinkLabel = !HiddenFeatures.LinkLabelHidden,
                            LinkText = url,
                            LinkData = url
                        })
                            dlog.ShowDialog(this);
                    });
                    return;
                }
                
                if (serverMax > currentVersion)
                    Alert.AddAlert(new GuiOldAlert());

                LoadTasksAsMeddlingActions(connection);
            }

            //
            // Every time we connect, make sure any host with other_config[maintenance_mode] == true
            // is disabled.
            //
            CheckMaintenanceMode(connection);

            if (HelpersGUI.iSCSIisUsed())
                HelpersGUI.PerformIQNCheck();

            if(licenseTimer != null)
                licenseTimer.CheckActiveServerLicense(connection, false);

            if (BrandManager.BrandConsole == "[XenCenter]" || BrandManager.BrandConsole == "XenCenter")
            {
                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool != null && pool.GetHealthCheckStatus() == Pool.HealthCheckStatus.Enabled)
                {
                    Program.BeginInvoke(Program.MainWindow, () =>
                    {
                        using (var dlg = new InformationDialog(
                                   string.Format(Messages.PROBLEM_HEALTH_CHECK_ON_CONNECTION, pool.Name(), BrandManager.BrandConsole),
                                   ThreeButtonDialog.ButtonOK))
                        {
                            if (dlg.ShowDialog() == DialogResult.OK)
                                new DisableHealthCheckAction(pool).RunAsync();
                        }
                    });
                }
            }

            Updates.RefreshUpdateAlerts(Updates.UpdateType.ServerPatches | Updates.UpdateType.ServerVersion);
            Updates.CheckHotfixEligibility(connection);
            RequestRefreshTreeView();

            CheckTlsVerification(connection);
        }

        private void CheckTlsVerification(IXenConnection connection)
        {
            //Use BeginInvoke so the UI is not blocked in a connection-in-progress state

            Program.BeginInvoke(Program.MainWindow, () =>
            {
                var pool = Helpers.GetPoolOfOne(connection);
                var cmd = new EnableTlsVerificationCommand(Program.MainWindow, pool, false);

                if (cmd.CanRun())
                {
                    var msg = string.Format("{0}\n\n{1}",
                        string.Format(Messages.MESSAGEBOX_ENABLE_TLS_VERIFICATION_BLURB, Helpers.GetName(connection)),
                        Messages.MESSAGEBOX_ENABLE_TLS_VERIFICATION_WARNING);

                    using (var dlg = new WarningDialog(msg,
                        new ThreeButtonDialog.TBDButton(Messages.MESSAGEBOX_ENABLE_TLS_VERIFICATION_BUTTON,
                            DialogResult.Yes, ThreeButtonDialog.ButtonType.ACCEPT, true),
                        ThreeButtonDialog.ButtonNo))
                        if (dlg.ShowDialog(this) == DialogResult.Yes)
                            cmd.Run();
                }
            });
        }

        private static bool SameProductBrand(Host host)
        {
            var brand = host.ProductBrand();
            return brand == BrandManager.ProductBrand || brand == BrandManager.LegacyProduct ||
                   BrandManager.ProductBrand == "[XenServerProduct]";
        }

        /// <summary>
        /// Ensures all hosts on the connection are disabled if they are in maintenance mode.
        /// </summary>
        /// <param name="connection"></param>
        private void CheckMaintenanceMode(IXenConnection connection)
        {
            foreach (Host host in connection.Cache.Hosts)
            {
                CheckMaintenanceMode(host);
            }
        }

        /// <summary>
        /// Ensures the host is disabled if it is in maintenance mode by spawning a new HostAction if necessary.
        /// </summary>
        /// <param name="host"></param>
        private void CheckMaintenanceMode(Host host)
        {
            if (host.IsLive() && host.MaintenanceMode() && host.enabled)
            {
                Program.Invoke(this, () => XenDialogBase.CloseAll(host));

                var action = new DisableHostAction(host);
                action.Completed += action_Completed;
                action.RunAsync();
            }
        }

        void MessageCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            XenAPI.Message m = (XenAPI.Message)e.Element;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (!m.ShowOnGraphs() && !m.IsSquelched())
                    Alert.AddAlert(MessageAlert.ParseMessage(m));
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                if (!m.ShowOnGraphs())
                    MessageAlert.RemoveAlert(m);
            }
        }

        void CollectionChanged<T>(object sender, CollectionChangeEventArgs e) where T : XenObject<T>
        {
            Program.AssertOnEventThread();

            T o = (T)e.Element;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (o is Pool)
                    ((Pool)e.Element).PropertyChanged += Pool_PropertyChanged;
                else if (o is Host)
                    ((Host)e.Element).PropertyChanged += Host_PropertyChanged;
                else if (o is VM)
                    ((VM)e.Element).PropertyChanged += VM_PropertyChanged;
                else
                    o.PropertyChanged += o_PropertyChanged;
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                if (o is Pool)
                    ((Pool)e.Element).PropertyChanged -= Pool_PropertyChanged;
                else if (o is Host)
                    ((Host)e.Element).PropertyChanged -= Host_PropertyChanged;
                else if (o is VM)
                    ((VM)e.Element).PropertyChanged -= VM_PropertyChanged;
                else
                    o.PropertyChanged -= o_PropertyChanged;

                if (o is VM)
                {
                    VM vm = (VM)e.Element;
                    ConsolePanel.CloseVncForSource(vm);
                    XenDialogBase.CloseAll(vm);
                }

                selectedTabs.Remove(o);
                PluginManager.DisposeURLs(o);
            }
        }

        private void Pool_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            Pool pool = (Pool)obj;
            switch (e.PropertyName)
            {
                case "other_config":
                    // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    // other_config contains which patches to ignore
                    Updates.RefreshUpdateAlerts(Updates.UpdateType.ServerPatches | Updates.UpdateType.ServerVersion);
                    break;

                case "name_label":
                    pool.Connection.FriendlyName = Helpers.GetName(pool);
                    break;
            }
        }

        private void Host_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            Host host = (Host)obj;
            switch (e.PropertyName)
            {
                case "allowed_operations":
                case "enabled":
                    // We want to ensure that a host is disabled if it is in maintenance mode, by starting a new DisableHostAction if necessary (CheckMaintenanceMode)
                    if (host.enabled && host.MaintenanceMode())
                    {
                        // This is an invalid state: the host is enabled but still "in maintenance mode";
                        // But maybe MaintenanceMode hasn't been updated yet, because host.enabled being processed before host.other_config (CA-75625);
                        // We'll check it again after the cache update operation is complete, in Connection_XenObjectsUpdated 
                        hostsInInvalidState.Add(host);
                    }
                    UpdateToolbars();
                    break;
                case "edition":
                case "license_server":
                case "license_params":
                    UpdateHeader();
                    UpdateToolbars();
                    Updates.CheckHotfixEligibility(host.Connection);
                    break;
                case "other_config":
                    // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    break;

                case "name_label":
                    //check whether it's a standalone host
                    if(Helpers.GetPool(host.Connection) == null)
                        host.Connection.FriendlyName = Helpers.GetName(host);
                    break;

                case "patches":
                    if (!Helpers.ElyOrGreater(host))
                        Updates.RefreshUpdateAlerts(Updates.UpdateType.ServerPatches | Updates.UpdateType.ServerVersion);
                    break;
                case "updates":
                    if (Helpers.ElyOrGreater(host))
                        Updates.RefreshUpdateAlerts(Updates.UpdateType.ServerPatches | Updates.UpdateType.ServerVersion);
                    break;
            }
        }

        private void VM_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            VM vm = (VM)obj;
            switch (e.PropertyName)
            {
                case "allowed_operations":
                case "is_a_template":
                case "resident_on":
                    UpdateToolbars();
                    break;

                case "power_state":
                case "other_config": // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    // Make all vms have the correct start times
                    UpdateBodgedTime(vm, e.PropertyName);
                    break;
            }
        }

        void o_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "allowed_operations":
                case "power_state":
                case "is_a_template":
                case "enabled":
                case "other_config": // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    break;
            }
        }

        // Update bodged startup time if the powerstate goes to running (vm started from halted), otherconfig last shutdown changed (vm rebooted) or start time changed (occurs a few seconds after start)
        private void UpdateBodgedTime(VM vm, string p)
        {
            if (vm == null)
                return;
            if (p == "power_state")
            {
                vm.SetBodgeStartupTime(DateTime.UtcNow); // always newer than current bodge startup time
            }
            else if (p == "other_config" && vm.other_config.ContainsKey("last_shutdown_time"))
            {
                DateTime newTime = vm.LastShutdownTime();
                if (newTime != DateTime.MinValue && newTime.Ticks > vm.GetBodgeStartupTime().Ticks)
                    vm.SetBodgeStartupTime(newTime); // only update if is newer than current bodge startup time
            }
        }

        private void Connection_ConnectionResult(object sender, Network.ConnectionResultEventArgs e)
        {
            RequestRefreshTreeView();
        }

        private void Connection_ConnectionClosed(IXenConnection conn)
        {
            RequestRefreshTreeView();
            gc();
        }

        // called whenever our connection with the Xen server fails (i.e., after we've successfully logged in)
        private void Connection_ConnectionLost(IXenConnection conn)
        {
            if (Program.Exiting)
                return;
            Program.Invoke(this, () => CloseActiveWizards(conn));
            RequestRefreshTreeView();
            gc();
        }

        private static void gc()
        {
            GC.Collect();
        }

        void connection_ConnectionReconnecting(IXenConnection conn)
        {
            if (Program.Exiting)
                return;
            RequestRefreshTreeView();
            gc();
        }

        private List<Host> hostsInInvalidState = new List<Host>();

        // called whenever Xen objects on the server change state
        void Connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            if (Program.Exiting)
                return;

            IXenConnection connection = (IXenConnection) sender;

            if (hostsInInvalidState.Count > 0)
            {
                foreach (var host in hostsInInvalidState.Where(host => host.Connection == connection))
                    CheckMaintenanceMode(host);
                hostsInInvalidState.RemoveAll(host => host.Connection == connection);
            }

            RequestRefreshTreeView();
        }

        /// <summary>
        /// Requests a refresh of the main tree view. The refresh will be managed such that we are not overloaded using an UpdateManager.
        /// </summary>
        public void RequestRefreshTreeView()
        {
            Program.Invoke(this, navigationPane.RequestRefreshTreeView);
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool _menuShortcutsEnabled = true;
        public bool MenuShortcutsEnabled
        {
            get { return _menuShortcutsEnabled; }
            set
            {
                if (value != _menuShortcutsEnabled)
                {
                    //if the VNC Console is active (the user is typing into it etc) all of the shortcuts for XenCenter are disabled
                    //IMPORTANT! add any shortcuts you want to pass to the VNC console into this if, else statement
                    _menuShortcutsEnabled = value;

                    // update the selection so menu items can enable/disable keyboard shortcuts as appropriate.
                    SelectionManager.RefreshSelection();
                }
            }
        }

        /// <summary>
        /// Must be called on the event thread.
        /// </summary>
        public void UpdateToolbars()
        {
            Program.AssertOnEventThread();

            try
            {
                ToolStrip.SuspendLayout();
                UpdateToolbarsCore();
                MainMenuBar_MenuActivate(null, null);
            }
            finally
            {
                ToolStrip.ResumeLayout();
            }

            // Save and restore focus on treeView, since selecting tabs in ChangeToNewTabs() has the
            // unavoidable side-effect of giving them focus - this is irritating if trying to navigate
            // the tree using the keyboard.

            navigationPane.SaveAndRestoreTreeViewFocus(ChangeToNewTabs);
        }

        private static int TOOLBAR_HEIGHT = 31;

        /// <summary>
        /// Updates the toolbar buttons.
        /// </summary>
        private void UpdateToolbarsCore()
        {
            // refresh the selection-manager
            SelectionManager.RefreshSelection();

            ToolStrip.Height = Properties.Settings.Default.ToolbarsEnabled ? TOOLBAR_HEIGHT : 0;
            ToolStrip.Enabled = Properties.Settings.Default.ToolbarsEnabled;
            ShowToolbarMenuItem.Checked = toolbarToolStripMenuItem.Checked = Properties.Settings.Default.ToolbarsEnabled;

            bool containerButtonsAvailable = startContainerToolStripButton.Enabled || stopContainerToolStripButton.Enabled || 
                resumeContainerToolStripButton.Enabled || pauseContainerToolStripButton.Enabled || restartContainerToolStripButton.Enabled;

            startContainerToolStripButton.Available = containerButtonsAvailable && startContainerToolStripButton.Enabled;
            stopContainerToolStripButton.Available = containerButtonsAvailable && (stopContainerToolStripButton.Enabled || !startContainerToolStripButton.Available);
            resumeContainerToolStripButton.Available = containerButtonsAvailable && resumeContainerToolStripButton.Enabled;
            pauseContainerToolStripButton.Available = containerButtonsAvailable && (pauseContainerToolStripButton.Enabled || !resumeContainerToolStripButton.Available);
            restartContainerToolStripButton.Available = containerButtonsAvailable;

            powerOnHostToolStripButton.Available = powerOnHostToolStripButton.Enabled;
            startVMToolStripButton.Available = startVMToolStripButton.Enabled;
            shutDownToolStripButton.Available = shutDownToolStripButton.Enabled || (!startVMToolStripButton.Available && !powerOnHostToolStripButton.Available && !containerButtonsAvailable);
            RebootToolbarButton.Available = RebootToolbarButton.Enabled || !containerButtonsAvailable;

            resumeToolStripButton.Available = resumeToolStripButton.Enabled;
            SuspendToolbarButton.Available = SuspendToolbarButton.Enabled || (!resumeToolStripButton.Available && !containerButtonsAvailable);

            ForceRebootToolbarButton.Available = ((ForceVMRebootCommand)ForceRebootToolbarButton.Command).ShowOnMainToolBar;
            ForceShutdownToolbarButton.Available = ((ForceVMShutDownCommand)ForceShutdownToolbarButton.Command).ShowOnMainToolBar;
        }

        private List<TabPage> GetNewTabPages()
        {
            IXenConnection selectionConnection = SelectionManager.Selection.GetConnectionOfFirstItem();
            Pool selectionPool = selectionConnection == null ? null : Helpers.GetPool(selectionConnection);

            // 'Home' tab is only visible if the 'Overview' tree node is selected, or if the tree is
            // empty (i.e. at startup).
            bool show_home = SelectionManager.Selection.Count == 1 && SelectionManager.Selection[0].Value == null;
            // The upsell pages use the first selected XenObject: but they're only shown if there is only one selected object (see calls to ShowTab() below).
            bool ha_upsell = Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictHA) && (selectionPool != null && !selectionPool.ha_enabled);
            bool wlb_upsell = Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictWLB);
            bool ad_upsell = Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictAD);
            bool is_connected = selectionConnection != null && selectionConnection.IsConnected;

            bool multi = SelectionManager.Selection.Count > 1;

            bool isPoolSelected = SelectionManager.Selection.FirstIs<Pool>();
            bool isVMSelected = SelectionManager.Selection.FirstIs<VM>();
            bool isHostSelected = SelectionManager.Selection.FirstIs<Host>();
            SR selectedSr = SelectionManager.Selection.First as SR;
            bool isSRSelected = selectedSr != null;
            bool isVdiSelected = SelectionManager.Selection.FirstIs<VDI>();
            bool isRealVMSelected = SelectionManager.Selection.FirstIsRealVM;
            bool isTemplateSelected = SelectionManager.Selection.FirstIsTemplate;
            bool isHostLive = SelectionManager.Selection.FirstIsLiveHost;
            bool isDockerContainerSelected = SelectionManager.Selection.First is DockerContainer;

            bool selectedTemplateHasProvisionXML = SelectionManager.Selection.FirstIsTemplate && ((VM)SelectionManager.Selection[0].XenObject).HasProvisionXML();

            var newTabs = new List<TabPage>();

            if (!SearchMode && show_home)
                newTabs.Add(TabPageHome);

            if (!multi && !SearchMode && (isVMSelected || (isHostSelected && (isHostLive || !is_connected)) ||
                                          isPoolSelected || isSRSelected || isVdiSelected || isDockerContainerSelected))
                newTabs.Add(TabPageGeneral);

            if (!multi && !SearchMode && (isVMSelected || (isHostSelected && isHostLive) || isPoolSelected))
                newTabs.Add(TabPageBallooning);

            if (!multi && !SearchMode && (isRealVMSelected || (isTemplateSelected && !selectedTemplateHasProvisionXML)))
                newTabs.Add(TabPageStorage);

            if (!multi && !SearchMode && isSRSelected)
                newTabs.Add(TabPageSR);

            if (!multi && !SearchMode && ((isHostSelected && isHostLive) || isPoolSelected))
                newTabs.Add(TabPagePhysicalStorage);

            if (!multi && !SearchMode && (isVMSelected || (isHostSelected && isHostLive) || isPoolSelected))
                newTabs.Add(TabPageNetwork);

            if (!multi && !SearchMode && isHostSelected && isHostLive)
                newTabs.Add(TabPageNICs);

            if (!multi && !SearchMode && isDockerContainerSelected &&
                !Helpers.StockholmOrGreater(SelectionManager.Selection.GetConnectionOfFirstItem()))
            {
                if (!(SelectionManager.Selection.First as DockerContainer).Parent.IsWindows())
                    newTabs.Add(TabPageDockerProcess);

                newTabs.Add(TabPageDockerDetails);
            }

            bool isPoolOrLiveStandaloneHost = isPoolSelected || (isHostSelected && isHostLive && selectionPool == null);

            if (!multi && !SearchMode && ((isHostSelected && isHostLive) || isPoolOrLiveStandaloneHost) &&
                !Helpers.FeatureForbidden(selectionConnection, Host.RestrictGpu))
                newTabs.Add(TabPageGPU);

            if (!multi && !SearchMode && isHostSelected && isHostLive && ((Host)SelectionManager.Selection.First).PUSBs.Count > 0 && !Helpers.FeatureForbidden(selectionConnection, Host.RestrictUsbPassthrough))
                newTabs.Add(TabPageUSB);

            var consoleFeatures = new List<TabPageFeature>();
            var otherFeatures = new List<TabPageFeature>();

            if (SelectionManager.Selection.Count == 1 && !SearchMode)
                GetFeatureTabPages(SelectionManager.Selection.FirstAsXenObject, out consoleFeatures, out otherFeatures);

            foreach (var f in consoleFeatures)
                newTabs.Add(f.TabPage);

            if (consoleFeatures.Count == 0 && !multi && !SearchMode && (isRealVMSelected || (isHostSelected && isHostLive)))
                newTabs.Add(TabPageConsole);

            if (consoleFeatures.Count == 0 && !multi && !SearchMode && isSRSelected && selectedSr.HasDriverDomain(out _))
                newTabs.Add(TabPageCvmConsole);

            if (!multi && !SearchMode && (isRealVMSelected || (isHostSelected && isHostLive)))
                newTabs.Add(TabPagePeformance);

            if (!multi && !SearchMode && isPoolSelected)
                newTabs.Add(ha_upsell ? TabPageHAUpsell : TabPageHA);
            
            if(!multi && !SearchMode && isRealVMSelected)
                newTabs.Add(TabPageSnapshots);

            //Any Clearwater XenServer, or WLB is not licensed on XenServer, the WLB tab and any WLB menu items disappear completely.
            if (!wlb_upsell && !multi && !SearchMode && isPoolSelected)
                newTabs.Add(TabPageWLB);

            if (!multi && !SearchMode && (isPoolSelected || isHostSelected && isHostLive))
                newTabs.Add(ad_upsell ? TabPageADUpsell : TabPageAD);

            if (!multi && !SearchMode && isPoolOrLiveStandaloneHost && !Helpers.FeatureForbidden(SelectionManager.Selection.FirstAsXenObject, Host.RestrictPvsCache)
                && Helpers.PvsCacheCapability(selectionConnection))
                newTabs.Add(TabPagePvs);

            foreach (var f in otherFeatures)
                newTabs.Add(f.TabPage);

            newTabs.Add(TabPageSearch);

            // N.B. Change NewTabs definition if you add more tabs here.

            return newTabs;
        }

        private void GetFeatureTabPages(IXenObject xenObject, out List<TabPageFeature> consoleFeatures, out List<TabPageFeature> otherFeatures)
        {
            consoleFeatures = new List<TabPageFeature>();
            otherFeatures = new List<TabPageFeature>();

            var plugins = PluginManager.Plugins;
            foreach (var p in plugins)
            {
                var features = p.Features;
                foreach (var feature in features)
                {
                    var f = feature as TabPageFeature;
                    if (f == null)
                        continue;

                    f.SelectedXenObject = xenObject;
                    if (!f.ShowTab)
                        continue;

                    if (f.IsConsoleReplacement)
                    {
                        f.SetUrl();
                        if (!f.IsError)
                            consoleFeatures.Add(f);
                    }
                    else
                    {
                        var page = GetLastSelectedPage(xenObject);
                        if (page != null && page.Tag == f)
                            f.SetUrl();
                        otherFeatures.Add(f);
                    }
                }
            }
        }

        private void ChangeToNewTabs()
        {
            var newTabs = GetNewTabPages();

            var pageToSelect = GetLastSelectedPage(SelectionManager.Selection.First);
            if (pageToSelect != null && !newTabs.Contains(pageToSelect))
                pageToSelect = null;

            TheTabControl.SuspendLayout();
            IgnoreTabChanges = true;

            try
            {
                foreach (TabPage page in TheTabControl.TabPages)
                {
                    if (!newTabs.Contains(page))
                        TheTabControl.TabPages.Remove(page);
                }

                int m = 0; // Index into TheTabControl.TabPages

                foreach (var newTab in newTabs)
                {
                    var index = TheTabControl.TabPages.IndexOf(newTab);
                    if (index < 0)
                        TheTabControl.TabPages.Insert(m, newTab);

                    m++;

                    if (newTab == pageToSelect)
                        TheTabControl.SelectedTab = newTab;
                }

                if (pageToSelect == null)
                    TheTabControl.SelectedTab = TheTabControl.TabPages[0];
            }
            finally
            {
                IgnoreTabChanges = false;
                TheTabControl.ResumeLayout();

                SetLastSelectedPage(SelectionManager.Selection.First, TheTabControl.SelectedTab);
            }
        }

        private void SetLastSelectedPage(object o, TabPage p)
        {
            if (SearchMode)
                return;

            if (o == null || !Properties.Settings.Default.RememberLastSelectedTab)
            {
                selectedOverviewTab = p;
            }
            else
            {
                selectedTabs[o] = p;
            }
        }

        private TabPage GetLastSelectedPage(object o)
        {
            return o == null || !Properties.Settings.Default.RememberLastSelectedTab
                ? selectedOverviewTab
                : selectedTabs.ContainsKey(o) ? selectedTabs[o] : null;
        }

        private void pluginManager_PluginsChanged()
        {
            UpdateToolbars();

            foreach (ToolStripMenuItem menu in MainMenuBar.Items)
            {
                //clear existing plugin items
                for (int i = menu.DropDownItems.Count - 1; i >= 0; i--)
                {
                    CommandToolStripMenuItem commandMenuItem = menu.DropDownItems[i] as CommandToolStripMenuItem;

                    if (commandMenuItem != null && (commandMenuItem.Command is MenuItemFeatureCommand 
                                                 || commandMenuItem.Command is ParentMenuItemFeatureCommand))
                    {
                        menu.DropDownItems.RemoveAt(i);

                        if (menu.DropDownItems.Count > 0 && menu.DropDownItems[i] is ToolStripSeparator)
                            menu.DropDownItems.RemoveAt(i);
                    }
                }

                // get insert index using the placeholder

                if (!pluginMenuItemStartIndexes.ContainsKey(menu))
                    continue;

                int insertIndex = pluginMenuItemStartIndexes[menu];

                bool itemAdded = false;

                // add plugin items for this menu at insertIndex
                foreach (PluginDescriptor plugin in PluginManager.Plugins)
                {
                    if (!plugin.Enabled)
                        continue;

                    foreach (Plugins.Feature feature in plugin.Features)
                    {
                        var menuItemFeature = feature as MenuItemFeature;

                        if (menuItemFeature != null && menuItemFeature.ParentFeature == null && (int)menuItemFeature.Menu == MainMenuBar.Items.IndexOf(menu))
                        {
                            Command cmd = menuItemFeature.GetCommand(this, SelectionManager.Selection);

                            menu.DropDownItems.Insert(insertIndex, new CommandToolStripMenuItem(cmd));
                            insertIndex++;
                            itemAdded = true;
                        }

                        var parentMenuItemFeature = feature as ParentMenuItemFeature;

                        if (parentMenuItemFeature != null && (int)parentMenuItemFeature.Menu == MainMenuBar.Items.IndexOf(menu))
                        {
                            Command cmd = parentMenuItemFeature.GetCommand(this, SelectionManager.Selection);
                            CommandToolStripMenuItem parentMenuItem = new CommandToolStripMenuItem(cmd);

                            menu.DropDownItems.Insert(insertIndex, parentMenuItem);
                            insertIndex++;
                            itemAdded = true;

                            foreach (MenuItemFeature childFeature in parentMenuItemFeature.Features)
                            {
                                Command childCommand = childFeature.GetCommand(this, SelectionManager.Selection);
                                parentMenuItem.DropDownItems.Add(new CommandToolStripMenuItem(childCommand));
                            }
                        }
                    }
                }

                if (itemAdded && insertIndex != menu.DropDownItems.Count)
                    menu.DropDownItems.Insert(insertIndex, new ToolStripSeparator());
            }
        }

        private void MainMenuBar_MenuActivate(object sender, EventArgs e)
        {
            bool vm = SelectionManager.Selection.FirstIsRealVM && !((VM)SelectionManager.Selection.First).Locked;

            exportSettingsToolStripMenuItem.Enabled = ConnectionsManager.XenConnectionsCopy.Count > 0;

            MenuShortcutsEnabled = true;

            startOnHostToolStripMenuItem.Available = startOnHostToolStripMenuItem.Enabled;
            resumeOnToolStripMenuItem.Available = resumeOnToolStripMenuItem.Enabled;
            relocateToolStripMenuItem.Available = relocateToolStripMenuItem.Enabled;
            sendCtrlAltDelToolStripMenuItem.Enabled = (TheTabControl.SelectedTab == TabPageConsole) && vm && ((VM)SelectionManager.Selection.First).power_state == vm_power_state.Running;

            IXenConnection conn = SelectionManager.Selection.GetConnectionOfAllItems();
            
            bool vmssOn = conn != null && Helpers.FalconOrGreater(conn);
            assignSnapshotScheduleToolStripMenuItem.Available = vmssOn;
            VMSnapshotScheduleToolStripMenuItem.Available = vmssOn;

            templatesToolStripMenuItem1.Checked = Properties.Settings.Default.DefaultTemplatesVisible;
            customTemplatesToolStripMenuItem.Checked = Properties.Settings.Default.UserTemplatesVisible;
            localStorageToolStripMenuItem.Checked = Properties.Settings.Default.LocalSRsVisible;
            ShowHiddenObjectsToolStripMenuItem.Checked = Properties.Settings.Default.ShowHiddenVMs;
            connectDisconnectToolStripMenuItem.Enabled = ConnectionsManager.XenConnectionsCopy.Count > 0;
            conversionToolStripMenuItem.Available = conn != null && conn.Cache.VMs.Any(v => v.IsConversionVM());
            installToolsToolStripMenuItem.Available = SelectionManager.Selection.Any(v => !Helpers.StockholmOrGreater(v.Connection));
            toolStripMenuItemInstallCertificate.Available = Helpers.StockholmOrGreater(conn);
            
            toolStripMenuItemRotateSecret.Available = SelectionManager.Selection.Any(s =>
                s.Connection != null && Helpers.StockholmOrGreater(s.Connection) &&
                !s.Connection.Cache.Hosts.Any(Host.RestrictPoolSecretRotation));
            
            toolStripMenuItemEnableTls.Available = SelectionManager.Selection.Any(s =>
                s.Connection != null && Helpers.CloudOrGreater(s.Connection) && Helpers.XapiEqualOrGreater_1_290_0(s.Connection) &&
                !s.Connection.Cache.Hosts.Any(Host.RestrictCertificateVerification) &&
                s.Connection.Cache.Pools.Any(p => !p.tls_verification_enabled));

            toolStripMenuItemVtpm.Available = SelectionManager.Selection.Any(s =>
                s.Connection != null && Helpers.CloudOrGreater(s.Connection) && Helpers.XapiEqualOrGreater_22_26_0(s.Connection) &&
                !s.Connection.Cache.Hosts.Any(Host.RestrictVtpm));
        }

        private void xenSourceOnTheWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.HOMEPAGE);
        }

        private void xenCenterPluginsOnTheWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.PLUGINS_URL);
        }

        private void aboutXenSourceAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(typeof(AboutDialog));
        }

        /// <summary>
        /// Apply license, if HostAncestorOfSelectedNode is null, show host picker, if filepath == "" show filepicker
        /// </summary>
        public void LaunchLicensePicker(string filepath)
        {
            HelpersGUI.BringFormToFront(this);
            OpenFileDialog dialog = null;
            DialogResult result = DialogResult.Cancel;
            if (filepath == "")
            {
                if (!Program.RunInAutomatedTestMode)
                {
                    dialog = new OpenFileDialog();
                    dialog.Multiselect = false;
                    dialog.Title = Messages.SELECT_LICENSE_KEY;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    dialog.Filter = string.Format("{0} (*.xslic)|*.xslic|{1} (*.*)|*.*",
                        string.Format(Messages.XS_LICENSE_FILES, BrandManager.ProductBrand), Messages.ALL_FILES);
                    dialog.ShowHelp = true;
                    dialog.HelpRequest += dialog_HelpRequest;
                    result = dialog.ShowDialog(this);
                }
            }
            else
            {
                result = DialogResult.OK;
            }

            if (result == DialogResult.OK || Program.RunInAutomatedTestMode)
            {
                if (Program.RunInAutomatedTestMode)
                {
                    filepath = string.Empty;
                }
                else if (filepath == string.Empty && dialog != null)
                {
                    filepath = dialog.FileName;
                }

                Host hostAncestor = SelectionManager.Selection.Count == 1 ? SelectionManager.Selection[0].HostAncestor : null;

                if (SelectionManager.Selection.Count == 1 && hostAncestor == null)
                {
                    SelectHostDialog hostdialog = new SelectHostDialog();
                    hostdialog.TheHost = null;
                    hostdialog.Owner = this;
                    hostdialog.ShowDialog(this);
                    if (string.IsNullOrEmpty(filepath) || hostdialog.DialogResult != DialogResult.OK)
                    {
                        return;
                    }
                    hostAncestor = hostdialog.TheHost;
                }

                DoLicenseAction(hostAncestor, filepath);

            }
        }

        private void DoLicenseAction(Host host, string filePath)
        {
            //null can happen if the application is started from, say,
            //double clicking on a license file without any connections on the tree
            if (host == null)
                return;

            var action = new ApplyLicenseAction(host, filePath);
            using (var actionProgress = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
            {
                actionProgress.Text = Messages.INSTALL_LICENSE_KEY;
                actionProgress.ShowDialog(this);
            }
        }

        private void dialog_HelpRequest(object sender, EventArgs e)
        {
            Help.HelpManager.Launch("LicenseKeyDialog");
        }

        private void TheTabControl_Deselected(object sender, TabControlEventArgs e)
        {
            TabPage t = e.TabPage;
            if (t == null)
                return;
            BaseTabPage tabPage = t.Controls.OfType<BaseTabPage>().FirstOrDefault();
            if (tabPage != null)
                tabPage.PageHidden();
        }

        /// <param name="sender"></param>
        /// <param name="e">If null, then we deduce the method was called by navigation panel
        /// events (e.g. navigating in the treeView). In this case do not focus the VNC console,
        /// we only do it if the user explicitly clicked on the console tab.</param>
        private void TheTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnoreTabChanges)
                return;

            TabPage t = TheTabControl.SelectedTab;

            if (!SearchMode)
                History.NewHistoryItem(new XenModelObjectHistoryItem(SelectionManager.Selection.FirstAsXenObject, t));

            if (t == TabPageConsole)
            {
                CvmConsolePanel.PauseAllDockedViews();

                if (SelectionManager.Selection.FirstIsRealVM)
                {
                    ConsolePanel.SetCurrentSource((VM)SelectionManager.Selection.First);
                    ConsolePanel.UnpauseActiveView(e != null && sender == TheTabControl);
                }
                else if (SelectionManager.Selection.FirstIs<Host>())
                {
                    ConsolePanel.SetCurrentSource((Host)SelectionManager.Selection.First);
                    ConsolePanel.UnpauseActiveView(e != null && sender == TheTabControl);
                }
                ConsolePanel.UpdateRDPResolution();
            }
            else if (t == TabPageCvmConsole)
            {
                ConsolePanel.PauseAllDockedViews();

                if (SelectionManager.Selection.First is SR sr && sr.HasDriverDomain(out var vm))
                {
                    CvmConsolePanel.SetCurrentSource(vm);
                    CvmConsolePanel.UnpauseActiveView(e != null && sender == TheTabControl);
                }
            }
            else
            {
                ConsolePanel.PauseAllDockedViews();  
                CvmConsolePanel.PauseAllDockedViews();

                if (t == TabPageGeneral)
                {
                    GeneralPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageBallooning)
                {
                    BallooningPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageSR)
                {
                    SrStoragePage.SR = SelectionManager.Selection.First as SR;
                }
                else if (t == TabPageNetwork)
                {
                    NetworkPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageUSB)
                {
                    UsbPage.XenObject = SelectionManager.Selection.FirstAsXenObject as Host;
                }
                else if (t == TabPageNICs)
                {
                    NICPage.Host = SelectionManager.Selection.First as Host;
                }
                else if (t == TabPageStorage)
                {
                    VMStoragePage.VM = SelectionManager.Selection.First as VM;
                }
                else if (t == TabPagePeformance)
                {
                    PerformancePage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageSearch && !SearchMode)
                {
                    var rootNode = SelectionManager.Selection.RootNode;
                    var rootNodeGrouping = rootNode == null ? null : rootNode.Tag as GroupingTag;
                    var search = rootNode == null ? null : rootNode.Tag as Search;

                    if (search != null)
                    {
                        SearchPage.Search = search;
                    }
                    else if (rootNodeGrouping != null)
                    {
                        var objectsView = rootNodeGrouping.Grouping as OrganizationViewObjects;
                        var vappsView = rootNodeGrouping.Grouping as OrganizationViewVapps;
                        var foldersView = rootNodeGrouping.Grouping as OrganizationViewFolders;

                        if (vappsView != null)
                        {
                            SearchPage.Search = Search.SearchForVappGroup(rootNodeGrouping.Grouping,
                                rootNodeGrouping.Parent, rootNodeGrouping.Group);
                        }
                        else if (objectsView != null)
                        {
                            //We are in Objects View
                            GroupingTag gt = null;

                            if (SelectionManager.Selection.Count == 1)
                            {
                                gt = SelectionManager.Selection.First as GroupingTag
                                    ?? SelectionManager.Selection[0].GroupAncestor;
                            }
                            else
                            {
                                //If multiple items have been selected we count the number of the grouping tags in the selection
                                var selectedGroups = SelectionManager.Selection.Where(s => s.GroupingTag != null).ToList();

                                //if exactly one grouping tag has been selected we show the search view for that one tag, but only if all the other items in the selection belong to this group/tag
                                if (selectedGroups.Count == 1)
                                {
                                    var groupingTag = selectedGroups[0].GroupingTag;

                                    if (SelectionManager.Selection.Where(s => s.GroupingTag == null).All(s => s.GroupAncestor == groupingTag))
                                        gt = groupingTag;
                                    else
                                        gt = null;
                                }
                                else
                                {
                                    gt = SelectionManager.Selection.GroupAncestor;
                                }
                            }

                            //if there has been a grouping tag determined above we use that
                            //if not we show the search view for the root node
                            if (gt != null)
                            {
                                SearchPage.Search = Search.SearchForNonVappGroup(gt.Grouping, gt.Parent, gt.Group);
                            }
                            else
                            {
                                SearchPage.Search = Search.SearchForNonVappGroup(rootNodeGrouping.Grouping, rootNodeGrouping.Parent, rootNodeGrouping.Group);
                            }
                        }
                        else if (foldersView != null)
                        {
                            SearchPage.Search = Search.SearchForFolderGroup(rootNodeGrouping.Grouping,
                                  rootNodeGrouping.Parent, rootNodeGrouping.Group);
                        }
                        else
                        {
                            SearchPage.Search = Search.SearchForNonVappGroup(rootNodeGrouping.Grouping,
                                  rootNodeGrouping.Parent, rootNodeGrouping.Group);
                        }
                    }
                    else
                    {
                        // Infrastructure View:
                        // - In case of single selection or multiple selection within the same pool,
                        //   find the top-level parent (pool or standalone server) and show that search
                        // - In case of multiple selection across pools or standalone servers,
                        //   or selection of the XenCenter node, or selection of a disconnected host,
                        //   show the default search.
                        var connection = SelectionManager.Selection.GetConnectionOfAllItems();
                        if (connection == null)
                        {
                            SearchPage.XenObject = null;
                        }
                        else
                        {
                            var pool = Helpers.GetPool(connection);
                            SearchPage.XenObject = pool ?? (IXenObject)Helpers.GetCoordinator(connection);
                        }
                    }
                }
                else if (t == TabPageHA)
                {
                    HAPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageWLB)
                {
                    WlbPage.Pool = SelectionManager.Selection.First as Pool;
                }
                else if (t == TabPageSnapshots)
                {
                    snapshotPage.VM = SelectionManager.Selection.First as VM;
                }
                else if (t == TabPagePhysicalStorage)
                {
                    PhysicalStoragePage.SetSelectionBroadcaster(SelectionManager, this);
                    PhysicalStoragePage.Host = SelectionManager.Selection.First as Host;
                    PhysicalStoragePage.Connection = SelectionManager.Selection.GetConnectionOfFirstItem();
                }
                else if (t == TabPageAD)
                {
                    AdPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageGPU)
                {
                    GpuPage.XenObject = SelectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageDockerProcess)
                {
                    DockerProcessPage.DockerContainer = SelectionManager.Selection.First as DockerContainer;
                }
                else if (t == TabPageDockerDetails)
                {
                    DockerDetailsPage.DockerContainer = SelectionManager.Selection.First as DockerContainer;
                }
                else if (t == TabPagePvs)
                {
                    PvsPage.Connection = SelectionManager.Selection.GetConnectionOfFirstItem();
                }
            }

            if (t == TabPageSearch)
                SearchPage.PanelShown();
            else
                SearchPage.PanelHidden();

            if (t == TabPageDockerDetails)
                DockerDetailsPage.ResumeRefresh();
            else
                DockerDetailsPage.PauseRefresh();

            if (t == TabPageDockerProcess)
                DockerProcessPage.ResumeRefresh();
            else
                DockerProcessPage.PauseRefresh();

            if (t != null)
                SetLastSelectedPage(SelectionManager.Selection.First, t);

            UpdateTabePageFeatures();
        }

        private void UpdateTabePageFeatures()
        {
            var plugins = PluginManager.Plugins;
            foreach (var p in plugins)
            {
                var features = p.Features;
                foreach (var feature in features)
                {
                    var f = feature as TabPageFeature;
                    if (f == null)
                        continue;

                    if (!f.ShowTab)
                        continue;

                    if (f.IsConsoleReplacement)
                    {
                        f.SetUrl();
                        continue;
                    }

                    var page = GetLastSelectedPage(f.SelectedXenObject);
                    if (page != null && page.Tag == f)
                        f.SetUrl();
                }
            }
        }

        /// <summary>
        /// The tabs that may be visible in the main GUI window. Used in SwitchToTab().
        /// </summary>
        public enum Tab
        {
            Home, General, Storage, Network, Console, CvmConsole, Performance, NICs, SR, DockerProcess, DockerDetails, USB, Search
        }

        public void SwitchToTab(Tab tab)
        {
            switch (tab)
            {
                case Tab.Home:
                    TheTabControl.SelectedTab = TabPageHome;
                    break;
                case Tab.General:
                    TheTabControl.SelectedTab = TabPageGeneral;
                    break;
                case Tab.Storage:
                    TheTabControl.SelectedTab = TabPageStorage;
                    break;
                case Tab.Network:
                    TheTabControl.SelectedTab = TabPageNetwork;
                    break;
                case Tab.Console:
                    TheTabControl.SelectedTab = TabPageConsole;
                    break;
                case Tab.CvmConsole:
                    TheTabControl.SelectedTab = TabPageCvmConsole;
                    break;
                case Tab.Performance:
                    TheTabControl.SelectedTab = TabPagePeformance;
                    break;
                case Tab.NICs:
                    TheTabControl.SelectedTab = TabPageNICs;
                    break;
                case Tab.SR:
                    TheTabControl.SelectedTab = TabPageSR;
                    break;
                case Tab.DockerProcess:
                    TheTabControl.SelectedTab = TabPageDockerProcess;
                    break;
                case Tab.DockerDetails:
                    TheTabControl.SelectedTab = TabPageDockerDetails;
                    break;
                case Tab.USB:
                    TheTabControl.SelectedTab = TabPageUSB;
                    break;
                case Tab.Search:
                    TheTabControl.SelectedTab = TabPageSearch;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void templatesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            templatesToolStripMenuItem1.Checked = !templatesToolStripMenuItem1.Checked;
            Properties.Settings.Default.DefaultTemplatesVisible = templatesToolStripMenuItem1.Checked;
            ViewSettingsChanged();
        }

        private void customTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customTemplatesToolStripMenuItem.Checked = !customTemplatesToolStripMenuItem.Checked;
            Properties.Settings.Default.UserTemplatesVisible = customTemplatesToolStripMenuItem.Checked;
            ViewSettingsChanged();
        }

        private void localStorageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            localStorageToolStripMenuItem.Checked = !localStorageToolStripMenuItem.Checked;
            Properties.Settings.Default.LocalSRsVisible = localStorageToolStripMenuItem.Checked;
            ViewSettingsChanged();
        }

        private void ShowHiddenObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHiddenObjectsToolStripMenuItem.Checked = !ShowHiddenObjectsToolStripMenuItem.Checked;
            Properties.Settings.Default.ShowHiddenVMs = ShowHiddenObjectsToolStripMenuItem.Checked;
            ViewSettingsChanged();
        }

        private void ViewSettingsChanged()
        {
            Settings.TrySaveSettings();
            navigationPane.UpdateSearch();
            RequestRefreshTreeView();
        }

        private void EditSelectedNodeInTreeView()
        {
            navigationPane.EditSelectedNode();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            bool currentTasks = false;
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (a is MeddlingAction || a.IsCompleted)
                    continue;

                currentTasks = true;
                break;
            }

            if (currentTasks)
            {
                e.Cancel = true;

                if (Program.RunInAutomatedTestMode ||
                    new Dialogs.WarningDialogs.CloseXenCenterWarningDialog().ShowDialog(this) == DialogResult.OK)
                {
                    this.Hide();

                    // Close all open forms
                    List<Form> forms = new List<Form>();
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form != this)
                        {
                            forms.Add(form);
                        }
                    }
                    foreach (Form form in forms)
                    {
                        form.Close();
                    }

                    foreach (ActionBase a in ConnectionsManager.History)
                    {
                        if(!Program.RunInAutomatedTestMode)
                        {
                            if (a is AsyncAction asyncAction)
                                asyncAction.PrepareForEventReloadAfterRestart();

                            if (!a.IsCompleted && a.CanCancel && !a.SafeToExit)
                                a.Cancel();
                        }
                        else
                        {
                            if (!a.IsCompleted && a.CanCancel)
                                a.Cancel();
                        }
                    }
                    ThreadPool.QueueUserWorkItem(CloseWhenActionsCanceled);
                }
                return;
            }

            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.WindowLocation = this.Location;
            base.OnClosing(e);
        }

        private void sendCtrlAltDelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConsolePanel.SendCAD();
        }

        #region IMainWindowCommandInterface Members

        /// <summary>
        /// Closes all per-Connection and per-VM forms for the given connection.
        /// Per-Host forms are excluded on purpose.
        /// </summary>
        /// <param name="connection"></param>
        public void CloseActiveWizards(IXenConnection connection)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                XenDialogBase.CloseAll(connection.Cache.VMs.Cast<IXenObject>().ToArray());

                if (activePoolWizards.TryGetValue(connection, out IList<Form> wizards))
                {
                    foreach (var wizard in wizards)
                    {
                        if (!wizard.IsDisposed)
                            wizard.Close();
                    }

                    activePoolWizards.Remove(connection);
                }
            });
        }

        /// <summary>
        /// Show the given wizard, and impose a one-wizard-per-connection limit.
        /// </summary>
        /// <param name="connection">The connection.  May be null, in which case the wizard
        /// is not added to any dictionary.  This should happen iff this is the New Pool Wizard.</param>
        /// <param name="wizard">The new wizard to show. May not be null.</param>
        /// <param name="parentForm">The form owning the wizard to be launched.</param>
        public void ShowPerConnectionWizard(IXenConnection connection, Form wizard, Form parentForm = null)
        {
            if (connection != null)
            {

                if (activePoolWizards.ContainsKey(connection))
                {
                    var w = activePoolWizards[connection].FirstOrDefault(x => x.GetType() == wizard.GetType());
                    if (w != null && !w.IsDisposed)
                    {
                        if (w.WindowState == FormWindowState.Minimized)
                        {
                            w.WindowState = FormWindowState.Normal;
                        }
                        w.Focus();
                        return;
                    }
                    if (w != null && w.IsDisposed)
                        activePoolWizards[connection].Remove(w);

                }

                //closeActivePoolWizards(connection);
                if (activePoolWizards.ContainsKey(connection))
                    activePoolWizards[connection].Add(wizard);
                else
                    activePoolWizards.Add(connection, new List<Form>() { wizard });

            }

            if (!wizard.Disposing && !wizard.IsDisposed && !Program.Exiting)
            {
                wizard.Show(parentForm ?? this);
            }
        }

        /// <summary>
        /// Shows a form of the specified type if it has already been created. If the form doesn't exist yet
        /// it is created first and then shown.
        /// </summary>
        /// <param name="type">The type of the form to be shown.</param>
        public Form ShowForm(Type type)
        {
            return ShowForm(type, null);
        }

        /// <summary>
        /// Shows a form of the specified type if it has already been created. If the form doesn't exist yet
        /// it is created first and then shown.
        /// </summary>
        /// <param name="type">The type of the form to be shown.</param>
        /// <param name="args">The arguments to pass to the form's consructor</param>
        public Form ShowForm(Type type, object[] args)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == type)
                {
                    HelpersGUI.BringFormToFront(form);
                    return form;
                }
            }

            Form newForm = (Form)Activator.CreateInstance(type, args);
            newForm.Show(this);
            return newForm;
        }

        public Form Form
        {
            get { return this; }
        }

        public void Invoke(MethodInvoker method)
        {
            Program.Invoke(this, method);
        }

        /// <summary>
        /// Selects the specified object in the treeview.
        /// </summary>
        /// <param name="xenObject">The object to be selected.</param>
        /// <returns>A value indicating whether selection was successful.</returns>
        public bool SelectObjectInTree(IXenObject xenObject)
        {
            return navigationPane.SelectObject(xenObject);
        }

        public Collection<IXenConnection> GetXenConnectionsCopy()
        {
            return new Collection<IXenConnection>(ConnectionsManager.XenConnectionsCopy);
        }

        public void SaveServerList()
        {
            Settings.SaveServerList();
        }

        public bool RunInAutomatedTestMode
        {
            get { return Program.RunInAutomatedTestMode; }
        }

        public void RemoveConnection(IXenConnection connection)
        {
            ConnectionsManager.ClearCacheAndRemoveConnection(connection);
        }

        public void PutSelectedNodeIntoEditMode()
        {
            EditSelectedNodeInTreeView();
        }

        public void TrySelectNewObjectInTree(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            TrySelectNewNode(tagMatch, selectNode, expandNode, ensureNodeVisible);
        }

        public void TrySelectNewObjectInTree(IXenConnection c, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            TrySelectNewNode(c, selectNode, expandNode, ensureNodeVisible);
        }

        #endregion

        #region Help

        private string getSelectedXenModelObjectType()
        {
            // for now, since there are few topics which depend on the selected object we shall just check the special cases
            // when more topic are added we can just return the ModelObjectName

            if (TheTabControl.SelectedTab == TabPageGeneral && SelectionManager.Selection.First is VM)
            {
                return "VM";
            }

            if (TheTabControl.SelectedTab == TabPagePhysicalStorage || TheTabControl.SelectedTab == TabPageStorage || TheTabControl.SelectedTab == TabPageSR)
            {
                if (SelectionManager.Selection.FirstIs<Pool>())
                    return "Pool";
                if (SelectionManager.Selection.FirstIs<Host>())
                    return "Server";
                if (SelectionManager.Selection.FirstIs<VM>())
                    return "VM";
                if (SelectionManager.Selection.FirstIs<SR>())
                    return "Storage";
            }

            if (TheTabControl.SelectedTab == TabPageNetwork)
            {
                if (SelectionManager.Selection.FirstIs<Host>())
                    return "Server";
                if (SelectionManager.Selection.FirstIs<VM>())
                    return "VM";
            }

            return "";
        }

        public void MainWindow_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            // CA-28064. MessageBox hack to stop the hlpevent it passes to MainWindows.
            if (Program.MainWindow.ContainsFocus && MenuShortcutsEnabled)
                LaunchHelp();
        }

        private void helpTopicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpManager.Launch("TOC");
        }

        private void helpContextMenuItem_Click(object sender, EventArgs e)
        {
            LaunchHelp();
        }

        private void LaunchHelp()
        {
            if (TheTabControl.SelectedTab.Tag is TabPageFeature tpf && tpf.HasHelp)
            {
                tpf.LaunchHelp();
                return;
            }

            HelpManager.Launch(TabHelpID());
        }

        private string TabHelpID()
        {
            if (alertPage.Visible)
                return alertPage.HelpID;

            if (updatesPage.Visible)
                return updatesPage.HelpID;

            if (eventsPage.Visible)
                return eventsPage.HelpID;

            if (TheTabControl.SelectedTab.Controls.Count > 0 && TheTabControl.SelectedTab.Controls[0] is IControlWithHelp ctrl)
                return ctrl.HelpID + getSelectedXenModelObjectType();

            return "TOC";
        }

        public bool HasHelp()
        {
            return HelpManager.TryGetTopicId(TabHelpID(), out _);
        }

        private void viewApplicationLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ViewLogFiles();
        }

        #endregion

        /// <summary>
        /// Used to select the pool or standalone host node for the specified connection which is about to appear in the tree.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="selectNode">if set to <c>true</c> then the pool/standalone host node will be selected.</param>
        /// <param name="expandNode">if set to <c>true</c> then the pool/standalone host node will be expanded.</param>
        /// <param name="ensureNodeVisible">if set to <c>true</c> then the matched node will be made visible.</param>
        public void TrySelectNewNode(IXenConnection connection, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            if (connection != null)
            {
                TrySelectNewNode(delegate(object o)
                {
                    if (o == null)
                    {
                        return false;
                    }
                    else if (o.Equals(Helpers.GetPool(connection)))
                    {
                        return true;
                    }
                    Host[] hosts = connection.Cache.Hosts;
                    return hosts.Length > 0 && o.Equals(hosts[0]);
                }, selectNode, expandNode, ensureNodeVisible);
            }
        }

        /// <summary>
        /// Used to select or expand a node which is about to appear in the tree. This is used so that new hosts, folders, pools
        /// etc. can be picked and then selected/expanded. 
        /// 
        /// It fires off a new thread and then repeatedly tries to select a node which matches the specified match
        /// delegate. It stops if it times out or is successful.
        /// </summary>
        /// <param name="tagMatch">A match for the tag of the node.</param>
        /// <param name="selectNode">if set to <c>true</c> then the matched node will be selected.</param>
        /// <param name="expandNode">if set to <c>true</c> then the matched node will be expanded.</param>
        /// <param name="ensureNodeVisible">if set to <c>true</c> then the matched node will be made visible.</param>
        public void TrySelectNewNode(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                bool success = false;

                for (int i = 0; i < 20 && !success; i++)
                {
                    Program.Invoke(Program.MainWindow, delegate
                        {
                            success = navigationPane.TryToSelectNewNode(tagMatch, selectNode, expandNode, ensureNodeVisible);
                        });
                    Thread.Sleep(500);
                }
            });
        }

        private void NotificationsPage_FiltersChanged()
        {
            SetFiltersLabel();
        }

        private void SetFiltersLabel()
        {
            labelFiltersOnOff.Visible = alertPage.Visible || updatesPage.Visible || eventsPage.Visible;
            bool filterIsOn = alertPage.Visible && alertPage.FilterIsOn ||
                              updatesPage.Visible && updatesPage.FilterIsOn ||
                              eventsPage.Visible && eventsPage.FilterIsOn;
            labelFiltersOnOff.Text = filterIsOn ? Messages.FILTERS_ON : Messages.FILTERS_OFF;
        }

        private void eventsPage_GoToXenObjectRequested(IXenObject obj)
        {
            navigationPane.SwitchToInfrastructureMode();
            navigationPane.SelectObject(obj);
        }

        private void Updates_CollectionChanged(CollectionChangeEventArgs e)
        {
            Program.Invoke(this, () =>
                {
                    int updatesCount = Updates.UpdateAlerts.Count;
                    navigationPane.UpdateNotificationsButton(NotificationsSubMode.Updates, updatesCount);

                    statusLabelUpdates.Text = string.Format(Messages.NOTIFICATIONS_SUBMODE_UPDATES_STATUS, updatesCount);
                    statusLabelUpdates.Visible = updatesCount > 0;

                    SetUpdateAlert();

                    if (updatesPage.Visible)
                    {
                        TitleLabel.Text = NotificationsSubModeItem.GetText(NotificationsSubMode.Updates, updatesCount);
                        TitleIcon.Image = NotificationsSubModeItem.GetImage(NotificationsSubMode.Updates, updatesCount);
                    }
                });
        }

        private void ClientUpdatesCheck_Completed()
        {
            Program.Invoke(this, () =>
            {
                toolStripMenuItemCfu.Enabled = true;
                SetUpdateAlert();
            });            
        }

        private void SetUpdateAlert()
        {
            updateAlert = Updates.UpdateAlerts.FirstOrDefault(update => update is ClientUpdateAlert) as ClientUpdateAlert;
            if (updateAlert != null)
                relNotesToolStripMenuItem.Text = string.Format(Messages.MAINWINDOW_UPDATE_RELEASE, updateAlert.NewVersion.Version);
            updateClientToolStripMenuItem.Visible = updateAlert != null;
        }

        private void ClientUpdatesCheck_Started()
        {
            Program.Invoke(this, () =>
            {
                updateClientToolStripMenuItem.Visible = false;
                toolStripMenuItemCfu.Enabled = false;
            });
        }

        private void CloseWhenActionsCanceled(object o)
        {
            int i = 0;
            while (true)
            {
                if (i > 20)
                    Program.ForcedExiting = true;

                if (i > 40 || AllActionsCompleted())
                {
                    Program.Invoke(this, Application.Exit);
                    break;
                }

                i++;
                Thread.Sleep(500);
            }
        }

        private bool AllActionsCompleted()
        {
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (!a.IsCompleted)
                    return false;
            }
            return true;
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OptionsDialog(PluginManager))
                dialog.ShowDialog(this);
        }

        internal void action_Completed(ActionBase sender)
        {
            if (Program.Exiting)
                return;

            sender.Completed -= action_Completed;
            RequestRefreshTreeView();
        }

        private void OpenGlobalImportWizard(string param)
        {
            HelpersGUI.BringFormToFront(this);
            Host hostAncestor = SelectionManager.Selection.Count == 1 ? SelectionManager.Selection[0].HostAncestor : null;
            new ImportWizard(SelectionManager.Selection.GetConnectionOfFirstItem(), hostAncestor, param).Show();
        }

        #region XenSearch

        private bool searchMode;
        /// <summary>
        /// SearchMode doesn't just mean we are looking at the Search tab.
        /// It's set when we import a search from a file; or when we double-click
        /// on a folder or tag name to search for it.
        /// </summary>
        private bool SearchMode
        {
            get
            {
                return searchMode;
            }
            set
            {
                if (searchMode == value)
                    return;

                searchMode = value;
                navigationPane.InSearchMode = value;
                UpdateToolbars();
            }
        }

        public bool DoSearch(string filename)
        {
            List<Search> searches = Search.LoadFile(filename);
            if (searches != null && searches.Count > 0)
            {
                Program.Invoke(Program.MainWindow, delegate()
                {
                    DoSearch(searches[0]);
                });
                return true;
            }
            return false;
        }

        public void DoSearch(Search search)
        {
            History.NewHistoryItem(new SearchHistoryItem(search));

            SearchMode = true;
            SearchPage.Search = search;

            UpdateHeader();
        }

        public void SearchForTag(string tag)
        {
            DoSearch(Search.SearchForTag(tag));
        }

        public void SearchForFolder(string path)
        {
            DoSearch(Search.SearchForFolder(path));
        }

        void SearchPanel_SearchChanged()
        {
            if (SearchMode)
                History.ReplaceHistoryItem(new SearchHistoryItem(SearchPage.Search));
            else
                History.ReplaceHistoryItem(new ModifiedSearchHistoryItem(
                    SelectionManager.Selection.FirstAsXenObject, SearchPage.Search));
        }

        /// <summary>
        /// Updates the shiny gradient bar with selected object name and icon.
        /// Also updates 'Logged in as:'.
        /// </summary>
        private void UpdateHeader()
        {
            if (navigationPane.currentMode == NavigationPane.NavigationMode.Notifications)
                return;

            var licenseColor = VerticalGradientPanel.TextColor;
            var licenseText = string.Empty;

            if (SearchMode && SearchPage.Search != null)
            {
                TitleLabel.Text = HelpersGUI.GetLocalizedSearchName(SearchPage.Search);
                TitleIcon.Image = Images.GetImage16For(SearchPage.Search);
            }
            else if (!SearchMode && SelectionManager.Selection.ContainsOneItemOfType<IXenObject>())
            {
                IXenObject xenObject = SelectionManager.Selection[0].XenObject;
                TitleLabel.Text = xenObject.NameWithLocation();
                TitleIcon.Image = Images.GetImage16For(xenObject);
                
                licenseText = GetLicenseStatusText(xenObject, out licenseColor);

                // When in folder view only show the logged in label if it is clear to which connection the object belongs (most likely pools and hosts)

                if (SelectionManager.Selection[0].PoolAncestor == null && SelectionManager.Selection[0].HostAncestor == null)
                    loggedInLabel1.Connection = null;
                else
                    loggedInLabel1.Connection = xenObject.Connection;
            }
            else
            {
                TitleLabel.Text = BrandManager.BrandConsole;
                TitleIcon.Image = Images.StaticImages.Logo;
                loggedInLabel1.Connection = null;
            }

            LicenseStatusTitleLabel.Text = licenseText;
            LicenseStatusTitleLabel.ForeColor = licenseColor;
            SetTitleLabelMaxWidth();
        }

        private string GetLicenseStatusText(IXenObject xenObject, out Color foreColor)
        {
            foreColor = VerticalGradientPanel.TextColor;

            var pool = xenObject as Pool;
            if (pool != null && pool.Connection != null && pool.Connection.IsConnected && pool.Connection.CacheIsPopulated)
            {
                if (pool.IsFreeLicenseOrExpired())
                {
                    foreColor = Color.Red;
                    return Messages.MAINWINDOW_HEADER_UNLICENSED;
                }

                return string.Format(Messages.MAINWINDOW_HEADER_LICENSED_WITH, Helpers.GetFriendlyLicenseName(pool));
            }

            var host = xenObject as Host;
            if (host != null && host.Connection != null && host.Connection.IsConnected && host.Connection.CacheIsPopulated)
            {
                if (host.IsFreeLicenseOrExpired())
                {
                    foreColor = Color.Red;
                    return Messages.MAINWINDOW_HEADER_UNLICENSED;
                }

                return string.Format(Messages.MAINWINDOW_HEADER_LICENSED_WITH, Helpers.GetFriendlyLicenseName(host));
            }

            return string.Empty;
        }

        private void SetTitleLabelMaxWidth()
        {
            TitleLabel.MaximumSize = new Size(tableLayoutPanel1.Width - loggedInLabel1.Width - LicenseStatusTitleLabel.Width - 6, TitleLabel.Height);
        }

        private void UpdateViewMenu(NavigationPane.NavigationMode mode)
        {
            //the order is the reverse from the order in which we want them to appear
            var items = new ToolStripItem []
                {
                    toolStripSeparator24,
                    ShowHiddenObjectsToolStripMenuItem,
                    localStorageToolStripMenuItem,
                    templatesToolStripMenuItem1,
                    customTemplatesToolStripMenuItem
                };

            if (mode == NavigationPane.NavigationMode.Infrastructure)
            {
                foreach (var item in items)
                {
                    if (!viewToolStripMenuItem.DropDownItems.Contains(item))
                        viewToolStripMenuItem.DropDownItems.Insert(0, item);
                }
            }
            else if (mode == NavigationPane.NavigationMode.Notifications)
            {
                 foreach (var item in items)
                    viewToolStripMenuItem.DropDownItems.Remove(item);
            }
            else
            {
                for (int i = 2; i < items.Length; i++)
                    viewToolStripMenuItem.DropDownItems.Remove(items[i]);

                for (int i = 0; i < 2; i++)
                    if (!viewToolStripMenuItem.DropDownItems.Contains(items[i]))
                        viewToolStripMenuItem.DropDownItems.Insert(0, items[i]);
            }

            pluginMenuItemStartIndexes[viewToolStripMenuItem] = viewToolStripMenuItem.DropDownItems.IndexOf(toolStripSeparator24) + 1;
        }

        void navigationPane_DragDropCommandActivated(string cmdText)
        {
            SetStatusBar(null, cmdText);
        }

        private void navigationPane_TreeViewSelectionChanged()
        {
            UpdateToolbars();

            // NB do not trigger updates to the panels in this method
            // instead, put them in TheTabControl_SelectedIndexChanged,
            // so only the selected tab is updated

            TheTabControl_SelectedIndexChanged(null, null);

            if (TheTabControl.SelectedTab != null)
                TheTabControl.SelectedTab.Refresh();

            UpdateHeader();
        }

        private void navigationPane_NotificationsSubModeChanged(NotificationsSubModeItem submodeItem)
        {
            switch (submodeItem.SubMode)
            {
                case NotificationsSubMode.Alerts:
                    if (updatesPage.Visible)
                        updatesPage.HidePage();
                    if (eventsPage.Visible)
                        eventsPage.HidePage();
                    alertPage.ShowPage();
                    break;
                case NotificationsSubMode.Updates:
                    if (alertPage.Visible)
                        alertPage.HidePage();
                    if (eventsPage.Visible)
                        eventsPage.HidePage();
                    updatesPage.ShowPage();
                break;
                case NotificationsSubMode.Events:
                    if (alertPage.Visible)
                        alertPage.HidePage();
                    if (updatesPage.Visible)
                        updatesPage.HidePage();
                    eventsPage.ShowPage();
                break;
            }

            SetFiltersLabel();

            TheTabControl.Visible = false;

            loggedInLabel1.Connection = null;
            TitleLabel.Text = submodeItem.Text;
            TitleIcon.Image = submodeItem.Image;
        }

        private void navigationPane_NavigationModeChanged(NavigationPane.NavigationMode mode)
        {
            if (mode == NavigationPane.NavigationMode.Notifications)
            {
                LicenseStatusTitleLabel.Text = string.Empty;
                TheTabControl.Visible = false;
            }
            else
            {
                bool tabControlWasVisible = TheTabControl.Visible;
                TheTabControl.Visible = true;
                if (alertPage.Visible)
                    alertPage.HidePage();
                if (updatesPage.Visible)
                    updatesPage.HidePage();
                if (eventsPage.Visible)
                    eventsPage.HidePage();

                // force an update of the selected tab when switching back from Notification view, 
                // as some tabs ignore the update events when not visible (e.g. Snapshots, HA)
                if (!tabControlWasVisible)
                    TheTabControl_SelectedIndexChanged(null, null);
            }

            SetFiltersLabel();
            UpdateViewMenu(mode);
        }

        private void navigationPane_TreeNodeBeforeSelected()
        {
            SearchMode = false;
        }

        private void navigationPane_TreeNodeClicked()
        {
            if (SearchMode)
            {
                SearchMode = false;
                TheTabControl_SelectedIndexChanged(null, null);
                UpdateHeader();
            }
        }

        private void navigationPane_TreeNodeRightClicked()
        {
           MainMenuBar_MenuActivate(MainMenuBar, new EventArgs());
        }

        private void navigationPane_TreeViewRefreshed()
        {
            // This is required to update search results when things change.
            if (TheTabControl.SelectedTab == TabPageGeneral)
                GeneralPage.BuildList();
            else if (TheTabControl.SelectedTab == TabPageSearch)
                SearchPage.BuildList();

            UpdateHeader();
            UpdateToolbars();
        }

        #endregion

        private void XenCenterAlerts_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(Program.MainWindow, () =>
            {
                var count = Alert.NonDismissingAlertCount;
                navigationPane.UpdateNotificationsButton(NotificationsSubMode.Alerts, count);

                statusLabelAlerts.Text = string.Format(Messages.NOTIFICATIONS_SUBMODE_ALERTS_STATUS, count);
                statusLabelAlerts.Visible = count > 0;

                if (alertPage.Visible)
                {
                    TitleLabel.Text = NotificationsSubModeItem.GetText(NotificationsSubMode.Alerts, count);
                    TitleIcon.Image = NotificationsSubModeItem.GetImage(NotificationsSubMode.Alerts, count);
                }
            });
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            History.Back(1);
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            History.Forward(1);
        }

        private void backButton_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripSplitButton button = sender as ToolStripSplitButton;
            if (button == null)
                return;

            History.PopulateBackDropDown(button);
        }

        private void forwardButton_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripSplitButton button = sender as ToolStripSplitButton;
            if (button == null)
                return;

            History.PopulateForwardDropDown(button);
        }

        private void LicenseManagerMenuItem_Click(object sender, EventArgs e)
        {
            licenseManagerLauncher.LaunchIfRequired(false, ConnectionsManager.XenConnections, SelectionManager.Selection);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RequestRefreshTreeView();
                if (TheTabControl.SelectedTab == TabPageSearch)
                    SearchPage.PanelProd();
            }
        }

        private void ShowToolbarMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ToolbarsEnabled = !Properties.Settings.Default.ToolbarsEnabled;
            UpdateToolbars();
        }

        private void MainMenuBar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolBarContextMenu.Show(Program.MainWindow, e.Location);
            }
        }

        /// <summary>
        /// Show a message telling the user that the connection has disappeared.  We check this after
        /// we've shown a dialog, in case it's happened in the time it took them to click OK.
        /// </summary>
        public static void ShowDisconnectedMessage(Control parent)
        {
            // We could have done some teardown by now, so we need to be paranoid about things going away
            // beneath us.
            if (Program.Exiting)
                return;
            if (parent == null || parent.Disposing || parent.IsDisposed)
            {
                parent = Program.MainWindow;
                if (parent.Disposing || parent.IsDisposed)
                    return;
            }
            using (var dlg = new WarningDialog(Messages.DISCONNECTED_BEFORE_ACTION_STARTED))
                dlg.ShowDialog(parent);
        }

        #region ISynchronizeInvoke Members

        // this explicit implementation of ISynchronizeInvoke is used to allow the model to update 
        // its API on the main program thread while being decoupled from MainWindow.

        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            return Program.BeginInvoke(this, method, args);
        }

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
            return EndInvoke(result);
        }

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            return Program.Invoke(this, method, args);
        }

        bool ISynchronizeInvoke.InvokeRequired
        {
            get { return InvokeRequired; }
        }

        #endregion

        private void importSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = string.Format(Messages.XENCENTER_CONFIG_FILTER, BrandManager.BrandConsole);
                if (dialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    try
                    {
                        log.InfoFormat("Importing server list from '{0}'", dialog.FileName);

                        XmlDocument xmlDocument = new XmlDocument();
                        using (var stream = dialog.OpenFile())
                            xmlDocument.Load(stream);
                        
                        foreach (XmlNode itemConnection in xmlDocument.GetElementsByTagName("XenConnection"))
                        {
                            var conn = new XenConnection();
                            foreach (XmlNode item in itemConnection.ChildNodes)
                            {

                                switch (item.Name)
                                {
                                    case "Hostname":
                                        conn.Hostname = item.InnerText;
                                        break;
                                    case "Port":
                                        conn.Port = int.Parse(item.InnerText);
                                        break;
                                    case "FriendlyName":
                                        conn.FriendlyName = item.InnerText;
                                        break;
                                }
                            }
                            if (null == ConnectionsManager.XenConnections.Find(existing => (existing.Hostname == conn.Hostname && existing.Port == conn.Port)))
                                ConnectionsManager.XenConnections.Add(conn);
                            RequestRefreshTreeView();
                        }

                        log.InfoFormat("Imported server list from '{0}' successfully.", dialog.FileName);
                    }
                    catch (XmlException)
                    {
                        log.ErrorFormat("Failed to import server list from '{0}'", dialog.FileName);

                        using (var dlg = new ErrorDialog(Messages.ERROR_IMPORTING_SERVER_LIST))
                            dlg.ShowDialog(this);
                    }
                }
            }
        }

        private void exportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = string.Format(Messages.XENCENTER_CONFIG_FILTER, BrandManager.BrandConsole);
                dialog.Title = Messages.ACTION_SAVE_CHANGES_IN_PROGRESS;
                dialog.CheckPathExists = true;
                if (dialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    log.InfoFormat("Exporting server list to '{0}'", dialog.FileName);

                    try
                    {
                        using (var xmlWriter = new XmlTextWriter(dialog.OpenFile(), Encoding.Unicode))
                        {
                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("XenConnections");
                            xmlWriter.WriteWhitespace("\n");

                            foreach (var connection in ConnectionsManager.XenConnections)
                            {
                                xmlWriter.WriteStartElement("XenConnection");
                                {
                                    xmlWriter.WriteElementString("Hostname", connection.Hostname);
                                    xmlWriter.WriteElementString("Port", connection.Port.ToString());
                                    xmlWriter.WriteWhitespace("\n  ");
                                    xmlWriter.WriteElementString("FriendlyName", connection.FriendlyName);
                                }
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteWhitespace("\n");
                            }

                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteEndDocument();
                        }

                        log.InfoFormat("Exported server list to '{0}' successfully.", dialog.FileName);
                    }
                    catch
                    {
                        log.ErrorFormat("Failed to export server list to '{0}'.", dialog.FileName);
                        throw;
                    }
                }
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            SetSplitterDistance();
        }

        FormWindowState lastState = FormWindowState.Normal;
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            TabPage t = TheTabControl.SelectedTab;
            if (t == TabPageConsole)
            {
                if (WindowState != lastState && WindowState != FormWindowState.Minimized)
                {
                    lastState = WindowState;
                    ConsolePanel.UpdateRDPResolution();
                }
                mainWindowResized = true;
            }
            SetSplitterDistance();
            SetTitleLabelMaxWidth();
        }

        private void SetSplitterDistance()
        {
            //CA-71697: chosen min size so the tab contents are visible
            int chosenPanel2MinSize = MinimumSize.Width * 3 / 5;
            int min = splitContainer1.Panel1MinSize;
            int max = splitContainer1.Width - chosenPanel2MinSize;

            if (max < min)
                return;

            splitContainer1.Panel2MinSize = chosenPanel2MinSize;

            if (splitContainer1.SplitterDistance < min)
                splitContainer1.SplitterDistance = min;
            else if (splitContainer1.SplitterDistance > max)
                splitContainer1.SplitterDistance = max;
        }
      
        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            TabPage t = TheTabControl.SelectedTab;
            if (t == TabPageConsole) 
            {
                if (mainWindowResized)
                    ConsolePanel.UpdateRDPResolution();
                mainWindowResized = false;
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            TabPage t = TheTabControl.SelectedTab;
            if (t == TabPageConsole)
                ConsolePanel.UpdateRDPResolution();

            SetTitleLabelMaxWidth();
        }

        private void statusLabelAlerts_Click(object sender, EventArgs e)
        {
            navigationPane.SwitchToNotificationsView(NotificationsSubMode.Alerts);
        }

        private void statusLabelUpdates_Click(object sender, EventArgs e)
        {
            navigationPane.SwitchToNotificationsView(NotificationsSubMode.Updates);
        }

        private void statusLabelErrors_Click(object sender, EventArgs e)
        {
            navigationPane.SwitchToNotificationsView(NotificationsSubMode.Events);
        }

        private void relNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (updateAlert != null)
                Program.OpenURL(updateAlert.WebPageLabel);
        }

        private void downloadInstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientUpdateAlert.DownloadAndInstallNewClient(updateAlert, this);
        }

        private void toolStripMenuItemCfu_Click(object sender, EventArgs e)
        {
            Updates.CheckForClientUpdates(true);
        }
    }
}
