using AppFarmacia.Models;
using AppFarmacia.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFarmacia.ViewModels
{
    [QueryProperty(nameof(VentaMostrar), "ventaMostrar")]
    public partial class PaginaDetalleVentaViewModel : ObservableObject
    {
        [ObservableProperty]
        private VentaMostrar? ventaMostrar;
        
        public ObservableCollection<ArticuloEnVentaMostrar> ArticulosEnVenta { get; set; } = [];
        //public AsyncRelayCommand HaciaAtrasCommand { get; }

        public async Task ObtenerDetalles()
        {
            try
            {
                var articulosDetalle = VentaMostrar?.ArticulosEnVenta ?? [];
                foreach (var aev in articulosDetalle)
                {
                    var articuloEnVentaMostrar = new ArticuloEnVentaMostrar();
                    await articuloEnVentaMostrar.InicializarAsync(aev);
                    this.ArticulosEnVenta.Add(articuloEnVentaMostrar);
                }
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
