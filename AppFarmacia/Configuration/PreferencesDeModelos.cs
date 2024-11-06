using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFarmacia.Configuration
{
    using Microsoft.Maui.Storage;

    public class PreferencesDeModelos
    {
        public int PlazoEntrega
        {
            get => Preferences.Get(nameof(PlazoEntrega), 1);
            set => Preferences.Set(nameof(PlazoEntrega), value);
        }

        public double NivelServicio
        {
            get => Preferences.Get(nameof(NivelServicio), 0.95);
            set => Preferences.Set(nameof(NivelServicio), value);
        }

        public int BTarea
        {
            get => Preferences.Get(nameof(BTarea), 700000);
            set => Preferences.Set(nameof(BTarea), value);
        }

        public int HorasMensualesDelTrabajador
        {
            get => Preferences.Get(nameof(HorasMensualesDelTrabajador), 160);
            set => Preferences.Set(nameof(HorasMensualesDelTrabajador), value);
        }

        public double TiempoPromedioParaHacerLaTarea
        {
            get => Preferences.Get(nameof(TiempoPromedioParaHacerLaTarea), 0.5);
            set => Preferences.Set(nameof(TiempoPromedioParaHacerLaTarea), value);
        }

        public int ValorDeAlquilerMensual
        {
            get => Preferences.Get(nameof(ValorDeAlquilerMensual), 2000000);
            set => Preferences.Set(nameof(ValorDeAlquilerMensual), value);
        }

        public int EspacioDeAlmacenamiento
        {
            get => Preferences.Get(nameof(EspacioDeAlmacenamiento), 28);
            set => Preferences.Set(nameof(EspacioDeAlmacenamiento), value);
        }

        public int UnidadesPorMetroCuadrado
        {
            get => Preferences.Get(nameof(UnidadesPorMetroCuadrado), 100);
            set => Preferences.Set(nameof(UnidadesPorMetroCuadrado), value);
        }



    }

}
