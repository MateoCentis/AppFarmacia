using AppFarmacia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using AppFarmacia.Configuration;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaConfiguracionViewModel : ObservableObject
    {
        [ObservableProperty]
        private ConfiguracionDeModelosPredictivos configuracionModelos;

        public PaginaConfiguracionViewModel()
        {
            // Cargar configuracion de Prediccion
            configuracionModelos = new ConfiguracionDeModelosPredictivos();
            configuracionModelos.CargarConfiguracion();
        }

        

        [RelayCommand]
        public async Task GuardarConfiguracionPrediccion()
        {
           await ConfiguracionModelos.Guardar();
        }
    }
}