using LoggingService.Models;
using LoggingService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace LoggingService.RabbitMQ.Implementation
{
    public class RabbitMqConsumer : BackgroundService, IRabbitMqConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingService _loggingService;

        private IConnection _connection;
        private IChannel _channel;
        private readonly string _queueName = "loggingservice";

        public RabbitMqConsumer(IConfiguration configuration, ILoggingService loggingService) 
        {
            _configuration = configuration;
            _loggingService = loggingService;

            var IsConnectionReady = Initialize();
        }

        private bool Initialize()
        {
            int retryCount = 5;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    var factory = new ConnectionFactory { HostName = _configuration["RabbitMQ:Host"] };
                    _connection = factory.CreateConnectionAsync().Result;
                    _channel = _connection.CreateChannelAsync().Result;
                    _channel.QueueDeclareAsync(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine("RabbitMQ not ready yet. Retrying...");
                    Thread.Sleep(10000);
                }
            }
            
            return false;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received message: {message}");

                    // Optional: Deserialize into your object
                    var messageDto = JsonSerializer.Deserialize<LoggingQueueMessageDto>(message);
                    _loggingService.LogInformation(messageDto.Message);
                };

                _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error consuming queue: {ex.Message}");
            }

            return Task.CompletedTask;  
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
