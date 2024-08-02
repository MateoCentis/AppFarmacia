namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class VencimientoDTO
    {
        public int IdVencimiento { get; set; }

        public DateOnly Fecha { get; set; }

        public int? IdArticulo { get; set; }
    }
}
