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

// En esta clase se introducen los métodos necesarios para generar una orden de compra en .txt/.csv/.xlsx/.pdf
public partial class PaginaGeneracionOrdenViewModel : ObservableObject  
{

    [ObservableProperty]
    private ObservableCollection<ArticuloEnCompra> listaArticulosComprar = [];//La lista que se muestra y va a la orden de compra

    [ObservableProperty]
    private ObservableCollection<ArticuloEnCompra> articulosSeleccionados = [];
    // Para el picker de selección de que formato generar la orden
    [ObservableProperty]
    private List<string> tiposDeArchivo = ["csv", "xlsx", "txt", "pdf"];

    [ObservableProperty]
    private string tipoArchivoSeleccionado = "csv";

    [ObservableProperty]
    private bool estaCargando = false;

    private readonly string ubicacionOrdenesGeneradas = "C:\\AppFarmacia\\OrdenesGeneradas";//Suponiendo que esta es la carpeta de la APP

    public PaginaGeneracionOrdenViewModel()
    {
        EstaCargando = true;
        Task.Run(async () => await ObtenerArticulosSugeridosParaComprar());
        EstaCargando = false;
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
        var articulo1 = new ArticuloEnCompra
        {
            IdArticuloCompra = 1,
            IdArticulo = 101,
            IdCompra = 5001,
            CantidadSugerida = 10,
            CantidadEncargada = 8,
            Costo = 15.75m,
            NombreArticulo = "Paracetamol 500mg"
        };

        articulo1.calcularMonto(); // Calcula el monto basado en Costo y CantidadEncargada
                                   // Monto esperado: 15.75 * 8 = 126.00
        var articulo2 = new ArticuloEnCompra
        {
            IdArticuloCompra = 2,
            IdArticulo = 102,
            IdCompra = 5002,
            CantidadSugerida = 20,
            CantidadEncargada = 15,
            Costo = 3.50m,
            NombreArticulo = "Ibuprofeno 200mg"
        };

        articulo2.calcularMonto();
        // Monto esperado: 3.50 * 15 = 52.50
        var articulo3 = new ArticuloEnCompra
        {
            IdArticuloCompra = 3,
            IdArticulo = 103,
            IdCompra = 5003,
            CantidadSugerida = 5,
            CantidadEncargada = 5,
            Costo = 50.00m,
            NombreArticulo = "Antibiótico 500mg"
        };

        articulo3.calcularMonto();
        // Monto esperado: 50.00 * 5 = 250.00
        var articulo4 = new ArticuloEnCompra
        {
            IdArticuloCompra = 4,
            IdArticulo = 104,
            IdCompra = 5004,
            CantidadSugerida = 12,
            CantidadEncargada = 10,
            Costo = 25.00m,
            NombreArticulo = "Jarabe para la tos"
        };

        articulo4.calcularMonto();
        // Monto esperado: 25.00 * 10 = 250.00
        var articulo5 = new ArticuloEnCompra
        {
            IdArticuloCompra = 5,
            IdArticulo = 105,
            IdCompra = 5005,
            CantidadSugerida = 30,
            CantidadEncargada = 25,
            Costo = 2.25m,
            NombreArticulo = "Alcohol en gel 100ml"
        };

        articulo5.calcularMonto();
        // Monto esperado: 2.25 * 25 = 56.25
        ListaArticulosComprar.Add(articulo1);
        ListaArticulosComprar.Add(articulo2);
        ListaArticulosComprar.Add(articulo3);
        ListaArticulosComprar.Add(articulo4);
        ListaArticulosComprar.Add(articulo5);
    }

    // ---------------------- Métodos para la generación de la orden de compra ----------------------
    // En función del tipo de archivo seleccionado, se llama a la función correspondiente
    [RelayCommand]
    private void GenerarOrden()
    {
        switch (TipoArchivoSeleccionado)
        {
            case "csv":
                Task.Run(async () => await GenerarOrdenCsv());
                break;
            case "xlsx":
                Task.Run(async () => await GenerarOrdenExcel());
                break;
            case "txt":
                Task.Run(async () => await GenerarOrdenTxt());
                break;
            case "pdf":
                Task.Run(async () => await GenerarOrdenPdf());
                break;
            default:
                break;
        }
    }

    
    async Task GenerarOrdenCsv()
    {
        DateTime fechaActual = DateTime.Now;
        var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.csv");
        try
        {
            using var writer = new StreamWriter(destino);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(ListaArticulosComprar);//Parece que no hay que especificar nada, escribe los atributos y chau
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Éxito", "Planilla exportada de forma exitosa", "OK"));
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
        var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.txt");

        try
        {
            using var writer = new StreamWriter(destino);
            writer.WriteLine("IdArticulo\tNombre\tCantidadEncargada\tCantidadSugerida");
            foreach (var articulo in ListaArticulosComprar)
            {
                writer.WriteLine($"{articulo.IdArticulo}\t{articulo.NombreArticulo}\t{articulo.CantidadEncargada}\t{articulo.CantidadSugerida}");
            }
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.DisplayAlert("Éxito", "Planilla exportada de forma exitosa", "OK"));
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
        var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.xlsx");

        try
        {
            using var workbook = new XLWorkbook();
            // Impresión del título
            var worksheet = workbook.Worksheets.Add("Orden de Compra");
            worksheet.Cell(1, 1).Value = "IdArticulo";
            worksheet.Cell(1, 2).Value = "Nombre";
            worksheet.Cell(1, 3).Value = "CantidadEncargada";
            worksheet.Cell(1, 4).Value = "CantidadSugerida";

            // Impresión de cada uno de los artículos
            for (int i = 0; i < ListaArticulosComprar.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = ListaArticulosComprar[i].IdArticulo;
                worksheet.Cell(i + 2, 2).Value = ListaArticulosComprar[i].NombreArticulo;
                worksheet.Cell(i + 2, 3).Value = ListaArticulosComprar[i].CantidadEncargada;
                worksheet.Cell(i + 2, 4).Value = ListaArticulosComprar[i].CantidadSugerida;
            }

            workbook.SaveAs(destino);
            await Shell.Current.DisplayAlert("Éxito", "Planilla exportada de forma exitosa", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Hubo un problema al exportar la planilla: {ex.Message}", "OK");
        }
    }

    // Este ya anda, hay que hacerlo que quede lindo nomás
    async Task GenerarOrdenPdf()
    {
        DateTime fechaActual = DateTime.Now;
        var destino = Path.Combine(ubicacionOrdenesGeneradas, $"OrdenGenerada_{fechaActual:dd_MM_yyyy_HH.mm}Hs.pdf");

        try
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var graphics = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12, XFontStyle.Regular);

            graphics.DrawString("Orden de Compra", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, new XRect(0, 0, page.Width, 40), XStringFormats.TopCenter);

            int yPoint = 40;
            foreach (var articulo in ListaArticulosComprar)
            {
                graphics.DrawString($"Id: {articulo.IdArticulo}, Nombre: {articulo.NombreArticulo}, Cantidad Encargada: {articulo.CantidadEncargada}, Cantidad Sugerida: {articulo.CantidadSugerida}",
                                    font, XBrushes.Black, new XRect(40, yPoint, page.Width - 80, page.Height - 40), XStringFormats.TopLeft);
                yPoint += 20;
            }

            document.Save(destino);
            await Shell.Current.DisplayAlert("Éxito", "Planilla exportada de forma exitosa", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Hubo un problema al exportar la planilla: {ex.Message}", "OK");
        }
    }
}
