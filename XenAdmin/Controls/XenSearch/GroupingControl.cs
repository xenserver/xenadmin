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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.CustomFields;
using XenAPI;
using XenAdmin.XenSearch;


namespace XenAdmin.Controls.XenSearch
{
    public partial class GroupingControl : UserControl
    {
        private static readonly List<GroupingType> potentialGroups;
        private static readonly FolderGroupingType folderGroupingType;
        private static readonly List<CustomFieldGroupingType> customFields;
        private Searcher searcher;
        private NonReopeningContextMenuStrip contextMenuStrip;
        private Button lastButtonClicked;

        public static event Action<CustomFieldGroupingType> CustomFieldRemoved;
        public event Action GroupingChanged;

        private const int MAX_GROUPS = 5;
        private const int innerGutter = 6;

        static GroupingControl()
        {
            potentialGroups = new List<GroupingType>();
            XenModelObjectPropertyGroupingType<Pool> poolGroup =
                new XenModelObjectPropertyGroupingType<Pool>(ObjectTypes.AllExcFolders & ~ObjectTypes.Pool, // i.e., all except Pool
                PropertyNames.pool, null);

			var applianceGroup = new XenModelObjectPropertyGroupingType<VM_appliance>(ObjectTypes.VM, PropertyNames.appliance, poolGroup);
            
			XenModelObjectPropertyGroupingType<Host> hostGroup =
                new XenModelObjectPropertyGroupingType<Host>(ObjectTypes.AllExcFolders & ~ObjectTypes.Pool & ~ObjectTypes.Server,
				PropertyNames.host, applianceGroup);
        	
            potentialGroups.Add(poolGroup);
            potentialGroups.Add(hostGroup);
            potentialGroups.Add(new PropertyGroupingType<String>(ObjectTypes.VM, PropertyNames.os_name));
            potentialGroups.Add(new PropertyGroupingType<vm_power_state>(ObjectTypes.VM, PropertyNames.power_state));
            potentialGroups.Add(new PropertyGroupingType<VM.VirtualisationStatus>(ObjectTypes.VM, PropertyNames.virtualisation_status));
            potentialGroups.Add(new PropertyGroupingType<ObjectTypes>(ObjectTypes.AllExcFolders, PropertyNames.type));
            potentialGroups.Add(new XenModelObjectPropertyGroupingType<XenAPI.Network>(ObjectTypes.VM, PropertyNames.networks, poolGroup));
            XenModelObjectPropertyGroupingType<SR> srGroup =
                new XenModelObjectPropertyGroupingType<SR>(ObjectTypes.VM | ObjectTypes.VDI, PropertyNames.storage, poolGroup);
            potentialGroups.Add(srGroup);
            potentialGroups.Add(new XenModelObjectPropertyGroupingType<VDI>(ObjectTypes.VM, PropertyNames.disks, srGroup));
            potentialGroups.Add(new PropertyGroupingType<VM.HA_Restart_Priority>(ObjectTypes.VM, PropertyNames.ha_restart_priority));
            potentialGroups.Add(new BoolGroupingType(ObjectTypes.VM, PropertyNames.read_caching_enabled));
            potentialGroups.Add(new BoolGroupingType(ObjectTypes.VM, PropertyNames.vendor_device_state));
			potentialGroups.Add(applianceGroup);
            potentialGroups.Add(new PropertyGroupingType<String>(ObjectTypes.AllExcFolders, PropertyNames.tags));
            potentialGroups.Add(new XenModelObjectPropertyGroupingType<VM>(
                ObjectTypes.AllExcFolders & ~ObjectTypes.Pool & ~ObjectTypes.Server & ~ObjectTypes.VM,
                PropertyNames.vm, hostGroup));
            potentialGroups.Add(new AllCustomFieldsGroupingType());

            folderGroupingType = new FolderGroupingType();
            customFields = new List<CustomFieldGroupingType>();
            OtherConfigAndTagsWatcher.OtherConfigChanged += OtherConfigWatcher_OtherConfigChanged;
            CustomFieldsManager.CustomFieldsChanged += OtherConfigWatcher_OtherConfigChanged;
        }

