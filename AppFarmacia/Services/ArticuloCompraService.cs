using AppFarmacia.Models;
using System.Diagnostics;
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

    public async Task<bool> PostArticulosEnCompra(ICollection<ArticuloEnCompra> articulosEnCompra)
    {
        try
        {
            foreach (var articuloEnCompra in articulosEnCompra)
            {
                var respuesta = await httpClient.PostAsJsonAsync("http://LocalHost:83/api/ArticulosEnCompra", articuloEnCompra);
                if (!respuesta.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Error en la solicitud: {respuesta.ReasonPhrase}");
                    return false;
                }
            }
            Debug.WriteLine("Artículos en compra agregados con éxito");
            return true;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }

        return true;
    }

}
