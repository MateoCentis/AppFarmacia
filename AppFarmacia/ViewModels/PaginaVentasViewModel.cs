using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using AppFarmacia.Models;
using AppFarmacia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppFarmacia.ViewModels
{
    public partial class PaginaVentasViewModel : ObservableObject
    {
        public ObservableCollection<Venta> ListaVentas { get; set; } = [];


    }
}
