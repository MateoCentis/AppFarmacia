using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using AppFarmacia.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;


namespace AppFarmacia.ViewModels
{
    public partial class PaginaPrediccionStockViewModel : ObservableObject
    {
        private readonly ArticulosService articulosService;
        private readonly CategoriasService categoriasService;
        private readonly StockService stocksService;

        [ObservableProperty]
        private int sizePagina;

        [ObservableProperty]
        private List<int> cantArticulos;

        [ObservableProperty]
        private int cantArticulosSeleccionada;


        [ObservableProperty]
        private Articulo? articuloSeleccionado;

        [ObservableProperty]
        private bool estaCargando;

        [ObservableProperty]
        private List<Articulo> listaArticulos;

        [ObservableProperty]
        private List<Articulo> listaArticulosMostrar;

        [ObservableProperty]
        private List<string> clasificaciones = ["A", "B", "C", "Todas"];

        [ObservableProperty]
        private string clasificacionSeleccionada = "Todas";

        public PaginaPrediccionStockViewModel()
        {

            this.articulosService = new ArticulosService();
            this.categoriasService = new CategoriasService();
            this.stocksService = new StockService();
            ListaArticulos = [];
            ListaArticulosMostrar = [];

            SizePagina = 20;
            CantArticulos = new List<int>([10, 50, 100, 500, 1000]);
            CantArticulosSeleccionada = 50;

            Task.Run(async () => await ObtenerArticulos());
        }

        // CARGA de artículos en lista completa
        [RelayCommand]
        private async Task ObtenerArticulos()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.EstaCargando = true;
                });

                var articulos = await articulosService.GetArticulos(CantArticulosSeleccionada);

                if (articulos == null || articulos.Count == 0)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        ListaArticulos = new List<Articulo>();
                        ListaArticulosMostrar = new List<Articulo>();
                        this.EstaCargando = false;
                    });
                    return;
                }

                // Iterar sobre cada artículo para obtener y asignar el nombre de la categoría
                foreach (var articulo in articulos)
                {
                    // Obtener el nombre de la categoría usando el IdCategoria
                    if (articulo.IdCategoria.HasValue)
                    {
                        try
                        {
                            Categoria categoria = await categoriasService.GetCategoriaPorId(articulo.IdCategoria.Value);
                            articulo.NombreCategoria = categoria?.Nombre ?? "Sin categoría";
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error obteniendo categoría para artículo {articulo.IdArticulo}: {ex.Message}");
                            articulo.NombreCategoria = "Sin categoría";
                        }
                    }
                    else
                    {
                        articulo.NombreCategoria = "Sin categoría";
                    }

                    try
                    {
                        Stock? stock = await stocksService.GetUltimoStockPorArticulo(articulo.IdArticulo);
                        articulo.UltimoStock = stock?.CantidadActual;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error obteniendo stock para artículo {articulo.IdArticulo}: {ex.Message}");
                        articulo.UltimoStock = null;
                    }
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ListaArticulos = articulos;
                    ListaArticulosMostrar = new List<Articulo>(ListaArticulos);
                    this.EstaCargando = false;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get articles: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.EstaCargando = false;
                    Shell.Current.DisplayAlert("Error al traer los artículos!", ex.Message, "OK");
                });
            }
        }

        // Filtrado 
        [RelayCommand]
        private void FiltrarArticulos()
        {
            // Filtrado por clasificación
            if (ClasificacionSeleccionada != "Todas" && !string.IsNullOrEmpty(ClasificacionSeleccionada))
            {
                ListaArticulosMostrar = ListaArticulos.Where(a => a.Clasificacion == ClasificacionSeleccionada).ToList();
            }
            else
            {
                // Si es "Todas" -> ListaArticulosMostrar es IGUAL a ListaArticulos
                ListaArticulosMostrar = new List<Articulo>(ListaArticulos);
            }
        }
    }
}
