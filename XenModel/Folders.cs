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
using System.Diagnostics;
using XenAPI;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Model
{
    public class Folders
    {
        public const String FOLDER = "folder";
        public const String PATH_SEPARATOR = "/";
        public static char[] PATH_SEPARATORS = new char[] { '/' };
        public const String EMPTY_FOLDERS = "EMPTY_FOLDERS";
        public const String EMPTY_FOLDERS_SEPARATOR = ";";
        public static char[] EMPTY_FOLDERS_SEPARATORS = new char[] { ';' };

        public static readonly Folder _root = null;

        static Folders()
        {
            _root = new Folder(null, Messages.FOLDERS);
            _root.opaque_ref = Folders.PATH_SEPARATOR;
        }

        private static bool updateEmptyFolders = false;

        public static void InitFolders()
        {
            Trace.Assert(InvokeHelper.Synchronizer != null);
            CollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged);

            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                AddConnection(connection);
        }

        private static void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            //Program.AssertOnEventThread();
            InvokeHelper.BeginInvoke(() =>
                                                        {
                                                            IXenConnection connection = e.Element as IXenConnection;
                                                            if (connection == null)
                                                                return;

                                                            switch (e.Action)
                                                            {
                                                                case CollectionChangeAction.Add:
                                                                    AddConnection(connection);
                                                                    break;

                                                                case CollectionChangeAction.Remove:
                                                                    RemoveConnection(connection);
                                                                    break;
                                                            }
                                                        });
        }

        private static CollectionChangeEventHandler CollectionChangedWithInvoke;
        private static void AddConnection(IXenConnection connection)
        {
            connection.Cache.RegisterCollectionChanged<Host>(CollectionChangedWithInvoke);
            connection.Cache.RegisterCollectionChanged<XenAPI.Network>(CollectionChangedWithInvoke);
            connection.Cache.RegisterCollectionChanged<Pool>(CollectionChangedWithInvoke);
            connection.Cache.RegisterCollectionChanged<SR>(CollectionChangedWithInvoke);
            connection.Cache.RegisterCollectionChanged<VDI>(CollectionChangedWithInvoke);
            connection.Cache.RegisterCollectionChanged<VM>(CollectionChangedWithInvoke);

            connection.XenObjectsUpdated += connection_XenObjectsUpdated;

            InvokeHelper.Invoke(delegate()
            {
                UpdateAll(connection.Cache.Hosts);
                UpdateAll(connection.Cache.Networks);
                UpdateAll(connection.Cache.Pools);
                UpdateAll(connection.Cache.SRs);
                UpdateAll(connection.Cache.VDIs);
                UpdateAll(connection.Cache.VMs);

                UpdateEmptyFolders(connection);
            });
        }

        private static void RemoveConnection(IXenConnection connection)
        {
            connection.Cache.DeregisterCollectionChanged<Host>(CollectionChangedWithInvoke);
            connection.Cache.DeregisterCollectionChanged<XenAPI.Network>(CollectionChangedWithInvoke);
            connection.Cache.DeregisterCollectionChanged<Pool>(CollectionChangedWithInvoke);
            connection.Cache.DeregisterCollectionChanged<SR>(CollectionChangedWithInvoke);
            connection.Cache.DeregisterCollectionChanged<VDI>(CollectionChangedWithInvoke);
            connection.Cache.DeregisterCollectionChanged<VM>(CollectionChangedWithInvoke);

            connection.XenObjectsUpdated -= connection_XenObjectsUpdated;
        }

        private static void CollectionChanged(Object sender, CollectionChangeEventArgs e)
        {
            InvokeHelper.AssertOnEventThread();

            IXenObject ixmo = e.Element as IXenObject;
            if (ixmo == null)
                return;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    ixmo.PropertyChanged += ServerXenObject_PropertyChanged;
                    UpdateFolder(ixmo);
                    if (ixmo is Pool)
                        UpdateEmptyFolders(ixmo.Connection);
                    break;

                case CollectionChangeAction.Remove:
                    ixmo.PropertyChanged -= ServerXenObject_PropertyChanged;
                    RemoveObject(ixmo);
                    break;

                default:
                    System.Diagnostics.Trace.Assert(false);
                    break;
            }
        }

        static void connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            IXenConnection connection = sender as IXenConnection;
            if (connection == null)
                return;

            if (updateEmptyFolders)
            {
                updateEmptyFolders = false;
                UpdateEmptyFolders(connection);
            }

            connection.Cache.CheckFoldersBatchChange();
        }

        private static void ServerXenObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IXenObject xenObject = (IXenObject) sender;
            if (e.PropertyName == "other_config")
            {
                UpdateFolder(xenObject);

                if (xenObject is Pool)
                    updateEmptyFolders = true;
            }
        }

        private static void RemoveObject(IXenObject ixmo)
        {
            InvokeHelper.AssertOnEventThread();

            XenRef<Folder> path = new XenRef<Folder>(ixmo.Path);
            Folder folder = ixmo.Connection.Resolve(path);
            if (folder != null)
            {
                folder.RemoveObject(ixmo);
                PotentiallyRemoveFolder(folder);
            }
        }

        private static void UpdateAll(IXenObject[] ixmos)
        {
            foreach (IXenObject ixmo in ixmos)
            {
                ixmo.PropertyChanged -= ServerXenObject_PropertyChanged;
                ixmo.PropertyChanged += ServerXenObject_PropertyChanged;
                UpdateFolder(ixmo);
            }
        }

        private static void UpdateFolder(IXenObject ixmo)
        {
            InvokeHelper.AssertOnEventThread();

            IXenConnection connection = ixmo.Connection;

            XenRef<Folder> oldPath = new XenRef<Folder>(ixmo.Path);
            Folder oldFolder = connection.Resolve(oldPath);

            String newPath = GetPathFromOtherConfig(ixmo);

            if (oldPath.opaque_ref.Equals(newPath) && (newPath.Trim().Length == 0 || oldFolder != null))
                return;

            Folder newFolder = GetOrCreateFolder(connection, newPath);
            
            if (newFolder != null)
                newFolder.AddObject(ixmo);

            if (oldFolder != null)
            {
                oldFolder.RemoveObject(ixmo);
                PotentiallyRemoveFolder(oldFolder);
            }

            ixmo.Path = newPath;
        }

        private static void PotentiallyRemoveFolder(Folder folder)
        {
            if (folder == null || folder.XenObjectsCount > 0)
                return;

            String[] emptyFolders = GetEmptyFolders(folder.Connection);

            if (Array.BinarySearch<String>(emptyFolders, folder.opaque_ref) >= 0)
                return;

            if (folder.Parent != null)
                folder.Parent.RemoveObject(folder);
            folder.Connection.Cache.RemoveFolder(new XenRef<Folder>(folder.opaque_ref));

            PotentiallyRemoveFolder(folder.Parent);
        }

        internal static string[] GetEmptyFolders(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            return pool == null ? new string[0] : GetEmptyFolders(pool);
        }

        internal static string[] GetEmptyFolders(Pool pool)
        {
            string emptyFolder = GetEmptyFoldersString(pool);
            if (emptyFolder == "")
                return new string[0];

            string[] emptyFolders = Array.FindAll<string>(
                emptyFolder.Split(EMPTY_FOLDERS_SEPARATORS),
                (Predicate<string>)delegate(string s) { return s.StartsWith("/"); });
            Array.Sort<string>(emptyFolders);

            return emptyFolders;
        }

        internal static string GetEmptyFoldersString(Pool pool)
        {
            Dictionary<string, string> other_config = pool.other_config;
            string v;
            return
                other_config == null ? "" :
                other_config.TryGetValue(Folders.EMPTY_FOLDERS, out v) ? v :
                "";
        }

        private static void UpdateEmptyFolders(IXenConnection connection)
        {
            InvokeHelper.AssertOnEventThread();

            String[] emptyFolders = GetEmptyFolders(connection);

            Folder root = connection.Resolve(new XenRef<Folder>(PATH_SEPARATOR));
            if (root != null)
                PurgeEmptyFolders(emptyFolders, root);

            foreach (String path in emptyFolders)
                GetOrCreateFolder(connection, path);
        }

        private static void PurgeEmptyFolders(String[] emptyFolders, Folder folder)
        {
            List<Folder> toRemove = new List<Folder>();

            foreach (IXenObject ixmo in folder.XenObjects)
            {
                Folder subFolder = ixmo as Folder;
                if (subFolder == null)
                    continue;

                PurgeEmptyFolders(emptyFolders, subFolder);

                if (subFolder.XenObjectsCount <= 0 &&
                    Array.BinarySearch<String>(emptyFolders, subFolder.opaque_ref) < 0)
                {
                    toRemove.Add(subFolder);
                    continue;
                }
            }

            foreach (Folder subFolder in toRemove)
            {
                folder.RemoveObject(subFolder);
                folder.Connection.Cache.RemoveFolder(new XenRef<Folder>(subFolder.opaque_ref));
            }
        }

        private static Folder GetOrCreateFolder(IXenConnection connection, String path)
        {
            String[] points = PointToPath(path);
            if (points == null)
                return null;

            return GetOrCreateFolder(connection, points, points.Length);
        }

        private static Folder GetOrCreateFolder(IXenConnection connection, String[] path, int i)
        {
            if(i < 0)
                return null;

            XenRef<Folder> pathToPoint = new XenRef<Folder>(PathToPoint(path, i));
           
            Folder folder = connection.Resolve(pathToPoint);
            if (folder != null)
                return folder;

            Folder parent = GetOrCreateFolder(connection, path, i - 1);

            String name = i - 1 >= 0 ? path[i - 1] : String.Empty;
            Folder _folder = new Folder(parent, name);
            _folder.Path = (parent == null ? "" : parent.opaque_ref);
            _folder.opaque_ref = pathToPoint.opaque_ref;
            _folder.Connection = connection;

            if(parent != null)
                parent.AddObject(_folder);

            connection.Cache.AddFolder(pathToPoint, _folder);

            return _folder;
        }

        public static Folder GetFolder(IXenObject o)
        {
            if (o == null || o.Connection == null)
                return null;
            return o.Connection.Resolve(new XenRef<Folder>(o.Path));
        }

        public static ComparableList<Folder> GetAncestorFolders(IXenObject o)
        {
            ComparableList<Folder> folders = new ComparableList<Folder>();

            for (Folder folder = GetFolder(o);
                folder != null && !folder.IsRootFolder;
                folder = folder.Parent)
            {
                folders.Add(folder);
            }

            return folders;
        }

        public static String[] PointToPath(String path)
        {
            path = path.Trim();
            if (String.IsNullOrEmpty(path))
                return null;

            path = path.TrimStart(PATH_SEPARATORS);
            return path.Split(PATH_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
        }

        public static String PathToPoint(String[] path, int depth)
        {
            return PATH_SEPARATOR + String.Join(PATH_SEPARATOR, path, 0, depth);
        }

        private static String GetPathFromOtherConfig(IXenObject o)
        {
            return GetFolderString(o) ?? "";
        }

        internal static string GetFolderString(IXenObject o)
        {
            Dictionary<string, string> other_config = Helpers.GetOtherConfig(o);
            string v;
            return
                other_config == null ? null :
                other_config.TryGetValue(FOLDER, out v) ? v :
                null;
        }

        public static void Create(IXenConnection connection, params string[] paths)
        {
            if (paths.Length > 0)
                new CreateFolderAction(connection, paths).RunAsync();
        }

        public static void Move(Session session, IXenObject ixmo, Folder target)
        {
            if (ixmo == null || target == null)
                return;

            // First check we're not moving parent -> child
            if (target.IsChildOf(ixmo))
                return;

            // Then check the object is not already in the folder
            for (int i = 0; i < target.XenObjects.Length; i++)
            {
                if (target.XenObjects[i].opaque_ref == ixmo.opaque_ref)
                    return;
            }

            FolderAction action = new MoveToFolderAction(ixmo, target);
            if (session == null)
                action.RunAsync();
            else
                action.RunExternal(session);
        }

        public static void Move(IXenObject ixmo, Folder target)
        {
            Move(null, ixmo, target);
        }
            
        public static void Move(Session session, IXenObject ixmo, string target)
        {
            Folder folder = GetOrCreateFolder(ixmo.Connection, target);
            Move(session, ixmo, folder);
        }

        public static void Rename(Folder folder, string new_name)
        {
            if (folder == null)
                return;

            new RenameFolderAction(folder, new_name).RunAsync();
        }

        public static void Unfolder(Session session, IXenObject ixmo)
        {
            if (ixmo == null)
                return;

            var action = new DeleteFolderAction(ixmo);
            if (session == null)
                action.RunAsync();
            else
                action.RunExternal(session);
        }

        public static void Unfolder(IXenObject ixmo)
        {
            Unfolder(null, ixmo);
        }

        // Fix up a path name to avoid empty folders or leading/trailing whitespace: CA-26707
        public static void FixupRelativePath(ref string name)
        {
            if (name == null)
                return;

            name = name.Trim();
            name = name.Replace("\t", " ");
            int oldLength;
            do
            {
                oldLength = name.Length;
                name = name.Replace("//", "/");
                name = name.Replace("/ ", "/");
                name = name.Replace(" /", "/");
            } while (oldLength != name.Length);
            name = name.Trim('/');
        }

        // The first part can end with "/" or not, but the second should have been
        // cleaned up with FixupRelativePath() first.
        public static String AppendPath(String first, String second)
        {
            return first.EndsWith(PATH_SEPARATOR) ?
                first + second : first + PATH_SEPARATOR + second;
        }

        /// <summary>
        /// Iterate over all immediate children of a folder, across all connections
        /// </summary>
        public static IEnumerable<IXenObject> Children(string path)
        {
            List<string> seen = new List<string>();

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.IsConnected || Helpers.GetPoolOfOne(connection) == null)
                    continue;

                Folder folder = connection.Resolve(new XenRef<Folder>(path));
                if (folder == null)
                    continue;

                foreach (IXenObject ixmo in folder.XenObjects)
                {
                    if (seen.Contains(ixmo.opaque_ref))
                        continue;

                    seen.Add(ixmo.opaque_ref);

                    yield return ixmo;
                }
            }
        }

        /// <summary>
        /// Iterate over all descendants of a folder, across all connections
        /// Returns folders themselves as well as their contents
        /// </summary>
        public static IEnumerable<IXenObject> Descendants(string path)
        {
            List<string> seen = new List<string>();

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.IsConnected || Helpers.GetPoolOfOne(connection) == null)
                    continue;

                Folder folder = connection.Resolve(new XenRef<Folder>(path));
                if (folder == null)
                    continue;

                foreach (IXenObject ixmo in folder.XenObjects)
                {
                    if (seen.Contains(ixmo.opaque_ref))
                        continue;

                    seen.Add(ixmo.opaque_ref);

                    yield return ixmo;

                    Folder subfolder = ixmo as Folder;
                    if (subfolder != null)
                    {
                        foreach (IXenObject ixmo2 in Folders.Descendants(subfolder.opaque_ref))
                            yield return ixmo2;
                    }
                }
            }
        }

        public static bool HasSubfolders(string path)
        {
            foreach (IXenObject ixmo in Children(path))
            {
                if (ixmo is Folder)
                    return true;
            }
            return false;
        }

        public static bool ContainsResources(string path)
        {
            foreach (IXenObject ixmo in Children(path))
            {
                if (!(ixmo is Folder) ||
                    ContainsResources(ixmo.opaque_ref))
                    return true;
            }
            return false;
        }

        public static String GetParent(String p)
        {
            String[] points = PointToPath(p);
            if (points == null)
                return null;
            return PathToPoint(points, points.Length - 1);
        }
    }
}
