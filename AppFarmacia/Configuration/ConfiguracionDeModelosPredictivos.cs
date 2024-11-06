
using System.Diagnostics;
using System.Globalization;
using AppFarmacia.Configuration;
using System.Configuration;

namespace AppFarmacia.Configuration
{
    public class ConfiguracionDeModelosPredictivos
    {
        public int PlazoEntrega { get; set; }
        public double NivelServicio { get; set; }
        public int BTarea { get; set; }
        public int HorasMensualesDelTrabajador { get; set; }
        public double TiempoPromedioParaHacerLaTarea { get; set; }
        public int ValorDeAlquilerMensual { get; set; }
        public int EspacioDeAlmacenamiento { get; set; }
        public int UnidadesPorMetroCuadrado { get; set; }
        public PreferencesDeModelos ConfigPrediccion = new PreferencesDeModelos();

        public void CargarConfiguracion()
        {
            try
            {
                // Cargar valores predeterminados de ConfigPrediccion
                PlazoEntrega = ConfigPrediccion.PlazoEntrega;
                NivelServicio = ConfigPrediccion.NivelServicio;
                BTarea = ConfigPrediccion.BTarea;
                HorasMensualesDelTrabajador = ConfigPrediccion.HorasMensualesDelTrabajador;
                TiempoPromedioParaHacerLaTarea = ConfigPrediccion.TiempoPromedioParaHacerLaTarea;
                ValorDeAlquilerMensual = ConfigPrediccion.ValorDeAlquilerMensual;
                EspacioDeAlmacenamiento = ConfigPrediccion.EspacioDeAlmacenamiento;
                UnidadesPorMetroCuadrado = ConfigPrediccion.UnidadesPorMetroCuadrado;
            }
            catch
            {
                Debug.WriteLine("Error al cargar la configuración.");
            }
            

           
        }

        public async Task Guardar()
        {
            try
            {
                // Guardar los valores en ConfigPrediccion
                ConfigPrediccion.PlazoEntrega = PlazoEntrega;
                ConfigPrediccion.NivelServicio = NivelServicio;
                ConfigPrediccion.BTarea = BTarea;
                ConfigPrediccion.HorasMensualesDelTrabajador = HorasMensualesDelTrabajador;
                ConfigPrediccion.TiempoPromedioParaHacerLaTarea = TiempoPromedioParaHacerLaTarea;
                ConfigPrediccion.ValorDeAlquilerMensual = ValorDeAlquilerMensual;
                ConfigPrediccion.EspacioDeAlmacenamiento = EspacioDeAlmacenamiento;
                ConfigPrediccion.UnidadesPorMetroCuadrado = UnidadesPorMetroCuadrado;
                // Mensaje que se guardo bien
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Configuración guardada correctamente.", "OK");
                }
            }
            catch
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Error al guardar la configuración.", "OK");
                }
            }
            
        }
    }
}

