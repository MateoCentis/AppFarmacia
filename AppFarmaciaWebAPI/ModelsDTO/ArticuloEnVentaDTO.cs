namespace AppFarmaciaWebAPI.ModelsDTO
{
    public class ArticuloEnVentaDTO
    {
        public int IdArticuloVenta { get; set; }
        public int Cantidad { get; set; }
        public int IdVenta { get; set; }
        public int IdArticulo { get; set; }
        public string? NombreArticulo { get; set; }
        public decimal Precio { get; set; }

    }

}
