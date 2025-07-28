using OrderService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace OrderService.RabbitMQ.Implementation
{
    public class RabbitMqProductCheck : IRabbitMqProductCheck, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _replyQueueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper;
        private readonly string _requestQueueName = "productservice";

        public RabbitMqProductCheck(IConfiguration configuration)
        {
            var factory = new ConnectionFactory { HostName = configuration["RabbitMQ:Host"] };
            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
            
            // Declare the request queue
            _channel.QueueDeclareAsync(_requestQueueName, durable: false, exclusive: false, autoDelete: false).Wait();
            
            // Create reply queue and get its name
            _replyQueueName = _channel.QueueDeclareAsync(queue: "", durable: false, exclusive: true, autoDelete: true).Result.QueueName;
            
            _callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += ConsumerReceivedAsync;
            
            _channel.BasicConsumeAsync(_replyQueueName, true, consumer).Wait();
        }

        private async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var response = Encoding.UTF8.GetString(ea.Body.ToArray());
            var correlationId = ea.BasicProperties.CorrelationId;
            if (_callbackMapper.TryRemove(correlationId, out var tcs))
            {
                tcs.SetResult(response);
            }
        }

        public async Task<ValidateProductResponseDto> CheckProductStockAvailabilityAsync(int productId, int requestedQuantity)
        {
            var ValidateProductRequestDto = new
            {
                ProductId = productId,
                RequestedQuantity = requestedQuantity
            };

            var message = JsonSerializer.Serialize(ValidateProductRequestDto);
            var correlationId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<string>();
            _callbackMapper.TryAdd(correlationId, tcs);

            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = _replyQueueName
            };

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _requestQueueName,
                mandatory: true,
                basicProperties: props,
                body: messageBytes);

            var result = await tcs.Task;
            return JsonSerializer.Deserialize<ValidateProductResponseDto>(result);
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
                _channel.CloseAsync().Wait();
            if (_connection.IsOpen)
                _connection.CloseAsync().Wait();
        }
    }
}
