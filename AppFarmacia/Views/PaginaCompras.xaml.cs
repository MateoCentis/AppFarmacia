using UraniumUI.Pages;

namespace AppFarmacia.Views;

public partial class PaginaCompras : UraniumContentPage
{
	public PaginaCompras()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		// Refrescar la lista de compras cuando se vuelve a esta página
		if (BindingContext is ViewModels.PaginaComprasViewModel vm)
		{
			await vm.ObtenerComprasCommand.ExecuteAsync(null);
		}
	}
}	 