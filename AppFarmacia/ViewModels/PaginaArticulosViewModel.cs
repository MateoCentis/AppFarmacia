using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using AppFarmacia.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaArticulosViewModel : ObservableObject
    {
        // Poner días de stock? => Necesidad para un día de funcionamiento = unidades vendidas en los últimos 30 días / 30
        private readonly ArticulosService articulosService;
        private readonly CategoriasService categoriasService;

        [ObservableProperty]
        private ArticuloMostrar articuloSeleccionado;//Sirve para implementar luego otras cosas
        
        [ObservableProperty]
        private int sizePagina;

        [ObservableProperty]
        private bool paginationEnabled;

        [ObservableProperty]
        private bool estaCargando;

        private ObservableCollection<ArticuloMostrar> _listaArticulos;
        private ObservableCollection<ArticuloMostrar> _listaArticulosCompleta;
        private Categoria? _categoriaSeleccionada;
        private ObservableCollection<Categoria> _listCategorias;
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
            this._listaArticulosCompleta = [];
            this._listCategorias = [];
            PaginationEnabled = true;
            SizePagina = 20;

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

        public ObservableCollection<ArticuloMostrar> ListaArticulos
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

        public ObservableCollection<ArticuloMostrar> ListaArticulosCompleta
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

        public ObservableCollection<Categoria> ListCategorias
        {
            get => _listCategorias;
            set
            {
                if (_listCategorias != value)
                {
                    _listCategorias = value;
                    OnPropertyChanged();
                }
            }
        }

        async Task ObtenerArticulos()
        {
            try
            {
                this.EstaCargando = true;
                var articulos = await articulosService.GetArticulos();

                //Hay que hacer si o si el foreach acá xd

                //this.ListaArticulosCompleta = new ObservableCollection<ArticuloMostrar>(articulos); // ESTO SE PUEDE HACER YA QUE TENGO _listaArticulos y ListaArticulos
                foreach (Articulo articulo in articulos)
                {
                    var articuloMostrar = new ArticuloMostrar();
                    await articuloMostrar.InicializarAsync(articulo);
                    this.ListaArticulosCompleta.Add(articuloMostrar);
                }
                this.EstaCargando = false;
                // Si la cantidad de artículos es igual distinta de cero => limpio
                //if (articulos.Count != 0)
                    //this.ListaArticulos.Clear();

                //foreach (var articulo in articulos)
                    //this.ListaArticulos.Add(articulo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get articles: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.DisplayAlert("Error articles!", ex.Message, "OK");
                });
            }
        }

        async Task ObtenerCategorias()
        {
            try
            {
                var categorias = await categoriasService.GetCategorias();

                this.ListCategorias = new ObservableCollection<Categoria>(categorias);

                var ningunaCategoria = new Categoria { Nombre = "Todas" };//Acá estaría bueno cambiar a "Todas" porque me parece más intuitivo (también podría ser "Cualquiera")
                ListCategorias.Insert(0,ningunaCategoria);

                // Establecer "Ninguna" como la opción seleccionada por defecto
                CategoriaSeleccionada = ningunaCategoria;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get categorias: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.DisplayAlert("Error categorias!", ex.Message, "OK");
                });
            }
        }

        private void FiltrarArticulos()
        {
            var articulosFiltrados = ListaArticulosCompleta.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                articulosFiltrados = articulosFiltrados.Where(a => a.Nombre.Contains(TextoBusqueda)).ToList();
            }
            
            if (CategoriaSeleccionada != null && CategoriaSeleccionada.Nombre != "Todas")
            { 
                articulosFiltrados = articulosFiltrados.Where(a => a.IdCategoria == CategoriaSeleccionada.IdCategoria).ToList();
            }


            this.ListaArticulos = new ObservableCollection<ArticuloMostrar>(articulosFiltrados);

            //Limpia la observableCollection y agrega los encontrados
            //ListaArticulos.Clear();
            //foreach (var articulo in articulosFiltrados)
            //{
            //    ListaArticulos.Add(articulo);
            //}
        }

        [RelayCommand]
        async Task VerArticulo()
        {
            if (ArticuloSeleccionado != null)
            {
                var parametroNavigation = new Dictionary<string, object>
                {
                    {"articuloMostrar",this.ArticuloSeleccionado}
                };

                await Shell.Current.GoToAsync(nameof(PaginaArticuloInformacion), parametroNavigation);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ningun artículo.", "OK");
            }
        }
    }
}
