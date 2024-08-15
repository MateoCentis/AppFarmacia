using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class PreciosService
    {
        List<Precio>? precios = [];
        private readonly HttpClient httpClient;

        public PreciosService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<Precio>> GetPrecios()
        {
            if (this.precios?.Count > 0)
            {
                return precios;
            }
            var respuesta = await httpClient.GetAsync("http://localhost:83/api/Precios");

            if (respuesta.IsSuccessStatusCode)
                this.precios = await respuesta.Content.ReadFromJsonAsync<List<Precio>>() ?? [];

            return this.precios!;
        }

        public async Task<Precio> GetPrecioPorId(int id)
        {
            Precio? precio = this.precios.FirstOrDefault(a => a.IdPrecio == id);
            if ( precio != null )
            {
                return precio;
            }

            var respuesta = await httpClient.GetAsync($"http://localhost:83/api/Precios/{id}");

            if (respuesta.IsSuccessStatusCode)
            {
                precio = await respuesta.Content.ReadFromJsonAsync<Precio>();
                return precio;
            }

            return null;
        }

        public async Task<List<Precio>> GetPreciosArticulo(int id)
        {
            var respuesta = await httpClient.GetAsync($"http://localhost:83/api/Precios/Articulo/{id}");
            List<Precio> precios_articulo = [];

            if (respuesta.IsSuccessStatusCode)
                precios_articulo = await respuesta.Content.ReadFromJsonAsync<List<Precio>>() ?? [];

            return precios_articulo!;
        }


    }
}
