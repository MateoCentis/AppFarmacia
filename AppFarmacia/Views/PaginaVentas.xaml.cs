using AppFarmacia.ViewModels;
namespace AppFarmacia.Views;

public partial class PaginaVentas : ContentPage
{
	public PaginaVentas(PaginaVentasViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}