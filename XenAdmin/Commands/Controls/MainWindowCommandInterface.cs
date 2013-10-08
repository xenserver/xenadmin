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
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using XenAdmin.Network;
using XenAPI;
using System.Collections.ObjectModel;
using XenAdmin.Actions;
using XenAdmin.Core;

namespace XenAdmin
{
    /// <summary>
    /// An interface by which the Commands communicate with MainWindow. This abstraction improves maintainability
    /// and testability of the Commands.
    /// </summary>
    public interface IMainWindow
    {
        void Invoke(MethodInvoker method);
        bool SelectObjectInTree(IXenObject xenObject);
        void TrySelectNewObjectInTree(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible);
        void TrySelectNewObjectInTree(IXenConnection c, bool selectNode, bool expandNode, bool ensureNodeVisible);
        void Refresh();
        void ShowPerXenModelObjectWizard(IXenObject obj, Form wizard);
        void ShowPerConnectionWizard(IXenConnection connection, Form wizard);
        void ShowForm(Type type);
        void ShowForm(Type type, object[] args);
        void CloseActiveWizards(IXenConnection connection);
        void CloseActiveWizards(IXenObject xenObject);
        Collection<IXenConnection> GetXenConnectionsCopy();
        void SaveServerList();
        ReadOnlyCollection<ActionBase> History { get; }
        bool DoSearch(string filename);
        bool RunInAutomatedTestMode { get;}
        void RemoveConnection(IXenConnection connection);
        void BringToFront();
        void PutSelectedNodeIntoEditMode();
        void SwitchToTab(MainWindow.Tab tab);
        bool MenuShortcutsEnabled { get;}
        Form Form { get; }
    }

    partial class MainWindow
    {
        /// <summary>
        /// The MainWindow implementation of <see cref="IMainWindowCommandInterface"/>. Used by Commands to
        /// communicate with <see cref="MainWindow"/>.
        /// </summary>
        internal class MainWindowCommandInterface : IMainWindow
        {
            private readonly MainWindow _owner;

            public MainWindowCommandInterface(MainWindow owner)
            {
                Util.ThrowIfParameterNull(owner, "owner");
                _owner = owner;
            }

            #region IMainWindowCommandInterface Members

            public Form Form
            {
                get
                {
                    return _owner;
                }
            }

            public void Invoke(MethodInvoker method)
            {
                Program.Invoke(_owner, method);
            }

            public bool SelectObjectInTree(IXenObject xenObject)
            {
                return _owner.SelectObject(xenObject);
            }

            public void Refresh()
            {
                _owner.RequestRefreshTreeView();
            }

            public void ShowPerXenModelObjectWizard(IXenObject obj, Form wizard)
            {
                _owner.ShowPerXenModelObjectWizard(obj, wizard);
            }

            public void ShowPerConnectionWizard(IXenConnection connection, Form wizard)
            {
                _owner.ShowPerConnectionWizard(connection, wizard);
            }

            public void ShowForm(Type type)
            {
                _owner.ShowForm(type);
            }

            public void ShowForm(Type type, object[] args)
            {
                _owner.ShowForm(type, args);
            }

            public void CloseActiveWizards(IXenConnection connection)
            {
                _owner.closeActiveWizards(connection);
            }

            public void CloseActiveWizards(IXenObject xenObject)
            {
                _owner.closeActiveWizards(xenObject);
            }

            public Collection<IXenConnection> GetXenConnectionsCopy()
            {
                return new Collection<IXenConnection>(ConnectionsManager.XenConnectionsCopy);
            }

            public void SaveServerList()
            {
                Settings.SaveServerList();
            }

            public ReadOnlyCollection<ActionBase> History
            {
                get
                {
                    return new ReadOnlyCollection<ActionBase>(ConnectionsManager.History);
                }
            }

            public bool DoSearch(string filename)
            {
                return _owner.DoSearch(filename);
            }

            public bool RunInAutomatedTestMode
            {
                get { return Program.RunInAutomatedTestMode; }
            }

            public void RemoveConnection(IXenConnection connection)
            {
                ConnectionsManager.ClearCacheAndRemoveConnection(connection);
            }

            public void BringToFront()
            {
                HelpersGUI.BringFormToFront(_owner);
            }

            public void PutSelectedNodeIntoEditMode()
            {
                _owner.EditSelectedNodeInTreeView();
            }

            public void SwitchToTab(Tab tab)
            {
                _owner.SwitchToTab(tab);
            }

            public void TrySelectNewObjectInTree(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
            {
                _owner.TrySelectNewNode(tagMatch, selectNode, expandNode, ensureNodeVisible);
            }

            public void TrySelectNewObjectInTree(IXenConnection c, bool selectNode, bool expandNode, bool ensureNodeVisible)
            {
                _owner.TrySelectNewNode(c, selectNode, expandNode, ensureNodeVisible);
            }

            public bool MenuShortcutsEnabled
            {
                get { return _owner._menuShortcuts; }
            }

            #endregion
        }
    }
}
