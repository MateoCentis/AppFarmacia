namespace AppFarmacia.Models
{ 
    public partial class ArticuloFinal
    {
        public int IdArticuloFinal { get; set; }
        public int IdArticulo { get; set; }
        public DateOnly Vencimiento { get; set; }
        public ICollection<Stock> Stocks { get; set; } = [];
        public ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = [];
    }
}