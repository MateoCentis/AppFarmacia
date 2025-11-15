using AppFarmacia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using CsvHelper;
using ClosedXML.Excel;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
namespace AppFarmacia.ViewModels;
using AppFarmacia.Models;
using AppFarmacia.Services;
using AppFarmacia.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

// En esta clase se introducen los métodos necesarios para generar una orden de compra en .txt/.csv/.xlsx/.pdf
public partial class PaginaGeneracionOrdenViewModel : ObservableObject  
{
    private readonly ArticulosService articulosService;
    private readonly CategoriasService categoriasService;
    private readonly CompraService compraService;
    private readonly FaltanteService faltanteService;

    [ObservableProperty]
    private ObservableCollection<ArticuloEnCompra> listaArticulosComprar = [];//La lista que se muestra y va a la orden de compra

    [ObservableProperty]
    private List<String> motivos = ["Punto de reposición", "SobreStock", "Faltante"];

    [ObservableProperty]
    private string? motivoSeleccionado;

    [ObservableProperty]
    private string descripcionCompraTexto;

    [ObservableProperty]
    private string proveedorCompra;

    [ObservableProperty]
    private ObservableCollection<ArticuloEnCompra> listaArticulosFiltrados = [];

    [ObservableProperty]
    private List<string> nombresArticulos;

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

    [ObservableProperty]
    private List<string> nombresCategorias;

    [ObservableProperty]
    private Categoria? categoriaSeleccionada;

    [ObservableProperty]
    private string? categoriaSeleccionadaNombre;

    [ObservableProperty]
    private string? proveedorCompraTexto;

    [ObservableProperty]
    private ObservableCollection<Categoria> listCategorias = [];

    [ObservableProperty]
    private ObservableCollection<ArticuloMostrar> listaArticulos = [];

    [ObservableProperty]
    private ObservableCollection<ArticuloMostrar> listaArticulosCompleta = [];

    [ObservableProperty]
    private ArticuloMostrar? articuloSeleccionadoDeListaCompleta; // Artículo seleccionado del data grid de todos los artículos


    [ObservableProperty]
    private ObservableCollection<ArticuloEnCompra> articulosSeleccionados = [];
    // Para el picker de selección de que formato generar la orden
    [ObservableProperty]
    private List<string> tiposDeArchivo = ["csv", "xlsx", "txt", "pdf"];

    [ObservableProperty]
    private string tipoArchivoSeleccionado = "csv";

    [ObservableProperty]
    private bool estaCargando = false;

    [ObservableProperty]
    private int sizePagina;

    [ObservableProperty]
    private bool paginationEnabled;

    private readonly string ubicacionOrdenesGeneradas = "C:\\AppFarmacia\\OrdenesGeneradas";//Suponiendo que esta es la carpeta de la APP

    public PaginaGeneracionOrdenViewModel()
    {
        this.articulosService = new ArticulosService();
        this.categoriasService = new CategoriasService();
        this.compraService = new CompraService();
        this.faltanteService = new FaltanteService();
        TextoBusqueda = string.Empty;
        NombresArticulos = [];
        NombresCategorias = [];
        PaginationEnabled = true;
        SizePagina = 20;
        CategoriaSeleccionadaNombre = "Todas";
        DescripcionCompraTexto = string.Empty;
        ProveedorCompra = string.Empty;

        // Carga inicial de los artículos
        Task.Run(async () => await ObtenerCategorias());
        Task.Run(async () => await ObtenerArticulos());
        Task.Run(async () => await ObtenerArticulosSugeridosParaComprar());
    }

    // ----------------------------- Métodos para eliminar y agregar artículos -----------------------------
    [RelayCommand]
    private void EliminarArticulosSeleccionados()
    {
        if (ArticulosSeleccionados.Count == 0) return;

        // Lista temporal para evitar modificar la colección mientras se itera
        var articulosAEliminar = ArticulosSeleccionados.ToList();

        foreach (var articulo in articulosAEliminar)
        {
            ListaArticulosComprar.Remove(articulo);
        }

        ArticulosSeleccionados.Clear();
    }

