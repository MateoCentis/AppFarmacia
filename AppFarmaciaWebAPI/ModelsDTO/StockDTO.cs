namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class StockDTO
    {
        public int IdStock { get; set; }
        public int IdArticulo { get; set; }
        public DateTime Fecha { get; set; }
        public int Flujo { get; set; }
        public int CantidadActual { get; set; }
    }
}
