namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class StockDTO
    {
        public int IdStock { get; set; }
        public int IdArticuloFinalDTO { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadActual { get; set; }
    }
}
