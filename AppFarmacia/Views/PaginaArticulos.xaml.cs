using AppFarmacia.ViewModels;

namespace AppFarmacia.Views;

public partial class PaginaArticulos : ContentPage
{
    public PaginaArticulos(PaginaArticulosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    //C�digo para el enter en la barra de b�squeda (es un evento no command)
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var vm = BindingContext as PaginaArticulosViewModel;
        if (vm != null && vm.FiltrarCommand.CanExecute(null))
        {
            vm.FiltrarCommand.Execute(null);
        }
    }

}