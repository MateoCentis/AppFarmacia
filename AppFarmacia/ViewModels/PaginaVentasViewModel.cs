using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using AppFarmacia.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaVentasViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<VentaMostrar> listaVentas = [];
        
        [ObservableProperty]
        private VentaMostrar ventaSeleccionada;

        private readonly VentasService VentasService;
        //public VentaMostrar VentaSeleccionada { get; set; }
        //public IAsyncRelayCommand VerDetalleCommand { get; }

        [ObservableProperty]
        private int sizePagina;

        [ObservableProperty]
        private bool paginationEnabled;

        public PaginaVentasViewModel()
        {
            this.VentasService = new VentasService();
            this.VentaSeleccionada = new VentaMostrar(new Venta());
            this.PaginationEnabled = true;
            this.SizePagina = 20;

            //VerDetalleCommand = new AsyncRelayCommand(VerDetalle);

        }

        [RelayCommand]
        async Task VerDetalle()
        {
            if (VentaSeleccionada != null)
            {
                var parametroNavigation = new Dictionary<string, object>
                {
                    {"idVenta",this.VentaSeleccionada.Id}
                };

                await Shell.Current.GoToAsync(nameof(PaginaDetalleVenta),parametroNavigation);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ninguna venta.", "OK");
            }

        }
        
        public async Task ObtenerVentas()
        {
            try
            {
                var ventas = await this.VentasService.GetVentas();
                if (ventas.Count != 0)
                    this.ListaVentas.Clear();

                // ¿Esto será lo que demora?
                foreach (Venta venta in ventas)
                    this.ListaVentas.Add(new VentaMostrar(venta));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"No hay ventas: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

    }
}
