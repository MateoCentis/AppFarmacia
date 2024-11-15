using System.Diagnostics;
using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    internal class CompraService
    {
        private List<Compra>? compras = new List<Compra>();
        private readonly HttpClient httpClient;
        private const string CadenaConexion = "http://localhost:83/api";
        private ArticuloCompraService articuloCompraService = new ArticuloCompraService();

        public CompraService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<List<Compra>> GetCompras(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Compras?fechaInicio={fechaInicio}&fechaFin={fechaFin}");

                if (respuesta.IsSuccessStatusCode)
                {
                    this.compras = await respuesta.Content.ReadFromJsonAsync<List<Compra>>() ?? [];
                }

                return this.compras!;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return this.compras!;
            }

        }

        public async Task<bool> PostCompra(Compra compra)
        {
            try
            {
                // Envía la compra al servidor
                var respuesta = await httpClient.PostAsJsonAsync($"{CadenaConexion}/Compras", compra);

                if (respuesta.IsSuccessStatusCode)
                {
                    // Lee y deserializa la respuesta como un objeto CompraDTO
                    var compraCreada = await respuesta.Content.ReadFromJsonAsync<Compra>();
                    if (compraCreada != null)
                    {
                        // Asigna el IdCompra recién creado a cada ArticuloEnCompra
                        foreach (var articulo in compra.ArticuloEnCompra)
                        {
                            articulo.IdCompra = compraCreada.IdCompra;
                            articulo.MotivoCompra = "No lo se";
                        }

                        // Llama al método PostArticulosEnCompra con la lista de artículos actualizada
                        bool resultado = await articuloCompraService.PostArticulosEnCompra(compra.ArticuloEnCompra);
                        if (!resultado)
                        {
                            Debug.WriteLine("Error al agregar los artículos en compra");
                            return false;
                        }

                        Debug.WriteLine("Compra realizada con éxito");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine("Error al procesar la respuesta de la API");
                        return false;
                    }
                }
                else
                {
                    Debug.WriteLine($"Error en la solicitud: {respuesta.ReasonPhrase}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
