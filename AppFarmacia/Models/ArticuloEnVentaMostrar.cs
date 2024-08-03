using AppFarmacia.Services;

namespace AppFarmacia.Models
{
    public class ArticuloEnVentaMostrar
    {
        public int IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Monto { get; set; }

        public ArticuloEnVentaMostrar()
        {
            NombreArticulo = string.Empty;
        }

        public async Task InicializarAsync(ArticuloEnVenta aev)
        {
            ArticulosService articuloService = new ArticulosService();
            Articulo? articulo = await articuloService.GetArticuloPorId(aev.IdArticulo);

            this.IdArticulo = articulo.IdArticulo;
            this.NombreArticulo = articulo.Nombre;
            this.Cantidad = aev.Cantidad;
            this.Precio = aev.Precio;
            this.Monto = this.Cantidad * this.Precio;
        }

    }
}
