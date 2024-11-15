using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
namespace AppFarmacia.Models;

public class ArticuloEnCompra : ObservableObject
{
    private int _cantidad;
    private decimal _monto;

    [JsonPropertyName("idArticuloCompra")]
    public int IdArticuloCompra { get; set; }

    [JsonPropertyName("idArticulo")]
    public int IdArticulo { get; set; }

    [JsonPropertyName("idCompra")]
    public int IdCompra { get; set; }

    [JsonPropertyName("cantidad")]
    public int Cantidad
    {
        get => _cantidad;
        set
        {
            if (SetProperty(ref _cantidad, value))
            {
                calcularMonto();
            }
        }
    }

    [JsonPropertyName("motivoCompra")]
    public string MotivoCompra { get; set; } = null!;

    public int CantidadFaltante { get; set; }

    public int CantidadSugerida { get; set; }

    

    // Si bien todo esto es medio cualquier cosa (lo del precio capaz no iría)
    public decimal Costo { get; set; }

    public string? NombreArticulo { get; set; }

    public decimal Monto
    {
        get => _monto;
        private set => SetProperty(ref _monto, value);
    }

    public void calcularMonto()
    {
        this.Monto = this.Costo * this.Cantidad;
    }
}
