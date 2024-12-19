using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
namespace AppFarmacia.Models;

using AppFarmacia.Services;

public class ArticuloEnCompra : ObservableObject
{
    private readonly StockService stockService = new();

    [JsonPropertyName("idArticuloCompra")]
    public int IdArticuloCompra { get; set; }

    [JsonPropertyName("idArticulo")]
    public int IdArticulo { get; set; }

    [JsonPropertyName("idCompra")]
    public int IdCompra { get; set; }

    [JsonPropertyName("cantidad")]
    public int Cantidad { get; set; }

    [JsonPropertyName("motivoCompra")]
    public string MotivoCompra { get; set; } = null!;

    [JsonPropertyName("nombreArticulo")]
    public string NombreArticulo { get; set; } = null!;

    public int CantidadSugerida { get; set; }
    public int CantidadFaltante { get; set; }

    public int stockActual { get; set; }

    public async Task ObtenerStockActual()
    {
        var stock = await stockService.GetUltimoStockPorArticulo(IdArticulo);
        stockActual = stock?.CantidadActual ?? 0;
    }

}
