using System.Net.Http.Json;
using AppFarmacia.Models;
namespace AppFarmacia.Services
{
    public class ArticulosFinalesService
    {
        List<ArticuloFinal>? articulos = [];
        private readonly HttpClient httpClient;

        public ArticulosFinalesService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<ArticuloFinal>> GetArticulos()
        {
            if (this.articulos?.Count > 0)
            {
                return articulos;
            }
            var respuesta = await httpClient.GetAsync("http://localhost:83/api/ArticulosFinales");

            if (respuesta.IsSuccessStatusCode)
                this.articulos = await respuesta.Content.ReadFromJsonAsync<List<ArticuloFinal>>() ?? [];

            return this.articulos!;
        }

        public async Task<ArticuloFinal?> GetArticuloPorId(int id)
        {
            ArticuloFinal? articulo = this.articulos.FirstOrDefault(a => a.IdArticuloFinal == id);
            if (articulo != null)
            {
                return articulo;
            }

            var respuesta = await httpClient.GetAsync($"http://localhost:83/api/ArticulosFinales/{id}");

            if (respuesta.IsSuccessStatusCode)
            {
                articulo = await respuesta.Content.ReadFromJsonAsync<ArticuloFinal>();
                return articulo;
            }

            return null;
        }

    }
}