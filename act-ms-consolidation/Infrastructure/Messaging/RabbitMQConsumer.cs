using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using act_ms_consolidation.Application.Interfaces;

namespace act_ms_consolidation.Infrastructure.Messaging
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RabbitMQConsumer
    {
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IConsolidationCacheHandler _consolidationCacheHandler;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQConsumer(IOptions<RabbitMQSettings> options, IConsolidationCacheHandler consolidationCacheHandler)
        {
            _rabbitMQSettings = options.Value;
            _consolidationCacheHandler = consolidationCacheHandler;
        }

        private async Task CreateConnectionAsync()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMQSettings.HostName,
                    UserName = _rabbitMQSettings.UserName,
                    Password = _rabbitMQSettings.Password
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                Console.WriteLine("RabbitMQ connection established successfully for consumer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create RabbitMQ connection: {ex.Message}");
            }
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                await CreateConnectionAsync();
            }

            if (_channel != null)
            {
                const string queueName = "transaction.queue";

                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);

                    try
                    {
                        var data = JsonConvert.DeserializeObject<dynamic>(messageJson);
                        string date = data?.date;

                        await _consolidationCacheHandler.HandleConsolidationCacheAsync(date);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }

                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                };

                await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

                Console.WriteLine("Waiting for messages...");
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
        }

        public void Stop()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            Console.WriteLine("RabbitMQ consumer connection closed.");
        }
    }
}
