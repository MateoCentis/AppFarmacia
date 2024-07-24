namespace AppFarmacia.Models
{
    public partial class Venta
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = [];
    }
}