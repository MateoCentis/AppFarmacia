using AppFarmacia.ViewModels;

namespace AppFarmacia.Views;

public partial class PaginaArticulos : ContentPage
{
	public PaginaArticulos(PaginaArticulosViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}