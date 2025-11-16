using AppFarmacia.Models;
using AppFarmacia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Linq;

namespace AppFarmacia.ViewModels;

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

    [ObservableProperty]
    private List<int> añosDisponibles = [];

    partial void OnYearSeleccionadoChanged(int value)
    {
        // Recargar las demandas cuando cambie el año seleccionado
        if (IdArticulo > 0)
        {
            _ = RecargarDemandasPorAño();
        }
    }

    private async Task RecargarDemandasPorAño()
    {
        try
        {
            EstaCargando = true;
            var demandasTask = ArticuloService.GetDemandasMensualesArticulo(IdArticulo, YearSeleccionado);
            await demandasTask;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                DemandasMensuales = demandasTask.Result;
                
                if (DemandasMensuales != null && DemandasMensuales.Count >= 12)
                {
                    GenerarGraficoDemandas();
                }
                else
                {
                    DemandasMensuales = Enumerable.Repeat(0, 12).ToList();
                    GenerarGraficoDemandas();
                }

                EstaCargando = false;
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                EstaCargando = false;
                Shell.Current.DisplayAlert("Error", $"Error al cargar las demandas del año {YearSeleccionado}: {ex.Message}", "OK");
            });
        }
    }

    [ObservableProperty]
    private bool estaCargando;

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
        
        // Inicializar años disponibles (últimos 5 años y año actual)
        var añoActual = DateTime.Now.Year;
        AñosDisponibles = Enumerable.Range(añoActual - 4, 6).Reverse().ToList();
    }
    
    partial void OnArticuloMostrarChanged(ArticuloMostrar? value)
    {
        if (value != null)
        {
            IdArticulo = value.IdArticulo;
            NombreArticulo = value.Nombre;

            // Cargar datos de forma asíncrona y optimizada
            _ = CargarListasArticuloAsync();
        }
    }

    private async Task CargarListasArticuloAsync()
    {
        try
        {
            EstaCargando = true;

            // Cargar los tres datos en paralelo para mejorar el rendimiento
            var preciosTask = PrecioService.GetPreciosArticulo(IdArticulo);
            var vencimientosTask = VencimientoService.GetVencimientosArticulo(IdArticulo);
            var demandasTask = ArticuloService.GetDemandasMensualesArticulo(IdArticulo, YearSeleccionado);

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(preciosTask, vencimientosTask, demandasTask);

            // Actualizar propiedades en el hilo principal
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                PreciosArticulo = preciosTask.Result;
                VencimientosArticulo = vencimientosTask.Result;
                DemandasMensuales = demandasTask.Result;

                // Generar el gráfico siempre, incluso si todos los valores son cero
                // Esto asegura que el gráfico se muestre aunque no haya datos
                if (DemandasMensuales != null && DemandasMensuales.Count >= 12)
                {
                    GenerarGraficoDemandas();
                }
                else
                {
                    // Si no hay suficientes datos, crear un gráfico con valores por defecto (ceros)
                    DemandasMensuales = Enumerable.Repeat(0, 12).ToList();
                    GenerarGraficoDemandas();
                }

                EstaCargando = false;
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                EstaCargando = false;
                Shell.Current.DisplayAlert("Error", $"Error al cargar los datos del artículo: {ex.Message}", "OK");
            });
        }
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
        // Validar que tenemos datos suficientes
        if (DemandasMensuales == null || DemandasMensuales.Count < 12)
        {
            // Si no hay datos, crear un gráfico con ceros
            DemandasMensuales = Enumerable.Repeat(0, 12).ToList();
        }

        //Definir las entries del gráfico
        var entries = new List<ChartEntry>();
        var maxDemanda = DemandasMensuales.Any() ? DemandasMensuales.Max() : 1;

        for (int i = 0; i < this.Meses.Count && i < DemandasMensuales.Count; ++i)
        {
            var demanda = DemandasMensuales[i];
            // Usar un color más suave cuando el valor es cero
            var color = demanda > 0 
                ? SKColor.Parse(GetColorForIndex(i)) 
                : SKColor.Parse("#CCCCCC"); // Gris claro para valores cero
            
            var entry = new ChartEntry(demanda)
            {
                Label = Meses[i],
                ValueLabel = demanda.ToString(),
                Color = color
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
            // Asegurar que el gráfico se muestre incluso si todos los valores son cero
            MinValue = 0,
            MaxValue = maxDemanda > 0 ? maxDemanda : 10, // Si todos son cero, usar un máximo de 10 para que se vea el gráfico
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
