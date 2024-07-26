using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaVentasViewModel : ObservableObject
    {
        public ObservableCollection<Venta> ListaVentas { get; set; } = [];
        private readonly VentasService ventasService;

        public PaginaVentasViewModel()
        {
            this.ventasService = new VentasService();

        }

        async Task ObtenerVentas()
        {
            try
            {
                var ventas = await ventasService.GetVentas();
                if (ventas.Count != 0)
                    this.ListaVentas.Clear();

                foreach (var articulo in ventas)
                    this.ListaVentas.Add(articulo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get articles: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

    }
}
