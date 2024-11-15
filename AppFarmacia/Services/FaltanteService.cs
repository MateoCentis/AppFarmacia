using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    internal class FaltanteService
    {
        private readonly HttpClient httpClient;
        private const string CadenaConexion = "http://localhost:83/api"; // Cadena de conexión centralizada

        public FaltanteService()
        {
            this.httpClient = new HttpClient();
        }


        public async Task<int> ObtenerFaltanteDeArticulo(int idArticulo)
        {
            int cantidadFaltante;
            try
            {
                var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Faltantes/Articulo/{idArticulo}");

                if (respuesta.IsSuccessStatusCode)
                {
                    cantidadFaltante = await respuesta.Content.ReadFromJsonAsync<int>();
                }
                else
                {
                    throw new Exception($"Error al obtener los faltantes para el artículo {idArticulo}. Estado: {respuesta.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                cantidadFaltante = 0;
            }

            return cantidadFaltante;
        }

    }
}
