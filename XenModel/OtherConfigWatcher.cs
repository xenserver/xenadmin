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
using System.ComponentModel;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin
{
    public class OtherConfigAndTagsWatcher
    {
        public static event EventHandler OtherConfigChanged;
        public static event EventHandler TagsChanged;
        public static event EventHandler GuiConfigChanged;
        private static bool FireOtherConfigEvent = false;
        private static bool FireTagsEvent = false;
        private static bool FireGuiConfigEvent = false;

        static OtherConfigAndTagsWatcher()
        {
            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;
            XenConnections_CollectionChanged(null, null);
        }

        public static void InitEventHandlers()
        {
            PoolCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<Pool>);
            VMCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<VM>);
            HostCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<Host>);
            SRCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<SR>);
            VDICollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<VDI>);
            NetworkCollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged<XenAPI.Network>);

            XenConnections_CollectionChanged(null, null);
        }

        private static CollectionChangeEventHandler PoolCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler VMCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler HostCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler SRCollectionChangedWithInvoke;
        private static CollectionChangeEventHandler VDICollectionChangedWithInvoke;
        private static CollectionChangeEventHandler NetworkCollectionChangedWithInvoke;
        
        private static void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {

                connection.Cache.RegisterCollectionChanged<Pool>(PoolCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<Host>(HostCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<VM>(VMCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<SR>(SRCollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<VDI>(VDICollectionChangedWithInvoke);
                connection.Cache.RegisterCollectionChanged<XenAPI.Network>(NetworkCollectionChangedWithInvoke);

                connection.XenObjectsUpdated -= connection_XenObjectsUpdated;
                connection.XenObjectsUpdated += connection_XenObjectsUpdated;
                connection.ConnectionStateChanged -= connection_ConnectionStateChanged;
                connection.ConnectionStateChanged += connection_ConnectionStateChanged;
            }

            FireOtherConfigEvent = true;
            FireTagsEvent = true;
            FireGuiConfigEvent = true;
        }

        private static void CollectionChanged<T>(object sender, CollectionChangeEventArgs e) where T : XenObject<T>
        {
            T xmo = e.Element as T;

            if (xmo == null)
                return;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    xmo.PropertyChanged += PropertyChanged<T>;
                    break;
                case CollectionChangeAction.Remove:
                    xmo.PropertyChanged -= PropertyChanged<T>;
                    break;
                case CollectionChangeAction.Refresh:
                    // As of writing, ChangeableDictionary never fires a Refresh event.
                    // If this changes, we need to take it into account here.
                    throw new NotImplementedException("CollectionChangeAction.Refresh is unhandled!");
            }

            FireOtherConfigEvent = true;
            FireTagsEvent = true;
            FireGuiConfigEvent = true;
        }

        private static void PropertyChanged<T>(object sender1, PropertyChangedEventArgs e) where T : XenObject<T>
        {
            if (e.PropertyName == "other_config")
                FireOtherConfigEvent = true;

            if (e.PropertyName == "tags")
                FireTagsEvent = true;

            if (e.PropertyName == "gui_config")
                FireGuiConfigEvent = true;
        }

        private static void connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            if (FireOtherConfigEvent)
                OnOtherConfigChanged();

            if (FireTagsEvent)
                OnTagsChanged();

            if (FireGuiConfigEvent)
                OnGuiConfigChanged();

            FireOtherConfigEvent = false;
            FireTagsEvent = false;
            FireGuiConfigEvent = false;
        }

        private static void connection_ConnectionStateChanged(object sender, EventArgs e)
        {
            InvokeHelper.Invoke(delegate()
            {
                OnOtherConfigChanged();
                OnTagsChanged();
                OnGuiConfigChanged();

                FireOtherConfigEvent = false;
                FireTagsEvent = false;
                FireGuiConfigEvent = false;
            });
        }

        private static void OnOtherConfigChanged()
        {
            InvokeHelper.AssertOnEventThread();

            if (OtherConfigChanged != null)
                OtherConfigChanged(null, new EventArgs());
        }

        private static void OnTagsChanged()
        {
            InvokeHelper.AssertOnEventThread();

            if (TagsChanged != null)
                TagsChanged(null, new EventArgs());
        }

        private static void OnGuiConfigChanged()
        {
            InvokeHelper.AssertOnEventThread();

            if (GuiConfigChanged != null)
                GuiConfigChanged(null, new EventArgs());
        }
    }
}
