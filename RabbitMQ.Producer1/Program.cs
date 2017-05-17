using RabbitMQ.Client;
using System;
using System.Linq;
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
            emmiterActor.Tell(new PublishingActor.StartEmmitMessages(1000000));

            Task.Delay(100).Wait();

            CreateSubscribers(system, 10);


            Console.WriteLine("Publishing");
            Console.ReadLine();

            system.Terminate();
        }


        private static void CreateSubscribers(ActorSystem system, int number)
        {
            Enumerable.Range(1, number).ToList().ForEach(i =>
            {
                var sub = system.ActorOf(Props.Create(typeof(SubscribeActor)), $"subscriber{i}");
                sub.Tell(new SubscribeActor.StartReceive());
            });
        }
    }



}
