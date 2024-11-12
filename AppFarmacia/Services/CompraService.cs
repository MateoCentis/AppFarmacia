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
                var respuesta = await httpClient.PostAsJsonAsync($"{CadenaConexion}/Compras", compra);
                if (respuesta.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Compra realizada con éxito");
                    return true;
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
