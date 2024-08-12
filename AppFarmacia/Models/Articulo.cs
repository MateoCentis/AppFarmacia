using System.Text.Json.Serialization;

namespace AppFarmacia.Models
{
    public class Articulo
    {
        [JsonPropertyName("idArticulo")]
        public int IdArticulo { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = null!;

        [JsonPropertyName("marca")]
        public string? Marca { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("idCategoria")]
        public int? IdCategoria { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; }

        [JsonPropertyName("idArticuloVentaDTO")]
        public ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = [];

        [JsonPropertyName("preciosDTO")]
        public ICollection<Precio> Precios { get; set; } = [];

        [JsonPropertyName("vencimientosDTO")]
        public ICollection<Vencimiento> Vencimientos { get; set; } = [];

        [JsonPropertyName("stocksDTO")]
        public ICollection<Stock> Stocks { get; set; } = [];
    }
}

