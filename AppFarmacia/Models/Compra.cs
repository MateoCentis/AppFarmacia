using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AppFarmacia.Models
{
    public partial class Compra : ObservableObject
    {
        [JsonPropertyName("idCompra")]
        public int IdCompra { get; set; }
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }
        [JsonPropertyName("proveedor")]
        public string? Proveedor { get; set; }
        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }
        [JsonPropertyName("compraConfirmada")]
        public bool CompraConfirmada { get; set; }

        public ICollection<ArticuloEnCompra> ArticuloEnCompra { get; set; } = [];

        // Dividir entre fecha y hora
        public string FechaStr => Fecha.ToString("dd/MM/yyyy");
        public string HoraStr => Fecha.ToString("HH:mm:ss");


    }
}
