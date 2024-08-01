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
        public string Categoria { get; set; }
        public decimal PrecioActual { get; set; }
        public DateOnly FechaUltimoPrecio { get; set; }
        public ICollection<ArticuloFinal> ArticulosFinales { get; set; } = [];

        public ArticuloMostrar()
        {
            this.Nombre = string.Empty;
            this.Marca = string.Empty;
            this.Descripcion = string.Empty;
            this.Categoria = string.Empty;
        }

        public async Task InicializarAsync(Articulo articulo)
        {
            this.IdArticulo = articulo.IdArticulo;
            this.Nombre = articulo.Nombre;
            this.Marca = articulo.Marca ?? string.Empty;
            this.Descripcion = articulo.Descripcion ?? string.Empty;
            this.ArticulosFinales = articulo.ArticulosFinales;

            // Si tiene categoría
            if (articulo.IdCategoria.HasValue)
            {
                CategoriasService categoriasService = new CategoriasService();
                Categoria categoria = await categoriasService.GetCategoriaPorId(articulo.IdCategoria.Value);
                this.Categoria = categoria.Nombre;
            }
            else
                this.Categoria = "-";
            
            if (articulo.Precios.Count > 0)//Si tiene algún precio
            {
                //Acá no se como vendrán ordenados los precios, tomo el primero o el último? jsjs
                Precio precio = articulo.Precios.Last();
                this.PrecioActual = precio.Valor;
                this.FechaUltimoPrecio = precio.Fecha;
            }
            else
            // Que poronga pongo si no tiene precios un artículo??
            {
                this.PrecioActual = 0m; // Precio predeterminado
                this.FechaUltimoPrecio = DateOnly.FromDateTime(DateTime.MinValue); // Fecha predeterminada
            }
        }
        }
    }
}
