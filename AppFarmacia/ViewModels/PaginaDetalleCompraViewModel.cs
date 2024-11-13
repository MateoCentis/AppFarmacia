using AppFarmacia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
namespace AppFarmacia.ViewModels;

[QueryProperty(nameof(IdCompra), "idCompra")]
public partial class PaginaDetalleCompraViewModel : ObservableObject
{
    //Falta implementar el servicio
    private readonly ArticuloCompraService ArticuloCompraService;

    [ObservableProperty]
    private int idCompra;

    [ObservableProperty]
    private List<ArticuloEnCompra> articulosEnCompra = [];

    public PaginaDetalleCompraViewModel()
    {
        ArticuloCompraService = new ArticuloCompraService();
    }

    public async Task ObtenerDetalles()
    {
        try
        {
            ArticulosEnCompra.Clear();
            var articulosDetalle = await ArticuloCompraService.GetArticulosEnComprasPorIdCompra(IdCompra);
            foreach (var articulo in articulosDetalle)
            {
                articulo.calcularMonto(); // Llama al método para calcular el monto
            }
            ArticulosEnCompra = new List<ArticuloEnCompra>(articulosDetalle);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
    }
}
