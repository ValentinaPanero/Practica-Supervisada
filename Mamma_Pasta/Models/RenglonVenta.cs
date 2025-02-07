namespace Mamma_Pasta.Models
{
    public class RenglonVenta
    {
        public int Id { get; set; }
        public int? VentaId { get; set; }
        public int? ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }

        public Venta? Ventas { get; set; }
        public Producto? Productos { get; set; }
    }
}
