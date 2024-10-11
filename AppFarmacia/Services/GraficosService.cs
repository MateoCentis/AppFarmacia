using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class GraficosService
    {
        private readonly HttpClient httpClient;
        private const string CadenaConexion = "http://localhost:83/api"; // Cadena de conexión centralizada

        public GraficosService()
        {
            this.httpClient = new HttpClient();
        }
        // Hacer un método para cada gráfico de la PaginaGraficoViewModels
        // De forma tal que se la información se calcule y sea enviada ya calculada
            // O puedo traerme todo y calcular acá (no recomendado)

        // Obtiene una lista de 12 elementos donde cada uno representa el monto total vendido del mes
        public async Task<List<float>> GetMontoVentasMensuales(int year)
        {
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Ventas/MontoVentasMensuales");

            if (respuesta.IsSuccessStatusCode)
            {
                return await respuesta.Content.ReadFromJsonAsync<List<float>>() ?? [];
            }
        }

        public async Task<List<float>> GetMontoVentasDiario(int year, int mes)
        {
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Ventas/MontoVentasDiario/{year}/{mes}");

            if (respuesta.IsSuccessStatusCode)
            {
                return await respuesta.Content.ReadFromJsonAsync<List<float>>() ?? [];
            }
        }

        // Retorna una lista de enteros, donde cada uno representa la cantidad vendida
        // en un horario.
        public async Task<List<int>> GetHorariosConMasVentasPorDia(int dia)
        {
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Ventas/HorariosConMasVentasPorDia/{dia}");

            if (respuesta.IsSuccessStatusCode)
            {
                return await respuesta.Content.ReadFromJsonAsync<List<int>>() ?? [];
            }

        }

        // Obtiene la cantidad de ventas por acción terapéutica (se puede usar con un gráfico de torta)
        public async Task<List<float>> GetVentasAccionTerapeutica()
        {
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Ventas/VentasAccionTerapeutica");

            if (respuesta.IsSuccessStatusCode)
            {
                return await respuesta.Content.ReadFromJsonAsync<List<float>>() ?? [];
            }
        }


    }
}
