namespace OrderService.RabbitMQ
{
    public interface IRabbitMqPublisher
    {
        public Task<bool> SendMessageToQueue(string queueName, string message);
    }
}
