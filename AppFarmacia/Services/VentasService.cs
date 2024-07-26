using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class VentasService
    {
        private List<Venta>? Ventas = [];
        private HttpClient httpClient;

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

            // Obtengo respuesta
            var respuesta = await httpClient.GetAsync("http://localhost:83/api/Ventas");

            // Si la respuesta es exitosa
            if (respuesta.IsSuccessStatusCode)
                this.Ventas = await respuesta.Content.ReadFromJsonAsync<List<Venta>>() ?? []; 

            return this.Ventas!;
        }
    }
}
