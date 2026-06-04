using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class RabbitMQPublisher : IMessagePublisher
{
    private readonly RabbitMQConnection _connection;

    public RabbitMQPublisher(IConfiguration configuration)
    {
        _connection = new RabbitMQConnection(configuration);
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey)
    {
        await using var connection = await _connection.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            body: body
        );

        Console.WriteLine($"[Publisher] Exchange: {exchange} | RoutingKey: {routingKey} | Mensaje: {json}");
    }

    public async Task PublicarPedido(PedidoEvent pedido)
    {
        await PublishAsync(pedido, "ecommerce.direct", "pedido.nuevo");
        Console.WriteLine($"[Direct] Pedido enviado: {pedido.PedidoId}");

        await PublishAsync(pedido, "ecommerce.fanout", "");
        Console.WriteLine($"[Fanout] Pedido broadcast: {pedido.PedidoId}");

        await PublishAsync(pedido, "ecommerce.topic", "pedido.creado");
        Console.WriteLine($"[Topic] Pedido publicado: {pedido.PedidoId}");
    }
}