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
                    string.Format(Messages.ACTION_START_CONTAINER_TITLE, dockerContainer.Name),
                    Messages.ACTION_START_CONTAINER_DESCRIPTION, 
                    Messages.ACTION_START_CONTAINER_END_DESCRIPTION, 
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
                    string.Format(Messages.ACTION_STOP_CONTAINER_TITLE, dockerContainer.Name),
                    Messages.ACTION_STOP_CONTAINER_DESCRIPTION,
                    Messages.ACTION_STOP_CONTAINER_END_DESCRIPTION,
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
                    string.Format(Messages.ACTION_PAUSE_CONTAINER_TITLE, dockerContainer.Name),
                    Messages.ACTION_PAUSE_CONTAINER_DESCRIPTION,
                    Messages.ACTION_PAUSE_CONTAINER_END_DESCRIPTION,
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
                    string.Format(Messages.ACTION_RESUME_CONTAINER_TITLE, dockerContainer.Name),
                    Messages.ACTION_RESUME_CONTAINER_DESCRIPTION,
                    Messages.ACTION_RESUME_CONTAINER_END_DESCRIPTION,
                    "unpause")
        { }
    }

    /// <summary>
    /// Restart the Docker container
    /// </summary>
    public class RestartDockerContainerAction : DockerContainerLifetimeAction
    {
        public RestartDockerContainerAction(DockerContainer dockerContainer)
            : base(dockerContainer,
                    string.Format(Messages.ACTION_RESTART_CONTAINER_TITLE, dockerContainer.Name),
                    Messages.ACTION_RESTART_CONTAINER_DESCRIPTION,
                    Messages.ACTION_RESTART_CONTAINER_END_DESCRIPTION,
                    "restart")
        { }
    }
}
