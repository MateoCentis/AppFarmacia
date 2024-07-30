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
        public Articulo ArticuloSeleccionado;
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

            //ObtenerArticulosCommand = new AsyncRelayCommand(ObtenerArticulos); No uso esto porque nunca se estaria ejecutando el comando ObtenerArticulosCommand, por lo tanto no se ejecuta ObtenerArticulos
            ObtenerArticulos(); //Con esto me aseguro que se ejecuta ObtenerArticulos. Lo podemos redefinir o sino lo dejamos asi

            OrdenarPorNombreCommand = new Command(OrdenarPorNombre);
            OrdenarPorDescriptionCommand = new Command(OrdenarPorDescription);
            OrdenarPorMarcaCommand = new Command(OrdenarPorBrand);
            BusquedaArticuloCommand = new Command(BusquedaArticulo);

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

        private void BusquedaArticulo()
        {
            var articulosEncontrados = new ObservableCollection<Articulo>(ListaArticulos.
                Where(encontrado => !string.IsNullOrWhiteSpace(encontrado.Nombre) && encontrado.Nombre.Contains(TextoBusqueda)) ?? []);

            if (articulosEncontrados.Count() > 0)
            {
                ListaArticulos.Clear();
                foreach (var articulo in articulosEncontrados)
                {
                    ListaArticulos.Add(articulo);
                }
            }
        }

        private void OrdenarPorNombre()
        {
            var sortedList = ListaArticulos.OrderBy(a => a.Nombre).ToList();
            ListaArticulos.Clear();
            foreach (var articulo in sortedList)
            {
                ListaArticulos.Add(articulo);
            }
        }

        private void OrdenarPorDescription()
        {
            var sortedList = ListaArticulos.OrderBy(a => a.Descripcion).ToList();
            ListaArticulos.Clear();
            foreach (var articulo in sortedList)
            {
                ListaArticulos.Add(articulo);
            }
        }

        private void OrdenarPorBrand()
        {
            var sortedList = ListaArticulos.OrderBy(a => a.Marca).ToList();
            ListaArticulos.Clear();
            foreach (var articulo in sortedList)
            {
                ListaArticulos.Add(articulo);
            }
        }



    }
}
