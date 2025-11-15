using System.Text.Json.Serialization;

namespace AppFarmacia.Models
{
    public partial class Stock
    {
        [JsonPropertyName("idStock")]
        public int IdStock { get; set; }
        
        [JsonPropertyName("idArticulo")]
        public int IdArticulo { get; set; }
        
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }
        
        [JsonPropertyName("cantidadActual")]
        public int CantidadActual { get; set; }
    }
}