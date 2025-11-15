using AppFarmacia.Models;
using AppFarmacia.Services;

namespace AppFarmacia.Models
{
    public class ArticuloMostrar
    {
        public int IdArticulo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int? IdCategoria { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public DateOnly FechaUltimoPrecio { get; set; }
        public int Stock {  get; set; }

        public string? Clasificacion { get; set; }
        public int? DemandaAnual { get; set; }
        public int? PuntoReposicion { get; set; }
        public int? CantidadAPedir { get; set; }
        public int? DemandaAnualHistorica { get; set; }
        public string? NombresDrogas { get; set; }

        public DateOnly? UltimoVencimiento {  get; set; }
        public Decimal UltimoPrecio { get; set; }
        public int UltimoStock { get; set; }

        public ArticuloMostrar()
        {
        }

        // Constructor optimizado: ya no hace llamadas HTTP individuales
        public ArticuloMostrar(Articulo articulo)
        {
            // Mapeo directo sin llamadas HTTP - la categoría viene del DTO
            this.IdArticulo = articulo.IdArticulo;
            this.Nombre = articulo.Nombre;
            this.Descripcion = articulo.Descripcion ?? string.Empty;
            this.IdCategoria = articulo.IdCategoria.HasValue ? articulo.IdCategoria.Value : (int?)null;
            this.Clasificacion = articulo.Clasificacion ?? string.Empty;
            this.DemandaAnual = articulo.DemandaAnual.HasValue ? articulo.DemandaAnual.Value : (int?)null;
            this.PuntoReposicion = articulo.PuntoReposicion.HasValue ? articulo.PuntoReposicion.Value : (int?)null;
            this.CantidadAPedir = articulo.CantidadAPedir.HasValue ? articulo.CantidadAPedir.Value : (int?)null;
            this.DemandaAnualHistorica = articulo.DemandaAnualHistorica.HasValue ? articulo.DemandaAnualHistorica.Value : (int?)null;
            this.NombresDrogas = articulo.NombresDrogas ?? string.Empty;
            this.UltimoPrecio = articulo.UltimoPrecio ?? 0;
            this.UltimoVencimiento = articulo.UltimoVencimiento; // Mantener null si no hay vencimiento
            this.UltimoStock = articulo.UltimoStock ?? 0;
            // Asignar Stock desde UltimoStock para que se muestre correctamente en la tabla
            this.Stock = articulo.UltimoStock ?? 0;
            
            // Usar el nombre de categoría que viene del DTO (evita N+1 queries)
            this.Categoria = !string.IsNullOrWhiteSpace(articulo.NombreCategoria) 
                ? articulo.NombreCategoria 
                : "-";
        }
    }
}
