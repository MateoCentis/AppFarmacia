namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class ArticuloEnVentaDTO
    {
        public int IdArticuloVenta { get; set; }
        public int Cantidad { get; set; }
        public int IdArticuloFinalDTO { get; set; }
        public int IdVentaDTO { get; set; }
        public decimal Precio { get; set; }
    }

}
