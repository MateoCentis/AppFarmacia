using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Microcharts;
using CommunityToolkit.Mvvm.Input;
using AppFarmacia.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Azure;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaGraficosViewModel : ObservableObject
    {
        // Ideas de otros gráficos
            // Un gráfico que muestre la historia completa de la farmacia, sin separarla por año (para facturación y cantidad)
        //Gráficos a realizarse:
            // Horarios con más ventas por día
            // Días con más ventas de un determinado mes
            // Años con más ventas
            // Gráfico de torta con acciones terapéuticas más vendidas
            // Pensar más ....
        

        //Propiedades - Gráfico VENTAS MENSUALES ------------------------------------------
        [ObservableProperty]
        private int yearSeleccionadoGraficoVentasMensuales;

        [ObservableProperty]
        private LineChart? ventasMensualesChart;

        //Propiedades - Gráfico VENTAS DIARIAS --------------------------------------------
        [ObservableProperty]
        private int yearSeleccionadoGraficoVentasDiarias;

        [ObservableProperty]
        private int mesSeleccionadoGraficoVentasDiarias;

        [ObservableProperty]
        private string nombreMesSeleccionadoGraficoVentasDiarias;

        // Propiedades - Gráfico HORARIOS --------------------------------------------------
        [ObservableProperty]
        private string diaSeleccionado = "Lunes"; // Día de la semana seleccionado

        [ObservableProperty]
        private LineChart? ventasDiariasChart;//null hasta ser llamado

        [ObservableProperty]
        private LineChart? ventasHorariosChart;

        [ObservableProperty]
        private List<DiaSemanaDto> ventasPorHora;

        // Propiedades - Gráfico CATEGORIAS -------------------------------------------------
        [ObservableProperty]
        private PieChart? ventasPorCategoriaChart;

        private List<VentaCategoriaDto> VentasPorCategoria;

        // Propiedades - Gráfico FACTURACIONES MENSUALES -------------------------------------
        [ObservableProperty]
        private LineChart? facturacionMensualChart;

        [ObservableProperty]
        private List<FacturacionMensual> facturacionesMensuales;

        [ObservableProperty]
        private int yearSeleccionadoFacturacionMensual;

        // Propiedades - Tabla de artículos más vendidos -------------------------------------
        [ObservableProperty]
        private List<ArticuloDTO> articulosMasVendidos;

        [ObservableProperty]
        private int yearSeleccionadoArticulosMasVendidos;

        [ObservableProperty]
        private int mesSeleccionadoArticulosMasVendidos;

        [ObservableProperty]
        private int cantidadArticulosMasVendidosAMostrar = 10;

        [ObservableProperty]
        private string nombreMesSeleccionadoArticulosMasVendidos;

        // Propiedades comunes ----------------------------------------------------------------
        [ObservableProperty]
        private List<String> diasSemana = ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"];

        [ObservableProperty]
        private List<string> meses = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio",
                                                "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
        private readonly List<String> colores = ["#3498db", "#e74c3c", "#2ecc71", "#9b59b6", "#f1c40f",
            "#e67e22", "#1abc9c", "#34495e", "#e84393", "#fd79a8",
            "#95a5a6" // Color para "otros"
            ];

        public List<int> YearsDisponibles { get; } = Enumerable.Range(2017, 13).ToList(); // Años desde 2017 hasta 2030

        private readonly VentasService VentaService;

        // Inicialización de gráficos y variables
        public PaginaGraficosViewModel()
        {
            //Que se inicialicen con fechas actuales
            YearSeleccionadoGraficoVentasDiarias = DateTime.Now.Year;
            MesSeleccionadoGraficoVentasDiarias = DateTime.Now.Month;
            NombreMesSeleccionadoGraficoVentasDiarias = Meses[MesSeleccionadoGraficoVentasDiarias - 1];
            
            YearSeleccionadoGraficoVentasMensuales = DateTime.Now.Year;
            YearSeleccionadoFacturacionMensual = DateTime.Now.Year;

            MesSeleccionadoArticulosMasVendidos = DateTime.Now.Month;
            YearSeleccionadoArticulosMasVendidos = DateTime.Now.Year;
            NombreMesSeleccionadoArticulosMasVendidos = Meses[MesSeleccionadoArticulosMasVendidos - 1];

            VentaService = new VentasService();

            // TODO: Inicializar los gráficos (tirar los comandos necesarios o hacerlo bien -> que paja)

            Task.Run(async () => await GenerarGraficoVentasDiarias());
            Task.Run(async () => await GenerarGraficoVentasMensuales());
            Task.Run(async () => await GenerarGraficoCategorias());
            Task.Run(async () => await GenerarGraficoHorarios());
            Task.Run(async () => await GenerarGraficoFacturacionMensual());
            Task.Run(async () => await LlenarTablaArticulosMasVendidos());
        }


        //--------------Gráfico que muestra monto total de ventas por mes para un determinado año--------------------
        [RelayCommand]
        private async Task GenerarGraficoVentasMensuales()
        {
            // Llamar a la API que obtiene las ventas mensuales para el año seleccionado
            var ventasPorMes = await VentaService.GetCantidadVendidaPorMes(YearSeleccionadoGraficoVentasMensuales);

            var ventasMonto = new float[12];//12 meses xd

            // Asignar los valores obtenidos al array de ventas
            foreach (var venta in ventasPorMes)
            {
                ventasMonto[venta.Mes - 1] = venta.TotalCantidadVendida;
            }

            var entries = ventasMonto.Select((value, index) => new ChartEntry(value)
            {
                Label = Meses[index], // Mostrar el nombre del mes
                ValueLabel = value.ToString("F2"), // Monto del mes
                Color = SKColor.Parse("#2c3e50") // Color del gráfico
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

        //-------------------Gráfico que muestras las ventas diarias para un mes y año determinados---------------------
        [RelayCommand]
        private async Task GenerarGraficoVentasDiarias()
        {
            // Castear de string -> entero 
            MesSeleccionadoGraficoVentasDiarias = Array.IndexOf(Meses.ToArray(), NombreMesSeleccionadoGraficoVentasDiarias) + 1;
            // Data para la generación adecuada del gráfico
            var diasEnElMes = DateTime.DaysInMonth(YearSeleccionadoGraficoVentasDiarias, MesSeleccionadoGraficoVentasDiarias);
            var primerDiaDelMes = new DateTime(YearSeleccionadoGraficoVentasDiarias, MesSeleccionadoGraficoVentasDiarias, 1);

            // Llamar a la API que obtiene las ventas diarias para el mes y año seleccionados
            var ventasPorDia = await VentaService.GetCantidadVendidaPorDia(YearSeleccionadoGraficoVentasDiarias, MesSeleccionadoGraficoVentasDiarias);

            var ventas = new float[diasEnElMes];

            // Asignar los valores obtenidos al array de ventas
            foreach (var venta in ventasPorDia)
            {
                ventas[venta.Dia - 1] = venta.CantidadVendida;
            }

            int diaInicioSemana = (int)primerDiaDelMes.DayOfWeek;
            // Hacer las entries con el vector de ventas
            var entries = ventas.Select((value, index) => new ChartEntry(value)
            {
                Label = DiasSemana[(diaInicioSemana + index - 1) % 7], //Esto me lo saqué de la galera, calculo anda bien
                ValueLabel = value.ToString("F2"), //Monto del día
                Color = SKColor.Parse("#2c3e50") //Color (se le pueden llegar a poner todos los colores definidos antes)
            }).ToArray();

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

        //------------------Gráfico que muestra los horarios pico de la semana----------------------------------
        [RelayCommand]
        private async Task GenerarGraficoHorarios()
        {
            // Leer toda la info de la API
            if (VentasPorHora == null)
            {
                VentasPorHora = await VentaService.GetCantidadVendidaPorHoraSemana();
            }

            // Conversión de días (0 para domingo, 6 para sábado)
            int diaNumero = DiasSemana.IndexOf(DiaSeleccionado) + 1;
            if (diaNumero == 7) diaNumero = 0; // Para que domingo sea 0

            // Filtrar las ventas según el día
            var ventasDelDia = VentasPorHora.FirstOrDefault(v => v.DiaSemana == diaNumero);

            // Asignar las ventas a las horas
            var ventas = new List<ChartEntry>();

            if (ventasDelDia != null)
            {
                foreach (var ventaHora in ventasDelDia.VentasPorHora)
                {
                    ventas.Add(new ChartEntry(ventaHora.CantidadVendida)
                    {
                        Label = $"{ventaHora.Hora}",
                        ValueLabel = ventaHora.CantidadVendida.ToString("F0"),
                        Color = SKColor.Parse("#2c3e50")
                    });
                }
            }

            VentasHorariosChart = new LineChart
            {
                Entries = ventas.ToArray(),
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 18,
                LabelTextSize = 24,
                ValueLabelTextSize = 16,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                BackgroundColor = SKColor.Parse("FFFFFF"),
                LabelOrientation = Orientation.Horizontal,
            };
        }

        // ------------------ Gráfico de torta con las categorías más vendidas (revisar que hacer porque son muchas)------------------
        [RelayCommand]
        private async Task GenerarGraficoCategorias()
        {
            // Llamar a la API solo si no se han cargado las ventas (porque sino es bastante lento)
            if (VentasPorCategoria == null)
            {
                VentasPorCategoria = await VentaService.GetCantidadVendidaPorCategoria();
            }

            // Ordenar por cantidad vendida y tomar las 10 más vendidas
            var ventasOrdenadas = VentasPorCategoria.OrderByDescending(v => v.CantidadVendida).ToList();
            var top10Categorias = ventasOrdenadas.Take(10).ToList();

            // A las otras se las define como otros
            var cantidadOtros = ventasOrdenadas.Skip(10).Sum(v => v.CantidadVendida);

            // Hacer las entries
            var entries = top10Categorias.Select((venta, index) => new ChartEntry(venta.CantidadVendida)
            {
                Label = string.IsNullOrEmpty(venta.Categoria) ? "Sin categoría" : venta.Categoria, // Asignar nombre si está vacío
                ValueLabel = venta.CantidadVendida.ToString("F0"),
                Color = SKColor.Parse(colores[index % colores.Count]) // Asignar colores de la lista
            }).ToList();

            if (cantidadOtros > 0)
            {
                entries.Add(new ChartEntry(cantidadOtros)
                {
                    Label = "Otros",
                    ValueLabel = cantidadOtros.ToString("F0"),
                    Color = SKColor.Parse(colores.Last()) // Usar el último color para "otros"
                });
            }

            // Crear el gráfico de torta con las entradas generadas
            VentasPorCategoriaChart = new PieChart
            {
                Entries = entries.ToArray(),
                LabelTextSize = 16,
                BackgroundColor = SKColor.Parse("FFFFFF"),
                LabelMode = LabelMode.RightOnly,
            };
        }

        // ------------------- Gráfico de facturación mensual -----------------------------------------
        [RelayCommand]
        private async Task GenerarGraficoFacturacionMensual()
        {
            if (FacturacionesMensuales == null)
            {
                FacturacionesMensuales = await VentaService.GetFacturacionMensual(YearSeleccionadoFacturacionMensual);
            }

            var facturacionMonto = new float[12];

            foreach (var facturacion in FacturacionesMensuales)
            {
                facturacionMonto[facturacion.Mes - 1] = (float)facturacion.TotalFacturacion;
            }

            // Entradas para el gráfico
            var entries = facturacionMonto.Select((value, index) => new ChartEntry(value)
            {
                Label = Meses[index], // Mes
                ValueLabel = value.ToString("F2"), // Monto 
                Color = SKColor.Parse("#2c3e50") // Color
            }).ToArray();

            // Definir el gráfico con los datos cargados
            FacturacionMensualChart = new LineChart
            {
                Entries = entries,
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 18,
                LabelTextSize = 24,
                ValueLabelTextSize = 16,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                ValueLabelOption = ValueLabelOption.TopOfElement,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                EnableYFadeOutGradient = false,
                BackgroundColor = SKColor.Parse("FFFFFF"),
            };
        }
        // ------------------- Tabla de artículos más vendidos -----------------------------------------
        [RelayCommand]
        private async Task LlenarTablaArticulosMasVendidos()
        {
            MesSeleccionadoArticulosMasVendidos = Array.IndexOf(Meses.ToArray(), nombreMesSeleccionadoArticulosMasVendidos) + 1;

            ArticulosMasVendidos = await VentaService.GetArticulosMasVendidos(YearSeleccionadoArticulosMasVendidos, MesSeleccionadoArticulosMasVendidos, 
                                                                                    CantidadArticulosMasVendidosAMostrar);
        }
    }
}
