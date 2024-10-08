using AppFarmacia.ViewModels;
namespace AppFarmacia.Views;

public partial class PaginaVentas : ContentPage
{
	private readonly PaginaVentasViewModel viewModel;
	public PaginaVentas(PaginaVentasViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (viewModel != null)
        {
            await this.viewModel.ObtenerVentas();
        }
    }
}