        private static void OtherConfigWatcher_OtherConfigChanged()
        {
            List<CustomFieldDefinition> customFieldDefinitions = CustomFieldsManager.GetCustomFields();

            // Add new custom fields
            foreach (CustomFieldDefinition definition in customFieldDefinitions)
                if (!customFields.Exists(cf => cf.definition.Equals(definition)))
                    customFields.Add(new CustomFieldGroupingType(ObjectTypes.AllExcFolders, definition));


            // Remove old ones
            foreach (CustomFieldGroupingType customFieldGroupingType in customFields.ToArray())
                if (!customFieldDefinitions.Exists(cfd => customFieldGroupingType.definition.Equals(cfd)))
                {
                    customFields.Remove(customFieldGroupingType);
                    OnCustomFieldRemoved(customFieldGroupingType);
                }
        }

        private static void OnCustomFieldRemoved(CustomFieldGroupingType groupingType)
        {
            if (CustomFieldRemoved != null)
                CustomFieldRemoved(groupingType);
        }

        private readonly List<Button> groups = new List<Button>(); // Button.tag = GroupType

        public GroupingControl()
        {
            InitializeComponent();

            AddGroup(potentialGroups[0]);

            CustomFieldRemoved += CustomFieldRemovalWatcher_CustomFieldRemoved; 
        }

        private void CustomFieldRemovalWatcher_CustomFieldRemoved(CustomFieldGroupingType groupingType) 
        {
            if (groupingType == null)
                return;
            foreach (Button button in groups.ToArray())
            {
                CustomFieldGroupingType groupType = button.Tag as CustomFieldGroupingType;
                if (groupType == null)
                    continue;
                if (groupType.Equals(groupingType))
                {
                    Remove(button);

                    Setup();
                }
            }
        }

        protected void OnGroupChanged()
        {
            if (GroupingChanged != null)
                GroupingChanged();
        }

        private void searcher_SearchForChanged()
        {
            RemoveUnwantedGroups();

            // If we're searching for folders, RemoveUnwantedGroups() has
            // removed everything. We then add in group by folder which is
            // mandatory.
            if (SearchingForFolders())
                AddGroup(folderGroupingType);
        }

        public Searcher Searcher
        {
            get { return searcher; }
            set
            {
                searcher = value;
                if (searcher != null)
                    searcher.SearchForChanged += searcher_SearchForChanged;
            }
        }

        public Grouping Grouping
        {
            get
            {
                Grouping group = null;

                Button[] buttons = groups.ToArray();

                for (int i = buttons.Length - 1; i >= 0; i--)
                {
                    Button button = buttons[i];

                    GroupingType groupType = button.Tag as GroupingType;
                    if (groupType == null)
                        return null;

                    group = groupType.GetGroup(group);
                }

                return group;
            }

            set
            {
                RemoveAll();

                Grouping grouping = value;

                while (grouping != null)
                {
                    FromGrouping(grouping);

                    grouping = grouping.subgrouping;
                }

                Setup();
            }
        }

        private bool SearchingForFolders()
        {
            return (searcher != null &&
                searcher.QueryScope != null &&
                searcher.QueryScope.WantType(ObjectTypes.Folder));
        }

        private void FromGrouping(Grouping grouping)
        {
            if (folderGroupingType.ForGrouping(grouping))
            {
                AddGroup(folderGroupingType);
                return;
            }

            foreach (GroupingType groupType in potentialGroups)
                if (groupType.ForGrouping(grouping))
                {
                    AddGroup(groupType);
                    return;
                }

            foreach (GroupingType groupType in customFields)
                if (groupType.ForGrouping(grouping))
                {
                    AddGroup(groupType);
                    return;
                }

            // If the custom field doesn't exist, we just create a new one
            CustomFieldGrouping customFieldGrouping = grouping as CustomFieldGrouping;
            if (customFieldGrouping == null)
                return;

            AddGroup(new CustomFieldGroupingType(ObjectTypes.AllExcFolders, customFieldGrouping.definition));
        }

