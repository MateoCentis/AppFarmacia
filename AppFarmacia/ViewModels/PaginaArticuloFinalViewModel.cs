using AppFarmacia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AppFarmacia.ViewModels
{
    [QueryProperty("ArticuloAMostrar", "ArticuloAMostrar")]
    public partial class PaginaArticuloFinalViewModel : ObservableObject
    {
        [ObservableProperty]
        private ArticuloMostrar articuloAMostrar;

        [ObservableProperty]
        private ObservableCollection<ArticuloFinal> listaArticulosFinales;

        [ObservableProperty]
        private string nombreArticulo;
        
        public PaginaArticuloFinalViewModel()
        {
            ListaArticulosFinales = [];
            
        }
        public async Task ObtenerArticulosFinales()
        {
            try
            {
                if (ArticuloAMostrar != null)
                {
                    var articulosFinales = ArticuloAMostrar.ArticulosFinales ?? [];
                    foreach (var articuloFinal in articulosFinales)
                    {
                        this.ListaArticulosFinales.Add(articuloFinal);
                    }
                    NombreArticulo = ArticuloAMostrar.Nombre;
                }
                else
                    await Shell.Current.DisplayAlert("Error!", "Artículo no encontrado", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

    }
}
