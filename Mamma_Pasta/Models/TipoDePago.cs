namespace Mamma_Pasta.Models
{
    public class TipoDePago
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public List<Cliente>? Clientes { get; set; }
    }
}
