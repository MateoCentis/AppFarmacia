namespace AppFarmacia.Views;
using UraniumUI.Pages;
using AppFarmacia.ViewModels;

public partial class PaginaDetalleCompra : UraniumContentPage
{
    private readonly PaginaDetalleCompraViewModel viewModel;
    public PaginaDetalleCompra(PaginaDetalleCompraViewModel viewModel)
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
