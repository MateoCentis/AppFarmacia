using AppFarmacia.ViewModels;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using UraniumUI.Pages;

namespace AppFarmacia.Views;

public partial class PaginaArticulos : UraniumContentPage
{

    public PaginaArticulos(PaginaArticulosViewModel viewModel)
    {
        InitializeComponent();
    }

    //C�digo para el enter en la barra de b�squeda (es un evento no command)
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var vm = BindingContext as PaginaArticulosViewModel;
        if (vm != null && vm.FiltrarArticulosCommand.CanExecute(null))
        {
            vm.FiltrarArticulosCommand.Execute(null);
        }
    }


}