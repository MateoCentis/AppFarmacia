using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class VencimientosService
    {
        List<Vencimiento>? vencimientos = [];
        private readonly HttpClient httpClient;

        public VencimientosService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<Vencimiento>> GetVencimiento()
        {
            if (this.vencimientos?.Count > 0)
            {
                return vencimientos;
            }
            var respuesta = await httpClient.GetAsync("http://localhost:83/api/Vencimientos");

            if (respuesta.IsSuccessStatusCode)
                this.vencimientos = await respuesta.Content.ReadFromJsonAsync<List<Vencimiento>>() ?? [];

            return this.vencimientos!;
        }

        public async Task<Vencimiento> GetVencimientoPorId(int id)
        {
            Vencimiento? vencimiento = this.vencimientos.FirstOrDefault(a => a.IdVencimiento == id);
            if (vencimiento != null)
            {
                return vencimiento;
            }

            var respuesta = await httpClient.GetAsync($"http://localhost:83/api/Vencimientos/{id}");

            if (respuesta.IsSuccessStatusCode)
            {
                vencimiento = await respuesta.Content.ReadFromJsonAsync<Vencimiento>();
                return vencimiento;
            }

            return null;
        }

        public async Task<List<Vencimiento>> GetVencimientosArticulo(int id)
        {
            var respuesta = await httpClient.GetAsync($"http://localhost:83/api/Vencimientos/Articulos/{id}");
            List<Vencimiento> vencimientos_articulo = [];

            if (respuesta.IsSuccessStatusCode)
                vencimientos_articulo = await respuesta.Content.ReadFromJsonAsync<List<Vencimiento>>() ?? [];

            return vencimientos_articulo!;
        }


    }
}
