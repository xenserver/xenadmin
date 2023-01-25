/* Copyright (c) Cloud Software Group Holdings, Inc. 
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

using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class PoolAdvancedEditPage : UserControl, IEditPage
    {
        private Pool _pool;

        public PoolAdvancedEditPage()
        {
            InitializeComponent();
            Text = Messages.ADVANCED_OPTIONS;
        }

        public string SubText => checkBoxCompression.Checked ? Messages.ENABLED_MIGRATION_COMPRESSION : Messages.DISABLED_MIGRATION_COMPRESSION;

        public Image Image => Images.StaticImages._002_Configure_h32bit_16;

        public bool HasChanged => checkBoxCompression.Checked != _pool.migration_compression;

        public bool ValidToSave => true;

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            if (clone is Pool pool)
            {
                _pool = pool;
                checkBoxCompression.Checked = pool.migration_compression;
            }
        }

        public AsyncAction SaveSettings()
        {
            var msg = string.Format(Messages.ACTION_ENABLE_MIGRATION_COMPRESSION, _pool.Name());
            
            return new DelegatedAsyncAction(_pool.Connection, msg, msg, null,
                delegate (Session session) { Pool.set_migration_compression(session, _pool.opaque_ref,checkBoxCompression.Checked); },
                true,
                "pool.set_migration_compression"
            );
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void HideLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }
    }
}
