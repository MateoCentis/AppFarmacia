using System.Diagnostics;
using System.Net.Http.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services
{
    public class CompraService
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
                // Formateo a string para request (YYYY-MM-dd) 
                string fechaInicioStr = fechaInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00";  // Inicio del día
                string fechaFinStr = fechaFin.Date.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss");  // Final del día

                var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Compras?fechaInicio={fechaInicioStr}&fechaFin={fechaFinStr}");

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

        public async Task<Compra?> GetCompraPorId(int id)
        {
            try
            {
                var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Compras/{id}");

                if (respuesta.IsSuccessStatusCode)
                {
                    var compra = await respuesta.Content.ReadFromJsonAsync<Compra>();
                    return compra;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
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
                            // El motivo ya viene asignado desde el ViewModel, no lo sobrescribimos
                            // articulo.MotivoCompra ya tiene el valor correcto
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

        public async Task<bool> ConfirmarCompra(int idCompra)
        {
            try
            {
                Debug.WriteLine($"[CompraService] ConfirmarCompra - ID: {idCompra}");
                var compra = await GetCompraPorId(idCompra);
                if (compra == null)
                {
                    Debug.WriteLine($"[CompraService] No se encontró la compra con ID: {idCompra}");
                    return false;
                }

                Debug.WriteLine($"[CompraService] Compra obtenida - ID: {compra.IdCompra}, CompraConfirmada ANTES: {compra.CompraConfirmada}");
                compra.CompraConfirmada = true;
                Debug.WriteLine($"[CompraService] CompraConfirmada DESPUÉS: {compra.CompraConfirmada}");
                Debug.WriteLine($"[CompraService] Enviando PUT a: {CadenaConexion}/Compras/{idCompra}");
                
                var respuesta = await httpClient.PutAsJsonAsync($"{CadenaConexion}/Compras/{idCompra}", compra);
                
                Debug.WriteLine($"[CompraService] Respuesta recibida - StatusCode: {respuesta.StatusCode}, IsSuccessStatusCode: {respuesta.IsSuccessStatusCode}");

                if (respuesta.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[CompraService] Compra confirmada exitosamente");
                    return true;
                }
                else
                {
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[CompraService] Error en la solicitud: {respuesta.ReasonPhrase}");
                    Debug.WriteLine($"[CompraService] Contenido de la respuesta: {contenido}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CompraService] Excepción al confirmar compra: {ex.Message}");
                Debug.WriteLine($"[CompraService] Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
