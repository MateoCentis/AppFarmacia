using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
        [ObservableProperty] //Lista donde se cargan todas las ventas
        private List<VentaMostrar> listaVentas = [];

        [ObservableProperty]
        private VentaMostrar ventaSeleccionada;

        private readonly VentasService VentasService;

        [ObservableProperty]
        private int sizePagina;

        [ObservableProperty]
        private bool paginationEnabled;

        [ObservableProperty]//Inicio de las ventas
        private DateTime fechaInicio = new DateTime(2017, 6, 1);

        [ObservableProperty]
        private DateTime fechaFin = DateTime.Now;

        public PaginaVentasViewModel()
        {
            this.VentasService = new VentasService();
            this.VentaSeleccionada = new VentaMostrar(new Venta());
            this.PaginationEnabled = true;
            this.SizePagina = 20;

            //Task.Run(async () => await ObtenerVentas());
        }

        // Redirecciona a la pantalla de detalle
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

        // Carga las ventas del sistema a la ListaCompletaVentas
        [RelayCommand]
        private async Task ObtenerVentas()
        {
            try
            {
                var ventas = await this.VentasService.GetVentas(FechaInicio, FechaFin);
                if (ventas.Count != 0)
                    ListaVentas.Clear();

                // Convertir la lista de Venta a VentaMostrar
                ListaVentas = ventas.Select(venta => new VentaMostrar(venta)).ToList();
                var perro = 5;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"No hay ventas: {ex.Message}");
                await Shell.Current.DisplayAlert("Error al cargar las ventas!:", ex.Message, "OK");
            }
        }

    }
}
