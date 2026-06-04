using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Console.WriteLine("=== Sistema de Pedidos E-commerce ===\n");

Console.WriteLine("¿Qué deseas hacer?");
Console.WriteLine("1. Publicar un pedido (Productor)");
Console.WriteLine("2. Escuchar notificaciones (Consumidor Notificaciones)");
Console.WriteLine("3. Escuchar inventario (Consumidor Inventario)");
Console.Write("\nElige una opción: ");

var opcion = Console.ReadLine();

if (opcion == "1")
{
    var producer = new RabbitMQPublisher(configuration);

    Console.WriteLine("\n--- Ingresa los datos del pedido ---");

    Console.Write("Nombre del cliente: ");
    var cliente = Console.ReadLine() ?? "Cliente";

    Console.Write("Producto: ");
    var producto = Console.ReadLine() ?? "Producto";

    Console.Write("Cantidad: ");
    int cantidad = int.TryParse(Console.ReadLine(), out int c) ? c : 1;

    Console.Write("Total ($): ");
    double total = double.TryParse(Console.ReadLine(), out double t) ? t : 0;

    var pedido = new PedidoEvent(
        pedidoId: $"PED-{new Random().Next(100, 999)}",
        cliente: cliente,
        producto: producto,
        cantidad: cantidad,
        total: total
    );

    Console.WriteLine($"\nProcesando pedido: {pedido.PedidoId}");
    Console.WriteLine($"Cliente:  {pedido.Cliente}");
    Console.WriteLine($"Producto: {pedido.Producto}");
    Console.WriteLine($"Cantidad: {pedido.Cantidad}");
    Console.WriteLine($"Total:    ${pedido.Total}\n");

    await producer.PublicarPedido(pedido);
    Console.WriteLine("\n Pedido publicado exitosamente en RabbitMQ!");
}
else if (opcion == "2")
{
    var consumer = new RabbitMQConsumer(configuration);
    await consumer.ConsumirPedidos();
}
else if (opcion == "3")
{
    var consumerInventario = new RabbitMQConsumerInventario(configuration);
    await consumerInventario.ConsumirInventario();
}