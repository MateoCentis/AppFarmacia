using System.Text.Json.Serialization;

namespace AppFarmacia.Models
{
    public partial class Venta
    {
        [JsonPropertyName("idVenta")]
        public int IdVenta { get; set; }
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }
        [JsonPropertyName("articulosEnVentaDTO")]
        public ICollection<ArticuloEnVenta> ArticulosEnVenta { get; set; } = [];

        [JsonPropertyName("montoTotal")]
        public decimal MontoTotal { get; set; }


        public decimal ObtenerMontoTotal()
        {
            decimal monto = 0;
            foreach (ArticuloEnVenta aev in ArticulosEnVenta)
            {
                monto += aev.Cantidad * aev.Precio;
            }
            return monto;
        }
    }
}