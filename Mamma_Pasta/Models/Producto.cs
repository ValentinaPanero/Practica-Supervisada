

namespace Mamma_Pasta.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int TipoProductoId { get; set; }
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
        public TipoProducto? tipoProductos { get; set; }
    }
}
