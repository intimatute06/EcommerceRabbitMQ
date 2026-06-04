using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Configuration;

public class RabbitMQConsumer : IMessageConsumer
{
    private readonly RabbitMQConnection _connection;

    public RabbitMQConsumer(IConfiguration configuration)
    {
        _connection = new RabbitMQConnection(configuration);
    }

    public async Task ConsumeAsync(string queue, Func<string, Task> onMessageReceived)
    {
        var connection = await _connection.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mensaje = Encoding.UTF8.GetString(body);
            await onMessageReceived(mensaje);
            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(
            queue: queue,
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine($"[Consumer] Escuchando cola: {queue}");
        Console.ReadLine();
    }

    public async Task ConsumirPedidos()
    {
        await ConsumeAsync("ecommerce.notificaciones", async mensaje =>
        {
            Console.WriteLine($"[NOTIFICACIONES] Mensaje recibido:");
            Console.WriteLine($"  {mensaje}\n");
        });
    }
}