        private void Setup()
        {
            SuspendLayout();

            bool folderSeen = false;
            int offset = 0;
            
            foreach (Button button in groups.ToArray())
            {
                // If the folder button is present, we must remove all other buttons.
                // But the folder button is always first, so it's sufficient just to
                // remove all later buttons.
                if (folderSeen)
                {
                    Remove(button);
                    continue;
                }

                if (button.Tag is FolderGroupingType)
                {
                    folderSeen = true;
                    // The folder button cannot be turned off if we're searching for folders
                    button.Enabled = !SearchingForFolders();
                }

                if (!dragging || draggedButton != button)
                {
                    button.Top = 3;
                    button.Left = offset;
                }

                offset += button.Width + innerGutter;
            }

            AddGroupButton.Left = offset;
            OnGroupChanged();

            AddGroupButton.Enabled = groups.Count < MAX_GROUPS && GetRemainingGroupTypes(null).Count > 0;

            ResumeLayout();
        }

        // Generate a list of valid groups for a button: disallowing already-used groups
        // and also ancestors of earlier buttons and descendants of later buttons.
        // "context" is the button to generate the list for.
        // Pass null for the Add Group button.
        private List<GroupingType> GetRemainingGroupTypes(Button context)
        {
            List<GroupingType> remainingGroupTypes = new List<GroupingType>(potentialGroups);
            foreach(GroupingType customField in customFields)
                remainingGroupTypes.Add(customField);

            // Remove group types which are not relevant to any of the things being searched for.
            foreach (GroupingType gt in remainingGroupTypes.ToArray())
            {
                if (!WantGroupingType(gt))
                    remainingGroupTypes.Remove(gt);
            }

            int posRelativeToContext = -1;  // -1 for before; 0 for context itself; +1 for after
            foreach (Button button in groups)
            {
                if (button == context)
                {
                    posRelativeToContext = 0;
                }
                else if (posRelativeToContext == 0)
                {
                    posRelativeToContext = 1;
                }

                GroupingType groupType = button.Tag as GroupingType;
                if(groupType == null)
                    continue;

                // Remove the button type itself.
                remainingGroupTypes.Remove(groupType);
                // Also if we are still to the left of context, also remove ancestor types of this button;
                // conversely, if we are to the right of context, remove descendant types of this button.
                // Also having Folder on another button precludes all other choices.
                foreach (GroupingType gt in remainingGroupTypes.ToArray())
                {
                    if (posRelativeToContext == -1 && groupType.IsDescendantOf(gt) ||
                        posRelativeToContext == 1 && gt.IsDescendantOf(groupType) ||
                        posRelativeToContext != 0 && groupType is FolderGroupingType)
                    {
                        remainingGroupTypes.Remove(gt);
                    }
                }
            }

            return remainingGroupTypes;
        }

        // Whether to show Folder on a button's drop-down menu
        // As before, pass context==null for the Add Group Button
        private bool ShowFolderOption(Button context)
        {
            // If this is the Add Group Button, we allow the option if there are no existing buttons
            if (context == null)
                return (groups.Count == 0);

            // For normal buttons, if this button already shows Folder, we don't allow it on the menu again
            GroupingType groupType = context.Tag as GroupingType;
            if (groupType is FolderGroupingType)
                return false;

            // Otheriwse we allow it only on the first button
            return (groups.Count != 0 && groups[0] == context);
        }

        private Button NewButton(GroupingType groupType)
        {
            Button button = new Button();
            button.AutoSize = true;
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button.Text = groupType.ToString();
            button.UseVisualStyleBackColor = true;

            button.TextAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.TextBeforeImage;
            button.Padding = new Padding(0, 0, 2, 0);
            button.Image = Properties.Resources.expanded_triangle;
            button.ImageAlign = ContentAlignment.MiddleRight;

            button.Tag = groupType;
            button.Click += new EventHandler(button_Click);
            button.MouseDown += new MouseEventHandler(button_MouseDown);
            button.MouseUp += new MouseEventHandler(button_MouseUp);
            button.MouseMove += new MouseEventHandler(button_MouseMove);

            return button;
        }

        private Button draggedButton = null;
        private Point clickPoint = Point.Empty;
        private int dragOffset = 0;
        private bool dragging = false;

        void button_MouseUp(object sender, MouseEventArgs e)
        {
            bool wasDragging = dragging;

            draggedButton = null;
            dragging = false;

            if(wasDragging)
                Setup();
        }

        void button_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            draggedButton = button;
            clickPoint = button.PointToScreen(e.Location);
            dragOffset = e.X;
        }

