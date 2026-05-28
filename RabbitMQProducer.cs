using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQProducer
{
    private readonly string _host = "localhost";
    private readonly string _vhost = "ecommerce";
    private readonly string _user = "guest";
    private readonly string _pass = "guest";

    public async Task PublicarPedido(PedidoEvent pedido)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _host,
            VirtualHost = _vhost,
            UserName = _user,
            Password = _pass
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var json = JsonSerializer.Serialize(pedido);
        var body = Encoding.UTF8.GetBytes(json);

        // 1. Direct Exchange - pedido específico
        await channel.BasicPublishAsync(
            exchange: "ecommerce.direct",
            routingKey: "pedido.nuevo",
            body: body
        );
        Console.WriteLine($"[Direct] Pedido enviado: {pedido.PedidoId}");

        // 2. Fanout Exchange - notifica a todos
        await channel.BasicPublishAsync(
            exchange: "ecommerce.fanout",
            routingKey: "",
            body: body
        );
        Console.WriteLine($"[Fanout] Pedido broadcast: {pedido.PedidoId}");

        // 3. Topic Exchange - con patrón
        await channel.BasicPublishAsync(
            exchange: "ecommerce.topic",
            routingKey: "pedido.creado",
            body: body
        );
        Console.WriteLine($"[Topic] Pedido publicado: {pedido.PedidoId}");
    }
}