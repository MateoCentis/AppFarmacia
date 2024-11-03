using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
namespace AppFarmacia.Models;

public class ArticuloEnCompra : ObservableObject
{
    private int _cantidadEncargada;
    private decimal _monto;

    [JsonPropertyName("idArticuloCompra")]
    public int IdArticuloCompra { get; set; }

    [JsonPropertyName("idArticulo")]
    public int IdArticulo { get; set; }

    [JsonPropertyName("idCompra")]
    public int IdCompra { get; set; }

    [JsonPropertyName("cantidadSugerida")]
    public int CantidadSugerida { get; set; }

    [JsonPropertyName("cantidadEncargada")]
    public int CantidadEncargada
    {
        get => _cantidadEncargada;
        set
        {
            if (SetProperty(ref _cantidadEncargada, value))
            {
                calcularMonto();
            }
        }
    }

    // Si bien todo esto es medio cualquier cosa (lo del precio capaz no iría
    [JsonPropertyName("costo")]
    public decimal Costo { get; set; }

    [JsonPropertyName("nombreArticulo")]
    public string? NombreArticulo { get; set; }

    public decimal Monto
    {
        get => _monto;
        private set => SetProperty(ref _monto, value);
    }

    public void calcularMonto()
    {
        this.Monto = this.Costo * this.CantidadEncargada;
    }
}
