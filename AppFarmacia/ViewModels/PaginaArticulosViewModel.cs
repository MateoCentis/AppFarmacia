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
        private ObservableCollection<Articulo> _listaArticulos;
        private ObservableCollection<Articulo> _listaArticulosCompleta;
        private readonly ArticulosService articulosService;
        private readonly CategoriasService categoriasService;
        public Articulo? ArticuloSeleccionado;//Sirve para implementar luego otras cosas
        private Categoria? _categoriaSeleccionada;
        public ObservableCollection<Categoria> ListCategorias { get; set; } = new ObservableCollection<Categoria>();
        private string _textoBusqueda;

        //Los ICommand se ejecutan a través de un evento de un controlador del front
        public ICommand ObtenerArticulosCommand { get; private set; }
        public ICommand ObtenerCategoriasCommand { get; private set; }
        public ICommand FiltrarCommand { get; }

        public PaginaArticulosViewModel()
        {
            this.articulosService = new ArticulosService();
            this.categoriasService = new CategoriasService();
            this._textoBusqueda = string.Empty;
            this._listaArticulos = [];
            ObtenerArticulosCommand = new Command(async () => await ObtenerArticulos());
            ObtenerCategoriasCommand = new Command(async () => await ObtenerCategorias());
            FiltrarCommand = new Command(FiltrarArticulos);

            // Carga inicial de los artículos
            Task.Run(async () => await ObtenerArticulos());
            Task.Run(async () => await ObtenerCategorias());
        }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
            }
        }

        public Categoria? CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                _categoriaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Articulo> ListaArticulos
        {
            get => _listaArticulos;
            set
            {
                if (_listaArticulos != value)
                {
                    _listaArticulos = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Articulo> ListaArticulosCompleta
        {
            get => _listaArticulosCompleta;
            set
            {
                if (_listaArticulosCompleta != value)
                {
                    _listaArticulosCompleta = value;
                    OnPropertyChanged();
                }
            }
        }

        async Task ObtenerArticulos()
        {
            try
            {
                var articulos = await articulosService.GetArticulos();
                this.ListaArticulosCompleta = new ObservableCollection<Articulo>(articulos); // ESTO SE PUEDE HACER YA QUE TENGO _listaArticulos y ListaArticulos
                // Si la cantidad de artículos es igual distinta de cero => limpio
                //if (articulos.Count != 0)
                    //this.ListaArticulos.Clear();

                //foreach (var articulo in articulos)
                    //this.ListaArticulos.Add(articulo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get articles: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        async Task ObtenerCategorias()
        {
            try
            {
                var categorias = await categoriasService.GetCategorias();
                // Si la cantidad de artículos es igual distinta de cero => limpio
                if (categorias.Count != 0)
                    this.ListCategorias.Clear();

                // Agregar la opción "Ninguna"
                var ningunaCategoria = new Categoria { Nombre = "Ninguna" };
                ListCategorias.Add(ningunaCategoria);

                foreach (var categoria in categorias)
                    ListCategorias.Add(categoria);

                // Establecer "Ninguna" como la opción seleccionada por defecto
                CategoriaSeleccionada = ningunaCategoria;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get categorias: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        private void FiltrarArticulos()
        {
            var articulosFiltrados = ListaArticulosCompleta.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                articulosFiltrados = articulosFiltrados.Where(a => a.Nombre.Contains(TextoBusqueda)).ToList();
            }
            
            if (CategoriaSeleccionada != null && CategoriaSeleccionada.Nombre != "Ninguna")
            {
                articulosFiltrados = articulosFiltrados.Where(a => a.IdCategoria == CategoriaSeleccionada.IdCategoria).ToList();
            }


            this.ListaArticulos = new ObservableCollection<Articulo>(articulosFiltrados);

            //Limpia la observableCollection y agrega los encontrados
            //ListaArticulos.Clear();
            //foreach (var articulo in articulosFiltrados)
            //{
            //    ListaArticulos.Add(articulo);
            //}
        }
    }
}
