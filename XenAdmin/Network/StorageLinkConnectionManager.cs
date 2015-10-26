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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using System.Net;


namespace XenAdmin.Network.StorageLink
{
    public class StorageLinkConnectionManager : ReadOnlyChangeableList<StorageLinkConnection>, IDisposable
    {
        private readonly Func<ISynchronizeInvoke> _synchronizer;
        private readonly ChangeableList<IXenConnection> _xenConnections;
        private readonly object _refreshLock = new object();
        private readonly object _collectionLock = new object();
        private readonly object _xenConnectionsLock;
        private bool _disposed;

        public event EventHandler<CreatedMockStorageLinkConnectionEventArgs> CreatedMockStorageLinkConnection;

        public StorageLinkConnectionManager(Func<ISynchronizeInvoke> synchronizer, ChangeableList<IXenConnection> xenConnections, object xenConnectionsLock)
        {
            Util.ThrowIfParameterNull(xenConnections, "xenConnections");
            Util.ThrowIfParameterNull(xenConnectionsLock, "xenConnectionsLock");

            _synchronizer = synchronizer;
            _xenConnections = xenConnections;
            _xenConnectionsLock = xenConnectionsLock;
            _xenConnections.CollectionChanged += XenConnections_CollectionChanged;
        }

        private void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            var connection = (IXenConnection)e.Element;
            
