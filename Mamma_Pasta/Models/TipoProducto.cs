namespace Mamma_Pasta.Models
{
    public class TipoProducto
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }

        public List<Producto>? Productos { get; set; }

    }
}
