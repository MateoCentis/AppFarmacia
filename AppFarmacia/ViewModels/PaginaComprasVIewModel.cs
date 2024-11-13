using AppFarmacia.Models;
using AppFarmacia.Services;
using AppFarmacia.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace AppFarmacia.ViewModels;

// Esta página es como la de ventas, muestra el historial de compras con sus respectivos productos, montos y cantidades
    //También te lleva a generar la orden? (charlar esto con Lucas )
public partial class PaginaComprasVIewModel : ObservableObject
{
    [ObservableProperty]// Se cargan todas las compras acá
    private List<Compra> listaCompras = [];

    [ObservableProperty]// Se usa para redirigir a la pantalla detalle compra
    private Compra compraSeleccionada;

    private readonly CompraService ComprasService;

    [ObservableProperty]
    private int sizePagina;

    [ObservableProperty]//Inicio de las ventas
    private DateTime fechaInicio = new DateTime(2017, 6, 1);

    [ObservableProperty]
    private DateTime fechaFin = DateTime.Now;

    public PaginaComprasVIewModel()
    {
        this.ComprasService = new CompraService();
        this.CompraSeleccionada = new Compra();
        this.SizePagina = 20;

        //Task.Run(async () => await ObtenerCompras());
    }

    // Redirecciona a la pantalla de detalle
    [RelayCommand]
    async Task VerDetalle()
    {
        if (compraSeleccionada != null)
        {
            var parametroNavigation = new Dictionary<string, object>
                {
                    {"idCompra",this.compraSeleccionada.IdCompra}
                };

            await Shell.Current.GoToAsync(nameof(PaginaDetalleCompra), parametroNavigation);
        }
        else
        {
            await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ninguna venta.", "OK");
        }

    }

    // Carga las ventas del sistema a la ListaCompletaVentas
    [RelayCommand]
    private async Task ObtenerCompras()
    {
        try
        {
            var compras = await this.ComprasService.GetCompras(FechaInicio, FechaFin);
            if (compras.Count != 0)
                ListaCompras.Clear();
            // Acá iría la conversión a compraMostrar pero no parece necesario hacer otra clase
            ListaCompras = compras;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"No hay compras: {ex.Message}");
            await Shell.Current.DisplayAlert("Error al cargar las compras!:", ex.Message, "OK");
        }
    }
}
