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
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;

using DotNetVnc;
using XenAdmin;
using XenAdmin.ConsoleView;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin
{
    /// <summary>
    /// XenServer hosted VM console events interface
    /// </summary>
    [Guid("4CF54BB1-3A27-4fe6-9BEC-03BD404AF367")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface IVMConsoleEvents
    {
        [DispIdAttribute(0x60020000)]
        void OnDisconnectedCallbackEvent(int EventID, string DisconnectReason);
        [DispIdAttribute(0x60020001)]
        void OnResolutionChangeCallbackEvent(string NewResolution);
    }

    /// <summary>
    /// XenServer hosted VM console access interface
    /// </summary>
    [Guid("FFD87368-B188-4921-BE52-B3F75967FC89")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [ComVisible(true)]
    public interface IVMConsole
    {
        #region interface functions
        bool Connect(string server, int port, string vm_uuid, string username, string password, int width, int height, bool show_border);
        bool ConnectConsole(string consoleuri, int width, int height, bool show_border);
        bool Disconnect();
        bool CanConnect(); /* may not be necessary */
        string GetVMResolution();
        bool IsConnected();
        void SendCtrlAltDel();
        #endregion
    }

    /// <summary>
    /// Class that implements the Active-X interface and allows a COM client 
    /// to connect to the VNC console of a XenServer hosted VM.
    /// </summary>
    [Guid("D52D9588-AB6E-425b-9D8C-74FBDA46C4F8")]
    [ProgId("XenAdmin.VNCControl")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [ComSourceInterfaces(typeof(IVMConsoleEvents))]
    public partial class VNCControl : UserControl, IVMConsole
    {
        #region Properties and Events
        // consts
        private const string SESSION_INVALID = "SESSION_INVALID";
        private const string HOST_IS_SLAVE = "HOST_IS_SLAVE";
        private const string HANDLE_INVALID = "HANDLE_INVALID";
        private const int SHORT_RETRY_COUNT = 10;
        private const int SHORT_RETRY_SLEEP_TIME = 100;
        private const int RETRY_SLEEP_TIME = 5000;
        private const int VNC_PORT = 5900;

        public MethodInvoker OnDetectVNC = null;
        public String m_vncIP = null;

        private VNCGraphicsClient m_vncClient = null;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Xen API related properties
        private XenAPI.Session m_session = null;
        private char[] m_vncPassword = null;
        private XenAPI.VM m_sourceVM = null;
        private bool m_sourceIsPV = false;
        //private bool UseSource = true; /* use VNC endpoint on host as opposed to guest */

        // Event Handlers
        private Dictionary<Set<int>, MethodInvoker> extraScans = new Dictionary<Set<int>, MethodInvoker>();
        private Dictionary<Set<Keys>, MethodInvoker> extraCodes = new Dictionary<Set<Keys>, MethodInvoker>();
        private Set<int> pressedScans = new Set<int>();
        #endregion // Properties

        #region Initializers and constructors
        public VNCControl()
        {
            InitializeComponent();
            initSubControl(0, 0, true, true);
        }

        /// <summary>
        /// Creates the actual VNC client control.
        /// </summary>
        private void initSubControl(int width, int height, bool scaling, bool show_border)
        {
            Program.MainWindow = this;
            bool wasFocused = m_vncClient != null && m_vncClient.Focused;
            this.Controls.Clear();

            Size newSize;
            Size oldSize = new Size(1024, 768);
            if (width == 0 || height == 0)
                newSize = new Size(1024, 768);
            else
                newSize = new Size(width, height);

            // Kill the old client.
            if (m_vncClient != null)
            {
                oldSize = m_vncClient.DesktopSize;
                m_vncClient.Disconnect();
                m_vncClient.Dispose();
                m_vncClient = null;
            }
            // Reset
            this.AutoScroll = false;
            this.AutoScrollMinSize = new Size(0, 0);
            m_vncClient = new VNCGraphicsClient(this);
            m_vncClient.Size            = newSize;
            this.Size = newSize;
            m_vncClient.UseSource       = true;
            m_vncClient.SendScanCodes   = !this.m_sourceIsPV;
            m_vncClient.Dock            = DockStyle.Fill;
            this.m_vncClient.Scaling    = scaling; // SCVMM requires a scaled image
            m_vncClient.DisplayBorder   = show_border;
            m_vncClient.AutoScaleDimensions = newSize;
            m_vncClient.AutoScaleMode = AutoScaleMode.Inherit;

            this.m_vncClient.DesktopResized += ResolutionChangeHandler;
            //this.m_vncClient.Resize += ResizeHandler;
            //this.m_vncClient.ErrorOccurred += ErrorHandler;
            //this.m_vncClient.ConnectionSuccess += ConnectionSuccess;
            //this.m_vncClient.ErrorOccurred
            //this.m_vncClient.MouseDown += new MouseEventHandler(vncClient_MouseDown);
            //this.m_vncClient.KeyDown += new KeyEventHandler(vncClient_KeyDown);
            
            m_vncClient.KeyHandler = new ConsoleKeyHandler();
            this.m_vncClient.Unpause();

            foreach (Set<Keys> keys in extraCodes.Keys)
            {
                m_vncClient.KeyHandler.AddKeyHandler(keys, extraCodes[keys]);
            }

            foreach (Set<int> keys in extraScans.Keys)
            {
                m_vncClient.KeyHandler.AddKeyHandler(keys, extraScans[keys]);
            }
        }
        #endregion

        #region Active-X interface implementation
        // externally subscribed events
        public delegate void OnDisconnectedCallbackHandler(int EventID, string DisconnectReason);
        public event OnDisconnectedCallbackHandler OnDisconnectedCallbackEvent;
                            
        public delegate void OnResolutionChangeCallbackHandler(string NewResolution);
        public event OnResolutionChangeCallbackHandler OnResolutionChangeCallbackEvent;
                                                      
        /// <summary>
        /// Connect to a VM's console
        /// </summary>
        /// <param name="server">XenServer to connect to</param>
        /// <param name="port">Port that XenAPI is availabl on</param>
        /// <param name="vm_uuid">The UUID of the VM whose console to connect to</param>
        /// <param name="username">username for the xenapi AND the VM's console</param>
        /// <param name="password">password for the xenapi AND the VM's console</param>
        /// <param name="width">Width to initialize the VNC control to</param>
        /// <param name="height">Height to initialize the VNC control to</param>
	/// <param name="show_border">Bool to show border around the control when in focus</param>
        /// <returns>true, if it succeeds and false if it doesnt</returns>
        [ComVisible(true)]
        public bool Connect(string server, int port, string vm_uuid, string username, string password, int width, int height, bool show_border)
        {
            // reinitiailize the VNC Control
            initSubControl(width, height, true, show_border);
            m_vncClient.ErrorOccurred += ConnectionErrorHandler;

            try {
                // Create a new XenAPI session
                m_session = new Session(Session.STANDARD_TIMEOUT, server, port);

                // Authenticate with username and password passed in. 
                // The third parameter tells the server which API version we support.
                m_session.login_with_password(username, password, API_Version.LATEST);
                m_vncPassword = password.ToCharArray();

                // Find the VM in question
                XenRef<VM> vmRef = VM.get_by_uuid(m_session, vm_uuid);
                m_sourceVM = VM.get_record(m_session, vmRef);

                // Check if this VM is PV or HVM
                m_sourceIsPV = (m_sourceVM.PV_bootloader.Length != 0); /* No PV bootloader specified implies HVM */

                // Get the console that uses the RFB (VNC) protocol
                List<XenRef<XenAPI.Console>> consoleRefs = VM.get_consoles(m_session, vmRef);
                XenAPI.Console console = null;
                foreach (XenRef<XenAPI.Console> consoleRef in consoleRefs)
                {
                    console = XenAPI.Console.get_record(m_session, consoleRef);
                    if (console.protocol == console_protocol.rfb)
                        break;
                    console = null;
                }

                if (console != null)
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectToConsole), new KeyValuePair<VNCGraphicsClient, XenAPI.Console>(m_vncClient, console));
                    ConnectHostedConsole(m_vncClient, console, m_session.uuid);

                // done with this session, log it out
                m_session.logout();
            }
            catch (Exception exn)
            {
                // call the expcetion handler directly
                this.ConnectionErrorHandler(this, exn);
            }
            return m_vncClient.Connected;
        }
	/// <summary>
	/// Connect to the VNC control via a URL
	/// </summary>
	/// <returns>true if the connection worked</returns>
	[ComVisible(true)]
	public bool ConnectConsole(string consoleuri, int width, int height, bool show_border)
	{

		//reinitialise the VNC Control
		initSubControl(width, height, true, show_border);
		m_vncClient.ErrorOccurred += ConnectionErrorHandler;

		try {


			XenAPI.Console console = new XenAPI.Console();
			console.protocol = console_protocol.rfb;
			console.location = consoleuri;

			Uri uri = new Uri(consoleuri);
			char[] delims = { '&', '=' , '?' };
			string qargs = uri.Query;
			string session_id = "";
			string vm_Opref = "";
			string console_ref = "";

			string[] args = qargs.Split(delims);
			int x = -1;
			int y = -1;
			int z = -1;
			int count = 0;
			
			foreach (string s in args)
			{
				if ( String.Equals(s, "session_id", StringComparison.Ordinal) ) {
					//The session_id value must be one greater in array
					x = count + 1;
				} else if ( String.Equals(s, "ref", StringComparison.Ordinal) ) {
					//The OpaqueRef for vnc console must be one greater in array
					y = count + 1;
				} else if (String.Equals(s, "uuid", StringComparison.Ordinal) ) {
				       //The uuid was passed for the vnc console - it must be one
				       //greater in the array.
				       z = count + 1;
				}
				count++;
			}

			//Checks for incorrect parsing of the console URL
			if( x == -1 || x == count) 
				this.ConnectionErrorHandler(this, new System.ApplicationException("Error: The session ID has been incorrectly parsed."));
			else 
				session_id = args[x];			

			if (console != null){
			
			try {			
              			m_session = new Session("http://" + uri.Host , session_id);
			} 
			catch (XenAPI.Failure f) {
					if (f.ErrorDescription[0] == HOST_IS_SLAVE)
					{
						string m_address = f.ErrorDescription[1];
						m_session = new Session("http://" + m_address, session_id);
					}

			}
			
	
			if( (y == -1 && z == -1) || (y == count && z == count)) {
			        //Check for the error case where neither uuid or vm_reference have been supplied.
				this.ConnectionErrorHandler(this, new System.ApplicationException("Error: The console reference has been incorrectly parsed."));
			}
			else if( y != -1 && y != count)	{		
			     	 //The console reference has been provided.
				 console_ref = args[y];
			}

			else if( z !=-1 && z != count){
			     	 //The console uuid has been supplied instead, we must get the VM reference.
				 console_ref = XenAPI.Console.get_by_uuid(m_session, args[z]);
			}			

			vm_Opref = XenAPI.Console.get_VM(m_session, console_ref);
			m_sourceVM = VM.get_record(m_session, vm_Opref);

			// Check if this VM is PV or HVM
                	m_sourceIsPV = (m_sourceVM.PV_bootloader.Length != 0);
			ConnectHostedConsole(m_vncClient, console, session_id);

			}
		}

		catch (Exception exn) {
			//call the exception handler directly
			this.ConnectionErrorHandler(this, exn);
		}
		return m_vncClient.Connected;
	}

        /// <summary>
        /// Disconnect the VNC control from the VM's console it is presently connected to
        /// </summary>
        /// <returns>true if the disconnect worked</returns>
        [ComVisible(true)]
        public bool Disconnect()
        {
            m_vncClient.ErrorOccurred -= ConnectionErrorHandler;
            if (m_vncClient != null)
            {
                m_vncClient.Disconnect();
                return !m_vncClient.Connected;
            }
            return true;
        }

        /// <summary>
        /// Check to see if the VNCControl can be connected to ????
        /// </summary>
        /// <returns></returns>
        [ComVisible(true)]
        public bool CanConnect() /* may not be necessary */
        {
            return !m_vncClient.Connected;
        }

	/// <summary>
	/// Return the current resolution of the connected VM
	/// </summary>
	/// <returns>widthxheight in pixels</returns>
	[ComVisible(true)]
	public string GetVMResolution()
	{
	    Size DeskSize = m_vncClient.DesktopSize;
	    return DeskSize.Width + "x" + DeskSize.Height;
	}
        
        /// <summary>
        /// Check to see if the VNC Control is presently connected to a VM's console 
        /// </summary>
        /// <returns>true or false</returns>
        [ComVisible(true)]
        public bool IsConnected()
        {
            return m_vncClient.Connected;
        }

        /// <summary>
        /// Send the CTRL+ALT+DEL key sequence to the VM's console. 
        /// If its a windows VM, it brings up the login dialog.
        /// </summary>
        [ComVisible(true)]
        public void SendCtrlAltDel()
        {
            /* send ctrl alt del into the connection */
            if (m_vncClient != null)
                m_vncClient.SendCAD();
        }
        #endregion // Interface implementation

        #region private helper APIs
        private void ConnectHostedConsole(VNCGraphicsClient v, XenAPI.Console console, string session_uuid)
        {
            //Program.AssertOffEventThread();
            Uri uri = new Uri(console.location);
            Stream stream = HTTP.CONNECT(uri, null, session_uuid, 0);
            InvokeConnection(v, stream, console, m_vncPassword);
        }

        private void InvokeConnection(VNCGraphicsClient v, Stream stream, XenAPI.Console console, char[] vncPassword)
        {
            Program.Invoke(this, delegate()
            {
                // This is the last chance that we have to make sure that we've not already
                // connected this VNCGraphicsClient.  Now that we are back on the event thread,
                // we're guaranteed that no-one will beat us to the v.connect() call.  We
                // hand over responsibility for closing the stream at that point, so we have to
                // close it ourselves if the client is already connected.
                if (v.Connected || v.Terminated)
                {
                    stream.Close();
                }
                else
                {
                    v.SendScanCodes = !m_sourceIsPV;
                    v.SourceVM = m_sourceVM;
                    v.Console = console;
                    v.Focus();
                    v.connect(stream, vncPassword);
                }
            }
            );
        }

        private Stream connectGuest(string ip_address, int port)
        {
            return HTTP.ConnectStream(new Uri(String.Format("http://{0}:{1}/", ip_address, port)), null, true, 0);
        }

        private void PollVNCPort(Object Sender)
        {
            m_vncIP = null;
            String openIP = PollPort(VNC_PORT, true);

            if (openIP != null)
            {
                if (OnDetectVNC != null)
                {
                    Program.Invoke(this, OnDetectVNC);
                }
                m_vncIP = openIP;
            }
        }

        /// <summary>
        /// scan each ip address (from the guest agent) for an open port
        /// </summary>
        /// <param name="port"></param>
        private String PollPort(int port, bool vnc)
        {
            try
            {
                Log.Debug("PollPort called");
                if (m_sourceVM == null)
                    return null;

                VM vm = m_sourceVM;

                XenRef<VM_guest_metrics> guestMetricsRef = vm.guest_metrics;
                if (guestMetricsRef == null)
                    return null;

                VM_guest_metrics metrics = XenAPI.VM_guest_metrics.get_record(m_session, vm.guest_metrics);
                if (metrics == null)
                    return null;
                Dictionary<string, string> networks = metrics.networks;

                if (networks == null)
                    return null;

                List<String> ipAddresses = new List<String>();
                foreach (String key in networks.Keys)
                {
                    if (key.EndsWith("ip"))
                        ipAddresses.Add(networks[key]);
                }

                foreach (String ipAddress in ipAddresses)
                {
                    try
                    {
                        Stream s = connectGuest(ipAddress, port);
                        if (vnc)
                        {
                            //SetPendingVNCConnection(s);
                        }
                        else
                        {
                            s.Close();
                        }
                        return ipAddress;
                    }
                    catch (Exception exn)
                    {
                        Log.Debug(exn);
                    }
                }
            }
            catch (WebException)
            {
                // xapi has gone away.
            }
            catch (IOException)
            {
                // xapi has gone away.
            }
            catch (XenAPI.Failure exn)
            {
                if (exn.ErrorDescription[0] == HANDLE_INVALID)
                {
                    // HANDLE_INVALID is fine -- the guest metrics are not there yet.
                }
                else if (exn.ErrorDescription[0] == SESSION_INVALID)
                {
                    // SESSION_INVALID is fine -- these will expire from time to time.
                    // We need to invalidate the session though.
                    //lock (activeSessionLock)
                    //{
                    m_session = null;
                    //}
                }
                else
                {
                    Log.Warn("Exception while polling VM for port " + port + ".", exn);
                }
            }
            catch (Exception e)
            {
                Log.Warn("Exception while polling VM for port " + port + ".", e);
            }
            return null;
        }

#if DEBUG
        // Test API for the harness
        public Dictionary<String, String> ListConsoles(String server, int port, String username, String password)
        {
            XenAPI.Session session = new Session(Session.STANDARD_TIMEOUT, server, port);
            Dictionary<String, String> dict = new Dictionary<String, String>();

            // Authenticate with username and password. The third parameter tells the server which API version we support.
            session.login_with_password(username, password, API_Version.LATEST);
            List<XenRef<XenAPI.Console>> consoleRefs = XenAPI.Console.get_all(session);
            foreach (XenRef<XenAPI.Console> consoleRef in consoleRefs)
            {
                XenAPI.Console console = XenAPI.Console.get_record(session, consoleRef);
                XenAPI.VM vm = XenAPI.VM.get_record(session, console.VM);
                dict.Add(vm.uuid, vm.name_label);
            }

            return dict;
        }
#endif
        private void ConnectionErrorHandler(object sender, Exception exn)
        {
            Program.Invoke(this, delegate()
            {
                Log.Debug(exn, exn);
                if(this.OnDisconnectedCallbackEvent != null)
                    this.OnDisconnectedCallbackEvent(0, exn.Message);
                else
                    MessageBox.Show(exn.Message, "VNCControl Error");
            });
        }

	private void ResolutionChangeHandler(object sender, EventArgs e)
	{

	    Program.Invoke(this, delegate()
            {
                if (this.OnResolutionChangeCallbackEvent != null)
                    this.OnResolutionChangeCallbackEvent(GetVMResolution());
            });
	}

        #endregion // private helpers

        #region COM registration and unregistration helpers
        [ComRegisterFunction()]
        public static void RegisterClass ( string key )
        {
          // Strip off HKEY_CLASSES_ROOT\ from the passed key as I don't need it
          StringBuilder sb = new StringBuilder ( key ) ;
          sb.Replace(@"HKEY_CLASSES_ROOT\","") ;

          // Open the CLSID\{guid} key for write access
          RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(),true);
          // And create the 'Control' key - this allows it to show up in 

          // the ActiveX control container 
          RegistryKey ctrl = k.CreateSubKey ( "Control" ) ; 
          ctrl.Close ( ) ;
          // Next create the CodeBase entry - needed if not string named and GACced.

          RegistryKey inprocServer32 = k.OpenSubKey ( "InprocServer32" , true ) ; 
          inprocServer32.SetValue ( "CodeBase" , Assembly.GetExecutingAssembly().CodeBase ) ; 
          inprocServer32.Close ( ) ;
          // Finally close the main key
          k.Close ( ) ;
        }

        [ComUnregisterFunction()]
        public static void UnregisterClass ( string key )
        {
          StringBuilder sb = new StringBuilder ( key ) ;
          sb.Replace(@"HKEY_CLASSES_ROOT\","") ;

          // Open HKCR\CLSID\{guid} for write access
          RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(),true);

          // Delete the 'Control' key, but don't throw an exception if it does not exist
          k.DeleteSubKey ( "Control" , false ) ;

          // Next open up InprocServer32
          RegistryKey inprocServer32 = k.OpenSubKey ( "InprocServer32" , true ) ;

          // And delete the CodeBase key, again not throwing if missing 
          k.DeleteSubKey ( "CodeBase" , false ) ;

          // Finally close the main key 
          k.Close ( ) ;
        }
        #endregion
    }
}
