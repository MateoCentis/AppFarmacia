namespace AppFarmacia.Models
{
    public partial class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = null!;
        public ICollection<Articulo> Articulos { get; set; } = [];
    }
}