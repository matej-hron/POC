using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using RabbitMQ.Client;
using RabbitMQ.Producer1.Tests.Messages;
using static RabbitMQ.Producer1.Helper;

namespace RabbitMQ.Producer1.Tests.Actors
{
    public class PublishingActor : ReceiveActor
    {
        private readonly MySerializer serializer;

        public PublishingActor()
        {
            serializer = new MySerializer();

            Receive<StartEmmitMessages>(m => EmmitMessages(m));

        }

        private void EmmitMessages(StartEmmitMessages startEmmitMessages)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            using(var materializer = Context.Materializer())
            {
                channel.ExchangeDeclare(exchange: "testexchange", type: "fanout");

                var source = Source.From(Enumerable.Range(1, startEmmitMessages.MessageCount)).Throttle(150, TimeSpan.FromSeconds(1), 1, ThrottleMode.Shaping);
                var t = source.RunForeach(i => { PublishMessage(i, channel); }, materializer);
                //Enumerable.Range(1, startEmmitMessages.MessageCount).ToList().ForEach(i => PublishMessage(i, channel));

                Task.Delay(TimeSpan.FromMinutes(100)).Wait();
            }

        }

        private void PublishMessage(int i, IModel channel)
        {
            if (i % 1000 == 0)
                Console.WriteLine($"published {i}");

            var data = new WorkMessage(i);

            channel.BasicPublish(exchange: "testexchange",
                routingKey: "",
                basicProperties: null,
                body: serializer.Serialize(data));

            Task.Delay(10).Wait();
        }


        public class StartEmmitMessages
        {
            public int MessageCount { get; set; }

            public StartEmmitMessages(int count)
            {
                MessageCount = count;
            }
        }
    }
}