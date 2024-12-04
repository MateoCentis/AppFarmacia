using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using AppFarmacia.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaNotificacionesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Notificacion> listaNotificaciones = new();

        [ObservableProperty]// 1 mes antes de hoy, para no trer todas las notificaciones
        private DateTime fechaInicio = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime fechaFin = DateTime.Now;

        private readonly NotificacionesService NotificaiconesService;
        public PaginaNotificacionesViewModel()
        {
            this.NotificaiconesService = new NotificacionesService();

            Task.Run(async () => await ObtenerNotificaciones());

        }

        [RelayCommand]
        async Task ObtenerNotificaciones()
        {
            try
            {
                var notificaciones = await this.NotificaiconesService.GetNotificaciones(FechaFin, FechaInicio);
                ListaNotificaciones = new ObservableCollection<Notificacion>(notificaciones);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        private async Task MarcarComoLeida(Notificacion notificacion)
        {
            try
            {
                if (notificacion != null)
                {
                    // Llama al servicio para marcar como leída en la base de datos
                    await this.NotificaiconesService.MarcarComoLeida(notificacion.IdNotificacion);

                    // Modifica directamente el estado del objeto en la lista
                    var notificacionEnLista = ListaNotificaciones.FirstOrDefault(n => n.IdNotificacion == notificacion.IdNotificacion);
                    if (notificacionEnLista != null)
                    {
                        notificacionEnLista.Leido = true; // Actualiza localmente
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}
