using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using AppFarmacia.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

 // Poner días de stock? => Necesidad para un día de funcionamiento = unidades vendidas en los últimos 30 días / 30 (PONER ESTO)
namespace AppFarmacia.ViewModels
{
    public partial class PaginaArticulosViewModel : ObservableObject
    {
        // Servicios
        private readonly ArticulosService articulosService;
        private readonly CategoriasService categoriasService;

        // Propiedades accesibles desde afuera
        [ObservableProperty]
        private ArticuloMostrar? articuloSeleccionado;//Sirve para implementar luego otras cosas
        
        [ObservableProperty]
        private int sizePagina;

        [ObservableProperty]
        private bool paginationEnabled;

        [ObservableProperty]
        private bool estaCargando;

        [ObservableProperty]
        private Categoria? categoriaSeleccionada;

        [ObservableProperty]
        private string? textoBusqueda;

        [ObservableProperty]
        private ObservableCollection<Categoria> listCategorias = [];

        private ObservableCollection<ArticuloMostrar> _listaArticulos;
        private ObservableCollection<ArticuloMostrar> _listaArticulosCompleta;

        [ObservableProperty]
        private List<string> nombresCategorias;

        public PaginaArticulosViewModel()
        {
            this.articulosService = new ArticulosService();
            this.categoriasService = new CategoriasService();
            TextoBusqueda = string.Empty;
            this._listaArticulos = [];
            this._listaArticulosCompleta = [];
            NombresCategorias = [];
            PaginationEnabled = true;
            SizePagina = 20;

            
            // Carga inicial de los artículos
            Task.Run(async () => await ObtenerArticulos());
            Task.Run(async () => await ObtenerCategorias());
        }

        // Código de mierdddda ----------------------------------------

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

        public string CategoriaSeleccionadaNombre
        {
            get => CategoriaSeleccionada?.Nombre ?? string.Empty; // El get devuelve el nombre (por cosas en el front)
            set
            {
                var categoria = ListCategorias.FirstOrDefault(c => c.Nombre == value);
                CategoriaSeleccionada = categoria;
                OnPropertyChanged();
            }
        }

        // Función que carga los artículos desde la API
        [RelayCommand]
        async Task ObtenerArticulos()
        {
            try
            {
                this.EstaCargando = true; 
                var articulos = await articulosService.GetArticulos();
                ListaArticulosCompleta = new ObservableCollection<ArticuloMostrar>(articulos.Select(a => new ArticuloMostrar(a)).ToList());
                this.EstaCargando = false;
                FiltrarArticulos();

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

        // Función que carga las categorías desde la API
        [RelayCommand]
        async Task ObtenerCategorias()
        {
            try
            {
                // Cargo las categorías e inserto la por defecto
                var categorias = await categoriasService.GetCategorias();
                ListCategorias = new ObservableCollection<Categoria>(categorias);
                var ningunaCategoria = new Categoria { Nombre = "Todas" };
                ListCategorias.Insert(0,ningunaCategoria);
                CategoriaSeleccionada = ningunaCategoria;
                NombresCategorias = ListCategorias.Select(c => c.Nombre).ToList();
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

        // Filtra artículos según el texto de búsqueda o la categoría seleccionada
        [RelayCommand]
        private void FiltrarArticulos()
        {
            var articulosFiltrados = ListaArticulosCompleta.AsEnumerable();

            //Filtro por texto de búsqueda
            if (!string.IsNullOrWhiteSpace(TextoBusqueda)) 
            {  // Agregado para que se ignoren minúsculas de mayúsculas
                articulosFiltrados = articulosFiltrados.Where(a => a.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            // Filtra las categorías, se excluye si la categoría es "Todas", nula o vacía
            if (CategoriaSeleccionada != null && CategoriaSeleccionada.Nombre != "Todas" && CategoriaSeleccionada.Nombre != "")
            { 
                articulosFiltrados = articulosFiltrados.Where(a => a.IdCategoria == CategoriaSeleccionada.IdCategoria).ToList();
            }


            this.ListaArticulos = new ObservableCollection<ArticuloMostrar>(articulosFiltrados);
        }

        // Función para redireccionar a la página de detalle de artículo
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
