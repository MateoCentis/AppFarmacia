using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
namespace AppFarmacia.Models;

using AppFarmacia.Services;

public partial class ArticuloEnCompra : ObservableObject
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

    [ObservableProperty]
    private int stockActual;

    public async Task ObtenerStockActual()
    {
        System.Diagnostics.Debug.WriteLine($"[ArticuloEnCompra] Obteniendo stock para artículo ID: {IdArticulo}");
        var stock = await stockService.GetUltimoStockPorArticulo(IdArticulo);
        StockActual = stock?.CantidadActual ?? 0;
        System.Diagnostics.Debug.WriteLine($"[ArticuloEnCompra] Stock obtenido para artículo {IdArticulo}: {StockActual}");
    }

}
