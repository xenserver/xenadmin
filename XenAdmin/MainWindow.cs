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
using XenAdmin.Wizards;
using XenAdmin.XenSearch;
using XenAdmin.Wizards.PatchingWizard;
using XenAdmin.Plugins;
using XenAdmin.Network.StorageLink;

using System.Linq;

[assembly: UIPermission(SecurityAction.RequestMinimum, Clipboard = UIPermissionClipboard.AllClipboard)]
namespace XenAdmin
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public partial class MainWindow : Form, ISynchronizeInvoke
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A mapping between objects in the tree and the associated selected tab.
        /// </summary>
        private Dictionary<object, TabPage> selectedTabs = new Dictionary<object, TabPage>();

        /// <summary>
        /// The selected tab for the overview node.
        /// </summary>
        private TabPage selectedOverviewTab = null;

        internal readonly PerformancePage PerformancePage = new PerformancePage();
        internal readonly GeneralTabPage GeneralPage = new GeneralTabPage();
        internal readonly BallooningPage BallooningPage = new BallooningPage();
        internal readonly BallooningUpsellPage BallooningUpsellPage = new BallooningUpsellPage();
        internal readonly ConsolePanel ConsolePanel = new ConsolePanel();
        internal readonly HAPage HAPage = new HAPage();
        internal readonly HAUpsellPage HAUpsellPage = new HAUpsellPage();
        internal readonly HistoryPage HistoryPage = new HistoryPage();
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

        private bool IgnoreTabChanges = false;
        public bool AllowHistorySwitch = false;
        private bool ToolbarsEnabled;

        private readonly Dictionary<IXenConnection, IList<Form>> activePoolWizards = new Dictionary<IXenConnection, IList<Form>>();
        private readonly Dictionary<IXenObject, Form> activeXenModelObjectWizards = new Dictionary<IXenObject, Form>();

        /// <summary>
        /// The arguments passed in on the command line.
        /// </summary>
        private string[] CommandLineParam = null;
        private ArgType CommandLineArgType = ArgType.None;

        private static BugToolWizard BugToolWizard;
        private static AboutDialog theAboutDialog;

        private static readonly Color HasAlertsColor = Color.Red;
        private static readonly Color NoAlertsColor = SystemColors.ControlText;
        private static readonly System.Windows.Forms.Timer CheckForUpdatesTimer = new System.Windows.Forms.Timer();

        private readonly UpdateManager treeViewUpdateManager = new UpdateManager(30 * 1000);
        private readonly MainWindowTreeBuilder treeBuilder;
        private readonly SelectionManager selectionManager = new SelectionManager();
        private readonly PluginManager pluginManager;
        private readonly ContextMenuBuilder contextMenuBuilder;
        private readonly MainWindowCommandInterface commandInterface;

        private readonly LicenseManagerLauncher licenseManagerLauncher;
        private readonly LicenseTimer licenseTimer;

        private Dictionary<ToolStripMenuItem, int> pluginMenuItemStartIndexes = new Dictionary<ToolStripMenuItem, int>();

        public MainWindow(ArgType argType, string[] args)
        {
            Program.MainWindow = this;
            licenseManagerLauncher = new LicenseManagerLauncher(Program.MainWindow);
            InvokeHelper.Initialize(this);

            InitializeComponent();
            SetMenuItemStartIndexes();
            Icon = Properties.Resources.AppIcon;

            treeView.ImageList = Images.ImageList16;
            if (treeView.ItemHeight < 18)
                treeView.ItemHeight = 18;  // otherwise it's too close together on XP and the icons crash into each other

            components.Add(SearchPage);
            components.Add(NICPage);
            components.Add(VMStoragePage);
            components.Add(SrStoragePage);
            components.Add(PerformancePage);
            components.Add(GeneralPage);
            components.Add(BallooningPage);
            components.Add(ConsolePanel);
            components.Add(NetworkPage);
            components.Add(HAPage);
            components.Add(HistoryPage);
            components.Add(HomePage);
            components.Add(WlbPage);
            components.Add(AdPage);

            AddTabContents(SearchPage, TabPageSearch);
            AddTabContents(VMStoragePage, TabPageStorage);
            AddTabContents(SrStoragePage, TabPageSR);
            AddTabContents(NICPage, TabPageNICs);
            AddTabContents(PerformancePage, TabPagePeformance);
            AddTabContents(GeneralPage, TabPageGeneral);
            AddTabContents(BallooningPage, TabPageBallooning);
            AddTabContents(BallooningUpsellPage, TabPageBallooningUpsell);
            AddTabContents(ConsolePanel, TabPageConsole);
            AddTabContents(NetworkPage, TabPageNetwork);
            AddTabContents(HAPage, TabPageHA);
            AddTabContents(HAUpsellPage, TabPageHAUpsell);
            AddTabContents(HistoryPage, TabPageHistory);
            AddTabContents(HomePage, TabPageHome);
            AddTabContents(WlbPage, TabPageWLB);
            AddTabContents(WLBUpsellPage, TabPageWLBUpsell);
            AddTabContents(PhysicalStoragePage, TabPagePhysicalStorage);
            AddTabContents(AdPage, TabPageAD);

            TheTabControl.SelectedIndexChanged += TheTabControl_SelectedIndexChanged;

            PoolCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Pool>);
            MessageCollectionChangedWithInvoke = Program.ProgramInvokeHandler(MessageCollectionChanged);
            HostCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Host>);
            VMCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<VM>);
            SRCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<SR>);
            FolderCollectionChangedWithInvoke = Program.ProgramInvokeHandler(CollectionChanged<Folder>);
            TaskCollectionChangedWithInvoke = Program.ProgramInvokeHandler(MeddlingActionManager.TaskCollectionChanged);
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;

            CommandLineArgType = argType;
            CommandLineParam = args;

            commandInterface = new MainWindowCommandInterface(this);
            pluginManager = new PluginManager();
            pluginManager.LoadPlugins();
            contextMenuBuilder = new ContextMenuBuilder(pluginManager, commandInterface);

            TreeSearchBox.SearchChanged += TreeSearchBox_SearchChanged;
            SearchPage.SearchChanged += SearchPanel_SearchChanged;

            Alert.XenCenterAlerts.CollectionChanged += XenCenterAlerts_CollectionChanged;

            FormFontFixer.Fix(this);

            Folders.InitFolders();

            OtherConfigAndTagsWatcher.InitEventHandlers();

            // Fix colour of text on gradient panels
            TitleLabel.ForeColor = Program.TitleBarForeColor;
            loggedInLabel1.SetTextColor(Program.TitleBarForeColor);

            TitleLeftLine.Visible = Environment.OSVersion.Version.Major != 6 || Application.RenderWithVisualStyles;

            VirtualTreeNode n = new VirtualTreeNode(Messages.XENCENTER);
            n.NodeFont = Program.DefaultFont;
            treeView.Nodes.Add(n);

            treeViewUpdateManager.Update += treeViewUpdateManager_Update;

            treeBuilder = new MainWindowTreeBuilder(treeView);


            selectionManager.BindTo(MainMenuBar.Items, commandInterface);
            selectionManager.BindTo(ToolStrip.Items, commandInterface);
            Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);

            licenseTimer = new LicenseTimer(licenseManagerLauncher);
            GeneralPage.LicenseLauncher = licenseManagerLauncher;
        }


        private void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
			if (e == null)
				return;

            if (e.SettingName == "AutoSwitchToRDP" || e.SettingName == "EnableRDPPolling")
            {
                ConsolePanel.ResetAllViews();

				if (selectionManager.Selection.FirstIsRealVM)
					ConsolePanel.setCurrentSource((VM)selectionManager.Selection.First);
				else if (selectionManager.Selection.FirstIsHost)
					ConsolePanel.setCurrentSource((Host)selectionManager.Selection.First);

                UnpauseVNC(sender == TheTabControl);
            }
        }

        private void SetMenuItemStartIndexes()
        {
            foreach (ToolStripMenuItem menu in MainMenuBar.Items)
            {
                foreach (ToolStripItem item in menu.DropDownItems)
                {
                    ToolStripMenuItem menuItem = item as ToolStripMenuItem;
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
                return selectionManager;
            }
        }

        internal ContextMenuBuilder ContextMenuBuilder
        {
            get
            {
                return contextMenuBuilder;
            }
        }

        internal IMainWindow CommandInterface
        {
            get
            {
                return commandInterface;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Program.AssertOnEventThread();

            History.EnableHistoryButtons();
            History.NewHistoryItem(new XenModelObjectHistoryItem(null, TabPageHome));

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
            }

            // Using the Load event ensures that the handle has been 
            // created:
            base.OnLoad(e);

            NewTabs[0] = TabPageHome;
            NewTabCount = 1;
            ChangeToNewTabs();

            ActiveControl = treeView;
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

        void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (Program.Exiting)
                return;

            Program.BeginInvoke(Program.MainWindow, () =>
                                                        {
                                                            ActionBase action = (ActionBase)e.Element;
                                                            if (action == null)
                                                                return;

                                                            action.Changed += actionChanged;
                                                            action.Completed += actionChanged;
                                                            actionChanged(action);
                                                        });
        }

        void actionChanged(ActionBase action)
        {
            if (Program.Exiting)
                return;

            Program.Invoke(this, () => actionChanged_(action));
        }

        void actionChanged_(ActionBase action)
        {
            // Be defensive against CA-8517.
            if (action.PercentComplete < 0 || action.PercentComplete > 100)
            {
                log.ErrorFormat("PercentComplete is erroneously {0}", action.PercentComplete);
            }

            // Don't show cancelled exception
            if (action.Exception != null && !(action.Exception is CancelledException))
            {
                bool logs_visible;
                IXenObject selected = selectionManager.Selection.FirstAsXenObject;
                if (selected != null && action.AppliesTo.Contains(selected.opaque_ref))
                {
                    if (TheTabControl.SelectedTab == TabPageHistory)
                    {
                        logs_visible = true;
                    }
                    else if (AllowHistorySwitch)
                    {
                        TheTabControl.SelectedTab = TabPageHistory;
                        logs_visible = true;
                    }
                    else
                    {
                        logs_visible = false;
                    }
                }
                else
                {
                    logs_visible = false;
                }

                if (!logs_visible)
                {
                    IXenObject model =
                        (IXenObject)action.VM ??
                        (IXenObject)action.Host ??
                        (IXenObject)action.Pool ??
                        (IXenObject)action.SR;
                    if (model != null)
                        model.InError = true;
                }

                RequestRefreshTreeView();
            }
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            MainMenuBar.Location = new Point(0, 0);
            treeView.SelectedNode = treeView.Nodes[0];

            if (ToolStrip.Renderer is ToolStripProfessionalRenderer)
            {
                ((ToolStripProfessionalRenderer)ToolStrip.Renderer).RoundedEdges = false;
            }

            ConnectionsManager.XenConnections.CollectionChanged += XenConnection_CollectionChanged;
            try
            {
                Settings.RestoreSession();
            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error("Could not load settings.", ex);
                Program.CloseSplash();
                new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.MESSAGEBOX_LOAD_CORRUPTED, Settings.GetUserConfigPath()),
                       Messages.MESSAGEBOX_LOAD_CORRUPTED_TITLE)).ShowDialog(this);
                Application.Exit();
                return; // Application.Exit() does not exit the current method.
            }
            ToolbarsEnabled = Properties.Settings.Default.ToolbarsEnabled;
            RequestRefreshTreeView();
            UpdateToolbars();

            ExpandPanel(HistoryPage);

            // kick-off connections for all the loaded server list
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.SaveDisconnected)
                {
                    XenConnectionUI.BeginConnect(connection, true, this, true);

                    // if there are fewer than 30 connections, then expand the tree nodes.
                    if (ConnectionsManager.XenConnectionsCopy.Count < 30)
                    {
                        connection.CachePopulated += connection_CachePopulatedOnStartup;
                    }
                }
            }

            Program.StorageLinkConnections.CollectionChanged += StorageLinkConnections_CollectionChanged;

            RequestRefreshTreeView();

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate(object o)
            {
                // Sleep a short time before closing the splash
                Thread.Sleep(500);
                Program.Invoke(Program.MainWindow, Program.CloseSplash);
            });

            if (!Program.RunInAutomatedTestMode && !Helpers.CommonCriteriaCertificationRelease)
            {
                if (!Properties.Settings.Default.SeenAllowUpdatesDialog)
                    new AllowUpdatesDialog(pluginManager).ShowDialog(this);

                // start checkforupdates thread
                CheckForUpdatesTimer.Interval = 1000 * 60 * 60 * 24; // 24 hours
                CheckForUpdatesTimer.Tick += Updates.Tick;
                CheckForUpdatesTimer.Start();
                Updates.AutomaticCheckForUpdates();
            }
            ProcessCommand(CommandLineArgType, CommandLineParam);
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

        private void StorageLinkConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            var con = ((StorageLinkConnection)e.Element);

            if (e.Action == CollectionChangeAction.Add)
            {
                con.ConnectionStateChanged += (s, ee) =>
                    {
                        if (ee.ConnectionState == StorageLinkConnectionState.Connected)
                        {
                            Program.Invoke(this, RefreshTreeView);

                            TrySelectNewNode(o =>
                            {
                                var server = con.Cache.Server;
                                return server != null && server.Equals(o);
                            }, false, true, false);

                        }
                        Program.Invoke(this, UpdateToolbars);
                    };

                con.Cache.Changed += Cache_Changed;
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                con.Cache.Changed -= Cache_Changed;
            }
            RequestRefreshTreeView();
        }

        private void Cache_Changed(object sender, EventArgs e)
        {
            RequestRefreshTreeView();
        }

        private void connection_CachePopulatedOnStartup(object sender, EventArgs e)
        {
            IXenConnection c = (IXenConnection)sender;
            c.CachePopulated -= connection_CachePopulatedOnStartup;
            TrySelectNewNode(c, false, true, false);
        }

        private bool Launched = false;
        internal void ProcessCommand(ArgType argType, string[] args)
        {
            switch (argType)
            {
                case ArgType.Import:
                    log.DebugFormat("Importing VM export from {0}", args[0]);
                    OpenGlobalImportWizard(args[0]);
                    break;
                case ArgType.License:
                    log.DebugFormat("Installing license from {0}", args[0]);
                    LaunchLicensePicker(args[0]);
                    break;
                case ArgType.Restore:
                    log.DebugFormat("Restoring host backup from {0}", args[0]);
                    new RestoreHostFromBackupCommand(commandInterface, null, args[0]).Execute();
                    break;
                case ArgType.Update:
                    log.DebugFormat("Installing server update from {0}", args[0]);
                    InstallUpdate(args[0]);
                    break;
                case ArgType.XenSearch:
                    log.DebugFormat("Importing saved XenSearch from '{0}'", args[0]);
                    new ImportSearchCommand(commandInterface, args[0]).Execute();
                    break;
                case ArgType.Connect:
                    log.DebugFormat("Connecting to server '{0}'", args[0]);
                    IXenConnection connection = new XenConnection();
                    connection.Hostname = args[0];
                    connection.Port = ConnectionsManager.DEFAULT_XEN_PORT;
                    connection.Username = args[1];
                    connection.Password = args[2];
                    if (ConnectionsManager.XenConnectionsContains(connection))
                        break;

                    lock (ConnectionsManager.ConnectionsLock)
                        ConnectionsManager.XenConnections.Add(connection);

                    XenConnectionUI.BeginConnect(connection, true, null, false);
                    break;
                case ArgType.None:
                    if (Launched)
                    {
                        // The user has launched the splash screen, but we're already running.
                        // Draw his attention.
                        BringToFront();
                        Activate();
                    }
                    break;
                case ArgType.Passwords:
                    System.Diagnostics.Trace.Assert(false);
                    break;
            }
            Launched = true;
        }

        private void ExpandPanel(UserControl panel)
        {
            panel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            panel.Location = new Point(0, 0);
            panel.Size = new Size(panel.Parent.Width, panel.Parent.Height);
        }

        // Manages UI and network updates whenever hosts are added and removed

        void XenConnection_CollectionChanged(object sender, CollectionChangeEventArgs e)
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
                IXenConnection connection = (IXenConnection)e.Element;
                if (connection == null)
                    return;

                if (e.Action == CollectionChangeAction.Add)
                {
                    connection.ClearingCache += connection_ClearingCache;
                    connection.ConnectionResult += Connection_ConnectionResult;
                    connection.ConnectionLost += Connection_ConnectionLost;
                    connection.ConnectionClosed += Connection_ConnectionClosed;
                    connection.ConnectionReconnecting += connection_ConnectionReconnecting;
                    connection.XenObjectsUpdated += Connection_XenObjectsUpdated;
                    connection.BeforeMajorChange += Connection_BeforeMajorChange;
                    connection.AfterMajorChange += Connection_AfterMajorChange;
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
                    connection.ClearingCache -= connection_ClearingCache;
                    connection.ConnectionResult -= Connection_ConnectionResult;
                    connection.ConnectionLost -= Connection_ConnectionLost;
                    connection.ConnectionClosed -= Connection_ConnectionClosed;
                    connection.ConnectionReconnecting -= connection_ConnectionReconnecting;
                    connection.XenObjectsUpdated -= Connection_XenObjectsUpdated;
                    connection.BeforeMajorChange -= Connection_BeforeMajorChange;
                    connection.AfterMajorChange -= Connection_AfterMajorChange;
                    connection.Cache.DeregisterCollectionChanged<XenAPI.Message>(MessageCollectionChangedWithInvoke);
                    connection.Cache.DeregisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                    connection.Cache.DeregisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                    connection.Cache.DeregisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                    connection.Cache.DeregisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                    connection.Cache.DeregisterCollectionChanged<Folder>(FolderCollectionChangedWithInvoke);

                    connection.Cache.DeregisterCollectionChanged<Task>(TaskCollectionChangedWithInvoke);

                    connection.CachePopulated -= connection_CachePopulated;

                    foreach (VM vm in connection.Cache.VMs)
                    {
                        this.ConsolePanel.closeVNCForSource(vm);
                    }

                    foreach (Host host in connection.Cache.Hosts)
                        foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
                            if (vm.is_control_domain)
                                this.ConsolePanel.closeVNCForSource(vm);

                    connection.EndConnect();

                    RequestRefreshTreeView();
                    //CA-41228 refresh submenu items when there are no connections
                    selectionManager.SetSelection(selectionManager.Selection);
                }
                // update ui
                //XenAdmin.Settings.SaveServerList();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connection_ClearingCache(object sender, EventArgs e)
        {
            IXenConnection connection = (IXenConnection)sender;
            closeActiveWizards(connection);
            lock (Alert.XenCenterAlertsLock)
                Alert.XenCenterAlerts.RemoveAll(new Predicate<Alert>(delegate(Alert alert) { return alert.Connection != null && alert.Connection.Equals(connection); }));
        }

        void connection_CachePopulated(object sender, EventArgs e)
        {
            IXenConnection connection = sender as IXenConnection;
            if (connection == null)
                return;

            Host master = Helpers.GetMaster(connection);
            if (master == null)
                return;

            log.InfoFormat("Connected to {0} (version {1}, build {2}.{3}) with XenCenter {4} (build {5})",
                Helpers.GetName(master), Helpers.HostProductVersionText(master), Helpers.HostProductVersion(master),
                Helpers.HostBuildNumber(master), Branding.PRODUCT_VERSION_TEXT, Program.Version.Revision);

            // When releasing a new version of the server, we should set xencenter_min and xencenter_max on the server
            // as follows:
            //
            // xencenter_min should be the lowest version of XenCenter we want the new server to work with. In the
            // (common) case that we want to force the user to upgrade XenCenter when they upgrade the server,
            // xencenter_min should equal the current version of XenCenter.  // if (server_min > current_version)
            //
            // xencenter_max should always equal the current version of XenCenter. This ensures that even if they are
            // not required to upgrade, we at least warn them.  // else if (server_max > current_version)

            int server_min = master.XenCenterMin;
            int server_max = master.XenCenterMax;

            if (server_min > 0 && server_max > 0)
            {
                int current_version = (int)API_Version.LATEST;

                if (server_min > current_version)
                {

                    connection.EndConnect();
                    //Program.Invoke(Program.MainWindow, RefreshTreeView);
                    Program.Invoke(Program.MainWindow, delegate()
                    {
                        string url = "https://" + connection.Hostname;
                        string message = string.Format(Messages.GUI_OUT_OF_DATE, Helpers.GetName(master), url);
                        int linkStart = message.Length - url.Length - 1;
                        int linkLength = url.Length;
                        string linkUrl = url;
                        new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Error, message, linkStart, linkLength, linkUrl, Messages.CONNECTION_REFUSED_TITLE)).ShowDialog(this);

                        new ActionBase(Messages.CONNECTION_REFUSED, message, false, true, Messages.CONNECTION_REFUSED_TITLE);
                    });
                    return;
                }
                else if (server_max > current_version)
                {
                    Alert.AddAlert(new GuiOldAlert());
                }

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
                licenseTimer.CheckActiveServerLicense(connection);
            Updates.CheckServerPatches();
            Updates.CheckServerVersion();
            RequestRefreshTreeView();
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
            if (host.IsLive && host.MaintenanceMode && host.enabled)
            {
                Program.MainWindow.closeActiveWizards(host);

                AllowHistorySwitch = true;
                var action = new DisableHostAction(host);
                action.Completed += action_Completed;
                action.RunAsync();
                Program.Invoke(this, UpdateToolbars);
            }
        }

        void MessageCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            XenAPI.Message m = (XenAPI.Message)e.Element;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (!m.ShowOnGraphs && !m.IsSquelched)
                    Alert.AddAlert(MessageAlert.ParseMessage(m));
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                if (!m.ShowOnGraphs)
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
                    ConsolePanel.closeVNCForSource(vm);
                    closeActiveWizards(vm);
                }

                selectedTabs.Remove(o);
                pluginManager.DisposeURLs((IXenObject)o);
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
                    Updates.CheckServerPatches();
                    Updates.CheckServerVersion();
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
                    if (host.enabled && host.MaintenanceMode)
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
                case "other_config":
                    // other_config may contain HideFromXenCenter
                    UpdateToolbars();
                    break;

                case "name_label":
                    //check whether it's a standalone host
                    if(Helpers.GetPool(host.Connection) == null)
                        host.Connection.FriendlyName = Helpers.GetName(host);
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
                    UpdateToolbars();
                    // Make all vms have the correct start times
                    UpdateBodgedTime(vm, e.PropertyName);
                    break;

                case "other_config":
                    // other_config may contain HideFromXenCenter
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
                    UpdateToolbars();
                    break;

                case "other_config":
                    // other_config may contain HideFromXenCenter
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
                vm.BodgeStartupTime = DateTime.UtcNow; // always newer than current bodge startup time
            }
            else if (p == "other_config" && vm.other_config.ContainsKey("last_shutdown_time"))
            {
                DateTime newTime = vm.LastShutdownTime;
                if (newTime != DateTime.MinValue && newTime.Ticks > vm.BodgeStartupTime.Ticks)
                    vm.BodgeStartupTime = newTime; // only update if is newer than current bodge startup time
            }
        }

        void Connection_ConnectionResult(object sender, Network.ConnectionResultEventArgs e)
        {
            RequestRefreshTreeView();
            Program.Invoke(this, (EventHandler<ConnectionResultEventArgs>)Connection_ConnectionResult_, sender, e);
        }

        private void Connection_ConnectionResult_(object sender, Network.ConnectionResultEventArgs e)
        {
            Program.AssertOnEventThread();
            try
            {
                UpdateToolbars();
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing more about this.
            }
        }

        void Connection_ConnectionClosed(object sender, EventArgs e)
        {
            RequestRefreshTreeView();
            Program.Invoke(this, (EventHandler<Network.ConnectionResultEventArgs>)Connection_ConnectionClosed_, sender, e);
            gc();
        }

        private void Connection_ConnectionClosed_(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            try
            {
                UpdateToolbars();
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Nothing more we can do with this.
            }
        }

        // called whenever our connection with the Xen server fails (i.e., after we've successfully logged in)
        void Connection_ConnectionLost(object sender, EventArgs e)
        {
            if (Program.Exiting)
                return;
            Program.Invoke(this, (EventHandler)Connection_ConnectionLost_, sender, e);
            RequestRefreshTreeView();
            gc();
        }

        private void Connection_ConnectionLost_(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            try
            {
                IXenConnection connection = (IXenConnection)sender;
                closeActiveWizards(connection);

                UpdateToolbars();
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing about this.
            }
        }

        private static void gc()
        {
            log_gc("Before");
            GC.Collect();
            log_gc("After");
        }

        private static void log_gc(string when)
        {
            log.DebugFormat("{0} GC: approx {1} bytes in use", when, GC.GetTotalMemory(false));
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                log.DebugFormat("Number of times GC has occurred for generation {0} objects: {1}", i, GC.CollectionCount(i));
            }
            log.Debug("GDI objects in use: " + Win32.GetGuiResourcesGDICount(Process.GetCurrentProcess().Handle));
            log.Debug("USER objects in use: " + Win32.GetGuiResourcesUserCount(Process.GetCurrentProcess().Handle));
        }

        void connection_ConnectionReconnecting(object sender, EventArgs e)
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

        void Connection_BeforeMajorChange(object sender, ConnectionMajorChangeEventArgs e)
        {
            if (e.Background)
                Connection_BeforeBackgroundMajorChange();
            else
                Program.Invoke(this, (EventHandler)Connection_BeforeMajorChange_, sender, e);
        }

        private void Connection_BeforeMajorChange_(object sender, EventArgs eventArgs)
        {
            Program.AssertOnEventThread();
            try
            {
                if (inMajorChange)
                    return;

                inMajorChange = true;
                SuspendRefreshTreeView();
                SuspendUpdateToolbars();
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing more about this.
            }
        }

        private void Connection_BeforeBackgroundMajorChange()
        {
            Program.AssertOffEventThread();
            try
            {
                Program.Invoke(this, delegate()
                {
                    SuspendRefreshTreeView();
                    SuspendUpdateToolbars();
                });
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing more about this.
            }
        }

        void Connection_AfterMajorChange(object sender, ConnectionMajorChangeEventArgs e)
        {
            if (e.Background)
                Connection_AfterBackgroundMajorChange();
            else
                Program.Invoke(this, (EventHandler)Connection_AfterMajorChange_, sender, e);
        }

        private void Connection_AfterMajorChange_(object sender, EventArgs eventArgs)
        {
            Program.AssertOnEventThread();
            try
            {
                try
                {
                    ResumeUpdateToolbars();
                }
                finally
                {
                    ResumeRefreshTreeView();
                }

                inMajorChange = false;
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing more about this.
            }
        }

        private void Connection_AfterBackgroundMajorChange()
        {
            Program.AssertOffEventThread();
            try
            {
                Program.Invoke(this, delegate()
                {
                    try
                    {
                        ResumeUpdateToolbars();
                    }
                    finally
                    {
                        ResumeRefreshTreeView();
                    }
                });
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Can do nothing more about this.
            }
        }

        private bool inMajorChange = false;

        public void MajorChange(MethodInvoker f)
        {
            Program.AssertOnEventThread();

            if (inMajorChange)
            {
                f();
                return;
            }

            inMajorChange = true;
            SuspendRefreshTreeView();
            SuspendUpdateToolbars();
            try
            {
                f();
            }
            catch (Exception e)
            {
                log.Debug("Exception thrown by target of MajorChange.", e);
                log.Debug(e, e);
                throw;
            }
            finally
            {
                try
                {
                    ResumeUpdateToolbars();
                }
                finally
                {
                    ResumeRefreshTreeView();
                }

                inMajorChange = false;
            }
        }

        public void BackgroundMajorChange(MethodInvoker f)
        {
            Program.AssertOffEventThread();

            Program.Invoke(this, delegate()
            {
                SuspendRefreshTreeView();
                SuspendUpdateToolbars();
            });
            try
            {
                f();
            }
            catch (Exception e)
            {
                log.Debug("Exception thrown by target of BackgroundMajorChange.", e);
                log.Debug(e, e);
                throw;
            }
            finally
            {
                Program.Invoke(this, delegate()
                {
                    try
                    {
                        ResumeUpdateToolbars();
                    }
                    finally
                    {
                        ResumeRefreshTreeView();
                    }
                });
            }
        }

        private static bool IsPool(VirtualTreeNode node)
        {
            return (node != null && node.Tag != null && node.Tag is XenAPI.Pool);
        }

        private static bool IsHost(VirtualTreeNode node)
        {
            return IsXenModelObject(node) && node.Tag is Host;
        }

        private static bool IsSR(VirtualTreeNode node)
        {
            return (node != null && node.Tag != null && node.Tag is SR);
        }

        private static bool IsVM(VirtualTreeNode node)
        {
            return node != null && node.Tag != null && node.Tag is VM;
        }

        /// <summary>
        /// A "real" VM is one that's not a template.
        /// </summary>
        private static bool IsRealVM(VirtualTreeNode node)
        {
            return IsVM(node) && !GetVM(node).is_a_template;
        }

        private static bool IsTemplate(VirtualTreeNode node)
        {
            return IsVM(node) && GetVM(node).is_a_template && !GetVM(node).is_a_snapshot;
        }

        private static bool IsSnapshot(VirtualTreeNode node)
        {
            return IsVM(node) && GetVM(node).is_a_snapshot;
        }

        private static bool IsXenModelObject(VirtualTreeNode node)
        {
            return node != null && node.Tag != null && node.Tag is IXenObject;
        }

        private static bool IsGroup(VirtualTreeNode node)
        {
            return node != null && node.Tag != null && node.Tag is GroupingTag;
        }

        private static IXenConnection GetXenConnection(VirtualTreeNode node)
        {
            if (node == null)
                return null;
            else if (node.Tag is IXenConnection)
                return (IXenConnection)node.Tag;
            else if (node.Tag is IXenObject)
                return ((IXenObject)node.Tag).Connection;
            else
                return null;
        }

        private static Pool GetPool(VirtualTreeNode node)
        {
            return IsPool(node) ? (Pool)node.Tag : null;
        }

        private static Host GetHost(VirtualTreeNode node)
        {
            return IsHost(node) ? (Host)node.Tag : null;
        }

        private static SR GetSR(VirtualTreeNode node)
        {
            return IsSR(node) ? (SR)node.Tag : null;
        }

        private static VM GetVM(VirtualTreeNode node)
        {
            return IsVM(node) ? (VM)node.Tag : null;
        }

        // Finds the pool of a node, if the object is in a pool
        private static Pool PoolOfNode(VirtualTreeNode node)
        {
            IXenConnection connection = GetXenConnection(node);
            return connection == null ? null : Helpers.GetPool(connection);
        }

        // Finds the pool of a node, if it's an ancestor node in the tree
        private static VirtualTreeNode PoolAncestorNodeOfNode(VirtualTreeNode node)
        {
            while (node != null)
            {
                if (IsPool(node))
                    return node;
                node = node.Parent;
            }
            return null;
        }

        private static Pool PoolAncestorOfNode(VirtualTreeNode node)
        {
            return GetPool(PoolAncestorNodeOfNode(node));
        }

        // Finds the host of a node, if it's an ancestor node in the tree
        private static VirtualTreeNode HostAncestorNodeOfNode(VirtualTreeNode node)
        {
            while (node != null)
            {
                if (IsHost(node))
                    return node;
                node = node.Parent;
            }
            return null;
        }

        private static Host HostAncestorOfNode(VirtualTreeNode node)
        {
            return GetHost(HostAncestorNodeOfNode(node));
        }

        private int ignoreRefreshTreeView = 0;
        private bool calledRefreshTreeView = false;
        private int ignoreUpdateToolbars = 0;
        private bool calledUpdateToolbars = false;

        void SuspendRefreshTreeView()
        {
            Program.AssertOnEventThread();
            if (ignoreRefreshTreeView == 0)
            {
                calledRefreshTreeView = false;
            }
            ignoreRefreshTreeView++;
        }

        void ResumeRefreshTreeView()
        {
            Program.AssertOnEventThread();
            ignoreRefreshTreeView--;
            if (ignoreRefreshTreeView == 0 && calledRefreshTreeView)
            {
                RequestRefreshTreeView();
            }
        }

        void SuspendUpdateToolbars()
        {
            Program.AssertOnEventThread();
            if (ignoreUpdateToolbars == 0)
            {
                calledUpdateToolbars = false;
            }
            ignoreUpdateToolbars++;
        }

        void ResumeUpdateToolbars()
        {
            Program.AssertOnEventThread();
            ignoreUpdateToolbars--;
            if (ignoreUpdateToolbars == 0 && calledUpdateToolbars)
            {
                UpdateToolbars();
            }
        }

        private void treeViewUpdateManager_Update(object sender, EventArgs e)
        {
            Program.AssertOffEventThread();
            RefreshTreeView();
        }

        /// <summary>
        /// Requests a refresh of the main tree view. The refresh will be managed such that we are not overloaded using an UpdateManager.
        /// </summary>
        public void RequestRefreshTreeView()
        {
            if (!Program.Exiting)
            {
                treeViewUpdateManager.RequestUpdate();
            }
        }

        /// <summary>
        /// Refreshes the main tree view. A new node tree is built on the calling thread and then merged into the main tree view
        /// on the main program thread.
        /// </summary>
        internal void RefreshTreeView()
        {
            VirtualTreeNode newRootNode = treeBuilder.CreateNewRootNode(TreeSearchBox.Search.AddFullTextFilter(searchTextBox.Text), TreeSearchBoxMode);

            Program.Invoke(this, () => RefreshTreeView(newRootNode));
        }

        /// <summary>
        /// Refreshes the tree view. The specified node tree is merged with the current node tree.
        /// </summary>
        /// <param name="newRootNode">The new node tree for the main treeview..</param>
        private void RefreshTreeView(VirtualTreeNode newRootNode)
        {
            Program.AssertOnEventThread();

            if (ignoreRefreshTreeView > 0)
            {
                calledRefreshTreeView = true;
                return;
            }

            ignoreRefreshTreeView++;  // Some events can be ignored while rebuilding the tree

            try
            {
                treeBuilder.RefreshTreeView(newRootNode, searchTextBox.Text, TreeSearchBoxMode);
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                    throw;
#endif
            }
            finally
            {
                ignoreRefreshTreeView--;
            }

            UpdateHeader();

            // This is required to update search results when things change.
            if (TheTabControl.SelectedTab == TabPageGeneral)
                GeneralPage.BuildList();
            else if (TheTabControl.SelectedTab == TabPageSearch)
                SearchPage.BuildList();
        }



        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool _menuShortcuts = true;
        public bool MenuShortcuts
        {
            set
            {
                if (value != _menuShortcuts)
                {
                    //if the VNC Console is active (the user is typing into it etc) all of the shortcuts for XenCenter are disabled
                    //IMPORTANT! add any shortcuts you want to pass to the VNC console into this if, else statement
                    _menuShortcuts = value;

                    // update the selection so menu items can enable/disable keyboard shortcuts as appropriate.
                    selectionManager.SetSelection(selectionManager.Selection);
                }
            }
        }

        /// <summary>
        /// Must be called on the event thread.
        /// </summary>
        public void UpdateToolbars()
        {
            Program.AssertOnEventThread();

            if (ignoreUpdateToolbars > 0)
            {
                calledUpdateToolbars = true;
                return;
            }

            ToolStrip.SuspendLayout();

            UpdateToolbarsCore();
            MainMenuBar_MenuActivate(null, null);

            ToolStrip.ResumeLayout();
        }

        private static int TOOLBAR_HEIGHT = 31;
        /// <summary>
        /// Updates the toolbar buttons. Also updates which tabs are visible.
        /// </summary>
        public void UpdateToolbarsCore()
        {
            // refresh the selection-manager
            selectionManager.SetSelection(selectionManager.Selection);

            ToolStrip.Height = ToolbarsEnabled ? TOOLBAR_HEIGHT : 0;
            ToolStrip.Enabled = ToolbarsEnabled;
            MenuBarToolStrip.Visible = MenuBarToolStrip.Enabled = !ToolbarsEnabled;
            ShowToolbarMenuItem.Checked = toolbarToolStripMenuItem.Checked = ToolbarsEnabled;

            powerOnHostToolStripButton.Available = powerOnHostToolStripButton.Enabled;
            startVMToolStripButton.Available = startVMToolStripButton.Enabled;
            shutDownToolStripButton.Available = shutDownToolStripButton.Enabled || (!startVMToolStripButton.Available && !powerOnHostToolStripButton.Available);

            resumeToolStripButton.Available = resumeToolStripButton.Enabled;
            SuspendToolbarButton.Available = SuspendToolbarButton.Enabled || !resumeToolStripButton.Available;

            ForceRebootToolbarButton.Available = ((ForceVMRebootCommand)ForceRebootToolbarButton.Command).ShowOnMainToolBar;
            ForceShutdownToolbarButton.Available = ((ForceVMShutDownCommand)ForceShutdownToolbarButton.Command).ShowOnMainToolBar;

            IXenConnection selectionConnection = selectionManager.Selection.GetConnectionOfFirstItem();
            Pool selectionPool = selectionConnection == null ? null : Helpers.GetPool(selectionConnection);
            Host selectionMaster = null == selectionPool ? null : selectionPool.Connection.Resolve(selectionPool.master);

            // 'Home' tab is only visible if the 'Overview' tree node is selected, or if the tree is
            // empty (i.e. at startup).
            bool show_home = selectionManager.Selection.Count == 1 && selectionManager.Selection[0].Value == null;
            // Only show the HA tab if the host's license has the HA flag set
            bool has_ha_license_flag = selectionMaster != null && !selectionMaster.RestrictHAOrlando;
            bool george_or_greater = Helpers.GeorgeOrGreater(selectionConnection);
            bool mr_or_greater = Helpers.MidnightRideOrGreater(selectionConnection);
            // The upsell pages use the first selected XenObject: but they're only shown if there is only one selected object (see calls to ShowTab() below).
            bool dmc_upsell = Helpers.FeatureForbidden(selectionManager.Selection.FirstAsXenObject, Host.RestrictDMC);
            bool ha_upsell = Helpers.FeatureForbidden(selectionManager.Selection.FirstAsXenObject, Host.RestrictHAFloodgate);
            bool wlb_upsell = Helpers.FeatureForbidden(selectionManager.Selection.FirstAsXenObject, Host.RestrictWLB);
            bool is_connected = selectionConnection != null && selectionConnection.IsConnected;

            bool multi = SelectionManager.Selection.Count > 1;

            bool isPoolSelected = selectionManager.Selection.FirstIsPool;
            bool isVMSelected = selectionManager.Selection.FirstIsVM;
            bool isHostSelected = selectionManager.Selection.FirstIsHost;
            bool isSRSelected = selectionManager.Selection.FirstIsSR;
            bool isRealVMSelected = selectionManager.Selection.FirstIsRealVM;
            bool isTemplateSelected = selectionManager.Selection.FirstIsTemplate;
            bool isHostLive = selectionManager.Selection.FirstIsLiveHost;
            bool isStorageLinkSelected = selectionManager.Selection.FirstIsStorageLink;
            bool isStorageLinkSRSelected = selectionManager.Selection.First is StorageLinkRepository && ((StorageLinkRepository)selectionManager.Selection.First).SR(ConnectionsManager.XenConnectionsCopy) != null;

            bool selectedTemplateHasProvisionXML = selectionManager.Selection.FirstIsTemplate && ((VM)selectionManager.Selection[0].XenObject).HasProvisionXML;

            NewTabCount = 0;
            ShowTab(TabPageHome, !SearchMode && show_home);
            ShowTab(TabPageSearch, (SearchMode || show_home || SearchTabVisible));
            ShowTab(TabPageGeneral, !multi && !SearchMode && (isVMSelected || (isHostSelected && (isHostLive || !is_connected)) || isPoolSelected || isSRSelected || isStorageLinkSelected));
            ShowTab(dmc_upsell ? TabPageBallooningUpsell : TabPageBallooning, !multi && !SearchMode && mr_or_greater && (isVMSelected || (isHostSelected && isHostLive) || isPoolSelected));
            ShowTab(TabPageStorage, !multi && !SearchMode && (isRealVMSelected || (isTemplateSelected && !selectedTemplateHasProvisionXML)));
            ShowTab(TabPageSR, !multi && !SearchMode && (isSRSelected || isStorageLinkSRSelected));
            ShowTab(TabPagePhysicalStorage, !multi && !SearchMode && ((isHostSelected && isHostLive) || isPoolSelected));
            ShowTab(TabPageNetwork, !multi && !SearchMode && (isVMSelected || (isHostSelected && isHostLive) || isPoolSelected));
            ShowTab(TabPageNICs, !multi && !SearchMode && ((isHostSelected && isHostLive)));

            pluginManager.SetSelectedXenObject(selectionManager.Selection.FirstAsXenObject);

            bool shownConsoleReplacement = false;
            foreach (TabPageFeature f in pluginManager.GetAllFeatures<TabPageFeature>(f => f.IsConsoleReplacement && !f.IsError && !multi && f.ShowTab))
            {
                ShowTab(f.TabPage, true);
                shownConsoleReplacement = true;
            }

            ShowTab(TabPageConsole, !shownConsoleReplacement && !multi && !SearchMode && (isRealVMSelected || (isHostSelected && isHostLive)));
            ShowTab(TabPagePeformance, !multi && !SearchMode && (isRealVMSelected || (isHostSelected && isHostLive)));
            ShowTab(ha_upsell ? TabPageHAUpsell : TabPageHA, !multi && !SearchMode && isPoolSelected && has_ha_license_flag);
            ShowTab(TabPageSnapshots, !multi && !SearchMode && george_or_greater && isRealVMSelected);
            
            //Disable the WLB tab from Clearwater onwards
            if(selectionManager.Selection.All(s=>!Helpers.ClearwaterOrGreater(s.Connection)))
                ShowTab(wlb_upsell ? TabPageWLBUpsell : TabPageWLB, !multi && !SearchMode && isPoolSelected && george_or_greater);
            
            ShowTab(TabPageAD, !multi && !SearchMode && (isPoolSelected || isHostSelected && isHostLive) && george_or_greater);

            // put plugin tabs before logs tab

            foreach (TabPageFeature f in pluginManager.GetAllFeatures<TabPageFeature>(f => !f.IsConsoleReplacement && !multi && f.ShowTab))
            {
                ShowTab(f.TabPage, true);
            }

            ShowTab(TabPageHistory, !SearchMode);

            // N.B. Change NewTabs definition if you add more tabs here.

            // Save and restore focus on treeView, since selecting tabs in ChangeToNewTabs() has the
            // unavoidable side-effect of giving them focus - this is irritating if trying to navigate
            // the tree using the keyboard.

            MajorChange(() =>
                {
                    bool treeViewHasFocus = treeView.ContainsFocus;
                    searchTextBox.SaveState();

                    ChangeToNewTabs();

                    if (!searchMode && treeViewHasFocus)
                        treeView.Focus();
                    searchTextBox.RestoreState();
                });
        }

        private bool SearchTabVisible
        {
            get
            {
                if (selectionManager.Selection.Count == 1)
                {
                    Host host = selectionManager.Selection[0].XenObject as Host;

                    if (host != null)
                    {
                        return host.IsLive;
                    }

                    if (selectionManager.Selection[0].XenObject is Pool)
                    {
                        return true;
                    }

                    if (selectionManager.Selection[0].GroupingTag != null)
                    {
                        return true;
                    }

                    if (selectionManager.Selection[0].XenObject is Folder)
                    {
                        return true;
                    }
                }
                else if (selectionManager.Selection.Count > 1)
                {
                    return true;
                }
                return false;
            }
        }

        private readonly TabPage[] NewTabs = new TabPage[512];
        int NewTabCount;
        private void ShowTab(TabPage page, bool visible)
        {
            if (visible)
            {
                NewTabs[NewTabCount] = page;
                NewTabCount++;
            }
        }

        private void ChangeToNewTabs()
        {
            TabPage new_selected_page = NewSelectedPage();
            TheTabControl.SuspendLayout();
            IgnoreTabChanges = true;
            try
            {
                TabControl.TabPageCollection p = TheTabControl.TabPages;
                int i = 0; // Index into NewTabs
                int m = 0; // Index into p

                while (i < NewTabCount)
                {
                    if (m == p.Count)
                    {
                        p.Add(NewTabs[i]);
                        if (new_selected_page == NewTabs[i])
                            TheTabControl.SelectedTab = new_selected_page;
                        m++;
                        i++;
                    }
                    else if (p[m] == NewTabs[i])
                    {
                        if (new_selected_page == NewTabs[i])
                            TheTabControl.SelectedTab = new_selected_page;
                        m++;
                        i++;
                    }
                    else if (NewTabsContains(p[m]))
                    {
                        p.Insert(m, NewTabs[i]);
                        if (new_selected_page == NewTabs[i])
                            TheTabControl.SelectedTab = new_selected_page;
                        m++;
                        i++;
                    }
                    else
                    {
                        if (TheTabControl.SelectedTab == p[m] && new_selected_page == NewTabs[i])
                        {
                            // This clause is deliberately targeted at the case when you go from
                            // Overview:Home to Host:Overview.
                            p.Insert(m, NewTabs[i]);
                            TheTabControl.SelectedTab = new_selected_page;
                            m++;
                            i++;
                        }
                        p.Remove(p[m]);
                    }
                }

                // Remove any tabs that are left at the end of the list.
                while (m < p.Count)
                {
                    TabPage removed = p[p.Count - 1];
                    p.Remove(removed);
                    int index = NewTabsIndexOf(removed);
                    if (index != -1)
                    {
                        // If this is a tab that we want, then we've got it in the list twice -- one
                        // reference was here when we entered this function, and is the one that we're
                        // pointing at now, and the other reference we've inserted through the loop above.
                        // This is bad -- p.Remove(removed) above has now invalidated removed.Parent
                        // (removed is still in the list through the other reference).  Fix this up by
                        // removing the other reference too and starting over.
                        // We can't do the Remove call in the loop above, because this has a poor visual
                        // effect.
                        p.Remove(removed);
                        p.Insert(index, removed);
                        if (new_selected_page == removed)
                            TheTabControl.SelectedTab = removed;
                    }
                }
            }
            finally
            {
                IgnoreTabChanges = false;
                TheTabControl.ResumeLayout();

                SetLastSelectedPage(selectionManager.Selection.First, TheTabControl.SelectedTab);
            }
        }

        private TabPage NewSelectedPage()
        {
            Object o = selectionManager.Selection.First;
            IXenObject s = o as IXenObject;
            if (s != null && s.InError && AllowHistorySwitch)
            {
                s.InError = false;
                return TabPageHistory;
            }
            else
            {
                TabPage last_selected_page = GetLastSelectedPage(o);
                return
                    last_selected_page != null && NewTabsContains(last_selected_page) ?
                        last_selected_page :
                        NewTabs[0];
            }
        }

        public void SetLastSelectedPage(object o, TabPage p)
        {
            if (searchMode)
                return;

            if (o == null)
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
            return
                o == null ? selectedOverviewTab :
                selectedTabs.ContainsKey(o) ? selectedTabs[o] :
                                               null;
        }

        private bool NewTabsContains(TabPage tp)
        {
            return NewTabsIndexOf(tp) != -1;
        }

        private int NewTabsIndexOf(TabPage tp)
        {
            for (int i = 0; i < NewTabCount; i++)
            {
                if (NewTabs[i] == tp)
                    return i;
            }
            return -1;
        }

        private void topLevelMenu_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;

            //clear existing plugin items
            for (int i = menu.DropDownItems.Count - 1; i >= 0; i--)
            {
                CommandToolStripMenuItem commandMenuItem = menu.DropDownItems[i] as CommandToolStripMenuItem;

                if (commandMenuItem != null && (commandMenuItem.Command is MenuItemFeatureCommand || commandMenuItem.Command is ParentMenuItemFeatureCommand))
                {
                    menu.DropDownItems.RemoveAt(i);

                    if (menu.DropDownItems[i] is ToolStripSeparator)
                    {
                        menu.DropDownItems.RemoveAt(i);
                    }
                }
            }

            // get insert index using the placeholder
            int insertIndex = pluginMenuItemStartIndexes[menu];

            bool addSeparatorAtEnd = false;

            // add plugin items for this menu at insertIndex
            foreach (PluginDescriptor plugin in pluginManager.Plugins)
            {
                if (!plugin.Enabled)
                {
                    continue;
                }

                foreach (Feature feature in plugin.Features)
                {
                    MenuItemFeature menuItemFeature = feature as MenuItemFeature;

                    if (menuItemFeature != null && menuItemFeature.ParentFeature == null && (int)menuItemFeature.Menu == MainMenuBar.Items.IndexOf(menu))
                    {
                        Command cmd = menuItemFeature.GetCommand(commandInterface, SelectionManager.Selection);

                        menu.DropDownItems.Insert(insertIndex, new CommandToolStripMenuItem(cmd));
                        insertIndex++;
                        addSeparatorAtEnd = true;
                    }

                    ParentMenuItemFeature parentMenuItemFeature = feature as ParentMenuItemFeature;

                    if (parentMenuItemFeature != null && (int)parentMenuItemFeature.Menu == MainMenuBar.Items.IndexOf(menu))
                    {
                        Command cmd = parentMenuItemFeature.GetCommand(commandInterface, SelectionManager.Selection);
                        CommandToolStripMenuItem parentMenuItem = new CommandToolStripMenuItem(cmd);

                        menu.DropDownItems.Insert(insertIndex, parentMenuItem);
                        insertIndex++;
                        addSeparatorAtEnd = true;

                        foreach (MenuItemFeature childFeature in parentMenuItemFeature.Features)
                        {
                            Command childCommand = childFeature.GetCommand(commandInterface, SelectionManager.Selection);
                            parentMenuItem.DropDownItems.Add(new CommandToolStripMenuItem(childCommand));
                        }
                    }
                }
            }

            if (addSeparatorAtEnd)
            {
                menu.DropDownItems.Insert(insertIndex, new ToolStripSeparator());
            }
        }

        private void MainMenuBar_MenuActivate(object sender, EventArgs e)
        {
            Host hostAncestor = selectionManager.Selection.Count == 1 ? selectionManager.Selection[0].HostAncestor : null;
            IXenConnection connection = selectionManager.Selection.GetConnectionOfFirstItem();
            bool vm = selectionManager.Selection.FirstIsRealVM && !((VM)selectionManager.Selection.First).Locked;

            Host best_host = hostAncestor ?? (connection == null ? null : Helpers.GetMaster(connection));
            bool george_or_greater = best_host != null && Helpers.GeorgeOrGreater(best_host);

            exportSettingsToolStripMenuItem.Enabled = ConnectionsManager.XenConnectionsCopy.Count > 0;
            bugToolToolStripMenuItem.Enabled = HelpersGUI.AtLeastOneConnectedConnection();

            this.MenuShortcuts = true;

            startOnHostToolStripMenuItem.Available = startOnHostToolStripMenuItem.Enabled;
            resumeOnToolStripMenuItem.Available = resumeOnToolStripMenuItem.Enabled;
            relocateToolStripMenuItem.Available = relocateToolStripMenuItem.Enabled;
            storageLinkToolStripMenuItem.Available = storageLinkToolStripMenuItem.Enabled;
            sendCtrlAltDelToolStripMenuItem.Enabled = (TheTabControl.SelectedTab == TabPageConsole) && vm && ((VM)selectionManager.Selection.First).power_state == vm_power_state.Running;

            templatesToolStripMenuItem1.Checked = Properties.Settings.Default.DefaultTemplatesVisible;
            customTemplatesToolStripMenuItem.Checked = Properties.Settings.Default.UserTemplatesVisible;
            localStorageToolStripMenuItem.Checked = Properties.Settings.Default.LocalSRsVisible;
            ShowHiddenObjectsToolStripMenuItem.Checked = Properties.Settings.Default.ShowHiddenVMs;
            connectDisconnectToolStripMenuItem.Enabled = ConnectionsManager.XenConnectionsCopy.Count > 0;

            checkForUpdatesToolStripMenuItem.Available = !Helpers.CommonCriteriaCertificationRelease;
        }

        // Which XenObject's are selectable in the tree, and draggable in the search results?
        public static bool IsSelectableXenModelObject(IXenObject o)
        {
            return o != null;
        }

        public static bool CanSelectNode(VirtualTreeNode node)
        {
            if (node.Tag == null) // XenCenter node
                return true;
            if (node.Tag is IXenObject)
                return IsSelectableXenModelObject(node.Tag as IXenObject);
            if (node.Tag is GroupingTag)
                return true;
            return false;
        }

        private void TreeView_BeforeSelect(object sender, VirtualTreeViewCancelEventArgs e)
        {
            if (e.Node == null)
                return;

            if (!CanSelectNode(e.Node))
            {
                e.Cancel = true;
                return;
            }

            SearchMode = false;

            AllowHistorySwitch = false;
        }

        private void treeView_SelectionsChanged(object sender, EventArgs e)
        {
            // this is fired when the selection of the main treeview changes.

            List<SelectedItem> items = new List<SelectedItem>();
            foreach (VirtualTreeNode node in treeView.SelectedNodes)
            {
                GroupingTag groupingTag = node.Tag as GroupingTag;
                IXenObject xenObject = node.Tag as IXenObject;

                if (xenObject != null)
                {
                    items.Add(new SelectedItem(xenObject, GetXenConnection(node), HostAncestorOfNode(node), PoolAncestorOfNode(node)));
                }
                else
                {
                    items.Add(new SelectedItem(groupingTag));
                }
            }

            // setting this sets the XenCenter selection. Everything that needs to know about the selection and
            // selection changes should use this object.

            selectionManager.SetSelection(items);

            UpdateToolbars();

            //
            // NB do not trigger updates to the panels in this method
            // instead, put them in TheTabControl_SelectedIndexChanged,
            // so only the selected tab is updated
            //

            TheTabControl_SelectedIndexChanged(sender, EventArgs.Empty);

            if (TheTabControl.SelectedTab != null)
                TheTabControl.SelectedTab.Refresh();

            UpdateHeader();
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
            if (theAboutDialog == null || theAboutDialog.IsDisposed)
            {
                theAboutDialog = new AboutDialog();
                theAboutDialog.Show(this);
            }
            else
            {
                theAboutDialog.BringToFront();
                theAboutDialog.Focus();
            }
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
                    dialog.Title = Messages.INSTALL_LICENSE_KEY;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    dialog.Filter = string.Format("{0} (*.xslic)|*.xslic|{1} (*.*)|*.*", Messages.XS_LICENSE_FILES, Messages.ALL_FILES);
                    dialog.ShowHelp = true;
                    dialog.HelpRequest += new EventHandler(dialog_HelpRequest);
                    result = dialog.ShowDialog(this);
                }
            }
            else
            {
                result = DialogResult.OK;
            }

            if (result == DialogResult.OK || Program.RunInAutomatedTestMode)
            {
                filepath = Program.RunInAutomatedTestMode ? "" : filepath == "" ? dialog.FileName : filepath;

                Host hostAncestor = selectionManager.Selection.Count == 1 ? selectionManager.Selection[0].HostAncestor : null;

                if (selectionManager.Selection.Count == 1 && hostAncestor == null)
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
            AllowHistorySwitch = true;
            ApplyLicenseAction action = new ApplyLicenseAction(host.Connection, host, filePath);
            ActionProgressDialog actionProgress = new ActionProgressDialog(action, ProgressBarStyle.Marquee);

            actionProgress.Text = Messages.INSTALL_LICENSE_KEY;
            actionProgress.ShowDialog(this);
        }

        private void dialog_HelpRequest(object sender, EventArgs e)
        {
            Help.HelpManager.Launch("LicenseKeyDialog");
        }

        private void TreeView_NodeMouseClick(object sender, VirtualTreeNodeMouseClickEventArgs e)
        {
            try
            {
                TreeView_NodeMouseClick_(sender, e);

                if (SearchMode)
                {
                    SearchMode = false;
                    TheTabControl_SelectedIndexChanged(null, null);
                    UpdateHeader();
                }
            }
            catch (Exception exn)
            {
                log.Error(exn, exn);
                // Swallow this exception -- there's no point throwing it on.
#if DEBUG
                throw;
#endif
            }
        }

        private void TreeView_NodeMouseClick_(object sender, VirtualTreeNodeMouseClickEventArgs e)
        {
            if (treeView.Nodes.Count < 1)
                return;

            if (e.Button != MouseButtons.Right)
                return;

            // Handle r-click menu stuff

            if (treeView.SelectedNodes.Count == 0)
            {
                treeView.SelectedNode = e.Node;

                if (treeView.SelectedNode != e.Node)  // if node is unselectable in TreeView_BeforeSelect: CA-26615
                {
                    return;
                }
            }
            else if (treeView.SelectedNodes.Contains(e.Node))
            {
                // don't change the selection - just show the menu.
            }
            else if (CanSelectNode(e.Node))
            {
                treeView.SelectedNode = e.Node;
            }
            else
            {
                // can't select node - don't show menu.
                return;
            }

            MainMenuBar_MenuActivate(MainMenuBar, new EventArgs());

            TreeContextMenu.SuspendLayout();
            TreeContextMenu.Items.Clear();

            if (e.Node == treeView.Nodes[0] && treeView.SelectedNodes.Count == 1)
            {
                // XenCenter (top most)

                TreeContextMenu.Items.Add(new CommandToolStripMenuItem(new AddHostCommand(commandInterface), true));
                TreeContextMenu.Items.Add(new CommandToolStripMenuItem(new NewPoolCommand(commandInterface, new SelectedItem[0]), true));
                TreeContextMenu.Items.Add(new CommandToolStripMenuItem(new ConnectAllHostsCommand(commandInterface), true));
                TreeContextMenu.Items.Add(new CommandToolStripMenuItem(new DisconnectAllHostsCommand(commandInterface), true));
            }
            else
            {
                TreeContextMenu.Items.AddRange(contextMenuBuilder.Build(SelectionManager.Selection));
            }

            int insertIndex = TreeContextMenu.Items.Count;

            if (TreeContextMenu.Items.Count > 0)
            {
                CommandToolStripMenuItem lastItem = TreeContextMenu.Items[TreeContextMenu.Items.Count - 1] as CommandToolStripMenuItem;

                if (lastItem != null && lastItem.Command is PropertiesCommand)
                {
                    insertIndex--;
                }
            }

            AddExpandCollapseItems(insertIndex, treeView.SelectedNodes, TreeContextMenu);
            AddOrgViewItems(insertIndex, treeView.SelectedNodes, TreeContextMenu);

            TreeContextMenu.ResumeLayout();

            if (TreeContextMenu.Items.Count > 0)
            {
                TreeContextMenu.Show(treeView, e.Location);
            }
        }

        private void AddExpandCollapseItems(int insertIndex, IList<VirtualTreeNode> nodes, ContextMenuStrip contextMenuStrip)
        {
            if (nodes.Count == 1 && nodes[0].Nodes.Count == 0)
            {
                return;
            }

            Command cmd = new CollapseChildTreeNodesCommand(commandInterface, nodes);
            if (cmd.CanExecute())
            {
                contextMenuStrip.Items.Insert(insertIndex, new CommandToolStripMenuItem(cmd, true));
            }

            cmd = new ExpandTreeNodesCommand(commandInterface, nodes);
            if (cmd.CanExecute())
            {
                contextMenuStrip.Items.Insert(insertIndex, new CommandToolStripMenuItem(cmd, true));
            }
        }

        private void AddOrgViewItems(int insertIndex, IList<VirtualTreeNode> nodes, ContextMenuStrip contextMenuStrip)
        {
            if ((TreeSearchBoxMode != XenAdmin.Controls.TreeSearchBox.Mode.Objects
                && TreeSearchBoxMode != XenAdmin.Controls.TreeSearchBox.Mode.Organization)
                || nodes.Count == 0)
            {
                return;
            }

            Command cmd = new RemoveFromFolderCommand(commandInterface, nodes);

            if (cmd.CanExecute())
            {
                contextMenuStrip.Items.Insert(insertIndex, new CommandToolStripMenuItem(cmd, true));
            }

            cmd = new UntagCommand(commandInterface, nodes);

            if (cmd.CanExecute())
            {
                contextMenuStrip.Items.Insert(insertIndex, new CommandToolStripMenuItem(cmd, true));
            }
        }

        /// <param name="e">
        /// If null, then we deduce the method was called by TreeView_AfterSelect
        /// and don't focus the VNC console. i.e. we only focus the VNC console if the user
        /// explicitly clicked on the console tab rather than arriving there by navigating
        /// in treeView.
        /// </param>
        private void TheTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowHistorySwitch = false;
            if (IgnoreTabChanges)
                return;

            TabPage t = TheTabControl.SelectedTab;

            if (!SearchMode)
            {
                History.NewHistoryItem(new XenModelObjectHistoryItem(selectionManager.Selection.FirstAsXenObject, t));
            }

            if (t != TabPageBallooning)
            {
                BallooningPage.IsHidden();
            }

            if (t == TabPageConsole)
            {
                if (selectionManager.Selection.FirstIsRealVM)
                {
                    ConsolePanel.setCurrentSource((VM)selectionManager.Selection.First);
                    UnpauseVNC(e != null && sender == TheTabControl);
                }
                else if (selectionManager.Selection.FirstIsHost)
                {
                    ConsolePanel.setCurrentSource((Host)selectionManager.Selection.First);
                    UnpauseVNC(e != null && sender == TheTabControl);
                }
            }
            else
            {
                ConsolePanel.PauseAllViews();

                if (t == TabPageGeneral)
                {
                    GeneralPage.XenObject = selectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageBallooning)
                {
                    BallooningPage.XenObject = selectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageSR)
                {
                    StorageLinkRepository slr = selectionManager.Selection.First as StorageLinkRepository;
                    SrStoragePage.SR = slr == null ? selectionManager.Selection.First as SR : slr.SR(ConnectionsManager.XenConnectionsCopy);
                }
                else if (t == TabPageNetwork)
                {
                    NetworkPage.XenObject = selectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageHistory)
                {
                    // Unmark node if user has now seen error in log tab
                    if (selectionManager.Selection.FirstAsXenObject != null)
                    {
                        selectionManager.Selection.FirstAsXenObject.InError = false;
                    }
                    RequestRefreshTreeView();
                }
                else if (t == TabPageNICs)
                {
                    NICPage.Host = selectionManager.Selection.First as Host;
                }
                else if (t == TabPageStorage)
                {
                    VMStoragePage.VM = selectionManager.Selection.First as VM;
                }
                else if (t == TabPagePeformance)
                {
                    PerformancePage.XenObject = selectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageSearch && !SearchMode)
                {
                    if (selectionManager.Selection.First is GroupingTag)
                    {
                        GroupingTag gt = (GroupingTag)selectionManager.Selection.First;
                        SearchPage.Search = Search.SearchForGroup(gt.Grouping, gt.Parent, gt.Group);
                    }
                    else
                    {
                        SearchPage.XenObject = selectionManager.Selection.Count > 1 ? null : selectionManager.Selection.FirstAsXenObject;
                    }
                }
                else if (t == TabPageHA)
                {
                    HAPage.XenObject = selectionManager.Selection.FirstAsXenObject;
                }
                else if (t == TabPageWLB)
                {
                    WlbPage.Pool = selectionManager.Selection.First as Pool;
                }
                else if (t == TabPageSnapshots)
                {
                    snapshotPage.VM = selectionManager.Selection.First as VM;
                }
                else if (t == TabPagePhysicalStorage)
                {
                    PhysicalStoragePage.SetSelectionBroadcaster(selectionManager, commandInterface);
                    PhysicalStoragePage.Host = selectionManager.Selection.First as Host;
                    PhysicalStoragePage.Connection = selectionManager.Selection.GetConnectionOfFirstItem();
                }
                else if (t == TabPageAD)
                {
                    AdPage.XenObject = selectionManager.Selection.FirstAsXenObject;
                }
            }

            if (t == TabPagePeformance)
            {
                PerformancePage.ResumeGraph();
            }
            else
            {
                PerformancePage.PauseGraph();
            }

            if (t == TabPageSearch)
            {
                SearchPage.PanelShown();
            }
            else
            {
                SearchPage.PanelHidden();
            }

            if (t != null)
            {
                SetLastSelectedPage(selectionManager.Selection.First, t);
            }
        }

        private void UnpauseVNC(bool focus)
        {
            ConsolePanel.UnpauseActiveView();
            if (focus)
            {
                ConsolePanel.FocusActiveView();
                ConsolePanel.SwitchIfRequired();
            }
        }

        /// <summary>
        /// The tabs that may be visible in the main GUI window. Used in SwitchToTab().
        /// </summary>
        public enum Tab
        {
            Overview, Home, Grouping, Settings, Storage, Network, Console, Performance, History, NICs, SR
        }

        public void SwitchToTab(Tab tab)
        {
            switch (tab)
            {
                case Tab.Overview:
                    TheTabControl.SelectedTab = TabPageSearch;
                    break;
                case Tab.Home:
                    TheTabControl.SelectedTab = TabPageHome;
                    break;
                case Tab.Settings:
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
                case Tab.Performance:
                    TheTabControl.SelectedTab = TabPagePeformance;
                    break;
                case Tab.History:
                    TheTabControl.SelectedTab = TabPageHistory;
                    break;
                case Tab.NICs:
                    TheTabControl.SelectedTab = TabPageNICs;
                    break;
                case Tab.SR:
                    TheTabControl.SelectedTab = TabPageSR;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #region "Window" main menu item

        private void windowToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            windowToolStripMenuItem.DropDown.Items.Clear();

            topLevelMenu_DropDownOpening(sender, e);

            foreach (Form form in GetAuxWindows())
            {
                ToolStripMenuItem item = NewToolStripMenuItem(form.Text.EscapeAmpersands());
                item.Tag = form;
                windowToolStripMenuItem.DropDown.Items.Add(item);
            }
        }

        private List<Form> GetAuxWindows()
        {
            List<Form> result = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                if (form != this && form.Text != "" && !(form is ConnectingToServerDialog))
                {
                    result.Add(form);
                }
            }
            return result;
        }

        private void windowToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag is Form)
            {
                HelpersGUI.BringFormToFront((Form)e.ClickedItem.Tag);
            }
        }

        #endregion

        private void templatesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            templatesToolStripMenuItem1.Checked = !templatesToolStripMenuItem1.Checked;
            XenAdmin.Properties.Settings.Default.DefaultTemplatesVisible = templatesToolStripMenuItem1.Checked;
            Settings.TrySaveSettings();
            RequestRefreshTreeView();
        }

        private void customTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customTemplatesToolStripMenuItem.Checked = !customTemplatesToolStripMenuItem.Checked;
            XenAdmin.Properties.Settings.Default.UserTemplatesVisible = customTemplatesToolStripMenuItem.Checked;
            Settings.TrySaveSettings();
            RequestRefreshTreeView();
        }

        private void localStorageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            localStorageToolStripMenuItem.Checked = !localStorageToolStripMenuItem.Checked;
            XenAdmin.Properties.Settings.Default.LocalSRsVisible = localStorageToolStripMenuItem.Checked;
            Settings.TrySaveSettings();
            RequestRefreshTreeView();
        }

        private const Single SCROLL_REGION = 20;
        private VirtualTreeNode _highlightedDragTarget;

        private void treeView_ItemDrag(object sender, VirtualTreeViewItemDragEventArgs e)
        {
            foreach (VirtualTreeNode node in e.Nodes)
            {
                if (node == null || node.TreeView == null)
                {
                    return;
                }
            }

            // select the node if it isn't already selected
            if (e.Nodes.Count == 1 && treeView.SelectedNode != e.Nodes[0])
            {
                treeView.SelectedNode = e.Nodes[0];
            }

            if (CanDrag())
            {
                DoDragDrop(new List<VirtualTreeNode>(e.Nodes).ToArray(), DragDropEffects.Move);
            }
        }

        private bool CanDrag()
        {
            if ((TreeSearchBoxMode != XenAdmin.Controls.TreeSearchBox.Mode.Objects
                && TreeSearchBoxMode != XenAdmin.Controls.TreeSearchBox.Mode.Organization))
            {
                return selectionManager.Selection.AllItemsAre<Host>() || selectionManager.Selection.AllItemsAre<VM>(vm => !vm.is_a_template);
            }
            foreach (SelectedItem item in selectionManager.Selection)
            {
                if (!IsSelectableXenModelObject(item.XenObject) || item.Connection == null || !item.Connection.IsConnected)
                {
                    return false;
                }
            }
            return true;
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            VirtualTreeNode targetNode = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));
            foreach (DragDropCommand cmd in GetDragDropCommands(targetNode, e.Data))
            {
                if (cmd.CanExecute())
                {
                    e.Effect = DragDropEffects.Move;
                    return;
                }
            }
        }

        private void treeView_DragLeave(object sender, EventArgs e)
        {
            ClearHighlightedDragTarget();
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            // CA-11457: When dragging in resource tree, view doesn't scroll
            // http://www.fmsinc.com/freE/NewTips/NET/NETtip21.asp

            Point pt = treeView.PointToClient(Cursor.Position);
            VirtualTreeNode targetNode = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));

            if ((pt.Y + SCROLL_REGION) > treeView.Height)
            {
                Win32.SendMessage(treeView.Handle, Win32.WM_VSCROLL, new IntPtr(1), IntPtr.Zero);
            }
            else if (pt.Y < SCROLL_REGION)
            {
                Win32.SendMessage(treeView.Handle, Win32.WM_VSCROLL, IntPtr.Zero, IntPtr.Zero);
            }

            VirtualTreeNode targetToHighlight = null;
            string statusBarText = null;

            foreach (DragDropCommand cmd in GetDragDropCommands(targetNode, e.Data))
            {
                if (cmd.CanExecute())
                {
                    targetToHighlight = cmd.HighlightNode;
                }
                if (cmd.StatusBarText != null)
                {
                    statusBarText = cmd.StatusBarText;
                }
            }

            if (targetToHighlight != null)
            {
                if (_highlightedDragTarget != targetToHighlight)
                {
                    ClearHighlightedDragTarget();
                    treeBuilder.HighlightedDragTarget = targetToHighlight.Tag;
                    _highlightedDragTarget = targetToHighlight;
                    _highlightedDragTarget.BackColor = SystemColors.Highlight;
                    _highlightedDragTarget.ForeColor = SystemColors.HighlightText;
                }
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                ClearHighlightedDragTarget();
                e.Effect = DragDropEffects.None;
            }
        }

        private void ClearHighlightedDragTarget()
        {
            if (_highlightedDragTarget != null)
            {
                _highlightedDragTarget.BackColor = treeView.BackColor;
                _highlightedDragTarget.ForeColor = treeView.ForeColor;
                _highlightedDragTarget = null;
                treeBuilder.HighlightedDragTarget = null;
            }
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            ClearHighlightedDragTarget();

            VirtualTreeNode targetNode = treeView.GetNodeAt(treeView.PointToClient(new Point(e.X, e.Y)));

            foreach (DragDropCommand cmd in GetDragDropCommands(targetNode, e.Data))
            {
                if (cmd.CanExecute())
                {
                    cmd.Execute();
                    return;
                }
            }
        }

        private List<DragDropCommand> GetDragDropCommands(VirtualTreeNode targetNode, IDataObject dragData)
        {
            List<DragDropCommand> commands = new List<DragDropCommand>();
            commands.Add(new DragDropAddHostToPoolCommand(commandInterface, targetNode, dragData));
            commands.Add(new DragDropMigrateVMCommand(commandInterface, targetNode, dragData));
            commands.Add(new DragDropRemoveHostFromPoolCommand(commandInterface, targetNode, dragData));
            commands.Add(new DragDropTagCommand(commandInterface, targetNode, dragData));
            commands.Add(new DragDropIntoFolderCommand(commandInterface, targetNode, dragData));
            return commands;
        }

        private void treeView_NodeMouseDoubleClick(object sender, VirtualTreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IXenConnection conn = GetXenConnection(e.Node);
                VM vm = GetVM(e.Node);
                Host host = GetHost(e.Node);

                if (conn != null && !conn.IsConnected)
                {
                    new ReconnectHostCommand(commandInterface, conn).Execute();
                }
                else if (vm != null)
                {
                    if (vm.is_a_template)
                    {
                        Command cmd = new NewVMCommand(commandInterface, selectionManager.Selection);
                        if (cmd.CanExecute())
                        {
                            treeView.SelectedNode = e.Node;
                            cmd.Execute();
                        }
                    }
                    else if (vm.power_state == vm_power_state.Halted && vm.allowed_operations.Contains(vm_operations.start))
                    {
                        Command cmd = new StartVMCommand(commandInterface, selectionManager.Selection);
                        if (cmd.CanExecute())
                        {
                            treeView.SelectedNode = e.Node;
                            cmd.Execute();
                        }
                    }
                    else if (vm.power_state == vm_power_state.Suspended && vm.allowed_operations.Contains(vm_operations.resume))
                    {
                        Command cmd = new ResumeVMCommand(commandInterface, selectionManager.Selection);
                        if (cmd.CanExecute())
                        {
                            treeView.SelectedNode = e.Node;
                            cmd.Execute();
                        }
                    }
                }
                else
                {
                    Command cmd = new PowerOnHostCommand(commandInterface, host);

                    if (cmd.CanExecute())
                    {
                        treeView.SelectedNode = e.Node;
                        cmd.Execute();
                    }
                }
            }
        }

        internal void treeView_EditSelectedNode()
        {
            SuspendRefreshTreeView();
            SuspendUpdateToolbars();
            treeView.LabelEdit = true;
            treeView.SelectedNode.BeginEdit();
        }

        private void treeView_AfterLabelEdit(object sender, VirtualNodeLabelEditEventArgs e)
        {
            VirtualTreeNode node = e.Node;
            treeView.LabelEdit = false;
            Folder folder = e.Node.Tag as Folder;
            GroupingTag groupingTag = e.Node.Tag as GroupingTag;
            Command command = null;
            object newTag = null;

            EventHandler<RenameCompletedEventArgs> completed = delegate(object s, RenameCompletedEventArgs ee)
            {
                Program.Invoke(this, delegate
                {
                    ResumeUpdateToolbars();
                    ResumeRefreshTreeView();

                    if (ee.Success)
                    {
                        // the new tag is updated on the node here. This ensures that the node stays selected 
                        // when the treeview is refreshed. If you don't set the tag like this, the treeview refresh code notices 
                        // that the tags are different and selects the parent node instead of this node.

                        // if the command fails for some reason, the refresh code will correctly revert the tag back to the original.
                        node.Tag = newTag;
                        RefreshTreeView();

                        // since the selected node doesn't actually change, then a selectionsChanged message doesn't get fired
                        // and the selection doesn't get updated to be the new tag/folder. Do it manually here instead.
                        treeView_SelectionsChanged(treeView, EventArgs.Empty);
                    }
                });
            };

            if (!string.IsNullOrEmpty(e.Label))
            {
                if (folder != null)
                {
                    RenameFolderCommand cmd = new RenameFolderCommand(commandInterface, folder, e.Label);
                    command = cmd;
                    cmd.Completed += completed;
                    newTag = new Folder(null, e.Label);
                }
                else if (groupingTag != null)
                {
                    RenameTagCommand cmd = new RenameTagCommand(commandInterface, groupingTag.Group.ToString(), e.Label);
                    command = cmd;
                    cmd.Completed += completed;
                    newTag = new GroupingTag(groupingTag.Grouping, groupingTag.Parent, e.Label);
                }
            }

            if (command != null && command.CanExecute())
            {
                command.Execute();
            }
            else
            {
                ResumeUpdateToolbars();
                ResumeRefreshTreeView();
                e.CancelEdit = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            bool currentTasks = false;
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (!a.IsCompleted)
                {
                    currentTasks = true;
                    break;
                }
            }
            if (currentTasks)
            {
                e.Cancel = true;
                DialogResult result = Program.RunInAutomatedTestMode ? DialogResult.OK :
                    new Dialogs.WarningDialogs.CloseXenCenterWarningDialog().ShowDialog(this);
                if (result == DialogResult.OK)
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

                    // Disconnect the named pipe
                    Program.DisconnectPipe();
                    foreach (ActionBase a in ConnectionsManager.History)
                    {
                        if(!Program.RunInAutomatedTestMode)
                        {
                            if (a is AsyncAction)
                            {
                                AsyncAction aa = (AsyncAction) a;
                                aa.PrepareForLogReloadAfterRestart();
                            }

                            if (!a.IsCompleted && a.CanCancel && !a.SafeToExit)
                                a.Cancel();
                        }
                        else
                        {
                            if (!a.IsCompleted && a.CanCancel)
                                a.Cancel();
                        }
                    }
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(CloseWhenActionsCanceled));
                }
                return;
            }

            // Disconnect the named pipe
            Program.DisconnectPipe();

            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.WindowLocation = this.Location;
            try
            {
                Settings.SaveServerList();
                Properties.Settings.Default.Save();
            }
            catch (ConfigurationErrorsException ex)
            {
                new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.MESSAGEBOX_SAVE_CORRUPTED, Settings.GetUserConfigPath()),
                       Messages.MESSAGEBOX_SAVE_CORRUPTED_TITLE)).ShowDialog(this);
                log.Error("Couldn't save settings");
                log.Error(ex, ex);
            }
            base.OnClosing(e);
        }

        private void sendCtrlAltDelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConsolePanel.SendCAD();
        }

        /// <summary>
        /// Closes all per-Connection and per-VM wizards for the given connection.
        /// </summary>
        /// <param name="connection"></param>
        public void closeActiveWizards(IXenConnection connection)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                // Close and remove any active wizards for any VMs
                foreach (VM vm in connection.Cache.VMs)
                {
                    closeActiveWizards(vm);
                }
                closeActivePoolWizards(connection);
            });
        }

        /// <summary>
        /// Closes all per-Connection wizards.
        /// </summary>
        /// <param name="connection"></param>
        private void closeActivePoolWizards(IXenConnection connection)
        {
            IList<Form> wizards;
            if (activePoolWizards.TryGetValue(connection, out wizards))
            {
                foreach (var wizard in wizards)
                {
                    if (!wizard.IsDisposed)
                    {
                        wizard.Close();
                    }
                }

                activePoolWizards.Remove(connection);
            }
        }

        /// <summary>
        /// Closes all per-XenObject wizards.
        /// </summary>
        /// <param name="obj"></param>
        public void closeActiveWizards(IXenObject obj)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                Form wizard;
                if (activeXenModelObjectWizards.TryGetValue(obj, out wizard))
                {
                    if (!wizard.IsDisposed)
                    {
                        wizard.Close();
                    }
                    activeXenModelObjectWizards.Remove(obj);
                }
            });
        }

        /// <summary>
        /// Show the given wizard, and impose a one-wizard-per-XenObject limit.
        /// </summary>
        /// <param name="obj">The relevant VM</param>
        /// <param name="wizard">The new wizard to show</param>
        public void ShowPerXenModelObjectWizard(IXenObject obj, Form wizard)
        {
            closeActiveWizards(obj);
            activeXenModelObjectWizards.Add(obj, wizard);
            wizard.Show(this);
        }

        /// <summary>
        /// Show the given wizard, and impose a one-wizard-per-connection limit.
        /// </summary>
        /// <param name="connection">The connection.  May be null, in which case the wizard
        /// is not addded to any dictionary.  This should happen iff this is the New Pool Wizard.</param>
        /// <param name="wizard">The new wizard to show. May not be null.</param>
        public void ShowPerConnectionWizard(IXenConnection connection, Form wizard)
        {
            if (connection != null)
            {

                if (activePoolWizards.ContainsKey(connection))
                {
                    var w = activePoolWizards[connection].Where(x => x.GetType() == wizard.GetType()).FirstOrDefault();
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
                wizard.Show(this);
            }
        }

        /// <summary>
        /// Shows a form of the specified type if it has already been created. If the form doesn't exist yet
        /// it is created first and then shown.
        /// </summary>
        /// <param name="type">The type of the form to be shown.</param>
        public void ShowForm(Type type)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == type)
                {
                    HelpersGUI.BringFormToFront(form);
                    return;
                }
            }

            Form newForm = (Form)Activator.CreateInstance(type);
            newForm.Show(this);
        }

        #region Help

        private string TabHelpID()
        {
            string modelObj = getSelectedXenModelObjectType();

            if (TheTabControl.SelectedTab == TabPageHome)
                return "TabPageHome" + modelObj;
            if (TheTabControl.SelectedTab == TabPageSearch)
                return "TabPageSearch" + modelObj;
            if (TheTabControl.SelectedTab == TabPageConsole)
                return "TabPageConsole" + modelObj;
            if (TheTabControl.SelectedTab == TabPageGeneral)
                return "TabPageSettings" + modelObj;
            if (TheTabControl.SelectedTab == TabPagePhysicalStorage || TheTabControl.SelectedTab == TabPageStorage || TheTabControl.SelectedTab == TabPageSR)
                return "TabPageStorage" + modelObj;
            if (TheTabControl.SelectedTab == TabPageNetwork)
                return "TabPageNetwork" + modelObj;
            if (TheTabControl.SelectedTab == TabPageNICs)
                return "TabPageNICs" + modelObj;
            if (TheTabControl.SelectedTab == TabPageWLB)
                return "TabPageWLB" + modelObj;
            if (TheTabControl.SelectedTab == TabPagePeformance)
                return "TabPagePerformance" + modelObj;
            if (TheTabControl.SelectedTab == TabPageHistory)
                return "TabPageHistory" + modelObj;
            if (TheTabControl.SelectedTab == TabPageHA)
                return "TabPageHA" + modelObj;
            if (TheTabControl.SelectedTab == TabPageHAUpsell)
                return "TabPageHAUpsell" + modelObj;
            if (TheTabControl.SelectedTab == TabPageSnapshots)
                return "TabPageSnapshots" + modelObj;
            if (TheTabControl.SelectedTab == TabPageBallooning)
                return "TabPageBallooning" + modelObj;
            if (TheTabControl.SelectedTab == TabPageAD)
                return "TabPageAD" + modelObj;
            if (TheTabControl.SelectedTab == TabPageBallooningUpsell)
                return "TabPageBallooningUpsell";
            if (TheTabControl.SelectedTab == TabPageWLBUpsell)
                return "TabPageWLBUpsell";
            return "TabPageUnknown";
        }

        private string getSelectedXenModelObjectType()
        {
            // for now, since there are few topics which depend on the selected object we shall just check the special cases
            // when more topic are added we can just return the ModelObjectName

            if (TheTabControl.SelectedTab == TabPageGeneral && selectionManager.Selection.First is VM)
            {
                return "VM";
            }

            if (TheTabControl.SelectedTab == TabPagePhysicalStorage || TheTabControl.SelectedTab == TabPageStorage || TheTabControl.SelectedTab == TabPageSR)
            {
                if (selectionManager.Selection.FirstIsPool)
                    return "Pool";
                if (selectionManager.Selection.FirstIsHost)
                    return "Server";
                if (selectionManager.Selection.FirstIsVM)
                    return "VM";
                if (selectionManager.Selection.FirstIsSR)
                    return "Storage";
            }

            if (TheTabControl.SelectedTab == TabPageNetwork)
            {
                if (selectionManager.Selection.FirstIsHost)
                    return "Server";
                if (selectionManager.Selection.FirstIsVM)
                    return "VM";
            }

            return "";
        }

        public void ShowHelpTOC()
        {
            ShowHelpTopic(null);
        }

        /// <summary>
        /// The never-shown Form that is the parent used in ShowHelp() calls.
        /// </summary>
        private static Form helpForm;

        public void ShowHelpTopic(string topicID)
        {
            if (helpForm == null)
            {
                helpForm = new Form();
                helpForm.CreateControl();
            }

            // Abandon all hope, ye who enter here: if you're ever tempted to directly invoke hh.exe, see first:
            // JAXC-43: Online help doesn't work if install XenCenter into the path that contains special characters.
            // hh.exe can't seem to cope with certain multi-byte characters in the path to the chm.
            // System.Windows.Forms.Help.ShowHelp() can cope with the special characters in the path, but has the
            // irritating behaviour that the launched help is always on top of the app window (CA-8863).
            // So we show the help 'on top' of an invisible dummy Form.

            string chm = Path.Combine(Program.AssemblyDir, InvisibleMessages.MAINWINDOW_HELP_PATH);
            if (topicID == null)
            {
                // Show TOC
                System.Windows.Forms.Help.ShowHelp(helpForm, chm, HelpNavigator.TableOfContents);
            }
            else
            {
                System.Windows.Forms.Help.ShowHelp(helpForm, chm, HelpNavigator.TopicId, topicID);
            }
        }

        public void MainWindow_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            // CA-28064. MessageBox hack to kill the hlpevent it passes to MainWindows.
            if (Program.MainWindow.ContainsFocus && _menuShortcuts)
                LaunchHelp();
        }

        private void helpTopicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelpTOC();
        }

        private void helpContextMenuItem_Click(object sender, EventArgs e)
        {
            LaunchHelp();
        }

        private void LaunchHelp()
        {
            if (TheTabControl.SelectedTab.Tag is TabPageFeature && ((TabPageFeature)TheTabControl.SelectedTab.Tag).HasHelp)
                ((TabPageFeature)TheTabControl.SelectedTab.Tag).LaunchHelp();
            else
                Help.HelpManager.Launch(TabHelpID());
        }

        public bool HasHelp()
        {
            return Help.HelpManager.HasHelpFor(TabHelpID());
        }

        private void debugHelpToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            //XenAdmin.Properties.Settings.Default.DebugHelp = debugHelpToolStripMenuItem.Checked;
            XenAdmin.Properties.Settings.Default.DebugHelp = false;
            Settings.TrySaveSettings();
        }

        private void viewApplicationLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ViewLogFiles();
        }

        #endregion

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(typeof (ManageUpdatesDialog));
        }

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
                        foreach (VirtualTreeNode node in treeView.AllNodes)
                        {
                            if (tagMatch(node.Tag))
                            {
                                if (selectNode)
                                {
                                    treeView.SelectedNode = node;
                                }
                                if (expandNode)
                                {
                                    node.Expand();
                                }
                                if (ensureNodeVisible)
                                {
                                    node.EnsureVisible();
                                }
                                success = true;
                                return;
                            }
                        }
                        success = false;
                    });
                    Thread.Sleep(500);
                }
            });
        }

        /// <summary>
        /// Selects the specified object in the treeview.
        /// </summary>
        /// <param name="o">The object to be selected.</param>
        /// <returns>A value indicating whether selection was successful.</returns>
        public bool SelectObject(IXenObject o)
        {
            return SelectObject(o, false);
        }

        /// <summary>
        /// Selects the specified object in the treeview.
        /// </summary>
        /// <param name="o">The object to be selected.</param>
        /// <param name="expand">A value specifying whether the node should be expanded when it's found. 
        /// If false, the node is left in the state it's found in.</param>
        /// <returns>A value indicating whether selection was successful.</returns>
        public bool SelectObject(IXenObject o, bool expand)
        {
            bool cancelled = false;
            if (treeView.Nodes.Count == 0)
                return false;

            bool success = SelectObject(o, treeView.Nodes[0], expand, ref cancelled);

            if (!success && !cancelled && searchTextBox.Text.Length > 0)
            {
                // if the node could not be found and the node *is* selectable then it means that
                // node isn't visible with the current search. So clear the search and try and
                // select the object again.

                // clear search.
                searchTextBox.Reset();

                // update the treeview
                RefreshTreeView();

                // and try again.
                return SelectObject(o, treeView.Nodes[0], expand, ref cancelled);
            }
            return success;
        }

        /// <summary>
        /// Selects the specified object in the tree.
        /// </summary>
        /// <param name="o">The object to be selected.</param>
        /// <param name="node">The node at which to start.</param>
        /// <param name="expand">Expand the node when it's found.</param>
        /// <param name="cancelled">if set to <c>true</c> then the node for the specified object was not allowed to be selected.</param>
        /// <returns>A value indicating whether selection was successful.</returns>
        private bool SelectObject(IXenObject o, VirtualTreeNode node, bool expand, ref bool cancelled)
        {
            IXenObject candidate = node.Tag as IXenObject;

            if (o == null || (candidate != null && candidate.opaque_ref == o.opaque_ref))
            {
                if (!CanSelectNode(node))
                {
                    cancelled = true;
                    return false;
                }

                treeView.SelectedNode = node;

                if (expand)
                {
                    node.Expand();
                }

                return true;
            }

            foreach (VirtualTreeNode child in node.Nodes)
            {
                if (SelectObject(o, child, expand, ref cancelled))
                    return true;
            }

            return false;
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
                System.Threading.Thread.Sleep(500);
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
            OptionsDialog dialog = new OptionsDialog(pluginManager);
            dialog.ShowDialog(this);
        }

        internal void action_Completed(ActionBase sender)
        {
            if (Program.Exiting)
            {
                return;
            }

            RequestRefreshTreeView();
            
            //Update toolbars, since if an action has just completed, various
            //buttons may need to be re-enabled. Applies to:
            // HostAction
            // EnableHAAction
            // DisableHAAction
            Program.Invoke(this, UpdateToolbars);
        }

        private void xenBugToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BugToolWizard == null || BugToolWizard.IsDisposed)
            {
                if (selectionManager.Selection != null && selectionManager.Selection.AllItemsAre<IXenObject>(x => x is Host || x is Pool))
                    BugToolWizard = new BugToolWizard(selectionManager.Selection.AsXenObjects<IXenObject>());
                else
                    BugToolWizard = new BugToolWizard();
                BugToolWizard.Show();
            }
            else
            {
                HelpersGUI.BringFormToFront(BugToolWizard);
            }
        }

        private void ShowHiddenObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHiddenObjectsToolStripMenuItem.Checked = !ShowHiddenObjectsToolStripMenuItem.Checked;
            XenAdmin.Properties.Settings.Default.ShowHiddenVMs = ShowHiddenObjectsToolStripMenuItem.Checked;
            Settings.TrySaveSettings();
            RequestRefreshTreeView();
        }

        internal void OpenGlobalImportWizard(string param)
        {
            HelpersGUI.BringFormToFront(this);
            Host hostAncestor = selectionManager.Selection.Count == 1 ? selectionManager.Selection[0].HostAncestor : null;
            new ImportWizard(selectionManager.Selection.GetConnectionOfFirstItem(), hostAncestor, param, false).Show();
        }

        internal void InstallUpdate(string path)
        {
            var wizard = new PatchingWizard();
            wizard.Show(this);
            wizard.NextStep();
            wizard.AddFile(path);
        }

        #region XenSearch

        public TreeSearchBox.Mode TreeSearchBoxMode
        { get { return TreeSearchBox.currentMode; } }

        // SearchMode doesn't just mean we are looking at the Search tab.
        // It's set when we import a search from a file; or when we double-click
        // on a folder or tag name to search for it.
        private bool searchMode = false;
        public bool SearchMode
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
                UpdateToolbarsCore();
            }
        }

        private bool DoSearch(string filename)
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
                    selectionManager.Selection.FirstAsXenObject, SearchPage.Search));
        }

        /// <summary>
        /// Updates the shiny gradient bar with selected object name and icon.
        /// Also updates 'Logged in as:'.
        /// </summary>
        private void UpdateHeader()
        {
            if (SearchMode && SearchPage.Search != null)
            {
                TitleLabel.Text = HelpersGUI.GetLocalizedSearchName(SearchPage.Search);
                TitleIcon.Image = Images.GetImage16For(SearchPage.Search);
            }
            else if (!SearchMode && selectionManager.Selection.ContainsOneItemOfType<IXenObject>())
            {
                IXenObject xenObject = selectionManager.Selection[0].XenObject;
                TitleLabel.Text = GetTitleLabel(xenObject);
                TitleIcon.Image = Images.GetImage16For(xenObject);
                // When in folder view only show the logged in label if it is clear to which connection the object belongs (most likely pools and hosts)

                if (selectionManager.Selection[0].PoolAncestor == null && selectionManager.Selection[0].HostAncestor == null)
                    loggedInLabel1.Connection = null;
                else
                    loggedInLabel1.Connection = xenObject.Connection;
            }
            else
            {
                TitleLabel.Text = Messages.XENCENTER;
                TitleIcon.Image = Properties.Resources.Logo;
                loggedInLabel1.Connection = null;
            }
        }

        string GetTitleLabel(IXenObject xenObject)
        {
            string name = Helpers.GetName(xenObject);
            VM vm = xenObject as VM;
            if (vm != null && vm.is_a_real_vm)
            {
                Host server = vm.Home();
                if (server != null)
                    return string.Format(Messages.VM_ON_SERVER, name, server);
                Pool pool = Helpers.GetPool(vm.Connection);
                if (pool != null)
                    return string.Format(Messages.VM_IN_POOL, name, pool);
            }
            return name;
        }
        
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            RequestRefreshTreeView();
        }

        void TreeSearchBox_SearchChanged(object sender, EventArgs e)
        {
            searchTextBox.Reset();
            RequestRefreshTreeView();
            FocusTreeView();
            SelectObject(null);            
        }

        #endregion

        private void AlertsToolbarButton_Click(object sender, EventArgs e)
        {
            ShowForm(typeof (AlertSummaryDialog));
        }

        private void UpdateAlertToolbarButton()
        {
            int validAlertCount = Alert.NonDismissingAlertCount;

            if (validAlertCount == 0 && AlertsToolbarButton.Text != Messages.SYSTEM_ALERTS_EMPTY)
            {
                AlertsToolbarButtonSmall.Font = AlertsToolbarButton.Font = Program.DefaultFont;
                AlertsToolbarButtonSmall.ForeColor = AlertsToolbarButton.ForeColor = NoAlertsColor;
                AlertsToolbarButtonSmall.Image = AlertsToolbarButton.Image = XenAdmin.Properties.Resources._000_Tick_h32bit_16;
                AlertsToolbarButtonSmall.Text = AlertsToolbarButton.Text = Messages.SYSTEM_ALERTS_EMPTY;
            }
            else if (validAlertCount > 0)
            {
                if (AlertsToolbarButton.Text == Messages.SYSTEM_ALERTS_EMPTY)
                {
                    AlertsToolbarButtonSmall.Font = AlertsToolbarButton.Font = Program.DefaultFontBoldUnderline;
                    AlertsToolbarButtonSmall.ForeColor = AlertsToolbarButton.ForeColor = HasAlertsColor;
                    AlertsToolbarButtonSmall.Image = AlertsToolbarButton.Image = XenAdmin.Properties.Resources._000_XenCenterAlerts_h32bit_24;
                }
                AlertsToolbarButtonSmall.Text = AlertsToolbarButton.Text = string.Format(Messages.SYSTEM_ALERTS_TOTAL, validAlertCount);
            }

        }

        void XenCenterAlerts_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            //Program.AssertOnEventThread();
            Program.BeginInvoke(Program.MainWindow, UpdateAlertToolbarButton);
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
            licenseManagerLauncher.LaunchIfRequired(false, ConnectionsManager.XenConnections, selectionManager.Selection);
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

        private void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Apps:
                    if (treeView.SelectedNode != null)
                    {
                        treeView.SelectedNode.EnsureVisible();
                        TreeView_NodeMouseClick(treeView, new VirtualTreeNodeMouseClickEventArgs(treeView.SelectedNode,
                            MouseButtons.Right, 1,
                            treeView.SelectedNode.Bounds.X,
                            treeView.SelectedNode.Bounds.Y + treeView.SelectedNode.Bounds.Height));
                    }
                    break;

                case Keys.F2:
                    PropertiesCommand cmd = new PropertiesCommand(commandInterface, selectionManager.Selection);
                    if (cmd.CanExecute())
                    {
                        cmd.Execute();
                    }
                    break;
            }
        }

        private void ShowToolbarMenuItem_Click(object sender, EventArgs e)
        {
            ToolbarsEnabled = !ToolbarsEnabled;
            Properties.Settings.Default.ToolbarsEnabled = ToolbarsEnabled;
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
        /// Equivalent to MainController.Confirm(conn, this, msg, args).
        /// </summary>
        public bool Confirm(IXenConnection conn, string title, string msg, params object[] args)
        {
            return Confirm(conn, this, title, msg, args);
        }

        /// <summary>
        /// Show a MessageBox asking to confirm an operation. The MessageBox will be parented to the given Control.
        /// Displays default "Yes" and "No" buttons ("Yes" button is selected by default).
        /// The args given will be ellipsised to Helpers.DEFAULT_NAME_TRIM_LENGTH, if they are strings.
        /// If in automated test mode, then always returns true.
        /// If the user refuses the operation, then returns false.
        /// If the given connection has disconnected in the time it takes the user to confirm,
        /// then shows an information MessageBox, and returns false.
        /// Otherwise, the user has agreed and the connection is still alive, so
        /// sets MainWindow.AllowHistorySwitch to true and returns true.
        /// </summary>
        public static bool Confirm(IXenConnection conn, Control parent, string title, string msg, params object[] args)
        {
            return Confirm(conn, parent, title, null, null, null, msg, args);
        }

        /// <summary>
        /// Show a MessageBox asking to confirm an operation. The MessageBox will be parented to the given Control.
        /// "Yes" and "No" buttons can be customized.
        /// The args given will be ellipsised to Helpers.DEFAULT_NAME_TRIM_LENGTH, if they are strings.
        /// If in automated test mode, then always returns true.
        /// If the user refuses the operation, then returns false.
        /// If the given connection has disconnected in the time it takes the user to confirm,
        /// then shows an information MessageBox, and returns false.
        /// Otherwise, the user has agreed and the connection is still alive, so
        /// sets MainWindow.AllowHistorySwitch to true and returns true.
        /// </summary>
        public static bool Confirm(IXenConnection conn, Control parent, string title,
            string helpName, ThreeButtonDialog.TBDButton buttonYes, ThreeButtonDialog.TBDButton buttonNo,
            string msg, params object[] args)
        {
            if (Program.RunInAutomatedTestMode)
                return true;

            Trim(args);

            var buttons = new[]
                {
                    buttonYes ?? ThreeButtonDialog.ButtonYes,
                    buttonNo ?? ThreeButtonDialog.ButtonNo
                };

            var details = new ThreeButtonDialog.Details(SystemIcons.Exclamation,
                args.Length == 0 ? msg : string.Format(msg, args), title);

            var dialog = String.IsNullOrEmpty(helpName)
                             ? new ThreeButtonDialog(details, buttons)
                             : new ThreeButtonDialog(details, helpName, buttons);

            if (dialog.ShowDialog(parent ?? Program.MainWindow) != DialogResult.Yes)
                return false;


            if (conn != null && !conn.IsConnected)
            {
                ShowDisconnectedMessage(parent);
                return false;
            }

            Program.MainWindow.AllowHistorySwitch = true;

            return true;
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
            new ThreeButtonDialog(
               new ThreeButtonDialog.Details(
                   SystemIcons.Warning,
                   Messages.DISCONNECTED_BEFORE_ACTION_STARTED,
                   Messages.XENCENTER)).ShowDialog(parent);
        }

        private static void Trim(object[] args)
        {
            int n = args.Length;
            for (int i = 0; i < n; i++)
            {
                if (args[i] is string)
                    args[i] = ((string)args[i]).Ellipsise(Helpers.DEFAULT_NAME_TRIM_LENGTH);
            }
        }

        /// <summary>
        /// Create a new ToolStripMenuItem, setting the appropriate default font, and configuring it with the given text, icon, and click handler.
        /// </summary>
        /// <param name="text">The menu item text.  Don't forget that this may need to have its ampersands escaped.</param>
        /// <param name="ico">May be null, in which case no icon is set.</param>
        /// <param name="clickHandler">May be null, in which case no click handler is set.</param>
        /// <returns>The new ToolStripMenuItem</returns>
        internal static ToolStripMenuItem NewToolStripMenuItem(string text, Image ico, EventHandler clickHandler)
        {
            ToolStripMenuItem m = new ToolStripMenuItem(text);
            m.Font = Program.DefaultFont;
            if (ico != null)
                m.Image = ico;
            if (clickHandler != null)
                m.Click += clickHandler;
            return m;
        }

        /// <summary>
        /// Equivalent to NewToolStripMenuItem(text, null, null).
        /// </summary>
        internal static ToolStripMenuItem NewToolStripMenuItem(string text)
        {
            return NewToolStripMenuItem(text, null, null);
        }

        /// <summary>
        /// Equivalent to NewToolStripMenuItem(text, null, clickHandler).
        /// </summary>
        internal static ToolStripMenuItem NewToolStripMenuItem(string text, EventHandler clickHandler)
        {
            return NewToolStripMenuItem(text, null, clickHandler);
        }

        internal void FocusTreeView()
        {
            treeView.Focus();
        }

        #region ISynchronizeInvoke Members

        // this explicit implementation of ISynchronizeInvoke is used to allow the model to update 
        // its API on the main program thread while being decoupled from MainWindow.

        // See StorageLinkConnection for an example of its usage.

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
                dialog.Filter = Messages.XENCENTER_CONFIG_FILTER;
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
                            RefreshTreeView();
                        }

                        log.InfoFormat("Imported server list from '{0}' successfully.", dialog.FileName);
                    }
                    catch (XmlException)
                    {
                        log.ErrorFormat("Failed to import server list from '{0}'", dialog.FileName);

                        new ThreeButtonDialog(
                      new ThreeButtonDialog.Details(SystemIcons.Error, Messages.ERRO_IMPORTING_SERVER_LIST, Messages.XENCENTER))
                      .ShowDialog(this);
                    }
                }
            }
        }

        private void exportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = Messages.XENCENTER_CONFIG_FILTER;
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

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            SetSplitterDistance();
        }

        private void SetSplitterDistance()
        {
            //CA-71697: chosen min size so the tab contents are visible
            int chosenPanel2MinSize = splitContainer1.Width/2;
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
    }
}
