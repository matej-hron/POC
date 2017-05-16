using System;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Producer1.Tests.Messages;

namespace RabbitMQ.Producer1.Tests.Actors
{
    public class SubscribeActor : ReceiveActor
    {

        private readonly MySerializer serializer;

        public SubscribeActor()
        {
            Receive<StartReceive>(m => Listen(m));
            serializer = new MySerializer();
        }

        private void Listen(StartReceive m)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "testexchange", type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                    exchange: "testexchange",
                    routingKey: "");

                Console.WriteLine("Receiving");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var data = serializer.Deserialize<WorkMessage>(body);
                    Console.WriteLine($"received {data.Id}");
                };

                channel.BasicConsume(queue: queueName,
                    noAck: true,
                    consumer: consumer);

                Task.Delay(TimeSpan.FromMinutes(1));

            }
        }

        public class StartReceive
        {
            
        }
    }
}