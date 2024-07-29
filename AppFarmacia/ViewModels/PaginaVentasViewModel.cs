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
        public ObservableCollection<VentaMostrar> ListaVentas { get; set; } = [];
        public VentaMostrar VentaSeleccionada { get; set; }
        public ICommand RecargarPaginaCommand { get; }
        public ICommand ActualizandoPaginaCommand { get; }
        public IAsyncRelayCommand VerDetalleCommand { get; }
        private readonly VentasService ventasService;

        public PaginaVentasViewModel()
        {
            this.ventasService = new VentasService();
            this.VentaSeleccionada = new VentaMostrar(new Venta());

            RecargarPaginaCommand = new Command(RecargarPagina);
            ActualizandoPaginaCommand = new Command(ActualizandoPagina);
            VerDetalleCommand = new AsyncRelayCommand(VerDetalle);

        }

        async Task VerDetalle()
        {
            if (VentaSeleccionada != null)
            {
                var parametroNavigation = new Dictionary<string, object>
                {
                    {"ventaMostrar",this.VentaSeleccionada}
                };

                await Shell.Current.GoToAsync(nameof(PaginaDetalleVenta),parametroNavigation);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ninguna venta.", "OK");
            }

        }

        private void RecargarPagina()
        {
            throw new NotImplementedException();
        }

        private void ActualizandoPagina()
        { 
            throw new NotImplementedException(); 
        }

        public async Task ObtenerVentas()
        {
            try
            {
                var ventas = await ventasService.GetVentas();
                if (ventas.Count != 0)
                    this.ListaVentas.Clear();

                foreach (var venta in ventas)
                    this.ListaVentas.Add(new VentaMostrar(venta));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get articles: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

    }
}
