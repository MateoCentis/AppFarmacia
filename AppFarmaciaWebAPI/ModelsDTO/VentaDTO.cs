namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class VentaDTO
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public ICollection<ArticuloEnVentaDTO> ArticulosEnVentaDTO { get; set; } = [];
        public decimal MontoTotal { get; set; }
    }
}