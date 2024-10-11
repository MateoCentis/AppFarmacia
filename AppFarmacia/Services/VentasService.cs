using System.Net.Http.Json;
using AppFarmacia.Models;

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
    public List<VentaHoraDto> VentasPorHora { get; set; }
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

        public async Task<List<Venta>> GetVentas()
        {
            // Si ya tengo ventas cargadas no llamo a la API
            if (this.Ventas?.Count > 0)
            {
                return Ventas;
            }

            // Obtengo respuesta           //Cambiar a localhost para usar en ambas PC's
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Ventas");

            // Si la respuesta es exitosa
            if (respuesta.IsSuccessStatusCode)
                this.Ventas = await respuesta.Content.ReadFromJsonAsync<List<Venta>>() ?? []; 

            return this.Ventas!;
        }

        // Lista de cantidades vendidas por mes dado un año determinado
        public async Task<List<VentaMesDto>> GetCantidadVendidaPorMes(int year)
        {
            var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-por-mes/{year}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<VentaMesDto>>() ?? [];
        }

        // Lista de cantidades vendidas por hora de la semana
        public async Task<List<DiaSemanaDto>> GetCantidadVendidaPorHoraSemana()
        {
            var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-por-hora-semana");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DiaSemanaDto>>() ?? [];
        }

        // Lista de cantidades vendidas por día dado un año y mes determinado
        public async Task<List<VentaDiaDto>> GetCantidadVendidaPorDia(int year, int mes)
        {
            var response = await httpClient.GetAsync($"{CadenaConexion}/Ventas/cantidad-por-dia/{year}/{mes}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<VentaDiaDto>>() ?? [];
        }

        // Lista de cantidades vendidas por categoría
        public async Task<List<VentaCategoriaDto>> GetCantidadVendidaPorCategoria()
        {
            var response = await httpClient.GetAsync("{CadenaConexion}/Ventas/cantidad-por-categoria");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<VentaCategoriaDto>>() ?? [];
        }
    }
}