    // Función que carga desde la API los productos que se encuentran en punto de reposición junto con su EOQ
    [RelayCommand]
    private async Task ObtenerArticulosSugeridosParaComprar()
    {
        EstaCargando = true;
        try
        {
            var articulosSugeridos = await articulosService.GetArticulosSugeridosParaComprar();
            var articulosEnCompraTasks = articulosSugeridos.Select(async a => new ArticuloEnCompra
            {
                IdArticulo = a.IdArticulo,         // Asigna el IdArticulo desde Articulo
                NombreArticulo = a.Nombre,         // Asigna el Nombre desde Articulo
                CantidadSugerida = a.CantidadAPedir ?? 0, // Asigna CantidadAPedir o 0 si es null
                CantidadFaltante = await faltanteService.ObtenerFaltanteDeArticulo(a.IdArticulo)
            });

            var articulosEnCompra = await Task.WhenAll(articulosEnCompraTasks); // Espera a que todas las tareas se completen por el "await faltanteService.ObtenerFaltanteDeArticulo"
            ListaArticulosComprar = new ObservableCollection<ArticuloEnCompra>(articulosEnCompra);
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Error", $"Hubo un problema al obtener los artículos: {ex.Message}", "OK"));
        }
        EstaCargando = false;
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
                Shell.Current.DisplayAlert("Error en artículos!", ex.Message, "OK");
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
            ListCategorias.Insert(0, ningunaCategoria);
            CategoriaSeleccionada = ningunaCategoria;
            NombresCategorias = ListCategorias.Select(c => c.Nombre).ToList();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get categorias: {ex.Message }");
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.DisplayAlert("Error en categorias!", ex.Message, "OK");
            });
        }
    }

    // ---------------------- Métodos para la generación de la orden de compra ----------------------
    // En función del tipo de archivo seleccionado, se llama a la función correspondiente
    [RelayCommand]
    private async Task PostCompra()
    {
        // Validar que hay artículos seleccionados
        if (ArticulosSeleccionados == null || ArticulosSeleccionados.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No se ha seleccionado ningún artículo para la orden de compra. Por favor, seleccione al menos un artículo de la lista.", "OK");
            return;
        }

        // Usar solo los artículos seleccionados
        var articulosParaCompra = ArticulosSeleccionados.ToList();

        Compra compra = new Compra
        {
            Fecha = DateTime.Now,
            Descripcion = DescripcionCompraTexto,
            Proveedor = ProveedorCompra, // Agregar proveedor
            ArticuloEnCompra = articulosParaCompra, // Solo artículos seleccionados
            CompraConfirmada = false
        };

        // Se guarda la compra
        bool resultadoCompra = await compraService.PostCompra(compra);
        if (resultadoCompra) 
        { 
            await Shell.Current.DisplayAlert("Éxito", "Compra realizada con éxito", "OK");
            // Limpiar la lista después de crear la compra
            ListaArticulosComprar.Clear();
            ArticulosSeleccionados?.Clear();
        }
        else 
        { 
            await Shell.Current.DisplayAlert("Error", "Hubo un problema al realizar la compra", "OK"); 
        }
    }


    [RelayCommand]
    private async Task GenerarOrden()
    {
        // Validar que hay artículos para exportar
        if (ListaArticulosComprar == null || ListaArticulosComprar.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No hay artículos para exportar", "OK");
            return;
        }

        // Filtrar solo los artículos seleccionados (si hay selección)
        var articulosParaExportar = ArticulosSeleccionados != null && ArticulosSeleccionados.Count > 0
            ? ArticulosSeleccionados.ToList()
            : ListaArticulosComprar.ToList();

        if (articulosParaExportar.Count == 0)
        {
            await Shell.Current.DisplayAlert("Error", "No se ha seleccionado ningún artículo para exportar", "OK");
            return;
        }

        // Guardar la lista original y usar la filtrada temporalmente
        var listaOriginal = ListaArticulosComprar.ToList();
        ListaArticulosComprar = new ObservableCollection<ArticuloEnCompra>(articulosParaExportar);

        try
        {
            switch (TipoArchivoSeleccionado)
            {
                case "csv":
                    await GenerarOrdenCsv();
                    break;
                case "xlsx":
                    await GenerarOrdenExcel();
                    break;
                case "txt":
                    await GenerarOrdenTxt();
                    break;
                case "pdf":
                    await GenerarOrdenPdf();
                    break;
                default:
                    await Shell.Current.DisplayAlert("Error", "Formato de archivo no válido", "OK");
                    break;
            }
        }
        finally
        {
            // Restaurar la lista original
            ListaArticulosComprar = new ObservableCollection<ArticuloEnCompra>(listaOriginal);
        }
    }
    [RelayCommand]
    private async Task AgregarArticulo() // Marcar el método como async
    {
        if (ArticuloSeleccionadoDeListaCompleta != null)
        {
            // Verificar si el artículo ya está en la lista
            if (ListaArticulosComprar.Any(a => a.IdArticulo == ArticuloSeleccionadoDeListaCompleta.IdArticulo))
            {
                return;
            }

            // Llamar a ObtenerFaltanteDeArticulo de manera asincrónica
            int cantidadFaltante = await faltanteService.ObtenerFaltanteDeArticulo(ArticuloSeleccionadoDeListaCompleta.IdArticulo);

            var articulo = new ArticuloEnCompra
            {
                IdArticulo = ArticuloSeleccionadoDeListaCompleta.IdArticulo,
                NombreArticulo = ArticuloSeleccionadoDeListaCompleta.Nombre,
                CantidadSugerida = ArticuloSeleccionadoDeListaCompleta.CantidadAPedir ?? 0,
                CantidadFaltante = cantidadFaltante, // Asignar el resultado de la tarea asincrónica
                MotivoCompra = MotivoSeleccionado ?? "Punto de reposición" // Asignar el motivo seleccionado
            };

            ListaArticulosComprar.Add(articulo);
            
            // Limpiar la selección después de agregar
            ArticuloSeleccionadoDeListaCompleta = null;
            MotivoSeleccionado = null;
        }
    }



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


        this.ListaArticulos = new ObservableCollection<ArticuloMostrar>(articulosFiltrados);
    }

    


    async Task GenerarOrdenCsv()
    {
        DateTime fechaActual = DateTime.Now;
        try
        {
            // Crear el directorio si no existe
            Directory.CreateDirectory(ubicacionOrdenesGeneradas);
            var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.csv");
            
            using var writer = new StreamWriter(destino);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(ListaArticulosComprar);
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Éxito", $"Planilla exportada exitosamente a:\n{destino}", "OK"));
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Error", $"Hubo un problema al exportar la planilla: {ex.Message}", "OK"));
        }
    }

    async Task GenerarOrdenTxt()
    {
        DateTime fechaActual = DateTime.Now;
        try
        {
            // Crear el directorio si no existe
            Directory.CreateDirectory(ubicacionOrdenesGeneradas);
            var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.txt");

            using var writer = new StreamWriter(destino);
            writer.WriteLine("IdArticulo\tNombre\tCantidadEncargada\tCantidadSugerida\tMotivo");
            foreach (var articulo in ListaArticulosComprar)
            {
                writer.WriteLine($"{articulo.IdArticulo}\t{articulo.NombreArticulo}\t{articulo.Cantidad}\t{articulo.CantidadSugerida}\t{articulo.MotivoCompra ?? "N/A"}");
            }
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Éxito", $"Planilla exportada exitosamente a:\n{destino}", "OK"));
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Error", $"Hubo un problema al exportar la planilla: {ex.Message}", "OK"));
        }
    }

    // Método para generar archivo Excel (.xlsx)
    async Task GenerarOrdenExcel()
    {
        DateTime fechaActual = DateTime.Now;
        try
        {
            // Crear el directorio si no existe
            Directory.CreateDirectory(ubicacionOrdenesGeneradas);
            var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.xlsx");

            using var workbook = new XLWorkbook();
            // Impresión del título
            var worksheet = workbook.Worksheets.Add("Orden de Compra");
            worksheet.Cell(1, 1).Value = "IdArticulo";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "CantidadEncargada";
            worksheet.Cell(1, 4).Value = "CantidadSugerida";
            worksheet.Cell(1, 5).Value = "Motivo";

            // Impresión de cada uno de los artículos
            for (int i = 0; i < ListaArticulosComprar.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = ListaArticulosComprar[i].IdArticulo;
                worksheet.Cell(i + 2, 2).Value = ListaArticulosComprar[i].NombreArticulo;
                worksheet.Cell(i + 2, 3).Value = ListaArticulosComprar[i].Cantidad;
                worksheet.Cell(i + 2, 4).Value = ListaArticulosComprar[i].CantidadSugerida;
                worksheet.Cell(i + 2, 5).Value = ListaArticulosComprar[i].MotivoCompra ?? "N/A";
            }

            workbook.SaveAs(destino);
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Éxito", $"Planilla exportada exitosamente a:\n{destino}", "OK"));
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Error", $"Hubo un problema al exportar la planilla: {ex.Message}", "OK"));
        }
    }

    // Este ya anda, hay que hacerlo que quede lindo nomás
    async Task GenerarOrdenPdf()
    {
        DateTime fechaActual = DateTime.Now;
        try
        {
            // Crear el directorio si no existe
            Directory.CreateDirectory(ubicacionOrdenesGeneradas);
            var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.pdf");

            using var document = new PdfDocument();
            var page = document.AddPage();
            var graphics = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12, XFontStyle.Regular);

            graphics.DrawString("Orden de Compra", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 40), XStringFormats.TopCenter);

            int yPoint = 60;
            foreach (var articulo in ListaArticulosComprar)
            {
                var texto = $"Id: {articulo.IdArticulo}, Nombre: {articulo.NombreArticulo}, Cantidad Encargada: {articulo.Cantidad}, Cantidad Sugerida: {articulo.CantidadSugerida}, Motivo: {articulo.MotivoCompra ?? "N/A"}";
                graphics.DrawString(texto,
                                    font, XBrushes.Black, new XRect(40, yPoint, page.Width - 80, page.Height - 40), XStringFormats.TopLeft);
                yPoint += 20;
                
                // Agregar nueva página si es necesario
                if (yPoint > page.Height - 40)
                {
                    page = document.AddPage();
                    graphics = XGraphics.FromPdfPage(page);
                    yPoint = 40;
                }
            }

            document.Save(destino);
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Éxito", $"Planilla exportada exitosamente a:\n{destino}", "OK"));
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Error", $"Hubo un problema al exportar la planilla: {ex.Message}", "OK"));
        }
    }
    
}
