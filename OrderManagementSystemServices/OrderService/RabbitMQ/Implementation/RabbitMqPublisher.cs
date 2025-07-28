using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace OrderService.RabbitMQ.Implementation
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IConfiguration _configuration;

        private IConnection _connection;
        private IChannel _channel;
        private readonly string _queueName = "loggingservice";
        public RabbitMqPublisher(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory { HostName = _configuration["RabbitMQ:Host"] };
            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
        }

        public async Task<bool> SendMessageToQueue(string queueName, string message)
        {
            var callWorked = false;
            try
            {
                _channel.QueueDeclareAsync(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var body = Encoding.UTF8.GetBytes(message);

                await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: "loggingservice", body: body);

                callWorked = true;
            }
            catch (Exception)
            {

                //log error
            }

            return callWorked;
        }
    }
}
