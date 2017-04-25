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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using XenAdmin.CustomFields;
using XenAPI;
using Citrix.XenCenter;

using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAdmin.XenSearch
{
    public abstract class QueryFilter
    {
        public abstract bool? Match(IXenObject o);

        public abstract QueryFilter GetSubQueryFor(PropertyNames property);

        #region Marshalling

        protected abstract void AddXmlAttributes(XmlDocument doc, XmlNode node);

        // overrides can return null in order not to get into the XML at all
        public virtual XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(SearchMarshalling.GetClassName(this));

            AddXmlAttributes(doc, node);

            return node;
        }

        #endregion
    }

    // A dummy filter that doesn't contribute to the search
    public class DummyQuery : QueryFilter
    {
        public DummyQuery() : base() { }
        // don't need public TrueQuery(XmlNode node) because they should never get into the XML

        public override bool? Match(IXenObject o)
        {
            return null;
        }

        public override QueryFilter GetSubQueryFor(PropertyNames property)
        {
            return null;
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        { }

        public override XmlNode ToXmlNode(XmlDocument doc)
        {
            return null;  // don't put these types in the XML
        }
    }

    public class GroupQuery : QueryFilter
    {
        public enum GroupQueryType { And, Or, Nor };

        public readonly GroupQueryType type;
        public readonly QueryFilter[] subqueries;

        public GroupQuery(QueryFilter[] subqueries, GroupQueryType type)
        {
            this.subqueries = subqueries;
            this.type = type;
        }

        public override bool? Match(IXenObject o)
        {
            bool? result = null;

            switch (type)
            {
                case GroupQueryType.And:
                    foreach (QueryFilter subquery in subqueries)
                    {
                        bool? b = subquery.Match(o);
                        if (b == false)
                            return false;
                        if (b == true)
                            result = true;
                    }
                    return result;

                case GroupQueryType.Or:
                    foreach (QueryFilter subquery in subqueries)
                    {
                        bool? b = subquery.Match(o);
                        if (b == true)
                            return true;
                        if (b == false)
                            result = false;
                    }
                    return result;

                case GroupQueryType.Nor:
                    foreach (QueryFilter subquery in subqueries)
                    {
                        bool? b = subquery.Match(o);
                        if (b == true)
                            return false;
                        if (b == false)
                            result = true;
                    }
                    return result;

                default:
                    return result;
            }
        }

        public override QueryFilter GetSubQueryFor(PropertyNames property)
        {
            List<QueryFilter> queries = new List<QueryFilter>();

            foreach (QueryFilter subquery in subqueries)
            {
                QueryFilter query = subquery.GetSubQueryFor(property);
                if (query != null)
                    queries.Add(query);
            }

            return queries.Count > 0 ? new GroupQuery(queries.ToArray(), this.type) : null;
        }

        public GroupQuery(XmlNode node)
        {
            this.type = (GroupQueryType) Enum.Parse(typeof(GroupQueryType), Helpers.GetXmlAttribute(node, "type"));

            List<QueryFilter> subqueries = new List<QueryFilter>();

            foreach (XmlNode child in node.ChildNodes)
            {
                QueryFilter query = (QueryFilter) SearchMarshalling.FromXmlNode(child);
                if (query != null)
                    subqueries.Add(query);                  
            }

            this.subqueries = subqueries.ToArray();
        }

        public override XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = base.ToXmlNode(doc);

            foreach (QueryFilter child in subqueries)
            {
                XmlNode childNode = child.ToXmlNode(doc);
                if (childNode != null)
                    node.AppendChild(childNode);
            }

            return node;
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
        }

        public override bool Equals(object obj)
        {
            GroupQuery other = obj as GroupQuery;
            if (other == null)
                return false;

            if (other.type != this.type)
                return false;

            int i;
            for (i = 0; i < subqueries.Length; i++)
            {
                if (i >= other.subqueries.Length)
                    return false;

                if (!subqueries[i].Equals(other.subqueries[i]))
                    return false;
            }

            if(i < other.subqueries.Length)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int) type;
        }
    }

    public abstract class CustomFieldQueryBase : QueryFilter
    {
        public readonly CustomFieldDefinition definition;

        public CustomFieldQueryBase(CustomFieldDefinition definition)
        {
            this.definition = definition;
        }

        public override QueryFilter GetSubQueryFor(PropertyNames property)
        {
            return null;
        }
    }

    // This should have been called CustomFieldStringQuery, but is still called CustomFieldQuery
    // for historical reasons (it's known by that name in some saved search XML files).
    public class CustomFieldQuery : CustomFieldQueryBase
    {
        public readonly String query;
        public readonly StringPropertyQuery.PropertyQueryType type;

        public CustomFieldQuery(CustomFieldDefinition definition, String query, StringPropertyQuery.PropertyQueryType type)
            : base(definition)
        {
            this.query = query;
            this.type = type;
        }

        public override bool? Match(IXenObject o)
        {
            String value = CustomFieldsManager.GetCustomFieldValue(o, definition) as String;
            if (value == null)
                return false;

            if (query == null)
                return true;

            return StringPropertyQuery.MatchString(value, query.ToLower(), type, false);
        }

        public CustomFieldQuery(XmlNode node)
            : base(new CustomFieldDefinition(node.FirstChild))
        {
            this.query = Helpers.GetXmlAttribute(node, "query");
            this.type = (StringPropertyQuery.PropertyQueryType)
                Enum.Parse(typeof(StringPropertyQuery.PropertyQueryType), Helpers.GetXmlAttribute(node, "type"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            node.AppendChild(definition.ToXmlNode(doc));

            SearchMarshalling.AddAttribute(doc, node, "query", query);
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
        }

        public override bool Equals(object obj)
        {
            CustomFieldQuery other = obj as CustomFieldQuery;
            if (other == null)
                return false;

            return definition.Equals(other.definition)
                && query.Equals(other.query)
                && type == other.type;
        }

        public override int GetHashCode()
        {
            return definition.GetHashCode() * query.GetHashCode() * type.GetHashCode();
        }
    }

    public class CustomFieldDateQuery : CustomFieldQueryBase
    {
        public readonly DateTime query;
        public readonly DatePropertyQuery.PropertyQueryType type;

        public CustomFieldDateQuery(CustomFieldDefinition definition, DateTime query, DatePropertyQuery.PropertyQueryType type)
            : base(definition)
        {
            this.query = query;
            this.type = type;
        }

        public override bool? Match(IXenObject o)
        {
            object value = CustomFieldsManager.GetCustomFieldValue(o, definition);
            if (!(value is DateTime))
                return false;

            return DatePropertyQuery.MatchDate((DateTime)value, query, type, DateTime.Now);
        }

        public CustomFieldDateQuery(XmlNode node)
            : base(new CustomFieldDefinition(node.FirstChild))
        {
            string queryString = Helpers.GetXmlAttribute(node, "query");
            this.query = DateTime.ParseExact(queryString, "yyyyMMdd", CultureInfo.InvariantCulture);
            this.type = (DatePropertyQuery.PropertyQueryType)
                Enum.Parse(typeof(DatePropertyQuery.PropertyQueryType), Helpers.GetXmlAttribute(node, "type"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            node.AppendChild(definition.ToXmlNode(doc));

            SearchMarshalling.AddAttribute(doc, node, "query", query.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
        }

        public override bool Equals(object obj)
        {
            CustomFieldDateQuery other = obj as CustomFieldDateQuery;
            if (other == null)
                return false;

            return definition.Equals(other.definition)
                && query.Equals(other.query)
                && type == other.type;
        }

        public override int GetHashCode()
        {
            return definition.GetHashCode() * query.GetHashCode() * type.GetHashCode();
        }
    }

    public abstract class PropertyQuery<T> : QueryFilter
    {
        public readonly PropertyNames property;
        private readonly PropertyAccessor propertyAccessor;

        internal readonly bool nullProtect; 

        protected PropertyQuery(PropertyNames property)
            : this(property, true)
        {
        }

        protected PropertyQuery(PropertyNames property, bool nullProtect)
        {
            this.property = property;
            this.propertyAccessor = PropertyAccessors.Get(property);
            this.nullProtect = nullProtect;
        }

        protected PropertyQuery(XmlNode node) 
            : this(node, true)
        {
        }

        protected PropertyQuery(XmlNode node, bool nullProtect) 
        {
            this.property = (PropertyNames)Enum.Parse(typeof(PropertyNames), Helpers.GetXmlAttribute(node, "property"));
            this.propertyAccessor = PropertyAccessors.Get(property);
            this.nullProtect = nullProtect;
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            SearchMarshalling.AddAttribute(doc, node, "property", property.ToString());
        }

        public abstract bool? MatchProperty(T o);

        public override bool? Match(IXenObject o)
        {
            Object value = propertyAccessor(o);

            if (!(value is T) && nullProtect)
                return false;
            
            return MatchProperty((T)value);
        }

        public override QueryFilter GetSubQueryFor(PropertyNames property)
        {
            return this.property == property ? this : null;
        }

        public override bool Equals(object obj)
        {
            PropertyQuery<T> other = obj as PropertyQuery<T>;
            if (other == null)
                return false;

            return other.property == this.property;
        }

        public override int GetHashCode()
        {
            return (int)property;
        }
    }
    
    public abstract class RecursivePropertyQuery<T> : PropertyQuery<T>
    {
        public readonly QueryFilter subQuery;

        public RecursivePropertyQuery(PropertyNames property, QueryFilter subQuery)
            : base(property)
        {
            this.subQuery = subQuery;
        }

        public RecursivePropertyQuery(XmlNode node)
            : base(node)
        {
            subQuery = (QueryFilter)SearchMarshalling.FromXmlNode(node.FirstChild);
        }

        public override XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = base.ToXmlNode(doc);
            XmlNode innerNode = subQuery.ToXmlNode(doc);
            node.AppendChild(innerNode);
            return node;
        }
    }

    public class RecursiveXMOPropertyQuery<T> : RecursivePropertyQuery<T> where T : XenObject<T>
    {
        public RecursiveXMOPropertyQuery(PropertyNames property, QueryFilter subQuery)
            : base(property, subQuery)
        { }

        public RecursiveXMOPropertyQuery(XmlNode node)
            : base(node)
        { }

        public override bool? MatchProperty(T o)
        {
            return subQuery.Match(o);
        }
    }

    public class RecursiveXMOListPropertyQuery<T> : RecursivePropertyQuery<List<T>> where T : XenObject<T>
    {
        public RecursiveXMOListPropertyQuery(PropertyNames property, QueryFilter subQuery)
            : base(property, subQuery)
        { }

        public RecursiveXMOListPropertyQuery(XmlNode node)
            : base(node)
        { }

        // Return true if *any* of the items in the list matches the inner query.
        // If matching for all the items on the list is indeterminate, we return
        // indeterminate (null) (e.g., the subquery is still "Select a filter...").
        // But if the list is empty we return false (e.g., filter by Server and the
        // item doesn't use a Server).
        public override bool? MatchProperty(List<T> list)
        {
            bool seenFalse = false;
            bool seenNull = false;
            foreach (T o in list)
            {
                if (o != null)
                {
                    bool? b = subQuery.Match(o);
                    if (b == true)
                        return true;
                    else if (b == false)
                        seenFalse = true;
                    else
                        seenNull = true;
                }
            }
            if (seenFalse)
                return false;
            else if (seenNull)
                return null;
            else
                return false;
        }
    }

    public class StringPropertyQuery : PropertyQuery<String>
    {
        public enum PropertyQueryType { exactmatch, contains, startswith, endswith, notcontain };

        public readonly String query;
        public readonly PropertyQueryType type;
        internal readonly bool caseSensitive;

        public StringPropertyQuery(PropertyNames property, String query, PropertyQueryType type, bool caseSensitive)
            : base(property)
        {
            this.query = query;
            this.type = type;
            this.caseSensitive = caseSensitive;
        }

        public StringPropertyQuery(XmlNode node) 
            : base(node)
        {
            this.caseSensitive = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "casesensitive"));
            this.query = Helpers.GetXmlAttribute(node, "query");
            this.type = (PropertyQueryType)Enum.Parse(typeof(PropertyQueryType), Helpers.GetXmlAttribute(node, "type"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "casesensitive", caseSensitive);
            SearchMarshalling.AddAttribute(doc, node, "query", query);
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
        }

        public override bool? MatchProperty(String parameter)
        {
            return MatchString(parameter, query, type, caseSensitive);
        }

        public static bool MatchString(String candidate, String query,
            PropertyQueryType type, bool caseSensitive)
        {
            if (String.IsNullOrEmpty(query))
                return true;

            String stringValue = caseSensitive ? candidate : candidate.ToLowerInvariant();
            String queryValue = caseSensitive ? query : query.ToLowerInvariant();

            switch (type)
            {
                case PropertyQueryType.contains:
                    return stringValue.Contains(queryValue);

                case PropertyQueryType.startswith:
                    return stringValue.StartsWith(queryValue);

                case PropertyQueryType.endswith:
                    return stringValue.EndsWith(queryValue);

                case PropertyQueryType.exactmatch:
                    return stringValue.Equals(queryValue);

                case PropertyQueryType.notcontain:
                    return !stringValue.Contains(queryValue);

                default:
                    return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            StringPropertyQuery other = obj as StringPropertyQuery;
            if (other == null)
                return false;

            return other.caseSensitive == this.caseSensitive && other.query == this.query && other.type == this.type;
        }

        public override int GetHashCode()
        {
            return query.GetHashCode();
        }
    }

    public class DatePropertyQuery : PropertyQuery<DateTime>
    {
        public enum PropertyQueryType { today, yesterday, thisweek, lastweek, before, after, exact };

        public readonly DateTime query;
        public readonly PropertyQueryType type;

        // For testing, it is necessary to be able to adjust the value of "now"
        private DateTime? pretendNow;

        public DateTime? PretendNow
        {
            set { pretendNow = value; }
        }

        public DateTime Now
        {
            get { return pretendNow.HasValue ? pretendNow.Value : DateTime.Now; }
        }

        public DatePropertyQuery(PropertyNames property, DateTime query, PropertyQueryType type)
            : base(property)
        {
            this.query = query;
            this.type = type;
            this.pretendNow = null;
        }

        public DatePropertyQuery(XmlNode node)
            : base(node)
        {
            string queryString = Helpers.GetXmlAttribute(node, "query");
            if (queryString.Length == 8)  // new style
                this.query = DateTime.ParseExact(queryString, "yyyyMMdd", CultureInfo.InvariantCulture);
            else  // old style
                this.query = TimeUtil.ParseISO8601DateTime(queryString);
            this.type = (PropertyQueryType)Enum.Parse(typeof(PropertyQueryType), Helpers.GetXmlAttribute(node, "type"));
            this.pretendNow = null;
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "query",  query.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
        }

        // Notes on the matching:
        // 1) The query is in local time, but the value is in UTC and needs to be
        //    translated to local time for comparison.
        // 2) Because the user can only specify a date, times never play any part
        //    in the matching.
        // 3) "Last week" means the day is between six days ago and today inclusive.
        // 4) "Before" and "after" include the day itself.
        public override bool? MatchProperty(DateTime value)
        {
            return MatchDate(value, query, type, Now);
        }

        public static bool? MatchDate(DateTime value, DateTime query, PropertyQueryType type, DateTime now)
        {
            DateTime valueLocal = value.ToLocalTime();

            switch (type)
            {
                case PropertyQueryType.today:
                    return now.Year == valueLocal.Year
                        && now.DayOfYear == valueLocal.DayOfYear;

                case PropertyQueryType.yesterday:
                    if (valueLocal.Year == now.Year)
                        return (valueLocal.DayOfYear == now.DayOfYear - 1);
                    else if (valueLocal.Year == now.Year - 1)
                        return (now.DayOfYear == 1 && valueLocal.DayOfYear == DaysInYear(valueLocal.Year));
                    else
                        return false;

                case PropertyQueryType.thisweek:
                    if (valueLocal.Year == now.Year)
                    {
                        int diff = now.DayOfYear - valueLocal.DayOfYear;
                        return (diff >= 0 && diff < 7);
                    }
                    else if (valueLocal.Year == now.Year - 1)
                    {
                        int diff = now.DayOfYear + DaysInYear(valueLocal.Year) - valueLocal.DayOfYear;
                        return (diff >= 0 && diff < 7);
                    }
                    else
                        return false;

                case PropertyQueryType.lastweek:
                    if (valueLocal.Year == now.Year)
                    {
                        int diff = now.DayOfYear - valueLocal.DayOfYear;
                        return (diff >= 7 && diff < 14);
                    }
                    else if (valueLocal.Year == now.Year - 1)
                    {
                        int diff = now.DayOfYear + DaysInYear(valueLocal.Year) - valueLocal.DayOfYear;
                        return (diff >= 7 && diff < 14);
                    }
                    else
                        return false;

                case PropertyQueryType.before:
                    return valueLocal.Date <= query.Date;

                case PropertyQueryType.after:
                    return valueLocal.Date >= query.Date;

                case PropertyQueryType.exact:
                    return valueLocal.Date == query.Date;

                default:
                    return false;
            }
        }

        private static int DaysInYear(int year)
        {
            return (DateTime.IsLeapYear(year) ? 366 : 365);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DatePropertyQuery other = obj as DatePropertyQuery;
            if (other == null)
                return false;

            return other.type == this.type && other.query == this.query;
        }

        public override int GetHashCode()
        {
            return query.GetHashCode();
        }
    }

    public class EnumPropertyQuery<T> : PropertyQuery<T>
    {
        public readonly T query;
        public readonly bool equals;

        public EnumPropertyQuery(PropertyNames property, T query, bool equals)
            : base(property)
        {
            this.query = query;
            this.equals = equals;

            System.Diagnostics.Trace.Assert(typeof(T).IsEnum);
        }

        public EnumPropertyQuery(XmlNode node)
            : base(node)
        {
            // Special case: "Type is SR" in legacy searches is now interpreted as
            // "Type is Remote SR". (The "type is" filter can't search for multiple types).
            if (typeof(T) == typeof(ObjectTypes) && Helpers.GetXmlAttribute(node, "query") == "SR")
                this.query = (T)Enum.Parse(typeof(T), "RemoteSR");
            else
                this.query = (T)Enum.Parse(typeof(T), Helpers.GetXmlAttribute(node, "query"));
            this.equals = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "equals"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "equals", equals);
            SearchMarshalling.AddAttribute(doc, node, "query", query.ToString());
        }

        public override bool? MatchProperty(T value)
        {
            return value.Equals(query) == equals;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            EnumPropertyQuery<T> other = obj as EnumPropertyQuery<T>;
            if (other == null)
                return false;

            return other.query.Equals(this.query) && other.equals == this.equals;
        }

        public override int GetHashCode()
        {
            return query.GetHashCode();
        }
    }

    public class ValuePropertyQuery : PropertyQuery<String>
    {
        public readonly String query;
        public readonly bool equals;

        public ValuePropertyQuery(PropertyNames property, String query, bool equals)
            : base(property)
        {
            this.query = query;
            this.equals = equals;
        }

        public ValuePropertyQuery(XmlNode node)
            : base(node)
        {
            this.query = Helpers.GetXmlAttribute(node, "query");
            this.equals = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "equals"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "equals", equals);
            SearchMarshalling.AddAttribute(doc, node, "query", query.ToString());
        }

        public override bool? MatchProperty(String value)
        {
            return value.Equals(query) == equals;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            ValuePropertyQuery other = obj as ValuePropertyQuery;
            if (other == null)
                return false;

            return other.equals == this.equals && other.query == this.query;
        }

        public override int GetHashCode()
        {
            return query.GetHashCode();
        }
    }

    public class XenModelObjectPropertyQuery<T> : PropertyQuery<T> where T : XenObject<T>
    {
        /// <summary>
        /// May be null, in which case this matches nothing.
        /// </summary>
        public readonly T query;

        public readonly bool equals;

        public XenModelObjectPropertyQuery(PropertyNames property, T query, bool equals)
            : base(property)
        {
            this.query = query;
            this.equals = equals;
        }

        public XenModelObjectPropertyQuery(XmlNode node)
            : base(node)
        {
            XmlAttribute uuid_node = node.Attributes["uuid"];
            if (uuid_node == null)
            {
                // Just for backwards compat.
                query = XenConnection.FindByRef(new XenRef<T>(Helpers.GetXmlAttribute(node, "query")));
            }
            else
            {
                query = XenConnection.FindByUUIDXenObject<T>(uuid_node.Value);
            }
            equals = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "equals"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "equals", equals);
            SearchMarshalling.AddAttribute(doc, node, "uuid", GetUUID());
        }

        private string GetUUID()
        {
            return query == null ? "invalid" : Helpers.GetUuid(query);
        }

        public override bool? MatchProperty(T value)
        {
            if (query == null)
                return false;

            return (Helpers.GetUuid(value) == Helpers.GetUuid(query)) == equals;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            XenModelObjectPropertyQuery<T> other = obj as XenModelObjectPropertyQuery<T>;
            if (other == null)
                return false;

            return other.equals == this.equals || Helpers.GetUuid(other.query) == Helpers.GetUuid(query);
        }

        public override int GetHashCode()
        {
            return query == null ? 0 : query.GetHashCode();
        }
    }

    public abstract class ListContainsQuery<T> : PropertyQuery<List<T>>
    {
        public readonly bool contains;

        protected ListContainsQuery(PropertyNames property, bool contains)
            : base (property)
        {
            this.contains = contains;
        }

        protected ListContainsQuery(XmlNode node)
            : base (node)
        {
            this.contains = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "contains"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "contains", contains);
        }

        public override sealed bool? MatchProperty(List<T> value)
        {
            return contains == value.Exists(delegate(T t)
                {
                    return MatchItem(t);
                });
        }

        protected abstract bool MatchItem(T t);

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            ListContainsQuery<T> other = obj as ListContainsQuery<T>;
            if (other == null)
                return false;

            return other.contains == this.contains;
        }

        public override abstract int GetHashCode();
    }

    public class XenModelObjectListContainsQuery<T> : ListContainsQuery<T> where T : XenObject<T>
    {
        public readonly String queryUUID;

        public XenModelObjectListContainsQuery(PropertyNames property, T query, bool contains)
            : base(property, contains)
        {
            this.queryUUID = Helpers.GetUuid(query);
        }

        public XenModelObjectListContainsQuery(XmlNode node)
            : base(node)
        {
            this.queryUUID = Helpers.GetXmlAttribute(node, "uuid");
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "uuid", queryUUID);
        }

        protected override bool  MatchItem(T t)
        {
            return t != null && queryUUID == Helpers.GetUuid(t);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            XenModelObjectListContainsQuery<T> other = obj as XenModelObjectListContainsQuery<T>;
            if (other == null)
                return false;

            return other.queryUUID == this.queryUUID;
        }

        public override int GetHashCode()
        {
            return queryUUID.GetHashCode();
        }
    }

    public class XenModelObjectListContainsNameQuery<T> : ListContainsQuery<T> where T : XenObject<T>
    {
        public readonly string query;
        public readonly StringPropertyQuery.PropertyQueryType type;

        public XenModelObjectListContainsNameQuery(PropertyNames property, StringPropertyQuery.PropertyQueryType type, String query)
            : base(property, true)
        {
            this.query = query;
            this.type = type;
        }

        public XenModelObjectListContainsNameQuery(XmlNode node)
            : base(node)
        {
            this.query = Helpers.GetXmlAttribute(node, "query");
            this.type = (StringPropertyQuery.PropertyQueryType)
                Enum.Parse(typeof(StringPropertyQuery.PropertyQueryType),
                           Helpers.GetXmlAttribute(node, "type"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "query", query);
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
        }

        protected override bool MatchItem(T value)
        {
            return StringPropertyQuery.MatchString(Helpers.GetName(value), query, type, false);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            XenModelObjectListContainsNameQuery<T> other = obj as XenModelObjectListContainsNameQuery<T>;
            if (other == null)
                return false;

            return other.query == this.query &&
                   other.type == this.type;
        }

        public override int GetHashCode()
        {
            return query.GetHashCode();
        }
    }

    public class StringListContainsQuery : ListContainsQuery<String>
    {
        public readonly String query;

        public StringListContainsQuery(PropertyNames property, String query, bool contains)
            : base(property, contains)
        {
            this.query = query;
        }

        public StringListContainsQuery(XmlNode node)
            : base(node)
        {
            this.query = Helpers.GetXmlAttribute(node, "query");
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "query", query);
        }

        protected override bool MatchItem(String value)
        {
            return value != null && value.Equals(query);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            StringListContainsQuery other = obj as StringListContainsQuery;
            if (other == null)
                return false;

            return other.query.Equals(this.query);
        }

        public override int GetHashCode()
        {
            return query.GetHashCode();
        }
    }

    public class ListEmptyQuery<T> : PropertyQuery<List<T>>
    {
        public readonly bool empty;

        public ListEmptyQuery(PropertyNames property, bool empty)
            : base(property)
        {
            this.empty = empty;
        }

        public ListEmptyQuery(XmlNode node)
            : base(node)
        {
            this.empty = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "empty"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "empty", empty);
        }

        public override sealed bool? MatchProperty(List<T> value)
        {
            return ((value.Count == 0) == empty);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            ListEmptyQuery<T> other = obj as ListEmptyQuery<T>;
            if (other == null)
                return false;

            return other.empty == this.empty;
        }

        public override int GetHashCode()
        {
            return empty.GetHashCode();
        }
    }

    public class IPAddressQuery : ListContainsQuery<ComparableAddress>
    {
        public readonly ComparableAddress address;

        public IPAddressQuery(PropertyNames property, ComparableAddress address)
            : base(property, true)
        {
            this.address = address;
        }

        public IPAddressQuery(XmlNode node)
            : base(node)
        {
            if (!ComparableAddress.TryParse(Helpers.GetXmlAttribute(node, "address"), true, true, out address))
                throw new InvalidOperationException(String.Format("'{0}' is not a valid specification", address));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "address", address.ToString());
        }

        protected override bool MatchItem(ComparableAddress t)
        {
            return address.Equals(t);
        }

        public override int GetHashCode()
        {
            return address.GetHashCode();
        }
    }

    public class BooleanQuery : PropertyQuery<bool>
    {
        public readonly bool query;

        public BooleanQuery(PropertyNames property, bool query)
            : base(property)
        {
            this.query = query;
        }

        public BooleanQuery(XmlNode node)
            : base(node)
        {
            this.query = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "query"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "query", query);
        }

        public override bool? MatchProperty(bool o)
        {
            return query == o;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            BooleanQuery other = obj as BooleanQuery;
            if (other == null)
                return false;

            return query == other.query;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class NullQuery<T> : PropertyQuery<T> where T : XenObject<T>
    {
        public readonly bool query;

        public NullQuery(PropertyNames property, bool query)
            : base(property, false)
        {
            this.query = query;
        }

        public NullQuery(XmlNode node) 
            : base(node, false)
        {
            this.query = SearchMarshalling.ParseBool(Helpers.GetXmlAttribute(node, "query"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "query", query);
        }

        public override bool? MatchProperty(T o)
        {
            return ((o == null) == query);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            NullQuery<T> other = obj as NullQuery<T>;
            if (other == null)
                return false;

            return query == other.query;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class IntPropertyQuery : PropertyQuery<long>
    {
        public enum PropertyQueryType { exactmatch, gt, lt };

        public readonly long query;
        public readonly PropertyQueryType type;

        public IntPropertyQuery(PropertyNames property, long query, PropertyQueryType type)
            : base(property)
        {
            this.query = query;
            this.type = type;
        }

        public IntPropertyQuery(XmlNode node) 
            : base(node)
        {
            this.query = Int64.Parse(Helpers.GetXmlAttribute(node, "query"));
            this.type = (PropertyQueryType)Enum.Parse(typeof(PropertyQueryType), Helpers.GetXmlAttribute(node, "type"));
        }

        protected override void AddXmlAttributes(XmlDocument doc, XmlNode node)
        {
            base.AddXmlAttributes(doc, node);

            SearchMarshalling.AddAttribute(doc, node, "query", query.ToString());
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
        }

        public override bool? MatchProperty(long o)
        {
            switch (type)
            {
                case PropertyQueryType.exactmatch:
                    return o == query;

                case PropertyQueryType.gt:
                    return o > query;

                case PropertyQueryType.lt:
                    return o < query;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            IntPropertyQuery other = obj as IntPropertyQuery;
            if (other == null)
                return false;

            return other.query == this.query && other.type == this.type;
        }

        public override int GetHashCode()
        {
            return (int)query;
        }
    }
}
