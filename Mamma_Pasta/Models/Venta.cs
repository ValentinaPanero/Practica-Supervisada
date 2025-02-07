namespace Mamma_Pasta.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        public List<RenglonVenta>? RengVentas { get; set; }
    }
}
