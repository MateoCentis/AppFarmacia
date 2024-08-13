using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class ArticuloVentaService
    {
        private List<ArticuloEnVenta>? ArticulosEnVenta = [];
        private readonly HttpClient httpClient;

        public ArticuloVentaService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<ArticuloEnVenta>> GetArticulosEnVentasPorIdVenta(int id)
        {
            // Si ya tengo ventas cargadas no llamo a la API
            if (this.ArticulosEnVenta?.Count > 0)
            {
                return ArticulosEnVenta;
            }

            // Obtengo respuesta           //Cambiar a localhost para usar en ambas PC's
            var respuesta = await httpClient.GetAsync($"http://LocalHost:83/api/ArticulosEnVenta/PorVentaId/{id}");


            // Si la respuesta es exitosa
            if (respuesta.IsSuccessStatusCode)
                this.ArticulosEnVenta = await respuesta.Content.ReadFromJsonAsync<List<ArticuloEnVenta>>() ?? [];

            return this.ArticulosEnVenta!;
        }
    }
}
