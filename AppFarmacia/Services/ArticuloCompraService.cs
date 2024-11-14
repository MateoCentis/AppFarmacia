using AppFarmacia.Models;
using System.Net.Http.Json;


namespace AppFarmacia.Services;

public class ArticuloCompraService
{
    private List<ArticuloEnCompra>? ArticulosEnCompra = [];
    private readonly HttpClient httpClient;

    public ArticuloCompraService()
    {
        this.httpClient = new HttpClient();
    }

    
    public async Task<List<ArticuloEnCompra>> GetArticulosEnCompraPorId(int idCompra)
    {
        // Si ya tengo ventas cargadas no llamo a la API
        if (this.ArticulosEnCompra?.Count > 0)
        {
            return ArticulosEnCompra;
        }
        // TODO: Ver si esto está implementado en la API
        var respuesta = await httpClient.GetAsync($"http://LocalHost:83/api/ArticulosEnCompra/PorCompraId/{idCompra}");


        // Si la respuesta es exitosa
        if (respuesta.IsSuccessStatusCode)
            this.ArticulosEnCompra = await respuesta.Content.ReadFromJsonAsync<List<ArticuloEnCompra>>() ?? [];

        return this.ArticulosEnCompra!;
    }

}
