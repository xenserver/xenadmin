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

using System;
using XenAPI;

namespace XenAdmin.Core
{
    internal enum RepoType
    {
        Base,
        EarlyAccess,
        Normal,
        DevTeam,
        Internal
    }

    internal class RepoDescriptor : IEquatable<RepoDescriptor>
    {
        public static readonly RepoDescriptor BaseRepo = new RepoDescriptor(RepoType.Base);
        public static readonly RepoDescriptor NormalRepo = new RepoDescriptor(RepoType.Normal);
        public static readonly RepoDescriptor EarlyAccessRepo = new RepoDescriptor(RepoType.EarlyAccess);
        public static readonly RepoDescriptor DevTeamRepo = new RepoDescriptor(RepoType.DevTeam);
        public static readonly RepoDescriptor InternalRepo = new RepoDescriptor(RepoType.Internal);

        /// <summary>
        /// Note that the Base Repo is not selectable
        /// </summary>
        public static RepoDescriptor[] AllRepos =
        {
            NormalRepo,
            EarlyAccessRepo,
            DevTeamRepo,
            InternalRepo
        };

        public RepoDescriptor(RepoType repoType)
        {
            RepoType = repoType;
            BinUrl = AssignBinUrl(repoType);
            SourceUrl = AssignSourceUrl(repoType);
            Key = AssignKey(repoType);
            FriendlyName = AssignFriendlyName(repoType);
        }

        public RepoType RepoType { get; }

        public string BinUrl { get; }

        public string SourceUrl { get; }

        public string Key { get; }

        public string FriendlyName { get; }

        public bool MatchesRepository(Repository repo)
        {
            return repo != null && (Key == repo.name_label || BinUrl == repo.binary_url);
        }

        public bool Equals(RepoDescriptor other)
        {
            //xapi does not allow duplicate name_label or binary_url
            return other != null && (Key == other.Key || BinUrl == other.BinUrl);
        }

        private string AssignBinUrl(RepoType repoType)
        {
            switch (repoType)
            {
                case RepoType.Base:
                    return Registry.GetYumRepoBaseBin() ?? BrandManager.YumRepoBaseBin;
                case RepoType.EarlyAccess:
                    return Registry.GetYumRepoEarlyAccessBin() ?? BrandManager.YumRepoEarlyAccessBin;
                case RepoType.Normal:
                    return Registry.GetYumRepoNormalBin() ?? BrandManager.YumRepoNormalBin;
                case RepoType.DevTeam:
                    return Registry.GetYumRepoDevTeamBin();
                case RepoType.Internal:
                    return Registry.GetYumRepoInternalBin();
                default:
                    throw new ArgumentOutOfRangeException(nameof(repoType), repoType, null);
            }
        }

        private string AssignSourceUrl(RepoType repoType)
        {
            switch (repoType)
            {
                case RepoType.Base:
                    return Registry.GetYumRepoBaseSource() ?? BrandManager.YumRepoBaseSource;
                case RepoType.EarlyAccess:
                    return Registry.GetYumRepoEarlyAccessSource() ?? BrandManager.YumRepoEarlyAccessSource;
                case RepoType.Normal:
                    return Registry.GetYumRepoNormalSource() ?? BrandManager.YumRepoNormalSource;
                case RepoType.DevTeam:
                    return Registry.GetYumRepoDevTeamSource();
                case RepoType.Internal:
                    return Registry.GetYumRepoInternalSource();
                default:
                    throw new ArgumentOutOfRangeException(nameof(repoType), repoType, null);
            }
        }

        private string AssignKey(RepoType repoType)
        {
            switch (repoType)
            {
                case RepoType.Base:
                    return "base_repo";
                case RepoType.EarlyAccess:
                    return "early_access_repo";
                case RepoType.Normal:
                    return "normal_repo";
                case RepoType.DevTeam:
                    return "dev_team_repo";
                case RepoType.Internal:
                    return "internal_repo";
                default:
                    throw new ArgumentOutOfRangeException(nameof(repoType), repoType, null);
            }
        }

        private string AssignFriendlyName(RepoType repoType)
        {
            switch (repoType)
            {
                case RepoType.Base:
                    return Messages.YUM_REPO_BASE;
                case RepoType.EarlyAccess:
                    return Messages.YUM_REPO_EARLY_ACCESS;
                case RepoType.Normal:
                    return Messages.YUM_REPO_NORMAL;
                case RepoType.DevTeam:
                    return "Dev Team";
                case RepoType.Internal:
                    return "Internal";
                default:
                    throw new ArgumentOutOfRangeException(nameof(repoType), repoType, null);
            }
        }
    }
}
