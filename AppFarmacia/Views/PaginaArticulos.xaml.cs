using AppFarmacia.ViewModels;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AppFarmacia.Views;

public partial class PaginaArticulos : ContentPage
{
    private PaginaSpinnerPopup popupCarga;

    public PaginaArticulos(PaginaArticulosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
    }

    //Código para el enter en la barra de búsqueda (es un evento no command)
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var vm = BindingContext as PaginaArticulosViewModel;
        if (vm != null && vm.FiltrarCommand.CanExecute(null))
        {
            vm.FiltrarCommand.Execute(null);
        }
    }


}