            if (e.Action == CollectionChangeAction.Add)
            {
                connection.CachePopulated += Refresh;
                connection.ConnectionStateChanged += Refresh;
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                connection.CachePopulated -= Refresh;
                connection.ConnectionStateChanged -= Refresh;
                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool != null)
                {
                    pool.PropertyChanged -= pool_PropertyChanged;
                }
            }
            if (Program.MainWindow != null)
                Program.BeginInvoke(Program.MainWindow, Refresh);
            else
            {
                Refresh();
            }
        }

        private void Refresh(object sender, EventArgs e)
        {
            Pool pool = Helpers.GetPoolOfOne((IXenConnection)sender);

            if (pool != null)
            {
                pool.PropertyChanged -= pool_PropertyChanged;
                pool.PropertyChanged += pool_PropertyChanged;
            }

            Refresh();
        }

        private void pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "other_config" || e.PropertyName == "restrictions")
            {
                Refresh();
            }
        }

        private List<IXenConnection> GetXenConnectionsCopy()
        {
            lock (_xenConnectionsLock)
            {
                return new List<IXenConnection>(_xenConnections);
            }
        }

        private void Invoke(MethodInvoker method)
        {
            if (_synchronizer() != null)
            {
                _synchronizer().Invoke(method, new object[0]);
            }
        }

        /// <summary>
        /// Moves deprecated locally stored StorageLink creds to the pool other-config. This can only be done by
        /// pool operators and above.
        /// </summary>
        private void MoveLocalCredentials()
        {
            // StorageLink credentials are stored in 3 places:

            // 1. In the device-config of any StorageLink PBDs.
            // 2. Locally in the Settings, once for each pool. This was a poor design choice in Midnight Ride.
            // 3. In the pool other-config. This is a replacement for the local settings. Implemented in Cowley.

            // This method moves locally stored settings to the pool other-config. This might not be
            // possible if the user doesn't have the privileges to do this.

            foreach (IXenConnection c in GetXenConnectionsCopy())
            {
                Pool pool = Helpers.GetPoolOfOne(c);

                if (c.IsConnected && pool != null && !Helpers.FeatureForbidden(c, Host.RestrictStorageChoices))
                {
                    Settings.CslgCredentials localCreds = null;
                    Invoke(() => localCreds = Settings.GetCslgCredentials(c));

                    if (localCreds != null && !string.IsNullOrEmpty(localCreds.Host) && !string.IsNullOrEmpty(localCreds.Username))
                    {
                        StorageLinkCredentials creds = pool.GetStorageLinkCredentials();

                        if (creds == null || string.IsNullOrEmpty(creds.Host))
                        {
                            // there aren't any creds on the pool object, but there are some locally.
                            // Attempt to move them.

                            try
                            {
                                string secretRef = Secret.get_by_uuid(c.Session, localCreds.PasswordSecret);
                                string password = Secret.get_value(c.Session, secretRef);

                                pool.SetStorageLinkCredentials(localCreds.Host, localCreds.Username, password);
                            }
                            catch (Failure)
                            {
                            }
                            catch (WebException)
                            {
                            }
                        }

                        // remove the local creds.
                        Invoke(() => Settings.SetCslgCredentials(c, new Settings.CslgCredentials(null, null, null)));
                    }
                }
            }
        }

        /// <summary>
        /// Gets all storage link credentials across all credentials.
        /// </summary>
        private List<StorageLinkCredentials> GetAllStorageLinkCredentials()
        {
            var output = new List<StorageLinkCredentials>();
            return output;
        }

        private void Refresh()
        {
            if (!_disposed)
            {
                ThreadPool.QueueUserWorkItem(Refresh);
            }
        }

        private void LaunchErrorDialog(string hostName)
        {
            new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error,
                                                                String.Format(Messages.PROBLEM_STORAGELINK_CONNECTION_FAILED,
                                                                              hostName).Replace("\\n", "\n"))).ShowDialog(Program.MainWindow);
        }

        private void Refresh(object state)
        {
            lock (_refreshLock)
            {
                MoveLocalCredentials();

                List<StorageLinkCredentials> allCreds = GetAllStorageLinkCredentials();

                // remove all storagelink connections that have out-of-date credentials            
                RemoveAll(s => null == allCreds.Find(c => c.Host == s.Host && c.Username == s.Username && c.Password == s.Password));

                // now add new storagelink connections
                foreach (StorageLinkCredentials creds in allCreds)
                {
                    if (Find(s => s.Host == creds.Host && s.Username == creds.Username) == null)
                    {
                        StorageLinkConnection con;

                        if (Program.RunInAutomatedTestMode)
                        {
                            var mockWebService = new MockStorageLinkWebService(creds.Host, creds.Username, creds.Password);
                            con = new StorageLinkConnection(_synchronizer(), mockWebService);

                            OnCreatedMockStorageLinkConnection(new CreatedMockStorageLinkConnectionEventArgs(mockWebService, con));
                        }
                        else
                        {
                            con = new StorageLinkConnection(_synchronizer(), creds.Host, creds.Username, creds.Password);
                        }

                        Invoke(() => InsertItem(Count, con));
                    }
                }
            }
        }

        protected override int RemoveAll(Predicate<StorageLinkConnection> match)
        {
            lock (_collectionLock)
            {
                return base.RemoveAll(match);
            }
        }

        protected override void Clear()
        {
            lock (_collectionLock)
            {
                base.Clear();
            }
        }

        protected override void RemoveItem(int index)
        {
            this[index].EndConnect();

            lock (_collectionLock)
            {
                this[index].ConnectionStateChanged -= item_Connected;
                base.RemoveItem(index);
            }
        }

        protected override void InsertItem(int index, StorageLinkConnection item)
        {
            lock (_collectionLock)
            {
                item.ConnectionStateChanged += item_Connected;
                base.InsertItem(index, item);
            }

            item.BeginConnect();

        }

        private void item_Connected(object sender, ConnectionStateChangedEventArgs e)
        {
            StorageLinkConnection c1 = (StorageLinkConnection)sender;

            if (e.ConnectionState == StorageLinkConnectionState.Disconnected && c1.SavedException != null)
            {
                LaunchErrorDialog(c1.Host);
                c1.SavedException = null;
            }

            if (e.ConnectionState == StorageLinkConnectionState.Connected)
            {
                // if this connection is a dup of another connection then remove it
                lock (_collectionLock)
                {
                    if (null != Find(c2 =>
                        {
                            StorageLinkServer s1 = c1.Cache.Server;
                            StorageLinkServer s2 = c2.Cache.Server;
                            return s1 != null && s1 != s2 && s1.Equals(s2);
                        }))
                    {
                        RemoveItem(IndexOf(c1));
                    }
                }
            }
        }

        public List<StorageLinkConnection> GetCopy()
        {
            lock (_collectionLock)
            {
                return new List<StorageLinkConnection>(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _xenConnections.CollectionChanged -= XenConnections_CollectionChanged;
                Clear();
            }
        }

        protected virtual void OnCreatedMockStorageLinkConnection(CreatedMockStorageLinkConnectionEventArgs e)
        {
            var handler = CreatedMockStorageLinkConnection;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class CreatedMockStorageLinkConnectionEventArgs : EventArgs
    {
        public MockStorageLinkWebService MockWebService { get; private set; }
        public StorageLinkConnection StorageLinkConnection { get; private set; }

        public CreatedMockStorageLinkConnectionEventArgs(MockStorageLinkWebService mockWebService, StorageLinkConnection storageLinkConnection)
        {
            Util.ThrowIfParameterNull(storageLinkConnection, "connection");

            MockWebService = mockWebService;
            StorageLinkConnection = storageLinkConnection;
        }
    }
}
