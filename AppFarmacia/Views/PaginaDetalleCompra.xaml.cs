namespace AppFarmacia.Views;
using UraniumUI.Pages;
using AppFarmacia.ViewModels;

public partial class PaginaDetalleCompra : UraniumContentPage
{
    public PaginaDetalleCompra()
	{
		InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PaginaDetalleCompraViewModel vm)
        {
            await vm.ObtenerDetalles();
        }
    }

}
