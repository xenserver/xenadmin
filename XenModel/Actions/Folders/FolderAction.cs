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

using XenAPI;

using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class FolderAction : AsyncAction
    {
        // CA-34379 Many folder operations do not work well with RBAC, because they operate across multiple connections.
        // Our strategy is roughly as follows:
        //
        // * As always, each Action has a designated Session. This may be an escalated Session in the case that we have
        //   sudo'ed, or passed in an escalated Session through RunExternal(). Sometimes, for example adding several
        //   objects to a folder, we can break the work down into several actions which can be escalated separately.
        //
        // * However, many actions, such as deleting or renaming a whole folder, inevitably operate across multiple
        //   connections. In that case we use this.Session for the relevant connection, and build up a dictionary of
        //   sessions for other connections using DuplicateSession(). However, these sessions are not escalated, so
        //   we block any such actions in the UI if we would need to operate on a read-only connection: otherwise we
        //   would sudo and then the action would still fail on the non-primary connection. (We could in principle
        //   escalate a whole collection of Sessions and pass them in, but it doesn't seem necessary, and perhaps
        //   it would be a bit odd because when renaming a folder, you feel you're operating on that folder, not really
        //   on the objects within it. For consistency, we even block the operation when all the objects are on a
        //   single read-only connection).
        //
        // * It's not actually quite true that adding an object to a folder only operates on that object's connection. If
        //   the folder was empty before, we should also now remove it from the empty folders list. But it's harmless to
        //   leave extra empty folders in the list, so we just catch the RBAC error and fail silently.
        //
        // * For safety, destructive operations are called last: duplication is not as bad as destructive failure.

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum Kind { New, Move, Delete, Rename }

        private readonly IXenObject obj;
        private readonly Folder folder;
        private readonly string path;
        private readonly string[] paths;
        private readonly Kind kind;

        private readonly Dictionary<IXenConnection, Session> Sessions = new Dictionary<IXenConnection, Session>();

        // Constructor used for Move and Delete
        public FolderAction(IXenObject obj, Folder folder, Kind kind)
            : base(obj.Connection, GetTitle(obj, folder, kind))
        {
            System.Diagnostics.Trace.Assert(kind == Kind.Move || kind == Kind.Delete);

            this.obj = obj;
            this.folder = folder;
            this.kind = kind;

            if ( obj.GetType() != typeof(Folder) )
            {
                ApiMethodsToRoleCheck.Add(obj.GetType().Name.ToLowerInvariant() + ".remove_from_other_config",
                                          Folders.FOLDER);
                ApiMethodsToRoleCheck.Add(obj.GetType().Name.ToLowerInvariant() + ".add_to_other_config",
                                          Folders.FOLDER);
            }

            ApiMethodsToRoleCheck.Add("pool.remove_from_other_config", Folders.EMPTY_FOLDERS);
            ApiMethodsToRoleCheck.Add("pool.add_to_other_config", Folders.EMPTY_FOLDERS);

            AppliesTo.Add(obj.opaque_ref);
            if (folder != null)
                AppliesTo.Add(folder.opaque_ref);
        }

        // Constructor used for New
        public FolderAction(IXenConnection connection, Kind kind, params string[] paths)
            : base(connection, GetTitleForNew(paths))
        {
            System.Diagnostics.Trace.Assert(kind == Kind.New);
            this.kind = kind;
            this.paths = paths;

            ApiMethodsToRoleCheck.Add("pool.remove_from_other_config", Folders.EMPTY_FOLDERS);
            ApiMethodsToRoleCheck.Add("pool.add_to_other_config", Folders.EMPTY_FOLDERS);

            foreach (string path in paths)
                AppliesTo.Add(path);
        }

        // Constructor used for Rename
        public FolderAction(Folder obj, String name, Kind kind)
            : base(obj.Connection, String.Format(Messages.RENAMING_FOLDER, Helpers.GetName(obj), name))
        {
            System.Diagnostics.Trace.Assert(kind == Kind.Rename);

            this.kind = kind;
            this.obj = obj;
            this.path = Folders.AppendPath(obj.Path, name);
            AppliesTo.Add(obj.opaque_ref);
            AppliesTo.Add(path);
        }

        internal static string GetTitle(IXenObject ixmo, Folder folder, Kind kind)
        {
            switch (kind)
            {
                case Kind.Move:
                    return String.Format(Messages.MOVE_OBJECT_TO_FOLDER, Helpers.GetName(ixmo), folder.Name);

                case Kind.Delete:
                    return String.Format(Messages.DELETING_FOLDER, Helpers.GetName(ixmo));

                default:
                    return String.Empty;
            }
        }

        internal static string GetTitleForNew(params string[] paths)
        {
            if (paths.Length == 1)
                return String.Format(Messages.CREATE_NEW_FOLDER, paths[0]);
            else
                return String.Format(Messages.CREATE_NEW_FOLDERS, String.Join("; ", paths));
        }

        protected override void Run()
        {
            CanCancel = true;
            switch (kind)
            {
                case Kind.Delete:
                case Kind.Move:
                    Description = folder == null ? Messages.DELETING_FOLDER : Messages.MOVING;
                    
                    DeleteOrMove(obj, folder, GetCancelling);
                    
                    Description = folder == null ? Messages.DELETED_FOLDER : Messages.MOVED;
                    break;

                case Kind.Rename:
                    Description = Messages.RENAMING;

                    Rename(obj, path, GetCancelling);

                    Description = Messages.RENAMED;
                    break;

                case Kind.New:
                    Description = paths.Length == 1 ? Messages.CREATING_NEW_FOLDER : Messages.CREATING_NEW_FOLDERS;

                    if(!AddFoldersToEmptyList(Connection, GetCancelling, paths))
                        throw new Exception(Messages.FOLDER_ALREADY_EXISTS);
                    Description = paths.Length == 1 ? Messages.CREATED_NEW_FOLDER : Messages.CREATED_NEW_FOLDERS;
                    break;
            }
        }

        public override void RecomputeCanCancel()
        {
            // CanCancel is always true.
        }

        internal void DeleteOrMove(IXenObject obj, Folder folder, Func<bool> cancelling)
        {
            // if, by moving candidate, we makes its parent
            // empty, then we must add from's parent to the empty list
            // CA-34379: Folder actions do not work with sudo. Make sure destructive operations are called last.

            string parent = obj.Path;
            if (!string.IsNullOrEmpty(parent))
            {
                if (new List<IXenObject>(Folders.Children(parent)).Count == 1)
                {
                    AddFoldersToEmptyList(obj.Connection, cancelling, parent);
                }
            }

            MoveContents(obj, folder == null ? null : folder.opaque_ref, cancelling);
        }

        internal void Rename(IXenObject obj, string path, Func<bool> cancelling)
        {
            // do the whole thing in a BackgroundMajorChange so the treeview doesn't get updated while the update
            // is only partially completed.

            ((XenConnection)Connection).OnBeforeMajorChange(true);
            try
            {
                // CA-34379: Folder actions do not work with sudo. Make sure destructive operations are called last.
                if (obj.opaque_ref != path)
                {
                    AddFoldersToEmptyList(obj.Connection, cancelling, path);

                    foreach (IXenObject ixmo in Folders.Children(obj.opaque_ref))
                        MoveContents(ixmo, path, cancelling);

                    RemoveFolderFromEmptyList(obj.opaque_ref, cancelling);
                }
            }
            finally
            {
                ((XenConnection)Connection).OnAfterMajorChange(true);
            }
        }

        private void MoveContents(IXenObject from, string to, Func<bool> cancelling)
        {
            ((XenConnection)Connection).OnBeforeMajorChange(true);
            try
            {
                MoveContents_(from, to, cancelling);
            }
            finally 
            {
                ((XenConnection)Connection).OnAfterMajorChange(true);
            }
            
        }

        private void MoveContents_(IXenObject from, string to, Func<bool> cancelling)
        {
            // CA-34379: Folder actions do not work with sudo. Make sure destructive operations are called last.
            if (cancelling())
                throw new CancelledException();

            Folder folder = from as Folder;
            if (folder == null)
            {
                log.DebugFormat("Moving {0} to {1}", Helpers.GetName(from), to ?? "<null>");
                SetFolder(from, to, cancelling);
                RemoveFolderFromEmptyList(to, cancelling);
                return;
            }

            string newLocation = to == null ? null : Folders.AppendPath(to, folder.name_label);

            log.DebugFormat("Moving contents of {0} to {1}", folder, newLocation ?? "<null>");

            bool empty = true;
            foreach (IXenObject ixmo in Folders.Children(folder.opaque_ref))
            {
                empty = false;

                MoveContents_(ixmo, newLocation, cancelling);
            }

            RemoveFolderFromEmptyList(from.opaque_ref, cancelling);

            if (empty && newLocation != null)
            {
                AddFoldersToEmptyList(from.Connection, cancelling, newLocation);
            }
        }

        // WARNING: This function isn't thread safe. It can't be called twice in quick succession
        // on separate threads, because we have to wait for the new empty folders list to come back
        // from the server in between. Each thread does that in WaitForEmptyFoldersCacheChange(),
        // but that won't stop another thread getting the wrong list.
        //Returns true if anyfolder was add to the empty list
        internal bool AddFoldersToEmptyList(IXenConnection connection, Func<bool> cancelling, params string[] paths)
        {
            if (connection == null)
                return false;

            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            List<string> emptyFolders = new List<string>(Folders.GetEmptyFolders(pool));
            bool anyAdded = false;
            foreach (string path in paths)
            {
                if (!emptyFolders.Contains(path))
                {
                    log.DebugFormat("Adding {0} to empty list on {1}", path, Helpers.GetName(pool));
                    emptyFolders.Add(path);
                    anyAdded = true;
                }
            }
            if (anyAdded)
            {
                SetEmptyFolders(connection, emptyFolders, cancelling);
                return true;
            }
            return false;
        }

        private void SetFolder(IXenObject xmo, string to, Func<bool> cancelling)
        {
            Session sess = GetSession(xmo.Connection);
            if (to == null)
                Helpers.RemoveFromOtherConfig(sess, xmo, Folders.FOLDER);
            else
                Helpers.SetOtherConfig(sess, xmo, Folders.FOLDER, to);
            WaitForFolderCacheChange(xmo, to, cancelling);
        }

        private Session GetSession(IXenConnection conn)
        {
            // First we look at this.Session. This allows us to sudo if we have only one connection.
            if (Session.Connection == conn)
                return Session;

            // Otherwise we dig into our dictionary of sessions for other connections. These cannot be sudo'ed,
            // because there is no good way to make a sudo dialog (or a series of dialogs) for several connections.
            if (Sessions.ContainsKey(conn))
                return Sessions[conn];
            Session s = conn.DuplicateSession();
            Sessions[conn] = s;
            return s;
        }

        private void RemoveFolderFromEmptyList(string path, Func<bool> cancelling)
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.IsConnected)
                    continue;

                List<string> emptyFolders = new List<string>(Folders.GetEmptyFolders(connection));
                if (emptyFolders.Contains(path))
                {
                    emptyFolders.Remove(path);
                    try
                    {
                        SetEmptyFolders(connection, emptyFolders, cancelling);
                    }
                    // We ignore RBAC exceptions. They are caused by trying to remove a folder from
                    // the empty list on a read-only connection. Leaving an additional empty folder
                    // lying around is harmless: we don't want to report an error for it. See CA-40412
                    // for an example of this.
                    catch (Exception e)
                    {
                        Failure f = e as Failure;
                        if (f == null || f.ErrorDescription[0] != Failure.RBAC_PERMISSION_DENIED)
                            throw;
                    }
                }
            }
        }

        private void SetEmptyFolders(IXenConnection connection, List<string> folders, Func<bool> cancelling)
        {
            string folder_str = string.Join(Folders.EMPTY_FOLDERS_SEPARATOR, folders.ToArray());
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, "Pool has gone away!");
            Helpers.SetOtherConfig(GetSession(pool.Connection), pool, Folders.EMPTY_FOLDERS, folder_str);
            WaitForEmptyFoldersCacheChange(pool, folder_str, cancelling);
        }

        private static void WaitForFolderCacheChange(IXenObject xmo, string expected, Func<bool> cancelling)
        {
            xmo.Connection.WaitFor(delegate()
            {
                return Folders.GetFolderString(xmo) == expected;
            },
                cancelling);
            if (cancelling())
                throw new CancelledException();
        }

        private static void WaitForEmptyFoldersCacheChange(Pool pool, string expected, Func<bool> cancelling)
        {
            pool.Connection.WaitFor(delegate()
            {
                return Folders.GetEmptyFoldersString(pool) == expected;
            },
                cancelling);
            if (cancelling())
                throw new CancelledException();
        }
    }
}
