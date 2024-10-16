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
        [ObservableProperty] // Lista que MUESTRA (VentaMostrar)
        private ObservableCollection<VentaMostrar> listaMostrarVentas = [];

        [ObservableProperty] //Lista donde se cargan todas las ventas
        private List<VentaMostrar> listaCompletaVentas = [];

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

        [RelayCommand]
        private async Task FiltrarFechas()
        {
            if (FechaInicio > FechaFin) // Validación
            {
                await Shell.Current.DisplayAlert("Error!", "La fecha de inicio no puede ser mayor que la fecha de fin.", "OK");
                return;
            }

            try
            {
                // Filtrar las ventas según las fechas seleccionadas
                var ventasFiltradas = ListaCompletaVentas
                    .Where(venta =>
                    {
                        // string -> datetime
                        if (DateTime.TryParse(venta.Fecha, out DateTime fechaVenta))
                        {
                            // DateTime -> DateOnly
                            //DateOnly dateOnlyVenta = DateOnly.FromDateTime(fechaVenta);
                            // Filtrado por rango
                            return fechaVenta >= FechaInicio && fechaVenta <= FechaFin;
                        }
                        return false; // Si no se puede convertir, se descarta la venta
                    })
                    .ToList(); // Aquí se obtiene la lista filtrada

                // Actualizar la lista de ventas mostradas
                ListaMostrarVentas = new ObservableCollection<VentaMostrar>(ventasFiltradas);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al filtrar las ventas: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
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
                var ventas = await this.VentasService.GetVentas();
                if (ventas.Count != 0)
                    ListaCompletaVentas.Clear();

                // Convertir la lista de Venta a VentaMostrar
                ListaCompletaVentas = ventas.Select(venta => new VentaMostrar(venta)).ToList();
                ListaMostrarVentas = new ObservableCollection<VentaMostrar>(ListaCompletaVentas);// Hago que se muestre todo
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"No hay ventas: {ex.Message}");
                await Shell.Current.DisplayAlert("Error al cargar las ventas!:", ex.Message, "OK");
            }
        }

    }
}
