using System.Net.Http.Json;
using AppFarmacia.Models;
namespace AppFarmacia.Services
{
    public class ArticulosService
    {
        List<Articulo>? articulos = [];
        HttpClient httpClient;

        public ArticulosService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<Articulo>> GetArticulos()
        {
            if (this.articulos?.Count > 0)
            {
                return articulos;
            }
            var respuesta = await httpClient.GetAsync("http://localhost:83/api/Articulos");

            if (respuesta.IsSuccessStatusCode)
                this.articulos = await respuesta.Content.ReadFromJsonAsync<List<Articulo>>() ?? [];

            return this.articulos!;
        }
    }
}