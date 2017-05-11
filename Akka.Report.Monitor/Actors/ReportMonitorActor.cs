using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Remote;
using Akka.Report.Monitor.Messages;
using Akka.Routing;
using CI.CBS.InternalModules.Reports.Messages;

namespace Akka.Report.Monitor.Actors
{
    public class ReportMonitorActor : ReceiveActor
    {
        public static string Name = "ReportMonitorActor";

        private readonly IActorRef coordinatorActorRef;
        private HashSet<Guid> requestIdsInProgress;

        public ReportMonitorActor(IActorRef coordinatorActorRef)
        {
            this.coordinatorActorRef = coordinatorActorRef;
            

            Ready();
        }

        private void Ready()
        {
            Console.WriteLine("I am ready");
            Receive<GetStatus>(m => HandleGetStatus(m));
        }

        private void Working()
        {
            Console.WriteLine("I am working");
            Receive<GetStatus>(_ =>
            {
                Console.WriteLine("I am busy with it!");
            });
            Receive<ReportStatusesMessage>(m =>
            {
                Console.WriteLine($@"(Requests = {m.RequestIds.Count()}) actor {Sender.Path}");

                foreach (var requestId in m.RequestIds)
                    requestIdsInProgress.Add(requestId);

            });

            Receive<CollectResults>(_ =>
            {
                Console.WriteLine("Requests in progress:");
                foreach (var requestId in requestIdsInProgress)
                {
                    Console.WriteLine(requestId);
                }

                Become(Ready);
            });
        }

        private void HandleGetStatus(GetStatus message)
        {
            Become(Working);
            requestIdsInProgress = new HashSet<Guid>();
            coordinatorActorRef.Tell(new Broadcast(new RequestReportStatusesMessage(Self)));

            

            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(10),
                Self,
                new CollectResults(), 
                Self);


        }
    }
}