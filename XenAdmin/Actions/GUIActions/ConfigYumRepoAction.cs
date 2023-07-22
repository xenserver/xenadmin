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

namespace XenAdmin.Actions.GUIActions
{
    internal class ConfigYumRepoAction : AsyncAction
    {
        private readonly RepoDescriptor _repoToEnable;
        private readonly Repository _introducedRepo;
        private readonly List<Repository> _oldEnabledRepos;
        private readonly Pool _pool;

        public ConfigYumRepoAction(IXenConnection connection, RepoDescriptor repoDescriptor)
            : base(connection, string.Empty)
        {
            Title = Description = string.Format(Messages.YUM_REPO_ACTION_CONFIG_TITLE, connection.Name, repoDescriptor.FriendlyName);

            _repoToEnable = repoDescriptor;

            _introducedRepo = connection.Cache.Repositories.FirstOrDefault(r => _repoToEnable.MatchesRepository(r));

            _pool = Helpers.GetPoolOfOne(Connection);

            _oldEnabledRepos = (from XenRef<Repository> repoRef in _pool.repositories
                let repo = Connection.Resolve(repoRef)
                where !_repoToEnable.MatchesRepository(repo)
                select repo).ToList();

            #region RBAC

            if (_oldEnabledRepos.Count > 0)
                ApiMethodsToRoleCheck.Add("Pool.remove_repository");

            if (_introducedRepo == null)
                ApiMethodsToRoleCheck.Add("Repository.introduce");

            ApiMethodsToRoleCheck.Add("Pool.add_repository");

            #endregion
        }

        protected override void Run()
        {
            foreach (var repo in _oldEnabledRepos)
            {
                var repoName = RepoDescriptor.AllRepos.FirstOrDefault(r => r.MatchesRepository(repo))?.FriendlyName ?? string.Empty;
                Description = string.Format(Messages.YUM_REPO_ACTION_CONFIG_DESCRIPTION_DISABLE, repoName);
                Pool.remove_repository(Session, _pool.opaque_ref, repo.opaque_ref);
            }

            XenRef<Repository> repoRef;

            if (_introducedRepo == null)
            {
                var baseRepo = RepoDescriptor.BaseRepo;

                if (!Connection.Cache.Repositories.Any(r => baseRepo.MatchesRepository(r)))
                {
                    Description = string.Format(Messages.YUM_REPO_ACTION_CONFIG_DESCRIPTION_INTRODUCE, baseRepo.FriendlyName);

                    Repository.introduce(Session, baseRepo.Key, baseRepo.FriendlyName, baseRepo.BinUrl, baseRepo.SourceUrl, false, "");
                }

                Description = string.Format(Messages.YUM_REPO_ACTION_CONFIG_DESCRIPTION_INTRODUCE, _repoToEnable.FriendlyName);

                repoRef = Repository.introduce(Session, _repoToEnable.Key, _repoToEnable.FriendlyName,
                    _repoToEnable.BinUrl, _repoToEnable.SourceUrl, true, "");
            }
            else
            {
                repoRef = new XenRef<Repository>(_introducedRepo.opaque_ref);
            }

            Description = string.Format(Messages.YUM_REPO_ACTION_CONFIG_DESCRIPTION_ENABLE, _repoToEnable.FriendlyName);

            Pool.add_repository(Session, _pool.opaque_ref, repoRef);

            //wait until the cache has been updated so that the config panel can show
            //the new value if the action was triggered by hitting the Apply button

            Connection.WaitFor(() => _pool.repositories.Contains(repoRef), null);
        }
    }
}
