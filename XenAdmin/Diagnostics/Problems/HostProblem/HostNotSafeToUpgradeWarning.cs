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

using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAPI;

namespace XenAdmin.Diagnostics.Problems.HostProblem
{
    public enum HostNotSafeToUpgradeReason { NotEnoughSpace, VdiPresent, UtilityPartitionPresent, LegacyPartitionTable, Default }

    public class HostNotSafeToUpgradeWarning : WarningWithMoreInfo
    {
        private readonly Host _host;
        private readonly HostNotSafeToUpgradeReason _reason;

        public HostNotSafeToUpgradeWarning(Check check, Host host, HostNotSafeToUpgradeReason reason)
            : base(check)
        {
            this._host = host;
            this._reason = reason;
        }

        public override string Title => Description;

        public override string Description => string.Format(Messages.UPDATES_WIZARD_PRECHECK_FAILED, _host.name_label, ShortMessage);

        public override string Message
        {
            get
            {
                var newPartitionInfo = string.Format(Messages.NOT_SAFE_TO_UPGRADE_NEW_PARTITION_INFO, BrandManager.ProductVersionPost82);
                switch (_reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_WARNING, BrandManager.ProductVersionPost82)}";
                    case HostNotSafeToUpgradeReason.VdiPresent:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT_WARNING, BrandManager.ProductVersionPost82)}";
                    case HostNotSafeToUpgradeReason.UtilityPartitionPresent:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION_WARNING, BrandManager.ProductVersionPost82)}";
                    case HostNotSafeToUpgradeReason.LegacyPartitionTable:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE_WARNING, BrandManager.ProductVersionPost82)}";
                    default:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_DEFAULT}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_WARNING, BrandManager.ProductVersionPost82)}";
                }
            }
        }

        private string ShortMessage
        {
            get
            {
                switch (_reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace:
                        return Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE;
                    case HostNotSafeToUpgradeReason.VdiPresent:
                        return Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT;
                    case HostNotSafeToUpgradeReason.UtilityPartitionPresent:
                        return Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION;
                    case HostNotSafeToUpgradeReason.LegacyPartitionTable:
                        return Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE;
                    default:
                        return Messages.NOT_SAFE_TO_UPGRADE_DEFAULT;
                }
            }
        }
    }

    public class HostNotSafeToUpgradeProblem : ProblemWithMoreInfo
    {
        private readonly Host _host;
        private readonly HostNotSafeToUpgradeReason _reason;

        public HostNotSafeToUpgradeProblem(Check check, Host host, HostNotSafeToUpgradeReason reason)
            : base(check)
        {
            this._host = host;
            this._reason = reason;
        }

        public override string Title => Description;

        public override string Description => string.Format(Messages.UPDATES_WIZARD_PRECHECK_FAILED, _host.name_label, ShortMessage);

        public override string Message
        {
            get
            {
                var newPartitionInfo = string.Format(Messages.NOT_SAFE_TO_UPGRADE_NEW_PARTITION_INFO, BrandManager.ProductVersionPost82);
                switch (_reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE_PROBLEM, BrandManager.ProductVersionPost82)}";
                    case HostNotSafeToUpgradeReason.VdiPresent:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT_PROBLEM, BrandManager.ProductVersionPost82)}";
                    case HostNotSafeToUpgradeReason.UtilityPartitionPresent:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION_PROBLEM, BrandManager.ProductVersionPost82)}";
                    case HostNotSafeToUpgradeReason.LegacyPartitionTable:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE_PROBLEM, BrandManager.ProductVersionPost82)}";
                    default:
                        return $"{Messages.NOT_SAFE_TO_UPGRADE_DEFAULT}\n\n{newPartitionInfo}\n\n{string.Format(Messages.NOT_SAFE_TO_UPGRADE_DEFAULT_PROBLEM, BrandManager.ProductVersionPost82)}";
                }
            }
        }

        private string ShortMessage
        {
            get
            {
                switch (_reason)
                {
                    case HostNotSafeToUpgradeReason.NotEnoughSpace:
                        return Messages.NOT_SAFE_TO_UPGRADE_NOT_ENOUGH_SPACE;
                    case HostNotSafeToUpgradeReason.VdiPresent:
                        return Messages.NOT_SAFE_TO_UPGRADE_VDI_PRESENT;
                    case HostNotSafeToUpgradeReason.UtilityPartitionPresent:
                        return Messages.NOT_SAFE_TO_UPGRADE_UTILITY_PARTITION;
                    case HostNotSafeToUpgradeReason.LegacyPartitionTable:
                        return Messages.NOT_SAFE_TO_UPGRADE_LEGACY_PARTITION_TABLE;
                    default:
                        return Messages.NOT_SAFE_TO_UPGRADE_DEFAULT;
                }
            }
        }
    }
}
