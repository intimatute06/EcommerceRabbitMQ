using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class RabbitMQConsumerInventario : IMessageConsumer
{
    private readonly RabbitMQConsumer _consumer;

    public RabbitMQConsumerInventario(IConfiguration configuration)
    {
        _consumer = new RabbitMQConsumer(configuration);
    }

    public async Task ConsumeAsync(string queue, Func<string, Task> onMessageReceived)
    {
        await _consumer.ConsumeAsync(queue, onMessageReceived);
    }

    public async Task ConsumirInventario()
    {
        await ConsumeAsync("ecommerce.inventario", async mensaje =>
        {
            var pedido = JsonSerializer.Deserialize<PedidoEvent>(mensaje);

            Console.WriteLine($"[INVENTARIO] Nuevo pedido recibido:");
            Console.WriteLine($"  ID:       {pedido?.PedidoId}");
            Console.WriteLine($"  Producto: {pedido?.Producto}");
            Console.WriteLine($"  Cantidad: {pedido?.Cantidad}");
            Console.WriteLine($"  Descontando stock...");
            Console.WriteLine($"  Stock actualizado correctamente!\n");
        });
    }
}