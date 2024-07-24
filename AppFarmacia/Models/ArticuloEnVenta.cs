namespace AppFarmacia.Models
{
    public class ArticuloEnVenta
    {
        public int IdArticuloVenta { get; set; }
        public int Cantidad { get; set; }
        public int IdArticuloFinal { get; set; }
        public int IdVenta { get; set; }
        public decimal Precio { get; set; }
    }
}