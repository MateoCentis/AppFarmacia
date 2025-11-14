using AppFarmaciaWebAPI.Models;
using System.Text.Json.Serialization;

namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class ArticuloDTO
    {
        // Se acceden a los atributos de artículo
        // De las relaciones:
        // Una Categoria
        // Lista de precios
        // Lista de stocks
        // Lista de vencimieentos
        public int IdArticulo { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Marca { get; set; }
        public string? Descripcion { get; set; } 
        public string? Codigo { get; set; }
        public int? IdCategoria { get; set; }
        public string? NombreCategoria { get; set; }
        public bool Activo { get; set; }
        public string? Clasificacion { get; set; }
        public int? DemandaAnual { get; set; }
        public int? PuntoReposicion { get; set; }
        public int? CantidadAPedir { get; set; }
        public int? DemandaAnualHistorica { get; set; }
        public string? NombresDrogas { get; set; }
        public DateOnly? UltimoVencimiento { get; set; }
        public decimal UltimoPrecio { get; set; }
        public int UltimoStock { get; set; }

        public ICollection<PrecioDTO> PreciosDTO { get; set; } = [];
        public ICollection<VencimientoDTO> VencimientosDTO { get; set; } = [];
        public ICollection<StockDTO> StocksDTO { get; set; } = [];
        public ICollection<ArticuloEnVentaDTO> ArticulosEnVentaDTO { get; set; } = [];
        public ICollection<ArticuloEnCompraDTO> ArticulosEnCompraDTO { get; set; } = [];
        public ICollection<FaltanteDTO> FaltantesDTO { get; set; } = [];
    }
}
