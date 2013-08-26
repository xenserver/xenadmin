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
using XenAdmin.Model;
using XenAPI;


namespace XenAdmin.XenSearch
{
    public class OrganizationalView : Grouping
    {
        private static readonly QueryFilter FoldersQuery = new NullQuery<Folder>(PropertyNames.folder, false);
        private static readonly QueryFilter vAppsQuery = new BooleanQuery(PropertyNames.in_any_appliance, true);
        private static readonly QueryFilter TagsQuery = new ListEmptyQuery<String>(PropertyNames.tags, false);
        private static readonly QueryFilter FieldsQuery = new BooleanQuery(PropertyNames.has_custom_fields, true);

        private static readonly Grouping FoldersGrouping = new FolderGrouping((Grouping)null);
        private static readonly Grouping vAppsGrouping = new XenModelObjectPropertyGrouping<VM_appliance>(PropertyNames.appliance, null);
        private static readonly Grouping TagsGrouping = new PropertyGrouping<String>(PropertyNames.tags, null);
        private static readonly Grouping TypesGrouping = new PropertyGrouping<ObjectTypes>(PropertyNames.type, null);
        private static readonly Grouping FieldsGrouping = new AllCustomFieldsGrouping((Grouping)null);

        private OrganizationalView() : 
            base((Grouping)null)
        {
        }

        public override string GroupingName
        {
            get { return String.Empty; }
        }

        public override object GetGroup(IXenObject o)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Icons GetGroupIcon(object obj)
        {
            String group = obj as String;
            if (group != null)
            {
                if (group == Messages.FOLDERS)
                    return Icons.Folder;
                if (group == Messages.VM_APPLIANCES)
                    return Icons.VmAppliance;
                if (group == Messages.TAGS)
                    return Icons.Tag;
                if (group == Messages.CUSTOM_FIELDS)
                    return Icons.CustomField;
            }
                
            return base.GetGroupIcon(group);
        }

        internal override QueryFilter GetSubquery(object parent, object val)
        {
            string v = val as string;
            if (v == Messages.FOLDERS)
                return FoldersQuery;
            if (v == Messages.VM_APPLIANCES)
                return vAppsQuery;
            if (v == Messages.TAGS)
                return TagsQuery;
            if (v == Messages.CUSTOM_FIELDS)
                return FieldsQuery;
            return null;
        }

        internal override Grouping GetSubgrouping(object val)
        {
            string v = val as string;
            if (v == Messages.FOLDERS)
                return FoldersGrouping;
            if (v == Messages.VM_APPLIANCES)
                return vAppsGrouping;
            if (v == Messages.TAGS)
                return TagsGrouping;
            if (v == Messages.TYPES)
                return TypesGrouping;
            if (v == Messages.CUSTOM_FIELDS)
                return FieldsGrouping;
            return null;
        }

        private static OrganizationalView instance;

        private static OrganizationalView Instance
        {
            get
            {
                if (instance == null)
                    instance = new OrganizationalView();

                return instance;
            }
        }

        /// <param name="includeFolders">
        /// Normally exclude folders even if in search: CA-27260
        /// </param>
        private static Search OrgViewSearch(Search search, bool includeFolders, QueryFilter addFilter, Grouping grouping)
        {
            QueryFilter filter;
            ObjectTypes types;

            if (search == null || search.Query == null || search.Query.QueryScope == null)
            {
                types = includeFolders ? ObjectTypes.AllIncFolders : ObjectTypes.AllExcFolders;
            }
            else
            {
                types = includeFolders
                            ? search.Query.QueryScope.ObjectTypes
                            : (search.Query.QueryScope.ObjectTypes & ~ObjectTypes.Folder);
            }

            QueryScope scope = new QueryScope(types);

            if (search == null || search.Query == null || search.Query.QueryFilter == null)
                filter = addFilter;
            else if (addFilter == null)
                filter = search.Query.QueryFilter;
            else
                filter = new GroupQuery(new QueryFilter[] { search.Query.QueryFilter, addFilter }, GroupQuery.GroupQueryType.And);

            return new Search(new Query(scope, filter), grouping, false, "", null, null, new Sort[] { });
        }

        private static Search FoldersSearch(Search search)
        {
            return OrgViewSearch(search, true, FoldersQuery, FoldersGrouping);
        }

        private static Search vAppsSearch(Search search)
        {
            return OrgViewSearch(search, false, vAppsQuery, vAppsGrouping);
        }

        private static Search TagsSearch(Search search)
        {
            return OrgViewSearch(search, false, TagsQuery, TagsGrouping);
        }

        private static Search TypesSearch(Search search)
        {
            return OrgViewSearch(search, false, null, TypesGrouping);
        }

        private static Search FieldsSearch(Search search)
        {
            return OrgViewSearch(search, false, FieldsQuery, FieldsGrouping);
        }

        public static void PopulateObjectView(IAcceptGroups adapter, Search search)
        {
            AddGroup(adapter, Messages.TYPES, TypesSearch(search));

            adapter.FinishedInThisGroup(true);
        }

        public static void PopulateTagsView(IAcceptGroups adapter, Search search)
        {
            AddGroup(adapter, Messages.TAGS, TagsSearch(search));
            adapter.FinishedInThisGroup(true);
        }

        public static void PopulateFoldersView(IAcceptGroups adapter, Search search)
        {
            AddGroup(adapter, Folders._root, FoldersSearch(search));
            adapter.FinishedInThisGroup(true);
        }

        public static void PopulateCustomFieldsView(IAcceptGroups adapter, Search search)
        {
            AddGroup(adapter, Messages.CUSTOM_FIELDS, FieldsSearch(search));
            adapter.FinishedInThisGroup(true);
        }

        public static void PopulateVappsView(IAcceptGroups adapter, Search search)
        {
            AddGroup(adapter, Messages.VM_APPLIANCES, vAppsSearch(search));
            adapter.FinishedInThisGroup(true);
        }

        private static void AddGroup(IAcceptGroups adapter, object label, Search search)
        {
            IAcceptGroups a = adapter.Add(Instance, label, 0);
            Group g = Group.GetGrouped(search);
            g.Populate(a, 1, false);
            a.FinishedInThisGroup(true);
        }
    }
}
