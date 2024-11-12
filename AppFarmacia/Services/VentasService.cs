using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppFarmacia.Models;

// Tipos de datos para obtener las respuestas de la API ---------------------------------------------
public class VentaMesDto
{
    public int Mes { get; set; }
    public int TotalCantidadVendida { get; set; }
}

public class VentaHoraDto
{
    public int Hora { get; set; }
    public int CantidadVendida { get; set; }
}

public class DiaSemanaDto
{
    public int DiaSemana { get; set; }
    public List<VentaHoraDto> VentasPorHora { get; set; } = [];
}

public class VentaDiaDto
{
    public int Dia { get; set; }
    public int CantidadVendida { get; set; }
}

public class VentaCategoriaDto
{
    public string Categoria { get; set; }
    public int CantidadVendida { get; set; }
}

public class FacturacionMensual
{
    public int Mes { get; set; }
    public decimal TotalFacturacion { get; set; }
}

public class ArticuloDTO
{
    [JsonPropertyName("idArticulo")]
    public int IdProducto { get; set; }
    [JsonPropertyName("nombre")]
    public string Nombre { get; set; }
    [JsonPropertyName("cantidadVendida")]
    public int CantidadVendida { get; set; }
}

public class CantidadVendidaHistoricaDTO
{
    public int Año { get; set; }
    public int Mes { get; set; }
    public int TotalCantidadVendida { get; set; }
}

public class FacturacionMensualHistoricaDTO
{
    public int Año { get; set; }
    public int Mes { get; set; }
    public decimal TotalFacturacion { get; set; }
}

// ---------------------------------------------------------------------------------------------------

namespace AppFarmacia.Services
{
    public class VentasService
    {
        private List<Venta>? Ventas = [];
        private readonly HttpClient httpClient;
        private const string CadenaConexion = "http://localhost:83/api";
        public VentasService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<Venta>> GetVentas(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Formateo a string para request (YYYY-MM-dd) 
                string fechaInicioStr = fechaInicio.ToString("yyyy-MM-dd");
                string fechaFinStr = fechaFin.ToString("yyyy-MM-dd");

                var url = $"{CadenaConexion}/Ventas?fechaInicio={fechaInicioStr}&fechaFin={fechaFinStr}";
                // Obtengo respuesta  
                var respuesta = await httpClient.GetAsync(url);

                // Si la respuesta es exitosa
                if (respuesta.IsSuccessStatusCode)
                {
                    this.Ventas = await respuesta.Content.ReadFromJsonAsync<List<Venta>>(); 
                    return this.Ventas ?? [];

                }
                else
                {
                    // Devoluciones de error
                    switch (respuesta.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            throw new Exception("No se encontró el recurso solicitado.");
                        case HttpStatusCode.BadRequest:
                            throw new Exception("Solicitud inválida. Verifique los parámetros de fecha.");
                        default:
                            throw new Exception($"Error en la solicitud: {respuesta.StatusCode}");
                    }
                }

            }
            catch (JsonException ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
        }

        // Lista de cantidades vendidas por mes dado un año determinado
        public async Task<List<VentaMesDto>> GetCantidadVendidaPorMes(int year)
        {
            try
            {
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-por-mes/{year}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<VentaMesDto>>() ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

        // Lista de cantidades vendidas por hora de la semana
        public async Task<List<DiaSemanaDto>> GetCantidadVendidaPorHoraSemana()
        {
            try
            {
                var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-por-hora-semana");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<DiaSemanaDto>>() ?? [];

            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

        // Lista de cantidades vendidas por día dado un año y mes determinado
        public async Task<List<VentaDiaDto>> GetCantidadVendidaPorDia(int year, int mes)
        {
            try
            {
                var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-por-dia/{year}/{mes}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<VentaDiaDto>>() ?? [];
            }
            catch (Exception ex) {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

        // Lista de cantidades vendidas por categoría
        public async Task<List<VentaCategoriaDto>> GetCantidadVendidaPorCategoria()
        {
            try
            {
                var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-por-categoria");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<VentaCategoriaDto>>() ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

        public async Task<List<FacturacionMensual>> GetFacturacionMensual(int year)
        {
            try
            {
                var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/facturacion-mensual/{year}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<FacturacionMensual>>() ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

        public async Task<List<ArticuloDTO>> GetArticulosMasVendidos(int year, int mes, int cantidad)
        {
            try
            {
                var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/articulos-mas-vendidos/{year}/{mes}/{cantidad}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ArticuloDTO>>() ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

        // Históricos
        public async Task<List<CantidadVendidaHistoricaDTO>> GetCantidadVendidaHistorico()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-vendida-historica");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<CantidadVendidaHistoricaDTO>>() ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

        public async Task<List<FacturacionMensualHistoricaDTO>> GetFacturacionHistorico()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/facturacion-mensual-historica");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<FacturacionMensualHistoricaDTO>>() ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
            
        }

    }
}
