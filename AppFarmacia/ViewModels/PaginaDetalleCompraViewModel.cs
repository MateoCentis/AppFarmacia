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
                var articulosDetalle = await ArticuloCompraService.GetArticulosEnCompraPorId(IdCompra);
                foreach (var articulo in articulosDetalle)
                {
                    articulo.ObtenerStockActual(); // Llama al método para calcular el monto
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


