using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task<List<Notificacion>> GetNotificaciones(int size = 0)
    {
        var respuesta = await httpClient.GetAsync($"{CadenaConexion}/Notificaciones?size={size}");

        if (respuesta.IsSuccessStatusCode)
        {
            this.notificaciones = await respuesta.Content.ReadFromJsonAsync<List<Notificacion>>() ?? [];
        }

        return this.notificaciones!;
    }


}
