namespace Mamma_Pasta.Models
{
    public class Paginador
    {
        public int paginaActual { get; set; }
        public int cantRegistros { get; set; }
        public int cantRegistrosPagina { get; set; }

        public int cantPaginas => (int)Math.Ceiling((decimal)cantRegistros / cantRegistrosPagina);

        public Dictionary<string, string> filtros { get; set; } = new Dictionary<string, string>();
    }
}
