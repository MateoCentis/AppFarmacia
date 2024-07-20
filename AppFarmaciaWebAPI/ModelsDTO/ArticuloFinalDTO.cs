namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class ArticuloFinalDTO
    {
        // DTO que es relaciona otros DTO's
            // IdArticulo
            // Stocks
            // ArticuloEnVenta
        public int IdArticuloFinal { get; set; }
        public int IdArticulo { get; set; }
        public DateOnly Vencimiento { get; set; }
        public ICollection<StockDTO> Stocks { get; set; } = [];
        public ICollection<ArticuloEnVentaDTO> ArticulosEnVenta { get; set; } = [];
    }
}
