﻿using System;
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


namespace AppFarmacia.ViewModels
{
    internal partial class PaginaPrediccionStockViewModel : ObservableObject
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

        private ObservableCollection<Articulo> _listaArticulos;

        [ObservableProperty]
        private List<string> clasificaciones = ["A", "B", "C", "Todas"];

        [ObservableProperty]
        private string clasificacionSeleccionada = "Todas";

        public ICommand? ObtenerArticulosCommand { get; private set; }

        public PaginaPrediccionStockViewModel()
        {

            this.articulosService = new ArticulosService();
            this.categoriasService = new CategoriasService();
            this.stocksService = new StockService();
            this._listaArticulos = [];

            SizePagina = 20;
            CantArticulos = new List<int>([10, 50, 100, 500, 1000]);
            CantArticulosSeleccionada = 50;

            ObtenerArticulosCommand = new Command(async () => await ObtenerArticulos());
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

        async Task ObtenerArticulos()
        {
            try
            {
                this.EstaCargando = true;

                // Obtener los artículos 
                var articulos = await articulosService.GetArticulos(CantArticulosSeleccionada);

                // Iterar sobre cada artículo para obtener y asignar el nombre de la categoría
                foreach (var articulo in articulos)
                {
                    // Obtener el nombre de la categoría usando el IdCategoria
                    if (articulo.IdCategoria.HasValue)
                    {
                        Categoria categoria = await categoriasService.GetCategoriaPorId(articulo.IdCategoria.Value);
                        articulo.NombreCategoria = categoria.Nombre;
                    }
                    else
                    {
                        articulo.NombreCategoria = "Sin categoría";
                    }

                    // Obtener el último stock del artículo
                    Stock? stock = await stocksService.GetUltimoStockPorArticulo(articulo.IdArticulo);
                    articulo.UltimoStock = stock?.CantidadActual;
                }

                // Asignar la lista de artículos a la propiedad ListaArticulos
                this.ListaArticulos = new ObservableCollection<Articulo>(articulos);

                this.EstaCargando = false;
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

        // NO está funcionando, hay que arreglarlo porque se está pisando la lista de artículos (hay que hacer una para mostrar y otra para guardar todos)
        [RelayCommand]
        private void FiltrarArticulos()
        {
            // Filtrar los artículos por clasificación
            if (ClasificacionSeleccionada != "Todas" && ClasificacionSeleccionada != null && ClasificacionSeleccionada != "")
            {
                var articulosFiltrados = ListaArticulos.Where(a => a.Clasificacion == ClasificacionSeleccionada).ToList();
                ListaArticulos = new ObservableCollection<Articulo>(articulosFiltrados);
            }
        }
    }
}
