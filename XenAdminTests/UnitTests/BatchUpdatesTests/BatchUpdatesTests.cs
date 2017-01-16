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
using NUnit.Framework;
using XenAdmin.Actions;
using Moq;
using XenAdmin.Core;
using XenAPI;
using System.Linq;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class AutomatedUpdatesTests
    {

        private static Mock<Host> GetAMockHost(string productVersion, string buildNumber, List<Pool_patch> applied_patches = null)
        {
            var id = System.Guid.NewGuid().ToString();

            MockObjectManager mom = new MockObjectManager();

            mom.CreateNewConnection(id);

            Mock<Host> master = mom.NewXenObject<Host>(id);
            Mock<Pool> pool = mom.NewXenObject<Pool>(id);
            XenRef<Host> masterRef = new XenRef<Host>("master-ref");
            pool.Setup(p => p.master).Returns(masterRef);

            mom.MockCacheFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Pool>>())).Returns(pool.Object);

            mom.MockConnectionFor(id).Setup(c => c.Resolve(masterRef)).Returns(master.Object);
            mom.MockConnectionFor(id).Setup(c => c.Cache.Hosts).Returns(new Host[] { master.Object });
            mom.MockConnectionFor(id).Setup(c => c.Cache.Pools).Returns(new Pool[] { pool.Object });

            master.Setup(h => h.software_version).Returns(new Dictionary<string, string>());
            master.Setup(h => h.ProductVersion).Returns(productVersion);
            master.Setup(h => h.AppliedPatches()).Returns(applied_patches ?? new List<Pool_patch>());
            master.Setup(h => h.BuildNumberRaw).Returns(buildNumber);
            master.Setup(h => h.uuid).Returns(id);
            return master;
        }


        private static List<XenServerVersion> GetAVersionWithGivenNumberOfPatches(int numberOfPatches)
        {
            var serverVersions = new List<XenServerVersion>();

            var version = new XenServerVersion("7.0.0", "XenServer Test 7", true, "", new List<XenServerPatch>(), new List<XenServerPatch>(), DateTime.MinValue.ToString(), "buildNo");
            for (int ii = 0; ii < numberOfPatches; ii++)
            {
                var patch = new XenServerPatch("patch_uuid_" + ii, "patch name " + ii, "patch description" + ii, "", "", "1.0", "", "", "1970-01-01T00:00:00Z", "", "1000");
                version.Patches.Add(patch);
                version.MinimalPatches.Add(patch);
            }
            serverVersions.Add(version);

            return serverVersions;
        }


        private static void RemoveANumberOfPatchesPseudoRandomly(List<XenServerPatch> patches, int numberOfPatchesToRemove)
        {
            int expectedCount = patches.Count - numberOfPatchesToRemove;
            var rnd = new Random(777); //pseudo random to be able to repeat this test...

            while (patches.Count > expectedCount)
            {
                patches.RemoveAt(rnd.Next(patches.Count));
            }
        }


        [Test]
        public void GetUpgradeSequenceForNullConnection()
        {
            Assert.AreEqual(XenAdmin.Core.Updates.GetUpgradeSequence(null), null);
        }

        /// <summary>
        /// Version exist
        /// No patches in updates
        /// Nothing installed on host
        /// Result: update sequence has the host, but empty sequence
        /// </summary>
        [Test]
        public void NoPatchesForCurrentVersion()
        {
            // Arrange

            var serverVersions = GetAVersionWithGivenNumberOfPatches(0);
            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber);
            SetXenServerVersionsInUpdates(serverVersions);

            // Act

            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            Assert.NotNull(upgradeSequence.First().Key);
            Assert.AreEqual(upgradeSequence.First().Key.uuid, master.Object.uuid);
            Assert.NotNull(upgradeSequence.First().Value);
            Assert.AreEqual(upgradeSequence.First().Value.Count, 0);
        }

        /// <summary>
        /// Version exist
        /// 1 patch in updates. 1 in minimal list.
        /// 1 has to be installed on host
        /// Result: update sequence has the host, with the one single patch in it
        /// </summary>
        [Test]
        public void OnePatchToBeInstalledForCurrentVersion()
        {
            // Arrange

            var serverVersions = GetAVersionWithGivenNumberOfPatches(1);
            var patch = serverVersions.First().Patches[0];
            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber);
            SetXenServerVersionsInUpdates(serverVersions);

            // Act

            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            Assert.NotNull(upgradeSequence.First().Key);
            Assert.AreEqual(upgradeSequence.First().Key.uuid, master.Object.uuid);
            Assert.NotNull(upgradeSequence.First().Value);
            Assert.AreEqual(upgradeSequence.First().Value.Count, 1);
            Assert.AreEqual(upgradeSequence.First().Value[0], patch);
        }


        /// <summary>
        /// Version exist
        /// 1 patch in updates. 1 in minimal list. But it is applied already on master.
        /// 0 has to be installed on host
        /// </summary>
        [Test]
        public void AlreadyInstalledOneFromMinimalForCurrentVersion()
        {
            // Arrange

            var serverVersions = GetAVersionWithGivenNumberOfPatches(1);
            var patch = serverVersions.First().Patches[0];

            Pool_patch pool_patch = new Pool_patch();
            pool_patch.uuid = patch.Uuid;

            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber, new List<Pool_patch>() { pool_patch });
            SetXenServerVersionsInUpdates(serverVersions);

            // Act

            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            Assert.NotNull(upgradeSequence.First().Key);
            Assert.AreEqual(upgradeSequence.First().Key.uuid, master.Object.uuid);
            Assert.NotNull(upgradeSequence.First().Value);
            Assert.AreEqual(upgradeSequence.First().Value.Count, 0);
        }


        /// <summary>
        /// Version exist
        /// 100 patch in updates. 100 in minimal list.
        /// 100 has to be installed on host
        /// Result: update sequence has all the 100 patches in it
        /// </summary>
        [Test]
        public void HundredPatchToBeInstalledForCurrentVersion()
        {
            // Arrange
            var serverVersions = GetAVersionWithGivenNumberOfPatches(100);
            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber);
            SetXenServerVersionsInUpdates(serverVersions);

            // Act
            
            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert
            
            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            var seqToCheck = upgradeSequence.First();
            Assert.NotNull(seqToCheck.Key);
            Assert.AreEqual(seqToCheck.Key.uuid, master.Object.uuid);
            Assert.NotNull(seqToCheck.Value);
            Assert.AreEqual(seqToCheck.Value.Count, 100);
            
            for (int ii = 100; ii < 0; --ii)
                Assert.AreEqual(seqToCheck.Value[ii], serverVersions.First().Patches[ii]);
        }

        /// <summary>
        /// Version exist
        /// 100 patch in updates. 51 in minimal list.
        /// 51 has to be installed on host
        /// Result: update sequence has all the 51 patches in it
        /// </summary>
        [Test]
        public void FiftyOnePatchToBeInstalledForCurrentVersion()
        {
            // Arrange
            var serverVersions = GetAVersionWithGivenNumberOfPatches(100);
            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber);
            RemoveANumberOfPatchesPseudoRandomly(serverVersions.First().MinimalPatches, 49);
            SetXenServerVersionsInUpdates(serverVersions);

            // Act

            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            var seqToCheck = upgradeSequence.First();
            Assert.NotNull(seqToCheck.Key);
            Assert.AreEqual(seqToCheck.Key.uuid, master.Object.uuid);
            Assert.NotNull(seqToCheck.Value);
            Assert.AreEqual(seqToCheck.Value.Count, 51);

            foreach (var patch in seqToCheck.Value)
                Assert.IsTrue(serverVersions.First().MinimalPatches.Contains(patch));

            Assert.False(seqToCheck.Value.Exists(seqpatch => !serverVersions.First().MinimalPatches.Contains(seqpatch)));
        }


        /// <summary>
        /// Version exist
        /// 100 patch in updates. 51 in minimal list.
        /// 41 has to be installed on host (only 41 of 51, because 10 is already installed from the Minimal list (as well as 5 other))
        /// Result: update sequence has all the 41 patches in it and all are from Minimal List and also not installed on the host
        /// </summary>
        [Test]
        public void FourtyOnePatchToBeInstalledForCurrentVersion()
        {
            // Arrange
            var serverVersions = GetAVersionWithGivenNumberOfPatches(100);
            RemoveANumberOfPatchesPseudoRandomly(serverVersions.First().MinimalPatches, 49);

            var pool_patches = new List<Pool_patch>();
            for (int ii = 0; ii < 10; ii++)
            {
                pool_patches.Add(new Pool_patch() { uuid = serverVersions.First().MinimalPatches[ii].Uuid });
            }

            var notMinimalPatchesInVersion = serverVersions.First().Patches.Where(p => !serverVersions.First().MinimalPatches.Exists(mp => mp.Uuid == p.Uuid)).ToList();
            for (int ii = 0; ii < 5; ii++)
            {
                pool_patches.Add(new Pool_patch() { uuid = notMinimalPatchesInVersion[ii].Uuid });
            }

            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber, pool_patches);

            SetXenServerVersionsInUpdates(serverVersions);

            // Act

            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            var seqToCheck = upgradeSequence.First();
            Assert.NotNull(seqToCheck.Key);
            Assert.AreEqual(seqToCheck.Key.uuid, master.Object.uuid);
            Assert.NotNull(seqToCheck.Value);
            Assert.AreEqual(seqToCheck.Value.Count, 41);

            foreach (var patch in seqToCheck.Value)
                Assert.IsTrue(serverVersions.First().MinimalPatches.Contains(patch) && !pool_patches.Exists(p  => p.uuid == patch.Uuid));

            Assert.False(seqToCheck.Value.Exists(seqpatch => !serverVersions.First().MinimalPatches.Contains(seqpatch)));
            Assert.True(seqToCheck.Value.Exists(seqpatch => !pool_patches.Exists(pp => pp.uuid == seqpatch.Uuid)));
        }


        /// <summary>
        /// Version exist
        /// 2 patch in updates. 1 in minimal list.
        /// 1 has to be installed on host
        /// Result: update sequence has the host, with the one single patch in it
        /// </summary>
        [Test]
        public void OnePatchOutOfTwoToBeInstalledForCurrentVersion()
        {
            // Arrange

            var serverVersions = GetAVersionWithGivenNumberOfPatches(2);
            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber);
            RemoveANumberOfPatchesPseudoRandomly(serverVersions.First().MinimalPatches, 1);
            SetXenServerVersionsInUpdates(serverVersions);

            // Act
            
            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            Assert.NotNull(upgradeSequence.First().Key);
            Assert.AreEqual(upgradeSequence.First().Key.uuid, master.Object.uuid);
            Assert.NotNull(upgradeSequence.First().Value);
            Assert.AreEqual(upgradeSequence.First().Value.Count, 1);
            Assert.AreEqual(upgradeSequence.First().Value[0], serverVersions.First().MinimalPatches[0]);
        }


        /// <summary>
        /// Version exist
        /// 2 patch in updates. 2 in minimal list.
        /// 2 has to be installed on host
        /// Result: update sequence has the host, with the one single patch in it
        /// </summary>
        [Test]
        public void TwoPatchToBeInstalleOutOfTwodForCurrentVersion()
        {
            // Arrange

            var serverVersions = GetAVersionWithGivenNumberOfPatches(2);
            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber);
            SetXenServerVersionsInUpdates(serverVersions);

            // Act

            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.NotNull(upgradeSequence);
            Assert.AreEqual(1, upgradeSequence.Count);
            Assert.NotNull(upgradeSequence.First().Key);
            Assert.AreEqual(upgradeSequence.First().Key.uuid, master.Object.uuid);
            Assert.NotNull(upgradeSequence.First().Value);
            Assert.AreEqual(upgradeSequence.First().Value.Count, 2);
            Assert.AreEqual(upgradeSequence.First().Value[0], serverVersions.First().MinimalPatches[0]);
            Assert.AreEqual(upgradeSequence.First().Value[1], serverVersions.First().MinimalPatches[1]);
        }


        /// <summary>
        /// Version does not exist
        /// Result: update sequence is null
        /// </summary>
        [Test]
        public void NoInfoForCurrentVersion()
        {
            // Arrange

            var serverVersions = GetAVersionWithGivenNumberOfPatches(200);
            var master = GetAMockHost(serverVersions.First().Oem, serverVersions.First().BuildNumber + "to_make_it_not_match");
            SetXenServerVersionsInUpdates(serverVersions);

            // Act

            var upgradeSequence = XenAdmin.Core.Updates.GetUpgradeSequence(master.Object.Connection);

            // Assert

            Assert.Null(upgradeSequence);
        }


        /// <summary>
        /// Updates is static class, can't be mocked without refactoring - using this method instead
        /// </summary>
        /// <param name="serverVersions"></param>
        private void SetXenServerVersionsInUpdates(List<XenServerVersion> serverVersions)
        {
            Updates.XenServerVersions = serverVersions;
        }
    }
}
