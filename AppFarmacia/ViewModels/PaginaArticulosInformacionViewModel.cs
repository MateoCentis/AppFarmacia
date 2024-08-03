using AppFarmacia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AppFarmacia.ViewModels
{
    [QueryProperty("ArticuloAMostrar", "ArticuloAMostrar")]
    public partial class PaginaArticuloInformacionViewModel : ObservableObject
    {
        [ObservableProperty]
        private ArticuloMostrar articuloAMostrar;


        [ObservableProperty]
        private string nombreArticulo;

        public PaginaArticuloInformacionViewModel()
        {
        }

        //Esto se puede usar para otras cosas
        //public async Task ObtenerArticulosFinales()
        //{
        //    try
        //    {
        //        if (ArticuloAMostrar != null)
        //        {
        //            var articulosFinales = ArticuloAMostrar.ArticulosFinales ?? [];
        //            foreach (var articuloFinal in articulosFinales)
        //            {
        //                this.ListaArticulosFinales.Add(articuloFinal);
        //            }
        //            NombreArticulo = ArticuloAMostrar.Nombre;
        //        }
        //        else
        //            await Shell.Current.DisplayAlert("Error!", "Artículo no encontrado", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        //    }
        //}

    }
}
