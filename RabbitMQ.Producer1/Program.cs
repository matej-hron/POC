using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using RabbitMQ.Producer1.Tests.Actors;

namespace RabbitMQ.Producer1
{
    class Program
    {
        public static void Main()
        {
            //BasicPublishTest();
            Run();
        }

        private static void Run()
        {
            // ActorSystem is a heavy object: create only one per application
            ActorSystem system = ActorSystem.Create("MySystem");
            var emmiterActor = system.ActorOf(Props.Create(typeof(PublishingActor)), "emmiter");
            emmiterActor.Tell(new PublishingActor.StartEmmitMessages(int.MaxValue));

            Task.Delay(100).Wait();

            var sub1 = system.ActorOf(Props.Create(typeof(SubscribeActor)), "subscriber1");
            var sub2 = system.ActorOf(Props.Create(typeof(SubscribeActor)), "subscriber2");
            sub1.Tell(new SubscribeActor.StartReceive());
            sub2.Tell(new SubscribeActor.StartReceive());


            Console.WriteLine("Publishing");
            Console.ReadLine();

            system.Terminate();
        }

    }
}
