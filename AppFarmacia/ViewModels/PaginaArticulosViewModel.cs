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
        public ObservableCollection<Articulo> ListaArticulos { get; set; } = [];
        private readonly ArticulosService articulosService;
        public Articulo ArticuloSeleccionado;//Sirve para implementar luego otras cosas
        private string textoBusqueda;
        public string TextoBusqueda
        {
            get => textoBusqueda;
            set
            {
                textoBusqueda = value;
                OnPropertyChanged();
                if (textoBusqueda.Length > 0)
                {
                    BusquedaArticuloCommand.Execute(null);
                }
                else
                {
                    Task.Run(async () => await ObtenerArticulos());
                }
            }
        }
        //Los ICommand se ejecutan a través de un evento de un controlador del front
        public ICommand ObtenerArticulosCommand { get; private set; }
        public ICommand OrdenarPorNombreCommand { get; }
        public ICommand OrdenarPorDescriptionCommand { get; }
        public ICommand OrdenarPorMarcaCommand { get; }
        public ICommand BusquedaArticuloCommand { get; }

        public PaginaArticulosViewModel()
        {
            this.articulosService = new ArticulosService();
            this.textoBusqueda = string.Empty;
            ObtenerArticulosCommand = new Command(async () => await ObtenerArticulos());
            BusquedaArticuloCommand = new Command(BusquedaArticulo);

            // Carga inicial de los artículos
            Task.Run(async () => await ObtenerArticulos());
        }


        async Task ObtenerArticulos()
        {
            try
            {
                var articulos = await articulosService.GetArticulos();
                // Si la cantidad de artículos es igual distinta de cero => limpio
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

        private void BusquedaArticulo()
        {
            List<Articulo> articulosEncontrados = new ObservableCollection<Articulo>(ListaArticulos.
                Where(encontrado => !string.IsNullOrWhiteSpace(encontrado.Nombre) &&
                encontrado.Nombre.Contains(TextoBusqueda))).ToList();

            //Limpia la observableCollection y agrega los encontrados
            ListaArticulos.Clear();
            foreach (var articulo in articulosEncontrados)
            {
                ListaArticulos.Add(articulo);
            }
        }

    }
}