        void button_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedButton == null)
                return;

            Point OnScreen = draggedButton.PointToScreen(e.Location);

            if (!dragging && Math.Abs(clickPoint.X - OnScreen.X) < 5)
                return;

            dragging = true;

            draggedButton.BringToFront();

            // Now we need to figure out where to move it,
            // both on the screen and in the internal array of buttons.
            // We need to take into account which buttons we have dragged it past,
            // and which buttons it's allowed to pass given the grouping hierarchy.
            // Note that if buttons are already out of order (legacy configuration),
            // we don't force them back in order, but they can't get any worse.

            GroupingType draggedGroupType = draggedButton.Tag as GroupingType;
            int draggedButtonLeft = draggedButton.Left;
            int draggedButtonRight = draggedButton.Left + draggedButton.Width;
            int draggedButtonIndex = groups.IndexOf(draggedButton);

            int offset = 0;
            int leftBarrier = -1;
            int swapWith = -1;
            int rightBarrier = -1;
            Button[] groupsArray = groups.ToArray();
            foreach (Button candidateButton in groupsArray)
            {
                GroupingType candidateGroupType = candidateButton.Tag as GroupingType;
                int candidateButtonMiddle = offset + (candidateButton.Width / 2);
                int candidateButtonIndex = groups.IndexOf(candidateButton);

                if (draggedGroupType != null && candidateGroupType != null)
                {
                    if (candidateButtonIndex < draggedButtonIndex)
                    {
                        if (draggedGroupType.IsDescendantOf(candidateGroupType))
                            leftBarrier = candidateButtonIndex;
                    }
                    else if (candidateButtonIndex > draggedButtonIndex)
                    {
                        if (candidateGroupType.IsDescendantOf(draggedGroupType))
                            rightBarrier = candidateButtonIndex;
                    }
                }


                if (swapWith == -1 &&
                    candidateButtonMiddle > draggedButtonLeft &&
                    candidateButtonMiddle < draggedButtonRight)
                {
                    swapWith = candidateButtonIndex;
                }

                offset += candidateButton.Width + innerGutter;
            }

            Point InControl = PointToClient(OnScreen);
            int potentialLocation = InControl.X - dragOffset;

            bool movedLeft = (swapWith >= 0 && swapWith < draggedButtonIndex);
            bool movedRight = (swapWith >= 0 && swapWith > draggedButtonIndex);
            bool movedNeither = !movedLeft && !movedRight;
            if (movedRight || movedNeither)
            {
                int maxRight = 0;
                if (rightBarrier >= 0)
                {
                    if (swapWith >= 0 && swapWith >= rightBarrier)
                        swapWith = rightBarrier - 1;
                    maxRight = groupsArray[rightBarrier].Left - draggedButton.Width - innerGutter;
                }
                else if (groupsArray.Length >= 2)
                    maxRight = groupsArray[groupsArray.Length - 2].Right + innerGutter;

                if (potentialLocation > maxRight)
                    potentialLocation = maxRight;
            }
            if (movedLeft || movedNeither)
            {
                int maxLeft = 0;
                if (leftBarrier >= 0)
                {
                    if (swapWith >= 0 && swapWith <= leftBarrier)
                        swapWith = leftBarrier + 1;
                    maxLeft = groupsArray[leftBarrier].Right + innerGutter;
                }

                if (potentialLocation < maxLeft)
                    potentialLocation = maxLeft;
            }

            draggedButton.Left = potentialLocation;
            if (swapWith >= 0 && swapWith != draggedButtonIndex)
            {
                groups.Remove(draggedButton);
                groups.Insert(swapWith, draggedButton);
                Setup();
            }
        }

        void AddItemToMenu(NonReopeningContextMenuStrip menu, string text, object tag, EventHandler clickHandler)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = text;
            menuItem.Tag = tag;
            menuItem.Click += clickHandler;
            menu.Items.Add(menuItem);
        }

        void button_Click(object sender, EventArgs e)
        {
            if (dragging)
                return;

            Button button = sender as Button;
            if (button == null)
                return;

            GroupingType groupType = button.Tag as GroupingType;
            if (groupType == null)
                return;

            if (button != lastButtonClicked)
                contextMenuStrip = new NonReopeningContextMenuStrip();  // new so that it can open immediately
            else if (!contextMenuStrip.CanOpen)
                return;
            else
                contextMenuStrip.Items.Clear();

            AddItemToMenu(contextMenuStrip, Messages.REMOVE_GROUPING, button,
                new EventHandler(removeGroupItem_Click));

            if (ShowFolderOption(button))
            {
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                AddItemToMenu(contextMenuStrip, folderGroupingType.ToString(),
                    new KeyValuePair<Button, GroupingType>(button, folderGroupingType),
                    new EventHandler(changeGroupItem_Click));
            }

            List<GroupingType> remainingGroupTypes = GetRemainingGroupTypes(button);
            if (remainingGroupTypes.Count > 0)
            {
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                foreach (GroupingType remainingGroupType in remainingGroupTypes)
                    AddItemToMenu(contextMenuStrip, remainingGroupType.ToString(),
                        new KeyValuePair<Button, GroupingType>(button, remainingGroupType),
                        new EventHandler(changeGroupItem_Click));
            }

            lastButtonClicked = button;
            contextMenuStrip.Show(this, new Point(button.Left, button.Bottom));

            //Setup();
        }

        void removeGroupItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = sender as ToolStripMenuItem;
            if (menuitem == null)
                return;

            Button button = menuitem.Tag as Button;
            if (button == null)
                return;

            Remove(button);

            Setup();
        }

        private void RemoveAll()
        {
            foreach (Button button in groups.ToArray())
                Remove(button);
        }

        private void Remove(Button button)
        {
            groups.Remove(button);
            Controls.Remove(button);
        }

        // Do we want this grouping type, based on the search-for?
        private bool WantGroupingType(GroupingType gt)
        {
            QueryScope scope = (searcher == null ? null : searcher.QueryScope);
            if (scope == null)
                return true;
            if (scope.WantType(ObjectTypes.Folder))
                return false;  // searching for folder forbids all grouping types (we add group by folder back in separately)
            return (scope.WantAnyOf(gt.AppliesTo));
        }

        private bool WantGroupingType(Button button)
        {
            GroupingType gt = button.Tag as GroupingType;
            return WantGroupingType(gt);
        }

        // Remove groups which don't match the current search-for
        private void RemoveUnwantedGroups()
        {
            foreach (Button button in groups.ToArray())
            {
                if (!WantGroupingType(button))
                    Remove(button);
            }
            Setup();
        }

        void changeGroupItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = sender as ToolStripMenuItem;
            if (menuitem == null)
                return;
             
            if(!(menuitem.Tag is KeyValuePair<Button, GroupingType>))
                return;

            KeyValuePair<Button, GroupingType> kvp = (KeyValuePair<Button, GroupingType>) menuitem.Tag;

            kvp.Key.Tag = kvp.Value;
            kvp.Key.Text = kvp.Value.ToString();

            Setup();
        }

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            if (groups.Count >= MAX_GROUPS)
                return;

            if (AddGroupButton != lastButtonClicked)
                contextMenuStrip = new NonReopeningContextMenuStrip();  // new so that it can open immediately
            else if (!contextMenuStrip.CanOpen)
                return;
            else
                contextMenuStrip.Items.Clear();

            if (ShowFolderOption(null))
            {
                AddItemToMenu(contextMenuStrip, folderGroupingType.ToString(), folderGroupingType,
                    new EventHandler(addGroupItem_Click));
                contextMenuStrip.Items.Add(new ToolStripSeparator());
            }

            foreach (GroupingType groupType in GetRemainingGroupTypes(null))
                AddItemToMenu(contextMenuStrip, groupType.ToString(), groupType,
                    new EventHandler(addGroupItem_Click));

            lastButtonClicked = AddGroupButton;
            contextMenuStrip.Show(this, new Point(AddGroupButton.Left, AddGroupButton.Bottom));
        }

        void addGroupItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
                return;

            GroupingType groupType = menuItem.Tag as GroupingType;
            if (groupType == null)
                return;

            AddGroup(groupType);
        }

        void AddGroup(GroupingType groupType)
        {
            Button button = NewButton(groupType);

            groups.Add(button);
            Controls.Add(button);

            Setup();
        }

        public abstract class GroupingType
        {
            private readonly ObjectTypes appliesTo;

            protected GroupingType(ObjectTypes appliesTo)
            {
                this.appliesTo = appliesTo;
            }

            public ObjectTypes AppliesTo
            {
                get { return appliesTo; }
            }

            public abstract Grouping GetGroup(Grouping subgrouping);

            public abstract bool ForGrouping(Grouping grouping);

            public virtual bool IsDescendantOf(GroupingType gt)
            {
                return false;
            }
        }

        public class FolderGroupingType : GroupingType
        {
            public FolderGroupingType()
                : base(ObjectTypes.AllIncFolders)
            {
            }

            public override Grouping GetGroup(Grouping subgrouping)
            {
                return new FolderGrouping(subgrouping);
            }

            public override bool ForGrouping(Grouping grouping)
            {
                return grouping is FolderGrouping;
            }
            
            public override string ToString()
            {
                return Messages.FOLDER;
            }
        }

        public class PropertyGroupingType<T> : GroupingType
        {
            protected readonly PropertyNames property;
            protected readonly String i18n;

            public PropertyGroupingType(ObjectTypes appliesTo, PropertyNames property)
                : base(appliesTo)
            {
                this.property = property;
                this.i18n = PropertyAccessors.PropertyNames_i18n[property];
            }

            public override string ToString()
            {
                return i18n;
            }

            public override Grouping GetGroup(Grouping subgrouping)
            {
                return new PropertyGrouping<T>(property, subgrouping);
            }

            public override bool ForGrouping(Grouping grouping)
            {
                PropertyGrouping<T> propertyGrouping = grouping as PropertyGrouping<T>;

                if (propertyGrouping == null)
                    return false;

                return propertyGrouping.property == property;
            }
        }

        public class BoolGroupingType : PropertyGroupingType<bool>
        {
            public BoolGroupingType(ObjectTypes appliesTo, PropertyNames property)
                : base(appliesTo, property)
            { }

            public override Grouping GetGroup(Grouping subgrouping)
            {
                return new BoolGrouping(property, subgrouping);
            }

            public override bool ForGrouping(Grouping grouping)
            {
                BoolGrouping propertyGrouping = grouping as BoolGrouping;

                if (propertyGrouping == null)
                    return false;

                return propertyGrouping.property == property;
            }
        }

        public class XenModelObjectPropertyGroupingType<T> 
            : PropertyGroupingType<T> where T : XenObject<T>
        {
            protected readonly GroupingType parent;  // the GroupingType next up in the tree: null for the top of the tree

            public XenModelObjectPropertyGroupingType(ObjectTypes appliesTo, PropertyNames property, GroupingType parent)
                : base(appliesTo, property)
            {
                this.parent = parent;
            }

            public override Grouping GetGroup(Grouping subgrouping)
            {
                return new XenModelObjectPropertyGrouping<T>(property, subgrouping);
            }

            public override bool IsDescendantOf(GroupingType gt)
            {
                return (gt == parent || (parent != null && parent.IsDescendantOf(gt)));
            }
        }

        public class CustomFieldGroupingType : GroupingType
        {
            public readonly CustomFieldDefinition definition;

            public CustomFieldGroupingType(ObjectTypes appliesTo, CustomFieldDefinition definition)
                : base(appliesTo)
            {
                this.definition = definition;
            }

            public override string ToString()
            {
                return definition.Name;
            }

            public override Grouping GetGroup(Grouping subgrouping)
            {
                return new CustomFieldGrouping(definition, subgrouping);
            }

            public override bool ForGrouping(Grouping grouping)
            {
                CustomFieldGrouping customFieldGrouping = grouping as CustomFieldGrouping;
                if (customFieldGrouping == null)
                    return false;

                return customFieldGrouping.definition.Equals(definition);
            }
        }

        public class AllCustomFieldsGroupingType : GroupingType
        {
            public AllCustomFieldsGroupingType()
                : base(ObjectTypes.AllExcFolders)
            {
            }

            public override Grouping GetGroup(Grouping subgrouping)
            {
                return new AllCustomFieldsGrouping(subgrouping);
            }

            public override bool ForGrouping(Grouping grouping)
            {
                return grouping is AllCustomFieldsGrouping;
            }
            
            public override string ToString()
            {
                return Messages.CUSTOM_FIELDS;
            }
        }
    }
}
