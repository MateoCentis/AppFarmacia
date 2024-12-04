using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AppFarmacia.Models;

namespace AppFarmacia.Services;

public class NotificacionesService
{
    private List<Notificacion>? notificaciones = new List<Notificacion>();
    private readonly HttpClient httpClient;
    private const string CadenaConexion = "http://localhost:83/api"; // Cadena de conexión centralizada
    
    public NotificacionesService()
    {
        this.httpClient = new HttpClient();
    }

    public async Task<List<Notificacion>> GetNotificaciones(DateTime FechaFin, DateTime FechaInicio)
    {
        try
        {
            // Formateo a string para request (YYYY-MM-dd) 
            string fechaInicioStr = FechaInicio.Date.ToString("yyyy-MM-dd") + " 00:00:00";  // Inicio del día
            string fechaFinStr = FechaFin.Date.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss");  // Final del día

            var url = $"{CadenaConexion}/Notificacion?fechaInicio={fechaInicioStr}&fechaFin={fechaFinStr}";

            var respuesta = await httpClient.GetAsync(url);

            if (respuesta.IsSuccessStatusCode)
            {
                this.notificaciones = await respuesta.Content.ReadFromJsonAsync<List<Notificacion>>();
                return this.notificaciones ?? [];

            }
            else
            {
                // Devoluciones de error
                switch (respuesta.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception("No se encontró el recurso solicitado.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Solicitud inválida. Verifique los parámetros de fecha.");
                    default:
                        throw new Exception($"Error en la solicitud: {respuesta.StatusCode}");
                }
            }
        }
        catch (JsonException ex)
        {
            throw new Exception($"Error en la solicitud: {ex.Message}");
        }
    }

    // Método para marcar como leída una notificación
    public async Task<bool> MarcarComoLeida(int idNotificacion)
    {
        try
        {
            var url = $"{CadenaConexion}/Notificacion/{idNotificacion}";

            var respuesta = await httpClient.PutAsync(url, null);

            if (respuesta.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                switch (respuesta.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new Exception("No se encontró el recurso solicitado.");
                    case HttpStatusCode.BadRequest:
                        throw new Exception("Solicitud inválida. Verifique los parámetros de fecha.");
                    default:
                        throw new Exception($"Error en la solicitud: {respuesta.StatusCode}");
                }
            }
        }
        catch (JsonException ex)
        {
            throw new Exception($"Error en la solicitud: {ex.Message}");
        }
    }


}
