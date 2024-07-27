using System.Text.Json.Serialization;

namespace AppFarmacia.Models
{
    public class ArticuloEnVenta
    {
        [JsonPropertyName("idArticuloVenta")]
        public int IdArticuloVenta { get; set; }

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }
        
        [JsonPropertyName("idArticuloFinal")]
        public int IdArticuloFinal { get; set; }
        
        [JsonPropertyName("idVenta")]
        public int IdVenta { get; set; }
        
        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }
    }
}