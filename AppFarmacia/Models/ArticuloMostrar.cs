using AppFarmacia.Models;
using AppFarmacia.Services;

namespace AppFarmacia.Models
{
    public class ArticuloMostrar
    {
        public int IdArticulo { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Descripcion { get; set; }
        public int? IdCategoria { get; set; }
        public string Categoria { get; set; }
        public decimal PrecioActual { get; set; }
        public DateOnly FechaUltimoPrecio { get; set; }
        public int Stock {  get; set; }

        public string? Clasificacion { get; set; }
        public int? DemandaAnual { get; set; }
        public int? PuntoReposicion { get; set; }
        public int? CantidadAPedir { get; set; }
        public int? DemandaAnualHistorica { get; set; }
        public string? NombresDrogas { get; set; }

        public DateOnly UltimoVencimiento {  get; set; }

        public ICollection<Vencimiento> Vencimientos { get; set; } = [];
        public ICollection<Precio> Precios { get; set; } = [];
        public ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = [];
        public ICollection<Stock> Stocks { get; set; } = [];

        public ArticuloMostrar()
        {
            this.Nombre = string.Empty;
            this.Marca = string.Empty;
            this.Descripcion = string.Empty;
            this.Categoria = string.Empty;
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
            this.Marca = articulo.Marca ?? string.Empty;
            this.Descripcion = articulo.Descripcion ?? string.Empty;
            this.IdCategoria = articulo.IdCategoria.HasValue ? articulo.IdCategoria.Value : (int?)null;
            this.ArticulosEnVenta = articulo.ArticulosEnVenta;
            this.Stocks = articulo.Stocks;
            this.Vencimientos = articulo.Vencimientos;
            this.Precios = articulo.Precios;
            this.Clasificacion = articulo.Clasificacion ?? string.Empty;
            this.DemandaAnual = articulo.DemandaAnual.HasValue ? articulo.DemandaAnual.Value : (int?)null;
            this.PuntoReposicion = articulo.PuntoReposicion.HasValue ? articulo.PuntoReposicion.Value : (int?)null;
            this.CantidadAPedir = articulo.CantidadAPedir.HasValue ? articulo.CantidadAPedir.Value : (int?)null;
            this.DemandaAnualHistorica = articulo.DemandaAnualHistorica.HasValue ? articulo.DemandaAnualHistorica.Value : (int?)null;
            this.NombresDrogas = articulo.NombresDrogas ?? string.Empty;

            // Si tiene categoría
            if (articulo.IdCategoria.HasValue)
            {
                CategoriasService categoriasService = new CategoriasService();
                Categoria categoria = await categoriasService.GetCategoriaPorId(articulo.IdCategoria.Value);
                this.Categoria = categoria.Nombre ?? "no cargó";// Acá siempre rompe cuando la API no carga, habría que hacer la validación de null?
            }
            else
                this.Categoria = "-";
            
            if (articulo.Precios.Count > 0)//Si tiene algún precio
            {
                //Acá no se como vendrán ordenados los precios, tomo el primero o el último? jsjs
                Precio precio = articulo.Precios.First();
                this.PrecioActual = precio.Valor;
                this.FechaUltimoPrecio = precio.Fecha;
            }
            else
            // Que poronga pongo si no tiene precios un artículo??
            {
                this.PrecioActual = 0m; // Precio predeterminado
                this.FechaUltimoPrecio = DateOnly.FromDateTime(DateTime.MinValue); // Fecha predeterminada
            }

            // Esto lo comenté porque se ahora cuando se envía desde el mapeo de la API
            //if (Vencimientos.Count > 0)
            //{
            //    this.FechaVencimientoMasCercano = Vencimientos.First().Fecha;
            //}
            //else
            //{
            //    this.FechaVencimientoMasCercano = DateOnly.FromDateTime(DateTime.MinValue);
            //}
            if (Stocks.Count > 0)
            {
                Stock stock = Stocks.First();
                this.Stock = stock.CantidadActual;
            }
            else
            {
                this.Stock = 0;
            }
        }
    }
}
