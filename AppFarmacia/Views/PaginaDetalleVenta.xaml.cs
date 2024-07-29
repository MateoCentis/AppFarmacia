using AppFarmacia.ViewModels;
namespace AppFarmacia.Views;

public partial class PaginaDetalleVenta : ContentPage
{
	private readonly PaginaDetalleVentaViewModel viewModel;
	public PaginaDetalleVenta(PaginaDetalleVentaViewModel viewModel)
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
            await this.viewModel.ObtenerDetalles();
        }
    }
}