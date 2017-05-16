using System.IO;
using Hyperion;

namespace RabbitMQ.Producer1
{
    public class MySerializer
    {
        private readonly Serializer serializer;

        public MySerializer()
        {
            serializer = new Serializer(new SerializerOptions(false, true));
        }

        public byte[] Serialize(object report)
        {
            if (report == null)
                return null;

            using (var ms = new MemoryStream())
            {
                serializer.Serialize(report, ms);

                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            using (var ms = new MemoryStream(data))
            {
                return serializer.Deserialize<T>(ms);
            }
        }
    }
}