namespace AppFarmacia.Models
{
    public class Articulo
    {
        public int IdArticulo { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Marca { get; set; }
        public string? Descripcion { get; set; }
        public string? Codigo { get; set; }
        public int? IdCategoria { get; set; }
        public bool Activo { get; set; }

        public ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = [];
        public ICollection<Precio> Precios { get; set; } = [];
        public ICollection<Vencimiento> Vencimientos { get; set; } = [];
        public ICollection<Stock> Stocks { get; set; } = [];
    }
}

