using ProductService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Models;

namespace ProductService.RabbitMQ.Implementation
{
    public class RabbitMqStockCheckConsumer : IHostedService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _requestQueueName = "productservice";

        public RabbitMqStockCheckConsumer(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory { HostName = configuration["RabbitMQ:Host"] };
            Initialize(factory);            
        }

        private bool Initialize(ConnectionFactory factory)
        {
            int retryCount = 5;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {                    
                    _connection = factory.CreateConnectionAsync().Result;
                    _channel = _connection.CreateChannelAsync().Result;
                    _channel.QueueDeclareAsync(queue: _requestQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false).Wait();
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleMessageAsync;

            await _channel.BasicConsumeAsync(_requestQueueName, false, consumer);
        }

        private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            AsyncEventingBasicConsumer consumer = (AsyncEventingBasicConsumer)sender;
            IChannel channel = consumer.Channel;
            string response = "false";

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var stockCheckResponse = new StockCheckResponse();
                    var productsService = scope.ServiceProvider.GetRequiredService<IProductsService>();

                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var request = JsonSerializer.Deserialize<StockCheckRequest>(message);

                    var product = await productsService.GetProductById(request.ProductId);

                    stockCheckResponse.ProductId = request.ProductId;

                    var hasStock = product != null && product.AvailableStock >= request.RequestedQuantity;
                    if (hasStock)
                    {
                        var updateDto = new UpdateProductDto
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            AvailableStock = product.AvailableStock - request.RequestedQuantity
                        };
                        
                        stockCheckResponse.AvailableStock = updateDto.AvailableStock;
                        stockCheckResponse.InStock = true;
                        stockCheckResponse.totalAmount = updateDto.Price * request.RequestedQuantity;

                        await productsService.UpdateProduct(updateDto);
                    }
                    else
                    {
                        stockCheckResponse.AvailableStock = product?.AvailableStock ?? 0;
                        stockCheckResponse.totalAmount = (product?.Price ?? 0) * request.RequestedQuantity;
                        stockCheckResponse.InStock = false;
                    }

                    response = JsonSerializer.Serialize(stockCheckResponse);
                }
            }
            catch (Exception)
            {
                response = JsonSerializer.Serialize(false);
            }
            finally
            {
                var replyProps = new BasicProperties
                {
                    CorrelationId = ea.BasicProperties.CorrelationId
                };

                var responseBytes = Encoding.UTF8.GetBytes(response);
                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: ea.BasicProperties.ReplyTo!,
                    mandatory: true,
                    basicProperties: replyProps,
                    body: responseBytes);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel.IsOpen)
                await _channel.CloseAsync();
            if (_connection.IsOpen)
                await _connection.CloseAsync();
        }

        private class StockCheckRequest
        {
            public int ProductId { get; set; }
            public int RequestedQuantity { get; set; }
        }

        private class StockCheckResponse
        {
            public int ProductId { get; set; }
            public int AvailableStock { get; set; }
            public bool InStock { get; set; }
            public decimal totalAmount { get; set; }
        }
    }
} 