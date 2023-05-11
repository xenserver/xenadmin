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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CFUValidator.CommandLineOptions;
using CFUValidator.OutputDecorators;
using CFUValidator.Updates;
using CFUValidator.Validators;
using Moq;
using XenAdmin;
using XenAdmin.Core;
using XenAdminTests;
using XenAPI;


namespace CFUValidator
{
    public class CFUValidationException : Exception
    {
        public CFUValidationException(string message) : base(message){}
    }

    public class CFUValidator
    {
        private readonly MockObjectManager mom = new MockObjectManager();
        private readonly XmlRetrieverFactory xmlFactory = new XmlRetrieverFactory();
        private const string id = "id";
        private readonly string _xmlLocation;
        private readonly string _serverVersion;
        private readonly OptionUsage UrlOrFile;
        private readonly List<string> _installedHotfixes;
        private readonly bool _checkHotfixContents;
        private readonly IConfigProvider _configProvider;

        public CFUValidator(OptionUsage urlOrFile, string xmlLocation, string serverVersion,
            List<string> installedHotfixes, bool checkHotfixContents, IConfigProvider configProvider)
        {
            if (urlOrFile != OptionUsage.File && urlOrFile != OptionUsage.Url)
                throw new ArgumentException("urlOrFile option should be either File or Url");

            mom.CreateNewConnection(id);
            ConnectionsManager.XenConnections.AddRange(mom.AllConnections);
            _xmlLocation = xmlLocation;
            _serverVersion = serverVersion;
            _installedHotfixes = installedHotfixes;
            UrlOrFile = urlOrFile;
            _checkHotfixContents = checkHotfixContents;
            _configProvider = configProvider;
        }

        public void Validate(Action<string> statusReporter)
        {
            statusReporter($"Getting check for updates XML from {_xmlLocation}...");
            ReadCheckForUpdatesXML(out var xenServerPatches, out var xenServerVersions, out var clientVersions);

            List<string> versionsToCheck;
            if (_serverVersion == CFUCommandLineOptionManager.AllVersions)
            {
                versionsToCheck = xenServerVersions.ConvertAll(i => i.Version.ToString()).Distinct().ToList();
            }
            else
            {
                CheckVersionExistsInCfu(_serverVersion, xenServerVersions);
                versionsToCheck = new List<string> {_serverVersion};
            }

            var summaryGenerators = new List<ISummaryGenerator>();
            foreach (string ver in versionsToCheck)
                summaryGenerators.AddRange(RunTestsForGivenServerVersion(ver, xenServerVersions, xenServerPatches, clientVersions, statusReporter));

            summaryGenerators.ForEach(s => statusReporter(s.GenerateSummary()));
        }

        private List<ISummaryGenerator> RunTestsForGivenServerVersion(string serverVersion, List<XenServerVersion> xenServerVersions,
            List<XenServerPatch> xenServerPatches, List<ClientVersion> clientVersions, Action<string> statusReporter)
        {
            statusReporter($"Generating server {serverVersion} mock-ups...");
            SetupMocks(serverVersion, xenServerPatches, xenServerVersions);

            statusReporter("Determining required client update...");
            var xcupdateAlerts = XenAdmin.Core.Updates.NewClientUpdateAlerts(clientVersions, new Version(serverVersion));

            statusReporter("Determining required XenServer update...");
            var updateAlerts = XenAdmin.Core.Updates.NewXenServerVersionAlerts(xenServerVersions).Where(alert => !alert.CanIgnore).ToList();

            statusReporter("Determining required patches...");
            var patchAlerts = XenAdmin.Core.Updates.NewXenServerPatchAlerts(xenServerVersions, xenServerPatches).Where(alert => !alert.CanIgnore).ToList();

            statusReporter("Running patch check(s), this may take some time...");

            var validators = new List<Validator>
            {
                new HfxEligibilityValidator(xenServerVersions),
                new CorePatchDetailsValidator(patchAlerts),
                new PatchURLValidator(patchAlerts)
            };

            if (_checkHotfixContents)
                validators.Add(new ZipContentsValidator(patchAlerts, _configProvider));

            validators.ForEach(v => v.Validate(statusReporter));

            var summaryGenerators = new List<ISummaryGenerator> {new HeaderDecorator(serverVersion, _xmlLocation)};
            summaryGenerators.AddRange(validators);
            summaryGenerators.Add(new ClientUpdateDecorator(xcupdateAlerts));
            summaryGenerators.Add(new XenServerUpdateDecorator(updateAlerts));
            summaryGenerators.Add(new PatchAlertDecorator(patchAlerts));
            return summaryGenerators;
        }

