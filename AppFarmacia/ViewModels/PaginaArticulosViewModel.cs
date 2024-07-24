using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaArticulosViewModel : ObservableObject
    {
        public ObservableCollection<Articulo> ListaArticulos { get; } = [];
        ArticulosService articulosService;

        public ICommand ObtenerArticulosCommand { get; }

        public PaginaArticulosViewModel(ArticulosService articulosService)
        {
            this.articulosService = articulosService;
            ObtenerArticulosCommand = new AsyncRelayCommand(ObtenerArticulos);
        }

        
        async Task ObtenerArticulos()
        {
            try
            {
                var articulos = await articulosService.GetArticulos();
                if (articulos.Count != 0)
                    this.ListaArticulos.Clear();

                foreach (var articulo in articulos)
                    this.ListaArticulos.Add(articulo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get articles: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }


        }
    }
}
