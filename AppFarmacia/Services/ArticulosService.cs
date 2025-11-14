using System.Net.Http.Json;
using System.Text.Json;
using AppFarmacia.Models;
using System.Linq;

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
            try
            {
                var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Articulos/{id}/Demandas/{year}");

                if (respuesta.IsSuccessStatusCode)
                {
                    var demandas = await respuesta.Content.ReadFromJsonAsync<List<int>>(jsonOptions);
                    // Si la respuesta es exitosa pero no hay datos, retornar lista con 12 ceros
                    return demandas ?? Enumerable.Repeat(0, 12).ToList();
                }
                else
                {
                    // Para cualquier error (404, 500, etc.), retornar lista con ceros
                    return Enumerable.Repeat(0, 12).ToList();
                }
            }
            catch (Exception)
            {
                // En caso de error de red u otro, retornar lista con ceros para evitar crashes
                return Enumerable.Repeat(0, 12).ToList();
            }
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
