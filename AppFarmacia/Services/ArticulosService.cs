using System.Net.Http.Json;
using System.Text.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class ArticulosService
    {
        private List<Articulo>? articulos = new List<Articulo>();
        private readonly HttpClient httpClient;
        private const string CadenaConexion = "http://localhost:83/api"; // Cadena de conexión centralizada
        
        // Opciones JSON para deserialización correcta
        // JsonPropertyName siempre tiene prioridad, pero PropertyNameCaseInsensitive ayuda con variaciones
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Permite mapeo flexible, JsonPropertyName tiene prioridad
        };

        public ArticulosService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<Articulo>> GetArticulos(int size = 0)
        {
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Articulos?size={size}");

            if (respuesta.IsSuccessStatusCode)
            {
                // Usar opciones JSON explícitas para asegurar deserialización correcta
                this.articulos = await respuesta.Content.ReadFromJsonAsync<List<Articulo>>(jsonOptions) ?? [];
            }

            return this.articulos!;
        }

        public async Task<Articulo?> GetArticuloPorId(int id)
        {
            Articulo? articulo = this.articulos.FirstOrDefault(a => a.IdArticulo == id);
            if (articulo != null)
            {
                return articulo;
            }

            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Articulos/{id}");

            if (respuesta.IsSuccessStatusCode)
            {
                articulo = await respuesta.Content.ReadFromJsonAsync<Articulo>();
                return articulo;
            }

            return null;
        }

        public async Task<List<int>> GetDemandasMensualesArticulo(int id, int year)
        {
            var demandas = new List<int>();
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Articulos/{id}/Demandas/{year}");

            if (respuesta.IsSuccessStatusCode)
            {
                demandas = await respuesta.Content.ReadFromJsonAsync<List<int>>() ?? [];
            }

            return demandas;
        }

        public async Task<List<Articulo>> GetArticulosSugeridosParaComprar()
        {

            var articulosSugeridos = new List<Articulo>();
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Articulos/ArticulosSugeridosParaComprar");

            if (respuesta.IsSuccessStatusCode)
            {
                articulosSugeridos = await respuesta.Content.ReadFromJsonAsync<List<Articulo>>() ?? [];
            }

            return articulosSugeridos;
        }
    }
}
