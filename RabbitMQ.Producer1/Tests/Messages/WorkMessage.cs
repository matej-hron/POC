namespace RabbitMQ.Producer1.Tests.Messages
{
    public class WorkMessage
    {
        public int Id { get; set; }

        public WorkMessage(int id)
        {
            Id = id;
        }

        public WorkMessage()
        {
            
        }
    }
}