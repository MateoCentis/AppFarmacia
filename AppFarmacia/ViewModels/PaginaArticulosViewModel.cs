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
        // Servicios para acceder a la información de la API
        private readonly ArticulosService articulosService;
        private readonly CategoriasService categoriasService;

        [ObservableProperty]
        private ArticuloMostrar? articuloSeleccionado;
        
        // Propiedades para el datagrid
        [ObservableProperty]
        private int sizePagina;

        [ObservableProperty]
        private bool paginationEnabled;

        [ObservableProperty]
        private bool estaCargando;

        // Propiedades para el filtrado (por categoría, por buscador y por vencimiento)
        [ObservableProperty]
        private Categoria? categoriaSeleccionada;

        [ObservableProperty]
        private List<string> nombresCategorias;

        [ObservableProperty]
        private List<string> nombresArticulos;

        //[ObservableProperty]
        //private string? textoBusqueda;

        [ObservableProperty]
        private ObservableCollection<Categoria> listCategorias = [];

        private ObservableCollection<ArticuloMostrar> _listaArticulos;
        private ObservableCollection<ArticuloMostrar> _listaArticulosCompleta;

        [ObservableProperty]
        private List<string> tiposVencimientos = ["Vencidos", "Por vencer", "Todos"];

        [ObservableProperty]
        private string tipoVencimientoSeleccionado = "Todos";

        public PaginaArticulosViewModel()
        {
            this.articulosService = new ArticulosService();
            this.categoriasService = new CategoriasService();
            TextoBusqueda = string.Empty;
            this._listaArticulos = [];
            this._listaArticulosCompleta = [];
            NombresArticulos = [];
            NombresCategorias = [];
            PaginationEnabled = true;
            SizePagina = 20;
            CategoriaSeleccionadaNombre = "Todas";

            // Carga inicial de los artículos
            Task.Run(async () => await ObtenerCategorias());
            Task.Run(async () => await ObtenerArticulos());
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

        private string textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => textoBusqueda;
            set
            {
                if (SetProperty(ref textoBusqueda, value))// Solo si hay diferencias
                {
                    FiltrarArticulos();
                }
            }
        }

        private string _categoriaSeleccionadaNombre = string.Empty;
        public string CategoriaSeleccionadaNombre
        {
            get => _categoriaSeleccionadaNombre;
            set
            {
                if (SetProperty(ref _categoriaSeleccionadaNombre, value))
                {
                    CategoriaSeleccionada = ListCategorias.FirstOrDefault(c => c.Nombre == value);
                    //FiltrarArticulos();// Esto hace que filtre para cada letra que ingresemos
                }
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
                NombresArticulos = ListaArticulosCompleta.Select(a => a.Nombre).ToList();
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

            //Filtro por TEXTO DE BÚSQUEDA
            if (!string.IsNullOrWhiteSpace(TextoBusqueda)) 
            {  // Agregado para que se ignoren minúsculas de mayúsculas
                articulosFiltrados = articulosFiltrados.Where(a => a.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            // Filtra las CATEGORIAS, se excluye si la categoría es "Todas", nula o vacía
            if (CategoriaSeleccionada != null && CategoriaSeleccionada.Nombre != "Todas" && CategoriaSeleccionada.Nombre != "")
            { 
                articulosFiltrados = articulosFiltrados.Where(a => a.IdCategoria == CategoriaSeleccionada.IdCategoria).ToList();
            }

            //Filtro por VENCIMIENTOS(el problema de esto es que no nos traemos los vencimientos, deberíamos traernos el último
            if (TipoVencimientoSeleccionado != "Todos" && TipoVencimientoSeleccionado != null && TipoVencimientoSeleccionado != "")
            {
                DateOnly fechaActual = DateOnly.FromDateTime(DateTime.Now); // Convertir DateTime a DateOnly

                // "Por vencer" -> Vencimientos dentro de los próximos 30 días
                if (TipoVencimientoSeleccionado == "Por vencer")
                {
                    articulosFiltrados = articulosFiltrados
                        .Where(a => a.UltimoVencimiento >= fechaActual &&// Si es mayor a la fecha actual pero menor a dentro de un mes
                                    a.UltimoVencimiento <= fechaActual.AddDays(30))
                        .ToList();
                }
                // "Vencidos" -> Vencimientos menores a la fecha actual
                else if (TipoVencimientoSeleccionado == "Vencidos")
                {
                    articulosFiltrados = articulosFiltrados
                        .Where(a => a.UltimoVencimiento < fechaActual)// Si es antes de la fecha actual -> vencido
                        .ToList();
                }
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
