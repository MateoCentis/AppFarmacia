using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class StockService
    {
        private List<Stock>? historial_Stock = new List<Stock>();
        private Stock? stock = new Stock();

        private readonly HttpClient httpClient;
        private const string CadenaConexion = "http://localhost:83/api";
        public StockService() 
        {
            this.httpClient = new HttpClient();
        }
        public async Task<Stock?> GetUltimoStockPorArticulo(int id_articulo)
        {
            var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Stocks/GetUltimoStockPorArticulo/{id_articulo}");
            this.stock =  await respuesta.Content.ReadFromJsonAsync<Stock>() ?? new Stock();
            return stock;
        }
    }
}
