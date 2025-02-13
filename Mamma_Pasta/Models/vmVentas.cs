using Mamma_Pasta.Models;

namespace Mamma_Pasta.Models
{
    public class vmVentas
    {
        public Venta? Ventas { get; set; }
        public List<RenglonVenta> RenglonesVentas { get; set; } = new List<RenglonVenta>();
        public Producto? Productos { get; set; }
        public int clienteId { get; set; }
        public Cliente? Clientes { get; set; }  
    }
}
