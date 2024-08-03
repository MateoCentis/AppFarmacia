namespace AppFarmacia.Models
{
    public class Vencimiento
    {
        public int IdVencimiento { get; set; }

        public DateOnly Fecha { get; set; }

        public int? IdArticulo { get; set; }
    }
}
