using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaPOF.Commands;
using AkkaPOF.Messages;

namespace AkkaPOF.Actors
{
    public class ReportProcessorActor : ReceiveActor
    {
        private readonly IActorRef reportStatusActor;
        private int actorNumber;

        public static int ProcessorTotalCount = 0;

        public ReportProcessorActor(IActorRef reportStatusActor)
        {
            this.reportStatusActor = reportStatusActor;
            //reportStatusActor = Context.ActorSelection("akka.tcp://MyActorSystem@127.0.0.1:8081/user/ReportStatusActor").ResolveOne(TimeSpan.FromMilliseconds(100000)).Result;
            Receive<ReportRequest>(m => CreateReport(m));
            actorNumber = ReportProcessorActor.ProcessorTotalCount;
            Interlocked.Increment(ref ReportProcessorActor.ProcessorTotalCount);
        }

        private void CreateReport(ReportRequest reportRequest)
        {
            Console.WriteLine($"Report Kurva  {actorNumber}");
            var requestStatusInfo = new RequestStatusInfo(reportRequest.RequestUid, RequestStatus.Assigned);

            reportStatusActor.Tell(requestStatusInfo);

            var newStatus = requestStatusInfo.WithNewStatusAndReportId(RequestStatus.Finished, 1);

            reportStatusActor.Tell(newStatus);
            Sender.Tell(newStatus);
        }
    }
}