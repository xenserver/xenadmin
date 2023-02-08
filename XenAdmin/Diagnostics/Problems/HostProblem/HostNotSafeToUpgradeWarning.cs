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

using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public enum HostNotSafeToUpgradeReason { NotEnoughSpace, VdiPresent, UtilityPartitionPresent, LegacyPartitionTable, Default }

    public class HostNotSafeToUpgradeWarning : WarningWithMoreInfo
    {
        private readonly Host _host;
        private readonly string _shortMessage;

        public HostNotSafeToUpgradeWarning(Check check, Host host, HostNotSafeToUpgradeReason reason)
            : base(check)
        {
            _host = host;
            var version = $"{BrandManager.ProductBrand} {BrandManager.ProductVersionPost82}";
            var newPartitionInfo = string.Format(Messages.NOT_SAFE_TO_UPGRADE_NEW_PARTITION_INFO, version);

            string detail;

            switch (reason)
            {
                case HostNotSafeToUpgradeReason.NotEnoughSpace:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_WARNING, version);
                    break;
                case HostNotSafeToUpgradeReason.VdiPresent:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT_WARNING, version);
                    break;
                case HostNotSafeToUpgradeReason.UtilityPartitionPresent:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION_WARNING, version);
                    break;
                case HostNotSafeToUpgradeReason.LegacyPartitionTable:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE_WARNING, version);
                    break;
                default:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_DEFAULT;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_WARNING, version);
                    break;
            }
            Message = $"{_shortMessage}\n\n{newPartitionInfo}\n\n{detail}";
        }

        public override string Message { get; }

        public override string Title => Description;

        public override string Description => string.Format(Messages.STRING_COLON_SPACE_STRING, _host.name_label, _shortMessage);
    }

    public class HostNotSafeToUpgradeProblem : ProblemWithMoreInfo
    {
        private readonly Host _host;
        private readonly string _shortMessage;

        public HostNotSafeToUpgradeProblem(Check check, Host host, HostNotSafeToUpgradeReason reason)
            : base(check)
        {
            _host = host;
            var version = $"{BrandManager.ProductBrand} {BrandManager.ProductVersionPost82}";
            var newPartitionInfo = string.Format(Messages.NOT_SAFE_TO_UPGRADE_NEW_PARTITION_INFO, version);
            string detail;

            switch (reason)
            {
                case HostNotSafeToUpgradeReason.NotEnoughSpace:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_PROBLEM, version);
                    break;
                case HostNotSafeToUpgradeReason.VdiPresent:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT_PROBLEM, version);
                    break;
                case HostNotSafeToUpgradeReason.UtilityPartitionPresent:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION_PROBLEM, version);
                    break;
                case HostNotSafeToUpgradeReason.LegacyPartitionTable:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE_PROBLEM, version);
                    break;
                default:
                    _shortMessage = Messages.NOT_SAFE_TO_UPGRADE_DEFAULT;
                    detail = string.Format(Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_PROBLEM, version);
                    break;
            }
            Message = $"{_shortMessage}\n\n{newPartitionInfo}\n\n{detail}";
        }

        public override string Message { get; }

        public override string Title => Description;

        public override string Description => string.Format(Messages.STRING_COLON_SPACE_STRING, _host.name_label, _shortMessage);
    }
}
