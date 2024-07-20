namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class ArticuloDTO
    {
        // Se acceden a los atributos de artículo
        // De las relaciones:
            // Una Categoria
            // Lista de precios
            // Lista de artículos finales
        public int IdArticulo { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Marca { get; set; }
        public string? Descripcion { get; set; }
        public int? IdCategoria { get; set; }
        public bool Activo { get; set; }
        public ICollection<ArticuloFinalDTO> ArticulosFinalesDTO { get; set; } = [];
        public ICollection<PrecioDTO> PreciosDTO { get; set; } = [];
    }
}
