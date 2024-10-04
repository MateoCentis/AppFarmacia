using AppFarmacia.Models;
using AppFarmacia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace AppFarmacia.ViewModels
{
    [QueryProperty(nameof(ArticuloMostrar), "articuloMostrar")]
    public partial class PaginaArticuloInformacionViewModel : ObservableObject
    {
        [ObservableProperty]
        private ArticuloMostrar? articuloMostrar;

        [ObservableProperty]
        private string nombreArticulo = string.Empty;

        [ObservableProperty]
        private int idArticulo;

        [ObservableProperty]
        private List<Precio> preciosArticulo = [];

        [ObservableProperty]
        private List<Vencimiento> vencimientosArticulo = [];

        [ObservableProperty]
        private List<int> demandasMensuales = [];

        [ObservableProperty]
        private List<int> demandas_Mensuales = [];

        [ObservableProperty]
        private LineChart? demandaMensualChart;

        [ObservableProperty]
        private int yearSeleccionado = DateTime.Now.Year;

        private readonly List<string> Meses = new List<string>
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };

        private readonly PreciosService PrecioService;
        private readonly VencimientosService VencimientoService;
        private readonly ArticulosService ArticuloService;

        public PaginaArticuloInformacionViewModel()
        {
            PrecioService = new PreciosService();
            VencimientoService = new VencimientosService();
            ArticuloService = new ArticulosService();
        }
        
        partial void OnArticuloMostrarChanged(ArticuloMostrar? value)
        {
            if (value != null)
            {
                IdArticulo = value.IdArticulo;
                NombreArticulo = value.Nombre;

                // Obtengo precios y vencimientos a partir del IdArticulo
                Task.Run(async () => await CargarListasArticulo());
            }
        }

        private async Task CargarListasArticulo()
        {
            PreciosArticulo = await PrecioService.GetPreciosArticulo(IdArticulo);
            VencimientosArticulo = await VencimientoService.GetVencimientosArticulo(IdArticulo);
            DemandasMensuales = await ArticuloService.GetDemandasMensualesArticulo(IdArticulo, YearSeleccionado);
            // Una vez que tengo las demandas genero el gráfico
            GenerarGraficoDemandas();
        }

        [RelayCommand]
        static async Task HaciaAtras()
        {
            await Shell.Current.GoToAsync("..");
        }


        //Habría que hacer un gráfico de precios y otro de demanda
        [RelayCommand]
        private void GenerarGraficoDemandas()
        {
            //Definir las entries del gráfico
            var entries = new List<ChartEntry>();

            for (int i = 0; i < this.Meses.Count; ++i)
            {
                var entry = new ChartEntry(DemandasMensuales[i])
                {
                    Label = Meses[i],
                    ValueLabel = DemandasMensuales[i].ToString(),
                    Color = SKColor.Parse(GetColorForIndex(i))
                };
                entries.Add(entry);
            }

            //creo el tipo de chart
            DemandaMensualChart = new LineChart
            {   //Data
                Entries = entries,
                //Lineas
                LineMode = LineMode.Straight,
                LineSize = 8,
                //Puntos
                PointMode = PointMode.Square,
                PointSize = 18,
                //Labels
                LabelTextSize = 24,
                ValueLabelTextSize = 16,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                ValueLabelOption = ValueLabelOption.TopOfElement,
                // Lineas
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                // Colores
                EnableYFadeOutGradient = false,
                BackgroundColor = SKColor.Parse("FFFFFF"),
            };

        }

        private string GetColorForIndex(int index)
        {
            var colors = new List<string>
            {
                "#ff0000", "#00ff00", "#0000ff", "#ffff00", "#ff00ff", "#00ffff",
                "#ff9900", "#009900", "#990099", "#3399ff", "#ff3333", "#33ff33"
            };

            return colors[index % colors.Count];
        }
    }
}
