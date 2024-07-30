using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class CategoriasService
    {
        List<Categoria>? categorias = [];
        private readonly HttpClient httpClient;

        public CategoriasService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<Categoria>> GetCategorias()
        {
            if (this.categorias?.Count > 0)
            {
                return categorias;
            }
            var respuesta = await httpClient.GetAsync("http://localhost:83/api/Categorias");

            if (respuesta.IsSuccessStatusCode)
                this.categorias = await respuesta.Content.ReadFromJsonAsync<List<Categoria>>() ?? [];

            return this.categorias!;
        }

    }
}
