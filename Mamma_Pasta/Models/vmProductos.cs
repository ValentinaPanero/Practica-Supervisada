using Microsoft.AspNetCore.Mvc.Rendering;
using Mamma_Pasta.Models;


namespace Mamma_Pasta.Models
{
    public class vmProductos
    {
        public SelectList ListTiposProd { get; set; }
        public List<Producto> ListProductos { get; set; }
        public string BusqNombre { get; set; }
        public int? TipoProductoId { get; set; }
        public Paginador paginadorVM { get; set; }
    }
}
