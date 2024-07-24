namespace AppFarmacia.Models
{
    public class Articulo
    {
        public int IdArticulo { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Marca { get; set; }
        public string? Descripcion { get; set; }
        public int? IdCategoria { get; set; }
        public bool Activo { get; set; }
        public ICollection<ArticuloFinal> ArticulosFinales { get; set; } = [];
        public ICollection<Precio> Precios { get; set; } = [];
    }
}

