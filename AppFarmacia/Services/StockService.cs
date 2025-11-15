using System.Net.Http.Json;
using System.Text.Json;
using AppFarmacia.Models;
using System.Net;

namespace AppFarmacia.Services
{
    public class StockService
    {
        private List<Stock>? historial_Stock = new List<Stock>();
        private Stock? stock = new Stock();

        private readonly HttpClient httpClient;
        private const string CadenaConexion = "http://localhost:83/api";
        
        // Opciones JSON para deserialización correcta
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        public StockService() 
        {
            this.httpClient = new HttpClient();
        }
        
        public async Task<Stock?> GetUltimoStockPorArticulo(int id_articulo)
        {
            try
            {
                var url = $"{CadenaConexion}/Stocks/GetUltimoStockPorArticulo/{id_articulo}";
                var respuesta = await httpClient.GetAsync(url);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Leer el contenido como string primero para debugging
                    var jsonContent = await respuesta.Content.ReadAsStringAsync();
                    
                    // Verificar que el contenido no esté vacío
                    if (string.IsNullOrWhiteSpace(jsonContent))
                    {
                        System.Diagnostics.Debug.WriteLine($"Respuesta vacía de la API para stock del artículo {id_articulo}: {url}");
                        return null;
                    }

                    // Intentar deserializar
                    try
                    {
                        this.stock = JsonSerializer.Deserialize<Stock>(jsonContent, jsonOptions);
                        return this.stock;
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deserializando JSON de stock para artículo {id_articulo}: {jsonEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"Contenido recibido (primeros 500 caracteres): {jsonContent.Substring(0, Math.Min(500, jsonContent.Length))}");
                        return null;
                    }
                }
                else if (respuesta.StatusCode == HttpStatusCode.NotFound)
                {
                    // No hay stock para este artículo, es normal
                    System.Diagnostics.Debug.WriteLine($"No se encontró stock para el artículo {id_articulo}");
                    return null;
                }
                else
                {
                    // Leer el contenido del error
                    var errorContent = await respuesta.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error en API de stock para artículo {id_articulo}: {respuesta.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error de conexión obteniendo stock para artículo {id_articulo}: {httpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inesperado obteniendo stock para artículo {id_articulo}: {ex.Message}");
                return null;
            }
        }
    }
}
