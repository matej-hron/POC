using System;
using Akka.Report.Monitor.Actors;
using Akka.Report.Monitor.Messages;
using CI.CBS.InternalModules.Reports;
using CI.CBS.InternalModules.Reports.Services;

namespace Akka.Report.Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var deployers = new[] {new MonitorDeployer()};
            var configuration = new AkkaConfiguration();
            var akkaService = new AkkaService(deployers, configuration);
            var monitor = akkaService.GetActorRef(ReportMonitorActor.Name);

            akkaService.Start( _ => Console.WriteLine("akka start failed"));

            Console.WriteLine("Press enter to request reporting status (x for exit)");
            var input = Console.ReadLine();

            while (input != "x")
            {
                monitor.Tell(new GetStatus(), null);
                input = Console.ReadLine();
            }

            Console.WriteLine("Stopping...");
            akkaService.Stop();
        }
    }
}
