using AppFarmacia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AppFarmacia.ViewModels
{
    [QueryProperty(nameof(ArticuloMostrar), "articuloMostrar")]
    public partial class PaginaArticuloInformacionViewModel : ObservableObject
    {
        [ObservableProperty]
        private ArticuloMostrar? articuloMostrar;

        [ObservableProperty]
        private string nombreArticulo;

        [ObservableProperty]
        private List<Precio> preciosArticulo = [];

        [ObservableProperty]
        private List<Vencimiento> vencimientosArticulo = [];

        public PaginaArticuloInformacionViewModel()
        {
        }

        partial void OnArticuloMostrarChanged(ArticuloMostrar? value)
        {
            if (value != null)
            {
                NombreArticulo = value.Nombre;
                VencimientosArticulo = value.Vencimientos.ToList();
                PreciosArticulo = value.Precios.ToList();
            }
        }

        // TODO: Esta función debería devolver la demanda mensual del artículo dado un año determinado
        // No la implemento porque me gustaría charlarlo con Lucas (no me acuerdo ni estoy seguro como chequeamos demanda)
        private List<Decimal> ObtenerDemandaMensual(int year)
        {
            throw new NotImplementedException();
        }
        //Esto se puede usar para otras cosas
        //public async Task ObtenerArticulosFinales()
        //{
        //    try
        //    {
        //        if (ArticuloAMostrar != null)
        //        {
        //            var articulosFinales = ArticuloAMostrar.ArticulosFinales ?? [];
        //            foreach (var articuloFinal in articulosFinales)
        //            {
        //                this.ListaArticulosFinales.Add(articuloFinal);
        //            }
        //            NombreArticulo = ArticuloAMostrar.Nombre;
        //        }
        //        else
        //            await Shell.Current.DisplayAlert("Error!", "Artículo no encontrado", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        //    }
        //}

    }
}
