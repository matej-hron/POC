using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaPOF.Actors;
using AkkaPOF.Messages;
using Akka.Routing;

namespace AkkaPOF
{
    class Program
    {
        public static ActorSystem System;

        static void Main(string[] args)
        {
            System = ActorSystem.Create("cb5");

            var reportStatusActor = System.ActorOf(Props.Create<ReportStatusActor>(), "ReportStatusActor");
            var reportProcessorActor = System.ActorOf(Props.Create(() => new ReportProcessorActor(reportStatusActor)).WithRouter(FromConfig.Instance), "report");           
            System.ActorOf(Props.Create(() => new ReportCoordinatorActor(reportProcessorActor, reportStatusActor)), "ReportCoordinatorActor");

            System.WhenTerminated.Wait();
        }


    }
}
