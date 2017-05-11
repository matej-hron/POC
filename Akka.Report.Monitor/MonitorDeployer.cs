using Akka.Actor;
using Akka.Report.Monitor.Actors;
using Akka.Routing;
using CI.CBS.InternalModules.Reports.Actors;
using CI.CBS.InternalModules.Reports.Services;

namespace Akka.Report.Monitor
{
    public class MonitorDeployer : ActorDeployerBase
    {
        public override void DeployTo(IAkkaService akkaService)
        {
            var coordinator = akkaService.Deploy(CoordinatorActor.ClientName, system => system.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), CoordinatorActor.ClientName));
            akkaService.Deploy(ReportMonitorActor.Name, system => system.ActorOf(Props.Create(()=> new ReportMonitorActor(coordinator)), ReportMonitorActor.Name));
        }

        public override string Role => "monitor";
    }
}