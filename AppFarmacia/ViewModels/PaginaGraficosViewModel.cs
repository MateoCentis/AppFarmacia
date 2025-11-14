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
using Microsoft.Maui.ApplicationModel;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaGraficosViewModel : ObservableObject
    {
        // Ideas de otros gráficos
            // Un gráfico que muestre la historia completa de la farmacia, sin separarla por año (para facturación y cantidad)

        //Propiedades - Gráfico VENTAS MENSUALES ------------------------------------------
        [ObservableProperty]
        private int yearSeleccionadoGraficoVentasMensuales;

        [ObservableProperty]
        private LineChart? ventasMensualesChart;

        [ObservableProperty]
        private bool estaCargandoVentasMensuales;

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

        [ObservableProperty]
        private int cantidadCategoriasAMostrar = 15;

        [ObservableProperty]
        private bool categoriaOtros = true;

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

        [ObservableProperty]
        private List<int> cantidadesArticulosMasVendidos = [10, 50, 100];

        // Propiedades - HISTÓRICO ------------------------------------------------------------
        [ObservableProperty]
        private LineChart? facturacionMensualHistoricoChart;

        [ObservableProperty]
        private LineChart? ventasMensualesHistoricoChart;

        // Propiedades comunes ----------------------------------------------------------------
        [ObservableProperty]
        private List<String> diasSemana = ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"];

        [ObservableProperty]
        private List<string> meses = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio",
                                                "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];

        private readonly List<string> colores = new List<string>
        {
            "#3498db",  // Azul
            "#e74c3c",  // Rojo
            "#2ecc71",  // Verde
            "#9b59b6",  // Morado
            "#f1c40f",  // Amarillo
            "#e67e22",  // Naranja
            "#1abc9c",  // Turquesa
            "#34495e",  // Gris Oscuro
            "#e84393",  // Rosa Fuerte
            "#fd79a8",  // Rosa Claro
            "#8e44ad",  // Morado Oscuro
            "#2c3e50",  // Azul Oscuro
            "#d35400",  // Naranja Oscuro
            "#27ae60",  // Verde Oscuro
            "#f39c12",  // Amarillo Mostaza
            "#c0392b",  // Rojo Oscuro
            "#16a085",  // Verde Agua
            "#2980b9",  // Azul Marino
            "#d63031",  // Rojo Intenso
            "#95a5a6"   // Gris claro para "otros"
        };

        public List<int> YearsDisponibles { get; } = Enumerable.Range(2017, 13).ToList(); // Años desde 2017 hasta 2030

        private readonly VentasService VentaService;
        private readonly LoggerService? logger;

        private readonly int ValueLabelSize = 16;
        private readonly int LineSize = 6;
        private readonly int PointSize = 16;
        private readonly int LabelTextSize = 20;

        private bool _isInitialized = false;

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

            logger = new LoggerService();
            VentaService = new VentasService(logger);

            // TODO: Inicializar los gráficos (tirar los comandos necesarios o hacerlo bien -> que paja)

            Task.Run(async () => 
            {
                await GenerarGraficoVentasDiarias();
                await GenerarGraficoVentasMensuales();
                await GenerarGraficoCategorias();
                await GenerarGraficoHorarios();
                await GenerarGraficoFacturacionMensual();
                await LlenarTablaArticulosMasVendidos();
                await GenerarGraficoHistoricoFarmacia();
                
                // Marcar como inicializado después de cargar todos los gráficos
                _isInitialized = true;
            });
        }


        // Método que se ejecuta cuando cambia el año seleccionado
        partial void OnYearSeleccionadoGraficoVentasMensualesChanged(int value)
        {
            // Solo regenerar el gráfico si ya se inicializó (evitar ejecución durante la inicialización)
            if (_isInitialized && value > 0)
            {
                // Regenerar el gráfico cuando cambia el año
                _ = GenerarGraficoVentasMensualesAsync();
            }
        }

        //--------------Gráfico que muestra monto total de ventas por mes para un determinado año--------------------
        [RelayCommand]
        private async Task GenerarGraficoVentasMensuales()
        {
            await GenerarGraficoVentasMensualesAsync();
        }

        private async Task GenerarGraficoVentasMensualesAsync()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    EstaCargandoVentasMensuales = true;
                });

                // Llama a la API que obtiene las ventas mensuales para el año seleccionado
                var ventasPorMes = await VentaService.GetCantidadVendidaPorMes(YearSeleccionadoGraficoVentasMensuales);

                var ventasMonto = new float[12];

                // Asignar los valores obtenidos al array de ventas
                if (ventasPorMes != null && ventasPorMes.Any())
                {
                    foreach (var venta in ventasPorMes)
                    {
                        // Validar que el mes esté en el rango correcto
                        if (venta.Mes >= 1 && venta.Mes <= 12)
                        {
                            // Asignar la cantidad vendida (convertir a float)
                            ventasMonto[venta.Mes - 1] = (float)venta.TotalCantidadVendida;
                        }
                    }
                }
                else
                {
                    // Si no hay datos, inicializar con ceros
                    for (int i = 0; i < 12; i++)
                    {
                        ventasMonto[i] = 0f;
                    }
                }

                // Debug: verificar que los datos se están recibiendo
                logger?.LogInfo($"GenerarGraficoVentasMensualesAsync: Año seleccionado: {YearSeleccionadoGraficoVentasMensuales}");
                logger?.LogInfo($"Cantidad de registros recibidos: {ventasPorMes?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Año seleccionado: {YearSeleccionadoGraficoVentasMensuales}");
                System.Diagnostics.Debug.WriteLine($"Cantidad de registros recibidos: {ventasPorMes?.Count ?? 0}");
                
                if (ventasPorMes != null && ventasPorMes.Any())
                {
                    foreach (var v in ventasPorMes)
                    {
                        logger?.LogDebug($"Mes: {v.Mes}, Cantidad: {v.TotalCantidadVendida}");
                        System.Diagnostics.Debug.WriteLine($"Mes: {v.Mes}, Cantidad: {v.TotalCantidadVendida}");
                    }
                }
                
                logger?.LogInfo($"Array ventasMonto: [{string.Join(", ", ventasMonto)}]");
                System.Diagnostics.Debug.WriteLine($"Array ventasMonto: [{string.Join(", ", ventasMonto)}]");

                // Calcular el máximo para asegurar que el gráfico se muestre correctamente
                // Si todos los valores son 0, usar un máximo de 10 para que el gráfico se vea
                var maxVentas = ventasMonto.Max() > 0 ? ventasMonto.Max() : 10;

                // Crear las entradas del gráfico
                var entries = new List<ChartEntry>();
                for (int i = 0; i < 12; i++)
                {
                    var value = ventasMonto[i];
                    // Asegurar que el valor sea al menos 0.1 para que se vea en el gráfico cuando es cero
                    // Esto ayuda a que la línea se muestre en la parte inferior del gráfico
                    var displayValue = value > 0 ? value : 0f;
                    entries.Add(new ChartEntry(displayValue)
                    {
                        Label = Meses[i],
                        ValueLabel = value.ToString("F0"),
                        Color = value > 0 ? SKColor.Parse("#3498db") : SKColor.Parse("#CCCCCC")
                    });
                }

                // Crear el gráfico antes de actualizar en el hilo principal
                var chartEntries = entries.ToArray();
                
                logger?.LogInfo($"Creando gráfico con {chartEntries.Length} entradas");
                logger?.LogInfo($"Max ventas: {maxVentas}");
                logger?.LogInfo($"Primeros valores: {ventasMonto[0]}, {ventasMonto[1]}, {ventasMonto[2]}");
                logger?.LogDebug($"Primeras entradas - Label: {chartEntries[0].Label}, Value: {chartEntries[0].Value}, Color: {chartEntries[0].Color}");
                System.Diagnostics.Debug.WriteLine($"Creando gráfico con {chartEntries.Length} entradas");
                System.Diagnostics.Debug.WriteLine($"Max ventas: {maxVentas}");
                System.Diagnostics.Debug.WriteLine($"Primeros valores: {ventasMonto[0]}, {ventasMonto[1]}, {ventasMonto[2]}");
                System.Diagnostics.Debug.WriteLine($"Primeras entradas - Label: {chartEntries[0].Label}, Value: {chartEntries[0].Value}, Color: {chartEntries[0].Color}");
                
                // Crear el nuevo gráfico
                var nuevoChart = new LineChart
                {
                    Entries = chartEntries,
                    //Lineas
                    LineMode = LineMode.Straight,
                    LineSize = LineSize,
                    //Puntos
                    PointMode = PointMode.Square,
                    PointSize = PointSize,
                    //Labels
                    LabelTextSize = LabelTextSize,
                    ValueLabelTextSize = ValueLabelSize,
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
                    MaxValue = maxVentas,
                    // Forzar que el gráfico siempre muestre el eje Y
                    AnimationDuration = TimeSpan.FromMilliseconds(800)
                };

                logger?.LogInfo($"Gráfico creado. Entries count: {nuevoChart.Entries?.Count() ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Gráfico creado. Entries count: {nuevoChart.Entries?.Count() ?? 0}");
                
                // Actualizar en el hilo principal
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // Primero establecer a null para forzar actualización del ChartView
                    VentasMensualesChart = null;
                    OnPropertyChanged(nameof(VentasMensualesChart));
                    
                    // Pequeño delay para asegurar que el ChartView detecte el cambio
                    Task.Delay(50).ContinueWith(_ =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // Establecer el nuevo gráfico
                            VentasMensualesChart = nuevoChart;
                            EstaCargandoVentasMensuales = false;
                            
                            // Forzar actualización de la propiedad
                            OnPropertyChanged(nameof(VentasMensualesChart));
                            
                            logger?.LogInfo($"Gráfico asignado a la propiedad. Chart es null: {VentasMensualesChart == null}");
                            logger?.LogInfo($"Gráfico tiene {VentasMensualesChart?.Entries?.Count() ?? 0} entradas");
                            System.Diagnostics.Debug.WriteLine($"Gráfico asignado a la propiedad. Chart es null: {VentasMensualesChart == null}");
                            System.Diagnostics.Debug.WriteLine($"Gráfico tiene {VentasMensualesChart?.Entries?.Count() ?? 0} entradas");
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                logger?.LogError($"Error en GenerarGraficoVentasMensualesAsync: {ex.Message}", ex);
                System.Diagnostics.Debug.WriteLine($"Error en GenerarGraficoVentasMensualesAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    EstaCargandoVentasMensuales = false;
                    // Crear un gráfico vacío con ceros en caso de error
                    var entries = Enumerable.Repeat(0f, 12).Select((value, index) => new ChartEntry(value)
                    {
                        Label = Meses[index],
                        ValueLabel = "0",
                        Color = SKColor.Parse("#CCCCCC")
                    }).ToArray();

                    VentasMensualesChart = new LineChart
                    {
                        Entries = entries,
                        LineMode = LineMode.Straight,
                        LineSize = LineSize,
                        PointMode = PointMode.Square,
                        PointSize = PointSize,
                        LabelTextSize = LabelTextSize,
                        ValueLabelTextSize = ValueLabelSize,
                        LabelOrientation = Orientation.Horizontal,
                        ValueLabelOrientation = Orientation.Horizontal,
                        ValueLabelOption = ValueLabelOption.TopOfElement,
                        ShowYAxisLines = true,
                        ShowYAxisText = true,
                        YAxisPosition = Position.Left,
                        EnableYFadeOutGradient = false,
                        BackgroundColor = SKColor.Parse("FFFFFF"),
                        MinValue = 0,
                        MaxValue = 10
                    };
                    
                    logger?.LogWarning("Gráfico de error creado con valores en cero");
                });
            }
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

            // Ajustar el índice para que comience desde lunes
            // Convertir el día de la semana para que sea 0 = Lunes, ..., 6 = Domingo
            diaInicioSemana = (diaInicioSemana + 6) % 7; // Hacer que domingo sea 6

            // Hacer las entries con el vector de ventas
            var entries = ventas.Select((value, index) => new ChartEntry(value)
            {
                Label = DiasSemana[(diaInicioSemana + index) % 7], // Usar el índice ajustado
                ValueLabel = value.ToString("F0"), // Monto del día
                Color = SKColor.Parse("#f1c40f") // Color
            }).ToArray();

            VentasDiariasChart = new LineChart 
            { 
                Entries = entries,
                //Lineas
                LineMode = LineMode.Straight,
                LineSize = LineSize,
                //Puntos
                PointMode = PointMode.Square,
                PointSize = PointSize,
                //Labels
                LabelTextSize = LabelTextSize,
                ValueLabelTextSize = ValueLabelSize,
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
                        Color = SKColor.Parse("#8e44ad")
                    });
                }
            }

            VentasHorariosChart = new LineChart
            {
                Entries = ventas.ToArray(), 
                LineMode = LineMode.Spline,
                LineSize = LineSize,
                PointMode = PointMode.Square,
                PointSize = PointSize,
                LabelTextSize = LabelTextSize,
                ValueLabelOption = ValueLabelOption.None,
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

            var ventasConCategoria = VentasPorCategoria.Where(v => !string.IsNullOrEmpty(v.Categoria)).ToList(); //Limpiar lo que no tiene categoría

            // Ordenar por cantidad vendida y tomar las 10 más vendidas
            var ventasOrdenadas = ventasConCategoria.OrderByDescending(v => v.CantidadVendida).ToList();
            var top10Categorias = ventasOrdenadas.Take(CantidadCategoriasAMostrar).ToList();

            // A las otras se las define como otros
            var cantidadOtros = ventasOrdenadas.Skip(CantidadCategoriasAMostrar).Sum(v => v.CantidadVendida);

            // Hacer las entries
            var entries = top10Categorias.Select((venta, index) => new ChartEntry(venta.CantidadVendida)
            {
                Label = venta.Categoria, // Asignar nombre si está vacío
                ValueLabel = venta.CantidadVendida.ToString("F0"),
                Color = SKColor.Parse(colores[index % colores.Count]) // Asignar colores de la lista
            }).ToList();

            if (cantidadOtros > 0 && CategoriaOtros)
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
                LabelTextSize = LabelTextSize - 8,
                BackgroundColor = SKColor.Parse("FFFFFF"),
                LabelMode = LabelMode.RightOnly,
            };
        }

        // ------------------- Gráfico de facturación mensual -----------------------------------------
        [RelayCommand]
        private async Task GenerarGraficoFacturacionMensual()
        {
            FacturacionesMensuales = await VentaService.GetFacturacionMensual(YearSeleccionadoFacturacionMensual);

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
                Color = SKColor.Parse("#e67e22") // Color (Naranja)
            }).ToArray();

            // Definir el gráfico con los datos cargados
            FacturacionMensualChart = new LineChart
            {
                Entries = entries,
                LineMode = LineMode.Straight,
                LineSize = LineSize,
                PointMode = PointMode.Circle,
                PointSize = PointSize,
                LabelTextSize = LabelTextSize,

                ValueLabelTextSize = ValueLabelSize,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                ValueLabelOption = ValueLabelOption.TopOfElement,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                EnableYFadeOutGradient = true,
                BackgroundColor = SKColor.Parse("FFFFFF"),
            };
        }
        // ------------------- Tabla de artículos más vendidos -----------------------------------------
        [RelayCommand]
        private async Task LlenarTablaArticulosMasVendidos()
        {
            MesSeleccionadoArticulosMasVendidos = Array.IndexOf(Meses.ToArray(), NombreMesSeleccionadoArticulosMasVendidos) + 1;

            ArticulosMasVendidos = await VentaService.GetArticulosMasVendidos(YearSeleccionadoArticulosMasVendidos, MesSeleccionadoArticulosMasVendidos, 
                                                                                    CantidadArticulosMasVendidosAMostrar);
        }

        // ------------------ Gráfico histórico de la farmacia -----------------------------------------
        [RelayCommand]
        private async Task GenerarGraficoHistoricoFarmacia()
        {
            // Obtener las ventas y facturación mensuales desde junio 2017 hasta el mes actual
            var ventasPorMes = await VentaService.GetCantidadVendidaHistorico();
            var facturacionPorMes = await VentaService.GetFacturacionHistorico();

            // Listas para las ventas y facturación
            var ventasEntries = new List<ChartEntry>();
            var facturacionEntries = new List<ChartEntry>();

            // Crear entradas para los meses de junio 2017 al mes actual
            DateTime fechaInicio = new DateTime(2017, 6, 1);
            DateTime fechaFin = DateTime.Now;
            int totalMeses = ((fechaFin.Year - fechaInicio.Year) * 12) + fechaFin.Month - fechaInicio.Month + 1;

            for (int i = 0; i < totalMeses; i++)
            {
                // Con nombres (el tema es que no se llegan a ver ;/)
                DateTime mesActual = fechaInicio.AddMonths(i);
                var mesString = $"{Meses[mesActual.Month - 1]} {mesActual.Year}"; // Ejemplo: "Junio 2017"
                //var mesString = mesActual.ToString("MM/yy");
                // Ventas
                var ventas = ventasPorMes.FirstOrDefault(v => v.Mes == mesActual.Month && v.Año == mesActual.Year)?.TotalCantidadVendida ?? 0;
                ventasEntries.Add(new ChartEntry(ventas)
                {
                    Label = mesString,
                    ValueLabel = ventas.ToString("F0"),
                    Color = SKColor.Parse("#3498db") // Azul para ventas
                });

                // Facturación
                var facturacion = facturacionPorMes.FirstOrDefault(f => f.Mes == mesActual.Month && f.Año == mesActual.Year)?.TotalFacturacion ?? 0;
                facturacionEntries.Add(new ChartEntry((float)facturacion)
                {
                    Label = mesString,
                    ValueLabel = facturacion.ToString("F2"),
                    Color = SKColor.Parse("#e67e22") // Naranja para facturación
                });
            }

            FacturacionMensualHistoricoChart = new LineChart
            {
                Entries = facturacionEntries,
                LineMode = LineMode.Straight,
                LineSize = LineSize,
                PointMode = PointMode.Circle,
                PointSize = PointSize,
                LabelTextSize = 16,
                LabelOrientation = Orientation.Vertical,
                //ValueLabelTextSize = 12,
                //ValueLabelOrientation = Orientation.Horizontal,
                ValueLabelOption = ValueLabelOption.None,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                BackgroundColor = SKColor.Parse("FFFFFF"),
            }; 
            // Crear el gráfico con ambas series en un solo gráfico
            VentasMensualesHistoricoChart = new LineChart
            {
                Entries = ventasEntries, // Combinar ambas series
                LineMode = LineMode.Straight,
                LineSize = LineSize,
                PointMode = PointMode.Square,
                PointSize = PointSize,
                LabelTextSize = 16,
                LabelOrientation = Orientation.Vertical,
                //ValueLabelTextSize = 12,
                //ValueLabelOrientation = Orientation.Horizontal,
                ValueLabelOption = ValueLabelOption.None,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                BackgroundColor = SKColor.Parse("FFFFFF"),
            };
        }
    }
}
