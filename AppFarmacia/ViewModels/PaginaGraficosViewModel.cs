using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Microcharts;
using CommunityToolkit.Mvvm.Input;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaGraficosViewModel : ObservableObject
    {
        // Lo que dijo Mati: 
        //Aquí se podrían incluir gráficos de ventas en pesos con segmentos temporales(por día, por semana, por
        //mes). Se pueden buscar distintas formas de evaluar las ventas:
        //- Cantidad de tickets(hace referencia a cuanta gente ingresó y compró en la farmacia)
        //- Cantidad de elementos por ticket promedio
        //- Cantidad de unidades vendidas → Aquí es clave la categorización, sobre todo en lo referido a
        //medicamentos, perfumería y dermocosmética
        //Hay mucho más para profundizar, si quieren lo vemos juntos

        //¿Gráficos que se pueden mostrar? (para ver más gráficos hablarlo con Lucas)
        //Lista de Articulos más vendidos de la farmacia?
        //Lista de artículos con más stock de la farmacia?
        //Horarios en los que más se vende diferenciando por día

        //Propiedades - Gráfico de Facturación mensual por ventas
        [ObservableProperty]
        private int yearSeleccionadoGraficoVentasMensuales;

        [ObservableProperty]
        private LineChart? ventasMensualesChart;

        //Propiedades - Gráfico de ventas diarias dado un mes y un año
        [ObservableProperty]
        private int yearSeleccionadoGraficoVentasDiarias;

        [ObservableProperty]
        private int mesSeleccionadoGraficoVentasDiarias;

        [ObservableProperty]
        private LineChart? ventasDiariasChart;//null hasta ser llamado

        // Propiedades añadidas
        public List<int> YearsDisponibles { get; } = Enumerable.Range(2000, 50).ToList(); // Años desde 2000 hasta 2049
        public List<int> MesesDisponibles { get; } = Enumerable.Range(1, 12).ToList(); // Acá estaría bueno mostrar nombres y luego parsear pero por ahora queda así


        public PaginaGraficosViewModel()
        {
            //Que se inicialicen con fechas actuales
            yearSeleccionadoGraficoVentasDiarias = DateTime.Now.Year;
            mesSeleccionadoGraficoVentasDiarias = DateTime.Now.Month;
            YearSeleccionadoGraficoVentasMensuales = DateTime.Now.Year;

            // TODO: Inicializar los gráficos (tirar los comandos necesarios o hacerlo bien -> que paja)
            Task.Run(async () => await GenerarGraficoVentasDiarias());
            Task.Run(async () => await GenerarGraficoVentasMensuales());
        }

        //Gráfico que muestra monto total de ventas por mes para un determinado año
        [RelayCommand]
        private async Task GenerarGraficoVentasMensuales()
        {
            //TODO: Obtener todas las ventas de un año y diferencias los montos por mes
            var ventasMonto = new float[MesesDisponibles.Count];
            // Se deja con DATA DEMO

            await Task.Delay(500);

            var random = new Random();
            for (int i = 0; i < MesesDisponibles.Count; i++)
            {
                ventasMonto[i] = (float)(random.NextDouble() * 10000);
            }

            var entries = ventasMonto.Select((value, index) => new ChartEntry(value)
            {
                Label = (index + 1).ToString(), //Día
                ValueLabel = value.ToString("F2"), //Monto del mes
                Color = SKColor.Parse("#2c3e50") //Color (se le pueden llegar a poner todos los colores definidos antes)
            }).ToArray();

            // Definición del gráfico ya cargados los datos
            VentasMensualesChart = new LineChart
            {
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
                // Animación
                //AnimationDuration = TimeSpan.FromMilliseconds(1500)

                //Leyenda -> esto está rompiendo no se por qué
                //LegendOption = SeriesLegendOption.Top,
            };


        }

        //Gráfico que muestras las ventas diarias para un mes y año determinados
        [RelayCommand]
        private async Task GenerarGraficoVentasDiarias()
        {
            
            var diasEnElMes = DateTime.DaysInMonth(YearSeleccionadoGraficoVentasDiarias, MesSeleccionadoGraficoVentasDiarias);
            var ventas = new float[diasEnElMes];
            // TODO: Obtener ventas de ese mes y año, luego obtener el monto por día y meterlo en "ventas"
            // Está en todo porque habría que optimizar esto y generar los comandos necesarios
            
            // Por ahora se deja con DATA DEMO
            await Task.Delay(500);

            var random = new Random();
            for (int i = 0; i < diasEnElMes; i++)
            {
                ventas[i] = (float)(random.NextDouble() * 1000);
            }


            // Hacer las entries con el vector de ventas
            var entries = ventas.Select((value, index) => new ChartEntry(value)
            {
                Label = (index + 1).ToString(), //Día
                ValueLabel = value.ToString("F2"), //Monto del día
                Color = SKColor.Parse("#2c3e50") //Color (se le pueden llegar a poner todos los colores definidos antes)
            }).ToArray();

            // Definición del gráfico ya cargados los datos
            VentasDiariasChart = new LineChart 
            { 
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




    }
}
