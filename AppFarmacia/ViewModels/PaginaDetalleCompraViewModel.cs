using AppFarmacia.Models;
using AppFarmacia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace AppFarmacia.ViewModels
{
    [QueryProperty(nameof(IdCompra), "idCompra")]
    public partial class PaginaDetalleCompraViewModel : ObservableObject
    {
        //Falta implementar el servicio
        private readonly ArticuloCompraService ArticuloCompraService;
        private readonly CompraService CompraService;

        [ObservableProperty]
        private int idCompra;

        [ObservableProperty]
        private List<ArticuloEnCompra> articulosEnCompra = [];

        [ObservableProperty]
        private string? proveedor;

        [ObservableProperty]
        private string? descripcion;

        [ObservableProperty]
        private bool compraConfirmada;

        public PaginaDetalleCompraViewModel()
        {
            ArticuloCompraService = new ArticuloCompraService();
            CompraService = new CompraService();
        }

        public async Task ObtenerDetalles()
        {
            try
            {
                // Obtener los datos de la compra (proveedor y descripción)
                var compra = await CompraService.GetCompraPorId(IdCompra);
                if (compra != null)
                {
                    Proveedor = compra.Proveedor ?? "-";
                    Descripcion = compra.Descripcion ?? "-";
                    CompraConfirmada = compra.CompraConfirmada;
                }

                // Obtener los artículos de la compra
                ArticulosEnCompra.Clear();
                var articulosDetalle = await ArticuloCompraService.GetArticulosEnCompraPorId(IdCompra);
                
                System.Diagnostics.Debug.WriteLine($"[PaginaDetalleCompraViewModel] Artículos obtenidos: {articulosDetalle.Count}");
                
                // Obtener el stock actual para cada artículo de forma asíncrona
                var tareasStock = articulosDetalle.Select(async articulo =>
                {
                    await articulo.ObtenerStockActual();
                    System.Diagnostics.Debug.WriteLine($"[PaginaDetalleCompraViewModel] Stock actualizado para artículo {articulo.IdArticulo}: {articulo.StockActual}");
                    return articulo;
                });
                
                await Task.WhenAll(tareasStock);
                
                // Asignar la lista después de que todos los stocks se hayan obtenido
                ArticulosEnCompra = new List<ArticuloEnCompra>(articulosDetalle);
                System.Diagnostics.Debug.WriteLine($"[PaginaDetalleCompraViewModel] Lista de artículos asignada con {ArticulosEnCompra.Count} elementos");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }


        [RelayCommand]
        static async Task HaciaAtras()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task ConfirmarCompra()
        {
            try
            {
                var resultado = await CompraService.ConfirmarCompra(IdCompra);
                if (resultado)
                {
                    CompraConfirmada = true;
                    
                    // Actualizar los stocks actuales después de confirmar la compra
                    var tareasStock = ArticulosEnCompra.Select(async articulo =>
                    {
                        await articulo.ObtenerStockActual();
                    });
                    await Task.WhenAll(tareasStock);
                    
                    await Shell.Current.DisplayAlert("Éxito", "Compra confirmada con éxito", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo confirmar la compra", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

    }
}


