using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMQConsumerInventario
{
    private readonly string _host = "localhost";
    private readonly string _vhost = "ecommerce";
    private readonly string _user = "guest";
    private readonly string _pass = "guest";

    public async Task ConsumirInventario()
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
            var pedido = JsonSerializer.Deserialize<PedidoEvent>(mensaje);

            Console.WriteLine($"[INVENTARIO] Nuevo pedido recibido:");
            Console.WriteLine($"  ID:       {pedido?.PedidoId}");
            Console.WriteLine($"  Producto: {pedido?.Producto}");
            Console.WriteLine($"  Cantidad: {pedido?.Cantidad}");
            Console.WriteLine($"  → Descontando stock...");
            Console.WriteLine($"  ✓ Stock actualizado correctamente!\n");

            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(
            queue: "ecommerce.inventario",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("=== Servicio de INVENTARIO escuchando ===");
        Console.WriteLine("Esperando pedidos... Presiona Enter para salir.\n");
        Console.ReadLine();
    }
}