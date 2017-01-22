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
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace XenAdmin.Commands
{
    /// <summary>
    /// This is used so that the Command property on CommandToolStripButton and CommandToolStripMenuItem can be set with the Designer.
    /// </summary>
    internal partial class CommandEditorControl : UserControl
    {
        public event EventHandler Closed;

        public CommandEditorControl(Type commandType, Command value)
        {
            InitializeComponent();

            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                if (commandType.IsAssignableFrom(type))
                {
                    try
                    {
                        Command c = (Command)Activator.CreateInstance(type);
                        Image image = c.MenuImage ?? c.ToolBarImage;
                        string text = c.GetType().Name;
                        ListViewItem item = new ListViewItem(text);
                        item.Tag = c;

                        if (image != null)
                        {
                            imageList.Images.Add(text, image);
                            item.ImageKey = text;
                        }

                        listView.Items.Add(item);

                        item.Selected = value != null && value.GetType() == c.GetType();
                    }
                    catch (MissingMethodException)
                    {
                    }
                }
            }

            foreach (ColumnHeader ch in listView.Columns)
            {
                ch.Width = -2;
            }

            listView.ListViewItemSorter = new Sorter();
        }

        public Command SelectedCommand
        {
            get
            {
                if (listView.SelectedItems.Count == 1)
                {
                    return (Command)listView.SelectedItems[0].Tag;
                }
                return null;
            }
        }

        private class Sorter : IComparer
        {
            public int Compare(object x, object y)
            {
                ListViewItem xx = (ListViewItem)x;
                ListViewItem yy = (ListViewItem)y;

                Command cx = (Command)xx.Tag;
                Command cy = (Command)yy.Tag;

                return cx.GetType().Name.CompareTo(cy.GetType().Name);
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }
    }
}
