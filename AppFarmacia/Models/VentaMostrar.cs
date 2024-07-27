namespace AppFarmacia.Models
{
    public class VentaMostrar
    {
        public int Id { get; set; }
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public decimal MontoTotal { get; set; }
        public ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = [];

        public VentaMostrar(Venta venta)
        {
            this.Id = venta.IdVenta;
            this.Fecha = venta.Fecha.ToString("dd-MM-yy");
            this.Hora = venta.Fecha.ToString("HH:mm");
            this.MontoTotal = venta.ObtenerMontoTotal();
            this.ArticulosEnVenta = venta.ArticulosEnVenta;
        }

    }
}
