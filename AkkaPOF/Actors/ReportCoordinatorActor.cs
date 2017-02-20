using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Routing;
using AkkaPOF.Commands;
using AkkaPOF.Messages;

namespace AkkaPOF.Actors
{
    public class ReportCoordinatorActor : ReceiveActor
    {
        private int counter;

        private readonly IActorRef reportProcessorActor;
        private readonly IActorRef reportStatusActor;


        public ReportCoordinatorActor()
        {
            Console.WriteLine("Constructing coordinator");
        }

        public ReportCoordinatorActor(IActorRef reportProcessorActor, IActorRef reportStatusActor)
        {
            Console.WriteLine("Constructing coordinator with report processr");

            //reportStatusActor = Context.ActorSelection("akka.tcp://cb5@127.0.0.1:8081/user/ReportStatusActor").ResolveOne(TimeSpan.FromMilliseconds(10000)).Result;
            this.reportProcessorActor = reportProcessorActor;
            this.reportStatusActor = reportStatusActor;

            Receive<ReportRequest>(m => ReceiveRequest(m));

        }

        private void ReceiveRequest(ReportRequest message)
        {
            Console.WriteLine("Koordinuj pico");
            var requestStatusMessage = new RequestStatusInfo(message.RequestUid, RequestStatus.New);            
            reportStatusActor.Tell(requestStatusMessage);

            Interlocked.Increment(ref this.counter);
            reportProcessorActor.Forward(message);         
        }

    }
}