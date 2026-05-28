using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMQConsumer
{
    private readonly string _host = "localhost";
    private readonly string _vhost = "ecommerce";
    private readonly string _user = "guest";
    private readonly string _pass = "guest";

    public async Task ConsumirPedidos()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _host,
            VirtualHost = _vhost,
            UserName = _user,
            Password = _pass
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mensaje = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[✓] Cola: {ea.RoutingKey}");
            Console.WriteLine($"    Mensaje: {mensaje}\n");
            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(
            queue: "ecommerce.notificaciones",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("Esperando mensajes... Presiona Enter para salir.");
        Console.ReadLine();
    }
}