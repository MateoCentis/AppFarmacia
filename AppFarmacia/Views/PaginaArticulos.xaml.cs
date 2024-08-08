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
        viewModel = (PaginaArticulosViewModel)BindingContext;
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
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

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PaginaArticulosViewModel.EstaCargando))
        {
            var viewModel = (PaginaArticulosViewModel)BindingContext;
            if (viewModel.EstaCargando)
            {
                // Mostrar el popup
                popupCarga = new PaginaSpinnerPopup();
                this.ShowPopup(popupCarga);
            }
            else
            {
                // Aseg�rate de que el popup no es nulo antes de cerrarlo
                if (popupCarga != null)
                {
                    popupCarga.Close();
                    popupCarga = null; // Resetear el popup a null despu�s de cerrarlo
                }
            }
        }
    }


}