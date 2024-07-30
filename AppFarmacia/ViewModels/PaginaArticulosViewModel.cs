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
        private readonly CategoriasService categoriasService;
        public Articulo? ArticuloSeleccionado;//Sirve para implementar luego otras cosas
        private Categoria? _categoriaSeleccionada;
        public ObservableCollection<Categoria> ListCategorias { get; set; } = [];
        private string _textoBusqueda;
        
        //Los ICommand se ejecutan a través de un evento de un controlador del front
        public ICommand ObtenerArticulosCommand { get; private set; }
        public ICommand ObtenerCategoriasCommand { get; private set; }
        public ICommand? OrdenarPorNombreCommand { get; }
        public ICommand? OrdenarPorDescriptionCommand { get; }
        public ICommand? OrdenarPorMarcaCommand { get; }
        public ICommand BusquedaArticuloTextoCommand { get; }
        public ICommand BusquedaArticuloCategoriaCommand { get; }

        public PaginaArticulosViewModel()
        {
            this.articulosService = new ArticulosService();
            this.categoriasService = new CategoriasService();
            this._textoBusqueda = string.Empty;
            ObtenerArticulosCommand = new Command(async () => await ObtenerArticulos());
            ObtenerCategoriasCommand = new Command(async () => await ObtenerCategorias());
            BusquedaArticuloTextoCommand = new Command(BusquedaArticuloTexto);
            BusquedaArticuloCategoriaCommand = new Command(BusquedaArticuloCategoria);

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
                if (_textoBusqueda.Length > 0)
                {
                    BusquedaArticuloTextoCommand.Execute(null);
                }
                else
                {
                    Task.Run(async () => await ObtenerArticulos());
                }
            }
        }

        public Categoria? CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                _categoriaSeleccionada = value;
                OnPropertyChanged();

                if (CategoriaSeleccionada != null)
                {
                    if (CategoriaSeleccionada.Nombre != "Ninguna")
                    {
                        BusquedaArticuloCategoriaCommand.Execute(null);

                    }
                    else
                    {
                        Task.Run(async () => await ObtenerArticulos());
                    }
                }
                
            }
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

        private void BusquedaArticuloTexto()
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

        private void BusquedaArticuloCategoria()
        {
            if (CategoriaSeleccionada != null)
            {
                List<Articulo> articulosEncontrados = new ObservableCollection<Articulo>(ListaArticulos.
                            Where(encontrado => !string.IsNullOrWhiteSpace(encontrado.Nombre) &&
                            encontrado.Nombre.Contains(TextoBusqueda) && encontrado.IdCategoria == CategoriaSeleccionada.IdCategoria)).ToList();

                //Limpia la observableCollection y agrega los encontrados
                ListaArticulos.Clear();
                foreach (var articulo in articulosEncontrados)
                {
                    ListaArticulos.Add(articulo);
                }
            }
        }

    }
}
