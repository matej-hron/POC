using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using EasyNetQ;
using RabbitMQ.Producer1.Tests.Actors;
using RabbitMQ.Producer1.Tests.Messages;

namespace RabbitMQ.Producer1
{
    class Program
    {
        public static void Main()
        {
            //BasicPublishTest();
            Run2();
        }

        public static void Run2()
        {
            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest;persistentMessages=true"); 

            var publisherTask = Task.Run(() =>
            {
                Enumerable.Range(1, 10000).ToList().ForEach(i =>
                {
                    if(i%1000==0)
                        Console.WriteLine($"publishing {i}");

                    var message = new WorkMessage(i);
                    bus.Publish(message);
                });
            });
            //publisherTask.Wait();
            var tasks = Enumerable.Range(1, 10).Select(i => Task.Run(() =>
            {

                using (var subscriptionResult = bus.SubscribeAsync<WorkMessage>($"sub{i}", w => Task.Run(() =>
                {
                    if (w.Id % 1000 == 0)
                        Console.WriteLine($"receiving: {DateTime.Now.ToShortTimeString()}-{w.Id}");
                })))
                {
                    Task.Delay(TimeSpan.FromMinutes(1)).Wait();
                }
            }));

            Task.WaitAll(tasks.ToArray());
        }


        private static void Run()
        {
            // ActorSystem is a heavy object: create only one per application
            ActorSystem system = ActorSystem.Create("MySystem");
            var emmiterActor = system.ActorOf(Props.Create(typeof(PublishingActor)), "emmiter");
            emmiterActor.Tell(new PublishingActor.StartEmmitMessages(1000000));

            Task.Delay(100).Wait();

            CreateSubscribers(system, 1);


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
