using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using act_ms_transaction.Application.Interfaces;

namespace act_ms_transaction.Infrastructure.Messaging
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TransactionPublisher: IMessageService
    {
        private readonly RabbitMQSettings _rabbitMQSettings;
        private IConnection? _connection;

        public TransactionPublisher(IOptions<RabbitMQSettings> options)
        {
            _rabbitMQSettings = options.Value;
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
                Console.WriteLine("RabbitMQ connection established successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create RabbitMQ connection: {ex.Message}");
            }
        }

        public async Task PublishAsync(string date)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                await CreateConnectionAsync();
            }

            if (_connection != null)
            {
                using var channel = await _connection.CreateChannelAsync();

                // Declarar o exchange
                await channel.ExchangeDeclareAsync(
                    exchange: "transaction.exchange",
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false
                );

                // Declarar a fila
                var queueName = "transaction.queue";
                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                // Vincular a fila ao exchange
                await channel.QueueBindAsync(
                    queue: queueName,
                    exchange: "transaction.exchange",
                    routingKey: ""
                );

                var message = new { date };
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                    exchange: "transaction.exchange",
                    routingKey: "",
                    body: body
                );

                Console.WriteLine($"Message sent to RabbitMQ: {json}");
            }
        }
    }
}
