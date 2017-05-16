using System;
using Akka.Actor;
using Akka.Streams;

namespace RabbitMQ.Producer1
{
    public static class Helper
    {
        public static T WithMaterializer<T>(Func<ActorMaterializer, T> call)
        {
            using (var system = ActorSystem.Create("system"))
            using (var materializer = system.Materializer())
            {
                return call(materializer);
            }

        }

        public static void WithMaterializer(Action<ActorMaterializer> call)
        {
            WithMaterializer<object>(m =>
            {
                call(m);
                return null;
            });
        }

    }
}