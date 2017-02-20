using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using AkkaPOF.Actors;
using AkkaPOF.Commands;
using AkkaPOF.Messages;

namespace AkkaPOF.Client
{
    class Program
    {
        public static ActorSystem System;

        static void Main(string[] args)
        {
            System = ActorSystem.Create("cb5");
            var coordinator = System.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "ReportCoordinatorActor");

            Console.WriteLine("Ready?");
            Console.ReadLine();

            Parallel.For(0, 10000, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, i =>
            {
                GetReport(coordinator);
            });


            System.WhenTerminated.Wait();
        }

        private static void GetReport(IActorRef coordinator)
        {
            var sw = new Stopwatch();
            sw.Start();
            var request = new ReportRequest(11);
            var responseTask = coordinator.Ask<RequestStatusInfo>(request, TimeSpan.FromSeconds(30));
            Task.WaitAny(responseTask, Task.Delay(30000));
            sw.Stop();
            if (responseTask.IsCompleted)
            {
                try
                {
                    var response = responseTask.Result;
                    Console.WriteLine($"result: {response.RequestUid} - {response.RequestStatus}");
                }
                catch (Exception exc)
                {
                }
            }
            else
            {
                Console.WriteLine("Timeout");
            }
        }

    }
}
