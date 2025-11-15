using AppFarmacia.Models;
using AppFarmacia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
                }

                // Obtener los artículos de la compra
                ArticulosEnCompra.Clear();
                var articulosDetalle = await ArticuloCompraService.GetArticulosEnCompraPorId(IdCompra);
                foreach (var articulo in articulosDetalle)
                {
                    articulo.ObtenerStockActual(); // Llama al método para calcular el stock actual
                }
                ArticulosEnCompra = new List<ArticuloEnCompra>(articulosDetalle);
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

    }
}


