using act_ms_consolidation.Infrastructure.Messaging;


public class RabbitMQConsumerBGService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumerBGService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("RabbitMQ Consumer Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer>();

            try
            {
                await consumer.StartListeningAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RabbitMQ Consumer: {ex.Message}");
                await Task.Delay(5000, stoppingToken); // Aguarda 5 segundos antes de tentar novamente
            }
        }

        Console.WriteLine("RabbitMQ Consumer Service stopped.");
    }
}
