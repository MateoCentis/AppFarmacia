namespace AppFarmacia.Views;
using AppFarmacia.ViewModels;
public partial class PaginaArticuloFinal : ContentPage
{
    private readonly PaginaArticuloFinalViewModel viewModel;
	public PaginaArticuloFinal(PaginaArticuloFinalViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (viewModel != null)
        {
            await this.viewModel.ObtenerArticulosFinales();
        }
    }
}
