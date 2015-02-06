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
using System.Xml;
using System.Windows.Forms;

using XenAdmin.Model;
using XenAdmin.Controls;
using XenAPI;


namespace XenAdmin.TabPages
{
    internal partial class DockerProcessPage : BaseTabPage
    {

        private DockerContainer docker = null;
        private Session session = null;
        private Dictionary<string, string> args = new Dictionary<string, string>();
        private string host = null;
        private readonly ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();

        public DockerProcessPage()
        {
            InitializeComponent();
            listView1.ListViewItemSorter = lvwColumnSorter;
            base.Text = Messages.DOCKER_PROCESS_TAB_TITLE;
        }

        public DockerContainer DockerContainer
        {
            get
            {
                return docker;
            }
            set
            {
                if (value == null) return;

                if (docker == null || !docker.Equals(value))
                {
                    docker = value;
                    if (docker.Connection == null || docker.Connection.Session == null)
                        return;
                    session = docker.Connection.Session;
                    host = XenAPI.Session.get_this_host(session, session.uuid);
                    args["vmuuid"] = docker.Parent.uuid;
                    args["object"] = docker.name_label;
                    listView1.Items.Clear();
                    labelRefresh.Text = Messages.LAST_REFRESH_IN_PROGRESS;
                    // Set timer to 10ms to make the empty form shown promptly and
                    // then fill the form with the contents retrieved from XenServer.
                    timer1.Interval = 10;
                    timer1.Enabled = true;
                }
            }
        }

        private void updateList()
        {
            bool getResult = false;
            string value = "";
            try
            {
                // call plugin "xscontainer" with fn "get_top"
                value = Host.call_plugin(session, host, "xscontainer", "get_top", args);
                getResult = value.ToLower().StartsWith("true");
            }
            catch (Failure)
            {
                // Could not retrieve process info.
                showInvalidInfo();
                return;
            }

            if (getResult)
            {
                // Parse the XML result
                XmlDocument xmlDoc = new System.Xml.XmlDocument();
                try
                {
                    xmlDoc.LoadXml(value.Substring(4));
                }
                catch (Exception)
                {
                    showInvalidInfo();
                    return;
                }

                string pid = "";
                string command = "";
                string cpuTime = "";
                string[] row = { "", "", "" };
                try
                {
                    listView1.SuspendLayout();
                    listView1.Items.Clear();
                    XmlNodeList processList = xmlDoc.GetElementsByTagName("Process");
                    foreach (XmlNode process in processList)
                    {
                        if (process.HasChildNodes)
                        {
                            foreach (XmlNode child in process.ChildNodes)
                            {
                                XmlNode v = child.FirstChild;
                                if (child.Name.Equals("PID"))
                                    pid = v.Value;
                                else if (child.Name.Equals("CMD"))
                                    command = v.Value;
                                else if (child.Name.Equals("TIME"))
                                    cpuTime = v.Value;
                            }

                            row[0] = pid;
                            row[1] = command;
                            row[2] = cpuTime;
                        }
                        listView1.Items.Add(new ListViewItem(row));
                    }
                }
                finally
                {
                    listView1.ResumeLayout();
                    labelRefresh.Text = string.Format(Messages.LAST_REFRESH_SUCCESS, DateTime.Now.ToString("HH:mm:ss"));
                }

            }

        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            updateList();
        }

        private void showInvalidInfo()
        {
            labelRefresh.Text = Messages.LAST_REFRESH_FAIL;
            listView1.Items.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Set timer to 20s as the interval to refresh the processes' info.
            timer1.Interval = 1000 * 20;
            updateList();
        }

        // Sort by column. Refer to the implementation of PhysicalStoragePage.
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            listView1.Sort();
        }
    }
}
