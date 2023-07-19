/* Copyright (c) Cloud Software Group, Inc. 
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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class AddRemoveRolesAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<Role> _toAdd;
        private readonly List<Role> _toRemove;
        private readonly Subject subject;

        public AddRemoveRolesAction(IXenConnection connection, Subject subject, List<Role> newRoles)
            : base(connection,
                string.Format(Messages.AD_ADDING_REMOVING_ROLES_ON, (subject.DisplayName ?? subject.SubjectName ?? subject.subject_identifier).Ellipsise(50)),
                Messages.AD_ADDING_REMOVING_ROLES,
                false)
        {
            var pool = Helpers.GetPool(connection);
            if (pool != null)
                Pool = pool;
            else
                Host = Helpers.GetCoordinator(connection);

            this.subject = subject;

            var serverRoles = Connection.Cache.Roles
                .Where(r => (!Helpers.CloudOrGreater(Connection) || !Helpers.XapiEqualOrGreater_22_5_0(Connection) || !r.is_internal) && r.subroles.Count > 0)
                .ToList();

            _toAdd = serverRoles.Where(role => newRoles.Contains(role) &&
                                               !subject.roles.Contains(new XenRef<Role>(role.opaque_ref))).ToList();

            _toRemove = serverRoles.Where(role => !newRoles.Contains(role) &&
                                                  subject.roles.Contains(new XenRef<Role>(role.opaque_ref))).ToList();

            if (_toAdd.Count > 0)
                ApiMethodsToRoleCheck.Add("subject.add_to_roles");

            if (_toRemove.Count > 0)
                ApiMethodsToRoleCheck.Add("subject.remove_from_roles");
        }

        protected override void Run()
        {
            int count = _toAdd.Count + _toRemove.Count;
            int done = 0;
            var subj = subject.DisplayName ?? subject.SubjectName ?? subject.subject_identifier;

            foreach (Role r in _toAdd)
            {
                log.DebugFormat("Adding role {0} to subject '{1}'.", r.FriendlyName(), subj);
                Subject.add_to_roles(Session, subject.opaque_ref, r.opaque_ref);
                PercentComplete = 100 * ++done / count;
            }

            foreach (Role r in _toRemove)
            {
                log.DebugFormat("Removing role {0} from subject '{1}'.", r.FriendlyName(), subj);
                Subject.remove_from_roles(Session, subject.opaque_ref, r.opaque_ref);
                PercentComplete = 100 * ++done / count;
            }

            Description = Messages.COMPLETED;
        }
    }
}
