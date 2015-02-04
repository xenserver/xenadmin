using System;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Model;

namespace XenAdmin.Actions
{
    public class DockerContainerLifetimeAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DockerContainer dockerContainer;
        private readonly string action;
        private readonly string endDescription;

        public DockerContainerLifetimeAction(DockerContainer dockerContainer,  string title, string startDescription, string endDescription, string action)
            : base(dockerContainer.Connection, title, startDescription)
        {
            this.endDescription = endDescription;
            this.dockerContainer = dockerContainer;
            this.action = action;
        }

        public static RbacMethodList StaticRBACDependencies = new RbacMethodList("Host.call_plugin");

        protected override void Run()
        {
            var host = Helpers.GetMaster(Connection);
            var result = false;
            try
            {
                var args = new Dictionary<string, string> { { "vmuuid", dockerContainer.Parent.uuid }, { "container", dockerContainer.container } };
                Result = Host.call_plugin(Session, host.opaque_ref, "xscontainer", action, args);
                result = Result.ToLower().StartsWith("true");
            }
            catch (Failure failure)
            {
                log.WarnFormat("Plugin call xscontainer.start({0}) on {1} failed with {2}", dockerContainer.uuid, host.Name,
                    failure.Message);
                throw;
            }

            if (result)
                Description = endDescription;
            else
            {
                log.WarnFormat("Plugin call xscontainer.start({0}) on {1} failed with {2}", dockerContainer.uuid, host.Name, Result);
                Exception = new Exception(Result ?? Messages.ERROR_UNKNOWN);
            }
        }
    }

    /// <summary>
    /// Start the Docker container
    /// </summary>
    public class StartDockerContainerAction : DockerContainerLifetimeAction
    {
        public StartDockerContainerAction(DockerContainer dockerContainer)
            : base(dockerContainer,
                    "Start Docker Container",
                    string.Format("Starting '{0}'", dockerContainer.Name),
                    string.Format("Start '{0}' completed", dockerContainer.Name),
                    "start")
        { }
    }

    /// <summary>
    /// Stop the Docker container
    /// </summary>
    public class StopDockerContainerAction : DockerContainerLifetimeAction
    {
        public StopDockerContainerAction(DockerContainer dockerContainer)
            : base(dockerContainer,
                    "Stop Docker Container",
                    string.Format("Stopping '{0}'", dockerContainer.Name),
                    string.Format("Stop '{0}' completed", dockerContainer.Name),
                    "stop")
        { }
    }

    /// <summary>
    /// Pause the Docker container
    /// </summary>
    public class PauseDockerContainerAction : DockerContainerLifetimeAction
    {
        public PauseDockerContainerAction(DockerContainer dockerContainer)
            : base(dockerContainer,
                    "Pause Docker Container",
                    string.Format("Pausing '{0}'", dockerContainer.Name),
                    string.Format("Pause '{0}' completed", dockerContainer.Name),
                    "pause")
        { }
    }

    /// <summary>
    /// Resume the Docker container
    /// </summary>
    public class ResumeDockerContainerAction : DockerContainerLifetimeAction
    {
        public ResumeDockerContainerAction(DockerContainer dockerContainer)
            : base(dockerContainer,
                    "Resume Docker Container",
                    string.Format("Resuming '{0}'", dockerContainer.Name),
                    string.Format("Resuming '{0}' completed", dockerContainer.Name),
                    "unpause")
        { }
    }
}
