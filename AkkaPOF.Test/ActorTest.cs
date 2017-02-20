using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit;
using AkkaPOF.Actors;
using AkkaPOF.Commands;
using AkkaPOF.Messages;
using NUnit.Framework;

namespace AkkaPOF.Test
{
    [TestFixture]
    public class ActorTest :TestKit
    {
        [Test]
        public void X()
        {
            
            var reportStatusActor = Sys.ActorOf(Props.Create(() => new ReportStatusActor()));
            var reportProcessor = Sys.ActorOf(Props.Create(() => new ReportProcessorActor(reportStatusActor)));
            var coordinator = Sys.ActorOf(Props.Create(() => new ReportCoordinatorActor(reportProcessor, reportStatusActor)));

            coordinator.Tell(new ReportRequest());

            var result = ExpectMsg<RequestStatusInfo>();
            Assert.That(result.RequestStatus, Is.EqualTo(RequestStatus.Finished));
        }

    }
}
