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

        public DateOnly UltimoVencimiento {  get; set; }
        public Decimal UltimoPrecio { get; set; }
        public int UltimoStock { get; set; }

        public ArticuloMostrar()
        {
        }

        public ArticuloMostrar(Articulo articulo)
        {
            InicializarAsync(articulo).Wait();
        }

        public async Task InicializarAsync(Articulo articulo)
        {
            // Van derecho
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
            this.UltimoVencimiento = articulo.UltimoVencimiento ?? new DateOnly();
            this.UltimoStock = articulo.UltimoStock ?? 0;

            // Si tiene categoría
            if (articulo.IdCategoria.HasValue)
            {
                CategoriasService categoriasService = new CategoriasService();
                Categoria categoria = await categoriasService.GetCategoriaPorId(articulo.IdCategoria.Value);
                this.Categoria = categoria.Nombre ?? "no cargó";// Acá siempre rompe cuando la API no carga, habría que hacer la validación de null?
            }
            else
                this.Categoria = "-";
            
        }
    }
}
