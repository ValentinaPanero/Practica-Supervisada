using Mamma_Pasta.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mamma_Pasta.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<TipoProducto> TiposProductos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<RenglonVenta> renglonesVentas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<TipoDePago> TiposDePago { get; set; }
    }
}
