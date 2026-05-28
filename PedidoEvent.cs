public class PedidoEvent
{
    public string PedidoId { get; set; }
    public string Cliente { get; set; }
    public string Producto { get; set; }
    public int Cantidad { get; set; }
    public double Total { get; set; }
    public DateTime Fecha { get; set; }

    public PedidoEvent(string pedidoId, string cliente, string producto, int cantidad, double total)
    {
        PedidoId = pedidoId;
        Cliente = cliente;
        Producto = producto;
        Cantidad = cantidad;
        Total = total;
        Fecha = DateTime.UtcNow;
    }
}