        private void CheckVersionExistsInCfu(string serverVersion, List<XenServerVersion> xenServerVersions)
        {
            if (xenServerVersions.All(v => v.Version.ToString() != serverVersion))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Could not find version {serverVersion} in the CFU file.");
                sb.AppendLine("Available versions are:");
                xenServerVersions.Select(i => i.Version.ToString()).Distinct().ToList().ForEach(v => sb.AppendLine(v));
                throw new CFUValidationException(sb.ToString());
            }
        }

        private void ReadCheckForUpdatesXML(out List<XenServerPatch> patches, out List<XenServerVersion> versions, out List<ClientVersion> xcVersions)
        {
            ICheckForUpdatesXMLSource checkForUpdates = xmlFactory.GetAction(UrlOrFile, _xmlLocation);
            checkForUpdates.RunAsync();
            
            ConsoleSpinner spinner = new ConsoleSpinner();
            while(!checkForUpdates.IsCompleted)
            {
                spinner.Turn(checkForUpdates.PercentComplete);
            }

            if (checkForUpdates.ErrorRaised != null)
                throw checkForUpdates.ErrorRaised;

            patches = checkForUpdates.XenServerPatches;
            versions = checkForUpdates.XenServerVersions;
            xcVersions = checkForUpdates.ClientVersions;
        }

        private void SetupMocks(string versionToCheck, List<XenServerPatch> xenServerPatches, List<XenServerVersion> xenServerVersions)
        {
            Mock<Host> coordinator = mom.NewXenObject<Host>(id);
            Mock<Pool> pool = mom.NewXenObject<Pool>(id);
            XenRef<Host> coordinatorRef = new XenRef<Host>("ref");
            pool.Setup(p => p.master).Returns(coordinatorRef);
            pool.Setup(p => p.other_config).Returns(new Dictionary<string, string>());
            mom.MockCacheFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Pool>>())).Returns(pool.Object);
            mom.MockConnectionFor(id).Setup(c => c.Resolve(coordinatorRef)).Returns(coordinator.Object);
            mom.MockConnectionFor(id).Setup(c => c.IsConnected).Returns(true);
            coordinator.Setup(h => h.software_version).Returns(new Dictionary<string, string>());
            coordinator.Setup(h => h.ProductVersion()).Returns(versionToCheck);
            coordinator.Setup(h => h.AppliedPatches()).Returns(GenerateMockPoolPatches(xenServerPatches));
            
            //Currently build number will be referenced first so if it's present hook it up
            string buildNumber = xenServerVersions.First(v => v.Version.ToString() == versionToCheck).BuildNumber;
            coordinator.Setup(h=>h.BuildNumberRaw()).Returns(buildNumber);
        }

        private List<Pool_patch> GenerateMockPoolPatches(List<XenServerPatch> xenServerPatches)
        {
            List<Pool_patch> patches = new List<Pool_patch>();

            foreach (string installedHotfix in _installedHotfixes)
            {
                string hotfix = installedHotfix;
                XenServerPatch match = xenServerPatches.Find(m => m.Name.Contains(hotfix));

                if(match == null)
                    throw new CFUValidationException("No patch could be found in the XML matching " + hotfix);

                Mock<Pool_patch> pp = mom.NewXenObject<Pool_patch>(id);
                pp.Setup(p => p.uuid).Returns(match.Uuid);
                patches.Add(pp.Object);
            }
            
            return patches;
        }

    }
}
