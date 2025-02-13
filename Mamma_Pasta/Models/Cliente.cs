namespace Mamma_Pasta.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public int Telefono { get; set; }
        public string? Direccion { get; set; }
        public int TipoDePagoId { get; set; }
        public TipoDePago? TiposDePago { get; set; }
    }
}
